using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace SaltyGame.Tests
{
    [TestFixture]
    public sealed class TerrainTileResolverTests
    {
        [Test]
        public void IsolatedGrassUsesTheClosedVariant()
        {
            var cells = CreateCells(0);

            Assert.That(TerrainTileResolver.ResolveGrassTileIndex(cells, 1, 1), Is.EqualTo(5));
        }

        [Test]
        public void FullyConnectedGrassUsesTheCenterVariant()
        {
            var cells = CreateCells(1 | 2 | 4 | 8);

            Assert.That(TerrainTileResolver.ResolveGrassTileIndex(cells, 1, 1), Is.EqualTo(0));
        }

        [Test]
        public void EveryNeighborMaskResolvesToOneUniqueAtlasVariant()
        {
            var variants = new System.Collections.Generic.HashSet<int>();

            for (var mask = 0; mask < TerrainTileResolver.VariantCount; mask++)
            {
                variants.Add(TerrainTileResolver.ResolveGrassAtlasIndex(mask));
            }

            Assert.That(variants.Count, Is.EqualTo(TerrainTileResolver.VariantCount));
            Assert.That(variants, Is.EquivalentTo(new[]
            {
                0, 1, 2, 3,
                4, 5, 6, 7,
                8, 9, 10, 11,
                12, 13, 14, 15,
            }));
        }

        [Test]
        public void InvalidNeighborMaskIsRejected()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(
                () => TerrainTileResolver.ResolveGrassAtlasIndex(TerrainTileResolver.VariantCount));
        }

        [Test]
        public void TerrainFamilyResolverUsesTheSameMaskRulesForDesert()
        {
            var desert = new TerrainDefinition(
                TerrainIds.Desert,
                isPassable: true,
                movementCost: 1f,
                providesResource: false,
                presentationColor: UnityEngine.Color.yellow);
            var cells = new Grid<SpeciesCell>(3, 3, (_, _) => SpeciesCell.Empty);
            cells.SetCell(1, 1, SpeciesCell.FromTerrain(desert));

            Assert.That(
                TerrainTileResolver.ResolveTerrainTileIndex(cells, 1, 1, TerrainIds.Desert),
                Is.EqualTo(5));
        }

        [Test]
        public void EditorSmartTilingPreviewLoadsTheAuthoredTerrainSheet()
        {
            var atlas = AssetDatabase.LoadAssetAtPath<Texture2D>(
                "Assets/Art/Terrain/Terrain_01_SpriteSheet.png");

            Assert.That(atlas, Is.Not.Null);
            Assert.That(atlas.width % 4, Is.EqualTo(0));
            Assert.That(atlas.height % 8, Is.EqualTo(0));
            Assert.That(atlas.width / 4, Is.EqualTo(128));
            Assert.That(atlas.height / 8, Is.EqualTo(128));

            for (var mask = 0; mask < TerrainTileResolver.VariantCount; mask++)
            {
                Assert.That(TerrainTileResolver.ResolveGrassAtlasIndex(mask), Is.InRange(0, 15));
            }
        }

        static Grid<SpeciesCell> CreateCells(int neighborMask)
        {
            var cells = new Grid<SpeciesCell>(3, 3, (_, _) => SpeciesCell.Empty);
            cells.SetCell(1, 1, SpeciesCell.Grass(1f));
            if ((neighborMask & 1) != 0) cells.SetCell(1, 2, SpeciesCell.Grass(1f));
            if ((neighborMask & 2) != 0) cells.SetCell(2, 1, SpeciesCell.Grass(1f));
            if ((neighborMask & 4) != 0) cells.SetCell(1, 0, SpeciesCell.Grass(1f));
            if ((neighborMask & 8) != 0) cells.SetCell(0, 1, SpeciesCell.Grass(1f));
            return cells;
        }
    }
}
