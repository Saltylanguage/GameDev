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
        public IEnumerator BootstrapCreatesAndAnimatesTheCavePreview()
        {
            yield return SceneManager.LoadSceneAsync("Boostrap");
            yield return null;

            var runtime = GameObject.Find("Game Runtime").GetComponent<GameRuntime>();

            Assert.That(runtime.CavePreview, Is.Not.Null);
            Assert.That(runtime.CavePreview.Cave, Is.Not.Null);
            Assert.That(runtime.CavePreview.CompletedSteps, Is.Zero);

            yield return new WaitForSeconds(0.35f);

            Assert.That(runtime.CavePreview.CompletedSteps, Is.GreaterThanOrEqualTo(1));
        }
    }
}
