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
    }
}
