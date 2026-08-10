using UnityEngine;

namespace SaltyGame
{
    public sealed class CellularAutomataPrototypeRuntime : MonoBehaviour
    {
        public LifeSimulationPreview LifePreview { get; private set; }

        void Awake()
        {
            LifePreview = gameObject.AddComponent<LifeSimulationPreview>();
        }
    }
}
