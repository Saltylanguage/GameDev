using UnityEngine;

namespace SaltyGame
{
    public sealed class ShelterInteractable : MonoBehaviour, IActivityTarget
    {
        GameObject shelterVisual;
        GameObject markerVisual;
        SpriteRenderer markerRenderer;
        InventoryState inventory;
        CampState camp;
        bool markerVisible;

        public string DisplayName => camp != null && camp.ShelterBuilt ? "Shelter" : "Shelter Site";
        public bool CanInteract => camp != null && !camp.ShelterBuilt && camp.CanBuildShelter(inventory);
        public Vector2 Position => transform.position;

        public void Initialize(GameObject shelterVisual, GameObject markerVisual, InventoryState inventory, CampState camp)
        {
            this.shelterVisual = shelterVisual;
            this.markerVisual = markerVisual;
            markerRenderer = markerVisual.GetComponent<SpriteRenderer>();
            this.inventory = inventory;
            this.camp = camp;
        }

        void Update()
        {
            if (markerVisible)
            {
                markerVisual.transform.localPosition = new Vector3(0f, 1.1f + Mathf.PingPong(Time.time * 1.5f, 0.18f), 0f);
                markerRenderer.color = new Color(1f, 0.82f, 0.12f, Mathf.Repeat(Time.time * 3f, 1f) < 0.5f ? 1f : 0.18f);
            }
        }

        public IActivity CreateActivity()
        {
            return new ShelterBuildActivity();
        }

        public void ApplyActivityResult(ActivityResult result)
        {
            if (result.Succeeded && camp.TryBuildShelter(inventory))
            {
                shelterVisual.SetActive(true);
                SetMarkerVisible(false);
            }
        }

        public void ResetForNewDay() { }

        public void SetMarkerVisible(bool visible)
        {
            var shouldShow = visible && camp != null && camp.CampfireBuilt && !camp.ShelterBuilt;
            if (markerVisible == shouldShow)
                return;

            markerVisible = shouldShow;
            markerVisual.SetActive(markerVisible);
        }
    }
}
