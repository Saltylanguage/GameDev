using UnityEngine;

namespace SaltyGame
{
    public sealed class JungleEdgeInteractable : MonoBehaviour, IActivityTarget
    {
        GameObject blockedVisualRoot;
        GameObject clearedVisualRoot;
        CampState camp;
        bool cleared;

        public string DisplayName => "Jungle Edge";
        public bool CanInteract => !cleared;
        public Vector2 Position => transform.position;

        public void Initialize(GameObject blockedVisualRoot, GameObject clearedVisualRoot, CampState camp)
        {
            this.blockedVisualRoot = blockedVisualRoot;
            this.clearedVisualRoot = clearedVisualRoot;
            this.camp = camp;
            this.blockedVisualRoot.SetActive(true);
            this.clearedVisualRoot.SetActive(false);
        }

        public IActivity CreateActivity()
        {
            return camp != null && camp.CrudeAxeCrafted
                ? new WoodChoppingActivity(4, 8)
                : new WoodChoppingActivity(8, 4);
        }

        public void ApplyActivityResult(ActivityResult result)
        {
            if (!result.Succeeded)
                return;

            cleared = true;
            blockedVisualRoot.SetActive(false);
            clearedVisualRoot.SetActive(true);
        }

        public string GetActionPrompt()
        {
            return camp != null && camp.CrudeAxeCrafted
                ? "[E] Clear jungle | 4 hits with axe"
                : "[E] Clear jungle | 8 hits by hand";
        }

        public void ResetForNewDay()
        {
            cleared = false;
            blockedVisualRoot.SetActive(true);
            clearedVisualRoot.SetActive(false);
        }
    }
}
