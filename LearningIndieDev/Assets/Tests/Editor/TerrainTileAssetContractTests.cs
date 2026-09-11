using System;
using System.Linq;
using NUnit.Framework;
using SaltyGame;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace SaltyGame.EditorTests
{
    [TestFixture]
    public sealed class TerrainTileAssetContractTests
    {
        const string TileRoot = "Assets/Art/Terrain/Blob/128";
        const string AtlasPath = "Assets/Art/Terrain/Terrain_01.spriteatlasv2";

        [Test]
        public void GrassAndDesertTilesMatchTheRuntimeMaskContract()
        {
            AssertFamily(TerrainVisualFamily.Grass);
            AssertFamily(TerrainVisualFamily.Desert);
        }

        [Test]
        public void TerrainAtlasPacksTheBlobTileRoot()
        {
            var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(AtlasPath);
            Assert.That(atlas, Is.Not.Null, $"Expected the terrain atlas at '{AtlasPath}'.");

            var packablePaths = SpriteAtlasExtensions.GetPackables(atlas)
                .Select(AssetDatabase.GetAssetPath)
                .ToArray();

            Assert.That(
                packablePaths,
                Does.Contain(TileRoot),
                $"The terrain atlas must pack '{TileRoot}' so newly imported mask sprites are included automatically.");
        }

        static void AssertFamily(TerrainVisualFamily family)
        {
            var prefix = TerrainVisualFamilies.GetSpritePrefix(family);
            var folder = $"{TileRoot}/{prefix}";
            var expectedPaths = TerrainTileResolver.AllValidMasks
                .Select(mask => $"{folder}/{prefix}_{mask:D3}.png")
                .ToArray();
            var actualPaths = AssetDatabase.FindAssets("t:Texture2D", new[] { folder })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => path.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToArray();

            Assert.That(
                actualPaths,
                Is.EquivalentTo(expectedPaths),
                $"{prefix} must contain exactly the 47 files named for TerrainTileResolver.AllValidMasks.");

            foreach (var path in expectedPaths)
            {
                AssertTile(path);
            }
        }

        static void AssertTile(string path)
        {
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            Assert.That(texture, Is.Not.Null, $"Could not load terrain texture '{path}'.");
            Assert.That(texture.width, Is.EqualTo(128), $"'{path}' must be 128 pixels wide.");
            Assert.That(texture.height, Is.EqualTo(128), $"'{path}' must be 128 pixels high.");

            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            Assert.That(importer, Is.Not.Null, $"'{path}' does not have a texture importer.");
            Assert.That(importer.textureType, Is.EqualTo(TextureImporterType.Sprite), $"'{path}' must import as a sprite.");
            Assert.That(importer.spriteImportMode, Is.EqualTo(SpriteImportMode.Single), $"'{path}' must be one sprite.");
            Assert.That(importer.spritePixelsPerUnit, Is.EqualTo(128f), $"'{path}' must use 128 pixels per unit.");
            Assert.That(importer.mipmapEnabled, Is.False, $"'{path}' must not generate mipmaps.");
            Assert.That(importer.filterMode, Is.EqualTo(FilterMode.Point), $"'{path}' must use point filtering.");

            var expectedName = System.IO.Path.GetFileNameWithoutExtension(path);
            var sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();
            Assert.That(sprites, Has.Length.EqualTo(1), $"'{path}' must import as exactly one sprite.");
            Assert.That(sprites[0].name, Is.EqualTo(expectedName), $"'{path}' must keep its stable runtime sprite name.");
        }
    }
}
