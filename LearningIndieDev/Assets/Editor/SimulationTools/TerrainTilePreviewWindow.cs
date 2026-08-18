#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SaltyGame.EditorTools
{
    /// <summary>Shows every smart-tiling mask against the authored terrain atlas.</summary>
    public sealed class TerrainTilePreviewWindow : EditorWindow
    {
        const string TerrainAtlasPath = "Assets/Art/Terrain/Terrain_01_SpriteSheet.png";
        const int AtlasColumns = 4;
        const int AtlasRows = 8;
        const int DesertAtlasOffset = 16;
        const float LabelHeight = 18f;

        Texture2D terrainAtlas;
        bool showDesert;

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
            LoadAtlas();
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Terrain smart-tiling preview", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Each tile is selected from the four cardinal neighbors: N=1, E=2, S=4, W=8. "
                + "The preview uses the same lookup table as the runtime board.",
                MessageType.Info);

            EditorGUILayout.BeginHorizontal();
            showDesert = EditorGUILayout.ToggleLeft("Show desert family (atlas rows 4-7)", showDesert);
            if (GUILayout.Button("Reload atlas", GUILayout.Width(100f)))
            {
                LoadAtlas();
            }

            EditorGUILayout.EndHorizontal();

            if (terrainAtlas == null)
            {
                EditorGUILayout.HelpBox($"Could not load {TerrainAtlasPath}.", MessageType.Warning);
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
                    DrawAtlasTile(tileRect, mask);
                    var labelRect = new Rect(tileRect.x, tileRect.yMax, tileRect.width, LabelHeight);
                    GUI.Label(labelRect, $"mask {mask}: {DescribeMask(mask)}", EditorStyles.centeredGreyMiniLabel);
                }
            }
        }

        void LoadAtlas()
        {
            terrainAtlas = AssetDatabase.LoadAssetAtPath<Texture2D>(TerrainAtlasPath);
        }

        void DrawAtlasTile(Rect destination, int mask)
        {
            var atlasIndex = TerrainTileResolver.ResolveGrassAtlasIndex(mask)
                + (showDesert ? DesertAtlasOffset : 0);
            var atlasColumn = atlasIndex % AtlasColumns;
            var atlasRow = atlasIndex / AtlasColumns;
            var uv = new Rect(
                atlasColumn / (float)AtlasColumns,
                1f - (atlasRow + 1f) / AtlasRows,
                1f / AtlasColumns,
                1f / AtlasRows);

            GUI.DrawTextureWithTexCoords(destination, terrainAtlas, uv, true);
        }

        static string DescribeMask(int mask)
        {
            var value = string.Empty;
            if ((mask & 1) != 0)
            {
                value += "N";
            }

            if ((mask & 2) != 0)
            {
                value += "E";
            }

            if ((mask & 4) != 0)
            {
                value += "S";
            }

            if ((mask & 8) != 0)
            {
                value += "W";
            }

            return value.Length == 0 ? "isolated" : value;
        }
    }
}
#endif
