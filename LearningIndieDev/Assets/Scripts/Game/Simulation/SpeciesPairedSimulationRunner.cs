using System;
using System.Collections.Generic;

namespace SaltyGame
{
    /// <summary>
    /// Advances two diagnostic worlds together and gives both worlds the same
    /// valid opportunity identity before normal combat resolution.
    /// </summary>
    public sealed class SpeciesPairedSimulationRunner
    {
        readonly CellularSimData baselineData;
        readonly CellularSimData blockPlusTwoData;
        readonly SpeciesExperimentalOptions experimentalOptions;

        public SpeciesPairedSimulationRunner(
            SimulationRunState baselineRun,
            CellularSimData baselineData,
            SimulationRunState blockPlusTwoRun,
            CellularSimData blockPlusTwoData,
            SpeciesCombatResolutionMode combatResolutionMode,
            SpeciesExperimentalOptions experimentalOptions = null)
        {
            BaselineRun = baselineRun ?? throw new ArgumentNullException(nameof(baselineRun));
            this.baselineData = baselineData ?? throw new ArgumentNullException(nameof(baselineData));
            BlockPlusTwoRun = blockPlusTwoRun ?? throw new ArgumentNullException(nameof(blockPlusTwoRun));
            this.blockPlusTwoData = blockPlusTwoData ?? throw new ArgumentNullException(nameof(blockPlusTwoData));
            this.experimentalOptions = experimentalOptions ?? SpeciesExperimentalOptions.None;
            if (baselineData.StepInterval != blockPlusTwoData.StepInterval)
            {
                throw new ArgumentException("Paired runs must use the same step interval.", nameof(blockPlusTwoData));
            }

            CombatResolutionMode = combatResolutionMode;
            OpportunityControl = new SpeciesPairedOpportunityControl();
        }

        public SimulationRunState BaselineRun { get; }
        public SimulationRunState BlockPlusTwoRun { get; }
        public SpeciesCombatResolutionMode CombatResolutionMode { get; }
        public SpeciesPairedOpportunityControl OpportunityControl { get; }

        public bool AdvanceOneTick()
        {
            if (BaselineRun.Status == SimulationRunStatus.Ready)
            {
                BaselineRun.Start();
            }

            if (BlockPlusTwoRun.Status == SimulationRunStatus.Ready)
            {
                BlockPlusTwoRun.Start();
            }

            if (BaselineRun.Status == SimulationRunStatus.Paused
                || BaselineRun.Status == SimulationRunStatus.Complete
                || BlockPlusTwoRun.Status == SimulationRunStatus.Paused
                || BlockPlusTwoRun.Status == SimulationRunStatus.Complete)
            {
                return false;
            }

            var baselineTick = BaselineRun.Tick + 1;
            var blockPlusTwoTick = BlockPlusTwoRun.Tick + 1;
            if (baselineTick != blockPlusTwoTick)
            {
                throw new InvalidOperationException("Paired runs must advance on the same tick.");
            }

            BaselineRun.Metrics.BeginTick(baselineTick);
            BlockPlusTwoRun.Metrics.BeginTick(blockPlusTwoTick);
            var seed = BaselineRun.Seed + BaselineRun.Tick;
            var observations = new List<SpeciesPairedOpportunityObservation>();
            var result = SpeciesSimulation.StepPaired(
                BaselineRun.Cells,
                baselineData.SpeciesRules,
                BlockPlusTwoRun.Cells,
                blockPlusTwoData.SpeciesRules,
                seed,
                baselineData.MaxPopulation,
                blockPlusTwoData.MaxPopulation,
                baselineData.TerrainDefinitions,
                blockPlusTwoData.TerrainDefinitions,
                baselineData.AlphaOffspringRules,
                blockPlusTwoData.AlphaOffspringRules,
                BaselineRun.Metrics,
                BlockPlusTwoRun.Metrics,
                CombatResolutionMode,
                out var baselineNext,
                out var blockPlusTwoNext,
                out var pairedOpportunityId,
                opportunityObservations: observations,
                tick: baselineTick,
                experimentalOptions: experimentalOptions);
            OpportunityControl.Add(result, pairedOpportunityId, observations);
            BaselineRun.Advance(baselineNext, baselineData.StepInterval);
            BlockPlusTwoRun.Advance(blockPlusTwoNext, blockPlusTwoData.StepInterval);
            return true;
        }
    }
}
