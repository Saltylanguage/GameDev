using System;
using System.Collections.Generic;

namespace SaltyGame
{
    public sealed class SpeciesSimulationRunner
    {
        CellularSimData simulationData;
        IReadOnlyDictionary<SpeciesId, SpeciesRules> rules;
        readonly float stepSeconds;
        readonly int maxPopulation;
        readonly SpeciesCombatResolutionMode combatResolutionMode;
        readonly SpeciesAttackOpportunityMode attackOpportunityMode;
        readonly bool stopOnPlayerExtinction;
        SpeciesExperimentalOptions experimentalOptions;
        Grid<SpeciesCell> previousCells;

        public SpeciesSimulationRunner(
            SimulationRunState run,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules,
            float stepSeconds,
            int maxPopulation = 0,
            SpeciesCombatResolutionMode combatResolutionMode = SpeciesCombatResolutionMode.OpposedRoll,
            SpeciesAttackOpportunityMode attackOpportunityMode = SpeciesAttackOpportunityMode.Natural,
            SpeciesExperimentalOptions experimentalOptions = null,
            IEnumerable<SpeciesUpgradeSnapshot> upgradeLoadout = null,
            bool stopOnPlayerExtinction = true)
        {
            Run = run ?? throw new ArgumentNullException(nameof(run));
            this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
            if (stepSeconds <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(stepSeconds), stepSeconds, "Simulation step must be greater than zero.");
            }

