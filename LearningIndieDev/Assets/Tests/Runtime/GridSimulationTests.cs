using System;
using NUnit.Framework;
using UnityEngine;

namespace SaltyGame.Tests
{
    [TestFixture]
    public sealed class GridSimulationTests
    {
        static readonly GridPattern MooreNeighborhood = new GridPattern(new[]
        {
            new Vector2Int(-1, 1),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(-1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(1, -1),
        });

        static readonly GridPattern HorizontalNeighborhood = new GridPattern(new[]
        {
            Vector2Int.left,
            Vector2Int.right,
        });

        [Test]
        public void StepSupportsBinaryLifeCellsWithoutChangingTheSource()
        {
            var source = new Grid<LifeCell>(3, 3);
            source.SetCell(1, 0, new LifeCell(true));
            source.SetCell(1, 1, new LifeCell(true));
            source.SetCell(1, 2, new LifeCell(true));

            var next = LifeSimulation.Step(source, MooreNeighborhood);

            Assert.That(source.GetCell(1, 0).IsAlive, Is.True);
            Assert.That(source.GetCell(0, 1).IsAlive, Is.False);
            Assert.That(next.GetCell(0, 1).IsAlive, Is.True);
            Assert.That(next.GetCell(1, 1).IsAlive, Is.True);
            Assert.That(next.GetCell(2, 1).IsAlive, Is.True);
            Assert.That(next.GetCell(1, 0).IsAlive, Is.False);
            Assert.That(next.GetCell(1, 2).IsAlive, Is.False);
        }

        [Test]
        public void StepSupportsContinuousHeatCells()
        {
            var source = new Grid<HeatCell>(3, 1, (x, _) => new HeatCell(x == 1 ? 100f : 0f));

            var next = GridSimulation.Step(source, (grid, x, y) =>
            {
                var total = grid.GetCell(x, y).Temperature;
                var sampleCount = 1;
                foreach (var offset in HorizontalNeighborhood.Offsets)
                {
                    if (grid.TryGetCell(x + offset.x, y + offset.y, out var neighbor))
                    {
                        total += neighbor.Temperature;
                        sampleCount++;
                    }
                }

                return new HeatCell(total / sampleCount);
            });

            Assert.That(next.GetCell(0, 0).Temperature, Is.EqualTo(50f));
            Assert.That(next.GetCell(1, 0).Temperature, Is.EqualTo(100f / 3f).Within(0.001f));
            Assert.That(next.GetCell(2, 0).Temperature, Is.EqualTo(50f));
        }

        [Test]
        public void StepSupportsMultiStateElementCells()
        {
            var source = new Grid<ElementCell>(3, 1);
            source.SetCell(0, 0, new ElementCell(ElementCell.State.Plant));
            source.SetCell(1, 0, new ElementCell(ElementCell.State.Fire));

            var next = GridSimulation.Step(source, (grid, x, y) =>
            {
                var state = grid.GetCell(x, y).CurrentState;
                if (state == ElementCell.State.Fire)
                {
                    return new ElementCell(ElementCell.State.Empty);
                }

                var touchesFire = CountMatchingNeighbors(
                    grid,
                    HorizontalNeighborhood,
                    x,
                    y,
                    cell => cell.CurrentState == ElementCell.State.Fire) > 0;
                return new ElementCell(state == ElementCell.State.Plant && touchesFire
                    ? ElementCell.State.Fire
                    : state);
            });

            Assert.That(next.GetCell(0, 0).CurrentState, Is.EqualTo(ElementCell.State.Fire));
            Assert.That(next.GetCell(1, 0).CurrentState, Is.EqualTo(ElementCell.State.Empty));
            Assert.That(next.GetCell(2, 0).CurrentState, Is.EqualTo(ElementCell.State.Empty));
        }

        static int CountMatchingNeighbors<T>(
            Grid<T> grid,
            GridPattern neighborhood,
            int x,
            int y,
            Func<T, bool> matches)
        {
            var count = 0;
            foreach (var offset in neighborhood.Offsets)
            {
                if (grid.TryGetCell(x + offset.x, y + offset.y, out var neighbor) && matches(neighbor))
                {
                    count++;
                }
            }

            return count;
        }
    }
}
