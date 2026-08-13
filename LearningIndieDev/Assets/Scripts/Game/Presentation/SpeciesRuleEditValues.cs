namespace SaltyGame
{
    /// <summary>
    /// Presentation-friendly values used to edit one species without exposing
    /// the preview's private draft or Unity/Noesis types to the simulation.
    /// </summary>
    public sealed class SpeciesRuleEditValues
    {
        public bool MovementEnabled;
        public string MovementSpeed;
        public int MovementPattern;
        public bool AttackEnabled;
        public string AttackAmount;
        public int AttackPattern;
        public string BlockAmount;
        public int BlockPattern;
        public int DietTarget;
        public int DietPattern;
        public int ReproductionPattern;
        public bool ReproductionEnabled;
        public string ReproductionChance;
        public string ReproductionNeighborCount;
        public string ReproductionFoodRequired;
        public string MaxReproductionGroupSize;
        public string StartingEnergy;
        public string ForageBelowEnergy;
        public string EnergyValue;
        public string Metabolism;
        public string VisionRange;
        public string Intelligence;
        public bool WiltEnabled;
        public string WiltChance;
        public string CrowdingEnergyPenalty;
        public string StartingFoodReserve;
        public bool SeedDropEnabled;
        public string SeedDropChance;
    }
}
