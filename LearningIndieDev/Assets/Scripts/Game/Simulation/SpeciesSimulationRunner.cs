using System;
using System.Collections.Generic;

namespace SaltyGame
{
    public sealed class SpeciesSimulationRunner
    {
        readonly IReadOnlyDictionary<SpeciesArchetype, SpeciesRules> rules;
        readonly float stepSeconds;

        public SpeciesSimulationRunner(
            SimulationRunState run,
            IReadOnlyDictionary<SpeciesArchetype, SpeciesRules> rules,
            float stepSeconds)
        {
            Run = run ?? throw new ArgumentNullException(nameof(run));
            this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
            if (stepSeconds <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(stepSeconds), stepSeconds, "Simulation step must be greater than zero.");
            }

            this.stepSeconds = stepSeconds;
        }

        public SimulationRunState Run { get; }

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
        }

        public bool AdvanceOneTick()
        {
            if (Run.Status == SimulationRunStatus.Ready)
            {
                Start();
            }

            if (Run.Status == SimulationRunStatus.Paused || Run.Status == SimulationRunStatus.Complete)
            {
                return false;
            }

            var next = SpeciesSimulation.Step(Run.Cells, rules, Run.Seed + Run.Tick);
            Run.Advance(next, stepSeconds);
            return true;
        }
    }
}
