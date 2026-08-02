using UnityEngine;

namespace SaltyGame
{
    public sealed class CampfireInteractable : MonoBehaviour, IActivityTarget
    {
        const int BerriesPerMeal = 2;

        GameObject fireVisual;
        InventoryState inventory;
        CampState camp;

        public string DisplayName => camp != null && camp.CampfireBuilt ? "Campfire" : "Campfire Site";
        public bool CanInteract => camp != null && (camp.CampfireBuilt || camp.CanBuildCampfire(inventory));
        public Vector2 Position => transform.position;
        public bool IsBuilt => camp != null && camp.CampfireBuilt;
        public bool CanCook => camp != null && camp.CampfireBuilt && inventory.Get(ResourceId.Berries) >= BerriesPerMeal;

        public void Initialize(GameObject fireVisual, InventoryState inventory, CampState camp)
        {
            this.fireVisual = fireVisual;
            this.inventory = inventory;
            this.camp = camp;
        }

        public IActivity CreateActivity()
        {
            if (!camp.CampfireBuilt)
                return new CampfireBuildActivity();

            return CanCook ? new CookingActivity() : null;
        }

        public void ApplyActivityResult(ActivityResult result)
        {
            if (!result.Succeeded)
                return;

            if (!camp.CampfireBuilt && camp.TryBuildCampfire(inventory))
            {
                fireVisual.SetActive(true);
                return;
            }

            if (result.ResourceId == ResourceId.CookedMeal)
                inventory.TryRemove(ResourceId.Berries, BerriesPerMeal);
        }

        public bool TryEat(SurvivalState survival, out string message)
        {
            if (!camp.CampfireBuilt)
            {
                message = "Build the campfire before eating at camp.";
                return false;
            }

            if (inventory.TryRemove(ResourceId.CookedMeal, 1))
            {
                message = $"Ate cooked berries: -{survival.EatCookedMeal()} hunger.";
                return true;
            }

            if (inventory.TryRemove(ResourceId.Berries, 1))
            {
                message = $"Ate raw berries: -{survival.EatRawBerries()} hunger.";
                return true;
            }

            message = "No berries or cooked meals to eat.";
            return false;
        }

        public string GetActionPrompt()
        {
            if (!camp.CampfireBuilt)
                return $"Press [E] to build campfire ({CampState.CampfireWoodCost} wood, {CampState.CampfireStoneCost} stone)";

            return $"[E] Cook 2 berries | [F] Eat | [R] Sleep | {GetCookStatus()}";
        }

        public string GetCookStatus()
        {
            return CanCook ? "Cooked food restores more hunger." : $"Need {BerriesPerMeal} berries to cook.";
        }

        public void ResetForNewDay() { }
    }
}
