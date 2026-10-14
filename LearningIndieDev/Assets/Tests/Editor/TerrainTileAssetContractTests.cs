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
    [Category("Authoring")]
    public sealed class TerrainTileAssetContractTests
    {
        const string TileRoot = "Assets/Art/Terrain/Blob/64";
        const string AtlasPath = "Assets/Art/Terrain/Terrain_01.spriteatlasv2";

        [Test]
        public void SharedVertexTilesPreserveEveryCompatibleGrassEdge()
        {
            var masks = Enumerable.Range(0, 16).Select(state =>
            {
                var cells = new Grid<SpeciesCell>(2, 2, (_, _) => SpeciesCell.Empty);
                if ((state & 1) != 0) cells.SetCell(0, 1, SpeciesCell.Grass(1f));
                if ((state & 2) != 0) cells.SetCell(1, 1, SpeciesCell.Grass(1f));
                if ((state & 4) != 0) cells.SetCell(1, 0, SpeciesCell.Grass(1f));
                if ((state & 8) != 0) cells.SetCell(0, 0, SpeciesCell.Grass(1f));
                return TerrainTileResolver.ResolveVertexMask(cells, 1, 1, TerrainVisualFamily.Grass);
            }).ToArray();
            Assert.That(masks.Distinct().Count(), Is.EqualTo(16));
            var textures = new System.Collections.Generic.Dictionary<int, Texture2D>();
            try
            {
                foreach (var mask in masks)
                {
                    var texture = new Texture2D(64, 64);
                    textures.Add(mask, texture);
                    Assert.That(texture.LoadImage(System.IO.File.ReadAllBytes($"{TileRoot}/Grass/Grass_{mask:D3}.png")), Is.True);
                }
                foreach (var a in masks)
                foreach (var b in masks)
                {
                    var horizontal = ((a & TerrainTileResolver.NorthEast) != 0) == ((b & TerrainTileResolver.NorthWest) != 0)
                        && ((a & TerrainTileResolver.SouthEast) != 0) == ((b & TerrainTileResolver.SouthWest) != 0);
                    var vertical = ((a & TerrainTileResolver.NorthWest) != 0) == ((b & TerrainTileResolver.SouthWest) != 0)
                        && ((a & TerrainTileResolver.NorthEast) != 0) == ((b & TerrainTileResolver.SouthEast) != 0);
                    for (var pixel = 0; pixel < 64; pixel++)
                    {
                        if (horizontal) Assert.That(IsGrass(textures[a].GetPixel(63, pixel)),
                            Is.EqualTo(IsGrass(textures[b].GetPixel(0, pixel))), $"Horizontal edge {a}/{b}, pixel {pixel}");
                        if (vertical) Assert.That(IsGrass(textures[a].GetPixel(pixel, 63)),
                            Is.EqualTo(IsGrass(textures[b].GetPixel(pixel, 0))), $"Vertical edge {a}/{b}, pixel {pixel}");
                    }
                }
            }
            finally
            {
                foreach (var texture in textures.Values) UnityEngine.Object.DestroyImmediate(texture);
            }
        }

        static bool IsGrass(Color color) => color.g > color.r + 5f / 255f;

        [Test]
        public void GrassTilesMatchTheRuntimeMaskContract()
        {
            AssertFamily(TerrainVisualFamily.Grass);
        }

        [Test]
        public void ForestEdgeFlourishesPreservePixelArtAndPackWithTerrain()
        {
            var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(AtlasPath);
            Assert.That(atlas, Is.Not.Null);
            var atlasImporter = (SpriteAtlasImporter)AssetImporter.GetAtPath(AtlasPath);
            Assert.That(atlasImporter.GetPlatformSettings("DefaultTexturePlatform").textureCompression,
                Is.EqualTo(TextureImporterCompression.Uncompressed),
                "Atlas compression must preserve Noesis premultiplied alpha at transparent sprite edges.");
            foreach (var name in ForestEdgeFlourishResolver.SpriteNames)
            {
                var path = $"{TileRoot}/ForestEdgeFlourishes/{name}.png";
                AssertTile(path);
                var sprite = atlas.GetSprite(name);
                Assert.That(sprite, Is.Not.Null, $"Missing packed flourish '{name}'.");
                UnityEngine.Object.DestroyImmediate(sprite);
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                Assert.That(importer.alphaIsTransparency, Is.True);
                Assert.That(AssetDatabase.GetLabels(importer), Does.Contain("Noesis"),
                    $"'{path}' must use Noesis premultiplied alpha to avoid opaque color squares.");
            }
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

        [Test]
        public void TerrainAtlasExposesEveryGrassMaskByName()
        {
            var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(AtlasPath);
            Assert.That(atlas, Is.Not.Null, $"Expected the terrain atlas at '{AtlasPath}'.");

            var sprites = new Sprite[atlas.spriteCount];
            atlas.GetSprites(sprites);
            var names = sprites.Where(sprite => sprite != null).Select(sprite => sprite.name).ToArray();

            foreach (var mask in TerrainTileResolver.AllValidMasks)
            {
                var expectedName = TerrainTileResolver.GetTerrainSpriteName(TerrainVisualFamily.Grass, mask);
                Assert.That(
                    names.Any(name => name.StartsWith(expectedName, StringComparison.Ordinal)),
                    Is.True,
                    $"The terrain atlas is missing '{expectedName}'.");
            }
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
            Assert.That(texture.width, Is.EqualTo(64), $"'{path}' must be 64 pixels wide.");
            Assert.That(texture.height, Is.EqualTo(64), $"'{path}' must be 64 pixels high.");

            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            Assert.That(importer, Is.Not.Null, $"'{path}' does not have a texture importer.");
            Assert.That(importer.textureType, Is.EqualTo(TextureImporterType.Sprite), $"'{path}' must import as a sprite.");
            Assert.That(importer.spriteImportMode, Is.EqualTo(SpriteImportMode.Single), $"'{path}' must be one sprite.");
            Assert.That(importer.spritePixelsPerUnit, Is.EqualTo(64f), $"'{path}' must use 64 pixels per unit.");
            Assert.That(importer.mipmapEnabled, Is.False, $"'{path}' must not generate mipmaps.");
            Assert.That(importer.filterMode, Is.EqualTo(FilterMode.Point), $"'{path}' must use point filtering.");
            Assert.That(importer.textureCompression, Is.EqualTo(TextureImporterCompression.Uncompressed), $"'{path}' must preserve authored pixel colors.");

            var expectedName = System.IO.Path.GetFileNameWithoutExtension(path);
            var sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();
            Assert.That(sprites, Has.Length.EqualTo(1), $"'{path}' must import as exactly one sprite.");
            Assert.That(sprites[0].name, Is.EqualTo(expectedName), $"'{path}' must keep its stable runtime sprite name.");
        }
    }
}
