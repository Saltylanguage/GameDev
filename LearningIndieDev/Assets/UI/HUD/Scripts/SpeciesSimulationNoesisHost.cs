using System.ComponentModel;
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
        [SerializeField] VM_SimulationShell viewModel;
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

        SpeciesSimulationBoard simulationBoard;

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

            if (view.Content == null)
            {
                Debug.LogError("SpeciesSimulationNoesisHost could not load its XAML content.", this);
                return;
            }

            view.Content.DataContext = viewModel;

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

            boardViewModel.Initialize(preview);
            simulationBoard = FindSimulationBoard(view.Content);
            if (simulationBoard == null)
            {
                Debug.LogError("SpeciesSimulationNoesisHost could not find the SimulationBoard control in its XAML content.", this);
                return;
            }

            simulationBoard.SetSpriteVisuals(
                viewModel.AnimalSprites,
                viewModel.GrassTerrainTiles,
                viewModel.DesertTerrainTiles);
            boardViewModel.PropertyChanged += HandleBoardPropertyChanged;
            ApplyBoardSnapshot();

            // The Lab is the setup entry point; the simulation scene should open live.
            if (preview.State == SpeciesPreviewState.Ready && viewModel.CanStart)
            {
                viewModel.StartCommand.Execute(null);
                ApplyBoardSnapshot();
            }
        }

        void OnDestroy()
        {
            if (boardViewModel != null)
            {
                boardViewModel.PropertyChanged -= HandleBoardPropertyChanged;
            }
        }

        void HandleBoardPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(VM_SimulationBoard.Snapshot))
            {
                ApplyBoardSnapshot();
            }
        }

        void ApplyBoardSnapshot()
        {
            simulationBoard?.SetSnapshot(boardViewModel?.Snapshot);
        }

        static SpeciesSimulationBoard FindSimulationBoard(FrameworkElement root)
        {
            if (root == null)
            {
                return null;
            }

            var board = root.FindName("SimulationBoard") as SpeciesSimulationBoard;
            if (board != null)
            {
                return board;
            }

            var window = root.FindName("SimulationWindow") as HeaderedContentControl;
            var windowContent = window?.Content as FrameworkElement;
            return windowContent?.FindName("SimulationBoard") as SpeciesSimulationBoard;
        }
    }
}
