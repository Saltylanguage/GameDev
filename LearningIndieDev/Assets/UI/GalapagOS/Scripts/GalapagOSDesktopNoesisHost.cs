using System;
using System.Collections.Generic;
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
        [SerializeField] SpeciesUpgradeAsset[] plannerTestMutations = Array.Empty<SpeciesUpgradeAsset>();
        [SerializeField] GenomeUpgradeAsset[] plannerTestGenomeNodes = Array.Empty<GenomeUpgradeAsset>();

        SpeciesSimulationBoard simulationBoard;
        bool simulationCompositionReady;
        bool profileLaunchActive;
        SimulationRunState trackedProfileRun;
        int trackedProfileTick;
        int lastReactionTick = -1;
        SimulationRunState lastReactionRun;
        string configuredPlannerSignature;

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
                if (!TryCreatePlannerLaunchRequest(out var launch, out var signature, out var validationMessage))
                {
                    Debug.LogError($"Desktop Simulation launch was rejected: {validationMessage}", this);
                    return false;
                }
                if (!simulationPreview.TryApplyLaunchRequest(launch, out validationMessage))
                {
                    Debug.LogError($"Desktop Simulation launch was rejected: {validationMessage}", this);
                    return false;
                }
                configuredPlannerSignature = signature;
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

            if (!TryCreatePlannerLaunchRequest(out var launch, out var signature, out var launchMessage))
            {
                Debug.LogError($"Desktop Simulation launch was rejected: {launchMessage}", this);
                return;
            }

            var plannerOptionsInjected = viewModel.UseTestMutation || viewModel.UseTestGenome;
            if (!string.Equals(signature, configuredPlannerSignature, StringComparison.Ordinal)
                || (preparingNextJourney && plannerOptionsInjected))
            {
                if (!simulationPreview.TryApplyLaunchRequest(launch, out launchMessage))
                {
                    Debug.LogError($"Desktop Simulation launch was rejected: {launchMessage}", this);
                    return;
                }
                configuredPlannerSignature = signature;
            }

            if (profileLaunchActive && profileSession != null && simulationPreview.SettingsEditable
                && !viewModel.UseTestGenome && !viewModel.UseTestMutation)
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

            // The journey map is the entry point for Forest Edge/Rabbit. Other
            // scenarios keep the prepared live-board launch behavior.
            if (preparingNextJourney)
            {
                if (!simulationPreview.JourneyActive && simulationPreview.ConfiguredFieldDataJourney == null)
                {
                    simulationPreview.StartSimulation();
                }
                simulationBoardViewModel.Initialize(simulationPreview);
                ApplyBoardSnapshot();
                if (simulationPreview.JourneyActive || simulationPreview.ConfiguredFieldDataJourney != null)
                {
                    simulationViewModel.OpenJourneyMapCommand.Execute(null);
                }
            }
            else if (simulationPreview.State == SpeciesPreviewState.Ready
                && simulationViewModel.CanStart)
            {
                if (simulationPreview.JourneyActive || simulationPreview.ConfiguredFieldDataJourney != null)
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

        bool TryCreatePlannerLaunchRequest(
            out SimulationLaunchRequest launch,
            out string signature,
            out string validationMessage)
        {
            launch = null;
            signature = string.Empty;
            validationMessage = string.Empty;
            if (viewModel == null || simulationPreview == null)
            {
                validationMessage = "The Expedition Planner is not connected to Simulation.";
                return false;
            }

            var profile = profileSession?.Current;
            var profileId = profile != null && profile.HasLoadedProfile
                ? profile.ProfileId
                : "desktop-sandbox";
            var genome = GenomeSimulationSnapshot.Empty;
            if (profile != null && profile.HasLoadedProfile)
            {
                if (!TryCaptureProfileGenome(out genome))
                {
                    validationMessage = "The loaded profile's active Genome could not be captured.";
                    return false;
                }
            }

            var speciesId = viewModel.PlannerPlayerSpeciesId;
            if (viewModel.UseTestGenome)
            {
                if (!TryCreateTestGenomeSnapshot(speciesId, genome, out var testGenome, out validationMessage))
                {
                    return false;
                }
                genome = testGenome;
            }

            var mutations = new List<SpeciesUpgradeSnapshot>();
            if (viewModel.UseTestMutation)
            {
                if (!TryFindSpeciesAsset(plannerTestMutations, speciesId, out var mutation)
                    || !mutation.TryCreateSnapshot(out var mutationSnapshot, out validationMessage))
                {
                    if (string.IsNullOrWhiteSpace(validationMessage))
                        validationMessage = $"No test Mutation is configured for '{speciesId}'.";
                    return false;
                }
                mutations.Add(mutationSnapshot);
            }

            launch = new SimulationLaunchRequest(
                profileId,
                viewModel.PlannerScenarioId,
                speciesId,
                VM_ExpeditionSetup.DefaultSeed,
                orderedUpgradeSnapshots: mutations,
                activeGenomeSnapshot: genome);
            signature = $"{profileId}|{viewModel.PlannerScenarioId}|{speciesId}|{genome.Fingerprint}|{string.Join(",", launch.OrderedUpgradeIds)}";
            return true;
        }

        bool TryCreateTestGenomeSnapshot(
            string speciesId,
            GenomeSimulationSnapshot baseSnapshot,
            out GenomeSimulationSnapshot snapshot,
            out string validationMessage)
        {
            snapshot = null;
            validationMessage = string.Empty;
            if (!TryFindSpeciesAsset(plannerTestGenomeNodes, speciesId, out var genomeAsset)
                || !genomeAsset.TryCreateSnapshot(out var node, out validationMessage))
            {
                if (string.IsNullOrWhiteSpace(validationMessage))
                    validationMessage = $"No test Genome node is configured for '{speciesId}'.";
                return false;
            }

            var species = new SpeciesId(speciesId);
            var map = viewModel.GenomeCatalog.GetSpeciesMap(species);
            var catalogNode = map?.GetNode(node.Id);
            if (catalogNode == null)
            {
                validationMessage = $"The test Genome node '{node.Id}' is missing from the active Genome catalog.";
                return false;
            }

            var speciesSnapshots = new List<SpeciesGenomeSnapshot>();
            if (baseSnapshot != null)
            {
                foreach (var existing in baseSnapshot.SpeciesGenomes.Values)
                {
                    if (existing.SpeciesId != species)
                        speciesSnapshots.Add(existing);
                }
            }
            speciesSnapshots.Add(new SpeciesGenomeSnapshot(
                species,
                new[] { catalogNode.Id },
                new[] { catalogNode },
                viewModel.GenomeCatalog.Fingerprint));
            snapshot = new GenomeSimulationSnapshot(speciesSnapshots);
            return true;
        }

        static bool TryFindSpeciesAsset<TAsset>(TAsset[] assets, string speciesId, out TAsset match)
            where TAsset : UnityEngine.Object
        {
            foreach (var asset in assets ?? Array.Empty<TAsset>())
            {
                if (asset == null) continue;
                var targetSpeciesId = asset is SpeciesUpgradeAsset mutation
                    ? mutation.TargetSpeciesId
                    : asset is GenomeUpgradeAsset genome ? genome.TargetSpeciesId : string.Empty;
                if (string.Equals(targetSpeciesId, speciesId, StringComparison.Ordinal))
                {
                    match = asset;
                    return true;
                }
            }

            match = null;
            return false;
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
                simulationBoard?.SetSelectedCell(simulationBoardViewModel.SelectedX, simulationBoardViewModel.SelectedY, simulationBoardViewModel.IsTrackingCreature);
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
            simulationBoard.SetSelectedCell(simulationBoardViewModel.SelectedX, simulationBoardViewModel.SelectedY, simulationBoardViewModel.IsTrackingCreature);
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
