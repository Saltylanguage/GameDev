#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SaltyGame.EditorTools
{
    /// <summary>Shows every named four-corner terrain variant.</summary>
    public sealed class TerrainTilePreviewWindow : EditorWindow
    {
        const string TerrainFolder = "Assets/Art/Terrain/Standardized/128/";
        const float LabelHeight = 18f;

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
                "Each named sprite represents one non-empty combination of the four simulation cells around a visual tile. "
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
            var tileSize = Mathf.Min(availableWidth / 4f, 160f);
            var gridHeight = tileSize * 4f + LabelHeight * 4f;
            var gridRect = GUILayoutUtility.GetRect(availableWidth, gridHeight);

            for (var row = 0; row < 4; row++)
            {
                for (var column = 0; column < 4; column++)
                {
                    var mask = row * 4 + column;
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
            terrainTiles = new Texture2D[TerrainTileResolver.TerrainVariantCount];
            for (var index = 0; index < terrainTiles.Length; index++)
            {
                var name = TerrainTileResolver.GetVariantName(index);
                var family = showDesert ? "Desert" : "Grass";
                terrainTiles[index] = AssetDatabase.LoadAssetAtPath<Texture2D>($"{TerrainFolder}{family}_{name}.png");
            }
        }

        void DrawTile(Rect destination, int mask)
        {
            var variantIndex = TerrainTileResolver.ResolveTerrainAtlasIndex(mask);
            if (variantIndex < 0 || terrainTiles[variantIndex] == null)
            {
                return;
            }

            GUI.DrawTexture(destination, terrainTiles[variantIndex], ScaleMode.StretchToFill, true);
        }

        static string DescribeMask(int mask)
        {
            if (mask == 0)
            {
                return "empty";
            }

            var value = string.Empty;
            if ((mask & TerrainTileResolver.NorthWest) != 0) value += "NW";
            if ((mask & TerrainTileResolver.NorthEast) != 0) value += "NE";
            if ((mask & TerrainTileResolver.SouthWest) != 0) value += "SW";
            if ((mask & TerrainTileResolver.SouthEast) != 0) value += "SE";
            return value;
        }
    }
}
#endif
