using System.Collections.Generic;
using NUnit.Framework;

namespace SaltyGame.Tests
{
    public sealed class PopulationInterventionTests
    {
        static SpeciesSimulationRunner Boundary(int hares = 3, int plants = 6, int foxes = 2, int max = 0,
            SpeciesId? player = null)
        {
            var cells = new Grid<SpeciesCell>(12, 8);
            for (var i = 0; i < hares; i++) cells.SetCell(i % 12, i / 12, new SpeciesCell(SpeciesIds.Herbivore, energy: 20));
            for (var i = 0; i < plants; i++) cells.SetCell(i % 12, 5 + i / 12, SpeciesCell.Grass(10));
            for (var i = 0; i < foxes; i++) cells.SetCell(i, 7, new SpeciesCell(SpeciesIds.Carnivore, energy: 20));
            var run = new SimulationRunState(cells, player ?? SpeciesIds.Herbivore, 123, 10f);
            run.ConfigureContinuousPhases(1);
            run.Start();
            run.Advance(cells.Copy(), 0.1f);
            return new SpeciesSimulationRunner(run, SpeciesRuleDefaults.Create(), 0.1f, max);
        }

        [TestCase("herbivore", 1)]
        [TestCase("herbivore", -1)]
        [TestCase("carnivore", 1)]
        [TestCase("carnivore", -1)]
        [TestCase("plant", 5)]
        [TestCase("plant", -5)]
        public void SixControlsChangeOnlyTheChosenPopulationAndDoNotAdvanceTime(string id, int amount)
        {
            var runner = Boundary();
            var run = runner.Run;
            var species = new SpeciesId(id);
            var before = SpeciesPopulationSnapshot.Create(run.Cells, run.Tick);
            Assert.That(runner.CanChangeBoundaryPopulation(species, amount, out var planned, out _), Is.True);
            Assert.That(runner.TryChangeBoundaryPopulation(species, amount, "test", out var actual), Is.True);
            Assert.That(actual, Is.EqualTo(planned).And.EqualTo(amount));
            var after = SpeciesPopulationSnapshot.Create(run.Cells, run.Tick);
            foreach (var entry in before.Counts)
                Assert.That(after.GetCount(entry.Key), Is.EqualTo(entry.Value + (entry.Key == species ? amount : 0)));
            Assert.That(run.Tick, Is.EqualTo(1));
            Assert.That(run.Status, Is.EqualTo(SimulationRunStatus.AwaitingDecision));
            Assert.That(run.Metrics.GetActivity(species).Births, Is.Zero);
            Assert.That(run.Metrics.GetActivity(species).Deaths, Is.Zero);
            Assert.That(run.PopulationInterventions, Has.Count.EqualTo(1));
        }

        [Test]
        public void RefillClampsToPoolAndRejectsAtCapacityWhileNpcGrantsIgnorePool()
        {
            var runner = Boundary(hares: 48);
            Assert.That(runner.TryChangeBoundaryPopulation(SpeciesIds.Herbivore, 10, "refresh", out var actual), Is.True);
            Assert.That(actual, Is.EqualTo(2));
            Assert.That(runner.TryChangeBoundaryPopulation(SpeciesIds.Herbivore, 1, "refresh", out actual), Is.False);
            Assert.That(actual, Is.Zero);
            Assert.That(runner.TryChangeBoundaryPopulation(SpeciesIds.Carnivore, 1, "event", out actual), Is.True);
            Assert.That(actual, Is.EqualTo(1));
        }

        [Test]
        public void RejectedChangesAreAtomicAndOnlyThePlayerSpeciesHasARemovalFloor()
        {
            var runner = Boundary(hares: 1, foxes: 1);
            var before = runner.Run.Cells;
            Assert.That(runner.TryChangeBoundaryPopulation(SpeciesIds.Herbivore, -1, "test", out _), Is.False);
            Assert.That(runner.TryChangeBoundaryPopulation(SpeciesIds.Plant, -7, "test", out _), Is.False);
            Assert.That(runner.TryChangeBoundaryPopulation(SpeciesIds.Plant, 100, "test", out _), Is.False);
            Assert.That(runner.Run.Cells, Is.SameAs(before));
            Assert.That(runner.Run.PopulationInterventions, Is.Empty);
            Assert.That(runner.TryChangeBoundaryPopulation(SpeciesIds.Carnivore, -1, "test", out _), Is.True);
            runner = Boundary(hares: 1, foxes: 1, player: SpeciesIds.Carnivore);
            Assert.That(runner.TryChangeBoundaryPopulation(SpeciesIds.Carnivore, -1, "test", out _), Is.False);
            Assert.That(runner.TryChangeBoundaryPopulation(SpeciesIds.Herbivore, -1, "test", out _), Is.True);
        }

        [Test]
        public void GlobalLimitAndNonBoundaryStatesRejectWithoutChanges()
        {
            var runner = Boundary(max: 11);
            Assert.That(runner.TryChangeBoundaryPopulation(SpeciesIds.Plant, 1, "test", out _), Is.False);
            Assert.That(runner.Run.ContinueWithoutUpgrade(), Is.True);
            Assert.That(runner.TryChangeBoundaryPopulation(SpeciesIds.Plant, -1, "test", out _), Is.False);
            runner.Pause();
            Assert.That(runner.TryChangeBoundaryPopulation(SpeciesIds.Plant, 1, "test", out _), Is.False);
        }

