using UnityEngine;
using UnityEngine.U2D;

namespace SaltyGame
{
    /// <summary>Small runtime surface for manually testing terrain smart tiles.</summary>
    public sealed class TerrainPaintPreview : MonoBehaviour
    {
        const int GridWidth = 20;
        const int GridHeight = 12;
        const float ToolbarHeight = 88f;
        const float Margin = 16f;

        static readonly TerrainDefinition Desert = new TerrainDefinition(
            TerrainIds.Desert,
            isPassable: true,
            movementCost: 1.5f,
            providesResource: false,
            presentationColor: new Color(0.8f, 0.63f, 0.3f));

        [SerializeField] SpriteAtlas terrainAtlas;

        readonly Sprite[] grassTiles = new Sprite[TerrainTileResolver.TerrainVariantCount];
        readonly Sprite[] desertTiles = new Sprite[TerrainTileResolver.TerrainVariantCount];
        Grid<SpeciesCell> cells;
        TerrainDefinition selectedTerrain = TerrainDefaults.Grass;

        void Awake()
        {
            cells = new Grid<SpeciesCell>(GridWidth, GridHeight, (_, _) => SpeciesCell.Empty);
            LoadTiles();
        }

        void OnGUI()
        {
            DrawToolbar();

            var boardRect = GetBoardRect();
            DrawBoard(boardRect);
            HandlePainting(boardRect, Event.current);
        }

        void DrawToolbar()
        {
            GUILayout.BeginArea(new Rect(Margin, 8f, Screen.width - Margin * 2f, ToolbarHeight));
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Tile picker:", GUILayout.Width(72f));
            DrawBrushButton("Bare", TerrainDefaults.Bare, desertTiles);
            DrawBrushButton("Grass", TerrainDefaults.Grass, grassTiles);
            DrawBrushButton("Desert", Desert, desertTiles);
            GUILayout.Space(12f);
            if (GUILayout.Button("Clear", GUILayout.Width(70f)))
            {
                cells = new Grid<SpeciesCell>(GridWidth, GridHeight, (_, _) => SpeciesCell.Empty);
            }

            GUILayout.FlexibleSpace();
            GUILayout.Label("Click and drag to paint");
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        void DrawBrushButton(string label, TerrainDefinition terrain, Sprite[] tiles)
        {
            var previousColor = GUI.backgroundColor;
            if (selectedTerrain.Id == terrain.Id)
            {
                GUI.backgroundColor = Color.cyan;
            }

            var rect = GUILayoutUtility.GetRect(72f, 64f, GUILayout.Width(72f), GUILayout.Height(64f));
            if (GUI.Button(rect, GUIContent.none))
            {
                selectedTerrain = terrain;
            }

            GUI.backgroundColor = previousColor;
            if (tiles[TerrainTileResolver.FullVariantIndex] != null)
            {
                DrawSprite(tiles[TerrainTileResolver.FullVariantIndex], new Rect(rect.x + 15f, rect.y + 4f, 42f, 42f));
            }

            GUI.Label(new Rect(rect.x, rect.yMax - 20f, rect.width, 18f), label, GUI.skin.label);
        }

        Rect GetBoardRect()
        {
            var availableWidth = Screen.width - Margin * 2f;
            var availableHeight = Screen.height - ToolbarHeight - Margin * 2f;
            var cellSize = Mathf.Floor(Mathf.Min(availableWidth / GridWidth, availableHeight / GridHeight));
            var width = cellSize * GridWidth;
            var height = cellSize * GridHeight;
            return new Rect((Screen.width - width) * 0.5f, ToolbarHeight + (availableHeight - height) * 0.5f, width, height);
        }

        void DrawBoard(Rect boardRect)
        {
            GUI.Box(boardRect, GUIContent.none);
            var cellWidth = boardRect.width / GridWidth;
            var cellHeight = boardRect.height / GridHeight;

            for (var y = 0; y < GridHeight; y++)
            {
                for (var x = 0; x < GridWidth; x++)
                {
                    var cell = cells.GetCell(x, y);
                    var rect = new Rect(
                        boardRect.x + x * cellWidth,
                        boardRect.y + (GridHeight - 1 - y) * cellHeight,
                        cellWidth,
                        cellHeight);
                    DrawCell(cell, rect, x, y);
                }
            }
        }

        void DrawCell(SpeciesCell cell, Rect rect, int x, int y)
        {
            var tiles = cell.TerrainId == TerrainIds.Grass ? grassTiles : desertTiles;
            var tileIndex = TerrainTileResolver.ResolveTerrainTileIndex(cells, x, y, cell.TerrainId);
            if (tileIndex >= 0 && tileIndex < tiles.Length && tiles[tileIndex] != null)
            {
                DrawSprite(tiles[tileIndex], rect);
            }
            else
            {
                GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.StretchToFill);
            }

            GUI.Box(rect, GUIContent.none);
        }

        void HandlePainting(Rect boardRect, Event currentEvent)
        {
            if ((currentEvent.type != EventType.MouseDown && currentEvent.type != EventType.MouseDrag)
                || currentEvent.button != 0
                || !TryGetCellAtPosition(boardRect, currentEvent.mousePosition, GridWidth, GridHeight, out var x, out var y))
            {
                return;
            }

            cells.SetCell(x, y, SpeciesCell.FromTerrain(selectedTerrain));
            currentEvent.Use();
        }

        void LoadTiles()
        {
            if (terrainAtlas == null)
            {
                Debug.LogWarning("Terrain Paint Preview needs a terrain sprite atlas.", this);
                return;
            }

            for (var index = 0; index < TerrainTileResolver.TerrainVariantCount; index++)
            {
                var variant = TerrainTileResolver.GetVariantName(index);
                grassTiles[index] = terrainAtlas.GetSprite($"Grass_{variant}");
                desertTiles[index] = terrainAtlas.GetSprite($"Desert_{variant}");
            }
        }

        static void DrawSprite(Sprite sprite, Rect destination)
        {
            var texture = sprite.texture;
            var source = sprite.textureRect;
            var uv = new Rect(
                source.x / texture.width,
                source.y / texture.height,
                source.width / texture.width,
                source.height / texture.height);
            GUI.DrawTextureWithTexCoords(destination, texture, uv, true);
        }

        public static bool TryGetCellAtPosition(
            Rect boardRect,
            Vector2 position,
            int width,
            int height,
            out int x,
            out int y)
        {
            if (!boardRect.Contains(position) || width <= 0 || height <= 0)
            {
                x = y = -1;
                return false;
            }

            x = Mathf.Min(width - 1, Mathf.FloorToInt((position.x - boardRect.x) / boardRect.width * width));
            var screenY = Mathf.Min(height - 1, Mathf.FloorToInt((position.y - boardRect.y) / boardRect.height * height));
            y = height - 1 - screenY;
            return true;
        }
    }
}
