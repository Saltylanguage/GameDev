using NUnit.Framework;

namespace SaltyGame.Tests
{
    [TestFixture]
    public sealed class TerrainTileResolverTests
    {
        [Test]
        public void EveryRawMaskNormalizesToOneOf47BlobMasks()
        {
            var normalized = new System.Collections.Generic.HashSet<int>();
            for (var raw = 0; raw <= TerrainTileResolver.FullMask; raw++)
            {
                var mask = TerrainTileResolver.NormalizeMask(raw);
                normalized.Add(mask);
                Assert.That(TerrainTileResolver.IsValidMask(mask), Is.True, $"Raw mask {raw} normalized to {mask}.");
            }

            Assert.That(normalized.Count, Is.EqualTo(TerrainTileResolver.TerrainVariantCount));
            Assert.That(normalized, Is.EquivalentTo(TerrainTileResolver.AllValidMasks));
        }

        [Test]
        public void DiagonalOnlyContactsPromoteAdjacentCardinals()
        {
            Assert.That(TerrainTileResolver.NormalizeMask(TerrainTileResolver.NorthEast),
                Is.EqualTo(TerrainTileResolver.North | TerrainTileResolver.NorthEast | TerrainTileResolver.East));
            Assert.That(
                TerrainTileResolver.NormalizeMask(
                    TerrainTileResolver.North | TerrainTileResolver.East | TerrainTileResolver.NorthEast),
                Is.EqualTo(TerrainTileResolver.North | TerrainTileResolver.East | TerrainTileResolver.NorthEast));
        }

        [Test]
        public void RotationMasksMatchGeneratedTiles()
        {
            Assert.That(TerrainTileResolver.IsValidMask(29), Is.True);
            Assert.That(TerrainTileResolver.IsValidMask(116), Is.True);
            Assert.That(TerrainTileResolver.IsValidMask(209), Is.True);
            Assert.That(TerrainTileResolver.IsValidMask(71), Is.True);
        }

        [Test]
        public void BareAndDesertSharePresentationFamily()
        {
            Assert.That(
                TerrainVisualFamilies.Get(TerrainIds.Bare),
                Is.EqualTo(TerrainVisualFamilies.Get(TerrainIds.Desert)));
            Assert.That(
                TerrainTileResolver.GetTerrainSpriteName(TerrainIds.Bare, TerrainTileResolver.FullMask),
                Is.EqualTo("Desert_255"));
        }

        [Test]
        public void TerrainMaskUsesCellCenteredEightNeighbors()
        {
            var cells = new Grid<SpeciesCell>(3, 3, (_, _) => SpeciesCell.Empty);
            cells.SetCell(1, 2, SpeciesCell.Grass(1f));
            cells.SetCell(2, 1, SpeciesCell.Grass(1f));
            cells.SetCell(2, 0, SpeciesCell.Grass(1f));
            cells.SetCell(1, 0, SpeciesCell.Grass(1f));

            Assert.That(
                TerrainTileResolver.ResolveTerrainMask(cells, 1, 1, TerrainIds.Grass),
                Is.EqualTo(29));
        }
    }
}
