using UnityEngine;

namespace SaltyGame
{
    public abstract class VM_LabFeature : MonoBehaviour
    {
        public abstract string FeatureId { get; }
        public abstract string Title { get; }
        public abstract string Description { get; }
        public abstract string StatusText { get; }
    }
}
