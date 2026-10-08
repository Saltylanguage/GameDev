using System;
using System.Collections;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace SaltyGame.PlayModeTests
{
    [Category("Graphics")]
    public sealed class GalapagOSVisualAcceptanceTests
    {
        const string SceneName = "GalapagOSDesktopTest";
        const string DesktopCameraName = "GalapagOS Desktop Test Camera";
        const string SimulationCameraName = "GalapagOS Simulation View";
        const string CurrentDefaultSettingsKey = "SaltyGame.SpeciesSimulationPreview.DefaultSettings.v5";
        const string PreviousDefaultSettingsKey = "SaltyGame.SpeciesSimulationPreview.DefaultSettings.v4";
        const string LegacyDefaultSettingsKey = "SaltyGame.SpeciesSimulationPreview.DefaultSettings.v3";

        [UnityTest]
        public IEnumerator LegacySquareGridSettingsMigrateOnStartup()
        {
            var keys = new[]
            {
                CurrentDefaultSettingsKey,
                PreviousDefaultSettingsKey,
                LegacyDefaultSettingsKey,
            };
            var hadValues = new bool[keys.Length];
            var originalValues = new string[keys.Length];
            for (var i = 0; i < keys.Length; i++)
            {
                hadValues[i] = PlayerPrefs.HasKey(keys[i]);
                originalValues[i] = PlayerPrefs.GetString(keys[i]);
            }

            try
            {
                yield return SceneManager.LoadSceneAsync(SceneName);
                yield return null;

                var desktopRoot = GameObject.Find(DesktopCameraName);
                Assert.That(desktopRoot, Is.Not.Null);
                var preview = desktopRoot.GetComponent<SpeciesSimulationPreview>();
                Assert.That(preview, Is.Not.Null);
                Assert.That(((UnityEngine.Object)GetProperty(preview, "SelectedScenario")).name, Is.EqualTo("ForestEdge"));

                preview.SaveCurrentSettingsAsDefault();
                var authoredSettings = PlayerPrefs.GetString(CurrentDefaultSettingsKey);
                var legacySettings = authoredSettings
                    .Replace($"\"width\":{(int)GetProperty(preview, "GridWidth")}", "\"width\":64")
                    .Replace($"\"height\":{(int)GetProperty(preview, "GridHeight")}", "\"height\":64");
                StringAssert.Contains("\"width\":64", legacySettings);
                StringAssert.Contains("\"height\":64", legacySettings);

                PlayerPrefs.SetString(PreviousDefaultSettingsKey, legacySettings);
                PlayerPrefs.DeleteKey(CurrentDefaultSettingsKey);
                PlayerPrefs.Save();

                yield return SceneManager.LoadSceneAsync(SceneName);
                yield return null;

                desktopRoot = GameObject.Find(DesktopCameraName);
                preview = desktopRoot.GetComponent<SpeciesSimulationPreview>();
                Assert.That(GetProperty(preview, "GridWidth"), Is.EqualTo(36));
                Assert.That(GetProperty(preview, "GridHeight"), Is.EqualTo(20));
                var migratedSettings = PlayerPrefs.GetString(CurrentDefaultSettingsKey);
                StringAssert.Contains("\"width\":36", migratedSettings);
                StringAssert.Contains("\"height\":20", migratedSettings);
            }
            finally
            {
                for (var i = 0; i < keys.Length; i++)
                {
                    if (hadValues[i])
                    {
                        PlayerPrefs.SetString(keys[i], originalValues[i]);
                    }
                    else
                    {
                        PlayerPrefs.DeleteKey(keys[i]);
                    }
                }

                PlayerPrefs.Save();
            }
        }

        [UnityTest]
        public IEnumerator GalapagOSDesktopHomeCapturesVisualEvidenceWithoutOpeningWindows()
        {
            Screen.SetResolution(
                GetVisualDimension("CELLSIM_VISUAL_WIDTH", 1280),
                GetVisualDimension("CELLSIM_VISUAL_HEIGHT", 720),
                false);
            yield return null;
            yield return SceneManager.LoadSceneAsync(SceneName);
            yield return null;

            var desktopRoot = GameObject.Find(DesktopCameraName);
            Assert.That(desktopRoot, Is.Not.Null);
            var desktopCamera = desktopRoot.GetComponent<Camera>();
            Assert.That(desktopCamera, Is.Not.Null);
            Assert.That(desktopCamera.enabled, Is.True);

            var viewModel = desktopRoot.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(
                GetProperty(viewModel, "LabWindowVisibility").ToString(),
                Is.EqualTo("Collapsed"),
                "The desktop home screen must start without the legacy Lab placeholder window.");
            var openWindows = (System.Collections.IList)GetProperty(viewModel, "OpenDesktopWindows");
            Assert.That(openWindows.Count, Is.Zero, "Home capture must start before any desktop app is opened.");

            var outputDirectory = TryGetVisualOutputDirectory();
            if (outputDirectory == null)
            {
                Assert.Pass("Set CELLSIM_VISUAL_OUTPUT to capture GalapagOS desktop-home evidence.");
            }

            yield return CaptureCamera(outputDirectory, "01-galapagos-desktop-home", desktopCamera);
        }

        [UnityTest]
        public IEnumerator GalapagOSDesktopAndSimulationCaptureGameViewEvidence()
        {
            Screen.SetResolution(
                GetVisualDimension("CELLSIM_VISUAL_WIDTH", 1280),
                GetVisualDimension("CELLSIM_VISUAL_HEIGHT", 720),
                false);
            yield return null;
            yield return SceneManager.LoadSceneAsync(SceneName);
            yield return null;

            var desktopRoot = GameObject.Find(DesktopCameraName);
            Assert.That(desktopRoot, Is.Not.Null);
            var viewModel = desktopRoot.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            Assert.That(viewModel, Is.Not.Null);

            var outputDirectory = TryGetVisualOutputDirectory();
            if (outputDirectory == null)
            {
                Assert.Pass("Set CELLSIM_VISUAL_OUTPUT to capture GalapagOS Game-view evidence.");
            }

            var openCommand = GetProperty(viewModel, "OpenDesktopIconCommand");
            yield return CaptureCamera(outputDirectory, "01-galapagos-desktop-home", desktopRoot.GetComponent<Camera>());

            openCommand.GetType().GetMethod("Execute")?.Invoke(openCommand, new object[] { "Field Notes" });
            var fieldNotesWindows = (System.Collections.IList)GetProperty(viewModel, "OpenDesktopWindows");
            Assert.That(fieldNotesWindows.Count, Is.EqualTo(1));
            Assert.That(GetProperty(fieldNotesWindows[0], "FieldGuideSurfaceVisibility").ToString(), Is.EqualTo("Visible"));
            Assert.That((float)GetProperty(fieldNotesWindows[0], "Width"), Is.InRange(1040f, 1510f));
            Assert.That((float)GetProperty(fieldNotesWindows[0], "Height"), Is.InRange(560f, 820f));
            yield return CaptureCamera(outputDirectory, "02-galapagos-field-notes", desktopRoot.GetComponent<Camera>());

            openCommand.GetType().GetMethod("Execute")?.Invoke(openCommand, new object[] { "Settings" });
            var openWindows = (System.Collections.IList)GetProperty(viewModel, "OpenDesktopWindows");
            Assert.That(openWindows.Count, Is.EqualTo(2));
            Assert.That(GetProperty(openWindows[1], "SettingsSurfaceVisibility").ToString(), Is.EqualTo("Visible"));
            yield return null;
            yield return null;
            yield return null;

            openCommand.GetType().GetMethod("Execute")?.Invoke(openCommand, new object[] { "Simulation" });
            yield return null;
            yield return null;
            yield return null;
            var simulationCamera = GameObject.Find(SimulationCameraName)?.GetComponent<Camera>();
            Assert.That(simulationCamera, Is.Not.Null);
            var preview = desktopRoot.GetComponent("SaltyGame.SpeciesSimulationPreview");
            var simulationViewModel = desktopRoot.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(preview, Is.Not.Null);
            Assert.That(simulationViewModel, Is.Not.Null);
            Assert.That(GetProperty(preview, "State").ToString(), Is.EqualTo("Ready"));
            Assert.That(GetProperty(simulationViewModel, "JourneyMapVisibility").ToString(), Is.EqualTo("Visible"));
            for (var frame = 0; frame < 10; frame++) yield return null;
            ResetJourneyHover(simulationViewModel, ((SpeciesSimulationPreview)preview).JourneyMap.StartNode.NodeId);
            yield return null;
            yield return CaptureCamera(outputDirectory, "03a-galapagos-journey-map", simulationCamera, settleFrames: 0);
            ExecuteCommand(GetProperty(simulationViewModel, "ToggleJourneyBackdropCommand"));
            yield return null;
            Assert.That(GetProperty(simulationViewModel, "JourneySummerBackdropVisibility").ToString(), Is.EqualTo("Visible"));
            ResetJourneyHover(simulationViewModel, ((SpeciesSimulationPreview)preview).JourneyMap.StartNode.NodeId);
            yield return null;
            yield return CaptureCamera(outputDirectory, "03b-galapagos-journey-summer-trails", simulationCamera, settleFrames: 0);
            var stonefall = FindJourneyNode(simulationViewModel, "stonefall");
            ExecuteCommand(GetProperty(stonefall, "HoverCommand"));
            Assert.That(GetProperty(stonefall, "LabelVisibility").ToString(), Is.EqualTo("Visible"));
            yield return null;
            yield return CaptureCamera(outputDirectory, "03c-galapagos-journey-node-hover", simulationCamera, settleFrames: 0);
            ExecuteCommand(GetProperty(stonefall, "LeaveCommand"));
            ExecuteCommand(GetProperty(simulationViewModel, "ToggleJourneyBackdropCommand"));
            ExecuteCommand(GetProperty(FindJourneyNode(simulationViewModel, "forest-edge-start"), "ChooseCommand"));
            yield return null;
            yield return CaptureCamera(outputDirectory, "03d-galapagos-first-reward", simulationCamera);
            var firstRewards = (Array)GetProperty(simulationViewModel, "JourneyRewardItems");
            ExecuteCommand(GetProperty(firstRewards.GetValue(0), "ChooseCommand"));
            ExecuteCommand(GetProperty(FindJourneyNode(simulationViewModel, "first-cycle"), "ChooseCommand"));
            yield return null;
            yield return null;
            yield return null;
            yield return CaptureCamera(
                outputDirectory,
                "03-galapagos-simulation",
                simulationCamera);

            Assert.That(simulationCamera.enabled, Is.True);
            Assert.That(desktopRoot.GetComponent<Camera>().enabled, Is.False);

            Assert.That(GetProperty(preview, "State").ToString(), Is.EqualTo("Running"));
            Assert.That(GetProperty(simulationViewModel, "SettingsVisibility").ToString(), Is.EqualTo("Collapsed"));
            Assert.That(GetProperty(simulationViewModel, "RunningVisibility").ToString(), Is.EqualTo("Visible"));
            Assert.That(((UnityEngine.Object)GetProperty(preview, "SelectedScenario")).name, Is.EqualTo("ForestEdge"));
            Assert.That(GetProperty(GetProperty(preview, "PlayerSpecies"), "Value"), Is.EqualTo("hare"));
            Assert.That(GetProperty(preview, "GridWidth"), Is.EqualTo(36));
            Assert.That(GetProperty(preview, "GridHeight"), Is.EqualTo(20));

            var boardViewModel = desktopRoot.GetComponent("SaltyGame.VM_SimulationBoard");
            Assert.That(boardViewModel, Is.Not.Null);
            var snapshot = GetProperty(boardViewModel, "Snapshot");
            Assert.That(snapshot, Is.Not.Null);
            Assert.That(GetProperty(snapshot, "Width"), Is.EqualTo(36));
            Assert.That(GetProperty(snapshot, "Height"), Is.EqualTo(20));

            Assert.That(GetProperty(simulationViewModel, "HerbivorePopulation"), Is.GreaterThan(0));
            Assert.That(GetProperty(simulationViewModel, "CarnivorePopulation"), Is.GreaterThan(0));

            ((SpeciesSimulationPreview)preview).PauseSimulation();
            var clearEventReaction = simulationViewModel.GetType().GetMethod(
                "ClearEventReaction", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(clearEventReaction, Is.Not.Null);
            clearEventReaction.Invoke(simulationViewModel, null);
            Assert.That(
                GetProperty(simulationViewModel, "EventReactionVisibility").ToString(),
                Is.EqualTo("Collapsed"),
                "The baseline capture should start with the event panel hidden.");
            var host = desktopRoot.GetComponent("SaltyGame.GalapagOSDesktopNoesisHost");
            Assert.That(host, Is.Not.Null);
            var board = host
                .GetType()
                .GetField("simulationBoard", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(host);
            Assert.That(board, Is.Not.Null);
            var presentFoxReaction = simulationViewModel.GetType().GetMethod(
                "PresentFoxHuntReaction",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(presentFoxReaction, Is.Not.Null);
            var setHuntCue = board.GetType().GetMethod("SetFoxHuntCue");
            Assert.That(setHuntCue, Is.Not.Null);
            var boardSnapshot = (SimulationBoardSnapshot)snapshot;
            var foxId = new SpeciesId("fox");
            setHuntCue.Invoke(board, new object[] { 0, 0, -1 });
            yield return null;
            yield return CaptureCamera(outputDirectory, "04-fox-hunt-before", simulationCamera, settleFrames: 1);
            var foxFound = false;
            for (var y = 0; y < boardSnapshot.Height && !foxFound; y++)
            {
                for (var x = 0; x < boardSnapshot.Width; x++)
                {
                    if (boardSnapshot.GetCell(x, y).SpeciesId != foxId)
                    {
                        continue;
                    }

                    setHuntCue.Invoke(board, new object[] { x, y, boardSnapshot.Tick + 1 });
                    foxFound = true;
                    break;
                }
            }

            Assert.That(foxFound, Is.True, "Visual proof needs a Fox on the board.");
            presentFoxReaction.Invoke(simulationViewModel, null);
            Assert.That(GetProperty(simulationViewModel, "EventReactionVisibility").ToString(), Is.EqualTo("Visible"));
            Assert.That(GetProperty(simulationViewModel, "EventReactionFoxVisibility").ToString(), Is.EqualTo("Visible"));
            Assert.That(GetProperty(simulationViewModel, "EventReactionTitleText"), Is.EqualTo("SNEAKY PAWS!"));
            yield return CaptureCamera(outputDirectory, "05-fox-hunt-cue", simulationCamera, settleFrames: 1);

            var setMatingCue = board.GetType().GetMethod("SetMatingCue");
            Assert.That(setMatingCue, Is.Not.Null);
            var presentBirthReaction = simulationViewModel.GetType().GetMethod(
                "PresentBirthReaction",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(presentBirthReaction, Is.Not.Null);
            setMatingCue.Invoke(board, new object[] { 0, 0, 0, 0, 0, 0, -1 });
            yield return null;
            yield return CaptureCamera(outputDirectory, "06-mating-before", simulationCamera, settleFrames: 1);

            var hareId = new SpeciesId("hare");
            var hareFound = false;
            for (var y = 0; y < boardSnapshot.Height && !hareFound; y++)
            {
                for (var x = 0; x < boardSnapshot.Width; x++)
                {
                    if (boardSnapshot.GetCell(x, y).SpeciesId != hareId)
                    {
                        continue;
                    }

                    var mateX = x + 1 < boardSnapshot.Width ? x + 1 : x - 1;
                    var childY = y + 1 < boardSnapshot.Height ? y + 1 : y - 1;
                    setMatingCue.Invoke(board, new object[] { x, y, mateX, y, x, childY, boardSnapshot.Tick + 2 });
                    hareFound = true;
                    break;
                }
            }

            Assert.That(hareFound, Is.True, "Visual proof needs a Hare on the board.");
            presentBirthReaction.Invoke(simulationViewModel, new object[] { 1, false });
            Assert.That(GetProperty(simulationViewModel, "EventReactionVisibility").ToString(), Is.EqualTo("Visible"));
            Assert.That(GetProperty(simulationViewModel, "EventReactionRabbitVisibility").ToString(), Is.EqualTo("Visible"));
            Assert.That(GetProperty(simulationViewModel, "EventReactionTitleText"), Is.EqualTo("NEW LITTLE LIFE!"));
            yield return CaptureCamera(outputDirectory, "07-mating-cue", simulationCamera, settleFrames: 12);
        }

        [UnityTest]
        public IEnumerator JourneyReportReturnsToMapWithAvailableRoutes()
        {
            yield return SceneManager.LoadSceneAsync(SceneName);
            yield return null;
            yield return null;

            var desktopRoot = GameObject.Find(DesktopCameraName);
            var preview = desktopRoot?.GetComponent<SpeciesSimulationPreview>();
            var desktopViewModel = desktopRoot?.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            var simulationViewModel = desktopRoot?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(preview, Is.Not.Null);
            Assert.That(desktopViewModel, Is.Not.Null);
            Assert.That(simulationViewModel, Is.Not.Null);
            Assert.That(preview.TryApplyContinuousPhases(true, "1", out var message), Is.True, message);

            ExecuteCommand(GetProperty(desktopViewModel, "OpenDesktopIconCommand"), "Simulation");
            Assert.That(GetProperty(simulationViewModel, "JourneyMapVisibility").ToString(), Is.EqualTo("Visible"));
            ExecuteCommand(GetProperty(FindJourneyNode(simulationViewModel, "forest-edge-start"), "ChooseCommand"));
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.JourneyNodeReward));
            var popupChoices = (Array)GetProperty(simulationViewModel, "JourneyRewardItems");
            Assert.That(popupChoices.Length, Is.EqualTo(3));
            ExecuteCommand(GetProperty(popupChoices.GetValue(0), "ChooseCommand"));
            ExecuteCommand(GetProperty(FindJourneyNode(simulationViewModel, "first-cycle"), "ChooseCommand"));
            for (var phase = 1; phase <= 5; phase++)
            {
                var deadline = Time.realtimeSinceStartup + 5f;
                while (preview.State != SpeciesPreviewState.PhaseDecision
                       && Time.realtimeSinceStartup < deadline)
                {
                    yield return null;
                }
                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
                ExecuteCommand(GetProperty(simulationViewModel, "ContinueWithoutUpgradeCommand"));
            }

            var boundaryDeadline = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.JourneyDecision
                   && Time.realtimeSinceStartup < boundaryDeadline)
            {
                yield return null;
            }
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.JourneyDecision));
            Assert.That(GetProperty(simulationViewModel, "JourneyMapVisibility").ToString(), Is.EqualTo("Collapsed"));
            Assert.That(GetProperty(simulationViewModel, "ResultsVisibility").ToString(), Is.EqualTo("Visible"));
            var outputDirectory = TryGetVisualOutputDirectory();
            var simulationCamera = GameObject.Find(SimulationCameraName)?.GetComponent<Camera>();
            if (outputDirectory != null && simulationCamera != null)
            {
                yield return CaptureCamera(outputDirectory, "08-cycle-one-report", simulationCamera);
            }
            ExecuteCommand(GetProperty(simulationViewModel, "PlayNextSimulationCommand"));
            Assert.That(GetProperty(simulationViewModel, "JourneyMapVisibility").ToString(), Is.EqualTo("Visible"));
            if (outputDirectory != null && simulationCamera != null)
            {
                yield return CaptureCamera(outputDirectory, "09-cycle-one-route-choice", simulationCamera);
            }
            var noesisView = GameObject.Find(SimulationCameraName)?.GetComponent("NoesisView");
            Assert.That(noesisView, Is.Not.Null);
            var viewContent = GetProperty(noesisView, "Content");
            var mapPanel = viewContent.GetType().GetMethod("FindName")
                ?.Invoke(viewContent, new object[] { "JourneyMapPanel" });
            Assert.That(mapPanel, Is.Not.Null);
            Assert.That(GetProperty(mapPanel, "IsVisible"), Is.EqualTo(true));
            Assert.That(GetProperty(simulationViewModel, "JourneyMapRouteGeometry"), Is.Not.Null);
            var visibleNodes = (Array)GetProperty(simulationViewModel, "JourneyMapNodes");
            Assert.That(visibleNodes.Length, Is.EqualTo(preview.JourneyMap.Nodes.Count));
            var seedfall = FindJourneyNode(simulationViewModel, "seedfall");
            Assert.That(GetProperty(seedfall, "CanChoose"), Is.EqualTo(true));
            ExecuteCommand(GetProperty(seedfall, "HoverCommand"));
            Assert.That(GetProperty(simulationViewModel, "JourneyDetailTitle"), Is.EqualTo(GetProperty(seedfall, "Title")));
            Assert.That(GetProperty(simulationViewModel, "JourneyDetailDescription"), Is.EqualTo(GetProperty(seedfall, "Description")));
            ExecuteCommand(GetProperty(seedfall, "ChooseCommand"));
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.JourneyNodeReward));
            Assert.That(GetProperty(simulationViewModel, "JourneyNodePopupVisibility").ToString(), Is.EqualTo("Visible"));
            if (outputDirectory != null && simulationCamera != null)
            {
                yield return CaptureCamera(outputDirectory, "10-habitat-condition-popup", simulationCamera);
            }
            var conditionChoices = (Array)GetProperty(simulationViewModel, "JourneyRewardItems");
            Assert.That(conditionChoices.Length, Is.EqualTo(1));
            ExecuteCommand(GetProperty(conditionChoices.GetValue(0), "ChooseCommand"));
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.JourneyDecision));
            Assert.That(GetProperty(simulationViewModel, "JourneyMapVisibility").ToString(), Is.EqualTo("Visible"));
            var secondSimulation = FindJourneyNode(simulationViewModel, "burrow-study");
            Assert.That(GetProperty(secondSimulation, "CanChoose"), Is.EqualTo(true));
            if (outputDirectory != null && simulationCamera != null)
            {
                yield return CaptureCamera(outputDirectory, "11-next-simulation-available", simulationCamera);
            }
            ExecuteCommand(GetProperty(secondSimulation, "ChooseCommand"));
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(GetProperty(simulationViewModel, "JourneyMapVisibility").ToString(), Is.EqualTo("Collapsed"));
        }

        [UnityTest]
        public IEnumerator ReopeningSimulationAfterResultsStartsANewRun()
        {
            yield return SceneManager.LoadSceneAsync(SceneName);
            yield return null;
            yield return null;

            var desktopRoot = GameObject.Find(DesktopCameraName);
            Assert.That(desktopRoot, Is.Not.Null);
            var desktopViewModel = desktopRoot.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            var openCommand = GetProperty(desktopViewModel, "OpenDesktopIconCommand");
            var preview = desktopRoot.GetComponent<SpeciesSimulationPreview>();
            var simulationViewModel = desktopRoot.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(preview, Is.Not.Null);
            Assert.That(simulationViewModel, Is.Not.Null);

            Assert.That(preview.TryApplyContinuousPhases(true, "1", out var phaseMessage), Is.True, phaseMessage);
            ExecuteCommand(openCommand, "Simulation");
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Ready));
            ExecuteCommand(GetProperty(FindJourneyNode(simulationViewModel, "forest-edge-start"), "ChooseCommand"));
            var rewards = (Array)GetProperty(simulationViewModel, "JourneyRewardItems");
            ExecuteCommand(GetProperty(rewards.GetValue(0), "ChooseCommand"));
            ExecuteCommand(GetProperty(FindJourneyNode(simulationViewModel, "first-cycle"), "ChooseCommand"));
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));

            var completedRun = preview.Run;
            preview.EndSimulation();
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
            yield return null;
            ExecuteCommand(GetProperty(simulationViewModel, "ReturnToLabCommand"));

            ExecuteCommand(openCommand, "Simulation");
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Ready));
            Assert.That(preview.Run, Is.Not.SameAs(completedRun));
            ExecuteCommand(GetProperty(FindJourneyNode(simulationViewModel, "forest-edge-start"), "ChooseCommand"));
            var secondRewards = (Array)GetProperty(simulationViewModel, "JourneyRewardItems");
            ExecuteCommand(GetProperty(secondRewards.GetValue(0), "ChooseCommand"));
            ExecuteCommand(GetProperty(FindJourneyNode(simulationViewModel, "first-cycle"), "ChooseCommand"));
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            var reopenedRun = preview.Run;
            ExecuteCommand(openCommand, "Simulation");
            Assert.That(preview.Run, Is.SameAs(reopenedRun), "Opening an already running simulation must not start another run.");
            Assert.That(preview.Progression.PurchasedUpgradeCount, Is.EqualTo(1));
            Assert.That(preview.Progression.Currency, Is.Zero);
            yield return null;
            Assert.That(GetProperty(simulationViewModel, "ResultsVisibility").ToString(), Is.EqualTo("Collapsed"));

            preview.EndSimulation();
            yield return null;
            ExecuteCommand(GetProperty(simulationViewModel, "ReturnToLabCommand"));
        }

        static object GetProperty(object target, string propertyName)
        {
            var property = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            Assert.That(property, Is.Not.Null, $"Expected property '{propertyName}'.");
            return property.GetValue(target);
        }

        static void ExecuteCommand(object command, params object[] arguments)
        {
            Assert.That(command, Is.Not.Null);
            var parameters = arguments.Length == 0 ? new object[] { null } : arguments;
            Assert.That((bool)command.GetType().GetMethod("CanExecute")?.Invoke(command, parameters), Is.True);
            command.GetType().GetMethod("Execute")?.Invoke(command, parameters);
        }

        static object FindJourneyNode(object simulationViewModel, string nodeId)
        {
            foreach (var node in (IEnumerable)GetProperty(simulationViewModel, "JourneyMapNodes"))
            {
                if (string.Equals((string)GetProperty(node, "NodeId"), nodeId, StringComparison.Ordinal))
                {
                    return node;
                }
            }

            Assert.Fail($"Journey node '{nodeId}' was not rendered.");
            return null;
        }

        static void ResetJourneyHover(object simulationViewModel, string startNodeId)
        {
            foreach (var node in (IEnumerable)GetProperty(simulationViewModel, "JourneyMapNodes"))
            {
                ExecuteCommand(GetProperty(node, "LeaveCommand"));
                Assert.That(GetProperty(node, "LabelVisibility").ToString(), Is.EqualTo("Collapsed"));
            }

            var start = FindJourneyNode(simulationViewModel, startNodeId);
            ExecuteCommand(GetProperty(start, "HoverCommand"));
            ExecuteCommand(GetProperty(start, "LeaveCommand"));
        }

        static string TryGetVisualOutputDirectory()
        {
            var configuredPath = VisualTestConfiguration.GetValue("CELLSIM_VISUAL_OUTPUT");
            if (string.IsNullOrWhiteSpace(configuredPath))
            {
                return null;
            }

            var projectRoot = Directory.GetParent(Application.dataPath).FullName;
            var directory = Path.IsPathRooted(configuredPath)
                ? configuredPath
                : Path.Combine(projectRoot, configuredPath);
            Directory.CreateDirectory(directory);
            return directory;
        }

        static IEnumerator CaptureCamera(string directory, string name, Camera camera, int settleFrames = 10)
        {
            for (var frame = 0; frame < settleFrames; frame++)
            {
                yield return null;
            }

            Assert.That(camera, Is.Not.Null, $"Visual evidence requires camera '{name}'.");
            var width = GetVisualDimension("CELLSIM_VISUAL_WIDTH", 1280);
            var height = GetVisualDimension("CELLSIM_VISUAL_HEIGHT", 720);
            var path = Path.Combine(directory, name + ".png");
            var previousTarget = camera.targetTexture;
            var previousActive = RenderTexture.active;
            var renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            var image = new Texture2D(width, height, TextureFormat.RGBA32, false);
            try
            {
                renderTexture.Create();
                camera.targetTexture = renderTexture;
                camera.Render();
                RenderTexture.active = renderTexture;
                image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                image.Apply();
                File.WriteAllBytes(path, image.EncodeToPNG());
            }
            finally
            {
                camera.targetTexture = previousTarget;
                RenderTexture.active = previousActive;
                UnityEngine.Object.Destroy(image);
                UnityEngine.Object.Destroy(renderTexture);
            }

            Assert.That(File.Exists(path), Is.True, $"Unity did not write screenshot '{path}'.");
        }

        static int GetVisualDimension(string environmentVariable, int fallback)
        {
            return int.TryParse(VisualTestConfiguration.GetValue(environmentVariable), out var value)
                ? Mathf.Max(1, value)
                : fallback;
        }
    }
}
