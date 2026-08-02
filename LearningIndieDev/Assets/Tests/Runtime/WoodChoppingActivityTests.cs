using NUnit.Framework;
using SaltyGame;
using UnityEngine;

namespace SaltyGame.Tests
{
    [TestFixture]
    public sealed class WoodChoppingActivityTests
    {
        [Test]
        public void ThreeStrongHitsCompleteAndAwardThreeWood()
        {
            var activity = new WoodChoppingActivity(6, 3);
            activity.Submit(0.5f);
            activity.Submit(0.5f);
            activity.Submit(0.5f);

            Assert.That(activity.IsComplete, Is.True);
            Assert.That(activity.Result.ResourceId, Is.EqualTo(ResourceId.Wood));
            Assert.That(activity.Result.Amount, Is.EqualTo(3));
        }

        [Test]
        public void ActivityControllerDeliversCompletionRewardToInventory()
        {
            var inventory = new InventoryState();
            var controller = new ActivityController(inventory);
            var target = new TestTarget();

            Assert.That(controller.Start(target), Is.True);
            controller.Tick(0.4f);
            controller.SubmitHit();
            controller.SubmitHit();
            controller.SubmitHit();
            controller.Tick(0f);

            Assert.That(inventory.Get(ResourceId.Wood), Is.EqualTo(3));
            Assert.That(target.Completed, Is.True);
        }

        [Test]
        public void GatheringRewardsMoreBerriesForStrongTiming()
        {
            var activity = new GatheringActivity(3);
            activity.Submit(0.5f);
            activity.Submit(0.5f);
            activity.Submit(0.5f);

            Assert.That(activity.IsComplete, Is.True);
            Assert.That(activity.Result.ResourceId, Is.EqualTo(ResourceId.Berries));
            Assert.That(activity.Result.Amount, Is.EqualTo(6));
        }

        [Test]
        public void MiningThreeStrongHitsAwardThreeStone()
        {
            var activity = new MiningActivity(6, 3);
            activity.Submit(0.5f);
            activity.Submit(0.5f);
            activity.Submit(0.5f);

            Assert.That(activity.IsComplete, Is.True);
            Assert.That(activity.Result.ResourceId, Is.EqualTo(ResourceId.Stone));
            Assert.That(activity.Result.Amount, Is.EqualTo(3));
        }

        [Test]
        public void GameRuntimeBuildsTheFirstVerticalSlice()
        {
            var root = new GameObject("Game Runtime Test");
            var runtime = root.AddComponent<GameRuntime>();
            runtime.Initialize();

            Assert.That(runtime.State, Is.EqualTo(GameState.Playing));
            Assert.That(runtime.World.PlayerTransform, Is.Not.Null);
            Assert.That(runtime.World.Targets.Count, Is.EqualTo(3));
            Assert.That(runtime.Inventory.Get(ResourceId.Wood), Is.EqualTo(0));
            Assert.That(runtime.Activities.IsActive, Is.False);

            var target = runtime.World.Targets[0];
            target.ApplyActivityResult(new ActivityResult(true, ResourceId.Wood, 3));
            Assert.That(target.CanInteract, Is.False);
            runtime.World.ResetTargetsForNewDay();
            Assert.That(target.CanInteract, Is.True);

            Object.DestroyImmediate(runtime.World.gameObject);
            Object.DestroyImmediate(root);
        }

        [Test]
        public void CompletingActivitiesAdvancesTheDayCycle()
        {
            var clock = new GameClock();

            Assert.That(clock.Day, Is.EqualTo(1));
            Assert.That(clock.TimeOfDay, Is.EqualTo(TimeOfDay.Morning));

            Assert.That(clock.AdvanceActivity(), Is.False);
            Assert.That(clock.TimeOfDay, Is.EqualTo(TimeOfDay.Afternoon));

            Assert.That(clock.AdvanceActivity(), Is.False);
            Assert.That(clock.TimeOfDay, Is.EqualTo(TimeOfDay.Night));

            Assert.That(clock.AdvanceActivity(), Is.True);
            Assert.That(clock.Day, Is.EqualTo(2));
            Assert.That(clock.TimeOfDay, Is.EqualTo(TimeOfDay.Morning));
        }

        sealed class TestTarget : IActivityTarget
        {
            public string DisplayName => "Test Tree";
            public bool CanInteract => !Completed;
            public UnityEngine.Vector2 Position => UnityEngine.Vector2.zero;
            public bool Completed { get; private set; }
            public IActivity CreateActivity() => new WoodChoppingActivity(6, 3);
            public void ApplyActivityResult(ActivityResult result) => Completed = result.Succeeded;
            public void ResetForNewDay() => Completed = false;
        }
    }
}
