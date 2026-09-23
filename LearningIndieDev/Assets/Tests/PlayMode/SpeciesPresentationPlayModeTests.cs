using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.TestTools;

namespace SaltyGame.PlayModeTests
{
    [Category("Graphics")]
    public sealed class SpeciesPresentationPlayModeTests
    {
        [UnityTest]
        public IEnumerator CellularPrototypeUsesSerializedComposition()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var root = GameObject.Find("Cellular Automata Prototype");
            var camera = GameObject.Find("Prototype Camera");
            Assert.That(root, Is.Not.Null);
            Assert.That(camera, Is.Not.Null);

            Assert.That(root.GetComponent("SaltyGame.CellularAutomataPrototypeRuntime"), Is.Not.Null);
            Assert.That(root.GetComponent("SaltyGame.SpeciesSimulationNoesisHost"), Is.Not.Null);
            Assert.That(root.GetComponent("SaltyGame.Helper_Simulation"), Is.Not.Null);
            Assert.That(root.GetComponent("SaltyGame.SpeciesSimulationPreview"), Is.Not.Null);
            Assert.That(camera.GetComponent<Camera>(), Is.Not.Null);
            Assert.That(camera.GetComponent("NoesisView"), Is.Not.Null);
            var viewModel = camera.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(camera.GetComponent("SaltyGame.VM_SimulationBoard"), Is.Not.Null);

            var canStart = viewModel.GetType().GetProperty("CanStart");
            Assert.That(canStart, Is.Not.Null);
            var preview = root.GetComponent<CellularAutomataPrototypeRuntime>().SpeciesPreview;
            Assert.That(preview.Run.Status, Is.EqualTo(SimulationRunStatus.Running));
            Assert.That(preview.SelectedScenario.name, Is.EqualTo("ForestEdge"));
            Assert.That(preview.PlayerSpecies.Value, Is.EqualTo("hare"));
            Assert.That(canStart.GetValue(viewModel), Is.False);
        }

        [UnityTest]
        public IEnumerator SimulationZoomCommandsChangeTheLiveBoardScale()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var camera = GameObject.Find("Prototype Camera");
            var viewModel = camera?.GetComponent("SaltyGame.VM_SimulationShell");
            var host = GameObject.Find("Cellular Automata Prototype")
                ?.GetComponent("SaltyGame.SpeciesSimulationNoesisHost");
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(host, Is.Not.Null);

            var viewModelType = viewModel.GetType();
            var boardField = host.GetType().GetField("simulationBoard", BindingFlags.Instance | BindingFlags.NonPublic);
            var board = boardField?.GetValue(host);
            Assert.That(board, Is.Not.Null);

            var zoomInCommand = viewModelType.GetProperty("ZoomInCommand")?.GetValue(viewModel);
            zoomInCommand?.GetType().GetMethod("Execute")?.Invoke(zoomInCommand, new object[] { null });
            yield return null;

            Assert.That(viewModelType.GetProperty("BoardZoom")?.GetValue(viewModel), Is.EqualTo(1.25f));
            Assert.That(board.GetType().GetProperty("Zoom")?.GetValue(board), Is.EqualTo(1.25f));

            var resetZoomCommand = viewModelType.GetProperty("ResetZoomCommand")?.GetValue(viewModel);
            resetZoomCommand?.GetType().GetMethod("Execute")?.Invoke(resetZoomCommand, new object[] { null });
            yield return null;

