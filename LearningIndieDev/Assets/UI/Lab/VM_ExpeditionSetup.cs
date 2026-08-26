using UnityEngine;

namespace SaltyGame
{
    public sealed class VM_ExpeditionSetup : VM_LabFeature
    {
        public override string FeatureId => "ExpeditionSetup";
        public override string Title => "EXPEDITION SETUP";
        public override string Description => "Choose a scenario and player species before entering the deterministic simulation.";
        public override string StatusText => "LAUNCH HANDOFF — SIMULATION WIRING COMES IN T6";
    }
}
