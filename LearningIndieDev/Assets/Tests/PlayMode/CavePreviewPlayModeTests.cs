using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace SaltyGame.PlayModeTests
{
    public sealed class CavePreviewPlayModeTests
    {
        [UnityTest]
        public IEnumerator CellularAutomataPrototypeCreatesAndAnimatesTheSpeciesPreview()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var runtime = Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>();

            Assert.That(runtime, Is.Not.Null);
            Assert.That(runtime.SpeciesPreview, Is.Not.Null);
            Assert.That(runtime.SpeciesPreview.Run, Is.Not.Null);
            Assert.That(runtime.SpeciesPreview.Run.Status, Is.EqualTo(SimulationRunStatus.Ready));

            runtime.SpeciesPreview.StartSimulation();

            yield return new WaitForSeconds(0.35f);

            Assert.That(runtime.SpeciesPreview.Run.Tick, Is.GreaterThanOrEqualTo(1));
        }
    }
}
