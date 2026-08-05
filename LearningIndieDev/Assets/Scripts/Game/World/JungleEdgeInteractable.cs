using UnityEngine;

namespace SaltyGame
{
    public sealed class JungleEdgeInteractable : MonoBehaviour, IActivityTarget
    {
        GameObject visualRoot;
        CampState camp;
        bool cleared;

        public string DisplayName => "Jungle Edge";
        public bool CanInteract => !cleared;
        public Vector2 Position => transform.position;

        public void Initialize(GameObject visualRoot, CampState camp)
        {
            this.visualRoot = visualRoot;
            this.camp = camp;
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
            visualRoot.SetActive(false);
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
            visualRoot.SetActive(true);
        }
    }
}
