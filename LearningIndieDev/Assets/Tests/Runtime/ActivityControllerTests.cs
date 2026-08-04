using NUnit.Framework;
using SaltyGame;
using UnityEngine;

namespace SaltyGame.Tests
{
    [TestFixture]
    public sealed class ActivityControllerTests
    {
        [Test]
        public void ActivityControllerDeliversCompletionRewardToInventory()
        {
            var inventory = new InventoryState();
            var survival = new SurvivalState(new SurvivalTuning());
            var controller = new ActivityController(inventory, survival);
            var target = new TestTarget();

            Assert.That(controller.Start(target), Is.True);
            controller.Tick(0.4f);
            controller.SubmitHit();
            controller.SubmitHit();
            controller.SubmitHit();

            Assert.That(controller.Tick(0f), Is.True);
            Assert.That(inventory.Get(ResourceId.Wood), Is.EqualTo(3));
            Assert.That(target.Completed, Is.True);
            Assert.That(survival.Energy, Is.EqualTo(80));
        }

        [Test]
        public void ActivityControllerRejectsWorkThePlayerCannotAfford()
        {
            var controller = new ActivityController(new InventoryState(), new SurvivalState(new SurvivalTuning(startingEnergy: 0)));

            Assert.That(controller.Start(new TestTarget()), Is.False);
            Assert.That(controller.LastFailureMessage, Does.Contain("Too tired"));
        }

        sealed class TestTarget : IActivityTarget
        {
            public string DisplayName => "Test Tree";
            public ActivityKind Kind => ActivityKind.WoodChopping;
            public bool RequiresTimingInput => true;
            public bool CanInteract => !Completed;
            public Vector2 Position => Vector2.zero;
            public bool Completed { get; private set; }
            public IActivity CreateActivity() => new WoodChoppingActivity(6, 3);
            public void ApplyActivityResult(ActivityResult result) => Completed = result.Succeeded;
            public void ResetForNewDay() => Completed = false;
        }
    }
}
