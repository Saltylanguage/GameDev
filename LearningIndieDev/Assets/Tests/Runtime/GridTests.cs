using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace SaltyGame.Tests
{
    [TestFixture]
    [Category("Core")]
    public sealed class GridTests
    {
        [Test]
        public void WrappingConnectsAllEdgesAndCopiesWithoutAliasingNeighbors()
        {
            var grid = new Grid<int>(3, 2, (x, y) => x + y * 3, wrapEdges: true);
            Assert.That(grid.GetCell(-1, 0), Is.EqualTo(2));
            Assert.That(grid.GetCell(3, 0), Is.Zero);
            Assert.That(grid.GetCell(0, -1), Is.EqualTo(3));
            Assert.That(grid.GetCell(0, 2), Is.Zero);
            Assert.That(grid.GetCell(-1, -1), Is.EqualTo(5));
            Assert.That(grid.GetDistance(0, 0, 2, 1), Is.EqualTo(1));
            var pattern = grid.GetPattern(GridPatternTemplates.CreateMooreRange(12));
            Assert.That(pattern.Count, Is.EqualTo(5));
            CollectionAssert.DoesNotContain(pattern.Offsets, Vector2Int.zero);
            var copy = grid.Copy();
            copy.SetCell(-1, -1, 99);
            Assert.That(copy.GetCell(2, 1), Is.EqualTo(99));
            Assert.That(grid.GetCell(2, 1), Is.EqualTo(5));
            Assert.That(grid.Copy(value => value).WrapEdges, Is.True);
            Assert.That(GridSimulation.Step(grid, (cells, x, y) => cells.GetCell(x, y)).WrapEdges, Is.True);
            Assert.That(new Grid<int>(1, 1, true).GetPattern(GridPatternTemplates.CreateMooreRange(1)).Count, Is.Zero);
        }

        [Test]
        public void GridAndStepsPreserveAnExplicitSparseTopology()
        {
            var topology = new HoleTopology(3, 2);
            var source = new Grid<int>(3, 2, (x, y) => x + y * 10, topology);

            Assert.That(source.Count, Is.EqualTo(5));
            Assert.That(source.TryGetCell(1, 0, out _), Is.False);
            Assert.That(source.TryResolveOffset(0, 0, Vector2Int.right, out _), Is.False);
            Assert.That(source.TryResolveOffset(0, 0, Vector2Int.left, out var wrapped), Is.True);
            Assert.That(wrapped, Is.EqualTo(new Vector2Int(2, 0)));

            var copy = source.Copy();
            var copiedPlayableCells = 0;
            var mappedCopy = source.Copy(value =>
            {
                copiedPlayableCells++;
                return value;
            });
            var stepped = GridSimulation.Step(source, (cells, x, y) => cells.GetCell(x, y) + 1);

            Assert.That(copy.Topology, Is.SameAs(topology));
            Assert.That(mappedCopy.Topology, Is.SameAs(topology));
            Assert.That(copiedPlayableCells, Is.EqualTo(5));
            Assert.That(stepped.Topology, Is.SameAs(topology));
            Assert.That(stepped.Count, Is.EqualTo(5));
            Assert.That(stepped.GetCell(2, 1), Is.EqualTo(13));
            Assert.That(stepped.TryGetCell(1, 0, out _), Is.False);
        }

        [Test]
        public void OffsetQueriesUseOriginSpecificSeamConnections()
        {
            var topology = new ShiftedSeamTopology(3, 3);
            var grid = new Grid<int>(3, 3, (x, y) => x + y * 3, topology);

            Assert.That(grid.TryGetCellAtOffset(0, 0, Vector2Int.left, out var cell), Is.True);
            Assert.That(cell, Is.EqualTo(8));
            Assert.That(grid.GetDistance(0, 0, 2, 2), Is.EqualTo(1));
        }

        [Test]
        public void SimulationDataPreservesTopologyAndFingerprintsItsSeams()
        {
            var topology = new ShiftedSeamTopology(3, 5);
            var data = new CellularSimData(3, 5, new Dictionary<SpeciesId, float>(),
                new Dictionary<SpeciesId, SpeciesRules>(), 1f, 0.1f, topology: topology);
            Assert.That(data.Copy().Topology, Is.SameAs(topology));
            Assert.That(data.WithRunTicks(2, 0.1f).Topology, Is.SameAs(topology));
            Assert.That(data.WithGridSize(3, 5).Topology, Is.SameAs(topology));
            Assert.That(SpeciesInitialGridFactory.Create(data, 42).Topology, Is.SameAs(topology));
            Assert.That(data.Fingerprint, Is.Not.EqualTo(data.WithTopology(new RectangularGridTopology(3, 5, true)).Fingerprint));
            Assert.Throws<NotSupportedException>(() => data.WithGridSize(4, 5));
            Assert.Throws<NotSupportedException>(() => data.WithWrapEdges(false));
            Assert.DoesNotThrow(() => new CellularSimData(3, 2,
                new Dictionary<SpeciesId, float>(), new Dictionary<SpeciesId, SpeciesRules>(),
                1f, 0.1f, topology: new HoleTopology(3, 2)));
        }

        [Test]
        public void PerceptionAndNavigationFollowTheSameCustomSeam()
        {
            var cells = new Grid<SpeciesCell>(3, 5, new ShiftedSeamTopology(3, 5));
            cells.SetCell(2, 2, SpeciesCell.Grass(2f));
            var empty = new GridPattern(Array.Empty<Vector2Int>());
            var left = new GridPattern(new[] { Vector2Int.left });
            var up = new GridPattern(new[] { Vector2Int.up });
            var rules = new SpeciesRules(movementSpeed: 1f, movementPattern: left,
                attackPattern: empty, attackAmount: 0, blockPattern: empty, blockAmount: 0,
                dietPattern: up, dietTarget: SpeciesIds.Plant, reproductionPattern: empty,
                reproductionNeighborCount: 0, awareness: new SpeciesAwarenessRules(visionRange: 1));
            Assert.That(SpeciesPerception.TryFindFoodTarget(cells, 0, 0, rules, new System.Random(5), out var food), Is.True);
            Assert.That(food.Location, Is.EqualTo(new Vector2Int(2, 2)));
            Assert.That(SpeciesNavigation.TryFindNextStep(cells, Vector2Int.zero,
                new Vector2Int(2, 3), left, up, new System.Random(5), out var step), Is.True);
            Assert.That(step, Is.EqualTo(new Vector2Int(2, 2)));
        }

        [Test]
        public void ConstructorCreatesGridWithExpectedDimensionsAndDefaults()
        {
            var grid = new Grid<int>(3, 2);

            Assert.That(grid.Width, Is.EqualTo(3));
            Assert.That(grid.Height, Is.EqualTo(2));
            Assert.That(grid.Count, Is.EqualTo(6));
            Assert.That(grid.GetCell(2, 1), Is.Zero);
        }

        [Test]
        public void CoordinateFactoryInitializesEveryCell()
        {
            var grid = new Grid<int>(3, 2, (x, y) => x + y * 10);

            Assert.That(grid.GetCell(0, 0), Is.EqualTo(0));
            Assert.That(grid.GetCell(2, 0), Is.EqualTo(2));
            Assert.That(grid.GetCell(0, 1), Is.EqualTo(10));
            Assert.That(grid.GetCell(2, 1), Is.EqualTo(12));
        }

        [Test]
        public void SetCellUpdatesOnlyTheRequestedLocation()
        {
            var grid = new Grid<string>(2, 2);

            grid.SetCell(1, 0, "occupied");

            Assert.That(grid.GetCell(1, 0), Is.EqualTo("occupied"));
            Assert.That(grid.GetCell(0, 0), Is.Null);
            Assert.That(grid.GetCell(1, 1), Is.Null);
        }

        [Test]
        public void TryMethodsReturnFalseForLocationsOutsideTheGrid()
        {
            var grid = new Grid<int>(2, 2);

            Assert.That(grid.TryGetCell(-1, 0, out var cell), Is.False);
            Assert.That(cell, Is.Zero);
            Assert.That(grid.TrySetCell(2, 1, 5), Is.False);
        }

        [TestCase(-1, 0)]
        [TestCase(0, -1)]
        [TestCase(2, 0)]
        [TestCase(0, 2)]
        public void DirectAccessThrowsForLocationsOutsideTheGrid(int x, int y)
        {
            var grid = new Grid<int>(2, 2);

            Assert.Throws<ArgumentOutOfRangeException>(() => grid.GetCell(x, y));
            Assert.Throws<ArgumentOutOfRangeException>(() => grid.SetCell(x, y, 1));
        }

        [Test]
        public void CopyCreatesIndependentGridStorage()
        {
            var original = new Grid<int>(2, 1, (x, _) => x);

            var copy = original.Copy();
            copy.SetCell(0, 0, 10);

            Assert.That(original.GetCell(0, 0), Is.Zero);
            Assert.That(copy.GetCell(0, 0), Is.EqualTo(10));
        }

        [Test]
        public void CopyFunctionSupportsIndependentReferenceTypeCells()
        {
            var original = new Grid<TestCell>(1, 1, (_, _) => new TestCell(4));

            var copy = original.Copy(cell => new TestCell(cell.Value));
            copy.GetCell(0, 0).Value = 9;

            Assert.That(original.GetCell(0, 0).Value, Is.EqualTo(4));
            Assert.That(copy.GetCell(0, 0).Value, Is.EqualTo(9));
        }

        sealed class TestCell
        {
            public TestCell(int value)
            {
                Value = value;
            }

            public int Value { get; set; }
        }

        sealed class HoleTopology : IGridTopology
        {
            readonly RectangularGridTopology rectangle;

            public HoleTopology(int width, int height)
            {
                rectangle = new RectangularGridTopology(width, height, wrapEdges: true);
            }

            public int Width => rectangle.Width;
            public int Height => rectangle.Height;
            public int CellCount => Width * Height - 1;
            public bool WrapEdges => true;
            public string Fingerprint => $"test-hole:{Width}x{Height}";
            public IEnumerable<Vector2Int> PlayableCoordinates
            {
                get
                {
                    foreach (var position in rectangle.PlayableCoordinates)
                        if (IsPlayableCell(position.x, position.y)) yield return position;
                }
            }

            public bool IsPlayableCell(int x, int y) => rectangle.IsPlayableCell(x, y) && !(x == 1 && y == 0);
            public bool TryResolveCoordinates(ref int x, ref int y) => rectangle.TryResolveCoordinates(ref x, ref y);

            public bool TryResolveOffset(int originX, int originY, Vector2Int offset, out Vector2Int target)
            {
                if (!rectangle.TryResolveOffset(originX, originY, offset, out target) || !IsPlayableCell(target.x, target.y))
                {
                    target = default;
                    return false;
                }
                return true;
            }

            public int GetDistance(int x, int y, int targetX, int targetY) => rectangle.GetDistance(x, y, targetX, targetY);
            public GridPattern GetPattern(GridPattern pattern) => rectangle.GetPattern(pattern);
            public GridPattern GetPattern(int originX, int originY, GridPattern pattern) => rectangle.GetPattern(originX, originY, pattern);
        }

        sealed class ShiftedSeamTopology : IGridTopology
        {
            readonly RectangularGridTopology rectangle;

            public ShiftedSeamTopology(int width, int height)
            {
                rectangle = new RectangularGridTopology(width, height, wrapEdges: true);
            }

            public int Width => rectangle.Width;
            public int Height => rectangle.Height;
            public int CellCount => rectangle.CellCount;
            public bool WrapEdges => true;
            public string Fingerprint => $"test-shifted-seam:{Width}x{Height}";
            public IEnumerable<Vector2Int> PlayableCoordinates => rectangle.PlayableCoordinates;
            public bool IsPlayableCell(int x, int y) => rectangle.IsPlayableCell(x, y);
            public bool TryResolveCoordinates(ref int x, ref int y) => rectangle.TryResolveCoordinates(ref x, ref y);

            public bool TryResolveOffset(int originX, int originY, Vector2Int offset, out Vector2Int target)
            {
                if (originX == 0 && originY == 0 && offset == Vector2Int.left)
                {
                    target = new Vector2Int(2, 2);
                    return true;
                }

                return rectangle.TryResolveOffset(originX, originY, offset, out target);
            }

            public int GetDistance(int x, int y, int targetX, int targetY)
            {
                if ((x == 0 && y == 0 && targetX == 2 && targetY == 2)
                    || (x == 2 && y == 2 && targetX == 0 && targetY == 0)) return 1;
                return rectangle.GetDistance(x, y, targetX, targetY);
            }

            public GridPattern GetPattern(GridPattern pattern) => rectangle.GetPattern(pattern);
            public GridPattern GetPattern(int originX, int originY, GridPattern pattern) => rectangle.GetPattern(originX, originY, pattern);
        }
    }
}
