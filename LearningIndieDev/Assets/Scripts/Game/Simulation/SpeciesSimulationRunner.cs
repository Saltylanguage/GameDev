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
        SpeciesExperimentalOptions experimentalOptions;
        Grid<SpeciesCell> previousCells;

        public SpeciesSimulationRunner(
            SimulationRunState run,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules,
            float stepSeconds,
            int maxPopulation = 0,
            SpeciesCombatResolutionMode combatResolutionMode = SpeciesCombatResolutionMode.LegacyFixedDamage,
            SpeciesAttackOpportunityMode attackOpportunityMode = SpeciesAttackOpportunityMode.Natural,
            SpeciesExperimentalOptions experimentalOptions = null,
            IEnumerable<SpeciesUpgradeSnapshot> upgradeLoadout = null)
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
            this.experimentalOptions = experimentalOptions ?? SpeciesExperimentalOptions.None;
            Run.SetUpgradeLoadout(upgradeLoadout);
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
            SpeciesCombatResolutionMode combatResolutionMode = SpeciesCombatResolutionMode.LegacyFixedDamage,
            SpeciesAttackOpportunityMode attackOpportunityMode = SpeciesAttackOpportunityMode.Natural,
            SpeciesExperimentalOptions experimentalOptions = null,
            IEnumerable<SpeciesUpgradeSnapshot> upgradeLoadout = null)
        {
            Run = run ?? throw new ArgumentNullException(nameof(run));
            this.simulationData = simulationData ?? throw new ArgumentNullException(nameof(simulationData));
            rules = simulationData.SpeciesRules;
            stepSeconds = simulationData.StepInterval;
            maxPopulation = simulationData.MaxPopulation;
            this.combatResolutionMode = combatResolutionMode;
            this.attackOpportunityMode = attackOpportunityMode;
            this.experimentalOptions = experimentalOptions ?? SpeciesExperimentalOptions.None;
            if (Run.TargetTicks == 0)
            {
                Run.SetTargetTicks(simulationData.RunTicks);
            }
            Run.SetUpgradeLoadout(upgradeLoadout);
            Run.SetRulesetFingerprint(simulationData.Fingerprint);
        }

        public SimulationRunState Run { get; }
        public float StepSeconds => stepSeconds;

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
            IEnumerable<SpeciesUpgradeSnapshot> nextUpgradeLoadout)
        {
            if (Run.Status != SimulationRunStatus.AwaitingDecision)
            {
                return false;
            }

            if (nextRules == null)
            {
                throw new ArgumentNullException(nameof(nextRules));
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
            Run.SetUpgradeLoadout(nextUpgradeLoadout);
            return true;
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
                    previousSource: previousCells)
                : SpeciesSimulation.Step(
                    Run.Cells,
                    simulationData,
                    Run.Seed + Run.Tick,
                    Run.Metrics,
                    combatResolutionMode,
                    attackOpportunityMode,
                    experimentalOptions,
                    previousSource: previousCells);
            previousCells = Run.Cells;
            Run.Advance(next, stepSeconds);
            return true;
        }
    }
}
