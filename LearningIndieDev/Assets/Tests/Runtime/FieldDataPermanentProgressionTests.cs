using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SaltyGame.Tests
{
    [TestFixture]
    [Category("Simulation")]
    public sealed class FieldDataPermanentProgressionTests
    {
        [TestCase(0, 10)]
        [TestCase(1, 15)]
        [TestCase(2, 20)]
        [TestCase(3, 25)]
        [TestCase(4, 30)]
        [TestCase(5, 35)]
        public void PopulationReservesRanksSetThePerNodeHarePurchaseAllowance(int rank, int allowance)
        {
            Assert.That(FieldDataPermanentProgressionDefinition.HarePurchaseAllowance(rank), Is.EqualTo(allowance));
            if (rank > 0)
                Assert.That(FieldDataPermanentProgressionDefinition.PopulationReservesResearchCost(rank), Is.EqualTo(rank * 5));
        }

        [Test]
        public void PopulationReservesRejectsRanksOutsideTheApprovedRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => FieldDataPermanentProgressionDefinition.HarePurchaseAllowance(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => FieldDataPermanentProgressionDefinition.HarePurchaseAllowance(6));
            Assert.Throws<ArgumentOutOfRangeException>(() => FieldDataPermanentProgressionDefinition.PopulationReservesResearchCost(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => FieldDataPermanentProgressionDefinition.PopulationReservesResearchCost(6));
        }

        [Test]
        public void MutationResearchUsesOwnershipNotEquipmentAndFreezesTheProfile()
        {
            var hare = new SpeciesId("hare");
            var empty = FieldDataPermanentProgressionDefinition.CaptureMutationResearch(null, hare);
            Assert.That(empty(SpeciesUpgradeCatalog.ToughHideId), Is.False);
            Assert.That(empty(SpeciesUpgradeCatalog.KeenSensesId), Is.False);
            var genome = new SpeciesGenomeProfile(hare,
                new[] { "hare.guarded-burrow", "hare.keen-hearing" }, Array.Empty<string>());
            var profile = new ProfileSessionSnapshot(true, "research", "Research", new[] { genome });
            var eligibility = FieldDataPermanentProgressionDefinition.CaptureMutationResearch(profile, hare);
            Assert.That(eligibility(SpeciesUpgradeCatalog.ToughHideId), Is.True);
            Assert.That(eligibility(SpeciesUpgradeCatalog.KeenSensesId), Is.True);
            Assert.That(eligibility(SpeciesUpgradeCatalog.LongVisionId), Is.True);
            Assert.That(eligibility(SpeciesUpgradeCatalog.EfficientDigestionId), Is.False);
            Assert.That(eligibility(SpeciesUpgradeCatalog.SeedDispersalId), Is.False);
            var progression = new SpeciesProgression(new SpeciesDefinition(hare, SpeciesRuleDefaults.Create()[SpeciesIds.Herbivore]), eligibility);
            Assert.That(progression.GetUpgradeLevel(SpeciesUpgradeCatalog.ToughHideId), Is.Zero);
            Assert.That(progression.TryApplyFieldDataReward(SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.SeedDispersalId).CreateSnapshot(hare)), Is.False);
            Assert.That(progression.TryApplyFieldDataReward(SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.ToughHideId).CreateSnapshot(hare)), Is.True);
            var beforeVision = progression.CurrentRules.Awareness.VisionRange;
            Assert.That(progression.TryApplyFieldDataReward(SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.LongVisionId).CreateSnapshot(hare)), Is.True);
            Assert.That(progression.CurrentRules.Awareness.VisionRange, Is.EqualTo(beforeVision + SpeciesUpgradeCatalog.LongVisionTiles));
            Assert.That(progression.TryApplyFieldDataReward(SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.LongVisionId).CreateSnapshot(hare)), Is.False);
            Assert.That(empty(SpeciesUpgradeCatalog.ToughHideId), Is.False, "Another profile's research must not change the frozen empty policy.");
        }

        [Test]
        public void HeightenedSensesResearchMakesBothMutationsAvailableAtPhaseAndFieldDataRewards()
        {
            var hare = new SpeciesId("hare");
            var profile = new ProfileSessionSnapshot(true, "senses", "Senses", new[]
            {
                new SpeciesGenomeProfile(hare, new[] { "hare.keen-hearing" }, Array.Empty<string>())
            });
            var eligible = FieldDataPermanentProgressionDefinition.CaptureMutationResearch(profile, hare);
            var ids = new[] { SpeciesUpgradeCatalog.LongVisionId, SpeciesUpgradeCatalog.KeenSensesId };
            var fieldData = SpeciesUpgradeCatalog.ResolveFieldDataOrdinaryRewardPool(hare, null);
            foreach (var id in ids)
            {
                Assert.That(eligible(id), Is.True);
                Assert.That(fieldData.Any(value => value.Id == id), Is.True);
                var offer = SpeciesUpgradeCatalog.CreateExperimentalHerbivoreMutationOffer(null, 0, 42,
                    upgrade => eligible(upgrade.Id));
                Assert.That(offer.Any(value => value.Id == id), Is.True);
            }
        }

        [Test]
        public void LegacyPrototypeAllocationRemainsEligibleAfterProfileMigration()
        {
            var hare = new SpeciesId("hare");
            const string legacyId = "poc.hare.tough-hide";
            var genome = new SpeciesGenomeProfile(hare, activeNodeIds: new[] { legacyId },
                allocations: new[] { new GenomeNodeAllocation(legacyId, 5, 2) });
            var profile = new ProfileSessionSnapshot(true, "legacy", "Legacy", new[] { genome });
            var eligible = FieldDataPermanentProgressionDefinition.CaptureMutationResearch(profile, hare);
            Assert.That(eligible(SpeciesUpgradeCatalog.ToughHideId), Is.True);
            Assert.That(eligible(SpeciesUpgradeCatalog.SeedDispersalId), Is.False);
        }

        [Test]
        public void LongStrideResearchMakesItsMovementMutationAvailableWithoutEquippingIt()
        {
            var hare = new SpeciesId("hare");
            var genome = new SpeciesGenomeProfile(hare,
                new[] { "hare.guarded-burrow", "hare.swift-digging" }, Array.Empty<string>());
            var profile = new ProfileSessionSnapshot(true, "stride", "Stride", new[] { genome });
            var eligible = FieldDataPermanentProgressionDefinition.CaptureMutationResearch(profile, hare);
            var pool = SpeciesUpgradeCatalog.ResolveFieldDataOrdinaryRewardPool(hare, null);
            var stride = pool.Single(value => value.Id == SpeciesUpgradeCatalog.FasterMovementId);
            Assert.That(stride.DisplayName, Is.EqualTo("LONG STRIDE"));
            Assert.That(eligible(stride.Id), Is.True);
            var progression = new SpeciesProgression(new SpeciesDefinition(hare,
                SpeciesRuleDefaults.Create()[SpeciesIds.Herbivore]), eligible);
            var before = progression.CurrentRules.MovementSpeed;
            Assert.That(progression.TryApplyFieldDataReward(stride), Is.True);
            Assert.That(progression.CurrentRules.MovementSpeed, Is.GreaterThan(before));
        }

        [Test]
        public void EveryOwnedLoadoutWithinCapacityCanUseTheFinalNodeWithoutActiveParents()
        {
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            var nodes = definition.HareMap.Nodes;
            var owned = nodes.Select(node => node.Id).ToArray();
            var validCount = 0;
            var terminalCount = 0;
            for (var mask = 0; mask < 1 << nodes.Count; mask++)
            {
                var active = nodes.Where((node, index) => (mask & (1 << index)) != 0)
                    .Select(node => node.Id).ToArray();
                if (active.Length > GenomeContract.ActiveCapacity) continue;
                var profile = new SpeciesGenomeProfile(new SpeciesId("hare"), owned, active);
                var frozen = definition.FreezeOwnedLoadout(profile);
                CollectionAssert.AreEqual(active, frozen.GetSpeciesGenome(new SpeciesId("hare")).ActiveNodeIds);
                validCount++;
                if (active.Contains("hare.warren-network")) terminalCount++;
            }

            Assert.That(validCount, Is.EqualTo(39203));
            Assert.That(terminalCount, Is.EqualTo(16384));
        }

        [Test]
        public void FrozenResearchLoadoutDoesNotGrantPermanentStatsAndKeepsThePocResolverStrict()
        {
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            var hare = new SpeciesId("hare");
            var active = new[] { "hare.keen-hearing", "hare.swift-digging", "hare.safe-foraging", "hare.warren-network" };
            var profile = new SpeciesGenomeProfile(hare, definition.HareMap.Nodes.Select(node => node.Id), active);
            var frozen = definition.FreezeOwnedLoadout(profile);
            var natural = SpeciesRuleDefaults.Create()[SpeciesIds.Herbivore];
            var effective = frozen.GetSpeciesGenome(hare).ApplyToNaturalRules(natural);
            Assert.That(effective.BlockAmount, Is.EqualTo(natural.BlockAmount));
            Assert.That(effective.Awareness.VisionRange, Is.EqualTo(natural.Awareness.VisionRange));
            Assert.That(effective.MovementSpeed, Is.EqualTo(natural.MovementSpeed));
            Assert.That(effective.DigestionEnergyBonus, Is.EqualTo(natural.DigestionEnergyBonus));
            Assert.That(frozen.GetPopulationPoolCapacity(hare), Is.EqualTo(50));
            Assert.That(frozen.GetSpeciesGenome(hare).CatalogFingerprint, Is.EqualTo(definition.Catalog.Fingerprint));
            Assert.Throws<InvalidOperationException>(() => new GenomeSimulationSnapshot(new[]
                { new SpeciesGenomeSnapshot(hare, active) }).Resolve(definition.Catalog));
        }

        [Test]
        public void MissingOwnedAncestorsAndUnknownOrWrongSpeciesNodesAreRejected()
        {
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            var hare = new SpeciesId("hare");
            var incomplete = new SpeciesGenomeProfile(hare,
                new[] { "hare.warren-network", "hare.safe-foraging", "hare.guarded-burrow" },
                new[] { "hare.warren-network" });
            Assert.Throws<InvalidOperationException>(() => definition.FreezeOwnedLoadout(incomplete));
            var unknown = new SpeciesGenomeProfile(hare, new[] { "legacy.unknown" }, new[] { "legacy.unknown" });
            Assert.Throws<InvalidOperationException>(() => definition.FreezeOwnedLoadout(unknown));
            Assert.Throws<ArgumentException>(() => definition.FreezeOwnedLoadout(new SpeciesGenomeProfile(new SpeciesId("fox"))));
            Assert.That(definition.FreezeOwnedLoadout(null).GetPopulationPoolCapacity(hare), Is.EqualTo(50));
        }

        static ProfileSessionSnapshot LaunchProfile(SpeciesGenomeProfile genome, bool licensed = true)
        {
            return new ProfileSessionSnapshot(true, "fd2-launch", "FD2 launch", new[] { genome },
                unlockedGenomeSpeciesIds: licensed ? new[] { genome.SpeciesId } : Array.Empty<SpeciesId>());
        }

        [TestCase("hare.guarded-burrow", null, 0f)]
        [TestCase("hare.keen-hearing", null, 0f)]
        [TestCase("hare.swift-digging", null, 0f)]
        [TestCase("hare.safe-foraging", null, 0f)]
        [TestCase("hare.warren-network", null, 0f)]
        public void FD2LaunchFreezesEveryApprovedNodeWithInactiveOwnedAncestors(string id, string attribute, float value)
        {
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            var genome = new SpeciesGenomeProfile(new SpeciesId("hare"), definition.HareMap.Nodes.Select(node => node.Id), new[] { id });
            var frozen = LaunchProfile(genome).CreateLaunchGenomeSnapshot(GenomeCatalogSnapshot.Empty).GetSpeciesGenome(genome.SpeciesId);
            Assert.That(frozen.ActiveNodeIds, Is.EqualTo(new[] { id }));
            Assert.That(frozen.CatalogFingerprint, Is.EqualTo(definition.Catalog.Fingerprint));
            if (attribute == null) Assert.That(frozen.ResolvedNodes.Single().Modifiers, Is.Empty);
            else
            {
                Assert.That(frozen.ResolvedNodes.Single().Modifiers.Single().AttributeId, Is.EqualTo(attribute));
                Assert.That(frozen.ResolvedNodes.Single().Modifiers.Single().SignedValue, Is.EqualTo(value));
            }
            Assert.That(frozen.ResolvedNodes.Single().Fingerprint, Is.EqualTo(definition.HareMap.GetNode(id).Fingerprint));
        }

        [Test]
        public void FD2LaunchAcceptsEveryOwnedLoadoutWithinCatalogCapacityAndRetainsItsOrder()
        {
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            var owned = definition.HareMap.Nodes.Select(node => node.Id).ToArray();
            for (var mask = 0; mask < 1 << owned.Length; mask++)
            {
                var selected = owned.Where((id, index) => (mask & (1 << index)) != 0).Reverse().ToArray();
                if (selected.Length > GenomeContract.ActiveCapacity) continue;
                var profile = LaunchProfile(new SpeciesGenomeProfile(new SpeciesId("hare"), owned, selected));
                Assert.That(profile.CreateLaunchGenomeSnapshot(definition.Catalog).GetSpeciesGenome(new SpeciesId("hare")).ActiveNodeIds,
                    Is.EqualTo(selected));
            }
        }

        [TestCase("license")]
        [TestCase("ancestor")]
        [TestCase("unknown")]
        [TestCase("poc-allocation")]
        public void FD2LaunchRejectsInvalidPermanentStateWithoutFallingBackToPocRules(string invalid)
        {
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            var hare = new SpeciesId("hare");
            var owned = definition.HareMap.Nodes.Select(node => node.Id).ToArray();
            if (invalid == "ancestor") owned = owned.Where(id => id != "hare.guarded-burrow").ToArray();
            var active = invalid == "unknown" ? "hare.unknown" : "hare.warren-network";
            if (invalid == "unknown") owned = owned.Concat(new[] { active }).ToArray();
            var allocations = invalid == "poc-allocation" ? new[] { new GenomeNodeAllocation(active, 5, 2) } : null;
            var profile = LaunchProfile(new SpeciesGenomeProfile(hare, owned, new[] { active }, allocations), invalid != "license");
            Assert.Throws<InvalidOperationException>(() => profile.CreateLaunchGenomeSnapshot(definition.Catalog));
        }

        [Test]
        public void FD2LaunchKeepsPocActiveParentRulesAndFreezesOtherSpeciesAlongsidePermanentHare()
        {
            var hare = new SpeciesId("hare");
            var fox = new SpeciesId("fox");
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            var root = new GenomeUpgradeNodeSnapshot("poc.root", "Root", "POC fixture", hare);
            var child = new GenomeUpgradeNodeSnapshot("poc.child", "Child", "POC fixture", hare, new[] { root.Id });
            var foxNode = new GenomeUpgradeNodeSnapshot("fox.vision", "Fox vision", "Fixture", fox,
                modifiers: new[] { new SpeciesUpgradeModifier(SpeciesAttributeIds.VisionRange, 3) });
            var catalog = new GenomeCatalogSnapshot(new[] { new SpeciesGenomeMapSnapshot(hare, new[] { root, child }),
                new SpeciesGenomeMapSnapshot(fox, new[] { foxNode }) });
            var invalidPoc = LaunchProfile(new SpeciesGenomeProfile(hare, new[] { root.Id, child.Id }, new[] { child.Id }));
            Assert.Throws<InvalidOperationException>(() => invalidPoc.CreateLaunchGenomeSnapshot(catalog));
            var validPoc = LaunchProfile(new SpeciesGenomeProfile(hare, new[] { root.Id, child.Id }, new[] { root.Id, child.Id }));
            Assert.That(validPoc.CreateLaunchGenomeSnapshot(catalog).GetSpeciesGenome(hare).ActiveNodeIds.Count, Is.EqualTo(2));
            var profile = LaunchProfile(new SpeciesGenomeProfile(hare, definition.HareMap.Nodes.Select(node => node.Id),
                new[] { "hare.warren-network" })).WithGenomeProfile(new SpeciesGenomeProfile(fox, new[] { foxNode.Id }, new[] { foxNode.Id }));
            var frozen = profile.CreateLaunchGenomeSnapshot(catalog);
            var defaults = SpeciesRuleDefaults.Create();
            var natural = new Dictionary<SpeciesId, SpeciesRules>(defaults)
            {
                [hare] = defaults[SpeciesIds.Herbivore],
                [fox] = defaults[SpeciesIds.Carnivore],
            };
            var effective = frozen.ApplyToNaturalRules(natural);
            Assert.That(frozen.GetPopulationPoolCapacity(hare), Is.EqualTo(50));
            Assert.That(effective[fox].Awareness.VisionRange, Is.EqualTo(natural[fox].Awareness.VisionRange + 3));
            Assert.That(effective[hare].BlockAmount, Is.EqualTo(natural[hare].BlockAmount));
            Assert.That(effective[SpeciesIds.Plant], Is.SameAs(natural[SpeciesIds.Plant]));
            var changed = profile.WithGenomeProfile(new SpeciesGenomeProfile(hare,
                definition.HareMap.Nodes.Select(node => node.Id), resetGeneration: 1)).CreateLaunchGenomeSnapshot(catalog);
            Assert.That(changed.GetPopulationPoolCapacity(hare), Is.EqualTo(50));
            Assert.That(frozen.GetPopulationPoolCapacity(hare), Is.EqualTo(50));
            Assert.That(changed.GetSpeciesGenome(fox).Fingerprint, Is.EqualTo(frozen.GetSpeciesGenome(fox).Fingerprint));
        }

        [Test]
        public void FD2LaunchRejectsDriftedAuthoredFd2MapAndStillAllowsUnlicensedEmptyJourneys()
        {
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            var hare = new SpeciesId("hare");
            var node = definition.HareMap.Nodes[0];
            var changed = new GenomeUpgradeNodeSnapshot(node.Id, node.DisplayName, node.Description, hare,
                node.PrerequisiteNodeIds, node.RabbitDataPrice, node.CapacityPoints,
                modifiers: new[] { new SpeciesUpgradeModifier(SpeciesAttributeIds.BlockAmount, 10) });
            var profile = LaunchProfile(new SpeciesGenomeProfile(hare, new[] { node.Id }, new[] { node.Id }));
            Assert.Throws<ArgumentException>(() => profile.CreateLaunchGenomeSnapshot(new GenomeCatalogSnapshot(new[]
                { new SpeciesGenomeMapSnapshot(hare, new[] { changed }) })));
            var empty = LaunchProfile(new SpeciesGenomeProfile(hare, new[] { node.Id }), licensed: false);
            Assert.That(empty.CreateLaunchGenomeSnapshot(definition.Catalog).GetSpeciesGenome(hare).ActiveNodeIds, Is.Empty);
            Assert.That(ProfileSessionSnapshot.Empty.CreateLaunchGenomeSnapshot(GenomeCatalogSnapshot.Empty).SpeciesGenomes, Is.Empty);
            Assert.Throws<ArgumentNullException>(() => profile.CreateLaunchGenomeSnapshot(null));
        }

        [Test]
        public void FD2LaunchFrozenRulesAndIdentitySurviveRunCheckpointResultAndLaterProfileChanges()
        {
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            var hare = new SpeciesId("hare");
            var owned = definition.HareMap.Nodes.Select(node => node.Id).ToArray();
            var profile = LaunchProfile(new SpeciesGenomeProfile(hare, owned,
                new[] { "hare.warren-network", "hare.swift-digging" }));
            var frozen = profile.CreateLaunchGenomeSnapshot(GenomeCatalogSnapshot.Empty);
            var cells = new Grid<SpeciesCell>(4, 4);
            cells.SetCell(1, 1, new SpeciesCell(hare, energy: 100));
            var run = new SimulationRunState(cells, hare, 100, 1f, targetTicks: 10, activeGenomeSnapshot: frozen);
            run.ConfigureContinuousPhases(1);
            var natural = new Dictionary<SpeciesId, SpeciesRules>(SpeciesRuleDefaults.Create())
            {
                [hare] = SpeciesRuleDefaults.Create()[SpeciesIds.Herbivore],
            };
            var runner = new SpeciesSimulationRunner(run, frozen.ApplyToNaturalRules(natural), 0.1f);
            runner.Start();
            Assert.That(runner.AdvanceOneTick(), Is.True);
            var fingerprint = frozen.Fingerprint;
            var changedProfile = profile.WithGenomeProfile(new SpeciesGenomeProfile(hare, owned, new[] { "hare.guarded-burrow" }, resetGeneration: 1));
            var next = changedProfile.CreateLaunchGenomeSnapshot(GenomeCatalogSnapshot.Empty);
            Assert.That(next.Fingerprint, Is.Not.EqualTo(fingerprint));
            Assert.That(run.ActiveGenomeSnapshot.Fingerprint, Is.EqualTo(fingerprint));
            Assert.That(run.CreateCheckpoint().Restore().ActiveGenomeSnapshot.Fingerprint, Is.EqualTo(fingerprint));
            Assert.That(SimulationRunResults.Create(run).ActiveGenomeSnapshot.Fingerprint, Is.EqualTo(fingerprint));
            Assert.That(run.ActiveGenomeSnapshot.GetPopulationPoolCapacity(hare), Is.EqualTo(50));
            Assert.That(next.GetPopulationPoolCapacity(hare), Is.EqualTo(50));
        }

        [Test]
        public void AuthoredCatalogValidationRejectsChangedTermsOrEffects()
        {
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            definition.ValidateAuthoredMap(definition.HareMap);
            var first = definition.HareMap.Nodes[0];
            var changed = new GenomeUpgradeNodeSnapshot(first.Id, first.DisplayName, first.Description,
                first.TargetSpecies, first.PrerequisiteNodeIds, first.RabbitDataPrice + 1,
                first.CapacityPoints, first.BranchName, first.Modifiers);
            var nodes = definition.HareMap.Nodes.Select(node => node.Id == first.Id ? changed : node);
            Assert.Throws<ArgumentException>(() => definition.ValidateAuthoredMap(
                new SpeciesGenomeMapSnapshot(definition.HareMap.SpeciesId, nodes)));
            CollectionAssert.AreEqual(new long[] { 10, 20, 30 }, definition.ConversionResearchPrices);
            Assert.That(definition.MaximumResearchLevel, Is.EqualTo(3));
            Assert.That(definition.ActiveCapacity, Is.EqualTo(8));
        }
    }
}
