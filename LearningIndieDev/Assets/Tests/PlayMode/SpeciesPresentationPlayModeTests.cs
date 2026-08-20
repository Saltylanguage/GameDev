using System;
using System.Collections;
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
    }
}
