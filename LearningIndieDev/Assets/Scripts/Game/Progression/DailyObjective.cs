namespace SaltyGame
{
    public sealed class DailyObjective
    {
        public string StatusText(InventoryState inventory, CampState camp, StormScenario storm)
        {
            if (!camp.CampfireBuilt)
                return $"Objective: Build a campfire | Wood {inventory.Get(ResourceId.Wood)}/{CampState.CampfireWoodCost} | Stone {inventory.Get(ResourceId.Stone)}/{CampState.CampfireStoneCost}";

            return storm.StatusText(inventory, camp);
        }
    }
}
