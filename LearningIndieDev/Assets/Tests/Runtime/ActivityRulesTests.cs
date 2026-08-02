using System;
using NUnit.Framework;
using SaltyGame;

namespace SaltyGame.Tests
{
    [TestFixture]
    public sealed class ActivityRulesTests
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
        public void ActivityDefinitionsRejectInvalidValues()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new WoodChoppingActivity(0, 3));
            Assert.Throws<ArgumentOutOfRangeException>(() => new WoodChoppingActivity(6, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MiningActivity(0, 3));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MiningActivity(6, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new GatheringActivity(0));
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
    }
}
