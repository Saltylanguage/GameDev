using System.Collections;
using System.ComponentModel;
using Noesis;
using UnityEngine;
using UnityEngine.U2D;

namespace SaltyGame
{
    public sealed class GalapagOSDesktopNoesisHost : MonoBehaviour
    {
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
        int lastReactionTick = -1;
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
            StartCoroutine(EnableDesktopAudioListener());
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
            var hasProfileRoute = Helper_SceneTransition.TryConsumeDesktopProfile(out var profileId);
            viewModel.BindProfileSession(profileSession, permanentEconomy: hasProfileRoute);
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

            if (simulationCompositionReady && simulationPreview != null)
            {
                SpeciesSimulationPreview.RunCompleted += HandleRunCompleted;
            }
        }

        IEnumerator EnableDesktopAudioListener()
        {
            var listener = GetComponent<AudioListener>();
            if (listener == null) listener = gameObject.AddComponent<AudioListener>();
            listener.enabled = false;
            while (true)
            {
                var otherListenerActive = false;
                foreach (var candidate in FindObjectsByType<AudioListener>())
                {
                    if (candidate == listener || !candidate.enabled) continue;
                    otherListenerActive = true;
                    break;
                }
                if (!otherListenerActive)
                {
                    listener.enabled = true;
                    yield break;
                }
                yield return null;
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
                if (!TryCaptureProfileGenome(out var genome))
                {
                    return false;
                }
                var launch = new SimulationLaunchRequest(
                    profile.ProfileId,
                    VM_ExpeditionSetup.DefaultScenarioId,
                    VM_ExpeditionSetup.DefaultPlayerSpeciesId,
                    VM_ExpeditionSetup.DefaultSeed,
                    activeGenomeSnapshot: genome);
                if (!simulationPreview.TryApplyLaunchRequest(launch, out var validationMessage))
                {
                    Debug.LogError($"Desktop Simulation launch was rejected: {validationMessage}", this);
                    return false;
                }
                simulationPreview.BindMutationResearch(profile);
                simulationPreview.BindGenomeLaunchSource(() =>
                {
                    if (!profileSession.Current.HasLoadedProfile || profileSession.Current.ProfileId != profile.ProfileId)
                        throw new System.InvalidOperationException("The journey's original profile must still be loaded.");
                    return profileSession.Current.CreateLaunchGenomeSnapshot(viewModel.GenomeCatalog);
                });
                simulationPreview.BindPermanentResults(new FieldDataPermanentResults(profileSession));
            }

            simulationViewModel.Initialize(simulationPreview, animalAtlas, terrainAtlas, foxSprite, rabbitSprite);
            simulationViewModel.BindDesktopClose(CloseSimulation);
            simulationBoardViewModel.Initialize(simulationPreview);
            return true;
        }

        bool TryCaptureProfileGenome(out GenomeSimulationSnapshot snapshot)
        {
            try
            {
                snapshot = profileSession.Current.CreateLaunchGenomeSnapshot(viewModel.GenomeCatalog);
                return true;
            }
            catch (System.Exception exception) when (exception is System.ArgumentException
                || exception is System.InvalidOperationException)
            {
                snapshot = null;
                Debug.LogError($"Desktop Genome launch was rejected: {exception.Message}", this);
                return false;
            }
        }

