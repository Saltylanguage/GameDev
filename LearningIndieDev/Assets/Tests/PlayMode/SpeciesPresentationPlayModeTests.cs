using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace SaltyGame.PlayModeTests
{
    public sealed class SpeciesPresentationPlayModeTests
    {
        [UnityTest]
        public IEnumerator CellularPrototypeInitializesEveryAuthoredAnimalSprite()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;
            yield return null;

            var camera = GameObject.Find("Prototype Camera");
            var viewModel = camera?.GetComponent("SaltyGame.SpeciesSimulationViewModel");
            Assert.That(viewModel, Is.Not.Null, "CellularAutomataPrototype must initialize SpeciesSimulationViewModel.");

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
                ?.GetComponent("SaltyGame.SpeciesSimulationViewModel");
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
    }
}
