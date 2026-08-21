using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace SaltyGame.Tests
{
    [TestFixture]
    public sealed class TerrainTileResolverTests
    {
        [Test]
        public void EmptyCornerMaskDrawsNoTile()
        {
            Assert.That(TerrainTileResolver.ResolveTerrainAtlasIndex(0), Is.EqualTo(-1));
        }

        [Test]
        public void EveryNonEmptyCornerMaskUsesOneNamedVariant()
        {
            var variants = new System.Collections.Generic.HashSet<int>();

            for (var mask = 1; mask < TerrainTileResolver.CornerMaskCount; mask++)
            {
                variants.Add(TerrainTileResolver.ResolveTerrainAtlasIndex(mask));
            }

            Assert.That(variants.Count, Is.EqualTo(TerrainTileResolver.TerrainVariantCount));
            Assert.That(variants, Is.EquivalentTo(new[]
            {
                0, 1, 2, 3, 4,
                5, 6, 7, 8, 9,
                10, 11, 12, 13, 14,
            }));
        }

        [Test]
        public void VariantNamesFollowCornerMaskOrder()
        {
            Assert.That(TerrainTileResolver.GetVariantName(0), Is.EqualTo("TopRight"));
            Assert.That(TerrainTileResolver.GetVariantName(2), Is.EqualTo("TopMiddle"));
            Assert.That(TerrainTileResolver.GetVariantName(9), Is.EqualTo("MiddleLeft"));
            Assert.That(TerrainTileResolver.GetVariantName(TerrainTileResolver.FullVariantIndex), Is.EqualTo("Full"));
        }

        [Test]
        public void CornerMaskSamplesTheFourCellsAroundTheVisualTile()
        {
            var cells = new Grid<SpeciesCell>(3, 3, (_, _) => SpeciesCell.Empty);
            cells.SetCell(0, 0, SpeciesCell.Grass(1f));
            cells.SetCell(1, 0, SpeciesCell.Grass(1f));
            cells.SetCell(0, 1, SpeciesCell.Grass(1f));
            cells.SetCell(1, 1, SpeciesCell.Grass(1f));

            Assert.That(
                TerrainTileResolver.ComputeCornerMask(cells, 1, 1, TerrainIds.Grass),
                Is.EqualTo(
                    TerrainTileResolver.SouthWest
                    | TerrainTileResolver.SouthEast
                    | TerrainTileResolver.NorthWest
                    | TerrainTileResolver.NorthEast));
        }

        [Test]
        public void CurrentCellOnlyUsesTheNorthEastVariant()
        {
            var cells = new Grid<SpeciesCell>(3, 3, (_, _) => SpeciesCell.Empty);
            cells.SetCell(1, 1, SpeciesCell.Grass(1f));

            Assert.That(
                TerrainTileResolver.ComputeCornerMask(cells, 1, 1, TerrainIds.Grass),
                Is.EqualTo(TerrainTileResolver.NorthEast));
            Assert.That(
                TerrainTileResolver.ResolveGrassTileIndex(cells, 1, 1),
                Is.EqualTo(TerrainTileResolver.ResolveTerrainAtlasIndex(TerrainTileResolver.NorthEast)));
        }

        [Test]
        public void DesertUsesTheSameCornerRules()
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
                Is.EqualTo(TerrainTileResolver.ResolveTerrainAtlasIndex(TerrainTileResolver.NorthEast)));
            Assert.That(
                TerrainTileResolver.GetTerrainSpriteName(TerrainIds.Desert, TerrainTileResolver.FullVariantIndex),
                Is.EqualTo("Desert_Full"));
        }

        [Test]
        public void InvalidCornerMaskIsRejected()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(
                () => TerrainTileResolver.ResolveTerrainAtlasIndex(TerrainTileResolver.CornerMaskCount));
        }
    }
}
