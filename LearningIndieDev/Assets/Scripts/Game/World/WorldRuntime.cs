using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
    public sealed class WorldRuntime : MonoBehaviour
    {
        readonly List<Sprite> generatedSprites = new List<Sprite>();
        Camera worldCamera;
        ShelterInteractable shelterSite;
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
            PlayerTransform = MakeSprite("Player", new Vector2(-2.8f, -0.8f), new Vector3(0.65f, 0.85f, 1f), new Color(0.94f, 0.33f, 0.22f), 2, transform).transform;
            var rockRoot = new GameObject("Rock");
            rockRoot.transform.SetParent(transform, false);
            rockRoot.transform.position = new Vector2(-0.4f, 1.9f);
            MakeSprite("Stone", Vector2.zero, new Vector3(1.2f, 0.8f, 1f), new Color(0.45f, 0.46f, 0.42f), 1, rockRoot.transform);
            var rock = rockRoot.AddComponent<RockInteractable>();
            rock.Initialize(rockRoot);

            var treeRoot = new GameObject("Tree");
            treeRoot.transform.SetParent(transform, false);
            treeRoot.transform.position = new Vector2(2.2f, -0.2f);
            MakeSprite("Trunk", Vector2.zero, new Vector3(0.58f, 2.15f, 1f), new Color(0.39f, 0.20f, 0.08f), 1, treeRoot.transform);
            MakeSprite("Leaves", new Vector2(0f, 1.2f), new Vector3(2.05f, 1.75f, 1f), new Color(0.09f, 0.42f, 0.17f), 1, treeRoot.transform);
            var tree = treeRoot.AddComponent<TreeInteractable>();
            tree.Initialize(treeRoot);

            var bushRoot = new GameObject("Berry Bush");
            bushRoot.transform.SetParent(transform, false);
            bushRoot.transform.position = new Vector2(-1.1f, -0.7f);
            MakeSprite("Bush", Vector2.zero, new Vector3(1.25f, 0.9f, 1f), new Color(0.25f, 0.12f, 0.48f), 1, bushRoot.transform);
            var bush = bushRoot.AddComponent<BerryBushInteractable>();
            bush.Initialize(bushRoot);

            var campRoot = new GameObject("Campfire Site");
            campRoot.transform.SetParent(transform, false);
            campRoot.transform.position = new Vector2(-1.2f, -2.2f);
            MakeSprite("Fire Ring", Vector2.zero, new Vector3(1.35f, 0.45f, 1f), new Color(0.24f, 0.22f, 0.20f), 1, campRoot.transform);
            var fire = MakeSprite("Campfire", new Vector2(0f, 0.35f), new Vector3(0.45f, 0.8f, 1f), new Color(1f, 0.45f, 0.08f), 2, campRoot.transform);
            fire.gameObject.SetActive(camp.CampfireBuilt);
            var campfire = campRoot.AddComponent<CampfireInteractable>();
            campfire.Initialize(fire.gameObject, inventory, camp);

            var shelterRoot = new GameObject("Shelter Site");
            shelterRoot.transform.SetParent(transform, false);
            shelterRoot.transform.position = new Vector2(1.5f, -2.2f);
            var shelter = MakeSprite("Shelter", Vector2.zero, new Vector3(1.4f, 1.1f, 1f), new Color(0.42f, 0.27f, 0.12f), 1, shelterRoot.transform);
            shelter.gameObject.SetActive(camp.ShelterBuilt);
            var shelterMarker = MakeSprite("Shelter Marker", new Vector2(0f, 1.1f), new Vector3(0.34f, 0.34f, 1f), new Color(1f, 0.82f, 0.12f), 3, shelterRoot.transform);
            shelterMarker.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
            shelterMarker.gameObject.SetActive(false);
            shelterSite = shelterRoot.AddComponent<ShelterInteractable>();
            shelterSite.Initialize(shelter.gameObject, shelterMarker.gameObject, inventory, camp);

            Targets = new IActivityTarget[] { tree, bush, rock, campfire, shelterSite };
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
