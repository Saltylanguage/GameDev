using System.Collections.Generic;
using NUnit.Framework;

namespace SaltyGame.Tests
{
    public sealed class SimulationManagerTests
    {
        [Test]
        public void LaunchRequestCopiesOrderedUpgrades()
        {
            var upgrades = new List<string> { "Trailblazer", "" };
            var request = new SimulationLaunchRequest(
                "profile-1",
                "ForestEdge",
                "hare",
                10100,
                upgrades,
                "ruleset-1");

            upgrades.Add("Warren");

            Assert.That(request.ProfileId, Is.EqualTo("profile-1"));
            Assert.That(request.ScenarioId, Is.EqualTo("ForestEdge"));
            Assert.That(request.PlayerSpeciesId, Is.EqualTo("hare"));
            Assert.That(request.Seed, Is.EqualTo(10100));
            Assert.That(request.RulesetFingerprint, Is.EqualTo("ruleset-1"));
            Assert.That(request.OrderedUpgradeIds, Is.EqualTo(new[] { "Trailblazer" }));
            Assert.That(request.OrderedUpgradeSnapshots, Is.Empty);
        }

        [Test]
        public void LaunchRequestCopiesImmutableUpgradeSnapshots()
        {
            var snapshot = new SpeciesUpgradeSnapshot(
                "launch-test",
                "Launch Test",
                "Test upgrade",
                SpeciesIds.Herbivore,
                0,
                new[]
                {
                    new SpeciesUpgradeModifier(SpeciesAttributeIds.MovementSpeed, 0.5f),
                });
            var request = new SimulationLaunchRequest(
                "profile-1",
                "ForestEdge",
                "hare",
                10100,
                orderedUpgradeSnapshots: new[] { snapshot });

            Assert.That(request.OrderedUpgradeSnapshots, Has.Count.EqualTo(1));
            Assert.That(request.OrderedUpgradeSnapshots[0], Is.SameAs(snapshot));
            Assert.That(request.OrderedUpgradeIds, Is.EqualTo(new[] { "launch-test" }));
        }

        [Test]
        public void LaunchRequestRejectsMismatchedUpgradeIdsAndSnapshots()
        {
            var snapshot = new SpeciesUpgradeSnapshot(
                "launch-test",
                "Launch Test",
                "Test upgrade",
                SpeciesIds.Herbivore,
                0,
                new[]
                {
                    new SpeciesUpgradeModifier(SpeciesAttributeIds.MovementSpeed, 0.5f),
                });

            Assert.Throws<System.ArgumentException>(() => new SimulationLaunchRequest(
                "profile-1",
                "ForestEdge",
                "hare",
                10100,
                orderedUpgradeIds: new[] { "different-id" },
                orderedUpgradeSnapshots: new[] { snapshot }));
        }

        [Test]
        public void AdvanceUsesFixedStepAndRaisesCompletionOnce()
        {
            var manager = CreateManager();
            var completionCount = 0;
            manager.RunCompleted += _ => completionCount++;

            Assert.That(manager.Start(), Is.True);
            manager.Advance(0.09f);
            Assert.That(manager.Run.Tick, Is.EqualTo(0));

            manager.Advance(0.02f);
            Assert.That(manager.Run.Tick, Is.EqualTo(1));

            manager.Advance(0.1f);
            Assert.That(manager.Run.Status, Is.EqualTo(SimulationRunStatus.Complete));
            Assert.That(completionCount, Is.EqualTo(1));

            manager.Advance(1f);
            Assert.That(completionCount, Is.EqualTo(1));
        }

