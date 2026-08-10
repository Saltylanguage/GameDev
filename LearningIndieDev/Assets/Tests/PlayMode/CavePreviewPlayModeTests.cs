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
        public IEnumerator CellularAutomataPrototypeCreatesAndAnimatesMixedLifePreview()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var runtime = Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>();

            Assert.That(runtime, Is.Not.Null);
            Assert.That(runtime.LifePreview, Is.Not.Null);
            Assert.That(runtime.LifePreview.Cells, Is.Not.Null);
            Assert.That(runtime.LifePreview.Generation, Is.Zero);

            yield return new WaitForSeconds(0.35f);

            Assert.That(runtime.LifePreview.Generation, Is.GreaterThanOrEqualTo(1));
        }
    }
}
