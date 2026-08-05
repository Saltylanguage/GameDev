namespace SaltyGame
{
    public sealed class CampState
    {
        public const int CampfireWoodCost = 4;
        public const int CampfireStoneCost = 2;
        public const int ShelterWoodCost = 4;
        public const int CrudeAxeWoodCost = 2;
        public const int CrudeAxeStoneCost = 2;

        public bool CampfireBuilt { get; private set; }
        public bool ShelterBuilt { get; private set; }
        public bool CrudeAxeCrafted { get; private set; }

        public bool CanBuildCampfire(InventoryState inventory)
        {
            return !CampfireBuilt && inventory.Get(ResourceId.Wood) >= CampfireWoodCost && inventory.Get(ResourceId.Stone) >= CampfireStoneCost;
        }

        public bool TryBuildCampfire(InventoryState inventory)
        {
            if (!CanBuildCampfire(inventory))
                return false;

            inventory.TryRemove(ResourceId.Wood, CampfireWoodCost);
            inventory.TryRemove(ResourceId.Stone, CampfireStoneCost);
            CampfireBuilt = true;
            return true;
        }

        public bool CanBuildShelter(InventoryState inventory)
        {
            return CampfireBuilt && !ShelterBuilt && inventory.Get(ResourceId.Wood) >= ShelterWoodCost;
        }

        public bool TryBuildShelter(InventoryState inventory)
        {
            if (!CanBuildShelter(inventory))
                return false;

            inventory.TryRemove(ResourceId.Wood, ShelterWoodCost);
            ShelterBuilt = true;
            return true;
        }

        public bool CanCraftCrudeAxe(InventoryState inventory)
        {
            return CampfireBuilt && !CrudeAxeCrafted && inventory.Get(ResourceId.Wood) >= CrudeAxeWoodCost && inventory.Get(ResourceId.Stone) >= CrudeAxeStoneCost;
        }

        public bool TryCraftCrudeAxe(InventoryState inventory)
        {
            if (!CanCraftCrudeAxe(inventory))
                return false;

            inventory.TryRemove(ResourceId.Wood, CrudeAxeWoodCost);
            inventory.TryRemove(ResourceId.Stone, CrudeAxeStoneCost);
            CrudeAxeCrafted = true;
            return true;
        }
    }
}