        [Test]
        public void ExplicitTickWindowCompletesAtConfiguredTick()
        {
            var data = new CellularSimData(
                width: 1,
                height: 1,
                startingProbabilities: new Dictionary<SpeciesId, float>(),
                speciesRules: new Dictionary<SpeciesId, SpeciesRules>(),
                runDurationSeconds: 20f,
                stepInterval: 0.1f).WithRunTicks(3, 0.1f);
            var run = new SimulationRunState(
                new Grid<SpeciesCell>(1, 1),
                SpeciesIds.Herbivore,
                seed: 42,
                durationSeconds: data.RunDurationSeconds);
            var manager = new SimulationManager();
            manager.SetRunner(new SpeciesSimulationRunner(run, data));

            Assert.That(data.RunTicks, Is.EqualTo(3));
            Assert.That(data.RunDurationSeconds, Is.EqualTo(0.3f).Within(0.0001f));
            Assert.That(manager.Start(), Is.True);

            manager.Advance(0.1f);
            manager.Advance(0.1f);
            Assert.That(run.Status, Is.EqualTo(SimulationRunStatus.Running));
            manager.Advance(0.1f);

            Assert.That(run.TargetTicks, Is.EqualTo(3));
            Assert.That(run.Tick, Is.EqualTo(3));
            Assert.That(run.Status, Is.EqualTo(SimulationRunStatus.Complete));
        }

        [Test]
        public void PauseStopsAdvancementUntilResume()
        {
            var manager = CreateManager(durationSeconds: 1f);

            Assert.That(manager.Start(), Is.True);
            manager.Advance(0.1f);
            Assert.That(manager.Run.Tick, Is.EqualTo(1));

            Assert.That(manager.Pause(), Is.True);
            manager.Advance(1f);
            Assert.That(manager.Run.Tick, Is.EqualTo(1));
            Assert.That(manager.Run.Status, Is.EqualTo(SimulationRunStatus.Paused));

            Assert.That(manager.Resume(), Is.True);
            manager.Advance(0.1f);
            Assert.That(manager.Run.Tick, Is.EqualTo(2));
        }

        [Test]
        public void ContinuousSkipPreservesWorldHistoryAndMetricsUntilTheSameAbsoluteTick()
        {
            var initialCells = CreateContinuityFixture();
            var rules = SpeciesRuleDefaults.Create();
            var uninterruptedRun = new SimulationRunState(
                initialCells.Copy(),
                SpeciesIds.Herbivore,
                seed: 42,
                durationSeconds: 20f,
                targetTicks: 200);
            var continuedRun = new SimulationRunState(
                initialCells.Copy(),
                SpeciesIds.Herbivore,
                seed: 42,
                durationSeconds: 20f,
                targetTicks: 200);
            continuedRun.ConfigureContinuousPhases(100);

            var uninterruptedManager = new SimulationManager();
            uninterruptedManager.SetRunner(new SpeciesSimulationRunner(uninterruptedRun, rules, 0.1f));
            var continuedManager = new SimulationManager();
            continuedManager.SetRunner(new SpeciesSimulationRunner(continuedRun, rules, 0.1f));
            var boundaryCount = 0;
            var completionCount = 0;
            continuedManager.PhaseBoundaryReached += _ => boundaryCount++;
            continuedManager.RunCompleted += _ => completionCount++;

            Assert.That(uninterruptedManager.Start(), Is.True);
            Assert.That(continuedManager.Start(), Is.True);
            AdvanceTicks(uninterruptedManager, 100);
            AdvanceTicks(continuedManager, 100);

            Assert.That(uninterruptedRun.Status, Is.EqualTo(SimulationRunStatus.Running));
            Assert.That(continuedRun.Status, Is.EqualTo(SimulationRunStatus.AwaitingDecision));
            Assert.That(continuedRun.Tick, Is.EqualTo(100));
            Assert.That(continuedRun.PhaseIndex, Is.EqualTo(1));
            Assert.That(continuedRun.PhaseStartTick, Is.EqualTo(0));
            Assert.That(continuedRun.PhaseEndTick, Is.EqualTo(100));
            Assert.That(boundaryCount, Is.EqualTo(1));
            Assert.That(completionCount, Is.EqualTo(0));
            AssertGridEqual(uninterruptedRun.Cells, continuedRun.Cells);
            AssertPopulationHistoryEqual(uninterruptedRun, continuedRun);

            var cellsAtBoundary = continuedRun.Cells;
            var history = continuedRun.PopulationHistory;
            var metrics = continuedRun.Metrics;
            Assert.That(continuedManager.ContinueWithoutUpgrade(), Is.True);
            Assert.That(continuedRun.Status, Is.EqualTo(SimulationRunStatus.Running));
            Assert.That(continuedRun.Cells, Is.SameAs(cellsAtBoundary));
            Assert.That(continuedRun.PopulationHistory, Is.SameAs(history));
            Assert.That(continuedRun.Metrics, Is.SameAs(metrics));
            Assert.That(continuedRun.PhaseIndex, Is.EqualTo(2));
            Assert.That(continuedRun.PhaseStartTick, Is.EqualTo(100));
            Assert.That(continuedRun.PhaseEndTick, Is.EqualTo(0));

            AdvanceTicks(uninterruptedManager, 100);
            AdvanceTicks(continuedManager, 100);

            Assert.That(uninterruptedRun.Status, Is.EqualTo(SimulationRunStatus.Complete));
            Assert.That(continuedRun.Status, Is.EqualTo(SimulationRunStatus.Complete));
            Assert.That(uninterruptedRun.Tick, Is.EqualTo(200));
            Assert.That(continuedRun.Tick, Is.EqualTo(200));
            Assert.That(continuedRun.PhaseIndex, Is.EqualTo(2));
            Assert.That(continuedRun.PhaseStartTick, Is.EqualTo(100));
            Assert.That(continuedRun.PhaseEndTick, Is.EqualTo(0));
            Assert.That(boundaryCount, Is.EqualTo(1));
            Assert.That(completionCount, Is.EqualTo(1));
            AssertGridEqual(uninterruptedRun.Cells, continuedRun.Cells);
            AssertPopulationHistoryEqual(uninterruptedRun, continuedRun);
            Assert.That(continuedRun.PopulationHistory, Has.Count.EqualTo(201));
            Assert.That(continuedRun.Metrics, Is.SameAs(metrics));

            continuedManager.Advance(10f);
            Assert.That(completionCount, Is.EqualTo(1));
            Assert.That(continuedManager.ContinueWithoutUpgrade(), Is.False);
        }