        [Test]
        public void RemovalPreservesTheOtherLayerAndDoesNotCreateDeathEvents()
        {
            var runner = Boundary(hares: 2, plants: 0, foxes: 0);
            var run = runner.Run;
            var creature = run.Cells.GetCell(0, 0);
            run.Cells.SetCell(0, 0, SpeciesCell.Grass(8).WithEntity(creature.SpeciesId, 1, 20, 0, 0, 0f,
                entityId: creature.EntityId));
            Assert.That(runner.TryChangeBoundaryPopulation(SpeciesIds.Plant, -1, "test", out _), Is.True);
            Assert.That(run.Cells.GetCell(0, 0).EntityId, Is.EqualTo(creature.EntityId));
            run.Cells.SetCell(1, 0, SpeciesCell.Grass(8).WithEntity(SpeciesIds.Carnivore, 1, 20, 0, 0, 0f));
            Assert.That(runner.TryChangeBoundaryPopulation(SpeciesIds.Carnivore, -1, "test", out _), Is.True);
            Assert.That(run.Cells.GetCell(1, 0).IsPlantResource, Is.True);
            Assert.That(run.Metrics.DeathEvents, Is.Empty);
        }

        [Test]
        public void CheckpointRetainsOrderedActionsAndReplaysTheSameNextPlacement()
        {
            var runner = Boundary();
            Assert.That(runner.TryChangeBoundaryPopulation(SpeciesIds.Plant, -5, "test", out _), Is.True);
            var restored = new SpeciesSimulationRunner(runner.CreateCheckpoint().Restore(), SpeciesRuleDefaults.Create(), 0.1f);
            Assert.That(restored.Run.PopulationInterventions, Has.Count.EqualTo(1));
            Assert.That(restored.Run.GetBoundaryPopulationRemoved(SpeciesIds.Plant), Is.EqualTo(5));
            Assert.That(runner.TryChangeBoundaryPopulation(SpeciesIds.Plant, 5, "test", out _), Is.True);
            Assert.That(restored.TryChangeBoundaryPopulation(SpeciesIds.Plant, 5, "test", out _), Is.True);
            for (var y = 0; y < 8; y++)
            for (var x = 0; x < 12; x++)
                Assert.That(restored.Run.Cells.GetCell(x, y).IsPlantResource,
                    Is.EqualTo(runner.Run.Cells.GetCell(x, y).IsPlantResource));
        }

        [Test]
        public void RemovalsReconcileWithoutCountingAsMortalityAndRestartDiscardsLiveInterventions()
        {
            var runner = Boundary();
            runner.TryChangeBoundaryPopulation(SpeciesIds.Herbivore, -1, "test", out _);
            runner.TryChangeBoundaryPopulation(SpeciesIds.Herbivore, 2, "test", out _);
            var run = runner.Run;
            var line = run.Metrics.CreateHerbivoreStatLine(SpeciesIds.Herbivore, 3, 4,
                run.GetBoundaryPopulationAdded(SpeciesIds.Herbivore), run.GetBoundaryPopulationRemoved(SpeciesIds.Herbivore));
            Assert.That(line.PopulationReconciled, Is.True);
            Assert.That(line.Starved + line.Preyed + line.Crowding, Is.Zero);
            runner.Restart();
            Assert.That(run.PopulationInterventions, Is.Empty);
            Assert.That(run.GetBoundaryPopulationAdded(SpeciesIds.Herbivore), Is.Zero);
            Assert.That(run.PopulationHistory[0].Herbivores, Is.EqualTo(3));
        }

        [Test]
        public void ZeroFoodPlantConfigurationRejectsARefillInsteadOfRecordingPhantomPlants()
        {
            var source = Boundary();
            var empty = new GridPattern(new UnityEngine.Vector2Int[0]);
            var rules = new Dictionary<SpeciesId, SpeciesRules>(SpeciesRuleDefaults.Create())
            {
                [SpeciesIds.Plant] = new SpeciesRules(0f, empty, empty, 0, empty, 0, empty, null, empty, 0,
                    startingFoodReserve: 0f, role: SpeciesRole.Plant),
            };
            var runner = new SpeciesSimulationRunner(source.Run, rules, 0.1f);
            Assert.That(runner.CanChangeBoundaryPopulation(SpeciesIds.Plant, 5, out _, out var reason), Is.False);
            Assert.That(reason, Does.Contain("no usable food"));
            Assert.That(runner.TryChangeBoundaryPopulation(SpeciesIds.Plant, 5, "test", out _), Is.False);
            Assert.That(runner.Run.PopulationInterventions, Is.Empty);
            Assert.That(SpeciesPopulationSnapshot.Create(runner.Run.Cells, runner.Run.Tick).Plants, Is.EqualTo(6));
        }

        [Test]
        public void FoodFeedbackShowsObservedDeclineAndExhaustionWithoutPreventingIt()
        {
            var runner = Boundary();
            runner.TryChangeBoundaryPopulation(SpeciesIds.Plant, -5, "test", out _);
            var food = new FoodPressureSnapshot(runner.Run, SpeciesIds.Plant);
            Assert.That(food.Status, Is.EqualTo("FOOD DECLINING"));
            Assert.That(food.Change, Is.EqualTo(-5));
            runner.TryChangeBoundaryPopulation(SpeciesIds.Plant, -1, "test", out _);
            Assert.That(new FoodPressureSnapshot(runner.Run, SpeciesIds.Plant).Status, Is.EqualTo("FOOD EXHAUSTED"));
        }
    }
}
