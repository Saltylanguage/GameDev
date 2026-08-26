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

            viewModel.Initialize(preview, animalAtlas, terrainAtlas, foxSprite, rabbitSprite);
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
