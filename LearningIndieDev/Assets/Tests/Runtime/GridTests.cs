using System;
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
    }
}
