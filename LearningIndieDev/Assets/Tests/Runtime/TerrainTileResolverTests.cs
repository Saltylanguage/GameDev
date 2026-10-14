using NUnit.Framework;

namespace SaltyGame.Tests
{
    [TestFixture]
    [Category("Core")]
    public sealed class TerrainTileResolverTests
    {
        [Test]
        public void StraightBoundaryUsesOneTransitionAndKeepsBothSides()
        {
            var cells = new Grid<SpeciesCell>(6, 4, (x, _) => x < 3 ? SpeciesCell.Grass(1f) : SpeciesCell.Empty);
            Assert.That(TerrainTileResolver.ResolveVertexMask(cells, 2, 2, TerrainVisualFamily.Grass), Is.EqualTo(255));
            Assert.That(TerrainTileResolver.ResolveVertexMask(cells, 3, 2, TerrainVisualFamily.Grass), Is.EqualTo(241));
            Assert.That(TerrainTileResolver.ResolveVertexMask(cells, 4, 2, TerrainVisualFamily.Grass), Is.Zero);
        }

        [Test]
        public void IsolatedGrassAndBareHoleRetainTheirOwnTerrainAtTheCenter()
        {
            var island = new Grid<SpeciesCell>(3, 3, (_, _) => SpeciesCell.Empty);
            island.SetCell(1, 1, SpeciesCell.Grass(1f));
            var hole = new Grid<SpeciesCell>(3, 3, (_, _) => SpeciesCell.Grass(1f));
            hole.SetCell(1, 1, SpeciesCell.Empty);
            var vertices = new[] { (1, 1), (2, 1), (1, 2), (2, 2) };
            var islandMasks = new[] { 7, 193, 28, 112 };
            var holeMasks = new[] { 253, 127, 247, 223 };
            for (var i = 0; i < vertices.Length; i++)
            {
                var (x, y) = vertices[i];
                Assert.That(TerrainTileResolver.ResolveVertexMask(island, x, y, TerrainVisualFamily.Grass), Is.EqualTo(islandMasks[i]));
                Assert.That(TerrainTileResolver.ResolveVertexMask(hole, x, y, TerrainVisualFamily.Grass), Is.EqualTo(holeMasks[i]));
            }
        }

        [Test]
        public void VertexSamplingExtendsNonWrappedEdgesAndHonorsWrappedEdges()
        {
            var clamped = new Grid<SpeciesCell>(2, 2, (x, _) => x == 0 ? SpeciesCell.Grass(1f) : SpeciesCell.Empty);
            var wrapped = new Grid<SpeciesCell>(2, 2, (x, _) => x == 0 ? SpeciesCell.Grass(1f) : SpeciesCell.Empty, wrapEdges: true);
            Assert.That(TerrainTileResolver.ResolveVertexMask(clamped, 0, 1, TerrainVisualFamily.Grass), Is.EqualTo(255));
            Assert.That(TerrainTileResolver.ResolveVertexMask(clamped, 2, 1, TerrainVisualFamily.Grass), Is.Zero);
            Assert.That(TerrainTileResolver.ResolveVertexMask(wrapped, 0, 1, TerrainVisualFamily.Grass), Is.EqualTo(31));
            Assert.That(TerrainTileResolver.ResolveVertexMask(wrapped, 2, 1, TerrainVisualFamily.Grass), Is.EqualTo(31));
        }

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
        public void GrassAndDesertOwnSeparatePresentationFamilies()
        {
            Assert.That(
                TerrainVisualFamilies.Get(TerrainIds.Grass),
                Is.EqualTo(TerrainVisualFamily.Grass));
            Assert.That(
                TerrainVisualFamilies.Get(TerrainIds.Desert),
                Is.EqualTo(TerrainVisualFamily.Desert));
            Assert.That(
                TerrainTileResolver.GetTerrainSpriteName(TerrainIds.Desert, TerrainTileResolver.FullMask),
                Is.EqualTo("Desert_255"));
        }

        [Test]
        public void BareUsesGrassNeighborMaskWithoutOwningVisualFamily()
        {
            Assert.That(TerrainVisualFamilies.TryGet(TerrainIds.Bare, out _), Is.False);
            Assert.That(
                TerrainVisualFamilies.TryGetTerrainTileFamily(TerrainIds.Bare, out var family),
                Is.True);
            Assert.That(family, Is.EqualTo(TerrainVisualFamily.Grass));
            Assert.Throws<System.ArgumentOutOfRangeException>(() =>
                TerrainTileResolver.GetTerrainSpriteName(TerrainIds.Bare, TerrainTileResolver.FullMask));

            var cells = new Grid<SpeciesCell>(1, 1, (_, _) => SpeciesCell.Empty);
            Assert.That(
                TerrainTileResolver.ResolveTerrainMask(cells, 0, 0, TerrainIds.Bare),
                Is.Zero);
        }

        [Test]
        public void GrassMaskCanBeResolvedAtABareCell()
        {
            var cells = new Grid<SpeciesCell>(3, 3, (_, _) => SpeciesCell.Grass(1f));
            cells.SetCell(1, 1, SpeciesCell.Empty);

            Assert.That(
                TerrainTileResolver.ResolveTerrainMask(cells, 1, 1, TerrainIds.Bare),
                Is.EqualTo(TerrainTileResolver.FullMask));
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
