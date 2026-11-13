using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SaltyGame.Tests
{
    [TestFixture]
    [Category("Simulation")]
    public sealed class HareBurrowTests
    {
        static readonly SpeciesId Hare = new SpeciesId("hare");
        static readonly SpeciesId Fox = new SpeciesId("fox");
        static readonly GridPattern None = new GridPattern(Array.Empty<UnityEngine.Vector2Int>());

        static SpeciesRules Rules(int attackRange = 0, int attack = 0, bool breed = false, int gestationTicks = 0) => new SpeciesRules(
            movementSpeed: 0f, movementPattern: None,
            attackPattern: attackRange == 0 ? None : GridPatternTemplates.CreateMooreRange(attackRange),
            attackAmount: attack, blockPattern: None, blockAmount: 0, dietPattern: None,
            dietTarget: attackRange == 0 ? (SpeciesId?)null : Hare,
            reproductionPattern: breed ? GridPatternTemplates.CreateMooreRange(1) : None,
            reproductionNeighborCount: breed ? 1 : 0, reproductionChance: breed ? 1f : 0f, reproductionFoodRequired: 1,
            metabolism: 0, role: attackRange == 0 ? SpeciesRole.Herbivore : SpeciesRole.Carnivore,
            gestationTicks: gestationTicks);

        static SpeciesUpgradeSnapshot Upgrade(string id) => SpeciesUpgradeCatalog.ResolveFieldDataOrdinaryRewardPool(
            Hare, Array.Empty<SpeciesUpgradeSnapshot>(), candidate => candidate == id).Single(upgrade => upgrade.Id == id);

        static SimulationRunState Start(int phaseLength, out SpeciesCell owner, GenomeSimulationSnapshot genome = null)
        {
            var cells = new Grid<SpeciesCell>(7, 7);
            owner = new SpeciesCell(Hare, energy: 48);
            cells.SetCell(3, 3, owner);
            var run = new SimulationRunState(cells, Hare, seed: 75, durationSeconds: 100f, activeGenomeSnapshot: genome);
            run.ConfigureContinuousPhases(phaseLength);
            return run;
        }

        static void PutFox(Grid<SpeciesCell> cells, int x, int y) => cells.SetCell(x, y, new SpeciesCell(Fox, energy: 100));

        [TestCase(3, 25, false, true)]
        [TestCase(3, 23, false, false)]
        [TestCase(3, 48, true, false)]
        [TestCase(0, 48, false, false)]
        public void DiggingRequiresEnergyAndAllEightNeighborCells(int x, int energy, bool occupiedDiagonal, bool digs)
        {
            var cells = new Grid<SpeciesCell>(7, 7);
            cells.SetCell(x, 3, new SpeciesCell(Hare, energy: energy));
            if (occupiedDiagonal) PutFox(cells, x + 1, 4);
            var run = new SimulationRunState(cells, Hare, seed: 75, durationSeconds: 100f);
            run.ConfigureContinuousPhases(10);
            var rules = new Dictionary<SpeciesId, SpeciesRules> { [Hare] = Rules(), [Fox] = Rules() };
            var runner = new SpeciesSimulationRunner(run, rules, 0.1f,
                upgradeLoadout: new[] { Upgrade(SpeciesUpgradeCatalog.BurrowingId) });

            Assert.That(runner.AdvanceOneTick(), Is.True);
            Assert.That(SimulationBoardSnapshot.Create(run, rules, Hare).HasBurrowAt(x, 3), Is.EqualTo(digs));
            Assert.That(run.Cells.GetCell(x, 3).Energy, Is.EqualTo(digs ? energy - 24 : energy));
        }

        [Test]
        public void HiddenOwnerCountsAsAliveAndEmergesAfterCheckpointWhenThreatLeaves()
        {
            var run = Start(2, out var owner);
            var rules = new Dictionary<SpeciesId, SpeciesRules> { [Hare] = Rules(), [Fox] = Rules(1, attack: 100) };
            var runner = new SpeciesSimulationRunner(run, rules, 0.1f, upgradeLoadout: new[] { Upgrade(SpeciesUpgradeCatalog.BurrowingId) });

            Assert.That(runner.AdvanceOneTick(), Is.True);
            Assert.That(run.Cells.GetCell(3, 3).Energy, Is.EqualTo(24));
            var board = SimulationBoardSnapshot.Create(run, rules, Hare);
            Assert.That(board.HasBurrowAt(3, 3), Is.True);
            Assert.That(board.HasBurrowAt(2, 3), Is.False);
            PutFox(run.Cells, 4, 3);
            Assert.That(runner.AdvanceOneTick(), Is.True);
            Assert.That(run.Cells.GetCell(3, 3).IsCreature, Is.False);
            Assert.That(run.HiddenHareCount, Is.EqualTo(1));
            Assert.That(run.PopulationHistory.Last().GetCount(Hare), Is.EqualTo(1));
            Assert.That(SimulationRunResults.Create(run).PlayerPopulation, Is.EqualTo(1));
            Assert.That(run.Status, Is.EqualTo(SimulationRunStatus.AwaitingDecision));

            var checkpoint = runner.CreateCheckpoint();
            var restored = checkpoint.Restore();
            Assert.That(restored.GetHareBurrowStateFingerprint(), Is.EqualTo(run.GetHareBurrowStateFingerprint()));
            restored.Cells.SetCell(4, 3, SpeciesCell.Empty);
            Assert.That(restored.ContinueWithoutUpgrade(), Is.True);
            var restoredRunner = new SpeciesSimulationRunner(restored, rules, 0.1f, upgradeLoadout: checkpoint.UpgradeLoadout);
            Assert.That(restoredRunner.AdvanceOneTick(), Is.True);
            Assert.That(restored.HiddenHareCount, Is.Zero);
            Assert.That(restored.Cells.GetCell(3, 3).EntityId, Is.EqualTo(owner.EntityId));
            Assert.That(restored.Cells.GetCell(3, 3).Energy, Is.EqualTo(24));
            Assert.That(checkpoint.Restore().HiddenHareCount, Is.EqualTo(1));
            Assert.That(restored.GetHareBurrowStateFingerprint(), Is.Not.EqualTo(run.GetHareBurrowStateFingerprint()));

            var repeat = Start(2, out var repeatOwner);
            Assert.That(repeatOwner.EntityId, Is.Not.EqualTo(owner.EntityId));
            var repeatRunner = new SpeciesSimulationRunner(repeat, rules, 0.1f, upgradeLoadout: checkpoint.UpgradeLoadout);
            Assert.That(repeatRunner.AdvanceOneTick(), Is.True);
            PutFox(repeat.Cells, 4, 3);
            Assert.That(repeatRunner.AdvanceOneTick(), Is.True);
            Assert.That(repeat.GetHareBurrowStateFingerprint(), Is.EqualTo(run.GetHareBurrowStateFingerprint()));
        }

        [Test]
        public void HareBornBeforeBurrowingWasAcquiredCanHideInParentsBurrow()
        {
            var run = Start(1, out var owner);
            var rules = new Dictionary<SpeciesId, SpeciesRules> { [Hare] = Rules(breed: true), [Fox] = Rules(2) };
            run.Cells.SetCell(4, 3, new SpeciesCell(Hare, energy: 48));
            var runner = new SpeciesSimulationRunner(run, rules, 0.1f);
            Assert.That(runner.AdvanceOneTick(), Is.True);
            var child = Enumerable.Range(0, run.Cells.Count).Select(i => run.Cells.GetCell(i % 7, i / 7))
                .First(cell => cell.IsCreature && run.IsHareDescendant(cell.EntityId, owner.EntityId));
            var surfaceOnly = new SimulationRunState(run.Cells.Copy(), Hare, seed: 75, durationSeconds: 100f);
            var familyState = run.GetHareJourneyStateFingerprint();
            Assert.That(run.GetHareBurrowStateFingerprint(), Is.Null);
            Assert.That(surfaceOnly.GetHareJourneyStateFingerprint(), Is.Null);
            Assert.That(familyState, Is.Not.Null);
            Assert.That(runner.CreateCheckpoint().Restore().GetHareJourneyStateFingerprint(), Is.EqualTo(familyState));
            for (var y = 0; y < 7; y++)
                for (var x = 0; x < 7; x++)
                    run.Cells.SetCell(x, y, SpeciesCell.Empty);
            run.Cells.SetCell(3, 3, owner);
            rules[Hare] = Rules();
            Assert.That(runner.InstallBoundaryState(rules, null, new[] { Upgrade(SpeciesUpgradeCatalog.BurrowingId) }), Is.True);
            Assert.That(run.ContinueWithoutUpgrade(), Is.True);
            Assert.That(runner.AdvanceOneTick(), Is.True);

            PutFox(run.Cells, 3, 1);
            run.Cells.SetCell(3, 2, child);
            run.Cells.SetCell(2, 3, new SpeciesCell(Hare, energy: 10));
            Assert.That(run.ContinueWithoutUpgrade(), Is.True);
            Assert.That(runner.AdvanceOneTick(), Is.True);
            Assert.That(run.HiddenHareCount, Is.EqualTo(2));
            Assert.That(run.Cells.GetCell(2, 3).IsCreature, Is.True);
        }

        [Test]
        public void GestatedHareCanUseAbsentParentsBurrowAfterCheckpointRestore()
        {
            var run = Start(1, out var owner);
            var rules = new Dictionary<SpeciesId, SpeciesRules> { [Hare] = Rules(breed: true, gestationTicks: 2), [Fox] = Rules(2) };
            var runner = new SpeciesSimulationRunner(run, rules, 0.1f, upgradeLoadout: new[] { Upgrade(SpeciesUpgradeCatalog.BurrowingId) });
            Assert.That(runner.AdvanceOneTick(), Is.True);
            Assert.That(SimulationBoardSnapshot.Create(run, rules, Hare).HasBurrowAt(3, 3), Is.True);

            var gestatingParent = new SpeciesCell(Hare, energy: 10);
            run.Cells.SetCell(2, 3, gestatingParent);
            Assert.That(run.ContinueWithoutUpgrade(), Is.True);
            Assert.That(runner.AdvanceOneTick(), Is.True);
            Assert.That(run.Cells.GetCell(2, 3).GestationTicksRemaining, Is.EqualTo(2));

            // The mate is gone at delivery, but its burrow is still active.
            run.Cells.SetCell(3, 3, SpeciesCell.Empty);
            for (var tick = 0; tick < 2; tick++)
            {
                Assert.That(run.ContinueWithoutUpgrade(), Is.True);
                Assert.That(runner.AdvanceOneTick(), Is.True);
            }

            var checkpoint = runner.CreateCheckpoint();
            var restored = checkpoint.Restore();
            var childIndex = Enumerable.Range(0, restored.Cells.Count).Single(index =>
            {
                var cell = restored.Cells.GetCell(index % restored.Cells.Width, index / restored.Cells.Width);
                return cell.IsCreature && cell.EntityId != gestatingParent.EntityId;
            });
            var childX = childIndex % restored.Cells.Width;
            var childY = childIndex / restored.Cells.Width;
            var child = restored.Cells.GetCell(childX, childY);
            Assert.That(restored.IsHareDescendant(child.EntityId, gestatingParent.EntityId), Is.True);
            Assert.That(restored.IsHareDescendant(child.EntityId, owner.EntityId), Is.True);

            restored.Cells.SetCell(childX, childY, SpeciesCell.Empty);
            restored.Cells.SetCell(2, 3, SpeciesCell.Empty);
            restored.Cells.SetCell(3, 2, child);
            var unrelated = new SpeciesCell(Hare, energy: 10);
            restored.Cells.SetCell(4, 2, unrelated);
            PutFox(restored.Cells, 3, 0);
            Assert.That(restored.ContinueWithoutUpgrade(), Is.True);
            var restoredRunner = new SpeciesSimulationRunner(restored, rules, 0.1f, upgradeLoadout: checkpoint.UpgradeLoadout);
            Assert.That(restoredRunner.AdvanceOneTick(), Is.True);
            Assert.That(restored.HiddenHareCount, Is.EqualTo(1));
            Assert.That(restored.Cells.GetCell(3, 2).IsCreature, Is.False);
            Assert.That(restored.Cells.GetCell(4, 2).EntityId, Is.EqualTo(unrelated.EntityId));
        }

        [Test]
        public void ExpiredBurrowKeepsOwnerProtectedUntilAnExitFrees()
        {
            var run = Start(300, out var owner);
            var rules = new Dictionary<SpeciesId, SpeciesRules> { [Hare] = Rules(), [Fox] = Rules(1) };
            var runner = new SpeciesSimulationRunner(run, rules, 0.1f, upgradeLoadout: new[] { Upgrade(SpeciesUpgradeCatalog.BurrowingId) });
            Assert.That(runner.AdvanceOneTick(), Is.True);
            PutFox(run.Cells, 4, 3);
            Assert.That(runner.AdvanceOneTick(), Is.True);
            Assert.That(run.HiddenHareCount, Is.EqualTo(1));
            for (var y = 2; y <= 4; y++)
                for (var x = 2; x <= 4; x++)
                    PutFox(run.Cells, x, y);
            while (run.Tick < 201) Assert.That(runner.AdvanceOneTick(), Is.True);
            Assert.That(run.HiddenHareCount, Is.EqualTo(1));
            run.Cells.SetCell(2, 2, SpeciesCell.Empty);
            Assert.That(runner.AdvanceOneTick(), Is.True);
            Assert.That(run.HiddenHareCount, Is.Zero);
            Assert.That(run.Cells.GetCell(2, 2).EntityId, Is.EqualTo(owner.EntityId));

            var emerged = run.Cells.GetCell(2, 2);
            for (var y = 2; y <= 4; y++)
                for (var x = 2; x <= 4; x++)
                    run.Cells.SetCell(x, y, SpeciesCell.Empty);
            run.Cells.SetCell(3, 3, emerged);
            Assert.That(runner.AdvanceOneTick(), Is.True);
            Assert.That(run.Cells.GetCell(3, 3).Energy, Is.EqualTo(24));
        }

        [TestCase(false, 0, 1)]
        [TestCase(true, 0, 3)]
        [TestCase(true, 2, 5)]
        public void SharedBurrowsAndExpansionControlWhoFits(bool shared, int expansion, int expected)
        {
            var run = Start(20, out _);
            var rules = new Dictionary<SpeciesId, SpeciesRules> { [Hare] = Rules(), [Fox] = Rules(2) };
            var upgrades = new List<SpeciesUpgradeSnapshot> { Upgrade(SpeciesUpgradeCatalog.BurrowingId) };
            if (shared) upgrades.Add(Upgrade(SpeciesUpgradeCatalog.SharedBurrowsId));
            for (var rank = 0; rank < expansion; rank++) upgrades.Add(Upgrade(SpeciesUpgradeCatalog.ExpandedBurrowsId));
            var runner = new SpeciesSimulationRunner(run, rules, 0.1f, upgradeLoadout: upgrades);
            Assert.That(runner.AdvanceOneTick(), Is.True);
            PutFox(run.Cells, 3, 1);
            foreach (var tile in new[] { (2, 2), (3, 2), (4, 2), (2, 3) })
                run.Cells.SetCell(tile.Item1, tile.Item2, new SpeciesCell(Hare, energy: 10));
            Assert.That(runner.AdvanceOneTick(), Is.True);
            Assert.That(run.HiddenHareCount, Is.EqualTo(expected));
            Assert.That(run.PopulationHistory.Last().GetCount(Hare), Is.EqualTo(5));
        }

        [Test]
        public void BornBurrowerGrantsDiggingWhileResearchAloneDoesNot()
        {
            var rules = new Dictionary<SpeciesId, SpeciesRules> { [Hare] = Rules() };
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            var research = new[]
            {
                FieldDataPermanentProgressionDefinition.BurrowingNodeId,
                FieldDataPermanentProgressionDefinition.SharedBurrowsNodeId,
                FieldDataPermanentProgressionDefinition.ExpandedBurrowsNodeId,
            };
            var starter = FieldDataPermanentProgressionDefinition.BornBurrowerNodeId;
            var owned = research.Append(starter).ToArray();
            var researched = definition.FreezeOwnedLoadout(new SpeciesGenomeProfile(Hare, owned, research));
            var equipped = definition.FreezeOwnedLoadout(new SpeciesGenomeProfile(Hare, owned, new[] { starter }));
            var researchRules = researched.ApplyToNaturalRules(rules);
            var starterRules = equipped.ApplyToNaturalRules(rules);
            Assert.That(researchRules[Hare], Is.SameAs(rules[Hare]));
            Assert.That(starterRules[Hare], Is.SameAs(rules[Hare]));
            var researchOnlyRun = Start(10, out _, researched);
            var starterRun = Start(10, out _, equipped);
            Assert.That(new SpeciesSimulationRunner(researchOnlyRun, researchRules, 0.1f).AdvanceOneTick(), Is.True);
            Assert.That(new SpeciesSimulationRunner(starterRun, starterRules, 0.1f).AdvanceOneTick(), Is.True);
            Assert.That(researchOnlyRun.Cells.GetCell(3, 3).Energy, Is.EqualTo(48));
            Assert.That(starterRun.Cells.GetCell(3, 3).Energy, Is.EqualTo(24));
        }
    }
}
