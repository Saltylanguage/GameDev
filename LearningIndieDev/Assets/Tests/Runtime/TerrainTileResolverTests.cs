using NUnit.Framework;

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

            for (var mask = 0; mask < TerrainTileResolver.CardinalVariantCount; mask++)
            {
                variants.Add(TerrainTileResolver.ResolveGrassAtlasIndex(mask));
            }

            Assert.That(variants.Count, Is.EqualTo(TerrainTileResolver.CardinalVariantCount));
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
                () => TerrainTileResolver.ResolveGrassAtlasIndex(TerrainTileResolver.NeighborMaskCount));
        }

        [Test]
        public void InvalidCardinalMaskIsRejected()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(
                () => TerrainTileResolver.ResolveCardinalAtlasIndex(TerrainTileResolver.CardinalVariantCount));
        }

        [Test]
        public void NeighborMaskIncludesDiagonalTerrain()
        {
            var cells = CreateCells(1 | 2 | 4 | 8);
            cells.SetCell(2, 2, SpeciesCell.Grass(1f));

            Assert.That(
                TerrainTileResolver.ComputeNeighborMask(cells, 1, 1, TerrainIds.Grass),
                Is.EqualTo(1 | 2 | 4 | 8 | TerrainTileResolver.NorthEast));
        }

        [Test]
        public void OpenDiagonalCornerIsReportedWhenCardinalSidesAreConnected()
        {
            var mask = 1 | 2 | 4 | 8;

            Assert.That(
                TerrainTileResolver.GetOpenDiagonalCorners(mask),
                Is.EqualTo(TerrainTileResolver.DiagonalMask));
        }

        [Test]
        public void ConnectedDiagonalCornerIsNotReportedAsOpen()
        {
            var mask = 1 | 2 | TerrainTileResolver.NorthEast;

            Assert.That(
                TerrainTileResolver.GetOpenDiagonalCorners(mask),
                Is.EqualTo(0));
        }

        [Test]
        public void BottomRightDiagonalGapIsReportedIndependently()
        {
            var cells = CreateCells(TerrainTileResolver.CardinalMask);
            cells.SetCell(0, 0, SpeciesCell.Grass(1f));
            cells.SetCell(2, 2, SpeciesCell.Grass(1f));
            cells.SetCell(0, 2, SpeciesCell.Grass(1f));

            var mask = TerrainTileResolver.ComputeNeighborMask(cells, 1, 1, TerrainIds.Grass);

            Assert.That(
                TerrainTileResolver.GetOpenDiagonalCorners(mask),
                Is.EqualTo(TerrainTileResolver.SouthEast));
        }

        [Test]
        public void NorthAndSouthGapsLeaveEastAndWestCardinalConnections()
        {
            var cells = CreateCells(TerrainTileResolver.East | TerrainTileResolver.West);

            Assert.That(
                TerrainTileResolver.ComputeNeighborMask(cells, 1, 1, TerrainIds.Grass),
                Is.EqualTo(TerrainTileResolver.East | TerrainTileResolver.West));
        }

        [Test]
        public void FourDiagonalGapsAreAllReported()
        {
            Assert.That(
                TerrainTileResolver.GetOpenDiagonalCorners(TerrainTileResolver.CardinalMask),
                Is.EqualTo(TerrainTileResolver.DiagonalMask));
        }

        [Test]
        public void DiagonalMaskUsesCardinalAtlasFallbackUntilCornerArtExists()
        {
            Assert.That(
                TerrainTileResolver.ResolveGrassAtlasIndex(TerrainTileResolver.CardinalMask),
                Is.EqualTo(TerrainTileResolver.ResolveGrassAtlasIndex(
                    TerrainTileResolver.CardinalMask | TerrainTileResolver.DiagonalMask)));
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
