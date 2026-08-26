using System;

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

        public event Action<SimulationRunState> RunCompleted;

        internal SpeciesSimulationRunner Runner => runner;
        public SimulationRunState Run => runner?.Run;
        public float StepSeconds => runner?.StepSeconds ?? 0f;

        public void SetRunner(SpeciesSimulationRunner nextRunner)
        {
            runner = nextRunner ?? throw new ArgumentNullException(nameof(nextRunner));
            accumulatedSeconds = 0f;
            completionRaised = false;
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
                    && runner.Run.Status != SimulationRunStatus.Paused))
            {
                return false;
            }

            runner.Restart();
            runner.Start();
            accumulatedSeconds = 0f;
            completionRaised = false;
            return true;
        }

        public bool Stop()
        {
            if (runner == null
                || (runner.Run.Status != SimulationRunStatus.Running
                    && runner.Run.Status != SimulationRunStatus.Paused))
            {
                return false;
            }

            runner = null;
            accumulatedSeconds = 0f;
            completionRaised = false;
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

            if (runner.Run.Status == SimulationRunStatus.Complete && !completionRaised)
            {
                completionRaised = true;
                RunCompleted?.Invoke(runner.Run);
            }
        }
    }
}