        [Test]
        public void ContinuousBoundaryDiscardsFrameRemainderBeforeContinuation()
        {
            var run = new SimulationRunState(
                CreateContinuityFixture(),
                SpeciesIds.Herbivore,
                seed: 42,
                durationSeconds: 2f);
            run.ConfigureContinuousPhases(10);
            var manager = new SimulationManager();
            manager.SetRunner(new SpeciesSimulationRunner(run, SpeciesRuleDefaults.Create(), 0.1f));

            Assert.That(manager.Start(), Is.True);
            manager.Advance(1.5f);

            Assert.That(run.Status, Is.EqualTo(SimulationRunStatus.AwaitingDecision));
            Assert.That(run.Tick, Is.EqualTo(10));
            Assert.That(manager.ContinueWithoutUpgrade(), Is.True);
            manager.Advance(0.1f);

            Assert.That(run.Status, Is.EqualTo(SimulationRunStatus.Running));
            Assert.That(run.Tick, Is.EqualTo(11));
        }

        [Test]
        public void BoundaryUpgradeInstallsNewRulesAndContinuesTheSameRun()
        {
            var rules = SpeciesRuleDefaults.Create();
            var data = new CellularSimData(
                width: 2,
                height: 1,
                startingProbabilities: new Dictionary<SpeciesId, float>(),
                speciesRules: rules,
                runDurationSeconds: 0.4f,
                stepInterval: 0.1f).WithRunTicks(4, 0.1f);
            var run = new SimulationRunState(
                CreateContinuityFixture(),
                SpeciesIds.Herbivore,
                seed: 42,
                durationSeconds: data.RunDurationSeconds);
            run.ConfigureContinuousPhases(2);
            var manager = new SimulationManager();
            manager.SetRunner(new SpeciesSimulationRunner(run, data));
            var initialFingerprint = run.RulesetFingerprint;
            var upgrade = new SpeciesUpgradeSnapshot(
                "boundary-movement",
                "Boundary Movement",
                "Live movement test",
                SpeciesIds.Herbivore,
                0,
                new[]
                {
                    new SpeciesUpgradeModifier(SpeciesAttributeIds.MovementSpeed, 0.5f),
                });
            var nextRules = new Dictionary<SpeciesId, SpeciesRules>(rules)
            {
                [SpeciesIds.Herbivore] = upgrade.Apply(rules[SpeciesIds.Herbivore]),
            };

            Assert.That(manager.Start(), Is.True);
            AdvanceTicks(manager, 2);
            Assert.That(run.Status, Is.EqualTo(SimulationRunStatus.AwaitingDecision));
            var cellsAtBoundary = run.Cells;

            Assert.That(
                manager.ContinueWithBoundaryState(nextRules, SpeciesExperimentalOptions.None, new[] { upgrade }),
                Is.True);
            Assert.That(run.Status, Is.EqualTo(SimulationRunStatus.Running));
            Assert.That(run.Tick, Is.EqualTo(2));
            Assert.That(run.Cells, Is.SameAs(cellsAtBoundary));
            Assert.That(run.UpgradeLoadout, Has.Count.EqualTo(1));
            Assert.That(run.UpgradeLoadout[0], Is.SameAs(upgrade));
            Assert.That(run.RulesetFingerprint, Is.Not.EqualTo(initialFingerprint));

            AdvanceTicks(manager, 2);
            Assert.That(run.Tick, Is.EqualTo(4));
            Assert.That(run.Status, Is.EqualTo(SimulationRunStatus.Complete));
        }

