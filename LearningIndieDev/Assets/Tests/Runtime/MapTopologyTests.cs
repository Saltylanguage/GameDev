using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace SaltyGame.Tests
{
    [TestFixture, Category("Simulation")]
    public sealed class MapTopologyTests
    {
        static readonly GridPattern Cardinal = new GridPattern(new[] { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down });

        // Geometry is a test fixture; author-selectable generation belongs to tickets 2 and 3.
        public static MapTopology Layout(string shape)
        {
            var cells = new List<Vector2Int>();
            for (var y = 0; y < 7; y++)
                for (var x = 0; x < 9; x++)
                {
                    var dx = x - 4;
                    var dy = y - 3;
                    var inside = shape == "square" ? Math.Abs(dx) <= 3
                        : shape == "circle" ? dx * dx + dy * dy <= 9
                        : shape == "oval" ? dx * dx * 9 + dy * dy * 16 <= 144
                        : shape == "diamond" ? Math.Abs(dx) + Math.Abs(dy) <= 3
                        : shape == "cross" ? Math.Abs(dx) <= 1 || Math.Abs(dy) <= 1
                        : shape == "irregular" ? !(x == 4 && y == 3) && !(x < 2 && y < 2)
                        : true;
                    if (inside) cells.Add(new Vector2Int(x, y));
                }
            var edges = new List<MapBoundaryConnection>();
            foreach (var row in cells.GroupBy(cell => cell.y))
                edges.Add(new MapBoundaryConnection(row.OrderBy(cell => cell.x).First(), Vector2Int.left, row.OrderBy(cell => cell.x).Last()));
            foreach (var column in cells.GroupBy(cell => cell.x))
                edges.Add(new MapBoundaryConnection(column.OrderBy(cell => cell.y).First(), Vector2Int.down, column.OrderBy(cell => cell.y).Last()));
            return new MapTopology(9, 7, cells, edges, shape + ":seed=17;size=9x7;roughness=0;holes=" + (shape == "irregular" ? 1 : 0));
        }

        [TestCase("rectangle"), TestCase("square"), TestCase("circle"), TestCase("oval"), TestCase("diamond"), TestCase("cross"), TestCase("irregular")]
        public void OpposingBoundaryConnectionsAreReciprocalAndOnlyReachPlayableCells(string shape)
        {
            var topology = Layout(shape);
            foreach (var edge in topology.CopyConnections())
            {
                Assert.That(topology.TryResolve(edge.source.x, edge.source.y, edge.direction, out var target), Is.True);
                Assert.That(target, Is.EqualTo(edge.target));
                Assert.That(topology.IsPlayableCell(target.x, target.y), Is.True);
                Assert.That(topology.TryResolve(target.x, target.y, -edge.direction, out var back), Is.True);
                Assert.That(back, Is.EqualTo(edge.source));
            }
            Assert.That(Layout(shape).Identity, Is.EqualTo(topology.Fingerprint));
        }

        [Test]
        public void AuthoredCellLayoutAlwaysReceivesBothAxisConnectionsWhileInteriorGapsStayAbsent()
        {
            var fixture = Layout("irregular");
            var map = new MapTopology(9, 7, fixture.CopyCells(), generationInputs: "irregular;seed=17");
            Assert.That(map.TryResolve(0, 3, Vector2Int.left, out var left), Is.True);
            Assert.That(left, Is.EqualTo(new Vector2Int(8, 3)));
            Assert.That(map.TryResolve(3, 0, Vector2Int.down, out var down), Is.True);
            Assert.That(down, Is.EqualTo(new Vector2Int(3, 6)));
            Assert.That(map.TryResolve(3, 3, Vector2Int.right, out _), Is.False);
        }

        [Test]
        public void GapsCannotBeReadWrittenSpawnedIntoOrCrossedByLongOffsets()
        {
            var topology = Layout("irregular");
            var grid = new Grid<int>(9, 7, topology: topology);
            Assert.That(grid.TryGetCell(4, 3, out _), Is.False);
            Assert.That(grid.TrySetCell(4, 3, 99), Is.False);
            Assert.Throws<ArgumentOutOfRangeException>(() => grid.SetCell(4, 3, 99));
            Assert.That(topology.TryResolve(3, 3, Vector2Int.right, out _), Is.False);
            Assert.That(topology.TryResolve(3, 3, new Vector2Int(2, 0), out _), Is.False);
            Assert.That(grid.Copy().Topology.Fingerprint, Is.EqualTo(topology.Fingerprint));
            var step = GridSimulation.Step(grid, (source, x, y) => 1);
            Assert.That(step.TryGetCell(4, 3, out _), Is.False);
            Assert.That(step.GetCell(3, 3), Is.EqualTo(1));
        }

        [Test]
        public void InvalidBoundaryPairingsAndConflictingEdgesAreRejected()
        {
            var cells = new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) };
            Assert.Throws<ArgumentException>(() => new MapTopology(3, 1, cells,
                new[] { new MapBoundaryConnection(cells[0], Vector2Int.right, cells[2]) }));
            Assert.Throws<ArgumentException>(() => new MapTopology(3, 1, cells,
                new[] { new MapBoundaryConnection(cells[0], Vector2Int.left, new Vector2Int(3, 0)) }));
        }

        [TestCase(false), TestCase(true)]
        public void GestationBirthsUseWrappedBoundaryCells(bool shaped)
        {
            var none = new GridPattern(Array.Empty<Vector2Int>());
            var left = new GridPattern(new[] { Vector2Int.left });
            var rules = new SpeciesRules(0f, none, none, 0, none, 0, none, null, left,
                0, 1f, reproductionFoodRequired: 1, metabolism: 0, gestationTicks: 1);
            var cells = shaped ? new Grid<SpeciesCell>(9, 7, topology: Layout("diamond")) : new Grid<SpeciesCell>(9, 7, true);
            var x = shaped ? 2 : 0;
            var target = shaped ? 6 : 8;
            cells.SetCell(x, 2, new SpeciesCell(SpeciesIds.Carnivore, energy: 10));
            var definitions = new Dictionary<SpeciesId, SpeciesRules> { [SpeciesIds.Carnivore] = rules };
            cells = SpeciesSimulation.Step(cells, definitions, 11);
            Assert.That(cells.GetCell(x, 2).PendingLitterSize, Is.EqualTo(1));
            cells = SpeciesSimulation.Step(cells, definitions, 11);
            Assert.That(cells.GetCell(target, 2).IsCreature, Is.True);
            Assert.That(cells.GetCell(x, 2).PendingLitterSize, Is.Zero);
        }

        public static CellularSimData Data(MapTopology topology)
        {
            var none = new GridPattern(Array.Empty<Vector2Int>());
            var rule = new SpeciesRules(1f, Cardinal, none, 0, none, 0, none, null, none, 0, 0f,
                startingEnergy: 50, metabolism: 0);
            return new CellularSimData(9, 7, new Dictionary<SpeciesId, float>(),
                new Dictionary<SpeciesId, SpeciesRules> { [SpeciesIds.Herbivore] = rule }, 10f, 0.1f,
                startingPopulations: new Dictionary<SpeciesId, int> { [SpeciesIds.Herbivore] = 4 }, topology: topology);
        }

        [Test]
        public void PlacementCopiesCheckpointsAndRepeatedRunsRetainTopologyAndNeverFillGaps()
        {
            var topology = Layout("irregular");
            var data = Data(topology);
            Assert.That(data.WithWrapEdges(false).WrapEdges, Is.True);
            Assert.That(data.Copy().WithRunTicks(20, 0.1f).Topology.Fingerprint, Is.EqualTo(topology.Fingerprint));
            Assert.Throws<NotSupportedException>(() => data.WithGridSize(10, 7));
            var changedInputs = new MapTopology(9, 7, topology.CopyCells(), topology.CopyConnections(), "seed=18");
            Assert.That(data.WithTopology(changedInputs).Fingerprint, Is.Not.EqualTo(data.Fingerprint));
            var first = SpeciesInitialGridFactory.Create(data, 17);
            var second = SpeciesInitialGridFactory.Create(data, 17);
            Assert.That(first.TryGetCell(4, 3, out _), Is.False);
            var run = new SimulationRunState(first, SpeciesIds.Herbivore, 17, data.RunDurationSeconds);
            run.ConfigureContinuousPhases(3);
            var runner = new SpeciesSimulationRunner(run, data);
            runner.Start();
            for (var tick = 0; tick < 3; tick++) runner.AdvanceOneTick();
            var restored = SpeciesSimulationRunner.RestoreCheckpoint(runner.CreateCheckpoint(), data);
            Assert.That(restored.Run.Cells.Topology.Fingerprint, Is.EqualTo(topology.Fingerprint));
            Assert.That(restored.Run.Cells.TryGetCell(4, 3, out _), Is.False);
            var secondRun = new SimulationRunState(second, SpeciesIds.Herbivore, 17, data.RunDurationSeconds);
            secondRun.ConfigureContinuousPhases(3);
            var secondRunner = new SpeciesSimulationRunner(secondRun, data);
            secondRunner.Start();
            for (var tick = 0; tick < 3; tick++) secondRunner.AdvanceOneTick();
            for (var y = 0; y < 7; y++)
                for (var x = 0; x < 9; x++)
                {
                    Assert.That(secondRun.Cells.TryGetCell(x, y, out var secondCell), Is.EqualTo(run.Cells.TryGetCell(x, y, out var firstCell)));
                    Assert.That(secondCell.SpeciesId, Is.EqualTo(firstCell.SpeciesId));
                    Assert.That(secondCell.Energy, Is.EqualTo(firstCell.Energy));
                    Assert.That(secondCell.Age, Is.EqualTo(firstCell.Age));
                }
            Assert.Throws<ArgumentException>(() => SpeciesSimulationRunner.RestoreCheckpoint(runner.CreateCheckpoint(), data.WithTopology(changedInputs)));
            var snapshot = SimulationBoardSnapshot.Create(run, data.SpeciesRules, SpeciesIds.Herbivore);
            Assert.That(snapshot.TryGetCell(4, 3, out _), Is.False);
            Assert.That(snapshot.TryGetCell(3, 3, out _), Is.True);
        }

        [Test]
        public void ShapeSeamsDriveMovementPerceptionCombatAndNavigation()
        {
            var topology = Layout("diamond");
            var origin = new Vector2Int(2, 2);
            var target = new Vector2Int(6, 2);
            var left = new GridPattern(new[] { Vector2Int.left });
            var empty = new GridPattern(Array.Empty<Vector2Int>());
            var herbivore = new SpeciesRules(1f, left, left, 0, empty, 0, left, null, empty, 0, 0f, 0);
            var grid = new Grid<SpeciesCell>(9, 7, topology: topology);
            grid.SetCell(origin.x, origin.y, new SpeciesCell(SpeciesIds.Herbivore, energy: 10));
            var moved = SpeciesSimulation.Step(grid, new Dictionary<SpeciesId, SpeciesRules> { [SpeciesIds.Herbivore] = herbivore }, 42);
            Assert.That(moved.GetCell(target.x, target.y).IsCreature, Is.True);
            Assert.That(grid.GetDistance(origin.x, origin.y, target.x, target.y), Is.EqualTo(1));
            var foodRules = new SpeciesRules(1f, left, left, 10, empty, 0, left, SpeciesIds.Herbivore,
                empty, 0, 0f, 0, awareness: new SpeciesAwarenessRules(2), role: SpeciesRole.Carnivore);
            grid.SetCell(origin.x, origin.y, new SpeciesCell(SpeciesIds.Carnivore, energy: 10));
            grid.SetCell(target.x, target.y, new SpeciesCell(SpeciesIds.Herbivore, energy: 10));
            Assert.That(SpeciesPerception.TryFindFoodTarget(grid, origin.x, origin.y, foodRules, new System.Random(42), out var perceived), Is.True);
            Assert.That(perceived.Location, Is.EqualTo(target));
            var rules = new Dictionary<SpeciesId, SpeciesRules> { [SpeciesIds.Carnivore] = foodRules, [SpeciesIds.Herbivore] = herbivore };
            var result = SpeciesSimulation.Step(grid, rules, 42, combatResolutionMode: SpeciesCombatResolutionMode.LegacyFixedDamage);
            Assert.That(result.GetCell(target.x, target.y).SpeciesId, Is.Not.EqualTo(SpeciesIds.Herbivore));
            grid.SetCell(target.x, target.y, SpeciesCell.Empty);
            Assert.That(SpeciesNavigation.TryFindNextStep(grid, origin, new Vector2Int(5, 2), Cardinal,
                left, new System.Random(42), out var next), Is.True);
            Assert.That(next, Is.EqualTo(target));
        }
    }
}
