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
        public IEnumerator CellularAutomataPrototypeCreatesAndAnimatesTheCavePreview()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var runtime = Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>();

            Assert.That(runtime, Is.Not.Null);
            Assert.That(runtime.Preview, Is.Not.Null);
            Assert.That(runtime.Preview.Cave, Is.Not.Null);
            Assert.That(runtime.Preview.CompletedSteps, Is.Zero);
            Assert.That(runtime.LifePreview, Is.Not.Null);
            Assert.That(runtime.LifePreview.Cells, Is.Not.Null);
            Assert.That(runtime.LifePreview.Generation, Is.Zero);

            yield return new WaitForSeconds(0.35f);

            Assert.That(runtime.Preview.CompletedSteps, Is.GreaterThanOrEqualTo(1));
            Assert.That(runtime.LifePreview.Generation, Is.GreaterThanOrEqualTo(1));
        }
    }
}
