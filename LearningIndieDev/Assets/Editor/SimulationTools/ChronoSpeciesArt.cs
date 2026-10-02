using System;
using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;

namespace SaltyGame.EditorTools
{
    public static class ChronoSpeciesArt
    {
        const string Root = "Assets/Art/Species/Animals/";

        // Existing paths and GUIDs are shared by the atlas, Noesis UI and Species Catalog.
        [MenuItem("Salty Game/Art/Rebuild Chrono Species Icons")]
        public static void Rebuild()
        {
            Export("Chrono_Fox 1.png", "Animals_01_Fox");
            Export("Chrono_Rabbit_Tile_Face.png", "Animals_01_Rabbit");

            var atlas = (SpriteAtlasImporter)AssetImporter.GetAtPath(Root + "Animals_01.spriteatlasv2");
            var settings = atlas.textureSettings;
            settings.filterMode = FilterMode.Point;
            settings.generateMipMaps = false;
            atlas.textureSettings = settings;
            foreach (var platform in new[] { "DefaultTexturePlatform", "Standalone", "WebGL" })
            {
                var platformSettings = atlas.GetPlatformSettings(platform);
                platformSettings.textureCompression = TextureImporterCompression.Uncompressed;
                platformSettings.format = TextureImporterFormat.RGBA32;
                if (platform != "DefaultTexturePlatform") platformSettings.overridden = false;
                atlas.SetPlatformSettings(platformSettings);
            }
            atlas.SaveAndReimport();
            AssetDatabase.SaveAssets();
            Debug.Log("Chrono species icons rebuilt: 32/64/128 for Fox and Rabbit, plus legacy atlas-compatible exports. Original artwork and asset GUIDs preserved.");
        }

        static void Export(string sourceName, string outputName)
        {
            var source = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            try
            {
                if (!source.LoadImage(File.ReadAllBytes(Root + sourceName)))
                    throw new InvalidDataException("Cannot decode " + sourceName);
                var pixels = source.GetPixels32();
                foreach (var size in new[] { 32, 64, 128, 1024 })
                {
                    var path = Root + "Standardized/" + (size == 1024 ? "" : size + "/") + outputName + ".png";
                    var output = new Texture2D(size, size, TextureFormat.RGBA32, false);
                    try
                    {
                        var resized = new Color32[size * size];
                        for (var y = 0; y < size; y++)
                        for (var x = 0; x < size; x++)
                        {
                            // Sample pixel centers; preserve the handmade palette and transparency.
                            var sx = Math.Min(source.width - 1, (2 * x + 1) * source.width / (2 * size));
                            var sy = Math.Min(source.height - 1, (2 * y + 1) * source.height / (2 * size));
                            resized[y * size + x] = pixels[sy * source.width + sx];
                        }
                        output.SetPixels32(resized);
                        output.Apply();
                        File.WriteAllBytes(path, output.EncodeToPNG());
                    }
                    finally { UnityEngine.Object.DestroyImmediate(output); }

                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
                    var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                    importer.filterMode = FilterMode.Point;
                    importer.mipmapEnabled = false;
                    importer.alphaIsTransparency = true;
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    foreach (var platform in new[] { "Standalone", "WebGL" })
                        importer.ClearPlatformTextureSettings(platform);
                    importer.SaveAndReimport();
                    var imported = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    if (imported.width != size || imported.height != size)
                        throw new InvalidDataException("Unexpected imported dimensions: " + path);
                    Debug.Log($"Chrono export: {path} ({size}x{size})");
                }
            }
            finally { UnityEngine.Object.DestroyImmediate(source); }
        }
    }
}
