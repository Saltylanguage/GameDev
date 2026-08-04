namespace SaltyGame
{
    public enum ActivityKind
    {
        WoodChopping,
        Mining,
        Gathering,
        Cooking,
        Building
    }

    public readonly struct ActivityCost
    {
        public int Energy { get; }
        public int Hunger { get; }

        public ActivityCost(int energy, int hunger)
        {
            Energy = energy;
            Hunger = hunger;
        }
    }
}
