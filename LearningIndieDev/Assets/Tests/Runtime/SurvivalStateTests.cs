using NUnit.Framework;
using SaltyGame;

namespace SaltyGame.Tests
{
    [TestFixture]
    public sealed class SurvivalStateTests
    {
        [Test]
        public void ActivitiesSpendConfiguredEnergyAndIncreaseHunger()
        {
            var survival = new SurvivalState(new SurvivalTuning());

            survival.CompleteActivity(ActivityKind.Mining);

            Assert.That(survival.Energy, Is.EqualTo(75));
            Assert.That(survival.Hunger, Is.EqualTo(32));
        }

        [Test]
        public void CookedFoodRestoresMoreHungerThanRawBerries()
        {
            var raw = new SurvivalState(new SurvivalTuning(startingHunger: 60));
            var cooked = new SurvivalState(new SurvivalTuning(startingHunger: 60));

            raw.EatRawBerries();
            cooked.EatCookedMeal();

            Assert.That(cooked.Hunger, Is.LessThan(raw.Hunger));
        }

        [Test]
        public void SleepingHungryReducesEnergyRecovery()
        {
            var rested = new SurvivalState(new SurvivalTuning(startingEnergy: 0, startingHunger: 20));
            var hungry = new SurvivalState(new SurvivalTuning(startingEnergy: 0, startingHunger: 80));

            var restedResult = rested.Sleep();
            var hungryResult = hungry.Sleep();

            Assert.That(restedResult.EnergyRecovered, Is.GreaterThan(hungryResult.EnergyRecovered));
            Assert.That(hungryResult.SleptHungry, Is.True);
        }
    }
}
