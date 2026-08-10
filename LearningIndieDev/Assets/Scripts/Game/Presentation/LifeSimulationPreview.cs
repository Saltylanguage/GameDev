using UnityEngine;

namespace SaltyGame
{
    public sealed class LifeSimulationPreview : MonoBehaviour
    {
        static readonly GridPattern Neighborhood = new GridPattern(new[]
        {
            new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1),
            new Vector2Int(-1, 0), new Vector2Int(1, 0),
            new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1),
        });

        [SerializeField, Min(1)] int width = 20;
        [SerializeField, Min(1)] int height = 20;
        [SerializeField] int seed = 12345;
        [SerializeField, Range(0f, 0.8f)] float lifeProbability = 0.45f;
        [SerializeField, Min(0.01f)] float stepInterval = 0.35f;
        [SerializeField] Color lifeColor = new Color(0.2f, 0.95f, 0.55f);
        [SerializeField] Color emptyColor = new Color(0.06f, 0.08f, 0.12f, 0.9f);
        [SerializeField] Color hotColor = new Color(1f, 0.25f, 0.05f);
        [SerializeField] Color plantColor = new Color(0.25f, 0.75f, 0.2f);
        [SerializeField] Color fireColor = new Color(1f, 0.55f, 0.05f);

        float timer;

        public Grid<LifeCell> Cells { get; private set; }
        public int Generation { get; private set; }

        void Awake()
        {
            Cells = LifeSimulation.CreateRandom(width, height, seed, lifeProbability);
        }

        void Update()
        {
            timer += Time.deltaTime;
            if (timer < stepInterval)
            {
                return;
            }

            timer = 0f;
            Cells = LifeSimulation.Step(Cells, Neighborhood, seed + Generation);
            Generation++;
        }

        void OnGUI()
        {
            const float padding = 16f;
            var cellSize = Mathf.Min(
                (Screen.width - padding * 2f) / Cells.Width,
                (Screen.height - padding * 2f) / Cells.Height);
            var gridWidth = Cells.Width * cellSize;
            var gridLeft = (Screen.width - gridWidth) * 0.5f;
            var gridTop = (Screen.height - Cells.Height * cellSize) * 0.5f;
            var previousColor = GUI.color;

            GUI.Label(
                new Rect(gridLeft, Mathf.Max(0f, gridTop - 24f), gridWidth, 24f),
                $"Mixed Life — generation {Generation}");
            for (var y = 0; y < Cells.Height; y++)
            {
                for (var x = 0; x < Cells.Width; x++)
                {
                    GUI.color = CellColor(Cells.GetCell(x, y));
                    GUI.DrawTexture(
                        new Rect(gridLeft + x * cellSize, gridTop + y * cellSize, cellSize - 1f, cellSize - 1f),
                        Texture2D.whiteTexture);
                }
            }

            GUI.color = previousColor;
        }

        Color CellColor(LifeCell cell)
        {
            switch (cell.CurrentState)
            {
                case LifeCell.State.Life:
                    return lifeColor;
                case LifeCell.State.Plant:
                    return plantColor;
                case LifeCell.State.Fire:
                    return fireColor;
                default:
                    return Color.Lerp(emptyColor, hotColor, cell.Temperature / 100f);
            }
        }
    }
}
