using System.ComponentModel;
using Noesis;
using UnityEngine;
using UnityEngine.U2D;

namespace SaltyGame
{
    public sealed class GalapagOSDesktopNoesisHost : MonoBehaviour
    {
        static readonly SpeciesId FoxSpeciesId = new SpeciesId("fox");

        [Header("Desktop Composition")]
        [SerializeField] Camera desktopCamera;
        [SerializeField] NoesisView view;
        [SerializeField] VM_GalapagOS_Desktop viewModel;
        [SerializeField] NoesisXaml xaml;
        [SerializeField] Helper_ProfileSession profileSession;
        [SerializeField] Helper_SceneTransition sceneTransition;

        [Header("Simulation Composition")]
        [SerializeField] Camera simulationCamera;
        [SerializeField] NoesisView simulationView;
        [SerializeField] NoesisXaml simulationXaml;
        [SerializeField] SpeciesSimulationPreview simulationPreview;
        [SerializeField] VM_SimulationShell simulationViewModel;
        [SerializeField] VM_SimulationBoard simulationBoardViewModel;
        [SerializeField] SpriteAtlas animalAtlas;
        [SerializeField] SpriteAtlas terrainAtlas;
        [SerializeField] Sprite foxSprite;
        [SerializeField] Sprite rabbitSprite;

        SpeciesSimulationBoard simulationBoard;
        bool simulationCompositionReady;
        bool profileLaunchActive;
        SimulationRunState trackedProfileRun;
        int trackedProfileTick;
        int lastBirthSoundTick = -1;
        int lastFoxHuntReactionTick = -1;
        SimulationRunState lastReactionRun;

        void Start()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (desktopCamera == null || view == null || viewModel == null || xaml == null)
            {
                Debug.LogError("GalapagOSDesktopNoesisHost requires serialized desktop camera, NoesisView, VM_GalapagOS_Desktop, and XAML references.", this);
                return;
            }

            desktopCamera.enabled = true;
            view.enabled = true;
            if (simulationCamera != null)
            {
                simulationCamera.enabled = false;
            }

            if (simulationView != null)
            {
                simulationView.enabled = false;
            }

            if (view.Content == null)
            {
                Debug.LogError("GalapagOSDesktopNoesisHost could not load the desktop XAML content.", this);
                return;
            }

            view.Content.DataContext = viewModel;
            viewModel.BindSimulationLauncher(OpenSimulation);
            viewModel.BindProfileSession(profileSession);
            var hasProfileRoute = Helper_SceneTransition.TryConsumeDesktopProfile(out var profileId);
            if (hasProfileRoute
                && (profileSession == null
                    || !profileSession.Current.HasLoadedProfile
                    || profileSession.Current.ProfileId != profileId))
            {
                RecoverProfileSelection("The selected profile could not be loaded for Desktop launch.");
                return;
            }

            simulationCompositionReady = TryInitializeSimulation(hasProfileRoute);
            if (hasProfileRoute && !simulationCompositionReady)
            {
                RecoverProfileSelection("The profile's Simulation launch settings could not be applied.");
                return;
            }

            profileLaunchActive = hasProfileRoute;
            if (profileLaunchActive)
            {
                profileSession.SetPlaytimeTracking(true);
                trackedProfileRun = simulationPreview.Run;
                trackedProfileTick = trackedProfileRun?.Tick ?? 0;
            }
        }

        bool TryInitializeSimulation(bool hasProfileRoute)
        {
            if (simulationCamera == null
                || simulationView == null
                || simulationXaml == null
                || simulationPreview == null
                || simulationViewModel == null
                || simulationBoardViewModel == null)
            {
                Debug.LogError(
                    "GalapagOSDesktopNoesisHost requires simulation XAML, preview, shell VM, and board VM references.",
                    this);
                return false;
            }

            if (hasProfileRoute)
            {
                var profile = profileSession.Current;
                var launch = new SimulationLaunchRequest(
                    profile.ProfileId,
                    VM_ExpeditionSetup.DefaultScenarioId,
                    VM_ExpeditionSetup.DefaultPlayerSpeciesId,
                    VM_ExpeditionSetup.DefaultSeed,
                    activeGenomeSnapshot: profile.CreateGenomeSnapshot());
                if (!simulationPreview.TryApplyLaunchRequest(launch, out var validationMessage))
                {
                    Debug.LogError($"Desktop Simulation launch was rejected: {validationMessage}", this);
                    return false;
                }
            }

            simulationViewModel.Initialize(simulationPreview, animalAtlas, terrainAtlas, foxSprite, rabbitSprite);
            simulationViewModel.BindDesktopClose(CloseSimulation);
            simulationBoardViewModel.Initialize(simulationPreview);
            return true;
        }

