using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
    public sealed class WorldRuntime : MonoBehaviour
    {
        readonly List<Sprite> generatedSprites = new List<Sprite>();
        Camera worldCamera;
        ShelterInteractable shelterSite;
        SurvivorInteractable survivor;
        SpriteRenderer interactionMarker;
        public Transform PlayerTransform { get; private set; }
        public IReadOnlyList<IActivityTarget> Targets { get; private set; }
        public bool IsBuilt { get; private set; }

        public void Build(Camera sceneCamera)
        {
            Build(sceneCamera, new InventoryState(), new CampState());
        }

        public void Build(Camera sceneCamera, InventoryState inventory, CampState camp)
        {
            if (IsBuilt)
                return;

            IsBuilt = true;
            var camera = sceneCamera;
            if (camera == null)
            {
                camera = new GameObject("Main Camera").AddComponent<Camera>();
                camera.tag = "MainCamera";
                camera.transform.SetParent(transform, false);
            }

            camera.orthographic = true;
            camera.orthographicSize = 5.5f;
            camera.transform.position = new Vector3(0f, 0f, -10f);
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.20f, 0.63f, 0.77f);
            worldCamera = camera;

            MakeSprite("Sand", Vector2.zero, new Vector3(13f, 9f, 1f), new Color(0.94f, 0.78f, 0.43f), -10, transform);
            MakeSprite("WaterEdge", new Vector2(0f, -4.2f), new Vector3(13f, 0.5f, 1f), new Color(0.11f, 0.48f, 0.72f), -9, transform);
            var playerRoot = new GameObject("Player");
            playerRoot.transform.SetParent(transform, false);
            playerRoot.transform.position = new Vector2(-2.8f, -0.8f);
            MakeSprite("Player Shadow", new Vector2(0f, -0.42f), new Vector3(0.72f, 0.18f, 1f), new Color(0.20f, 0.15f, 0.10f, 0.45f), 1, playerRoot.transform);
            MakeSprite("Player Body", new Vector2(0f, -0.05f), new Vector3(0.42f, 0.58f, 1f), new Color(0.94f, 0.33f, 0.22f), 2, playerRoot.transform);
            MakeSprite("Player Head", new Vector2(0f, 0.38f), new Vector3(0.34f, 0.34f, 1f), new Color(0.98f, 0.68f, 0.42f), 3, playerRoot.transform);
            MakeSprite("Player Pack", new Vector2(-0.25f, -0.04f), new Vector3(0.16f, 0.35f, 1f), new Color(0.30f, 0.18f, 0.10f), 2, playerRoot.transform);
            PlayerTransform = playerRoot.transform;
            var rockRoot = new GameObject("Rock");
            rockRoot.transform.SetParent(transform, false);
            rockRoot.transform.position = new Vector2(-0.4f, 1.9f);
            MakeSprite("Rock Shadow", new Vector2(0f, -0.32f), new Vector3(1.25f, 0.22f, 1f), new Color(0.28f, 0.22f, 0.15f, 0.45f), 0, rockRoot.transform);
            var rockFace = MakeSprite("Stone", new Vector2(0f, 0.05f), new Vector3(1.15f, 0.72f, 1f), new Color(0.45f, 0.46f, 0.42f), 1, rockRoot.transform);
            rockFace.transform.localRotation = Quaternion.Euler(0f, 0f, -8f);
            var rockHighlight = MakeSprite("Stone Highlight", new Vector2(-0.2f, 0.18f), new Vector3(0.42f, 0.16f, 1f), new Color(0.68f, 0.69f, 0.62f), 2, rockRoot.transform);
            rockHighlight.transform.localRotation = Quaternion.Euler(0f, 0f, -8f);
            var rock = rockRoot.AddComponent<RockInteractable>();
            rock.Initialize(rockRoot);

            var treeRoot = new GameObject("Tree");
            treeRoot.transform.SetParent(transform, false);
            treeRoot.transform.position = new Vector2(2.2f, -0.2f);
            MakeSprite("Tree Shadow", new Vector2(0f, -1.05f), new Vector3(1.55f, 0.22f, 1f), new Color(0.28f, 0.22f, 0.15f, 0.45f), 0, treeRoot.transform);
            MakeSprite("Trunk", new Vector2(0f, -0.18f), new Vector3(0.58f, 2.15f, 1f), new Color(0.39f, 0.20f, 0.08f), 1, treeRoot.transform);
            MakeSprite("Branch Left", new Vector2(-0.35f, 0.58f), new Vector3(0.75f, 0.16f, 1f), new Color(0.39f, 0.20f, 0.08f), 1, treeRoot.transform).transform.localRotation = Quaternion.Euler(0f, 0f, 25f);
            MakeSprite("Leaves Left", new Vector2(-0.55f, 1.2f), new Vector3(1.35f, 1.2f, 1f), new Color(0.07f, 0.34f, 0.13f), 1, treeRoot.transform);
            MakeSprite("Leaves Right", new Vector2(0.48f, 1.3f), new Vector3(1.3f, 1.25f, 1f), new Color(0.10f, 0.46f, 0.18f), 2, treeRoot.transform);
            var tree = treeRoot.AddComponent<TreeInteractable>();
            tree.Initialize(treeRoot, camp);

            var bushRoot = new GameObject("Berry Bush");
            bushRoot.transform.SetParent(transform, false);
            bushRoot.transform.position = new Vector2(-1.1f, -0.7f);
            MakeSprite("Bush Shadow", new Vector2(0f, -0.36f), new Vector3(1.35f, 0.18f, 1f), new Color(0.28f, 0.22f, 0.15f, 0.45f), 0, bushRoot.transform);
            MakeSprite("Bush Foliage", Vector2.zero, new Vector3(1.25f, 0.9f, 1f), new Color(0.10f, 0.30f, 0.16f), 1, bushRoot.transform);
            MakeSprite("Berry Left", new Vector2(-0.32f, 0.13f), new Vector3(0.18f, 0.18f, 1f), new Color(0.72f, 0.12f, 0.36f), 2, bushRoot.transform);
            MakeSprite("Berry Center", new Vector2(0.05f, 0.28f), new Vector3(0.2f, 0.2f, 1f), new Color(0.86f, 0.16f, 0.42f), 2, bushRoot.transform);
            MakeSprite("Berry Right", new Vector2(0.34f, -0.02f), new Vector3(0.18f, 0.18f, 1f), new Color(0.65f, 0.08f, 0.32f), 2, bushRoot.transform);
            var bush = bushRoot.AddComponent<BerryBushInteractable>();
            bush.Initialize(bushRoot);

            var campRoot = new GameObject("Campfire Site");
            campRoot.transform.SetParent(transform, false);
            campRoot.transform.position = new Vector2(-1.2f, -2.2f);
            MakeSprite("Fire Ring", Vector2.zero, new Vector3(1.35f, 0.38f, 1f), new Color(0.24f, 0.22f, 0.20f), 1, campRoot.transform);
            MakeSprite("Fire Stone Left", new Vector2(-0.48f, 0.05f), new Vector3(0.28f, 0.28f, 1f), new Color(0.38f, 0.36f, 0.31f), 2, campRoot.transform);
            MakeSprite("Fire Stone Right", new Vector2(0.48f, 0.05f), new Vector3(0.28f, 0.28f, 1f), new Color(0.38f, 0.36f, 0.31f), 2, campRoot.transform);
            var fire = MakeSprite("Campfire Flame", new Vector2(0f, 0.35f), new Vector3(0.45f, 0.8f, 1f), new Color(1f, 0.45f, 0.08f), 2, campRoot.transform);
            MakeSprite("Campfire Core", new Vector2(0f, 0.27f), new Vector3(0.22f, 0.42f, 1f), new Color(1f, 0.84f, 0.15f), 3, campRoot.transform).gameObject.SetActive(camp.CampfireBuilt);
            fire.gameObject.SetActive(camp.CampfireBuilt);
            var campfire = campRoot.AddComponent<CampfireInteractable>();
            campfire.Initialize(fire.gameObject, inventory, camp);

            var shelterRoot = new GameObject("Shelter Site");
            shelterRoot.transform.SetParent(transform, false);
            shelterRoot.transform.position = new Vector2(1.5f, -2.2f);
            MakeSprite("Shelter Shadow", new Vector2(0f, -0.56f), new Vector3(1.75f, 0.2f, 1f), new Color(0.28f, 0.22f, 0.15f, 0.45f), 0, shelterRoot.transform);
            var shelter = MakeSprite("Shelter", new Vector2(0f, -0.1f), new Vector3(1.4f, 0.9f, 1f), new Color(0.42f, 0.27f, 0.12f), 1, shelterRoot.transform);
            MakeSprite("Shelter Roof", new Vector2(0f, 0.48f), new Vector3(1.75f, 0.28f, 1f), new Color(0.29f, 0.16f, 0.08f), 2, shelterRoot.transform).transform.localRotation = Quaternion.Euler(0f, 0f, -4f);
            MakeSprite("Shelter Entrance", new Vector2(0f, -0.2f), new Vector3(0.38f, 0.52f, 1f), new Color(0.12f, 0.10f, 0.08f), 2, shelterRoot.transform);
            shelter.gameObject.SetActive(camp.ShelterBuilt);
            var shelterMarker = MakeSprite("Shelter Marker", new Vector2(0f, 1.1f), new Vector3(0.34f, 0.34f, 1f), new Color(1f, 0.82f, 0.12f), 3, shelterRoot.transform);
            shelterMarker.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
            shelterMarker.gameObject.SetActive(false);
            shelterSite = shelterRoot.AddComponent<ShelterInteractable>();
            shelterSite.Initialize(shelter.gameObject, shelterMarker.gameObject, inventory, camp);

            var jungleRoot = new GameObject("Jungle Edge");
            jungleRoot.transform.SetParent(transform, false);
            jungleRoot.transform.position = new Vector2(4.2f, 1.25f);
            MakeSprite("Jungle Canopy", new Vector2(0.3f, 0.1f), new Vector3(1.95f, 3.8f, 1f), new Color(0.05f, 0.24f, 0.11f), -2, jungleRoot.transform);
            MakeSprite("Jungle Canopy Top", new Vector2(0.0f, 1.7f), new Vector3(1.4f, 0.85f, 1f), new Color(0.08f, 0.34f, 0.14f), -1, jungleRoot.transform);
            MakeSprite("Jungle Canopy Mid", new Vector2(0.72f, 0.8f), new Vector3(0.95f, 1.25f, 1f), new Color(0.07f, 0.30f, 0.12f), -1, jungleRoot.transform);
            MakeSprite("Jungle Border", new Vector2(-0.68f, 0f), new Vector3(0.18f, 3.2f, 1f), new Color(0.12f, 0.42f, 0.16f), 1, jungleRoot.transform);
            MakeSprite("Jungle Path", new Vector2(-0.25f, -1.3f), new Vector3(0.95f, 1.3f, 1f), new Color(0.64f, 0.49f, 0.22f), 1, jungleRoot.transform);
            MakeSprite("Jungle Clearing", new Vector2(0.25f, -0.15f), new Vector3(0.75f, 1.1f, 1f), new Color(0.55f, 0.42f, 0.18f), 1, jungleRoot.transform);
            var jungleEdge = jungleRoot.AddComponent<JungleEdgeInteractable>();
            jungleEdge.Initialize(jungleRoot, camp);

            var survivorRoot = new GameObject("Mara");
            survivorRoot.transform.SetParent(transform, false);
            MakeSprite("Mara Shadow", new Vector2(0f, -0.42f), new Vector3(0.68f, 0.16f, 1f), new Color(0.20f, 0.15f, 0.10f, 0.45f), 1, survivorRoot.transform);
            MakeSprite("Mara Body", new Vector2(0f, -0.05f), new Vector3(0.42f, 0.58f, 1f), new Color(0.82f, 0.20f, 0.52f), 2, survivorRoot.transform);
            MakeSprite("Mara Head", new Vector2(0f, 0.38f), new Vector3(0.34f, 0.34f, 1f), new Color(0.98f, 0.68f, 0.42f), 3, survivorRoot.transform);
            MakeSprite("Mara Pack", new Vector2(0.25f, -0.04f), new Vector3(0.16f, 0.35f, 1f), new Color(0.30f, 0.18f, 0.10f), 2, survivorRoot.transform);
            survivor = survivorRoot.AddComponent<SurvivorInteractable>();
            survivor.Initialize(survivorRoot, inventory);

            Targets = new IActivityTarget[] { tree, bush, rock, campfire, shelterSite, jungleEdge, survivor };
            interactionMarker = MakeSprite("Interaction Marker", Vector2.zero, new Vector3(0.34f, 0.34f, 1f), new Color(1f, 0.82f, 0.12f, 0.9f), 4, transform);
            interactionMarker.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
            interactionMarker.gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            foreach (var sprite in generatedSprites)
            {
                if (sprite == null)
                    continue;

                if (Application.isPlaying)
                    Destroy(sprite);
                else
                    DestroyImmediate(sprite);
            }
        }

        public void SetTimeOfDay(TimeOfDay timeOfDay)
        {
            if (worldCamera == null)
                return;

            worldCamera.backgroundColor = timeOfDay switch
            {
                TimeOfDay.Afternoon => new Color(0.91f, 0.52f, 0.28f),
                TimeOfDay.Night => new Color(0.05f, 0.08f, 0.20f),
                _ => new Color(0.20f, 0.63f, 0.77f)
            };
            survivor?.SetTimeOfDay(timeOfDay);
        }

        public void ResetTargetsForNewDay()
        {
            foreach (var target in Targets)
                target.ResetForNewDay();
        }

        public void SetShelterMarkerVisible(bool visible)
        {
            if (shelterSite != null)
                shelterSite.SetMarkerVisible(visible);
        }

        public void SetInteractionTarget(IActivityTarget target)
        {
            if (interactionMarker == null)
                return;

            if (target == null)
            {
                interactionMarker.gameObject.SetActive(false);
                return;
            }

            interactionMarker.transform.position = (Vector3)target.Position + Vector3.up * 0.82f;
            interactionMarker.transform.localScale = target is JungleEdgeInteractable
                ? new Vector3(0.52f, 0.52f, 1f)
                : new Vector3(0.34f, 0.34f, 1f);
            interactionMarker.gameObject.SetActive(true);
        }

        SpriteRenderer MakeSprite(string name, Vector2 position, Vector3 scale, Color color, int sortingOrder, Transform parent = null)
        {
            var sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
            generatedSprites.Add(sprite);
            var item = new GameObject(name);
            item.transform.SetParent(parent, false);
            item.transform.localPosition = position;
            item.transform.localScale = scale;
            var renderer = item.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;
            return renderer;
        }
    }

}
