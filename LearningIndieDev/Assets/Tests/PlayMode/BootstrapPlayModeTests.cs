using System.Collections;
using NUnit.Framework;
using SaltyGame;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace SaltyGame.PlayModeTests
{
    public sealed class BootstrapPlayModeTests
    {
        [UnityTest]
        public IEnumerator BootstrapStartsTheRuntimeAndBuildsTargets()
        {
            yield return SceneManager.LoadSceneAsync("Boostrap");
            yield return null;

            var root = GameObject.Find("Game Runtime");
            Assert.That(root, Is.Not.Null);

            var runtime = root.GetComponent<GameRuntime>();
            Assert.That(runtime, Is.Not.Null);
            Assert.That(runtime.State, Is.EqualTo(GameState.Playing));
            Assert.That(runtime.World.IsBuilt, Is.True);
            Assert.That(runtime.World.PlayerTransform, Is.Not.Null);
            Assert.That(runtime.World.Targets.Count, Is.EqualTo(3));
        }
    }
}
