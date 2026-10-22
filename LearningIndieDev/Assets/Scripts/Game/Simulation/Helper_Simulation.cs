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

        public bool AdvanceOneTickWhilePaused()
        {
            return simulationManager.AdvanceOneTickWhilePaused();
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

        public bool CanChangeBoundaryPopulation(SpeciesId species, int requested, out int actual, out string reason)
        {
            actual = 0;
            reason = "No active simulation.";
            return simulationManager != null && simulationManager.CanChangeBoundaryPopulation(species, requested, out actual, out reason);
        }

        public bool TryChangeBoundaryPopulation(SpeciesId species, int requested, string source, out int actual)
        {
            actual = 0;
            return simulationManager != null && simulationManager.TryChangeBoundaryPopulation(species, requested, source, out actual);
        }

        public bool TryAddBoundaryPopulation(SpeciesId species, int amount)
        {
            return simulationManager.TryAddBoundaryPopulation(species, amount);
        }

        public bool TryAddBoundaryPopulation(SpeciesId species, int amount, out int added, bool ignorePopulationPool = false)
        {
            return simulationManager.TryAddBoundaryPopulation(species, amount, out added, ignorePopulationPool);
        }

        public bool ContinueWithBoundaryState(
            IReadOnlyDictionary<SpeciesId, SpeciesRules> nextRules,
            SpeciesExperimentalOptions nextExperimentalOptions,
            IEnumerable<SpeciesUpgradeSnapshot> nextUpgradeLoadout,
            SpeciesUpgradeSnapshot selectedUpgrade = null)
        {
            return simulationManager.ContinueWithBoundaryState(
                nextRules,
                nextExperimentalOptions,
                nextUpgradeLoadout,
                selectedUpgrade);
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
