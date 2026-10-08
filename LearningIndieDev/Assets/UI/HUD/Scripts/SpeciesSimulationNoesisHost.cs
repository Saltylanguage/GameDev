using System.ComponentModel;
using Noesis;
using UnityEngine;
using UnityEngine.U2D;

namespace SaltyGame
{
    public sealed class SpeciesSimulationNoesisHost : MonoBehaviour
    {
        static readonly SpeciesId FoxSpeciesId = new SpeciesId("fox");

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
        int lastBirthSoundTick = -1;
        int lastFoxHuntReactionTick = -1;
        SimulationRunState lastReactionRun;

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

            var hasLaunchRequest = Helper_SceneTransition.TryConsumeSimulationLaunch(out var launch);
            if (hasLaunchRequest)
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

            simulationBoard.ZoomRequested += HandleBoardZoomRequested;
            simulationBoard.SetSpriteVisuals(
                viewModel.AnimalSprites,
                viewModel.GrassTerrainTiles,
                viewModel.DesertTerrainTiles);
            boardViewModel.PropertyChanged += HandleBoardPropertyChanged;
            viewModel.PropertyChanged += HandleViewModelPropertyChanged;
            ApplyBoardSnapshot();

            // Journey launch requests begin on the map; direct test scenes still open live.
            if (preview.State == SpeciesPreviewState.Results)
            {
                preview.PlayNextSimulation(startImmediately: !preview.JourneyActive);
                boardViewModel.Initialize(preview);
                if (preview.JourneyActive)
                {
                    viewModel.OpenJourneyMapCommand.Execute(null);
                }
            }
            else if (preview.State == SpeciesPreviewState.Ready && viewModel.CanStart)
            {
                if (hasLaunchRequest && preview.JourneyActive)
                {
                    viewModel.OpenJourneyMapCommand.Execute(null);
                }
                else
                {
                    viewModel.StartCommand.Execute(null);
                }
            }

            ApplyBoardSnapshot();
        }

        void OnDestroy()
        {
            if (simulationBoard != null)
            {
                simulationBoard.ZoomRequested -= HandleBoardZoomRequested;
            }

            if (boardViewModel != null)
            {
                boardViewModel.PropertyChanged -= HandleBoardPropertyChanged;
            }

            if (viewModel != null)
            {
                viewModel.PropertyChanged -= HandleViewModelPropertyChanged;
            }
        }

        void Update()
        {
            simulationBoard?.UpdateFoxHuntCue();
            simulationBoard?.UpdateMatingCue(
                preview != null && preview.State == SpeciesPreviewState.Paused);
        }

        void HandleBoardPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(VM_SimulationBoard.Snapshot))
            {
                ApplyBoardSnapshot();
            }
        }

        void HandleViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(VM_SimulationShell.BoardZoom))
            {
                ApplyBoardZoom();
            }
        }

        void ApplyBoardSnapshot()
        {
            if (simulationBoard == null || boardViewModel == null)
            {
                return;
            }

            var currentRun = preview != null ? preview.Run : null;
            if (!ReferenceEquals(lastReactionRun, currentRun))
            {
                lastReactionRun = currentRun;
                lastBirthSoundTick = -1;
                lastFoxHuntReactionTick = -1;
                viewModel?.ClearEventReaction();
            }

            simulationBoard.SetSnapshot(boardViewModel.Snapshot);
            ApplyBoardZoom();
            simulationBoard.SetFoxHuntCue(
                boardViewModel.FoxHuntCueX,
                boardViewModel.FoxHuntCueY,
                boardViewModel.FoxHuntCueTick);
            simulationBoard.SetHuntFootprints(boardViewModel.HuntFootprints);
            simulationBoard.SetMatingCue(
                boardViewModel.MatingCueX,
                boardViewModel.MatingCueY,
                boardViewModel.MatingCueMateX,
                boardViewModel.MatingCueMateY,
                boardViewModel.MatingCueOffspringX,
                boardViewModel.MatingCueOffspringY,
                boardViewModel.MatingCueTick);
            simulationBoard.SetBirthCues(boardViewModel.RecentBirths);
            if (boardViewModel.FoxHuntCueTick < 0)
            {
                lastFoxHuntReactionTick = -1;
            }
            else if (boardViewModel.FoxHuntCueTick != lastFoxHuntReactionTick)
            {
                viewModel?.PresentFoxHuntReaction();
                lastFoxHuntReactionTick = boardViewModel.FoxHuntCueTick;
            }

            if (boardViewModel.MatingCueTick < 0)
            {
                lastBirthSoundTick = -1;
            }
            if (boardViewModel.MatingCueTick >= 0
                && boardViewModel.MatingCueTick != lastBirthSoundTick)
            {
                SimulationBirthChime.Play(gameObject);
                var births = boardViewModel.RecentBirths;
                var foxPortrait = births.Count > 0 && births[0].Species == FoxSpeciesId;
                viewModel?.PresentBirthReaction(births.Count, foxPortrait);
                lastBirthSoundTick = boardViewModel.MatingCueTick;
            }
        }

        void ApplyBoardZoom()
        {
            if (simulationBoard != null && viewModel != null)
            {
                simulationBoard.Zoom = viewModel.BoardZoom;
            }
        }

        void HandleBoardZoomRequested(float zoom)
        {
            viewModel?.SetBoardZoom(zoom);
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
