using UnityEngine;

namespace SaltyGame
{
    public sealed class BerryBushInteractable : MonoBehaviour, IActivityTarget
    {
        GameObject visualRoot;
        bool depleted;

        public string DisplayName => "Berry Bush";
        public bool CanInteract => !depleted;
        public Vector2 Position => transform.position;

        public void Initialize(GameObject visualRoot)
        {
            this.visualRoot = visualRoot;
        }

        public IActivity CreateActivity()
        {
            return new GatheringActivity(requiredActions: 3);
        }

        public void ApplyActivityResult(ActivityResult result)
        {
            if (!result.Succeeded)
                return;

            depleted = true;
            visualRoot.SetActive(false);
        }

        public void ResetForNewDay()
        {
            depleted = false;
            visualRoot.SetActive(true);
        }
    }
}