        [Test]
        public void ContinuousRestartFromDecisionBoundaryRestoresInitialState()
        {
            var initialCells = CreateContinuityFixture();
            var run = new SimulationRunState(
                initialCells,
                SpeciesIds.Herbivore,
                seed: 42,
                durationSeconds: 2f);
            run.ConfigureContinuousPhases(10);
            var manager = new SimulationManager();
            manager.SetRunner(new SpeciesSimulationRunner(run, SpeciesRuleDefaults.Create(), 0.1f));

            Assert.That(manager.Start(), Is.True);
            AdvanceTicks(manager, 10);
            Assert.That(run.Status, Is.EqualTo(SimulationRunStatus.AwaitingDecision));

            Assert.That(manager.Restart(), Is.True);
            Assert.That(run.Status, Is.EqualTo(SimulationRunStatus.Running));
            Assert.That(run.Tick, Is.EqualTo(0));
            Assert.That(run.ElapsedSeconds, Is.EqualTo(0f));
            Assert.That(run.PhaseIndex, Is.EqualTo(1));
            Assert.That(run.PhaseStartTick, Is.EqualTo(0));
            Assert.That(run.PhaseEndTick, Is.EqualTo(0));
            Assert.That(run.PopulationHistory, Has.Count.EqualTo(1));
            AssertGridEqual(initialCells, run.Cells);
        }

        [Test]
        public void BoardSnapshotCopiesCellsAndSpeciesRoles()
        {
            var grid = new Grid<SpeciesCell>(2, 1, (x, _) => x == 0
                ? SpeciesCell.Grass(3f)
                : new SpeciesCell(SpeciesIds.Herbivore, health: 4, energy: 7));
            var run = new SimulationRunState(
                grid,
                SpeciesIds.Herbivore,
                seed: 17,
                durationSeconds: 1f);

            var snapshot = SimulationBoardSnapshot.Create(
                run,
                SpeciesRuleDefaults.Create(),
                SpeciesIds.Herbivore);

            grid.SetCell(0, 0, SpeciesCell.Empty);

            Assert.That(snapshot.Width, Is.EqualTo(2));
            Assert.That(snapshot.Height, Is.EqualTo(1));
            Assert.That(snapshot.GetCell(0, 0).IsPlantResource, Is.True);
            Assert.That(
                TerrainTileResolver.IsValidMask(snapshot.GetCell(0, 0).TerrainVariantMask),
                Is.True);
            Assert.That(snapshot.GetCell(1, 0).Health, Is.EqualTo(4));
            Assert.That(
                snapshot.SpeciesRoles[SpeciesIds.Carnivore],
                Is.EqualTo(SpeciesRole.Carnivore));
        }

        static SimulationManager CreateManager(float durationSeconds = 0.2f)
        {
            var data = new CellularSimData(
                width: 1,
                height: 1,
                startingProbabilities: new Dictionary<SpeciesId, float>(),
                speciesRules: new Dictionary<SpeciesId, SpeciesRules>(),
                runDurationSeconds: durationSeconds,
                stepInterval: 0.1f);
            var run = new SimulationRunState(
                new Grid<SpeciesCell>(1, 1, (_, __) => SpeciesCell.Empty),
                SpeciesIds.Herbivore,
                seed: 42,
                durationSeconds: durationSeconds);
            var manager = new SimulationManager();
            manager.SetRunner(new SpeciesSimulationRunner(run, data));
            return manager;
        }

