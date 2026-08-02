using UnityEngine;

namespace SaltyGame
{
    public sealed class TreeInteractable : MonoBehaviour, IActivityTarget
    {
        GameObject visualRoot;
        bool depleted;

        public string DisplayName => "Tree";
        public bool CanInteract => !depleted;
        public Vector2 Position => transform.position;

        public void Initialize(GameObject visualRoot)
        {
            this.visualRoot = visualRoot;
        }

        public IActivity CreateActivity()
        {
            return new WoodChoppingActivity(health: 6, rewardAmount: 3);
        }

        public void ApplyActivityResult(ActivityResult result)
        {
            if (!result.Succeeded)
                return;

            depleted = true;
            visualRoot.SetActive(false);
        }
    }
}
