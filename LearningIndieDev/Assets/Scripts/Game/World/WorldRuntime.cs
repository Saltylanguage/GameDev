using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
    public sealed class WorldRuntime : MonoBehaviour
    {
        readonly List<Sprite> generatedSprites = new List<Sprite>();
        readonly List<Transform> depthSortedRoots = new List<Transform>();
        Camera worldCamera;
        Texture2D artAtlas;
        Texture2D tileAtlas;
        Texture2D jungleEntranceClosedTiles;
        Texture2D jungleEntranceOpenTiles;
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
            artAtlas = Resources.Load<Texture2D>("Art/IslandChores_ArtAtlas128");
            if (artAtlas != null)
            {
                artAtlas.filterMode = FilterMode.Point;
                artAtlas.wrapMode = TextureWrapMode.Clamp;
            }
            tileAtlas = Resources.Load<Texture2D>("Art/IslandChores_TileAtlas128_SeamSafe");
            if (tileAtlas != null)
            {
                tileAtlas.filterMode = FilterMode.Point;
                tileAtlas.wrapMode = TextureWrapMode.Clamp;
            }
            jungleEntranceClosedTiles = Resources.Load<Texture2D>("Art/IslandChores_JungleEntranceClosedTiles128");
            if (jungleEntranceClosedTiles != null)
            {
                jungleEntranceClosedTiles.filterMode = FilterMode.Point;
                jungleEntranceClosedTiles.wrapMode = TextureWrapMode.Clamp;
            }
            jungleEntranceOpenTiles = Resources.Load<Texture2D>("Art/IslandChores_JungleEntranceOpenTiles128");
            if (jungleEntranceOpenTiles != null)
            {
                jungleEntranceOpenTiles.filterMode = FilterMode.Point;
                jungleEntranceOpenTiles.wrapMode = TextureWrapMode.Clamp;
            }

            var environmentRoot = new GameObject("Environment");
            environmentRoot.transform.SetParent(transform, false);
            var backgroundRoot = new GameObject("Background Layer");
            backgroundRoot.transform.SetParent(environmentRoot.transform, false);
            var groundRoot = new GameObject("Playable Beach Layer");
            groundRoot.transform.SetParent(environmentRoot.transform, false);
            var foregroundRoot = new GameObject("Foreground Foliage Layer");
            foregroundRoot.transform.SetParent(environmentRoot.transform, false);

            MakeTileField("Beach Tiles", 0, 3, 13, 7, new Vector2(-6.5f, -3.3f), -20, groundRoot.transform);
            MakeTileField("Ocean Tiles", 1, 2, 13, 2, new Vector2(-6.5f, -5.3f), -30, backgroundRoot.transform);
            MakeSprite("Shoreline Foam", new Vector2(0f, -3.42f), new Vector3(13f, 0.12f, 1f), new Color(0.78f, 0.91f, 0.83f, 0.75f), -19, foregroundRoot.transform);
            MakeWave("Wave A", new Vector2(-4.9f, -4.05f), 0.18f, backgroundRoot.transform);
            MakeWave("Wave B", new Vector2(-2.4f, -4.55f), -0.12f, backgroundRoot.transform);
            MakeWave("Wave C", new Vector2(0.4f, -3.98f), 0.14f, backgroundRoot.transform);
            MakeWave("Wave D", new Vector2(3.2f, -4.48f), -0.16f, backgroundRoot.transform);
            MakeWave("Wave E", new Vector2(5.0f, -4.08f), 0.1f, backgroundRoot.transform);

            MakeTileField("Jungle Canopy Tiles", 3, 1, 13, 2, new Vector2(-6.5f, 3.2f), -18, backgroundRoot.transform);

            MakeDriftwood("Driftwood Left", new Vector2(-0.2f, -2.95f), 12f, groundRoot.transform);
            MakeDriftwood("Driftwood Right", new Vector2(3.0f, -2.65f), -18f, groundRoot.transform);
            MakeBeachRock("Beach Rock", new Vector2(4.2f, -1.55f), 0.7f, groundRoot.transform);
            var playerRoot = new GameObject("Player");
            playerRoot.transform.SetParent(transform, false);
            playerRoot.transform.position = new Vector2(-2.8f, -0.8f);
            if (artAtlas != null)
                MakeAtlasSprite("Player", 0, 3, Vector2.zero, Vector3.one, 2, playerRoot.transform);
            else
            {
                MakeSprite("Player Shadow", new Vector2(0f, -0.42f), new Vector3(0.72f, 0.18f, 1f), new Color(0.20f, 0.15f, 0.10f, 0.45f), 1, playerRoot.transform);
                MakeSprite("Player Body", new Vector2(0f, -0.05f), new Vector3(0.42f, 0.58f, 1f), new Color(0.94f, 0.33f, 0.22f), 2, playerRoot.transform);
                MakeSprite("Player Head", new Vector2(0f, 0.38f), new Vector3(0.34f, 0.34f, 1f), new Color(0.98f, 0.68f, 0.42f), 3, playerRoot.transform);
                MakeSprite("Player Pack", new Vector2(-0.25f, -0.04f), new Vector3(0.16f, 0.35f, 1f), new Color(0.30f, 0.18f, 0.10f), 2, playerRoot.transform);
            }
            PlayerTransform = playerRoot.transform;
            var rockRoot = new GameObject("Rock");
            rockRoot.transform.SetParent(transform, false);
            rockRoot.transform.position = new Vector2(-0.4f, 1.9f);
            if (artAtlas != null)
                MakeAtlasSprite("Rock", 2, 3, Vector2.zero, new Vector3(1.7f, 1.7f, 1f), 1, rockRoot.transform);
            else
            {
                MakeSprite("Rock Shadow", new Vector2(0f, -0.32f), new Vector3(1.25f, 0.22f, 1f), new Color(0.28f, 0.22f, 0.15f, 0.45f), 0, rockRoot.transform);
                var rockFace = MakeSprite("Stone", new Vector2(0f, 0.05f), new Vector3(1.15f, 0.72f, 1f), new Color(0.45f, 0.46f, 0.42f), 1, rockRoot.transform);
                rockFace.transform.localRotation = Quaternion.Euler(0f, 0f, -8f);
                var rockHighlight = MakeSprite("Stone Highlight", new Vector2(-0.2f, 0.18f), new Vector3(0.42f, 0.16f, 1f), new Color(0.68f, 0.69f, 0.62f), 2, rockRoot.transform);
                rockHighlight.transform.localRotation = Quaternion.Euler(0f, 0f, -8f);
            }
            var rock = rockRoot.AddComponent<RockInteractable>();
            rock.Initialize(rockRoot);

            var treeLeft = MakeTree("Palm Tree Left", new Vector2(0.2f, 0.8f), groundRoot.transform, camp);
            var treeRight = MakeTree("Palm Tree Right", new Vector2(3.7f, 0.9f), groundRoot.transform, camp);

            var bushRoot = new GameObject("Berry Bush");
            bushRoot.transform.SetParent(transform, false);
            bushRoot.transform.position = new Vector2(-1.1f, -0.7f);
            if (artAtlas != null)
                MakeAtlasSprite("Berry Bush", 3, 3, Vector2.zero, new Vector3(1.2f, 1.2f, 1f), 1, bushRoot.transform);
            else
            {
                MakeSprite("Bush Shadow", new Vector2(0f, -0.36f), new Vector3(1.35f, 0.18f, 1f), new Color(0.28f, 0.22f, 0.15f, 0.45f), 0, bushRoot.transform);
                MakeSprite("Bush Foliage", Vector2.zero, new Vector3(1.25f, 0.9f, 1f), new Color(0.10f, 0.30f, 0.16f), 1, bushRoot.transform);
                MakeSprite("Berry Left", new Vector2(-0.32f, 0.13f), new Vector3(0.18f, 0.18f, 1f), new Color(0.72f, 0.12f, 0.36f), 2, bushRoot.transform);
                MakeSprite("Berry Center", new Vector2(0.05f, 0.28f), new Vector3(0.2f, 0.2f, 1f), new Color(0.86f, 0.16f, 0.42f), 2, bushRoot.transform);
                MakeSprite("Berry Right", new Vector2(0.34f, -0.02f), new Vector3(0.18f, 0.18f, 1f), new Color(0.65f, 0.08f, 0.32f), 2, bushRoot.transform);
            }
            var bush = bushRoot.AddComponent<BerryBushInteractable>();
            bush.Initialize(bushRoot);

            var campRoot = new GameObject("Campfire Site");
            campRoot.transform.SetParent(transform, false);
            campRoot.transform.position = new Vector2(-1.2f, -2.2f);
            SpriteRenderer fire;
            if (artAtlas != null)
                fire = MakeAtlasSprite("Campfire", 0, 2, Vector2.zero, new Vector3(1.3f, 1.3f, 1f), 2, campRoot.transform);
            else
            {
                MakeSprite("Fire Ring", Vector2.zero, new Vector3(1.35f, 0.38f, 1f), new Color(0.24f, 0.22f, 0.20f), 1, campRoot.transform);
                MakeSprite("Fire Stone Left", new Vector2(-0.48f, 0.05f), new Vector3(0.28f, 0.28f, 1f), new Color(0.38f, 0.36f, 0.31f), 2, campRoot.transform);
                MakeSprite("Fire Stone Right", new Vector2(0.48f, 0.05f), new Vector3(0.28f, 0.28f, 1f), new Color(0.38f, 0.36f, 0.31f), 2, campRoot.transform);
                fire = MakeSprite("Campfire Flame", new Vector2(0f, 0.35f), new Vector3(0.45f, 0.8f, 1f), new Color(1f, 0.45f, 0.08f), 2, campRoot.transform);
                MakeSprite("Campfire Core", new Vector2(0f, 0.27f), new Vector3(0.22f, 0.42f, 1f), new Color(1f, 0.84f, 0.15f), 3, campRoot.transform).gameObject.SetActive(camp.CampfireBuilt);
            }
            fire.gameObject.SetActive(camp.CampfireBuilt);
            var campfire = campRoot.AddComponent<CampfireInteractable>();
            campfire.Initialize(fire.gameObject, inventory, camp);

            var shelterRoot = new GameObject("Shelter Site");
            shelterRoot.transform.SetParent(transform, false);
            shelterRoot.transform.position = new Vector2(1.5f, -2.2f);
            SpriteRenderer shelter;
            if (artAtlas != null)
                shelter = MakeAtlasSprite("Shelter", 1, 2, Vector2.zero, new Vector3(1.25f, 1.25f, 1f), 1, shelterRoot.transform);
            else
            {
                MakeSprite("Shelter Shadow", new Vector2(0f, -0.56f), new Vector3(1.75f, 0.2f, 1f), new Color(0.28f, 0.22f, 0.15f, 0.45f), 0, shelterRoot.transform);
                shelter = MakeSprite("Shelter", new Vector2(0f, -0.1f), new Vector3(1.4f, 0.9f, 1f), new Color(0.42f, 0.27f, 0.12f), 1, shelterRoot.transform);
                MakeSprite("Shelter Roof", new Vector2(0f, 0.48f), new Vector3(1.75f, 0.28f, 1f), new Color(0.29f, 0.16f, 0.08f), 2, shelterRoot.transform).transform.localRotation = Quaternion.Euler(0f, 0f, -4f);
                MakeSprite("Shelter Entrance", new Vector2(0f, -0.2f), new Vector3(0.38f, 0.52f, 1f), new Color(0.12f, 0.10f, 0.08f), 2, shelterRoot.transform);
            }
            shelter.gameObject.SetActive(camp.ShelterBuilt);
            var shelterMarker = MakeSprite("Shelter Marker", new Vector2(0f, 1.1f), new Vector3(0.34f, 0.34f, 1f), new Color(1f, 0.82f, 0.12f), 3, shelterRoot.transform);
            shelterMarker.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
            shelterMarker.gameObject.SetActive(false);
            shelterSite = shelterRoot.AddComponent<ShelterInteractable>();
            shelterSite.Initialize(shelter.gameObject, shelterMarker.gameObject, inventory, camp);

            var routeRoot = new GameObject("Jungle Exit Route");
            routeRoot.transform.SetParent(backgroundRoot.transform, false);
            if (jungleEntranceOpenTiles != null)
                MakeTextureTileField("Jungle Entrance Open", jungleEntranceOpenTiles, 3, 2, new Vector2(1.95f, 2.2f), -17, routeRoot.transform);
            routeRoot.SetActive(false);

            var jungleRoot = new GameObject("Jungle Edge");
            jungleRoot.transform.SetParent(transform, false);
            jungleRoot.transform.position = new Vector2(3.45f, 3.7f);
            if (jungleEntranceClosedTiles != null)
                MakeTextureTileField("Jungle Entrance Closed", jungleEntranceClosedTiles, 3, 2, new Vector2(-1.5f, -1.5f), 1, jungleRoot.transform);
            var jungleEdge = jungleRoot.AddComponent<JungleEdgeInteractable>();
            jungleEdge.Initialize(jungleRoot, routeRoot, camp);

            var survivorRoot = new GameObject("Mara");
            survivorRoot.transform.SetParent(transform, false);
            if (artAtlas != null)
                MakeAtlasSprite("Mara", 1, 3, Vector2.zero, Vector3.one, 2, survivorRoot.transform);
            else
            {
                MakeSprite("Mara Shadow", new Vector2(0f, -0.42f), new Vector3(0.68f, 0.16f, 1f), new Color(0.20f, 0.15f, 0.10f, 0.45f), 1, survivorRoot.transform);
                MakeSprite("Mara Body", new Vector2(0f, -0.05f), new Vector3(0.42f, 0.58f, 1f), new Color(0.82f, 0.20f, 0.52f), 2, survivorRoot.transform);
                MakeSprite("Mara Head", new Vector2(0f, 0.38f), new Vector3(0.34f, 0.34f, 1f), new Color(0.98f, 0.68f, 0.42f), 3, survivorRoot.transform);
                MakeSprite("Mara Pack", new Vector2(0.25f, -0.04f), new Vector3(0.16f, 0.35f, 1f), new Color(0.30f, 0.18f, 0.10f), 2, survivorRoot.transform);
            }
            survivor = survivorRoot.AddComponent<SurvivorInteractable>();
            survivor.Initialize(survivorRoot, inventory);

            depthSortedRoots.Add(playerRoot.transform);
            depthSortedRoots.Add(rockRoot.transform);
            depthSortedRoots.Add(bushRoot.transform);
            depthSortedRoots.Add(campRoot.transform);
            depthSortedRoots.Add(shelterRoot.transform);
            depthSortedRoots.Add(jungleRoot.transform);
            depthSortedRoots.Add(survivorRoot.transform);
            Targets = new IActivityTarget[] { treeLeft, bush, rock, campfire, shelterSite, jungleEdge, survivor, treeRight };
            interactionMarker = MakeSprite("Interaction Marker", Vector2.zero, new Vector3(0.34f, 0.34f, 1f), new Color(1f, 0.82f, 0.12f, 0.9f), 4, transform);
            interactionMarker.sortingOrder = 3000;
            interactionMarker.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
            interactionMarker.gameObject.SetActive(false);
        }

        void LateUpdate()
        {
            foreach (var root in depthSortedRoots)
                ApplyDepthSorting(root);
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

        void ApplyDepthSorting(Transform actor)
        {
            if (actor == null)
                return;

            var sortingOrder = 1000 - Mathf.RoundToInt(actor.position.y * 100f);
            var renderers = actor.GetComponentsInChildren<SpriteRenderer>();
            for (var index = 0; index < renderers.Length; index++)
                renderers[index].sortingOrder = sortingOrder + index;
        }

        void MakeWave(string name, Vector2 position, float tilt, Transform parent)
        {
            var wave = MakeSprite(name, position, new Vector3(0.9f, 0.06f, 1f), new Color(0.55f, 0.85f, 0.91f, 0.75f), -29, parent);
            wave.transform.localRotation = Quaternion.Euler(0f, 0f, tilt * 100f);
        }

        TreeInteractable MakeTree(string name, Vector2 position, Transform parent, CampState camp)
        {
            var root = new GameObject(name);
            root.transform.SetParent(parent, false);
            root.transform.localPosition = position;
            if (artAtlas != null)
                MakeAtlasSprite("Palm Tree", 1, 1, Vector2.zero, new Vector3(1.45f, 1.45f, 1f), 1, root.transform);
            else
            {
                MakeSprite("Tree Shadow", new Vector2(0f, -0.75f), new Vector3(1.25f, 0.18f, 1f), new Color(0.28f, 0.22f, 0.15f, 0.45f), 0, root.transform);
                MakeSprite("Palm Trunk", new Vector2(0f, -0.05f), new Vector3(0.32f, 1.45f, 1f), new Color(0.46f, 0.25f, 0.10f), 1, root.transform);
                MakeSprite("Palm Leaves", new Vector2(0f, 0.78f), new Vector3(1.45f, 0.9f, 1f), new Color(0.08f, 0.38f, 0.16f), 1, root.transform);
            }

            var tree = root.AddComponent<TreeInteractable>();
            tree.Initialize(root, camp);
            depthSortedRoots.Add(root.transform);
            return tree;
        }

        void MakeDriftwood(string name, Vector2 position, float tilt, Transform parent)
        {
            var root = new GameObject(name);
            root.transform.SetParent(parent, false);
            root.transform.position = position;
            var plank = MakeSprite("Washed Plank", Vector2.zero, new Vector3(1.15f, 0.16f, 1f), new Color(0.39f, 0.23f, 0.11f), 0, root.transform);
            plank.transform.localRotation = Quaternion.Euler(0f, 0f, tilt);
            var shortPlank = MakeSprite("Broken Plank", new Vector2(0.34f, 0.2f), new Vector3(0.42f, 0.13f, 1f), new Color(0.52f, 0.31f, 0.14f), 1, root.transform);
            shortPlank.transform.localRotation = Quaternion.Euler(0f, 0f, tilt - 28f);
            MakeSprite("Wood Shadow", new Vector2(0f, -0.14f), new Vector3(1.4f, 0.12f, 1f), new Color(0.28f, 0.20f, 0.12f, 0.35f), -1, root.transform);
        }

        void MakeBeachRock(string name, Vector2 position, float scale, Transform parent)
        {
            var root = new GameObject(name);
            root.transform.SetParent(parent, false);
            root.transform.position = position;
            if (artAtlas != null)
                MakeAtlasSprite("Beach Stone", 2, 1, Vector2.zero, new Vector3(0.8f, 0.8f, 1f) * scale, 0, root.transform);
            else
                MakeSprite("Beach Stone", Vector2.zero, new Vector3(0.55f, 0.35f, 1f) * scale, new Color(0.42f, 0.43f, 0.40f), 0, root.transform);
            MakeSprite("Beach Stone Shadow", new Vector2(0f, -0.22f), new Vector3(0.8f, 0.12f, 1f) * scale, new Color(0.28f, 0.22f, 0.15f, 0.35f), -1, root.transform);
            depthSortedRoots.Add(root.transform);
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

        void MakeTileField(string name, int column, int row, int width, int height, Vector2 origin, int sortingOrder, Transform parent)
        {
            if (tileAtlas == null)
            {
                var fallbackColor = row == 2
                    ? new Color(0.08f, 0.44f, 0.68f)
                    : row == 1
                        ? new Color(0.04f, 0.22f, 0.12f)
                        : new Color(0.94f, 0.78f, 0.43f);
                MakeSprite(name, origin + new Vector2(width, height) * 0.5f, new Vector3(width, height, 1f), fallbackColor, sortingOrder, parent);
                return;
            }

            var cellWidth = tileAtlas.width / 4;
            var cellHeight = tileAtlas.height / 4;
            var rect = new Rect(column * cellWidth, row * cellHeight, cellWidth, cellHeight);
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var sprite = Sprite.Create(tileAtlas, rect, new Vector2(0.5f, 0.5f), 128f, 1u);
                    generatedSprites.Add(sprite);
                    var item = new GameObject($"{name} {x},{y}");
                    item.transform.SetParent(parent, false);
                    item.transform.localPosition = origin + new Vector2(x + 0.5f, y + 0.5f);
                    var renderer = item.AddComponent<SpriteRenderer>();
                    renderer.sprite = sprite;
                    renderer.sortingOrder = sortingOrder;
                }
            }
        }

        void MakeTextureTileField(string name, Texture2D texture, int width, int height, Vector2 origin, int sortingOrder, Transform parent)
        {
            var cellWidth = texture.width / width;
            var cellHeight = texture.height / height;
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var rect = new Rect(x * cellWidth, y * cellHeight, cellWidth, cellHeight);
                    var sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 128f, 1u);
                    generatedSprites.Add(sprite);
                    var item = new GameObject($"{name} {x},{y}");
                    item.transform.SetParent(parent, false);
                    item.transform.localPosition = origin + new Vector2(x + 0.5f, y + 0.5f);
                    var renderer = item.AddComponent<SpriteRenderer>();
                    renderer.sprite = sprite;
                    renderer.sortingOrder = sortingOrder;
                }
            }
        }

        SpriteRenderer MakeTextureSprite(string name, Texture2D texture, Vector2 position, Vector3 scale, int sortingOrder, Transform parent)
        {
            var sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 128f);
            generatedSprites.Add(sprite);
            var item = new GameObject(name);
            item.transform.SetParent(parent, false);
            item.transform.localPosition = position;
            item.transform.localScale = scale;
            var renderer = item.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = sortingOrder;
            return renderer;
        }

        SpriteRenderer MakeAtlasSprite(string name, int column, int row, Vector2 position, Vector3 scale, int sortingOrder, Transform parent)
        {
            var cellWidth = artAtlas.width / 4;
            var cellHeight = artAtlas.height / 4;
            var rect = new Rect(column * cellWidth, row * cellHeight, cellWidth, cellHeight);
            var sprite = Sprite.Create(artAtlas, rect, new Vector2(0.5f, 0.5f), 128f);
            generatedSprites.Add(sprite);
            var item = new GameObject(name);
            item.transform.SetParent(parent, false);
            item.transform.localPosition = position;
            item.transform.localScale = scale;
            var renderer = item.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = sortingOrder;
            return renderer;
        }
    }

}
