using System;
using System.Collections.Generic;
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

        public event Action<SimulationRunState> PhaseBoundaryReached
        {
            add
            {
                simulationManager.PhaseBoundaryReached += value;
            }
            remove
            {
                simulationManager.PhaseBoundaryReached -= value;
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

        public bool ContinueWithoutUpgrade()
        {
            return simulationManager.ContinueWithoutUpgrade();
        }

        public bool ContinueWithBoundaryState(
            IReadOnlyDictionary<SpeciesId, SpeciesRules> nextRules,
            SpeciesExperimentalOptions nextExperimentalOptions,
            IEnumerable<SpeciesUpgradeSnapshot> nextUpgradeLoadout)
        {
            return simulationManager.ContinueWithBoundaryState(
                nextRules,
                nextExperimentalOptions,
                nextUpgradeLoadout);
        }

        public bool EndRun()
        {
            return simulationManager.End();
        }

        public void Advance(float deltaSeconds)
        {
            simulationManager.Advance(deltaSeconds);
        }
    }
}
