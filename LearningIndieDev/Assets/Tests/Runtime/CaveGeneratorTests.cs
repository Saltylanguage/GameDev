using NUnit.Framework;
using UnityEngine;

namespace SaltyGame.Tests
{
    [TestFixture]
    public sealed class CaveGeneratorTests
    {
        [Test]
        public void GenerateProducesTheSameCaveForTheSameSeed()
        {
            var settings = new CaveGenerationSettings(12, 8, simulationSteps: 3);

            var first = CaveGenerator.Generate(settings, 12345);
            var second = CaveGenerator.Generate(settings, 12345);

            for (var y = 0; y < settings.Height; y++)
            {
                for (var x = 0; x < settings.Width; x++)
                {
                    Assert.That(second.GetCell(x, y).IsWall, Is.EqualTo(first.GetCell(x, y).IsWall));
                }
            }
        }

        [Test]
        public void InitialGenerationAlwaysCreatesBoundaryWalls()
        {
            var settings = new CaveGenerationSettings(5, 4, initialWallProbability: 0f, simulationSteps: 0);

            var cave = CaveGenerator.Generate(settings, 1);

            for (var x = 0; x < cave.Width; x++)
            {
                Assert.That(cave.GetCell(x, 0).IsWall, Is.True);
                Assert.That(cave.GetCell(x, cave.Height - 1).IsWall, Is.True);
            }

            for (var y = 0; y < cave.Height; y++)
            {
                Assert.That(cave.GetCell(0, y).IsWall, Is.True);
                Assert.That(cave.GetCell(cave.Width - 1, y).IsWall, Is.True);
            }

            Assert.That(cave.GetCell(2, 2).IsWall, Is.False);
        }

        [Test]
        public void SimulateStepAppliesTheSuppliedNeighborhoodAndThreshold()
        {
            var source = new Grid<CaveCell>(3, 1, (x, _) => new CaveCell(x == 0));
            var lookLeft = new GridPattern(new[] { Vector2Int.left });

            var result = CaveGenerator.SimulateStep(source, lookLeft, 1);

            Assert.That(result.GetCell(0, 0).IsWall, Is.True, "Locations outside the grid count as walls.");
            Assert.That(result.GetCell(1, 0).IsWall, Is.True, "The wall to the left should create a wall.");
            Assert.That(result.GetCell(2, 0).IsWall, Is.False);
        }

        [Test]
        public void SimulateStepDoesNotModifyItsSourceGrid()
        {
            var source = new Grid<CaveCell>(3, 3, (_, _) => new CaveCell(true));
            source.SetCell(1, 1, new CaveCell(false));

            var result = CaveGenerator.SimulateStep(source, CaveCell.Neighborhood, 5);

            Assert.That(source.GetCell(1, 1).IsWall, Is.False);
            Assert.That(result.GetCell(1, 1).IsWall, Is.True);
        }

        [Test]
        public void CaveCellNeighborhoodContainsTheEightSurroundingOffsets()
        {
            Assert.That(CaveCell.Neighborhood.Count, Is.EqualTo(8));
            Assert.That(CaveCell.Neighborhood.Offsets, Does.Contain(Vector2Int.up));
            Assert.That(CaveCell.Neighborhood.Offsets, Does.Contain(Vector2Int.down));
            Assert.That(CaveCell.Neighborhood.Offsets, Does.Contain(Vector2Int.left));
            Assert.That(CaveCell.Neighborhood.Offsets, Does.Contain(Vector2Int.right));
            Assert.That(CaveCell.Neighborhood.Offsets, Does.Not.Contain(Vector2Int.zero));
        }
    }
}
