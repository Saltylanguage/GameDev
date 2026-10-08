using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        public IEnumerator DesktopJourneyLaunchShowsMapBeforeFirstSimulation()
        {
            var transitionObject = new GameObject("Journey launch test transition");
            var transition = transitionObject.AddComponent<Helper_SceneTransition>();
            var startingUpgrade = new SpeciesUpgradeSnapshot(
                "journey-test-starting-block",
                "Starting Block",
                "A frozen launch upgrade for the journey test.",
                new SpeciesId("hare"),
                0,
                new[] { new SpeciesUpgradeModifier(SpeciesAttributeIds.BlockAmount, 1f) });
            var launch = new SimulationLaunchRequest(
                "journey-test", "ForestEdge", "hare", 10100,
                orderedUpgradeSnapshots: new[] { startingUpgrade });
            Assert.That(transition.LoadSimulation(launch), Is.True);
            yield return null;

            var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>()?.SpeciesPreview;
            var viewModel = GameObject.Find("Prototype Camera")?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(preview, Is.Not.Null);
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(preview.JourneyActive, Is.True);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Ready));
            Assert.That(preview.Run.Status, Is.EqualTo(SimulationRunStatus.Ready));
            Assert.That(viewModel.GetType().GetProperty("JourneyMapVisibility")
                ?.GetValue(viewModel)?.ToString(), Is.EqualTo("Visible"));
            Assert.That(preview.ChooseJourneyNode("forest-edge-start"), Is.True);
            Assert.That(preview.ClaimJourneyNodeReward(0), Is.True);
            Assert.That(preview.ChooseJourneyNode("first-cycle"), Is.True);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(preview.Run.Status, Is.EqualTo(SimulationRunStatus.Running));
            Assert.That(preview.Run.UpgradeLoadout.Any(
                upgrade => upgrade.Id == startingUpgrade.Id), Is.True);
        }

        [UnityTest]
        public IEnumerator DesktopSimulationIconOpensTheJourneyMap()
        {
            yield return SceneManager.LoadSceneAsync("GalapagOSDesktopTest");
            yield return null;
            yield return null;

            var desktopViewModel = GameObject.Find("GalapagOS Desktop Test Camera")
                ?.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            var preview = UnityEngine.Object.FindAnyObjectByType<SpeciesSimulationPreview>();
            var simulationViewModel = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(
                    FindObjectsSortMode.None)
                .FirstOrDefault(component => component.GetType().FullName == "SaltyGame.VM_SimulationShell");
            Assert.That(desktopViewModel, Is.Not.Null);
            Assert.That(preview, Is.Not.Null);
            Assert.That(simulationViewModel, Is.Not.Null);
            Assert.That(preview.JourneyActive, Is.True);

            var open = desktopViewModel.GetType().GetProperty("OpenDesktopIconCommand")
                ?.GetValue(desktopViewModel);
            Assert.That(open, Is.Not.Null);
            open.GetType().GetMethod("Execute")?.Invoke(open, new object[] { "Simulation" });
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Ready));
            Assert.That(simulationViewModel.GetType().GetProperty("JourneyMapVisibility")
                ?.GetValue(simulationViewModel)?.ToString(), Is.EqualTo("Visible"));
            var back = simulationViewModel.GetType().GetProperty("CloseJourneyMapCommand")
                ?.GetValue(simulationViewModel);
            back?.GetType().GetMethod("Execute")?.Invoke(back, new object[] { null });
            Assert.That(GameObject.Find("GalapagOS Desktop Test Camera")?.GetComponent<Camera>()?.enabled,
                Is.True);
            open.GetType().GetMethod("Execute")?.Invoke(open, new object[] { "Simulation" });
            Assert.That(simulationViewModel.GetType().GetProperty("JourneyMapVisibility")
                ?.GetValue(simulationViewModel)?.ToString(), Is.EqualTo("Visible"));
            Assert.That(preview.ChooseJourneyNode("forest-edge-start"), Is.True);
            Assert.That(preview.ClaimJourneyNodeReward(0), Is.True);
            Assert.That(preview.ChooseJourneyNode("first-cycle"), Is.True);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
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
        public IEnumerator SlashlineIsAvailableWithoutDeveloperModeWhileUpgradeDiagnosticsStayHidden()
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
                "36",
                "20",
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
            Assert.That(summaryVisibility, Is.EqualTo("Visible"));
            StringAssert.Contains("ADD:", summary);
            StringAssert.Contains("FPO:", summary);
            Assert.That(viewModel.GetType().GetProperty("ExperimentalUpgradeCountVisibility")
                ?.GetValue(viewModel)?.ToString(), Is.EqualTo("Collapsed"));
            var showNotesCommand = viewModel.GetType().GetProperty("ShowFieldNotesCommand")?.GetValue(viewModel);
            showNotesCommand?.GetType().GetMethod("Execute")?.Invoke(showNotesCommand, new object[] { null });
            Assert.That(viewModel.GetType().GetProperty("FieldNotesVisibility")
                ?.GetValue(viewModel)?.ToString(), Is.EqualTo("Visible"));
            var closeNotesCommand = viewModel.GetType().GetProperty("CloseFieldNotesCommand")?.GetValue(viewModel);
            closeNotesCommand?.GetType().GetMethod("Execute")?.Invoke(closeNotesCommand, new object[] { null });
            Assert.That(viewModel.GetType().GetProperty("FieldNotesVisibility")
                ?.GetValue(viewModel)?.ToString(), Is.EqualTo("Collapsed"));

            viewModel.GetType().GetProperty("DeveloperMode")?.SetValue(viewModel, true);
            summary = viewModel.GetType()
                .GetProperty("ExperimentalHerbivoreStatLineSummary")
                ?.GetValue(viewModel) as string;
            summaryVisibility = viewModel.GetType()
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
        public IEnumerator GardenersPathCanBeChosenAcrossFivePhaseDecisions()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;
            var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
            preview.StopSimulation();
            preview.ResetToStart();
            Assert.That(preview.TrySetJourneyPrototypeEnabled(false), Is.True);
            Assert.That(preview.TrySetPlayerSpecies("hare", out var message), Is.True, message);
            Assert.That(preview.TryApplyGlobalSettingsForTicksWithStartingPopulations("36", "20", "1", "720", "0", "6", "0.1",
                "0", "0", "0", false, "400", "25", "15", out message), Is.True, message);
            Assert.That(preview.TryApplyExperimentalFeatures(true, false, "0", out message), Is.True, message);
            Assert.That(preview.TryApplyContinuousPhases(true, "1", out message), Is.True, message);
            preview.StartSimulation();
            preview.PauseSimulation();
            var baseline = preview.ActiveSpeciesRules[preview.PlayerSpecies];
            var path = new[] { "efficient-digestion", "seed-dispersal", "efficient-digestion", "seed-dispersal", "efficient-digestion" };
            foreach (var desired in path)
            {
                Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
                var index = -1;
                for (var option = 0; option < preview.RewardOptionCount; option++)
                    if (preview.GetRewardOptionId(option) == desired) index = option;
                Assert.That(index, Is.GreaterThanOrEqualTo(0), desired);
                var tick = preview.Run.Tick;
                var currency = preview.Progression.Currency;
                Assert.That(preview.PurchaseReward(index), Is.True);
                Assert.That(preview.Run.Tick, Is.EqualTo(tick));
                Assert.That(preview.Progression.Currency, Is.EqualTo(currency));
                preview.PauseSimulation();
            }
            Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
            Assert.That(preview.ActiveSpeciesRules[preview.PlayerSpecies].SeedDropChance,
                Is.EqualTo(baseline.SeedDropChance + 0.02f).Within(0.000001f));
            Assert.That(preview.ActiveSpeciesRules[preview.PlayerSpecies].DigestionEnergyBonus,
                Is.EqualTo(baseline.DigestionEnergyBonus + 0.3f).Within(0.000001f));
            Assert.That(preview.Run.UpgradeAcquisitionTimeline.Count, Is.EqualTo(5));
            var phasePlants = 0;
            foreach (var phase in preview.Run.PhaseResults)
                phasePlants += phase.Metrics.GetActivity(preview.PlayerSpecies).SeedDropSuccesses;
            Assert.That(phasePlants, Is.EqualTo(preview.Run.Metrics.GetActivity(preview.PlayerSpecies).SeedDropSuccesses));
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

        [UnityTest]
        public IEnumerator AuthoredStartingPopulationRejectsGridThatCannotFitIt()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
            preview.StopSimulation();
            var runBefore = preview.Run;

            Assert.That(preview.TryApplyGlobalSettingsForTicks(
                "8", "8", preview.BaseSeed.ToString(CultureInfo.InvariantCulture),
                preview.MaximumPopulation.ToString(CultureInfo.InvariantCulture),
                preview.MinimumPopulation.ToString(CultureInfo.InvariantCulture),
                "4", "0.01", "0", "1", "0", false, out var message), Is.False);
            StringAssert.Contains("Starting populations cannot exceed the grid capacity", message);
            Assert.That(preview.Run, Is.SameAs(runBefore));
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
            Assert.That(preview.TrySetJourneyPrototypeEnabled(false), Is.True);
            var viewModel = GameObject.Find("Prototype Camera")
                ?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(viewModel, Is.Not.Null);
            var phaseSettingsApplied = preview.TryApplyContinuousPhases(
                enabled: true,
                phaseLengthValue: "2",
                out var phaseMessage);
            Assert.That(phaseSettingsApplied, Is.True, phaseMessage);

            var settingsApplied = preview.TryApplyGlobalSettingsForTicks(
                "36",
                "20",
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
            Assert.That(preview.RewardOptionCount, Is.EqualTo(3));
            var offeredIds = Enumerable.Range(0, preview.RewardOptionCount)
                .Select(preview.GetRewardOptionId)
                .ToArray();
            Assert.That(offeredIds.Distinct().Count(), Is.EqualTo(3));
            Assert.That(offeredIds.All(SpeciesUpgradeCatalog.IsExperimentalHerbivoreMutationId), Is.True);
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

            var currencyBeforeSkip = preview.Progression.Currency;
            preview.ContinueWithoutUpgrade();
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(preview.Run, Is.SameAs(run));
            Assert.That(preview.Progression.Currency, Is.EqualTo(currencyBeforeSkip));

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
        public IEnumerator JourneyNodeRewardsAndSimulationsContinueTheSameBiome()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
            var viewModel = GameObject.Find("Prototype Camera").GetComponent("SaltyGame.VM_SimulationShell");
            preview.StopSimulation();
            Assert.That(preview.TryApplyWrapEdges(true, out var wrapMessage), Is.True, wrapMessage);
            Assert.That(preview.TryApplyContinuousPhases(true, "2", out var message), Is.True, message);
            Assert.That(preview.JourneyActive, Is.True);
            Assert.That(preview.CanChooseJourneyNode("forest-edge-start"), Is.True);
            Assert.That(preview.CanChooseJourneyNode("first-cycle"), Is.False);
            Assert.That(preview.ChooseJourneyNode("forest-edge-start"), Is.True);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.JourneyNodeReward));
            Assert.That(preview.JourneyRewardOptionCount, Is.EqualTo(3));
            Assert.That(preview.ClaimJourneyNodeReward(0), Is.True);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Ready));
            Assert.That(preview.PurchasedUpgradeCount, Is.EqualTo(1));
            Assert.That(preview.HasVisitedJourneyNode(preview.JourneyMap.StartNode), Is.True);
            Assert.That(preview.ChooseJourneyNode("first-cycle"), Is.True);
            var run = preview.Run;
            Assert.That(run.Cells.WrapEdges, Is.True);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(run.TargetTicks, Is.EqualTo(24));
            Assert.That(run.UpgradeLoadout.Any(upgrade => upgrade.Id == "tough-hide"), Is.True);

            for (var phase = 1; phase <= 5; phase++)
            {
                var timeout = Time.realtimeSinceStartup + 5f;
                while (preview.State != SpeciesPreviewState.PhaseDecision && Time.realtimeSinceStartup < timeout)
                {
                    yield return null;
                }
                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
                if (phase == 1)
                {
                    Assert.That(preview.PurchaseReward(0), Is.True);
                }
                else
                {
                    preview.ContinueWithoutUpgrade();
                }
            }

            var boundary = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.JourneyDecision && Time.realtimeSinceStartup < boundary)
            {
                yield return null;
            }
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.JourneyDecision));
            Assert.That(run.PhaseResults, Has.Count.EqualTo(6));
            Assert.That(run.Tick, Is.EqualTo(12));
            var mutationCount = preview.PurchasedUpgradeCount;
            Assert.That(mutationCount, Is.GreaterThanOrEqualTo(2));
            Assert.That(viewModel.GetType().GetProperty("ResultsVisibility")?.GetValue(viewModel)?.ToString(),
                Is.EqualTo("Visible"));
            Assert.That(viewModel.GetType().GetProperty("JourneyMapVisibility")?.GetValue(viewModel)?.ToString(),
                Is.EqualTo("Collapsed"));
            Assert.That(preview.ChooseJourneyNode("burrow-study"), Is.False);

            var cells = run.Cells;
            var historyCount = run.PopulationHistory.Count;
            var seedDropBefore = preview.ActiveSpeciesRules[new SpeciesId("plant")].SeedDropChance;
            Assert.That(preview.ChooseJourneyNode("seedfall"), Is.True);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.JourneyNodeReward));
            Assert.That(preview.JourneyRewardOptionCount, Is.EqualTo(1));
            Assert.That(preview.ClaimJourneyNodeReward(0), Is.True);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.JourneyDecision));
            Assert.That(preview.PurchasedUpgradeCount, Is.EqualTo(mutationCount));
            Assert.That(preview.ActiveSpeciesRules[new SpeciesId("plant")].SeedDropChance,
                Is.EqualTo(seedDropBefore + 0.1f).Within(0.0001f));
            Assert.That(preview.ChooseJourneyNode("burrow-study"), Is.True);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(preview.Run, Is.SameAs(run));
            Assert.That(run.Cells, Is.SameAs(cells));
            Assert.That(run.Cells.WrapEdges, Is.True);
            Assert.That(run.PopulationHistory.Count, Is.EqualTo(historyCount));
            Assert.That(run.PhaseIndex, Is.EqualTo(7));
            Assert.That(run.UpgradeLoadout.Count(upgrade => upgrade.TargetSpecies == preview.PlayerSpecies),
                Is.GreaterThanOrEqualTo(mutationCount));
            Assert.That(run.UpgradeLoadout.Any(upgrade => upgrade.Id == "journey.seedfall"), Is.True);

            for (var phase = 7; phase <= 11; phase++)
            {
                var timeout = Time.realtimeSinceStartup + 5f;
                while (preview.State != SpeciesPreviewState.PhaseDecision && Time.realtimeSinceStartup < timeout)
                {
                    yield return null;
                }
                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
                preview.ContinueWithoutUpgrade();
            }
            var finish = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.Results && Time.realtimeSinceStartup < finish)
            {
                yield return null;
            }
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
            Assert.That(run.Tick, Is.EqualTo(24));
            Assert.That(run.PhaseResults, Has.Count.EqualTo(12));
            Assert.That(preview.PurchasedUpgradeCount, Is.EqualTo(mutationCount));
            Assert.That(preview.HasVisitedJourneyNode(preview.JourneyCurrentNode), Is.True);

            var next = viewModel.GetType().GetProperty("PlayNextSimulationCommand")?.GetValue(viewModel);
            next?.GetType().GetMethod("Execute")?.Invoke(next, new object[] { null });
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
            Assert.That(viewModel.GetType().GetProperty("JourneyMapVisibility")?.GetValue(viewModel)?.ToString(),
                Is.EqualTo("Visible"));
            var fresh = viewModel.GetType().GetProperty("StartNewJourneyCommand")?.GetValue(viewModel);
            fresh?.GetType().GetMethod("Execute")?.Invoke(fresh, new object[] { null });
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Ready));
            Assert.That(preview.PurchasedUpgradeCount, Is.Zero);
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
            Assert.That(preview.TrySetJourneyPrototypeEnabled(false), Is.True);
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
            Assert.That(preview.TrySetJourneyPrototypeEnabled(false), Is.True);
            Assert.That(preview.TryApplyContinuousPhases(true, "2", out var phaseMessage), Is.True, phaseMessage);
            Assert.That(preview.TryApplyGlobalSettingsForTicks(
                "36",
                "20",
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
            var timeout = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.PhaseDecision
                   && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
            Assert.That(preview.GetRewardOptionId(0), Is.EqualTo("tough-hide"));
            StringAssert.Contains("TOUGH HIDE", preview.GetRewardOptionDisplayName(0));
            StringAssert.Contains("Block more incoming attacks", preview.GetRewardOptionDisplayName(0));
            StringAssert.DoesNotContain("TRAILBLAZER", preview.GetRewardOptionDisplayName(0));
            StringAssert.Contains("FREE", preview.GetRewardOptionDisplayName(0));
            StringAssert.DoesNotContain("COST", preview.GetRewardOptionDisplayName(0));
            Assert.That(preview.Progression.TrySpend(preview.Progression.Currency), Is.True);
            Assert.That(preview.Progression.Currency, Is.Zero);
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
            Assert.That(preview.Progression.Currency, Is.EqualTo(currencyAtBoundary));

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
        public IEnumerator FoxOfferHidesBroodDriveWhenItsHareResponseIsCapped()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
            preview.StopSimulation();
            Assert.That(preview.TryApplyExperimentalFeatures(true, true, "0", out var message), Is.True, message);
            Assert.That(preview.TrySetPlayerSpecies("fox", out message), Is.True, message);
            Assert.That(preview.TryApplyContinuousPhases(true, "2", out message), Is.True, message);
            Assert.That(preview.TryApplyGlobalSettingsForTicks(
                "36", "20", "3", preview.MaximumPopulation.ToString(CultureInfo.InvariantCulture),
                preview.MinimumPopulation.ToString(CultureInfo.InvariantCulture),
                "4", "0.01", "0", "1", "0", false, out message), Is.True, message);

            preview.StartSimulation();
            var timeout = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.PhaseDecision && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
            Assert.That(preview.ActiveSpeciesRules[new SpeciesId("hare")].ReproductionChance, Is.EqualTo(1f));
            Assert.That(preview.RewardOptionCount, Is.EqualTo(3));
            Assert.That(preview.GetRewardOptionId(0), Is.Not.EqualTo(SpeciesUpgradeCatalog.BroodDriveId));
            Assert.That(preview.GetRewardOptionId(1), Is.Not.EqualTo(SpeciesUpgradeCatalog.BroodDriveId));
            Assert.That(preview.CanPurchaseReward(0), Is.True);
            Assert.That(preview.PurchaseReward(0), Is.True);
        }

        [UnityTest]
        public IEnumerator TrailblazerFivePickPathWorksThroughNormalPhaseOffersAndPausedSteps()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;
            var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
            preview.StopSimulation();
            Assert.That(preview.TrySetJourneyPrototypeEnabled(false), Is.True);
            Assert.That(preview.TryApplyGlobalSettingsForTicksWithStartingPopulations(
                "36", "20", "5", "720", "0", "6", "0.1", "0", "0", "0", false,
                "400", "25", "15", out var message), Is.True, message);
            Assert.That(preview.TryApplyExperimentalFeatures(true, false, "0", out message), Is.True, message);
            Assert.That(preview.TryApplyContinuousPhases(true, "1", out message), Is.True, message);
            preview.StartSimulation();
            var run = preview.Run;
            var originalRules = preview.Progression.CurrentRules;
            var picks = new[] { "faster-movement", "threat-exposure", "faster-movement", "threat-exposure", "faster-movement" };
            for (var phase = 0; phase < picks.Length; phase++)
            {
                preview.PauseSimulation();
                Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
                Assert.That(preview.AdvanceOneTickWhilePaused(), Is.False);
                var option = Enumerable.Range(0, preview.RewardOptionCount)
                    .First(index => preview.GetRewardOptionId(index) == picks[phase]);
                var currency = preview.Progression.Currency;
                Assert.That(preview.PurchaseReward(option), Is.True);
                Assert.That(preview.Progression.Currency, Is.EqualTo(currency));
                Assert.That(preview.Run, Is.SameAs(run));
                Assert.That(run.UpgradeAcquisitionTimeline[phase].EffectiveTick, Is.EqualTo(phase + 1));
            }
            Assert.That(preview.Progression.CurrentRules.MovementSpeed, Is.EqualTo(originalRules.MovementSpeed + 1.5f));
            Assert.That(preview.Progression.CurrentRules.FleeMovementSpeedBonus, Is.EqualTo(originalRules.FleeMovementSpeedBonus));
            Assert.That(preview.Progression.PreContactAvoidanceChance, Is.EqualTo(0.16f).Within(0.00001f));
            Assert.That(run.UpgradeLoadout[1].PreContactAvoidanceChanceBonus, Is.EqualTo(0.08f));
            Assert.That(run.UpgradeLoadout[1].Modifiers, Is.Empty);
            preview.PauseSimulation();
            Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
            Assert.That(run.Tick, Is.EqualTo(6));
            Assert.That(run.PhaseResults, Has.Count.EqualTo(6));
        }

        [UnityTest]
        public IEnumerator ApplicableHerbivoreSkillsRetainLevelsAndAcquisitionsAcrossDecisionBoundaries()
        {
            var ids = new[] { "tough-hide", "efficient-digestion", "crowding-tolerance",
                "threat-exposure" };
            for (var skillIndex = 0; skillIndex < ids.Length; skillIndex++)
            {
                yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
                yield return null;
                var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
                preview.StopSimulation();
                Assert.That(preview.TrySetJourneyPrototypeEnabled(false), Is.True);
                Assert.That(preview.TryApplyExperimentalFeatures(true, "0", out var message), Is.True, message);
                Assert.That(preview.TryApplyContinuousPhases(true, "1", out message), Is.True, message);
                // The seed selects the first experimental offer deterministically.
                Assert.That(preview.TryApplyGlobalSettingsForTicks(
                    "36", "20", skillIndex.ToString(CultureInfo.InvariantCulture),
                    preview.MaximumPopulation.ToString(CultureInfo.InvariantCulture),
                    preview.MinimumPopulation.ToString(CultureInfo.InvariantCulture),
                    "10", "0.01", "0", "1", "0", false, out message), Is.True, message);
                preview.StartSimulation();
                var run = preview.Run;
                var originalRules = preview.Progression.CurrentRules;
                preview.Progression.AddCurrency(100);
                Assert.That(preview.Progression.CanApplyFreeUpgrade(
                    SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.ReproductiveDriveId)), Is.False);
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
                    Assert.That(preview.RewardOptionCount, Is.EqualTo(3));
                    var offeredIds = Enumerable.Range(0, preview.RewardOptionCount)
                        .Select(preview.GetRewardOptionId)
                        .ToArray();
                    Assert.That(offeredIds.Distinct().Count(), Is.EqualTo(3));
                    Assert.That(offeredIds.All(SpeciesUpgradeCatalog.IsExperimentalHerbivoreMutationId), Is.True);
                    Assert.That(preview.GetRewardOptionDisplayName(0), Does.Contain($"LV {phase + 1}"));
                    Assert.That(preview.GetRewardOptionDisplayName(0), Does.Not.Contain("COST"));
                    Assert.That(preview.GetRewardOptionDisplayName(0), Does.Not.Contain("DATA"));
                    Assert.That(preview.GetRewardOptionId(0), Is.EqualTo(upgrade.Id));
                    if (upgrade.Id == SpeciesUpgradeCatalog.ToughHideId && phase > 1)
                    {
                        var completedPhase = run.PhaseResults[run.PhaseResults.Count - 1];
                        var blockedIncomingAttacks = completedPhase.Metrics.CombatRollEvents.Count(
                            combat => combat.TargetSpecies == preview.PlayerSpecies && !combat.Hit);
                        StringAssert.Contains(
                            $"Incoming attacks blocked this phase: {blockedIncomingAttacks}",
                            preview.PhaseRewardMessage);
                    }
                    Assert.That(preview.CanPurchaseReward(0), Is.True);
                    var currency = preview.Progression.Currency;
                    Assert.That(preview.PurchaseReward(0), Is.True);
                    Assert.That(preview.Run, Is.SameAs(run));
                    Assert.That(run.Tick, Is.EqualTo(phase));
                    Assert.That(preview.Progression.GetUpgradeLevel(upgrade.Id), Is.EqualTo(phase + 1));
                    Assert.That(preview.Progression.Currency, Is.EqualTo(currency));
                    Assert.That(run.UpgradeLoadout, Has.Count.EqualTo(phase + 1));
                    Assert.That(run.UpgradeAcquisitionTimeline, Has.Count.EqualTo(phase + 1));
                    Assert.That(run.UpgradeLoadout[phase].Cost, Is.Zero);
                    Assert.That(run.UpgradeAcquisitionTimeline[phase].EffectiveTick, Is.EqualTo(phase));
                    Assert.That(run.UpgradeAcquisitionTimeline[phase].Order, Is.EqualTo(phase));
                    Assert.That(preview.ActiveSpeciesRules[preview.PlayerSpecies], Is.SameAs(preview.Progression.CurrentRules));
                    if (upgrade.Id == "threat-exposure")
                    {
                        Assert.That(preview.Progression.CurrentRules.FleeMovementSpeedBonus,
                            Is.EqualTo(originalRules.FleeMovementSpeedBonus));
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
                "36", "20", preview.BaseSeed.ToString(CultureInfo.InvariantCulture),
                preview.MaximumPopulation.ToString(CultureInfo.InvariantCulture),
                preview.MinimumPopulation.ToString(CultureInfo.InvariantCulture),
                "4", "0.01", "0", "1", "0", false, out message), Is.True, message);

            preview.StartSimulation();
            preview.Progression.AddCurrency(10);
            var run = preview.Run;
            var timeout = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.PhaseDecision && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
            Assert.That(preview.RewardOptionCount, Is.EqualTo(3));
            Assert.That(
                preview.GetRewardOptionId(2),
                Is.EqualTo(SpeciesUpgradeCatalog.PopulationReinforcementId));
            StringAssert.Contains("+1 FOX", preview.GetRewardOptionDisplayName(2));
            StringAssert.Contains("FREE", preview.GetRewardOptionDisplayName(2));
            StringAssert.DoesNotContain("COST", preview.GetRewardOptionDisplayName(2));
            var predatorSkillIds = new[] { "relentless-pursuit", "piercing-bite", "hunt-urgency", "brood-drive" };
            for (var index = 0; index < 2; index++)
            {
                var optionId = preview.GetRewardOptionId(index);
                Assert.That(Array.IndexOf(predatorSkillIds, optionId), Is.GreaterThanOrEqualTo(0));
                StringAssert.Contains("FREE", preview.GetRewardOptionDisplayName(index));
                StringAssert.DoesNotContain("COST", preview.GetRewardOptionDisplayName(index));
                StringAssert.DoesNotContain("FASTER", preview.GetRewardOptionDisplayName(index));
                StringAssert.DoesNotContain("ATTACK", preview.GetRewardOptionDisplayName(index));
                StringAssert.DoesNotContain("BLOCK", preview.GetRewardOptionDisplayName(index));
            }

            Assert.That(preview.Progression.TrySpend(preview.Progression.Currency), Is.True);
            Assert.That(preview.Progression.Currency, Is.Zero);
            Assert.That(preview.CanPurchaseReward(2), Is.True);
            Assert.That(preview.PurchaseReward(2), Is.True);
            Assert.That(preview.Progression.Currency, Is.Zero);
            Assert.That(run.UpgradeLoadout, Has.Count.EqualTo(1));
            Assert.That(run.UpgradeLoadout[0].PopulationToAdd, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator AuthoredMutationIsFreeAtPhaseBoundary()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
            preview.StopSimulation();
            typeof(SpeciesSimulationPreview)
                .GetField("bevExperimentalFeaturesEnabled", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(preview, false);
            Assert.That(preview.TryApplyContinuousPhases(true, "1", out var message), Is.True, message);
            Assert.That(preview.TryApplyGlobalSettingsForTicksWithStartingPopulations(
                "8", "8", preview.BaseSeed.ToString(CultureInfo.InvariantCulture),
                preview.MaximumPopulation.ToString(CultureInfo.InvariantCulture),
                preview.MinimumPopulation.ToString(CultureInfo.InvariantCulture),
                "4", "0.01", "0", "1", "0", false,
                "20", "10", "0", out message), Is.True, message);

            preview.StartSimulation();
            var timeout = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.PhaseDecision && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
            Assert.That(preview.RewardOptionCount, Is.GreaterThan(0));
            StringAssert.Contains("FREE", preview.GetRewardOptionDisplayName(0));
            StringAssert.DoesNotContain("COST", preview.GetRewardOptionDisplayName(0));
            Assert.That(preview.Progression.TrySpend(preview.Progression.Currency), Is.True);
            Assert.That(preview.Progression.Currency, Is.Zero);
            Assert.That(preview.CanPurchaseReward(0), Is.True);
            Assert.That(preview.PurchaseReward(0), Is.True);
            Assert.That(preview.Progression.Currency, Is.Zero);
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
                "36",
                "20",
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
