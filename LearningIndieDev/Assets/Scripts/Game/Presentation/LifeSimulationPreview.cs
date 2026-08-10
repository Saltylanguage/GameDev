using UnityEngine;

namespace SaltyGame
{
    public sealed class LifeSimulationPreview : MonoBehaviour
    {
        static readonly GridPattern Neighborhood = new GridPattern(new[]
        {
            new Vector2Int(-1, 1),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(-1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(1, -1),
        });

        [SerializeField, Min(1)] int width = 20;
        [SerializeField, Min(1)] int height = 20;
        [SerializeField] int seed = 12345;
        [SerializeField, Range(0f, 1f)] float aliveProbability = 0.45f;
        [SerializeField, Min(0.01f)] float stepInterval = 0.35f;
        [SerializeField, Min(2f)] float cellSize = 10f;
        [SerializeField] Color aliveColor = new Color(0.2f, 0.95f, 0.55f);
        [SerializeField] Color deadColor = new Color(0.06f, 0.08f, 0.12f, 0.9f);

        float timer;

        public Grid<LifeCell> Cells { get; private set; }
        public int Generation { get; private set; }

        void Awake()
        {
            Cells = LifeSimulation.CreateRandom(width, height, seed, aliveProbability);
        }

        void Update()
        {
            timer += Time.deltaTime;
            if (timer < stepInterval)
            {
                return;
            }

            timer = 0f;
            Cells = LifeSimulation.Step(Cells, Neighborhood);
            Generation++;
        }

        void OnGUI()
        {
            GUI.Label(new Rect(16f, 12f, 240f, 24f), $"LifeCell — generation {Generation}");

            var previousColor = GUI.color;
            for (var y = 0; y < Cells.Height; y++)
            {
                for (var x = 0; x < Cells.Width; x++)
                {
                    GUI.color = Cells.GetCell(x, y).IsAlive ? aliveColor : deadColor;
                    GUI.DrawTexture(
                        new Rect(16f + x * cellSize, 38f + y * cellSize, cellSize - 1f, cellSize - 1f),
                        Texture2D.whiteTexture);
                }
            }

            GUI.color = previousColor;
        }
    }
}
