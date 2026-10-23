namespace SaltyGame
{
    // Numeric values are part of serialized journey assets; preserve them when extending this enum.
    public enum JourneyNodeKind
    {
        Simulation = 0,
        Condition = 1,
        Upgrade = 2,
        Event = 3,
        Finale = 4,
        Reward = 5,
        Shop = 6,
        Refresh = 7,
    }
}
