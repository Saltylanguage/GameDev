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

            var selectedScenarioIndex = viewModel.GetType().GetProperty("SelectedScenarioIndex");
            var canStart = viewModel.GetType().GetProperty("CanStart");
            Assert.That(selectedScenarioIndex, Is.Not.Null);
            Assert.That(canStart, Is.Not.Null);

            selectedScenarioIndex.SetValue(viewModel, 2);
            Assert.That(root.GetComponent<CellularAutomataPrototypeRuntime>().SpeciesPreview.SelectedScenario.name,
                Is.EqualTo("Wetland"));
            Assert.That(canStart.GetValue(viewModel), Is.True);
            selectedScenarioIndex.SetValue(viewModel, 1);
            Assert.That(canStart.GetValue(viewModel), Is.True);
            selectedScenarioIndex.SetValue(viewModel, 0);
            Assert.That(canStart.GetValue(viewModel), Is.True);
            selectedScenarioIndex.SetValue(viewModel, 2);
            Assert.That(canStart.GetValue(viewModel), Is.True);
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
        public IEnumerator DefaultFeaturesShowHerbivoreStatLineAfterRun()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var runtime = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>();
            Assert.That(runtime, Is.Not.Null);
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

            var viewModelType = viewModel.GetType();
            viewModelType.GetProperty("DeveloperMode")?.SetValue(viewModel, true);
            viewModelType.GetProperty("MaximumPopulationText")?.SetValue(viewModel, "100");
            viewModelType.GetProperty("PlantStartingPopulationText")?.SetValue(viewModel, "40");
            viewModelType.GetProperty("HerbivoreStartingPopulationText")?.SetValue(viewModel, "40");
            viewModelType.GetProperty("CarnivoreStartingPopulationText")?.SetValue(viewModel, "40");

            var runSeedBefore = runtime.SpeciesPreview.Run.Seed;
            var applySettingsCommand = viewModelType.GetProperty("ApplySettingsCommand")?.GetValue(viewModel);
            applySettingsCommand?.GetType().GetMethod("Execute")?.Invoke(applySettingsCommand, new object[] { null });

            Assert.That(runtime.SpeciesPreview.Run.Seed, Is.EqualTo(runSeedBefore));
            Assert.That(viewModelType.GetProperty("PlantStartingPopulationText")?.GetValue(viewModel), Is.EqualTo("40"));
            Assert.That(viewModelType.GetProperty("HerbivoreStartingPopulationText")?.GetValue(viewModel), Is.EqualTo("40"));
            Assert.That(viewModelType.GetProperty("CarnivoreStartingPopulationText")?.GetValue(viewModel), Is.EqualTo("40"));
            var settingsMessage = viewModelType.GetProperty("SettingsMessage")?.GetValue(viewModel) as string;
            StringAssert.Contains("total 120 cannot exceed maximum population 100", settingsMessage);
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
            Assert.That(run.TargetTicks, Is.EqualTo(20));
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
            Assert.That(run.Tick, Is.EqualTo(20));
            Assert.That(run.Status, Is.EqualTo(SimulationRunStatus.Complete));
            Assert.That(
                viewModel.GetType().GetProperty("ResultsTitleText")?.GetValue(viewModel)?.ToString(),
                Is.EqualTo("Expedition complete"));
            Assert.That(
                viewModel.GetType().GetProperty("PlayNextSimulationText")?.GetValue(viewModel)?.ToString(),
                Is.EqualTo("START NEW EXPEDITION"));
        }

        [UnityTest]
        public IEnumerator PhaseDecisionCanPurchaseAuthoredUpgradeAndResumeSameRun()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var runtime = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>();
            Assert.That(runtime, Is.Not.Null);

            var preview = runtime.SpeciesPreview;
            Assert.That(preview.TryApplyContinuousPhases(true, "2", out var phaseMessage), Is.True, phaseMessage);
            Assert.That(preview.TryApplyGlobalSettingsForTicks(
                "8",
                "8",
                preview.BaseSeed.ToString(CultureInfo.InvariantCulture),
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
            Assert.That(run.Tick, Is.EqualTo(20));
            Assert.That(run.Status, Is.EqualTo(SimulationRunStatus.Complete));
            Assert.That(preview.CanPurchaseReward(0), Is.False);
        }

        [UnityTest]
        public IEnumerator CoupledResponseAppliesToTheCounterpartAtTheSameBoundary()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
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
        public IEnumerator FiveHerbivoreSkillsRetainLevelsAndAcquisitionsAcrossPhaseBoundaries()
        {
            var ids = new[] { "tough-hide", "efficient-digestion", "crowding-tolerance",
                "reproductive-drive", "threat-exposure" };
            for (var skillIndex = 0; skillIndex < ids.Length; skillIndex++)
            {
                yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
                yield return null;
                var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
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
                // Preload one level so nine reward boundaries reach the Level 10 cap.
                Assert.That(preview.Progression.TryPurchase(upgrade), Is.True);
                for (var phase = 1; phase <= 9; phase++)
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
                Assert.That(preview.Progression.CanPurchase(upgrade), Is.False);
                StringAssert.Contains("MAX LEVEL", preview.GetRewardOptionDisplayName(0));
                preview.EndSimulation();
            }
        }

        [UnityTest]
        public IEnumerator PredatorPhaseRewardsUseAuthoredPredatorSkills()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
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

            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            runtime = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>();
            viewModel = GameObject.Find("Prototype Camera")
                ?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(runtime, Is.Not.Null);
            Assert.That(viewModel, Is.Not.Null);

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
