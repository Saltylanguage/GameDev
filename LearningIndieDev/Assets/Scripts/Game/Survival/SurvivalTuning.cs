namespace SaltyGame
{
    public sealed class SurvivalTuning
    {
        public int MaxHunger { get; }
        public int MaxEnergy { get; }
        public int StartingHunger { get; }
        public int StartingEnergy { get; }
        public int RawBerryHungerRestore { get; }
        public int CookedMealHungerRestore { get; }
        public int HungryThreshold { get; }
        public int RestedSleepRecovery { get; }
        public int HungrySleepRecovery { get; }
        public int SleepHungerIncrease { get; }

        public SurvivalTuning(int maxHunger = 100, int maxEnergy = 100, int startingHunger = 20, int startingEnergy = 100, int rawBerryHungerRestore = 15, int cookedMealHungerRestore = 35, int hungryThreshold = 70, int restedSleepRecovery = 70, int hungrySleepRecovery = 35, int sleepHungerIncrease = 12)
        {
            MaxHunger = maxHunger;
            MaxEnergy = maxEnergy;
            StartingHunger = startingHunger;
            StartingEnergy = startingEnergy;
            RawBerryHungerRestore = rawBerryHungerRestore;
            CookedMealHungerRestore = cookedMealHungerRestore;
            HungryThreshold = hungryThreshold;
            RestedSleepRecovery = restedSleepRecovery;
            HungrySleepRecovery = hungrySleepRecovery;
            SleepHungerIncrease = sleepHungerIncrease;
        }

        public ActivityCost CostFor(ActivityKind kind)
        {
            return kind switch
            {
                ActivityKind.WoodChopping => new ActivityCost(20, 10),
                ActivityKind.Mining => new ActivityCost(25, 12),
                ActivityKind.Gathering => new ActivityCost(10, 6),
                ActivityKind.Cooking => new ActivityCost(5, 4),
                ActivityKind.Building => new ActivityCost(5, 4),
                _ => default
            };
        }
    }
}
