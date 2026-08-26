using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
    public sealed class CellularAutomataPrototypeRuntime : MonoBehaviour
    {
        [Header("Serialized Composition")]
        [SerializeField] Helper_Simulation simulationHelper;
        [SerializeField] SpeciesSimulationPreview speciesPreview;
        [SerializeField] Camera backgroundCamera;

        [Header("Runtime Scenarios")]
        [SerializeField] List<ScenarioDefinitionAsset> scenarioOptions = new List<ScenarioDefinitionAsset>();
        [SerializeField, Min(-1)] int selectedScenarioIndex = -1;

        public SpeciesSimulationPreview SpeciesPreview => speciesPreview;
        public Helper_Simulation SimulationHelper => simulationHelper;

        void Awake()
        {
            if (simulationHelper == null || speciesPreview == null || backgroundCamera == null)
            {
                Debug.LogError(
                    "CellularAutomataPrototypeRuntime requires serialized helper, preview, and camera references.",
                    this);
                enabled = false;
                return;
            }

            speciesPreview.BindSimulationHelper(simulationHelper);
            speciesPreview.ConfigureScenarioOptions(scenarioOptions, selectedScenarioIndex);
        }
    }
}
