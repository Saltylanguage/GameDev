using System;
using UnityEngine;

namespace SaltyGame
{
    /// <summary>
    /// Unity-facing micro-API for simulation commands and lifecycle forwarding.
    /// Authoritative run state remains in <see cref="SimulationManager"/>.
    /// </summary>
    public sealed class Helper_Simulation : MonoBehaviour
    {
        readonly SimulationManager simulationManager = new SimulationManager();

        public event Action<SimulationRunState> RunCompleted
        {
            add
            {
                simulationManager.RunCompleted += value;
            }
            remove
            {
                simulationManager.RunCompleted -= value;
            }
        }

        public SimulationRunState Run => simulationManager.Run;

        public void SetRunner(SpeciesSimulationRunner runner)
        {
            simulationManager.SetRunner(runner);
        }

        public bool StartRun()
        {
            return simulationManager.Start();
        }

        public bool PauseRun()
        {
            return simulationManager.Pause();
        }

        public bool ResumeRun()
        {
            return simulationManager.Resume();
        }

        public bool RestartRun()
        {
            return simulationManager.Restart();
        }

        public bool StopRun()
        {
            return simulationManager.Stop();
        }

        public void Advance(float deltaSeconds)
        {
            simulationManager.Advance(deltaSeconds);
        }
    }
}
