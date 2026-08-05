using UnityEngine;

namespace SaltyGame
{
    public sealed class SurvivorInteractable : MonoBehaviour, IActivityTarget
    {
        GameObject visualRoot;
        InventoryState inventory;
        bool helpedToday;

        public string DisplayName => "Mara";
        public bool CanInteract => !helpedToday;
        public Vector2 Position => transform.position;

        public void Initialize(GameObject visualRoot, InventoryState inventory)
        {
            this.visualRoot = visualRoot;
            this.inventory = inventory;
        }

        public IActivity CreateActivity()
        {
            return null;
        }

        public bool TrySendScavenging(out string message)
        {
            if (helpedToday)
            {
                message = "Mara is already out scavenging today.";
                return false;
            }

            helpedToday = true;
            inventory.Add(ResourceId.Berries, 1);
            transform.localPosition = new Vector2(3.5f, 0.2f);
            message = "Mara is scavenging. She brought back 1 bonus berry.";
            return true;
        }

        public void ApplyActivityResult(ActivityResult result)
        {
            if (!result.Succeeded)
                return;

            helpedToday = true;
            inventory.Add(ResourceId.Berries, 1);
        }

        public void SetTimeOfDay(TimeOfDay timeOfDay)
        {
            transform.localPosition = timeOfDay switch
            {
                TimeOfDay.Afternoon => new Vector2(3.5f, 0.2f),
                TimeOfDay.Night => new Vector2(-0.1f, -1.45f),
                _ => new Vector2(-1.1f, 0.45f)
            };
        }

        public string GetActionPrompt()
        {
            return "Press [E] send Mara scavenging | No time cost | +1 bonus berry";
        }

        public void ResetForNewDay()
        {
            helpedToday = false;
            visualRoot.SetActive(true);
        }
    }
}
