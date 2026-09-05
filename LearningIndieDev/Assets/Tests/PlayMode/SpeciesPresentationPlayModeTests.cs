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
            Assert.That(camera.GetComponent("SaltyGame.VM_SimulationShell"), Is.Not.Null);
            Assert.That(camera.GetComponent("SaltyGame.VM_SimulationBoard"), Is.Not.Null);
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
        public IEnumerator BevExperimentalFeaturesShowHerbivoreStatLineAfterRun()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var runtime = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>();
            Assert.That(runtime, Is.Not.Null);
            Assert.That(runtime.SpeciesPreview.BevExperimentalFeaturesEnabled, Is.False);
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
            StringAssert.Contains("herbivore stat line", message);

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
        public IEnumerator ContinuousPhaseDecisionResumesTheSamePreviewRun()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var runtime = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>();
            Assert.That(runtime, Is.Not.Null);

            var preview = runtime.SpeciesPreview;
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

            var viewModel = GameObject.Find("Prototype Camera")
                ?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(
                viewModel.GetType().GetProperty("PhaseDecisionVisibility")?.GetValue(viewModel)?.ToString(),
                Is.EqualTo("Visible"));
            Assert.That(
                viewModel.GetType().GetProperty("BoardVisibility")?.GetValue(viewModel)?.ToString(),
                Is.EqualTo("Visible"));

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
        public IEnumerator PhaseDecisionCanPurchaseLegacyUpgradeAndResumeSameRun()
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
            Assert.That(preview.GetRewardOptionId(0), Is.EqualTo(SpeciesUpgradeCatalog.FasterMovementId));
            Assert.That(preview.CanPurchaseReward(0), Is.True);
            var currencyAtBoundary = preview.Progression.Currency;
            var movementBefore = preview.ActiveSpeciesRules[preview.PlayerSpecies].MovementSpeed;

            Assert.That(preview.PurchaseReward(0), Is.True);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(preview.Run, Is.SameAs(run));
            Assert.That(run.Status, Is.EqualTo(SimulationRunStatus.Running));
            Assert.That(run.Tick, Is.EqualTo(2));
            Assert.That(preview.Progression.PurchasedUpgradeCount, Is.EqualTo(1));
            Assert.That(run.UpgradeLoadout.Count, Is.EqualTo(1));
            Assert.That(
                preview.ActiveSpeciesRules[preview.PlayerSpecies].MovementSpeed,
                Is.EqualTo(movementBefore + 0.5f));
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