        void OpenSimulation()
        {
            if (!simulationCompositionReady)
            {
                Debug.LogError("GalapagOSDesktopNoesisHost cannot open Simulation because its composition is not ready.", this);
                return;
            }

            var preparingNextJourney = simulationPreview.State == SpeciesPreviewState.Results;
            if (preparingNextJourney)
            {
                simulationPreview.PlayNextSimulation(startImmediately: false);
            }

            if (profileLaunchActive && profileSession != null && simulationPreview.SettingsEditable)
            {
                if (!TryCaptureProfileGenome(out var genome))
                {
                    return;
                }
                if (!simulationPreview.TrySetActiveGenomeBeforeJourney(genome, out var genomeMessage))
                {
                    Debug.LogError($"Desktop Genome launch was rejected: {genomeMessage}", this);
                    return;
                }
                if (simulationPreview.JourneyCurrentNode == null)
                    simulationPreview.BindMutationResearch(profileSession.Current);
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
            simulationBoard.CellSelected += HandleCellSelected;
            simulationBoard.DataContext = simulationBoardViewModel;
            simulationView.Content.DataContext = simulationViewModel;
            simulationBoardViewModel.PropertyChanged -= HandleBoardPropertyChanged;
            simulationBoardViewModel.PropertyChanged += HandleBoardPropertyChanged;
            ApplyBoardSnapshot();
            SimulationSoundPalette.StartBed(gameObject);

            if (preparingNextJourney)
            {
                if (!simulationPreview.JourneyActive && simulationPreview.ConfiguredFieldDataJourney == null)
                {
                    simulationPreview.StartSimulation();
                }
                simulationBoardViewModel.Initialize(simulationPreview);
                ApplyBoardSnapshot();
                if (simulationPreview.JourneyMap != null)
                {
                    simulationViewModel.OpenLaunchSelection();
                }
                else if (simulationPreview.JourneyActive || simulationPreview.ConfiguredFieldDataJourney != null)
                {
                    simulationViewModel.OpenJourneyMapCommand.Execute(null);
                }
            }
            else if (simulationPreview.State == SpeciesPreviewState.Ready
                && simulationViewModel.CanStart)
            {
                if (simulationPreview.JourneyMap != null)
                {
                    simulationViewModel.OpenLaunchSelection();
                }
                else if (simulationPreview.JourneyActive || simulationPreview.ConfiguredFieldDataJourney != null)
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
            if (!simulationPreview.TryLeavePermanentResults()) return;
            SimulationSoundPalette.StopBed(gameObject);
            simulationBoardViewModel.PropertyChanged -= HandleBoardPropertyChanged;
            if (simulationBoard != null) simulationBoard.CellSelected -= HandleCellSelected;
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
            SimulationSoundPalette.StopBed(gameObject);
            if (simulationPreview != null) simulationPreview.BindGenomeLaunchSource(null);
            if (simulationPreview != null) simulationPreview.BindPermanentResults(null);
            SpeciesSimulationPreview.RunCompleted -= HandleRunCompleted;
            TrackSimulationTicks();
            if (profileLaunchActive && profileSession != null)
            {
                profileSession.Flush();
            }

            if (simulationBoardViewModel != null)
            {
                simulationBoardViewModel.PropertyChanged -= HandleBoardPropertyChanged;
            }
            if (simulationBoard != null) simulationBoard.CellSelected -= HandleCellSelected;
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

        void HandleRunCompleted(SpeciesSimulationPreview preview, SimulationRunState run)
        {
            if (!profileLaunchActive
                || profileSession == null
                || preview == null
                || !ReferenceEquals(preview, simulationPreview)
                || !preview.JourneyActive
                || run == null
                || !run.SupportsContinuation
                || !preview.LastRunRewardsEligible
                || string.IsNullOrWhiteSpace(preview.RunCompletionId))
            {
                return;
            }

            try
            {
                profileSession.RecordSuccessfulJourney(preview.RunCompletionId);
            }
            catch (System.Exception exception)
            {
                Debug.LogError($"Could not save successful journey rewards: {exception.Message}", this);
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
            else if (args.PropertyName == nameof(VM_SimulationBoard.HasSelection))
            {
                simulationBoard?.SetSelectedCell(simulationBoardViewModel.SelectedX, simulationBoardViewModel.SelectedY);
            }
        }

        void ApplyBoardSnapshot()
        {
            if (simulationBoard == null || simulationBoardViewModel == null)
            {
                return;
            }

            var currentRun = simulationPreview != null ? simulationPreview.Run : null;
            if (!ReferenceEquals(lastReactionRun, currentRun)
                || (currentRun != null && currentRun.Tick < lastReactionTick))
            {
                lastReactionRun = currentRun;
                lastReactionTick = currentRun?.Tick ?? -1;
                simulationViewModel?.ResetRunActivity();
            }

            simulationBoard.SetSnapshot(simulationBoardViewModel.Snapshot);
            simulationBoard.SetSelectedCell(simulationBoardViewModel.SelectedX, simulationBoardViewModel.SelectedY);
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
            if (currentRun != null && currentRun.Tick > lastReactionTick)
            {
                if (simulationViewModel != null && simulationViewModel.PresentRunActivity(
                    currentRun, lastReactionTick, simulationBoardViewModel.FoxHuntCueTick > lastReactionTick))
                {
                    SimulationBirthChime.Play(gameObject);
                }
                lastReactionTick = currentRun.Tick;
            }
        }

        void HandleCellSelected(int x, int y)
        {
            simulationBoardViewModel?.SelectCell(x, y);
            SimulationSoundPalette.Play(gameObject, SimulationSound.Selection);
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
