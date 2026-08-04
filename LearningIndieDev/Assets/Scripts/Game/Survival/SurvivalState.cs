using System;

namespace SaltyGame
{
    public sealed class SurvivalState
    {
        readonly SurvivalTuning tuning;

        public int Hunger { get; private set; }
        public int Energy { get; private set; }

        public SurvivalState(SurvivalTuning tuning)
        {
            this.tuning = tuning ?? throw new ArgumentNullException(nameof(tuning));
            Hunger = tuning.StartingHunger;
            Energy = tuning.StartingEnergy;
        }

        public bool CanStart(ActivityKind kind)
        {
            return Energy >= tuning.CostFor(kind).Energy;
        }

        public string CannotStartMessage(ActivityKind kind)
        {
            var cost = tuning.CostFor(kind);
            return $"Too tired: {cost.Energy} energy required ({Energy} available).";
        }

        public void CompleteActivity(ActivityKind kind)
        {
            var cost = tuning.CostFor(kind);
            Energy = Math.Max(0, Energy - cost.Energy);
            Hunger = Math.Min(tuning.MaxHunger, Hunger + cost.Hunger);
        }

        public int EatRawBerries()
        {
            Hunger = Math.Max(0, Hunger - tuning.RawBerryHungerRestore);
            return tuning.RawBerryHungerRestore;
        }

        public int EatCookedMeal()
        {
            Hunger = Math.Max(0, Hunger - tuning.CookedMealHungerRestore);
            return tuning.CookedMealHungerRestore;
        }

        public SleepResult Sleep()
        {
            var sleptHungry = Hunger >= tuning.HungryThreshold;
            var recovery = sleptHungry ? tuning.HungrySleepRecovery : tuning.RestedSleepRecovery;
            Energy = Math.Min(tuning.MaxEnergy, Energy + recovery);
            Hunger = Math.Min(tuning.MaxHunger, Hunger + tuning.SleepHungerIncrease);
            return new SleepResult(recovery, sleptHungry, Hunger, Energy);
        }

        public void ApplyStormExposure()
        {
            Energy = Math.Max(0, Energy - 25);
            Hunger = Math.Min(tuning.MaxHunger, Hunger + 20);
        }
    }

    public readonly struct SleepResult
    {
        public int EnergyRecovered { get; }
        public bool SleptHungry { get; }
        public int Hunger { get; }
        public int Energy { get; }

        public SleepResult(int energyRecovered, bool sleptHungry, int hunger, int energy)
        {
            EnergyRecovered = energyRecovered;
            SleptHungry = sleptHungry;
            Hunger = hunger;
            Energy = energy;
        }
    }
}
