using Noesis;
using UnityEngine;
using UnityEngine.U2D;

namespace SaltyGame
{
    public sealed class SpeciesSimulationNoesisHost : MonoBehaviour
    {
        [Header("Serialized Composition")]
        [SerializeField] SpeciesSimulationPreview preview;
        [SerializeField] Camera uiCamera;
        [SerializeField] NoesisView view;
        [SerializeField] SpeciesSimulationViewModel viewModel;
        [SerializeField] VM_SimulationBoard boardViewModel;
        [SerializeField] Helper_ProfileSession profileSession;
        [SerializeField] Helper_SceneTransition sceneTransition;

        [Header("UI Assets")]
        [SerializeField] NoesisXaml xaml;
        [SerializeField] SpriteAtlas animalAtlas;
        [SerializeField] SpriteAtlas terrainAtlas;
        [SerializeField] Sprite foxSprite;
        [SerializeField] Sprite rabbitSprite;
        [SerializeField] bool enableNoesisUi = true;

        void Start()
        {
            if (!enableNoesisUi || xaml == null)
            {
                return;
            }

            if (preview == null
                || uiCamera == null
                || view == null
                || viewModel == null
                || boardViewModel == null)
            {
                Debug.LogError(
                    "SpeciesSimulationNoesisHost requires serialized preview, camera, view, shell VM, and board VM references.",
                    this);
                return;
            }

            view.enabled = false;
            view.Xaml = xaml;
            view.enabled = true;

            if (Helper_SceneTransition.TryConsumeSimulationLaunch(out var launch))
            {
                if (!preview.TryApplyLaunchRequest(launch, out var validationMessage))
                {
                    Debug.LogError($"Simulation launch request was rejected: {validationMessage}", this);
                    return;
                }
            }

            viewModel.Initialize(preview, animalAtlas, terrainAtlas, foxSprite, rabbitSprite);
            viewModel.BindSceneTransition(sceneTransition, profileSession);
            viewModel.BindToView(view);

            boardViewModel.Initialize(preview);
            boardViewModel.SetSpriteVisuals(
                viewModel.AnimalSprites,
                viewModel.GrassTerrainTiles,
                viewModel.DesertTerrainTiles);
            boardViewModel.BindToView(view);
        }
    }
}
