using NUnit.Framework;
using SaltyGame;
using UnityEngine;

namespace SaltyGame.Tests
{
    [TestFixture]
    public sealed class GameRuntimeTests
    {
        GameObject runtimeRoot;

        [TearDown]
        public void TearDown()
        {
            if (runtimeRoot != null)
                Object.DestroyImmediate(runtimeRoot);
        }

        [Test]
        public void GameRuntimeBuildsTheFirstVerticalSliceAndResetsTargets()
        {
            runtimeRoot = new GameObject("Game Runtime Test");
            var runtime = runtimeRoot.AddComponent<GameRuntime>();
            runtime.Initialize();

            Assert.That(runtime.State, Is.EqualTo(GameState.Playing));
            Assert.That(runtime.World.IsBuilt, Is.True);
            Assert.That(runtime.World.transform.parent, Is.EqualTo(runtimeRoot.transform));
            Assert.That(runtime.World.PlayerTransform, Is.Not.Null);
            Assert.That(runtime.World.Targets.Count, Is.EqualTo(3));

            var worldChildCount = runtime.World.transform.childCount;
            runtime.World.Build(null);
            Assert.That(runtime.World.transform.childCount, Is.EqualTo(worldChildCount));
            Assert.That(runtime.Inventory.Get(ResourceId.Wood), Is.EqualTo(0));
            Assert.That(runtime.Activities.IsActive, Is.False);

            var target = runtime.World.Targets[0];
            target.ApplyActivityResult(new ActivityResult(true, ResourceId.Wood, 3));
            Assert.That(target.CanInteract, Is.False);
            runtime.World.ResetTargetsForNewDay();
            Assert.That(target.CanInteract, Is.True);
        }
    }
}
