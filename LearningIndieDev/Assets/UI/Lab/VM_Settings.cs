using UnityEngine;

namespace SaltyGame
{
    public sealed class VM_Settings : VM_LabFeature
    {
        public override string FeatureId => "Settings";
        public override string Title => "SETTINGS";
        public override string Description => "Presentation and accessibility preferences will live here without owning simulation state.";
        public override string StatusText => "SETTINGS SURFACE — UI-ONLY PROTOTYPE";
    }
}