            Assert.That(viewModelType.GetProperty("BoardZoom")?.GetValue(viewModel), Is.EqualTo(1f));
            Assert.That(board.GetType().GetProperty("Zoom")?.GetValue(board), Is.EqualTo(1f));
        }

        [UnityTest]
        public IEnumerator CellularPrototypeInitializesEveryAuthoredAnimalSprite()
        {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
            {
                Assert.Ignore("Animal sprite initialization requires a graphics-capable Unity player.");
            }

            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;
            yield return null;

            var camera = GameObject.Find("Prototype Camera");
            var viewModel = camera?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(viewModel, Is.Not.Null, "CellularAutomataPrototype must initialize VM_SimulationShell.");

            var sprites = viewModel.GetType()
                .GetField("animalSprites", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(viewModel) as Array;
            Assert.That(sprites, Is.Not.Null, "The animal atlas must produce presentation sprites at runtime.");
            Assert.That(sprites.Length, Is.EqualTo(8), "The authored animal roster contains eight presentation slots.");

            for (var index = 0; index < sprites.Length; index++)
            {
                Assert.That(sprites.GetValue(index), Is.Not.Null, $"Animal presentation slot {index} must be initialized.");
            }
        }

        [UnityTest]
        public IEnumerator CellularPrototypeInitializesEveryAuthoredGrassTerrainSprite()
        {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
            {
                Assert.Ignore("Terrain sprite initialization requires a graphics-capable Unity player.");
            }

            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;
            yield return null;

            var camera = GameObject.Find("Prototype Camera");
            var viewModel = camera?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(viewModel, Is.Not.Null, "CellularAutomataPrototype must initialize VM_SimulationShell.");

            var grassTiles = viewModel.GetType()
                .GetField("grassTerrainTiles", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(viewModel) as Array;
            Assert.That(grassTiles, Is.Not.Null, "The terrain atlas must produce Grass presentation sprites at runtime.");
            Assert.That(grassTiles.Length, Is.EqualTo(256), "Grass presentation uses direct eight-bit mask slots.");

            foreach (var mask in TerrainTileResolver.AllValidMasks)
            {
                Assert.That(grassTiles.GetValue(mask), Is.Not.Null, $"Grass mask {mask:D3} must be initialized.");
            }

            var boardViewModel = camera.GetComponent("SaltyGame.VM_SimulationBoard");
            Assert.That(boardViewModel, Is.Not.Null);
            var snapshot = boardViewModel.GetType()
                .GetProperty("Snapshot", BindingFlags.Instance | BindingFlags.Public)
                ?.GetValue(boardViewModel) as SimulationBoardSnapshot;
            Assert.That(snapshot, Is.Not.Null, "The prototype scene must provide a terrain snapshot.");
            var grassCellCount = 0;
            foreach (var cell in snapshot.Cells)
            {
                if (cell.TerrainId == TerrainIds.Grass)
                {
                    grassCellCount++;
                }
            }

            Assert.That(grassCellCount, Is.GreaterThan(0), "ForestEdge must place Grass cells for the renderer to tile.");

            var desertTiles = viewModel.GetType()
                .GetField("desertTerrainTiles", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(viewModel) as Array;
            Assert.That(desertTiles, Is.Not.Null, "Missing Desert art must not disable the terrain atlas family slots.");
        }

        [UnityTest]
        public IEnumerator DefaultFeaturesShowHerbivoreStatLineAfterRun()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var runtime = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>();
            Assert.That(runtime, Is.Not.Null);
            runtime.SpeciesPreview.StopSimulation();
            Assert.That(runtime.SpeciesPreview.BevExperimentalFeaturesEnabled, Is.True);
            Assert.That(runtime.SpeciesPreview.FoxAttackCooldownTicks, Is.EqualTo(0));

            var continuousSettingsApplied = runtime.SpeciesPreview.TryApplyContinuousPhases(
                enabled: false,
                phaseLengthValue: string.Empty,
                out var continuousMessage);
            Assert.That(continuousSettingsApplied, Is.True, continuousMessage);

            var applied = runtime.SpeciesPreview.TryApplyExperimentalFeatures(
                true,
                "2",
                out var message);

            Assert.That(applied, Is.True, message);
            Assert.That(runtime.SpeciesPreview.BevExperimentalFeaturesEnabled, Is.True);
            Assert.That(runtime.SpeciesPreview.FoxAttackCooldownTicks, Is.EqualTo(2));
            StringAssert.Contains("species stat lines", message);

            var settingsApplied = runtime.SpeciesPreview.TryApplyGlobalSettings(
                "8",
                "8",
                runtime.SpeciesPreview.BaseSeed.ToString(CultureInfo.InvariantCulture),
                runtime.SpeciesPreview.MaximumPopulation.ToString(CultureInfo.InvariantCulture),
                runtime.SpeciesPreview.MinimumPopulation.ToString(CultureInfo.InvariantCulture),
                "0.05",
                "0.01",
                runtime.SpeciesPreview.PlantProbability.ToString(CultureInfo.InvariantCulture),
                runtime.SpeciesPreview.HerbivoreProbability.ToString(CultureInfo.InvariantCulture),
                runtime.SpeciesPreview.CarnivoreProbability.ToString(CultureInfo.InvariantCulture),
                randomizeSeed: false,
                out var settingsMessage);
            Assert.That(settingsApplied, Is.True, settingsMessage);

            runtime.SpeciesPreview.StartSimulation();
            var timeout = Time.realtimeSinceStartup + 5f;
            while (runtime.SpeciesPreview.State != SpeciesPreviewState.Rewards
                   && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(runtime.SpeciesPreview.State, Is.EqualTo(SpeciesPreviewState.Rewards));
            var viewModel = GameObject.Find("Prototype Camera")
                ?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(viewModel, Is.Not.Null);
            var summary = viewModel.GetType()
                .GetProperty("ExperimentalHerbivoreStatLineSummary")
                ?.GetValue(viewModel) as string;
            var summaryVisibility = viewModel.GetType()
                .GetProperty("ExperimentalHerbivoreStatLineSummaryVisibility")
                ?.GetValue(viewModel)
                ?.ToString();
            var expectedStartingPopulation = runtime.SpeciesPreview.Run.PopulationHistory[0]
                .GetCount(runtime.SpeciesPreview.PlayerSpecies);
            StringAssert.Contains($"SPO: {expectedStartingPopulation}", summary);
            StringAssert.Contains("SPO:", summary);
            StringAssert.Contains("HPS:", summary);
            StringAssert.Contains("EHS:", summary);
            StringAssert.Contains("eAVI:", summary);
            StringAssert.Contains("predAVG:", summary);
            StringAssert.Contains("APS:", summary);
            Assert.That(summaryVisibility, Is.EqualTo("Visible"));
            Assert.That(runtime.SpeciesPreview.RewardOptionCount, Is.EqualTo(2));
            Assert.That(runtime.SpeciesPreview.GetRewardOptionId(0), Is.Not.Empty);
            Assert.That(
                runtime.SpeciesPreview.GetRewardOptionId(1),
                Is.Not.EqualTo(runtime.SpeciesPreview.GetRewardOptionId(0)));
            var thirdRewardVisibility = viewModel.GetType()
                .GetProperty("RewardOption3Visibility")
                ?.GetValue(viewModel)
                ?.ToString();
            Assert.That(thirdRewardVisibility, Is.EqualTo("Collapsed"));

            runtime.SpeciesPreview.ContinueWithoutUpgrade();
            yield return null;
        }

        [UnityTest]
        public IEnumerator DeveloperSettingsCanSetExactStartingPopulations()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
            preview.StopSimulation();
            var viewModel = GameObject.Find("Prototype Camera")
                ?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(viewModel, Is.Not.Null);
            var viewModelType = viewModel.GetType();
            viewModelType.GetProperty("DeveloperMode")?.SetValue(viewModel, true);
            viewModelType.GetProperty("PlantStartingPopulationText")?.SetValue(viewModel, "3");
            viewModelType.GetProperty("HerbivoreStartingPopulationText")?.SetValue(viewModel, "2");
            viewModelType.GetProperty("CarnivoreStartingPopulationText")?.SetValue(viewModel, "1");
            var applySettingsCommand = viewModelType.GetProperty("ApplySettingsCommand")?.GetValue(viewModel);
            applySettingsCommand?.GetType().GetMethod("Execute")?.Invoke(applySettingsCommand, new object[] { null });

            Assert.That(preview.PlantStartingPopulation, Is.EqualTo(3));
            Assert.That(preview.HerbivoreStartingPopulation, Is.EqualTo(2));
            Assert.That(preview.CarnivoreStartingPopulation, Is.EqualTo(1));
            var settingsMessage = viewModelType.GetProperty("SettingsMessage")?.GetValue(viewModel) as string;
            StringAssert.Contains("Starting populations: plant 3, herbivore 2, carnivore 1", settingsMessage);

            preview.ResetToStart();
            preview.StartSimulation();
            var openingPopulation = preview.Run.PopulationHistory[0];
            Assert.That(openingPopulation.GetCount(SpeciesIds.Plant), Is.EqualTo(3));
            Assert.That(openingPopulation.GetCount(FindSpeciesId(preview, SpeciesRole.Herbivore)), Is.EqualTo(2));
            Assert.That(openingPopulation.GetCount(FindSpeciesId(preview, SpeciesRole.Carnivore)), Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator DeveloperSettingsRejectingPopulationCapPreservesInputAndRunSeed()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var runtime = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>();
            var viewModel = GameObject.Find("Prototype Camera")
                ?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(runtime, Is.Not.Null);
            Assert.That(viewModel, Is.Not.Null);
            runtime.SpeciesPreview.StopSimulation();

            var viewModelType = viewModel.GetType();
            viewModelType.GetProperty("DeveloperMode")?.SetValue(viewModel, true);
            viewModelType.GetProperty("MaximumPopulationText")?.SetValue(viewModel, "100");
            viewModelType.GetProperty("PlantStartingPopulationText")?.SetValue(viewModel, "40");
            viewModelType.GetProperty("HerbivoreStartingPopulationText")?.SetValue(viewModel, "40");
            viewModelType.GetProperty("CarnivoreStartingPopulationText")?.SetValue(viewModel, "40");

            var runBeforeInvalidSettings = runtime.SpeciesPreview.Run;
            var runSeedBefore = runBeforeInvalidSettings.Seed;
            var applySettingsCommand = viewModelType.GetProperty("ApplySettingsCommand")?.GetValue(viewModel);
            applySettingsCommand?.GetType().GetMethod("Execute")?.Invoke(applySettingsCommand, new object[] { null });

            var settingsMessage = viewModelType.GetProperty("SettingsMessage")?.GetValue(viewModel) as string;
            StringAssert.Contains("total 120 cannot exceed maximum population 100", settingsMessage);
            Assert.That(runtime.SpeciesPreview.Run, Is.SameAs(runBeforeInvalidSettings));
            Assert.That(runtime.SpeciesPreview.Run.Seed, Is.EqualTo(runSeedBefore));
            Assert.That(viewModelType.GetProperty("PlantStartingPopulationText")?.GetValue(viewModel), Is.EqualTo("40"));
            Assert.That(viewModelType.GetProperty("HerbivoreStartingPopulationText")?.GetValue(viewModel), Is.EqualTo("40"));
            Assert.That(viewModelType.GetProperty("CarnivoreStartingPopulationText")?.GetValue(viewModel), Is.EqualTo("40"));
        }

        static SpeciesId FindSpeciesId(SpeciesSimulationPreview preview, SpeciesRole role)
        {
            foreach (var entry in preview.ActiveSpeciesRules)
            {
                if (entry.Value.Role == role)
                {
                    return entry.Key;
                }
            }

            Assert.Fail($"No species with role {role} is active.");
            return default;
        }

        [UnityTest]
        public IEnumerator ContinuousPhaseDecisionResumesTheSamePreviewRun()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var runtime = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>();
            Assert.That(runtime, Is.Not.Null);

            var preview = runtime.SpeciesPreview;
            preview.StopSimulation();
            var viewModel = GameObject.Find("Prototype Camera")
                ?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(viewModel, Is.Not.Null);
            var phaseSettingsApplied = preview.TryApplyContinuousPhases(
                enabled: true,
                phaseLengthValue: "2",
                out var phaseMessage);
            Assert.That(phaseSettingsApplied, Is.True, phaseMessage);

            var settingsApplied = preview.TryApplyGlobalSettingsForTicks(
                "8",
                "8",
                preview.BaseSeed.ToString(CultureInfo.InvariantCulture),
                preview.MaximumPopulation.ToString(CultureInfo.InvariantCulture),
                preview.MinimumPopulation.ToString(CultureInfo.InvariantCulture),
                "4",
                "0.01",
                preview.PlantProbability.ToString(CultureInfo.InvariantCulture),
                preview.HerbivoreProbability.ToString(CultureInfo.InvariantCulture),
                preview.CarnivoreProbability.ToString(CultureInfo.InvariantCulture),
                randomizeSeed: false,
                out var settingsMessage);
            Assert.That(settingsApplied, Is.True, settingsMessage);

            preview.StartSimulation();
            var run = preview.Run;

            // Pausing changes only simulation progression. The same shell and
            // board remain the active presentation while the player decides
            // when to resume.
            preview.PauseSimulation();
            yield return null;

            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Paused));
            Assert.That(preview.Run, Is.SameAs(run));
            Assert.That(
                viewModel.GetType().GetProperty("BoardVisibility")?.GetValue(viewModel)?.ToString(),
                Is.EqualTo("Visible"));
            Assert.That(
                viewModel.GetType().GetProperty("CanCloseWindow")?.GetValue(viewModel),
                Is.EqualTo(false));

            preview.ResumeSimulation();

            var timeout = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.PhaseDecision
                   && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
            Assert.That(preview.Run, Is.SameAs(run));
            Assert.That(run.Tick, Is.EqualTo(2));
            Assert.That(run.TargetTicks, Is.EqualTo(2 * SpeciesSimulationPreview.ContinuousExpeditionPhaseCount));
            Assert.That(run.Status, Is.EqualTo(SimulationRunStatus.AwaitingDecision));

            Assert.That(
                viewModel.GetType().GetProperty("PhaseDecisionVisibility")?.GetValue(viewModel)?.ToString(),
                Is.EqualTo("Visible"));
            Assert.That(
                viewModel.GetType().GetProperty("BoardVisibility")?.GetValue(viewModel)?.ToString(),
                Is.EqualTo("Visible"));
            Assert.That(
                viewModel.GetType().GetProperty("CanCloseWindow")?.GetValue(viewModel),
                Is.EqualTo(false));

            preview.ContinueWithoutUpgrade();
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(preview.Run, Is.SameAs(run));

            for (var phase = 2; phase < SpeciesSimulationPreview.ContinuousExpeditionPhaseCount; phase++)
            {
                timeout = Time.realtimeSinceStartup + 5f;
                while (preview.State != SpeciesPreviewState.PhaseDecision
                       && Time.realtimeSinceStartup < timeout)
                {
                    yield return null;
                }

                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
                Assert.That(run.Tick, Is.EqualTo(phase * 2));
                preview.ContinueWithoutUpgrade();
                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
                Assert.That(preview.Run, Is.SameAs(run));
            }

            timeout = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.Results
                   && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
            Assert.That(preview.Run, Is.SameAs(run));
            Assert.That(run.Tick, Is.EqualTo(2 * SpeciesSimulationPreview.ContinuousExpeditionPhaseCount));
            Assert.That(run.Status, Is.EqualTo(SimulationRunStatus.Complete));
            Assert.That(
                viewModel.GetType().GetProperty("ResultsTitleText")?.GetValue(viewModel)?.ToString(),
                Is.EqualTo("Expedition complete"));
            Assert.That(
                viewModel.GetType().GetProperty("PlayNextSimulationText")?.GetValue(viewModel)?.ToString(),
                Is.EqualTo("START NEW EXPEDITION"));
            Assert.That(preview.LastExpeditionFailed, Is.False);
            var earnedCurrency = preview.Progression.Currency;
            Assert.That(earnedCurrency, Is.GreaterThan(0));
            var completedRun = preview.Run;
            preview.PlayNextSimulation();
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(preview.Run, Is.Not.SameAs(completedRun));
            Assert.That(preview.Progression.Currency, Is.EqualTo(earnedCurrency));
            Assert.That(preview.Progression.PurchasedUpgradeCount, Is.Zero);
        }

        [UnityTest]
        public IEnumerator ResultsActionsStartTheNextExpedition()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
            var viewModel = GameObject.Find("Prototype Camera")
                ?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(preview, Is.Not.Null);
            Assert.That(viewModel, Is.Not.Null);

            // The scene host auto-starts on load; reset to the editable Ready
            // state before configuring this focused action test.
            preview.ResetToStart();
            Assert.That(preview.TryApplyContinuousPhases(true, "1", out var phaseMessage), Is.True, phaseMessage);
            Assert.That(preview.TryApplyGlobalSettingsForTicksWithStartingPopulations(
                "8",
                "8",
                preview.BaseSeed.ToString(CultureInfo.InvariantCulture),
                preview.MaximumPopulation.ToString(CultureInfo.InvariantCulture),
                preview.MinimumPopulation.ToString(CultureInfo.InvariantCulture),
                "10",
                "0.01",
                preview.PlantProbability.ToString(CultureInfo.InvariantCulture),
                preview.HerbivoreProbability.ToString(CultureInfo.InvariantCulture),
                preview.CarnivoreProbability.ToString(CultureInfo.InvariantCulture),
                randomizeSeed: false,
                "4",
                "2",
                "1",
                out var settingsMessage), Is.True, settingsMessage);

            preview.StartSimulation();
            var abandonedRun = preview.Run;
            preview.Progression.AddCurrency(20);
            Assert.That(preview.Progression.TryPurchase(SpeciesUpgradeCatalog.Create("tough-hide")), Is.True);
            preview.EndSimulation();
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
            Assert.That(preview.LastRunEndedEarly, Is.True);
            Assert.That(preview.Progression.Currency, Is.Zero);

            var playNextCommand = viewModel.GetType().GetProperty("PlayNextSimulationCommand")?.GetValue(viewModel);
            playNextCommand?.GetType().GetMethod("Execute")?.Invoke(playNextCommand, new object[] { null });
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(preview.Run.Status, Is.EqualTo(SimulationRunStatus.Running));
            Assert.That(preview.Run, Is.Not.SameAs(abandonedRun));
            Assert.That(preview.Progression.PurchasedUpgradeCount, Is.Zero);
            Assert.That(preview.Progression.Currency, Is.Zero);

            preview.EndSimulation();
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));

            var resetCommand = viewModel.GetType().GetProperty("ResetCommand")?.GetValue(viewModel);
            resetCommand?.GetType().GetMethod("Execute")?.Invoke(resetCommand, new object[] { null });
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(preview.Run.Status, Is.EqualTo(SimulationRunStatus.Running));
        }

        [UnityTest]
        public IEnumerator EndConfirmationCanCancelRunningAndForfeitAtThePhaseBoundary()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var runtime = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>();
            var preview = runtime.SpeciesPreview;
            var viewModel = GameObject.Find("Prototype Camera").GetComponent("SaltyGame.VM_SimulationShell");
            var viewModelType = viewModel.GetType();
            var endCommand = viewModelType.GetProperty("EndCommand")?.GetValue(viewModel);
            var confirmEndCommand = viewModelType.GetProperty("ConfirmEndCommand")?.GetValue(viewModel);
            var cancelEndConfirmationCommand = viewModelType.GetProperty("CancelEndConfirmationCommand")?.GetValue(viewModel);
            preview.StopSimulation();
            Assert.That(preview.TryApplyContinuousPhases(true, "2", out var phaseMessage), Is.True, phaseMessage);
            preview.StartSimulation();
            yield return null;
            Assert.That(
                endCommand.GetType().GetMethod("CanExecute")?.Invoke(endCommand, new object[] { null }),
                Is.EqualTo(true));
            var run = preview.Run;
            var tickBeforeConfirmation = run.Tick;

            confirmEndCommand.GetType().GetMethod("Execute")?.Invoke(confirmEndCommand, new object[] { null });
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(preview.Run, Is.SameAs(run));
            endCommand.GetType().GetMethod("Execute")?.Invoke(endCommand, new object[] { null });
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Paused));
            Assert.That(viewModelType.GetProperty("EndConfirmationVisibility")?.GetValue(viewModel)?.ToString(), Is.EqualTo("Visible"));
            Assert.That(viewModelType.GetProperty("CanCloseWindow")?.GetValue(viewModel), Is.False);
            cancelEndConfirmationCommand.GetType().GetMethod("Execute")?.Invoke(cancelEndConfirmationCommand, new object[] { null });
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(preview.Run, Is.SameAs(run));
            Assert.That(run.Tick, Is.EqualTo(tickBeforeConfirmation));

            preview.PauseSimulation();
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Paused));
            var pausedTick = run.Tick;
            endCommand.GetType().GetMethod("Execute")?.Invoke(endCommand, new object[] { null });
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Paused));
            Assert.That(viewModelType.GetProperty("EndConfirmationVisibility")?.GetValue(viewModel)?.ToString(), Is.EqualTo("Visible"));
            cancelEndConfirmationCommand.GetType().GetMethod("Execute")?.Invoke(cancelEndConfirmationCommand, new object[] { null });
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Paused));
            Assert.That(preview.Run, Is.SameAs(run));
            Assert.That(run.Tick, Is.EqualTo(pausedTick));
            preview.ResumeSimulation();
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));

            var timeout = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.PhaseDecision
                   && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
            var decisionTick = run.Tick;
            preview.Progression.AddCurrency(7);
            var dataAtDecision = preview.Progression.Currency;
            endCommand.GetType().GetMethod("Execute")?.Invoke(endCommand, new object[] { null });
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
            Assert.That(viewModelType.GetProperty("EndConfirmationVisibility")?.GetValue(viewModel)?.ToString(), Is.EqualTo("Visible"));
            cancelEndConfirmationCommand.GetType().GetMethod("Execute")?.Invoke(cancelEndConfirmationCommand, new object[] { null });
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
            Assert.That(preview.Run, Is.SameAs(run));
            Assert.That(run.Tick, Is.EqualTo(decisionTick));
            Assert.That(preview.Progression.Currency, Is.EqualTo(dataAtDecision));

            endCommand.GetType().GetMethod("Execute")?.Invoke(endCommand, new object[] { null });
            confirmEndCommand.GetType().GetMethod("Execute")?.Invoke(confirmEndCommand, new object[] { null });

            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
            Assert.That(preview.Run, Is.SameAs(run));
            Assert.That(run.Tick, Is.EqualTo(decisionTick));
            Assert.That(preview.LastRunEndedEarly, Is.True);
            Assert.That(preview.LastExpeditionFailed, Is.False);
            Assert.That(preview.Progression.Currency, Is.Zero);
            Assert.That(viewModelType.GetProperty("EndConfirmationVisibility")?.GetValue(viewModel)?.ToString(), Is.EqualTo("Collapsed"));
            Assert.That(viewModelType.GetProperty("ResultsTitleText")?.GetValue(viewModel), Is.EqualTo("Expedition ended"));
        }

        [UnityTest]
        public IEnumerator PlayerSpeciesExtinctionFailsImmediatelyWithoutRewards()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
            preview.StopSimulation();
            Assert.That(preview.TryApplyContinuousPhases(true, "2", out var phaseMessage), Is.True, phaseMessage);

            var playerRole = preview.ActiveSpeciesRules[preview.PlayerSpecies].Role;
            var herbivores = playerRole == SpeciesRole.Herbivore ? 0 : 1;
            var carnivores = playerRole == SpeciesRole.Carnivore ? 0 : 1;
            Assert.That(
                playerRole == SpeciesRole.Herbivore || playerRole == SpeciesRole.Carnivore,
                Is.True);
            Assert.That(preview.TryApplyGlobalSettingsForTicksWithStartingPopulations(
                "8",
                "8",
                preview.BaseSeed.ToString(CultureInfo.InvariantCulture),
                preview.MaximumPopulation.ToString(CultureInfo.InvariantCulture),
                preview.MinimumPopulation.ToString(CultureInfo.InvariantCulture),
                "20",
                "0.01",
                "0",
                "0",
                "0",
                randomizeSeed: false,
                "0",
                herbivores.ToString(CultureInfo.InvariantCulture),
                carnivores.ToString(CultureInfo.InvariantCulture),
                out var settingsMessage), Is.True, settingsMessage);

            preview.StartSimulation();
            preview.Progression.AddCurrency(9);
            var timeout = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.Results && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
            Assert.That(preview.Run.Tick, Is.EqualTo(1));
            Assert.That(preview.LastExpeditionFailed, Is.True);
            Assert.That(preview.LastRunEndedEarly, Is.False);
            Assert.That(preview.Progression.Currency, Is.Zero);
            yield return null;
            var viewModel = GameObject.Find("Prototype Camera").GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(
                viewModel.GetType().GetProperty("ResultsTitleText")?.GetValue(viewModel),
                Is.EqualTo("Expedition failed"));
        }

        [UnityTest]
        public IEnumerator PhaseDecisionCanPurchaseAuthoredUpgradeAndResumeSameRun()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var runtime = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>();
            Assert.That(runtime, Is.Not.Null);

            var preview = runtime.SpeciesPreview;
            preview.StopSimulation();
            Assert.That(preview.TryApplyContinuousPhases(true, "2", out var phaseMessage), Is.True, phaseMessage);
            Assert.That(preview.TryApplyGlobalSettingsForTicks(
                "8",
                "8",
                "0",
                preview.MaximumPopulation.ToString(CultureInfo.InvariantCulture),
                preview.MinimumPopulation.ToString(CultureInfo.InvariantCulture),
                "4",
                "0.01",
                "0",
                "1",
                "0",
                randomizeSeed: false,
                out var settingsMessage), Is.True, settingsMessage);

            preview.StartSimulation();
            var run = preview.Run;
            // Give the test a deterministic purchase budget after Start has
            // created the session progression. The boundary still adds its
            // survivor-based phase reward independently.
            preview.Progression.AddCurrency(10);

            var timeout = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.PhaseDecision
                   && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
            Assert.That(preview.GetRewardOptionId(0), Is.EqualTo("tough-hide"));
            StringAssert.Contains("TOUGH HIDE", preview.GetRewardOptionDisplayName(0));
            StringAssert.Contains("Block Amount", preview.GetRewardOptionDisplayName(0));
            StringAssert.DoesNotContain("TRAILBLAZER", preview.GetRewardOptionDisplayName(0));
            Assert.That(preview.CanPurchaseReward(0), Is.True);
            var currencyAtBoundary = preview.Progression.Currency;
            var blockBefore = preview.ActiveSpeciesRules[preview.PlayerSpecies].BlockAmount;

            Assert.That(preview.PurchaseReward(0), Is.True);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(preview.Run, Is.SameAs(run));
            Assert.That(run.Status, Is.EqualTo(SimulationRunStatus.Running));
            Assert.That(run.Tick, Is.EqualTo(2));
            Assert.That(preview.Progression.PurchasedUpgradeCount, Is.EqualTo(1));
            Assert.That(run.UpgradeLoadout.Count, Is.EqualTo(1));
            Assert.That(
                preview.ActiveSpeciesRules[preview.PlayerSpecies].BlockAmount,
                Is.EqualTo(blockBefore + 2));
            Assert.That(preview.Progression.Currency, Is.EqualTo(currencyAtBoundary - 5));

            // A repeated click cannot purchase or apply the same boundary twice.
            Assert.That(preview.PurchaseReward(0), Is.False);
            Assert.That(preview.Progression.PurchasedUpgradeCount, Is.EqualTo(1));
            Assert.That(run.UpgradeLoadout.Count, Is.EqualTo(1));

            for (var phase = 2; phase < SpeciesSimulationPreview.ContinuousExpeditionPhaseCount; phase++)
            {
                timeout = Time.realtimeSinceStartup + 5f;
                while (preview.State != SpeciesPreviewState.PhaseDecision
                       && Time.realtimeSinceStartup < timeout)
                {
                    yield return null;
                }

                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
                Assert.That(run.Tick, Is.EqualTo(phase * 2));
                preview.ContinueWithoutUpgrade();
                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            }

            timeout = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.Results
                   && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
            Assert.That(preview.Run, Is.SameAs(run));
            Assert.That(run.Tick, Is.EqualTo(2 * SpeciesSimulationPreview.ContinuousExpeditionPhaseCount));
            Assert.That(run.Status, Is.EqualTo(SimulationRunStatus.Complete));
            Assert.That(preview.CanPurchaseReward(0), Is.False);

            var earnedFieldData = preview.Progression.Currency;
            preview.PlayNextSimulation();
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(preview.Run, Is.Not.SameAs(run));
            Assert.That(preview.Progression.PurchasedUpgradeCount, Is.Zero);
            Assert.That(preview.Progression.Currency, Is.EqualTo(earnedFieldData));
        }

        [UnityTest]
        public IEnumerator CoupledResponseAppliesToTheCounterpartAtTheSameBoundary()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
            preview.StopSimulation();
            var viewModel = GameObject.Find("Prototype Camera")
                ?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(viewModel, Is.Not.Null);
            var viewModelType = viewModel.GetType();
            viewModelType.GetProperty("CoupledSpeciesResponsesEnabled")?.SetValue(viewModel, true);
            StringAssert.Contains(
                "ON (EXPERIMENTAL)",
                viewModelType.GetProperty("CoupledSpeciesResponsesToggleText")?.GetValue(viewModel) as string);
            var applySettingsCommand = viewModelType.GetProperty("ApplySettingsCommand")?.GetValue(viewModel);
            applySettingsCommand?.GetType().GetMethod("Execute")?.Invoke(applySettingsCommand, new object[] { null });
            Assert.That(preview.CoupledSpeciesResponsesEnabled, Is.True);
            Assert.That(preview.TryApplyExperimentalFeatures(true, true, "0", out var message), Is.True, message);
            Assert.That(preview.TryApplyContinuousPhases(true, "1", out message), Is.True, message);
            Assert.That(preview.TryApplyGlobalSettingsForTicksWithStartingPopulations(
                "8", "8", "0", preview.MaximumPopulation.ToString(CultureInfo.InvariantCulture),
                preview.MinimumPopulation.ToString(CultureInfo.InvariantCulture),
                "2", "0.01", "0", "1", "0", false, "0", "1", "0", out message), Is.True, message);
            preview.StartSimulation();
            preview.Progression.AddCurrency(10);
            var run = preview.Run;
            var timeout = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.PhaseDecision && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
            Assert.That(preview.GetRewardOptionId(0), Is.EqualTo(SpeciesUpgradeCatalog.ToughHideId));
            StringAssert.Contains("Fox responds: PIERCING BITE", preview.GetRewardOptionDisplayName(0));
            Assert.That(SpeciesUpgradeCatalog.TryGetCoupledResponse(
                preview.PlayerSpecies,
                SpeciesUpgradeCatalog.ToughHideId,
                out var carnivore,
                out _), Is.True);
            var attackModifier = preview.ActiveSpeciesRules[carnivore].AttackModifier;

            Assert.That(preview.PurchaseReward(0), Is.True);
            Assert.That(run.UpgradeLoadout, Has.Count.EqualTo(2));
            Assert.That(run.UpgradeLoadout[0].TargetSpecies, Is.EqualTo(preview.PlayerSpecies));
            Assert.That(run.UpgradeLoadout[1].TargetSpecies, Is.EqualTo(carnivore));
            Assert.That(run.UpgradeLoadout[1].Id, Is.EqualTo(SpeciesUpgradeCatalog.PiercingBiteId));
            Assert.That(run.UpgradeAcquisitionTimeline[0].Source,
                Is.EqualTo(SimulationUpgradeAcquisition.PlayerChoiceSource));
            Assert.That(run.UpgradeAcquisitionTimeline[1].Source,
                Is.EqualTo(SimulationUpgradeAcquisition.CoupledResponseSource));
            Assert.That(run.UpgradeAcquisitionTimeline[1].TriggeringUpgradeId,
                Is.EqualTo(SpeciesUpgradeCatalog.ToughHideId));
            Assert.That(preview.ActiveSpeciesRules[carnivore].AttackModifier, Is.EqualTo(attackModifier + 1));
            StringAssert.Contains("PIERCING BITE", preview.PhaseRewardMessage);
        }

        [UnityTest]
        public IEnumerator FiveHerbivoreSkillsRetainLevelsAndAcquisitionsAcrossDecisionBoundaries()
        {
            var ids = new[] { "tough-hide", "efficient-digestion", "crowding-tolerance",
                "reproductive-drive", "threat-exposure" };
            for (var skillIndex = 0; skillIndex < ids.Length; skillIndex++)
            {
                yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
                yield return null;
                var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
                preview.StopSimulation();
                Assert.That(preview.TryApplyExperimentalFeatures(true, "0", out var message), Is.True, message);
                Assert.That(preview.TryApplyContinuousPhases(true, "1", out message), Is.True, message);
                // The seed selects the first experimental offer deterministically.
                Assert.That(preview.TryApplyGlobalSettingsForTicks(
                    "8", "8", skillIndex.ToString(CultureInfo.InvariantCulture),
                    preview.MaximumPopulation.ToString(CultureInfo.InvariantCulture),
                    preview.MinimumPopulation.ToString(CultureInfo.InvariantCulture),
                    "10", "0.01", "0", "1", "0", false, out message), Is.True, message);
                preview.StartSimulation();
                var run = preview.Run;
                var originalRules = preview.Progression.CurrentRules;
                preview.Progression.AddCurrency(100);
                var upgrade = SpeciesUpgradeCatalog.Create(ids[skillIndex]);
                // Preload one level, then exercise every in-expedition choice.
                Assert.That(preview.Progression.TryPurchase(upgrade), Is.True);
                for (var phase = 1; phase < SpeciesSimulationPreview.ContinuousExpeditionPhaseCount; phase++)
                {
                    var timeout = Time.realtimeSinceStartup + 5f;
                    while (preview.State != SpeciesPreviewState.PhaseDecision && Time.realtimeSinceStartup < timeout)
                    {
                        yield return null;
                    }
                    Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
                    Assert.That(preview.GetRewardOptionId(0), Is.EqualTo(upgrade.Id));
                    Assert.That(preview.CanPurchaseReward(0), Is.True);
                    var currency = preview.Progression.Currency;
                    Assert.That(preview.PurchaseReward(0), Is.True);
                    Assert.That(preview.Run, Is.SameAs(run));
                    Assert.That(run.Tick, Is.EqualTo(phase));
                    Assert.That(preview.Progression.GetUpgradeLevel(upgrade.Id), Is.EqualTo(phase + 1));
                    Assert.That(preview.Progression.Currency, Is.EqualTo(currency - 5));
                    Assert.That(run.UpgradeLoadout, Has.Count.EqualTo(phase + 1));
                    Assert.That(run.UpgradeAcquisitionTimeline, Has.Count.EqualTo(phase + 1));
                    Assert.That(run.UpgradeAcquisitionTimeline[phase].EffectiveTick, Is.EqualTo(phase));
                    Assert.That(run.UpgradeAcquisitionTimeline[phase].Order, Is.EqualTo(phase));
                    Assert.That(preview.ActiveSpeciesRules[preview.PlayerSpecies], Is.SameAs(preview.Progression.CurrentRules));
                    if (upgrade.Id == "threat-exposure")
                    {
                        Assert.That(preview.Progression.CurrentRules.FleeMovementSpeedBonus,
                            Is.EqualTo(originalRules.FleeMovementSpeedBonus + 0.75f));
                        Assert.That(preview.Progression.PreContactAvoidanceChance,
                            Is.EqualTo((phase + 1) * 0.08f).Within(0.00001f));
                    }
                    Assert.That(preview.PurchaseReward(0), Is.False, "A repeated click must not charge again.");
                }
                var finalTimeout = Time.realtimeSinceStartup + 5f;
                while (preview.State != SpeciesPreviewState.Results && Time.realtimeSinceStartup < finalTimeout)
                {
                    yield return null;
                }

                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
                Assert.That(run.Tick, Is.EqualTo(SpeciesSimulationPreview.ContinuousExpeditionPhaseCount));
                Assert.That(run.PhaseResults, Has.Count.EqualTo(SpeciesSimulationPreview.ContinuousExpeditionPhaseCount));
            }
        }

        [UnityTest]
        public IEnumerator PredatorPhaseRewardsUseAuthoredPredatorSkills()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
            preview.StopSimulation();
            Assert.That(preview.TryApplyExperimentalFeatures(true, "0", out var message), Is.True, message);
            Assert.That(preview.TrySetPlayerSpecies("fox", out message), Is.True, message);
            Assert.That(preview.TryApplyContinuousPhases(true, "2", out message), Is.True, message);
            Assert.That(preview.TryApplyGlobalSettingsForTicks(
                "8", "8", preview.BaseSeed.ToString(CultureInfo.InvariantCulture),
                preview.MaximumPopulation.ToString(CultureInfo.InvariantCulture),
                preview.MinimumPopulation.ToString(CultureInfo.InvariantCulture),
                "4", "0.01", "0", "1", "0", false, out message), Is.True, message);

            preview.StartSimulation();
            preview.Progression.AddCurrency(10);
            var timeout = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.PhaseDecision && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
            Assert.That(preview.RewardOptionCount, Is.EqualTo(2));
            var predatorSkillIds = new[] { "relentless-pursuit", "piercing-bite", "hunt-urgency", "brood-drive" };
            for (var index = 0; index < preview.RewardOptionCount; index++)
            {
                var optionId = preview.GetRewardOptionId(index);
                Assert.That(Array.IndexOf(predatorSkillIds, optionId), Is.GreaterThanOrEqualTo(0));
                StringAssert.DoesNotContain("FASTER", preview.GetRewardOptionDisplayName(index));
                StringAssert.DoesNotContain("ATTACK", preview.GetRewardOptionDisplayName(index));
                StringAssert.DoesNotContain("BLOCK", preview.GetRewardOptionDisplayName(index));
            }
        }

        [UnityTest]
        public IEnumerator PlayerModeUsesContinuousPhasesAndDeveloperModeCanSelectSingleRun()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var runtime = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>();
            var viewModel = GameObject.Find("Prototype Camera")
                ?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(runtime, Is.Not.Null);
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(runtime.SpeciesPreview.ContinuousPhasesEnabled, Is.True);

            runtime.SpeciesPreview.StopSimulation();

            var continuousSettingsApplied = runtime.SpeciesPreview.TryApplyContinuousPhases(
                enabled: false,
                phaseLengthValue: string.Empty,
                out var continuousMessage);
            Assert.That(continuousSettingsApplied, Is.True, continuousMessage);
            var settingsApplied = runtime.SpeciesPreview.TryApplyGlobalSettingsForTicks(
                "8",
                "8",
                runtime.SpeciesPreview.BaseSeed.ToString(CultureInfo.InvariantCulture),
                runtime.SpeciesPreview.MaximumPopulation.ToString(CultureInfo.InvariantCulture),
                runtime.SpeciesPreview.MinimumPopulation.ToString(CultureInfo.InvariantCulture),
                "4",
                "0.01",
                runtime.SpeciesPreview.PlantProbability.ToString(CultureInfo.InvariantCulture),
                runtime.SpeciesPreview.HerbivoreProbability.ToString(CultureInfo.InvariantCulture),
                runtime.SpeciesPreview.CarnivoreProbability.ToString(CultureInfo.InvariantCulture),
                randomizeSeed: false,
                out var settingsMessage);
            Assert.That(settingsApplied, Is.True, settingsMessage);

            // Player mode reapplies the VM's continuous settings on start. Keep
            // this test's boundary short enough to observe the decision state.
            viewModel.GetType().GetProperty("RunTicksText")?.SetValue(viewModel, "40");
            viewModel.GetType().GetProperty("PhaseLengthTicksText")?.SetValue(viewModel, "4");
            viewModel.GetType().GetProperty("DeveloperMode")?.SetValue(viewModel, false);
            var playerStartCommand = viewModel.GetType().GetProperty("StartCommand")?.GetValue(viewModel);
            playerStartCommand?.GetType().GetMethod("Execute")?.Invoke(playerStartCommand, new object[] { null });

            var timeout = Time.realtimeSinceStartup + 5f;
            while (runtime.SpeciesPreview.State != SpeciesPreviewState.PhaseDecision
                   && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(runtime.SpeciesPreview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
            var phaseDecisionTitle = viewModel.GetType()
                .GetProperty("PhaseDecisionTitleText")
                ?.GetValue(viewModel) as string;
            Assert.That(phaseDecisionTitle, Is.EqualTo("PHASE 01 COMPLETE"));

            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            runtime = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>();
            viewModel = GameObject.Find("Prototype Camera")
                ?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(runtime, Is.Not.Null);
            Assert.That(viewModel, Is.Not.Null);

            runtime.SpeciesPreview.StopSimulation();
            var viewModelType = viewModel.GetType();
            viewModelType.GetProperty("DeveloperMode")?.SetValue(viewModel, true);
            viewModelType.GetProperty("RunTicksText")?.SetValue(viewModel, "4");
            viewModelType.GetProperty("PhaseLengthTicksText")?.SetValue(viewModel, "2");
            viewModelType.GetProperty("ContinuousPhasesEnabled")?.SetValue(viewModel, false);
            var applySettingsCommand = viewModelType.GetProperty("ApplySettingsCommand")?.GetValue(viewModel);
            applySettingsCommand?.GetType().GetMethod("Execute")?.Invoke(applySettingsCommand, new object[] { null });
            Assert.That(runtime.SpeciesPreview.ContinuousPhasesEnabled, Is.False);

            var startCommand = viewModelType.GetProperty("StartCommand")?.GetValue(viewModel);
            startCommand?.GetType().GetMethod("Execute")?.Invoke(startCommand, new object[] { null });
            timeout = Time.realtimeSinceStartup + 5f;
            while (runtime.SpeciesPreview.State != SpeciesPreviewState.Rewards
                   && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(runtime.SpeciesPreview.State, Is.EqualTo(SpeciesPreviewState.Rewards));
            Assert.That(runtime.SpeciesPreview.Run.Status, Is.EqualTo(SimulationRunStatus.Complete));
        }
    }
}
