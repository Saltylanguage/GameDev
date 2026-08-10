using UnityEngine;

namespace SaltyGame
{
    public sealed class CavePreview : MonoBehaviour
    {
        [Header("Generation")]
        [SerializeField, Min(1)] int width = 64;
        [SerializeField, Min(1)] int height = 40;
        [SerializeField] int seed = 12345;
        [SerializeField, Range(0f, 1f)] float initialWallProbability = 0.45f;
        [SerializeField, Range(0, 8)] int wallNeighborThreshold = 5;
        [SerializeField, Min(0)] int simulationSteps = 5;

        [Header("Preview")]
        [SerializeField, Min(0.01f)] float stepInterval = 0.3f;
        [SerializeField, Min(1f)] float pixelsPerUnit = 6f;
        [SerializeField] Color wallColor = new Color(0.12f, 0.14f, 0.18f);
        [SerializeField] Color floorColor = new Color(0.72f, 0.63f, 0.43f);
        [SerializeField] bool animateOnStart = true;

        Texture2D previewTexture;
        Sprite previewSprite;
        SpriteRenderer previewRenderer;
        Grid<CaveCell> cave;
        float stepTimer;
        int completedSteps;
        float currentPixelsPerUnit;
        bool initialized;
        bool isAnimating;

        public Grid<CaveCell> Cave => cave;
        public int CompletedSteps => completedSteps;
        public bool IsAnimating => isAnimating;

        public void Initialize()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;
            var previewObject = new GameObject("Cave Preview Renderer");
            previewObject.transform.SetParent(transform, false);
            previewRenderer = previewObject.AddComponent<SpriteRenderer>();
            previewRenderer.sortingOrder = 5000;
            Regenerate();
        }

        void Update()
        {
            if (!isAnimating || cave == null)
            {
                return;
            }

            stepTimer += Time.deltaTime;
            if (stepTimer < stepInterval)
            {
                return;
            }

            stepTimer = 0f;
            AdvanceOneStep();
        }

        [ContextMenu("Regenerate Preview")]
        public void Regenerate()
        {
            if (!initialized)
            {
                Initialize();
                return;
            }

            RecreateTextureIfNeeded();

            var settings = new CaveGenerationSettings(
                width,
                height,
                initialWallProbability,
                simulationSteps: 0,
                wallNeighborThreshold);

            cave = CaveGenerator.Generate(settings, seed);
            completedSteps = 0;
            stepTimer = 0f;
            isAnimating = animateOnStart && simulationSteps > 0;
            RenderCave();
        }

        [ContextMenu("Advance One Step")]
        public void AdvanceOneStep()
        {
            if (cave == null)
            {
                Regenerate();
                return;
            }

            cave = CaveGenerator.SimulateStep(cave, CaveCell.Neighborhood, wallNeighborThreshold);
            completedSteps++;
            isAnimating = completedSteps < simulationSteps;
            RenderCave();
        }

        void RecreateTextureIfNeeded()
        {
            if (previewTexture != null
                && previewTexture.width == width
                && previewTexture.height == height
                && Mathf.Approximately(currentPixelsPerUnit, pixelsPerUnit))
            {
                return;
            }

            DestroyPreviewAssets();
            previewTexture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                name = "Generated Cave Preview",
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
            };
            previewSprite = Sprite.Create(
                previewTexture,
                new Rect(0f, 0f, width, height),
                new Vector2(0.5f, 0.5f),
                pixelsPerUnit);
            previewSprite.name = "Generated Cave Preview";
            previewRenderer.sprite = previewSprite;
            currentPixelsPerUnit = pixelsPerUnit;
        }

        void RenderCave()
        {
            for (var y = 0; y < cave.Height; y++)
            {
                for (var x = 0; x < cave.Width; x++)
                {
                    previewTexture.SetPixel(x, y, cave.GetCell(x, y).IsWall ? wallColor : floorColor);
                }
            }

            previewTexture.Apply(false);
        }

        void OnDestroy()
        {
            DestroyPreviewAssets();
        }

        void DestroyPreviewAssets()
        {
            if (previewSprite != null)
            {
                Destroy(previewSprite);
                previewSprite = null;
            }

            if (previewTexture != null)
            {
                Destroy(previewTexture);
                previewTexture = null;
            }
        }
    }
}
