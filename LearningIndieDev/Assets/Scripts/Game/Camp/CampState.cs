namespace SaltyGame
{
    public sealed class CampState
    {
        public const int CampfireWoodCost = 4;
        public const int CampfireStoneCost = 2;
        public const int ShelterWoodCost = 4;

        public bool CampfireBuilt { get; private set; }
        public bool ShelterBuilt { get; private set; }

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
    }
}