        static Grid<SpeciesCell> CreateContinuityFixture()
        {
            return new Grid<SpeciesCell>(2, 1, (x, _) => x == 0
                ? new SpeciesCell(SpeciesIds.Herbivore, health: 10, energy: 24)
                : SpeciesCell.Grass(10f));
        }

        static void AdvanceTicks(SimulationManager manager, int count)
        {
            for (var index = 0; index < count; index++)
            {
                manager.Advance(0.1f);
            }
        }

        static void AssertGridEqual(Grid<SpeciesCell> expected, Grid<SpeciesCell> actual)
        {
            Assert.That(actual.Width, Is.EqualTo(expected.Width));
            Assert.That(actual.Height, Is.EqualTo(expected.Height));
            for (var y = 0; y < expected.Height; y++)
            {
                for (var x = 0; x < expected.Width; x++)
                {
                    var expectedCell = expected.GetCell(x, y);
                    var actualCell = actual.GetCell(x, y);
                    Assert.That(actualCell.IsOccupied, Is.EqualTo(expectedCell.IsOccupied));
                    Assert.That(actualCell.IsCreature, Is.EqualTo(expectedCell.IsCreature));
                    Assert.That(actualCell.IsTerrainResource, Is.EqualTo(expectedCell.IsTerrainResource));
                    Assert.That(actualCell.IsPlantResource, Is.EqualTo(expectedCell.IsPlantResource));
                    Assert.That(actualCell.IsPassable, Is.EqualTo(expectedCell.IsPassable));
                    Assert.That(actualCell.SpeciesId, Is.EqualTo(expectedCell.SpeciesId));
                    Assert.That(actualCell.ResourceSpeciesId, Is.EqualTo(expectedCell.ResourceSpeciesId));
                    Assert.That(actualCell.TerrainId, Is.EqualTo(expectedCell.TerrainId));
                    Assert.That(actualCell.EntityId, Is.EqualTo(expectedCell.EntityId));
                    Assert.That(actualCell.Health, Is.EqualTo(expectedCell.Health));
                    Assert.That(actualCell.Energy, Is.EqualTo(expectedCell.Energy));
                    Assert.That(actualCell.Age, Is.EqualTo(expectedCell.Age));
                    Assert.That(actualCell.FoodEaten, Is.EqualTo(expectedCell.FoodEaten));
                    Assert.That(actualCell.FoodReserve, Is.EqualTo(expectedCell.FoodReserve).Within(0.0001f));
                    Assert.That(actualCell.IsAlpha, Is.EqualTo(expectedCell.IsAlpha));
                    Assert.That(actualCell.TerrainEnergy, Is.EqualTo(expectedCell.TerrainEnergy).Within(0.0001f));
                    Assert.That(actualCell.MovementCost, Is.EqualTo(expectedCell.MovementCost).Within(0.0001f));
                    Assert.That(actualCell.BehaviorState, Is.EqualTo(expectedCell.BehaviorState));
                    Assert.That(actualCell.BehaviorStateTicks, Is.EqualTo(expectedCell.BehaviorStateTicks));
                    Assert.That(
                        actualCell.AttackCooldownTicksRemaining,
                        Is.EqualTo(expectedCell.AttackCooldownTicksRemaining));
                }
            }
        }

        static void AssertPopulationHistoryEqual(SimulationRunState expected, SimulationRunState actual)
        {
            Assert.That(actual.PopulationHistory, Has.Count.EqualTo(expected.PopulationHistory.Count));
            for (var index = 0; index < expected.PopulationHistory.Count; index++)
            {
                var expectedSnapshot = expected.PopulationHistory[index];
                var actualSnapshot = actual.PopulationHistory[index];
                Assert.That(actualSnapshot.Tick, Is.EqualTo(expectedSnapshot.Tick));
                Assert.That(actualSnapshot.Empty, Is.EqualTo(expectedSnapshot.Empty));
                foreach (var entry in expectedSnapshot.Counts)
                {
                    Assert.That(actualSnapshot.GetCount(entry.Key), Is.EqualTo(entry.Value));
                }
            }
        }
    }
}
