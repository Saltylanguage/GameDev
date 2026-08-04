namespace SaltyGame
{
    public sealed class StormScenario
    {
        const int StormDay = 2;

        public bool IsResolved { get; private set; }

        public string StatusText(InventoryState inventory, CampState camp)
        {
            if (IsResolved)
                return "Objective complete: You weathered the storm. Keep the camp supplied.";

            if (!camp.ShelterBuilt)
                return $"Storm forecast: Build shelter before sleeping on Day {StormDay} | Wood {inventory.Get(ResourceId.Wood)}/{CampState.ShelterWoodCost}";

            return $"Storm forecast: Shelter ready. Sleep at camp on Day {StormDay} to weather the storm.";
        }

        public string Resolve(int endingDay, InventoryState inventory, CampState camp, SurvivalState survival)
        {
            if (IsResolved)
                return "Morning returns to a stronger camp.";

            if (endingDay != StormDay)
                return StatusText(inventory, camp);

            IsResolved = true;
            if (camp.ShelterBuilt)
                return "The storm passed safely under your shelter.";

            survival.ApplyStormExposure();
            return "The storm hit your open camp: -25 energy, +20 hunger. You can recover.";
        }
    }
}
