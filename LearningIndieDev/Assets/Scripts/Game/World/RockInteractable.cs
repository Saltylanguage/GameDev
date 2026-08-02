using UnityEngine;

namespace SaltyGame
{
    public sealed class RockInteractable : MonoBehaviour, IActivityTarget
    {
        GameObject visualRoot;
        bool depleted;

        public string DisplayName => "Rock";
        public bool CanInteract => !depleted;
        public Vector2 Position => transform.position;

        public void Initialize(GameObject visualRoot)
        {
            this.visualRoot = visualRoot;
        }

        public IActivity CreateActivity()
        {
            return new MiningActivity(6, 3);
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
