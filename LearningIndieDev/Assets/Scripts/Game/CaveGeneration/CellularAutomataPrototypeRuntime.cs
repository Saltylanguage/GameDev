using UnityEngine;

namespace SaltyGame
{
    public sealed class CellularAutomataPrototypeRuntime : MonoBehaviour
    {
        public SpeciesSimulationPreview SpeciesPreview { get; private set; }

        void Awake()
        {
            SpeciesPreview = gameObject.AddComponent<SpeciesSimulationPreview>();
        }
    }
}