            this.stepSeconds = stepSeconds;
            if (maxPopulation < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxPopulation), maxPopulation, "Maximum population cannot be negative.");
            }

            this.maxPopulation = maxPopulation;
            this.combatResolutionMode = combatResolutionMode;
            this.attackOpportunityMode = attackOpportunityMode;
            this.stopOnPlayerExtinction = stopOnPlayerExtinction;
            this.experimentalOptions = experimentalOptions ?? SpeciesExperimentalOptions.None;
            Run.SetUpgradeLoadout(upgradeLoadout, this.experimentalOptions.CoupledSpeciesResponsesEnabled);
        }

        [Obsolete("Use the SpeciesId overload instead.")]
        public SpeciesSimulationRunner(
            SimulationRunState run,
            IReadOnlyDictionary<SpeciesArchetype, SpeciesRules> rules,
            float stepSeconds,
            int maxPopulation = 0)
            : this(run, SpeciesIdConversions.FromLegacy(rules), stepSeconds, maxPopulation)
        {
        }

        public SpeciesSimulationRunner(
            SimulationRunState run,
            CellularSimData simulationData,
            SpeciesCombatResolutionMode combatResolutionMode = SpeciesCombatResolutionMode.OpposedRoll,
            SpeciesAttackOpportunityMode attackOpportunityMode = SpeciesAttackOpportunityMode.Natural,
            SpeciesExperimentalOptions experimentalOptions = null,
            IEnumerable<SpeciesUpgradeSnapshot> upgradeLoadout = null,
            bool stopOnPlayerExtinction = true)
        {
            Run = run ?? throw new ArgumentNullException(nameof(run));
            this.simulationData = simulationData ?? throw new ArgumentNullException(nameof(simulationData));
            rules = simulationData.SpeciesRules;
            stepSeconds = simulationData.StepInterval;
            maxPopulation = simulationData.MaxPopulation;
            this.combatResolutionMode = combatResolutionMode;
            this.attackOpportunityMode = attackOpportunityMode;
            this.stopOnPlayerExtinction = stopOnPlayerExtinction;
            this.experimentalOptions = experimentalOptions ?? SpeciesExperimentalOptions.None;
            if (Run.TargetTicks == 0)
            {
                Run.SetTargetTicks(simulationData.RunTicks);
            }
            Run.SetUpgradeLoadout(upgradeLoadout, this.experimentalOptions.CoupledSpeciesResponsesEnabled);
            Run.SetRulesetFingerprint(simulationData.Fingerprint);
        }

        public SimulationRunState Run { get; }
        public float StepSeconds => stepSeconds;

        public SimulationRunCheckpoint CreateCheckpoint()
        {
            return Run.CreateCheckpoint(previousCells);
        }

        public static SpeciesSimulationRunner RestoreCheckpoint(
            SimulationRunCheckpoint checkpoint,
            CellularSimData simulationData,
            SpeciesCombatResolutionMode combatResolutionMode = SpeciesCombatResolutionMode.OpposedRoll,
            SpeciesAttackOpportunityMode attackOpportunityMode = SpeciesAttackOpportunityMode.Natural,
            SpeciesExperimentalOptions experimentalOptions = null)
        {
            if (checkpoint == null)
            {
                throw new ArgumentNullException(nameof(checkpoint));
            }

            if (simulationData == null)
            {
                throw new ArgumentNullException(nameof(simulationData));
            }

            var runner = new SpeciesSimulationRunner(
                checkpoint.Restore(),
                simulationData,
                combatResolutionMode,
                attackOpportunityMode,
                experimentalOptions,
                checkpoint.UpgradeLoadout);
            runner.previousCells = checkpoint.CopyPreviousCells();
            return runner;
        }

        public void Start()
        {
            Run.Start();
        }

        public void Pause()
        {
            Run.Pause();
        }

        public void Resume()
        {
            Run.Resume();
        }

        public void Restart()
        {
            Run.Restart();
            previousCells = null;
        }

        public bool InstallBoundaryState(
            IReadOnlyDictionary<SpeciesId, SpeciesRules> nextRules,
            SpeciesExperimentalOptions nextExperimentalOptions,
            IEnumerable<SpeciesUpgradeSnapshot> nextUpgradeLoadout,
            SpeciesUpgradeSnapshot selectedUpgrade = null)
        {
            if (Run.Status != SimulationRunStatus.AwaitingDecision)
            {
                return false;
            }

            if (nextRules == null)
            {
                throw new ArgumentNullException(nameof(nextRules));
            }

            if (selectedUpgrade != null && selectedUpgrade.PopulationToAdd > 0
                && (!nextRules.TryGetValue(selectedUpgrade.TargetSpecies, out var targetRules)
                    || !TryAddBoundaryPopulation(
                        selectedUpgrade.TargetSpecies, targetRules, selectedUpgrade.PopulationToAdd)))
            {
                return false;
            }

            if (simulationData != null)
            {
                var updatedData = simulationData;
                foreach (var entry in nextRules)
                {
                    if (!updatedData.SpeciesRules.TryGetValue(entry.Key, out var currentRules)
                        || !ReferenceEquals(currentRules, entry.Value))
                    {
                        updatedData = updatedData.WithSpeciesRules(entry.Key, entry.Value);
                    }
                }

                simulationData = updatedData;
                rules = updatedData.SpeciesRules;
                Run.SetRulesetFingerprint(updatedData.Fingerprint);
                if (Run.TargetTicks == 0)
                {
                    Run.SetTargetTicks(updatedData.RunTicks);
                }
            }
            else
            {
                rules = nextRules;
            }

            experimentalOptions = nextExperimentalOptions ?? SpeciesExperimentalOptions.None;
            Run.SetUpgradeLoadout(nextUpgradeLoadout, experimentalOptions.CoupledSpeciesResponsesEnabled);
            return true;
        }

        public bool TryAddBoundaryPopulation(SpeciesId species, int amount)
        {
            return TryAddBoundaryPopulation(species, amount, out _);
        }

        public bool TryAddBoundaryPopulation(SpeciesId species, int amount, out int added, bool ignorePopulationPool = false)
        {
            added = 0;
            if (Run.Status != SimulationRunStatus.AwaitingDecision
                || !rules.TryGetValue(species, out var targetRules))
            {
                return false;
            }

            return TryAddBoundaryPopulation(species, targetRules, amount, out added, ignorePopulationPool);
        }

        bool TryAddBoundaryPopulation(SpeciesId species, SpeciesRules targetRules, int amount)
        {
            return TryAddBoundaryPopulation(species, targetRules, amount, out _);
        }

        bool TryAddBoundaryPopulation(SpeciesId species, SpeciesRules targetRules, int amount, out int added, bool ignorePopulationPool = false)
        {
            added = 0;
            if (Run.Status != SimulationRunStatus.AwaitingDecision)
            {
                return false;
            }
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
            if (species == Run.PlayerSpeciesId && !ignorePopulationPool)
                amount = Math.Min(amount, Run.PopulationPoolSpace);
            if (amount == 0) return false;

            var currentCells = Run.Cells;
            var initialCells = Run.CopyInitialCells();
            if (!SpeciesSimulation.CanAddBoundaryPopulation(currentCells, amount, maxPopulation)
                || !SpeciesSimulation.CanAddBoundaryPopulation(initialCells, amount, maxPopulation)
                || !SpeciesSimulation.TryAddBoundaryPopulation(
                    currentCells, species, targetRules, amount, maxPopulation,
                    CreateBoundaryPopulationSeed(isRestartGrid: false), out var nextCells)
                || !SpeciesSimulation.TryAddBoundaryPopulation(
                    initialCells, species, targetRules, amount, maxPopulation,
                    CreateBoundaryPopulationSeed(isRestartGrid: true), out var nextInitialCells)
                || !Run.InstallBoundaryPopulation(nextCells, nextInitialCells, species, amount))
            {
                return false;
            }

            previousCells = Run.Cells;
            added = amount;
            return true;
        }

        TerrainDefinition BoundaryPlantTerrain => simulationData != null
            && simulationData.TerrainDefinitions.TryGetValue(TerrainIds.Grass, out var authoredGrass)
                ? authoredGrass : TerrainDefaults.Grass;

        public bool CanChangeBoundaryPopulation(SpeciesId species, int requested, out int actual, out string reason)
        {
            actual = 0;
            reason = "Species is unavailable in this scenario.";
            return rules.TryGetValue(species, out var targetRules)
                && PopulationIntervention.TryPlan(Run, species, targetRules, requested, maxPopulation, BoundaryPlantTerrain,
                    out actual, out _, out reason);
        }

        public bool TryChangeBoundaryPopulation(SpeciesId species, int requested, string source, out int actual)
        {
            actual = 0;
            if (string.IsNullOrWhiteSpace(source) || !rules.TryGetValue(species, out var targetRules)
                || !PopulationIntervention.TryPlan(Run, species, targetRules, requested, maxPopulation, BoundaryPlantTerrain,
                    out var amount, out var candidates, out _)) return false;
            var seed = unchecked(CreateBoundaryPopulationSeed(false) * 31 + Run.PopulationInterventions.Count);
            var next = PopulationIntervention.Apply(Run.Cells, species, targetRules, amount, candidates, seed, BoundaryPlantTerrain);
            Run.InstallPopulationIntervention(next,
                new PopulationIntervention(species, amount, source, Run.Tick, Run.PhaseIndex, seed));
            previousCells = Run.Cells;
            actual = amount;
            return true;
        }

        int CreateBoundaryPopulationSeed(bool isRestartGrid)
        {
            unchecked
            {
                var value = Run.Seed;
                value = value * 31 + Run.Tick;
                value = value * 31 + Run.PhaseIndex;
                value = value * 31 + Run.UpgradeLoadout.Count;
                return value ^ (isRestartGrid ? 0x6D2B79F5 : 0x1B873593);
            }
        }

        public bool AdvanceOneTick()
        {
            if (Run.Status == SimulationRunStatus.Ready)
            {
                Start();
            }

            if (Run.Status == SimulationRunStatus.Paused
                || Run.Status == SimulationRunStatus.AwaitingDecision
                || Run.Status == SimulationRunStatus.Complete)
            {
                return false;
            }

            Run.Metrics.BeginTick(Run.Tick + 1);
            var next = simulationData == null
                ? SpeciesSimulation.Step(
                    Run.Cells,
                    rules,
                    Run.Seed + Run.Tick,
                    maxPopulation,
                    metrics: Run.Metrics,
                    combatResolutionMode: combatResolutionMode,
                    attackOpportunityMode: attackOpportunityMode,
                    experimentalOptions: experimentalOptions,
                    previousSource: previousCells,
                    stepIntervalSeconds: stepSeconds,
                    genomeSnapshot: Run.ActiveGenomeSnapshot)
                : SpeciesSimulation.Step(
                    Run.Cells,
                    simulationData,
                    Run.Seed + Run.Tick,
                    Run.Metrics,
                    combatResolutionMode,
                    attackOpportunityMode,
                    experimentalOptions,
                    previousSource: previousCells,
                    genomeSnapshot: Run.ActiveGenomeSnapshot);
            previousCells = Run.Cells;
            Run.Advance(next, stepSeconds);
            if (stopOnPlayerExtinction && Run.SupportsContinuation
                && Run.Status != SimulationRunStatus.Complete
                && Run.PopulationHistory[Run.PopulationHistory.Count - 1].GetCount(Run.PlayerSpeciesId) == 0)
            {
                Run.End();
            }

            return true;
        }
    }
}
