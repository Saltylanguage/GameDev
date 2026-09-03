#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SaltyGame.EditorTools
{
    /// <summary>Shows every named eight-neighbor blob terrain variant.</summary>
    public sealed class TerrainTilePreviewWindow : EditorWindow
    {
        const string TerrainFolder = "Assets/Art/Terrain/Blob/128";
        const float LabelHeight = 18f;
        const int PreviewColumns = 7;

        Texture2D[] terrainTiles;
        bool showDesert;
        bool loadedDesert;

        [MenuItem("Salty Game/Simulation/Preview Terrain Smart Tiles")]
        static void Open()
        {
            var window = GetWindow<TerrainTilePreviewWindow>();
            window.titleContent = new GUIContent("Terrain Smart Tiles");
            window.minSize = new Vector2(520f, 620f);
            window.Show();
        }

        void OnEnable()
        {
            LoadTiles();
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Terrain smart-tiling preview", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Each named sprite represents one normalized eight-neighbor blob mask around a visual tile. "
                + "Mask 0 is intentionally empty.",
                MessageType.Info);

            EditorGUILayout.BeginHorizontal();
            showDesert = EditorGUILayout.ToggleLeft("Show Desert_ family", showDesert);
            if (showDesert != loadedDesert)
            {
                LoadTiles();
            }

            if (GUILayout.Button("Reload tiles", GUILayout.Width(100f)))
            {
                LoadTiles();
            }

            EditorGUILayout.EndHorizontal();

            if (terrainTiles == null)
            {
                EditorGUILayout.HelpBox("Could not load the named terrain sprites.", MessageType.Warning);
                return;
            }

            var availableWidth = Mathf.Max(320f, position.width - 24f);
            var tileSize = Mathf.Min(availableWidth / PreviewColumns, 160f);
            var gridHeight = tileSize * PreviewColumns + LabelHeight * PreviewColumns;
            var gridRect = GUILayoutUtility.GetRect(availableWidth, gridHeight);

            for (var row = 0; row < PreviewColumns; row++)
            {
                for (var column = 0; column < PreviewColumns; column++)
                {
                    var index = row * PreviewColumns + column;
                    if (index >= TerrainTileResolver.AllValidMasks.Count)
                    {
                        continue;
                    }

                    var mask = TerrainTileResolver.AllValidMasks[index];
                    var tileRect = new Rect(
                        gridRect.x + column * tileSize,
                        gridRect.y + row * (tileSize + LabelHeight),
                        tileSize,
                        tileSize);

                    EditorGUI.DrawRect(tileRect, new Color(0.12f, 0.12f, 0.12f));
                    DrawTile(tileRect, mask);
                    var labelRect = new Rect(tileRect.x, tileRect.yMax, tileRect.width, LabelHeight);
                    GUI.Label(labelRect, $"mask {mask}: {DescribeMask(mask)}", EditorStyles.centeredGreyMiniLabel);
                }
            }
        }

        void LoadTiles()
        {
            loadedDesert = showDesert;
            terrainTiles = new Texture2D[TerrainTileResolver.AllValidMasks.Count];
            for (var index = 0; index < terrainTiles.Length; index++)
            {
                var mask = TerrainTileResolver.AllValidMasks[index];
                var family = showDesert ? "Desert" : "Grass";
                terrainTiles[index] = AssetDatabase.LoadAssetAtPath<Texture2D>(
                    $"{TerrainFolder}/{family}/{family}_{mask:D3}.png");
            }
        }

        void DrawTile(Rect destination, int mask)
        {
            var variantIndex = FindMaskIndex(mask);
            if (variantIndex < 0 || terrainTiles[variantIndex] == null)
            {
                return;
            }

            GUI.DrawTexture(destination, terrainTiles[variantIndex], ScaleMode.StretchToFill, true);
        }

        static int FindMaskIndex(int mask)
        {
            for (var index = 0; index < TerrainTileResolver.AllValidMasks.Count; index++)
            {
                if (TerrainTileResolver.AllValidMasks[index] == mask)
                {
                    return index;
                }
            }

            return -1;
        }

        static string DescribeMask(int mask)
        {
            if (mask == 0)
            {
                return "empty";
            }

            var value = string.Empty;
            if ((mask & TerrainTileResolver.North) != 0) value += "N";
            if ((mask & TerrainTileResolver.NorthEast) != 0) value += "NE";
            if ((mask & TerrainTileResolver.East) != 0) value += "E";
            if ((mask & TerrainTileResolver.SouthEast) != 0) value += "SE";
            if ((mask & TerrainTileResolver.South) != 0) value += "S";
            if ((mask & TerrainTileResolver.SouthWest) != 0) value += "SW";
            if ((mask & TerrainTileResolver.West) != 0) value += "W";
            if ((mask & TerrainTileResolver.NorthWest) != 0) value += "NW";
            return value;
        }
    }
}
#endif