        void OpenSimulation()
        {
            if (!simulationCompositionReady)
            {
                Debug.LogError("GalapagOSDesktopNoesisHost cannot open Simulation because its composition is not ready.", this);
                return;
            }

            desktopCamera.enabled = false;
            view.enabled = false;
            simulationCamera.enabled = true;
            simulationView.enabled = true;

            if (simulationView.Content == null)
            {
                Debug.LogError("GalapagOSDesktopNoesisHost could not load the simulation XAML content.", this);
                CloseSimulation();
                return;
            }

            simulationView.Content.DataContext = simulationViewModel;
            simulationBoard = FindSimulationBoard(simulationView.Content);
            if (simulationBoard == null)
            {
                Debug.LogError("GalapagOSDesktopNoesisHost could not find the SimulationBoard control in its XAML content.", this);
                CloseSimulation();
                return;
            }

            simulationBoard.SetSpriteVisuals(
                simulationViewModel.AnimalSprites,
                simulationViewModel.GrassTerrainTiles,
                simulationViewModel.DesertTerrainTiles);
            simulationBoardViewModel.PropertyChanged -= HandleBoardPropertyChanged;
            simulationBoardViewModel.PropertyChanged += HandleBoardPropertyChanged;
            ApplyBoardSnapshot();

            // The journey map is the entry point for Forest Edge/Hare. Other
            // scenarios keep the prepared live-board launch behavior.
            if (simulationPreview.State == SpeciesPreviewState.Results)
            {
                simulationPreview.PlayNextSimulation(
                    startImmediately: !simulationPreview.JourneyActive);
                simulationBoardViewModel.Initialize(simulationPreview);
                ApplyBoardSnapshot();
                if (simulationPreview.JourneyActive)
                {
                    simulationViewModel.OpenJourneyMapCommand.Execute(null);
                }
            }
            else if (simulationPreview.State == SpeciesPreviewState.Ready
                && simulationViewModel.CanStart)
            {
                if (simulationPreview.JourneyActive)
                {
                    simulationViewModel.OpenJourneyMapCommand.Execute(null);
                }
                else
                {
                    simulationViewModel.StartCommand.Execute(null);
                }
                ApplyBoardSnapshot();
            }
        }

        void CloseSimulation()
        {
            simulationBoardViewModel.PropertyChanged -= HandleBoardPropertyChanged;
            simulationBoard?.ClearFoxHuntCue();
            simulationBoard?.ClearMatingCue();
            simulationBoard = null;
            simulationView.enabled = false;
            simulationCamera.enabled = false;
            desktopCamera.enabled = true;
            view.enabled = true;
        }

        void OnDestroy()
        {
            TrackSimulationTicks();
            if (profileLaunchActive && profileSession != null)
            {
                profileSession.Flush();
            }

            if (simulationBoardViewModel != null)
            {
                simulationBoardViewModel.PropertyChanged -= HandleBoardPropertyChanged;
            }
        }

        void Update()
        {
            TrackSimulationTicks();
            simulationBoard?.UpdateFoxHuntCue();
            simulationBoard?.UpdateMatingCue(
                simulationPreview != null && simulationPreview.State == SpeciesPreviewState.Paused);
        }

        void TrackSimulationTicks()
        {
            if (!profileLaunchActive || profileSession == null || simulationPreview == null)
            {
                return;
            }

            var run = simulationPreview.Run;
            if (!ReferenceEquals(run, trackedProfileRun))
            {
                trackedProfileRun = run;
                trackedProfileTick = 0;
            }

            if (run != null && run.Tick > trackedProfileTick)
            {
                profileSession.RecordSimulationTicks(run.Tick - trackedProfileTick);
                trackedProfileTick = run.Tick;
            }
        }

        void RecoverProfileSelection(string message)
        {
            Debug.LogError(message, this);
            if (sceneTransition == null || !sceneTransition.ReturnToProfileSelection())
            {
                Debug.LogError("Desktop profile recovery could not open Profile Selection.", this);
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
            if (simulationBoard == null || simulationBoardViewModel == null)
            {
                return;
            }

            var currentRun = simulationPreview != null ? simulationPreview.Run : null;
            if (!ReferenceEquals(lastReactionRun, currentRun))
            {
                lastReactionRun = currentRun;
                lastBirthSoundTick = -1;
                lastFoxHuntReactionTick = -1;
                simulationViewModel?.ClearEventReaction();
            }

            simulationBoard.SetSnapshot(simulationBoardViewModel.Snapshot);
            simulationBoard.SetForestEdgeFlourishes(simulationViewModel.ForestEdgeFlourishes);
            simulationBoard.SetFoxHuntCue(
                simulationBoardViewModel.FoxHuntCueX,
                simulationBoardViewModel.FoxHuntCueY,
                simulationBoardViewModel.FoxHuntCueTick);
            simulationBoard.SetHuntFootprints(simulationBoardViewModel.HuntFootprints);
            simulationBoard.SetMatingCue(
                simulationBoardViewModel.MatingCueX,
                simulationBoardViewModel.MatingCueY,
                simulationBoardViewModel.MatingCueMateX,
                simulationBoardViewModel.MatingCueMateY,
                simulationBoardViewModel.MatingCueOffspringX,
                simulationBoardViewModel.MatingCueOffspringY,
                simulationBoardViewModel.MatingCueTick);
            simulationBoard.SetBirthCues(simulationBoardViewModel.RecentBirths);
            if (simulationBoardViewModel.FoxHuntCueTick < 0)
            {
                lastFoxHuntReactionTick = -1;
            }
            else if (simulationBoardViewModel.FoxHuntCueTick != lastFoxHuntReactionTick)
            {
                simulationViewModel?.PresentFoxHuntReaction();
                lastFoxHuntReactionTick = simulationBoardViewModel.FoxHuntCueTick;
            }

            if (simulationBoardViewModel.MatingCueTick < 0)
            {
                lastBirthSoundTick = -1;
            }
            if (simulationBoardViewModel.MatingCueTick >= 0
                && simulationBoardViewModel.MatingCueTick != lastBirthSoundTick)
            {
                SimulationBirthChime.Play(gameObject);
                var births = simulationBoardViewModel.RecentBirths;
                var foxPortrait = births.Count > 0 && births[0].Species == FoxSpeciesId;
                simulationViewModel?.PresentBirthReaction(births.Count, foxPortrait);
                lastBirthSoundTick = simulationBoardViewModel.MatingCueTick;
            }
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
