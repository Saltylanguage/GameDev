using UnityEngine;

namespace SaltyGame
{
    public sealed class VM_ExpeditionSetup : VM_LabFeature
    {
        public const string DefaultScenarioId = "ForestEdge";
        public const string DefaultPlayerSpeciesId = "hare";
        public const int DefaultSeed = 10100;

        public override string FeatureId => "ExpeditionSetup";
        public override string Title => "EXPEDITION SETUP";
        public override string Description => "Choose a scenario and player species before entering the deterministic simulation.";
        public override string StatusText => "FOREST EDGE · HARE · SEED 10100";

        public string ScenarioId => DefaultScenarioId;
        public string PlayerSpeciesId => DefaultPlayerSpeciesId;
        public int Seed => DefaultSeed;

        public SimulationLaunchRequest CreateLaunchRequest(ProfileSessionSnapshot profile)
        {
            if (profile == null || !profile.HasLoadedProfile)
            {
                return null;
            }

            return new SimulationLaunchRequest(
                profile.ProfileId,
                ScenarioId,
                PlayerSpeciesId,
                Seed);
        }
    }
}
