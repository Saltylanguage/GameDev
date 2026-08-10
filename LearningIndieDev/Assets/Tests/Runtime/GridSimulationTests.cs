using NUnit.Framework;
using UnityEngine;

namespace SaltyGame.Tests
{
    [TestFixture]
    public sealed class GridSimulationTests
    {
        static readonly GridPattern MooreNeighborhood = new GridPattern(new[]
        {
            new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1),
            new Vector2Int(-1, 0), new Vector2Int(1, 0),
            new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1),
        });

        static readonly GridPattern HorizontalNeighborhood = new GridPattern(new[]
        {
            Vector2Int.left,
            Vector2Int.right,
        });

        [Test]
        public void InitializationIsDeterministicForASeed()
        {
            var first = LifeSimulation.CreateRandom(6, 4, 1234, 0.35f);
            var second = LifeSimulation.CreateRandom(6, 4, 1234, 0.35f);

            for (var y = 0; y < first.Height; y++)
            {
                for (var x = 0; x < first.Width; x++)
                {
                    Assert.That(second.GetCell(x, y).CurrentState, Is.EqualTo(first.GetCell(x, y).CurrentState));
                }
            }
        }

        [Test]
        public void LifeRulesReadThePreviousGeneration()
        {
            var source = new Grid<LifeCell>(3, 3);
            source.SetCell(1, 0, new LifeCell(LifeCell.State.Life));
            source.SetCell(1, 1, new LifeCell(LifeCell.State.Life));
            source.SetCell(1, 2, new LifeCell(LifeCell.State.Life));

            var next = LifeSimulation.Step(source, MooreNeighborhood, 123);

            Assert.That(source.GetCell(0, 1).CurrentState, Is.EqualTo(LifeCell.State.Empty));
            Assert.That(next.GetCell(0, 1).CurrentState, Is.EqualTo(LifeCell.State.Life));
            Assert.That(next.GetCell(1, 1).CurrentState, Is.EqualTo(LifeCell.State.Life));
            Assert.That(next.GetCell(2, 1).CurrentState, Is.EqualTo(LifeCell.State.Life));
        }

        [Test]
        public void CellTypesInteractInOneAtomicStep()
        {
            var source = new Grid<LifeCell>(3, 1);
            source.SetCell(0, 0, new LifeCell(LifeCell.State.Plant));
            source.SetCell(1, 0, new LifeCell(LifeCell.State.Fire, 100f));

            var next = LifeSimulation.Step(source, HorizontalNeighborhood, 123);

            Assert.That(next.GetCell(0, 0).CurrentState, Is.EqualTo(LifeCell.State.Fire));
            Assert.That(next.GetCell(1, 0).CurrentState, Is.EqualTo(LifeCell.State.Empty));
            Assert.That(next.GetCell(2, 0).Temperature, Is.EqualTo(50f));
        }
    }
}
