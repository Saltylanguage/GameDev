using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
    public sealed class WorldRuntime : MonoBehaviour
    {
        public Transform PlayerTransform { get; private set; }
        public IReadOnlyList<IActivityTarget> Targets { get; private set; }

        public void Build()
        {
            var camera = Camera.main;
            if (camera == null)
            {
                camera = new GameObject("Main Camera").AddComponent<Camera>();
                camera.tag = "MainCamera";
                camera.transform.SetParent(transform, false);
            }

            camera.orthographic = true;
            camera.orthographicSize = 5.5f;
            camera.transform.position = new Vector3(0f, 0f, -10f);
            camera.backgroundColor = new Color(0.20f, 0.63f, 0.77f);

            MakeSprite("Sand", Vector2.zero, new Vector3(13f, 9f, 1f), new Color(0.94f, 0.78f, 0.43f), -10, transform);
            MakeSprite("WaterEdge", new Vector2(0f, -4.2f), new Vector3(13f, 0.5f, 1f), new Color(0.11f, 0.48f, 0.72f), -9, transform);
            PlayerTransform = MakeSprite("Player", new Vector2(-2.8f, -0.8f), new Vector3(0.65f, 0.85f, 1f), new Color(0.94f, 0.33f, 0.22f), 2, transform).transform;
            MakeSprite("Rock", new Vector2(-0.4f, 1.9f), new Vector3(1.2f, 0.8f, 1f), new Color(0.45f, 0.46f, 0.42f), 1, transform);

            var treeRoot = new GameObject("Tree");
            treeRoot.transform.SetParent(transform, false);
            treeRoot.transform.position = new Vector2(2.2f, -0.2f);
            MakeSprite("Trunk", Vector2.zero, new Vector3(0.58f, 2.15f, 1f), new Color(0.39f, 0.20f, 0.08f), 1, treeRoot.transform);
            MakeSprite("Leaves", new Vector2(0f, 1.2f), new Vector3(2.05f, 1.75f, 1f), new Color(0.09f, 0.42f, 0.17f), 1, treeRoot.transform);
            var tree = treeRoot.AddComponent<TreeInteractable>();
            tree.Initialize(treeRoot);
            Targets = new[] { (IActivityTarget)tree };
        }

        static SpriteRenderer MakeSprite(string name, Vector2 position, Vector3 scale, Color color, int sortingOrder, Transform parent = null)
        {
            var sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
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
