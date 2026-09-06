using System;
using System.Collections.Generic;

namespace SaltyGame
{
    /// <summary>
    /// Owns the lifecycle and fixed-step advancement of one simulation runner.
    /// It deliberately contains no Unity or presentation dependencies.
    /// </summary>
    public sealed class SimulationManager
    {
        SpeciesSimulationRunner runner;
        float accumulatedSeconds;
        bool completionRaised;
        bool boundaryRaised;

        public event Action<SimulationRunState> RunCompleted;
        public event Action<SimulationRunState> PhaseBoundaryReached;

        internal SpeciesSimulationRunner Runner => runner;
        public SimulationRunState Run => runner?.Run;
        public float StepSeconds => runner?.StepSeconds ?? 0f;

        public void SetRunner(SpeciesSimulationRunner nextRunner)
        {
            runner = nextRunner ?? throw new ArgumentNullException(nameof(nextRunner));
            accumulatedSeconds = 0f;
            completionRaised = false;
            boundaryRaised = false;
        }

        public bool Start()
        {
            if (runner == null || runner.Run.Status != SimulationRunStatus.Ready)
            {
                return false;
            }

            runner.Start();
            return true;
        }

        public bool Pause()
        {
            if (runner == null || runner.Run.Status != SimulationRunStatus.Running)
            {
                return false;
            }

            runner.Pause();
            return true;
        }

        public bool Resume()
        {
            if (runner == null || runner.Run.Status != SimulationRunStatus.Paused)
            {
                return false;
            }

            runner.Resume();
            return true;
        }

        public bool Restart()
        {
            if (runner == null
                || (runner.Run.Status != SimulationRunStatus.Running
                    && runner.Run.Status != SimulationRunStatus.Paused
                    && runner.Run.Status != SimulationRunStatus.AwaitingDecision))
            {
                return false;
            }

            runner.Restart();
            runner.Start();
            accumulatedSeconds = 0f;
            completionRaised = false;
            boundaryRaised = false;
            return true;
        }

        public bool Stop()
        {
            if (runner == null
                || (runner.Run.Status != SimulationRunStatus.Running
                    && runner.Run.Status != SimulationRunStatus.Paused
                    && runner.Run.Status != SimulationRunStatus.AwaitingDecision))
            {
                return false;
            }

            runner = null;
            accumulatedSeconds = 0f;
            completionRaised = false;
            boundaryRaised = false;
            return true;
        }

        public bool ContinueWithoutUpgrade()
        {
            if (runner == null || !runner.Run.ContinueWithoutUpgrade())
            {
                return false;
            }

            accumulatedSeconds = 0f;
            boundaryRaised = false;
            return true;
        }

        public bool ContinueWithBoundaryState(
            IReadOnlyDictionary<SpeciesId, SpeciesRules> nextRules,
            SpeciesExperimentalOptions nextExperimentalOptions,
            IEnumerable<SpeciesUpgradeSnapshot> nextUpgradeLoadout)
        {
            if (runner == null
                || runner.Run.Status != SimulationRunStatus.AwaitingDecision
                || !runner.InstallBoundaryState(nextRules, nextExperimentalOptions, nextUpgradeLoadout)
                || !runner.Run.ContinueWithoutUpgrade())
            {
                return false;
            }

            accumulatedSeconds = 0f;
            boundaryRaised = false;
            return true;
        }

        public bool End()
        {
            if (runner == null || !runner.Run.End())
            {
                return false;
            }

            accumulatedSeconds = 0f;
            RaiseCompletionIfNeeded();
            return true;
        }

        public void Advance(float deltaSeconds)
        {
            if (deltaSeconds < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(deltaSeconds), deltaSeconds, "Delta time cannot be negative.");
            }

            if (runner == null || runner.Run.Status != SimulationRunStatus.Running)
            {
                return;
            }

            accumulatedSeconds += deltaSeconds;
            while (accumulatedSeconds >= runner.StepSeconds
                   && runner.Run.Status == SimulationRunStatus.Running)
            {
                accumulatedSeconds -= runner.StepSeconds;
                runner.AdvanceOneTick();
            }

            if (runner.Run.Status == SimulationRunStatus.AwaitingDecision)
            {
                accumulatedSeconds = 0f;
                if (!boundaryRaised)
                {
                    boundaryRaised = true;
                    PhaseBoundaryReached?.Invoke(runner.Run);
                }
            }

            RaiseCompletionIfNeeded();
        }

        void RaiseCompletionIfNeeded()
        {
            if (runner.Run.Status != SimulationRunStatus.Complete || completionRaised)
            {
                return;
            }

            completionRaised = true;
            RunCompleted?.Invoke(runner.Run);
        }
    }
}
