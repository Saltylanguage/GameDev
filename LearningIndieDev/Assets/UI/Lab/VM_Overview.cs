using UnityEngine;

namespace SaltyGame
{
    public sealed class VM_Overview : VM_LabFeature
    {
        public override string FeatureId => "Overview";
        public override string Title => "LAB OVERVIEW";
        public override string Description => "Review the current expedition profile and prepare the next experiment.";
        public override string StatusText => "SYSTEMS NOMINAL — EXPEDITION READY";
    }
}
