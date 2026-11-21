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
        [TestCase(false), TestCase(true)]
        public void IntroductoryRabbitAccessKeepsOtherResearchAndEquipmentGates(bool ownsResearch)
        {
            var rabbit = new SpeciesId("rabbit");
            var definition = new SpeciesDefinition(rabbit, SpeciesRuleDefaults.Create()[SpeciesIds.Herbivore]);
            var introductory = new SpeciesProgression(definition, id => ownsResearch,
                usesGeneLabResearch: true, mutationEquipment: id => false, introductoryRabbitMutations: true);
            var ordinary = new SpeciesProgression(definition, id => ownsResearch,
                usesGeneLabResearch: true, mutationEquipment: id => false);
            foreach (var id in new[] { SpeciesUpgradeCatalog.ThreatExposureId,
                SpeciesUpgradeCatalog.ToughHideId, SpeciesUpgradeCatalog.ReproductiveDriveId })
            {
                Assert.That(introductory.CanApplyFreeUpgrade(SpeciesUpgradeCatalog.Create(id)), Is.True, id);
                Assert.That(ordinary.CanApplyFreeUpgrade(SpeciesUpgradeCatalog.Create(id)), Is.False, id);
            }
            foreach (var id in new[] { SpeciesUpgradeCatalog.LongVisionId,
                SpeciesUpgradeCatalog.QuickEscapeId, SpeciesUpgradeCatalog.OpenGroundSprintId })
                Assert.That(introductory.HasMutationAccess(id), Is.False, id);
        }

        [TestCase(0, 10)]
        [TestCase(1, 15)]
        [TestCase(2, 20)]
        [TestCase(3, 25)]
        [TestCase(4, 30)]
        [TestCase(5, 35)]
        public void PopulationReservesRanksSetThePerNodeRabbitPurchaseAllowance(int rank, int allowance)
        {
            Assert.That(FieldDataPermanentProgressionDefinition.RabbitPurchaseAllowance(rank), Is.EqualTo(allowance));
            if (rank > 0)
                Assert.That(FieldDataPermanentProgressionDefinition.PopulationReservesResearchCost(rank), Is.EqualTo(30));
        }

        [Test]
        public void PopulationReservesRejectsRanksOutsideTheApprovedRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => FieldDataPermanentProgressionDefinition.RabbitPurchaseAllowance(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => FieldDataPermanentProgressionDefinition.RabbitPurchaseAllowance(6));
            Assert.Throws<ArgumentOutOfRangeException>(() => FieldDataPermanentProgressionDefinition.PopulationReservesResearchCost(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => FieldDataPermanentProgressionDefinition.PopulationReservesResearchCost(6));
        }

        [Test]
        public void MutationResearchUsesOwnershipNotEquipmentAndFreezesTheProfile()
        {
            var rabbit = new SpeciesId("rabbit");
            var empty = FieldDataPermanentProgressionDefinition.CaptureMutationResearch(null, rabbit);
            Assert.That(empty(SpeciesUpgradeCatalog.ToughHideId), Is.False);
            Assert.That(empty(SpeciesUpgradeCatalog.KeenSensesId), Is.False);
            var genome = new SpeciesGenomeProfile(rabbit,
                new[] { "rabbit.guarded-burrow", "rabbit.keen-hearing" }, Array.Empty<string>());
            var profile = new ProfileSessionSnapshot(true, "research", "Research", new[] { genome });
            var eligibility = FieldDataPermanentProgressionDefinition.CaptureMutationResearch(profile, rabbit);
            Assert.That(eligibility(SpeciesUpgradeCatalog.ToughHideId), Is.True);
            Assert.That(eligibility(SpeciesUpgradeCatalog.KeenSensesId), Is.True);
            Assert.That(eligibility(SpeciesUpgradeCatalog.LongVisionId), Is.True);
            Assert.That(eligibility(SpeciesUpgradeCatalog.EfficientDigestionId), Is.False);
            Assert.That(eligibility(SpeciesUpgradeCatalog.SeedDispersalId), Is.False);
            var progression = new SpeciesProgression(new SpeciesDefinition(rabbit, SpeciesRuleDefaults.Create()[SpeciesIds.Herbivore]), eligibility);
            Assert.That(progression.GetUpgradeLevel(SpeciesUpgradeCatalog.ToughHideId), Is.Zero);
            Assert.That(progression.TryApplyFieldDataReward(SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.SeedDispersalId).CreateSnapshot(rabbit)), Is.False);
            Assert.That(progression.TryApplyFieldDataReward(SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.ToughHideId).CreateSnapshot(rabbit)), Is.True);
            var beforeVision = progression.CurrentRules.Awareness.VisionRange;
            Assert.That(progression.TryApplyFieldDataReward(SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.LongVisionId).CreateSnapshot(rabbit)), Is.True);
            Assert.That(progression.CurrentRules.Awareness.VisionRange, Is.EqualTo(beforeVision + SpeciesUpgradeCatalog.LongVisionTiles));
            Assert.That(progression.TryApplyFieldDataReward(SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.LongVisionId).CreateSnapshot(rabbit)), Is.False);
            Assert.That(empty(SpeciesUpgradeCatalog.ToughHideId), Is.False, "Another profile's research must not change the frozen empty policy.");
        }

        [Test]
        public void LitterMutationRankLimitFollowsResearchedLitterUpgradeRank()
        {
            var rabbit = new SpeciesId("rabbit");
            var definition = new SpeciesDefinition(rabbit, SpeciesRuleDefaults.Create()[SpeciesIds.Herbivore]);
            var reward = SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.LargerMaximumLitterId).CreateSnapshot(rabbit);
            foreach (var rank in new[] { 1, 2, 3 })
            {
                var profile = new ProfileSessionSnapshot(true, "litter-" + rank, "Litter", new[]
                {
                    new SpeciesGenomeProfile(rabbit, new[] { "rabbit.larger-litters" }, new[] { "rabbit.larger-litters" },
                        litterUpgradeRank: rank)
                });
                var research = FieldDataPermanentProgressionDefinition.CaptureMutationResearch(profile, rabbit);
                var limits = FieldDataPermanentProgressionDefinition.CaptureMutationRankLimit(profile, rabbit);
                var progression = new SpeciesProgression(definition, research, usesGeneLabResearch: true,
                    mutationRankLimit: limits,
                    mutationEquipment: FieldDataPermanentProgressionDefinition.CaptureMutationEquipment(profile, rabbit));
                for (var level = 1; level <= rank; level++)
                    Assert.That(progression.TryApplyFieldDataReward(reward), Is.True, $"Research rank {rank}, mutation rank {level}");
                Assert.That(progression.TryApplyFieldDataReward(reward), Is.False, $"Research rank {rank} must cap the next Mutation rank");
            }
        }

        [Test]
        public void MinimumLitterMutationRankLimitFollowsResearchedLitterUpgradeRank()
        {
            var rabbit = new SpeciesId("rabbit");
            var definition = new SpeciesDefinition(rabbit, SpeciesRuleDefaults.Create()[SpeciesIds.Herbivore]);
            var increaseMaximum = SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.LargerMaximumLitterId)
                .CreateSnapshot(rabbit);
            var increaseMinimum = SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.LargerMinimumLitterId)
                .CreateSnapshot(rabbit);
            foreach (var rank in new[] { 1, 2, 3 })
            {
                var profile = new ProfileSessionSnapshot(true, "minimum-litter-" + rank, "Minimum Litter", new[]
                {
                    new SpeciesGenomeProfile(rabbit, new[] { FieldDataPermanentProgressionDefinition.LitterUpgradeNodeId },
                        new[] { FieldDataPermanentProgressionDefinition.LitterUpgradeNodeId }, litterUpgradeRank: rank)
                });
                var research = FieldDataPermanentProgressionDefinition.CaptureMutationResearch(profile, rabbit);
                var limits = FieldDataPermanentProgressionDefinition.CaptureMutationRankLimit(profile, rabbit);
                var progression = new SpeciesProgression(definition, research, usesGeneLabResearch: true,
                    mutationRankLimit: limits,
                    mutationEquipment: FieldDataPermanentProgressionDefinition.CaptureMutationEquipment(profile, rabbit));

                Assert.That(limits(SpeciesUpgradeCatalog.LargerMinimumLitterId), Is.EqualTo(rank));
                for (var level = 1; level <= rank; level++)
                    Assert.That(progression.TryApplyFieldDataReward(increaseMaximum), Is.True,
                        $"Research rank {rank}, maximum litter rank {level}");
                for (var level = 1; level <= rank; level++)
                    Assert.That(progression.TryApplyFieldDataReward(increaseMinimum), Is.True,
                        $"Research rank {rank}, minimum litter rank {level}");
                Assert.That(progression.TryApplyFieldDataReward(increaseMinimum), Is.False,
                    $"Research rank {rank} must cap the next minimum-litter Mutation rank");
            }
        }

        [Test]
        public void HeightenedSensesResearchMakesBothMutationsAvailableAtPhaseAndFieldDataRewards()
        {
            var rabbit = new SpeciesId("rabbit");
            var profile = new ProfileSessionSnapshot(true, "senses", "Senses", new[]
            {
                new SpeciesGenomeProfile(rabbit, new[] { "rabbit.keen-hearing" }, Array.Empty<string>())
            });
            var eligible = FieldDataPermanentProgressionDefinition.CaptureMutationResearch(profile, rabbit);
            var ids = new[] { SpeciesUpgradeCatalog.LongVisionId, SpeciesUpgradeCatalog.KeenSensesId };
            var fieldData = SpeciesUpgradeCatalog.ResolveFieldDataOrdinaryRewardPool(rabbit, null);
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
        public void QuickEscapeIsAvailableAsAFieldDataAndPhaseMutation()
        {
            var rabbit = new SpeciesId("rabbit");
            var fieldData = SpeciesUpgradeCatalog.ResolveFieldDataOrdinaryRewardPool(rabbit, null);
            var phaseOffer = SpeciesUpgradeCatalog.CreateExperimentalHerbivoreMutationOffer(
                SpeciesUpgradeCatalog.QuickEscapeId, rotation: 0, seed: 42);
            var research = FieldDataPermanentProgressionDefinition.CaptureMutationResearch(
                new ProfileSessionSnapshot(true, "escape", "Escape", new[]
                {
                    new SpeciesGenomeProfile(rabbit, new[] { "rabbit.escape-instinct" }, Array.Empty<string>())
                }), rabbit);

            Assert.That(fieldData.Any(value => value.Id == SpeciesUpgradeCatalog.QuickEscapeId), Is.True);
            Assert.That(phaseOffer.Any(value => value.Id == SpeciesUpgradeCatalog.QuickEscapeId), Is.True);
            Assert.That(research(SpeciesUpgradeCatalog.QuickEscapeId), Is.True);
            Assert.That(
                SpeciesUpgradeCatalog.GetFieldDataRewardMaxLevel(rabbit, SpeciesUpgradeCatalog.QuickEscapeId),
                Is.EqualTo(SpeciesUpgradeCatalog.QuickEscapeMaxLevel));
        }

        [TestCase(SpeciesUpgradeCatalog.ThreatExposureId, "rabbit.escape-instinct")]
        [TestCase(SpeciesUpgradeCatalog.ToughHideId, "rabbit.guarded-burrow")]
        [TestCase(SpeciesUpgradeCatalog.ReproductiveDriveId, "rabbit.reproductive-drive")]
        public void ForestEdgeStarterMutationCanStackWithoutResearch(string id, string researchNode)
        {
            var rabbit = new SpeciesId("rabbit");
            var research = FieldDataPermanentProgressionDefinition.CaptureMutationResearch(
                new ProfileSessionSnapshot(true, "fresh", "Fresh"), rabbit);
            var definition = new SpeciesDefinition(rabbit, SpeciesRuleDefaults.Create()[SpeciesIds.Herbivore]);
            var starter = new SpeciesProgression(definition, research, introductoryRabbitMutations: true);
            var upgrade = SpeciesUpgradeCatalog.Create(id);

            Assert.That(research(id), Is.False);
            Assert.That(starter.HasMutationAccess(id), Is.True);
            Assert.That(starter.TryApplyFreeUpgrade(upgrade), Is.True);
            Assert.That(starter.GetUpgradeLevel(id), Is.EqualTo(1));
            Assert.That(starter.HasMutationAccess(id), Is.True);
            Assert.That(starter.TryApplyFreeUpgrade(upgrade), Is.True);
            Assert.That(starter.GetUpgradeLevel(id), Is.EqualTo(2));
            var nextOffer = SpeciesUpgradeCatalog.CreateExperimentalHerbivoreMutationOffer(
                id, rotation: 1, seed: 42, canOffer: starter.CanApplyFreeUpgrade);
            Assert.That(nextOffer.Select(option => option.Id), Is.EquivalentTo(new[]
            {
                SpeciesUpgradeCatalog.ThreatExposureId,
                SpeciesUpgradeCatalog.ToughHideId,
                SpeciesUpgradeCatalog.ReproductiveDriveId,
            }));
            Assert.That(new SpeciesProgression(definition, research).HasMutationAccess(id), Is.False);

            var ownedResearch = FieldDataPermanentProgressionDefinition.CaptureMutationResearch(
                new ProfileSessionSnapshot(true, "owned", "Owned", new[]
                {
                    new SpeciesGenomeProfile(rabbit, new[] { researchNode }, Array.Empty<string>())
                }), rabbit);
            var researched = new SpeciesProgression(definition, ownedResearch, introductoryRabbitMutations: true);
            Assert.That(researched.TryApplyFreeUpgrade(upgrade), Is.True);
            Assert.That(researched.TryApplyFreeUpgrade(upgrade), Is.True);
            Assert.That(researched.GetUpgradeLevel(id), Is.EqualTo(2));
        }

        [Test]
        public void LegacyPrototypeAllocationRemainsEligibleAfterProfileMigration()
        {
            var rabbit = new SpeciesId("rabbit");
            const string legacyId = "poc.rabbit.tough-hide";
            var genome = new SpeciesGenomeProfile(rabbit, activeNodeIds: new[] { legacyId },
                allocations: new[] { new GenomeNodeAllocation(legacyId, 5, 2) });
            var profile = new ProfileSessionSnapshot(true, "legacy", "Legacy", new[] { genome });
            var eligible = FieldDataPermanentProgressionDefinition.CaptureMutationResearch(profile, rabbit);
            Assert.That(eligible(SpeciesUpgradeCatalog.ToughHideId), Is.True);
            Assert.That(eligible(SpeciesUpgradeCatalog.SeedDispersalId), Is.False);
        }

        [Test]
        public void EquippedLongStrideResearchMakesItsMovementMutationAvailable()
        {
            var rabbit = new SpeciesId("rabbit");
            var genome = new SpeciesGenomeProfile(rabbit,
                new[] { "rabbit.guarded-burrow", "rabbit.swift-digging" }, new[] { "rabbit.swift-digging" });
            var profile = new ProfileSessionSnapshot(true, "stride", "Stride", new[] { genome });
            var eligible = FieldDataPermanentProgressionDefinition.CaptureMutationResearch(profile, rabbit);
            var equipped = FieldDataPermanentProgressionDefinition.CaptureMutationEquipment(profile, rabbit);
            var pool = SpeciesUpgradeCatalog.ResolveFieldDataOrdinaryRewardPool(rabbit, null);
            var stride = pool.Single(value => value.Id == SpeciesUpgradeCatalog.FasterMovementId);
            Assert.That(stride.DisplayName, Is.EqualTo("LONG STRIDE"));
            Assert.That(eligible(stride.Id), Is.True);
            var progression = new SpeciesProgression(new SpeciesDefinition(rabbit,
                SpeciesRuleDefaults.Create()[SpeciesIds.Herbivore]), eligible, usesGeneLabResearch: true,
                mutationEquipment: equipped);
            var before = progression.CurrentRules.MovementSpeed;
            Assert.That(progression.TryApplyFieldDataReward(stride), Is.True);
            Assert.That(progression.CurrentRules.MovementSpeed, Is.GreaterThan(before));
        }

        [Test]
        public void GestationUpgradeResearchUnlocksThreeRankFasterGestationMutation()
        {
            var rabbit = new SpeciesId("rabbit");
            var definition = new SpeciesDefinition(rabbit, SpeciesRuleDefaults.Create()[SpeciesIds.Herbivore]);
            var profile = new ProfileSessionSnapshot(true, "gestation", "Gestation", new[]
            {
                new SpeciesGenomeProfile(rabbit, new[]
                    { FieldDataPermanentProgressionDefinition.GestationUpgradeNodeId },
                    new[] { FieldDataPermanentProgressionDefinition.GestationUpgradeNodeId })
            });
            var research = FieldDataPermanentProgressionDefinition.CaptureMutationResearch(profile, rabbit);
            var equipment = FieldDataPermanentProgressionDefinition.CaptureMutationEquipment(profile, rabbit);
            var limits = FieldDataPermanentProgressionDefinition.CaptureMutationRankLimit(profile, rabbit);
            var reward = SpeciesUpgradeCatalog.ResolveFieldDataOrdinaryRewardPool(rabbit, null)
                .Single(upgrade => upgrade.Id == SpeciesUpgradeCatalog.FasterGestationId);
            var phaseOffers = SpeciesUpgradeCatalog.CreateExperimentalHerbivoreMutationOffer(null, 0, 42,
                upgrade => research(upgrade.Id) && equipment(upgrade.Id));

            Assert.That(research(SpeciesUpgradeCatalog.FasterGestationId), Is.True);
            Assert.That(limits(SpeciesUpgradeCatalog.FasterGestationId), Is.EqualTo(3));
            Assert.That(phaseOffers.Select(upgrade => upgrade.Id), Does.Contain(SpeciesUpgradeCatalog.FasterGestationId));

            var progression = new SpeciesProgression(definition, research, usesGeneLabResearch: true,
                mutationRankLimit: limits, mutationEquipment: equipment);
            for (var rank = 1; rank <= SpeciesUpgradeCatalog.FasterGestationMaxLevel; rank++)
            {
                Assert.That(progression.TryApplyFieldDataReward(reward), Is.True, $"Mutation rank {rank}");
                var expectedTicks = (int)Math.Max(1L,
                    ((long)definition.Rules.GestationTicks * (100 - 15 * rank) + 50) / 100);
                Assert.That(progression.CurrentRules.GestationTicks,
                    Is.EqualTo(expectedTicks));
            }
            Assert.That(progression.TryApplyFieldDataReward(reward), Is.False, "Research supports only three ranks.");
        }

        [Test]
        public void OpenGroundSprintResearchUnlocksOneRankMovementMutation()
        {
            var rabbit = new SpeciesId("rabbit");
            var definition = new SpeciesDefinition(rabbit, SpeciesRuleDefaults.Create()[SpeciesIds.Herbivore]);
            var profile = new ProfileSessionSnapshot(true, "sprint", "Sprint", new[]
            {
                new SpeciesGenomeProfile(rabbit, new[]
                    { FieldDataPermanentProgressionDefinition.SprintNodeId },
                    new[] { FieldDataPermanentProgressionDefinition.SprintNodeId })
            });
            var research = FieldDataPermanentProgressionDefinition.CaptureMutationResearch(profile, rabbit);
            var equipment = FieldDataPermanentProgressionDefinition.CaptureMutationEquipment(profile, rabbit);
            var limits = FieldDataPermanentProgressionDefinition.CaptureMutationRankLimit(profile, rabbit);
            var reward = SpeciesUpgradeCatalog.ResolveFieldDataOrdinaryRewardPool(rabbit, null)
                .Single(upgrade => upgrade.Id == SpeciesUpgradeCatalog.OpenGroundSprintId);
            var phaseOffers = SpeciesUpgradeCatalog.CreateExperimentalHerbivoreMutationOffer(null, 0, 42,
                upgrade => research(upgrade.Id) && equipment(upgrade.Id));

            Assert.That(research(SpeciesUpgradeCatalog.OpenGroundSprintId), Is.True);
            Assert.That(limits(SpeciesUpgradeCatalog.OpenGroundSprintId), Is.EqualTo(1));
            Assert.That(phaseOffers.Select(upgrade => upgrade.Id), Does.Contain(SpeciesUpgradeCatalog.OpenGroundSprintId));

            var progression = new SpeciesProgression(definition, research, usesGeneLabResearch: true,
                mutationRankLimit: limits, mutationEquipment: equipment);
            var initialMovement = progression.CurrentRules.MovementSpeed;
            Assert.That(progression.TryApplyFieldDataReward(reward), Is.True);
            Assert.That(progression.CurrentRules.MovementSpeed,
                Is.EqualTo(initialMovement + SpeciesUpgradeCatalog.OpenGroundSprintMovementSpeedBonus));
            Assert.That(progression.TryApplyFieldDataReward(reward), Is.False, "Sprint is a one-rank Mutation.");

            var unresearched = FieldDataPermanentProgressionDefinition.CaptureMutationResearch(
                new ProfileSessionSnapshot(true, "new", "New", new[]
                {
                    new SpeciesGenomeProfile(rabbit, Array.Empty<string>(), Array.Empty<string>())
                }), rabbit);
            Assert.That(unresearched(SpeciesUpgradeCatalog.OpenGroundSprintId), Is.False);

            var bornBurrowerLoadout = new ProfileSessionSnapshot(true, "burrower", "Burrower", new[]
            {
                new SpeciesGenomeProfile(rabbit,
                    new[] { FieldDataPermanentProgressionDefinition.SprintNodeId,
                        FieldDataPermanentProgressionDefinition.BornBurrowerNodeId },
                    new[] { FieldDataPermanentProgressionDefinition.BornBurrowerNodeId })
            });
            Assert.That(rabbit.Value, Is.EqualTo("rabbit"));
            Assert.That(bornBurrowerLoadout.TryGetGenomeProfile(rabbit, out var bornBurrowerGenome), Is.True);
            CollectionAssert.AreEqual(new[] { FieldDataPermanentProgressionDefinition.BornBurrowerNodeId },
                bornBurrowerGenome.ActiveNodeIds);
            var starterResearch = FieldDataPermanentProgressionDefinition.CaptureMutationResearch(
                bornBurrowerLoadout, rabbit);
            var starterEquipment = FieldDataPermanentProgressionDefinition.CaptureMutationEquipment(
                bornBurrowerLoadout, rabbit);
            Assert.That(starterResearch(SpeciesUpgradeCatalog.OpenGroundSprintId), Is.True,
                "Sprint research remains owned even when its Mutation is excluded by the active starter gene.");
            Assert.That(starterEquipment(SpeciesUpgradeCatalog.OpenGroundSprintId), Is.False,
                "Born Burrower excludes the Open-ground Sprint Mutation while equipped.");
        }

        [Test]
        public void EveryOwnedLoadoutWithinCapacityCanUseTheFinalNodeWithoutActiveParents()
        {
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            var nodes = definition.RabbitMap.Nodes;
            var owned = nodes.Select(node => node.Id).ToArray();
            var validCount = 0;
            var terminalCount = 0;
            for (var mask = 0; mask < 1 << nodes.Count; mask++)
            {
                var active = nodes.Where((node, index) => (mask & (1 << index)) != 0)
                    .Select(node => node.Id).ToArray();
                if (active.Length > GenomeContract.ActiveCapacity) continue;
                var profile = new SpeciesGenomeProfile(new SpeciesId("rabbit"), owned, active);
                var frozen = definition.FreezeOwnedLoadout(profile);
                CollectionAssert.AreEqual(active, frozen.GetSpeciesGenome(new SpeciesId("rabbit")).ActiveNodeIds);
                validCount++;
                if (active.Contains("rabbit.warren-network")) terminalCount++;
            }

            Assert.That(validCount, Is.EqualTo(106762));
            Assert.That(terminalCount, Is.EqualTo(41226));
        }

        [Test]
        public void FrozenResearchLoadoutDoesNotGrantPermanentStatsAndKeepsThePocResolverStrict()
        {
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            var rabbit = new SpeciesId("rabbit");
            var active = new[] { "rabbit.keen-hearing", "rabbit.swift-digging", "rabbit.safe-foraging", "rabbit.warren-network" };
            var profile = new SpeciesGenomeProfile(rabbit, definition.RabbitMap.Nodes.Select(node => node.Id), active);
            var frozen = definition.FreezeOwnedLoadout(profile);
            var natural = SpeciesRuleDefaults.Create()[SpeciesIds.Herbivore];
            var effective = frozen.GetSpeciesGenome(rabbit).ApplyToNaturalRules(natural);
            Assert.That(effective.BlockAmount, Is.EqualTo(natural.BlockAmount));
            Assert.That(effective.Awareness.VisionRange, Is.EqualTo(natural.Awareness.VisionRange));
            Assert.That(effective.MovementSpeed, Is.EqualTo(natural.MovementSpeed));
            Assert.That(effective.DigestionEnergyBonus, Is.EqualTo(natural.DigestionEnergyBonus));
            Assert.That(frozen.GetPopulationPoolCapacity(rabbit), Is.EqualTo(50));
            Assert.That(frozen.GetSpeciesGenome(rabbit).CatalogFingerprint, Is.EqualTo(definition.Catalog.Fingerprint));
            Assert.Throws<InvalidOperationException>(() => new GenomeSimulationSnapshot(new[]
                { new SpeciesGenomeSnapshot(rabbit, active) }).Resolve(definition.Catalog));
        }

        [Test]
        public void LegacyActiveNodesRemainLaunchableAndUnknownOrWrongSpeciesNodesAreRejected()
        {
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            var rabbit = new SpeciesId("rabbit");
            var incomplete = new SpeciesGenomeProfile(rabbit,
                new[] { "rabbit.warren-network", "rabbit.safe-foraging", "rabbit.guarded-burrow" },
                new[] { "rabbit.warren-network" });
            Assert.That(definition.FreezeOwnedLoadout(incomplete).GetSpeciesGenome(rabbit).ActiveNodeIds,
                Is.EqualTo(new[] { "rabbit.warren-network" }));
            var unknown = new SpeciesGenomeProfile(rabbit, new[] { "legacy.unknown" }, new[] { "legacy.unknown" });
            Assert.Throws<InvalidOperationException>(() => definition.FreezeOwnedLoadout(unknown));
            Assert.Throws<ArgumentException>(() => definition.FreezeOwnedLoadout(new SpeciesGenomeProfile(new SpeciesId("fox"))));
            Assert.That(definition.FreezeOwnedLoadout(null).GetPopulationPoolCapacity(rabbit), Is.EqualTo(50));
        }

        static ProfileSessionSnapshot LaunchProfile(SpeciesGenomeProfile genome, bool licensed = true)
        {
            return new ProfileSessionSnapshot(true, "fd2-launch", "FD2 launch", new[] { genome },
                unlockedGenomeSpeciesIds: licensed ? new[] { genome.SpeciesId } : Array.Empty<SpeciesId>());
        }

        [TestCase("rabbit.guarded-burrow", null, 0f)]
        [TestCase("rabbit.keen-hearing", null, 0f)]
        [TestCase("rabbit.swift-digging", null, 0f)]
        [TestCase("rabbit.safe-foraging", null, 0f)]
        [TestCase("rabbit.warren-network", null, 0f)]
        public void FD2LaunchFreezesEveryApprovedNodeWithInactiveOwnedAncestors(string id, string attribute, float value)
        {
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            var genome = new SpeciesGenomeProfile(new SpeciesId("rabbit"), definition.RabbitMap.Nodes.Select(node => node.Id), new[] { id });
            var frozen = LaunchProfile(genome).CreateLaunchGenomeSnapshot(GenomeCatalogSnapshot.Empty).GetSpeciesGenome(genome.SpeciesId);
            Assert.That(frozen.ActiveNodeIds, Is.EqualTo(new[] { id }));
            Assert.That(frozen.CatalogFingerprint, Is.EqualTo(definition.Catalog.Fingerprint));
            if (attribute == null) Assert.That(frozen.ResolvedNodes.Single().Modifiers, Is.Empty);
            else
            {
                Assert.That(frozen.ResolvedNodes.Single().Modifiers.Single().AttributeId, Is.EqualTo(attribute));
                Assert.That(frozen.ResolvedNodes.Single().Modifiers.Single().SignedValue, Is.EqualTo(value));
            }
            Assert.That(frozen.ResolvedNodes.Single().Fingerprint, Is.EqualTo(definition.RabbitMap.GetNode(id).Fingerprint));
        }

        [Test]
        public void FD2LaunchAcceptsEveryOwnedLoadoutWithinCatalogCapacityAndRetainsItsOrder()
        {
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            var owned = definition.RabbitMap.Nodes.Select(node => node.Id).ToArray();
            for (var mask = 0; mask < 1 << owned.Length; mask++)
            {
                var selected = owned.Where((id, index) => (mask & (1 << index)) != 0).Reverse().ToArray();
                if (selected.Length > GenomeContract.ActiveCapacity) continue;
                var profile = LaunchProfile(new SpeciesGenomeProfile(new SpeciesId("rabbit"), owned, selected));
                Assert.That(profile.CreateLaunchGenomeSnapshot(definition.Catalog).GetSpeciesGenome(new SpeciesId("rabbit")).ActiveNodeIds,
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
            var rabbit = new SpeciesId("rabbit");
            var owned = definition.RabbitMap.Nodes.Select(node => node.Id).ToArray();
            if (invalid == "ancestor")
            {
                // Older profiles could own and equip Population Reserves before the approved
                // prerequisite path existed; preserve that saved loadout as grandfathered.
                owned = owned.Where(id => id != FieldDataPermanentProgressionDefinition.ExpandedBurrowsNodeId).ToArray();
                var legacy = LaunchProfile(new SpeciesGenomeProfile(rabbit, owned, new[] { "rabbit.warren-network" }));
                Assert.DoesNotThrow(() => legacy.CreateLaunchGenomeSnapshot(definition.Catalog));
                return;
            }
            var active = invalid == "unknown" ? "rabbit.unknown" : "rabbit.warren-network";
            if (invalid == "unknown") owned = owned.Concat(new[] { active }).ToArray();
            var allocations = invalid == "poc-allocation" ? new[] { new GenomeNodeAllocation(active, 5, 2) } : null;
            var profile = LaunchProfile(new SpeciesGenomeProfile(rabbit, owned, new[] { active }, allocations), invalid != "license");
            Assert.Throws<InvalidOperationException>(() => profile.CreateLaunchGenomeSnapshot(definition.Catalog));
        }

        [Test]
        public void FD2LaunchKeepsPocActiveParentRulesAndFreezesOtherSpeciesAlongsidePermanentRabbit()
        {
            var rabbit = new SpeciesId("rabbit");
            var fox = new SpeciesId("fox");
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            var root = new GenomeUpgradeNodeSnapshot("poc.root", "Root", "POC fixture", rabbit);
            var child = new GenomeUpgradeNodeSnapshot("poc.child", "Child", "POC fixture", rabbit, new[] { root.Id });
            var foxNode = new GenomeUpgradeNodeSnapshot("fox.vision", "Fox vision", "Fixture", fox,
                modifiers: new[] { new SpeciesUpgradeModifier(SpeciesAttributeIds.VisionRange, 3) });
            var catalog = new GenomeCatalogSnapshot(new[] { new SpeciesGenomeMapSnapshot(rabbit, new[] { root, child }),
                new SpeciesGenomeMapSnapshot(fox, new[] { foxNode }) });
            var invalidPoc = LaunchProfile(new SpeciesGenomeProfile(rabbit, new[] { root.Id, child.Id }, new[] { child.Id }));
            Assert.Throws<InvalidOperationException>(() => invalidPoc.CreateLaunchGenomeSnapshot(catalog));
            var validPoc = LaunchProfile(new SpeciesGenomeProfile(rabbit, new[] { root.Id, child.Id }, new[] { root.Id, child.Id }));
            Assert.That(validPoc.CreateLaunchGenomeSnapshot(catalog).GetSpeciesGenome(rabbit).ActiveNodeIds.Count, Is.EqualTo(2));
            var profile = LaunchProfile(new SpeciesGenomeProfile(rabbit, definition.RabbitMap.Nodes.Select(node => node.Id),
                new[] { "rabbit.warren-network" })).WithGenomeProfile(new SpeciesGenomeProfile(fox, new[] { foxNode.Id }, new[] { foxNode.Id }));
            var frozen = profile.CreateLaunchGenomeSnapshot(catalog);
            var defaults = SpeciesRuleDefaults.Create();
            var natural = new Dictionary<SpeciesId, SpeciesRules>(defaults)
            {
                [rabbit] = defaults[SpeciesIds.Herbivore],
                [fox] = defaults[SpeciesIds.Carnivore],
            };
            var effective = frozen.ApplyToNaturalRules(natural);
            Assert.That(frozen.GetPopulationPoolCapacity(rabbit), Is.EqualTo(50));
            Assert.That(effective[fox].Awareness.VisionRange, Is.EqualTo(natural[fox].Awareness.VisionRange + 3));
            Assert.That(effective[rabbit].BlockAmount, Is.EqualTo(natural[rabbit].BlockAmount));
            Assert.That(effective[SpeciesIds.Plant], Is.SameAs(natural[SpeciesIds.Plant]));
            var changed = profile.WithGenomeProfile(new SpeciesGenomeProfile(rabbit,
                definition.RabbitMap.Nodes.Select(node => node.Id), resetGeneration: 1)).CreateLaunchGenomeSnapshot(catalog);
            Assert.That(changed.GetPopulationPoolCapacity(rabbit), Is.EqualTo(50));
            Assert.That(frozen.GetPopulationPoolCapacity(rabbit), Is.EqualTo(50));
            Assert.That(changed.GetSpeciesGenome(fox).Fingerprint, Is.EqualTo(frozen.GetSpeciesGenome(fox).Fingerprint));
        }

        [Test]
        public void FD2LaunchRejectsDriftedAuthoredFd2MapAndStillAllowsUnlicensedEmptyJourneys()
        {
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            var rabbit = new SpeciesId("rabbit");
            var node = definition.RabbitMap.Nodes[0];
            var changed = new GenomeUpgradeNodeSnapshot(node.Id, node.DisplayName, node.Description, rabbit,
                node.PrerequisiteNodeIds, node.RabbitDataPrice, node.CapacityPoints,
                modifiers: new[] { new SpeciesUpgradeModifier(SpeciesAttributeIds.BlockAmount, 10) });
            var profile = LaunchProfile(new SpeciesGenomeProfile(rabbit, new[] { node.Id }, new[] { node.Id }));
            Assert.Throws<ArgumentException>(() => profile.CreateLaunchGenomeSnapshot(new GenomeCatalogSnapshot(new[]
                { new SpeciesGenomeMapSnapshot(rabbit, new[] { changed }) })));
            var empty = LaunchProfile(new SpeciesGenomeProfile(rabbit, new[] { node.Id }), licensed: false);
            Assert.That(empty.CreateLaunchGenomeSnapshot(definition.Catalog).GetSpeciesGenome(rabbit).ActiveNodeIds, Is.Empty);
            Assert.That(ProfileSessionSnapshot.Empty.CreateLaunchGenomeSnapshot(GenomeCatalogSnapshot.Empty).SpeciesGenomes, Is.Empty);
            Assert.Throws<ArgumentNullException>(() => profile.CreateLaunchGenomeSnapshot(null));
        }

        [Test]
        public void FD2LaunchFrozenRulesAndIdentitySurviveRunCheckpointResultAndLaterProfileChanges()
        {
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            var rabbit = new SpeciesId("rabbit");
            var owned = definition.RabbitMap.Nodes.Select(node => node.Id).ToArray();
            var profile = LaunchProfile(new SpeciesGenomeProfile(rabbit, owned,
                new[] { "rabbit.warren-network", "rabbit.swift-digging" }));
            var frozen = profile.CreateLaunchGenomeSnapshot(GenomeCatalogSnapshot.Empty);
            var cells = new Grid<SpeciesCell>(4, 4);
            cells.SetCell(1, 1, new SpeciesCell(rabbit, energy: 100));
            var run = new SimulationRunState(cells, rabbit, 100, 1f, targetTicks: 10, activeGenomeSnapshot: frozen);
            run.ConfigureContinuousPhases(1);
            var natural = new Dictionary<SpeciesId, SpeciesRules>(SpeciesRuleDefaults.Create())
            {
                [rabbit] = SpeciesRuleDefaults.Create()[SpeciesIds.Herbivore],
            };
            var runner = new SpeciesSimulationRunner(run, frozen.ApplyToNaturalRules(natural), 0.1f);
            runner.Start();
            Assert.That(runner.AdvanceOneTick(), Is.True);
            var fingerprint = frozen.Fingerprint;
            var changedProfile = profile.WithGenomeProfile(new SpeciesGenomeProfile(rabbit, owned, new[] { "rabbit.guarded-burrow" }, resetGeneration: 1));
            var next = changedProfile.CreateLaunchGenomeSnapshot(GenomeCatalogSnapshot.Empty);
            Assert.That(next.Fingerprint, Is.Not.EqualTo(fingerprint));
            Assert.That(run.ActiveGenomeSnapshot.Fingerprint, Is.EqualTo(fingerprint));
            Assert.That(run.CreateCheckpoint().Restore().ActiveGenomeSnapshot.Fingerprint, Is.EqualTo(fingerprint));
            Assert.That(SimulationRunResults.Create(run).ActiveGenomeSnapshot.Fingerprint, Is.EqualTo(fingerprint));
            Assert.That(run.ActiveGenomeSnapshot.GetPopulationPoolCapacity(rabbit), Is.EqualTo(50));
            Assert.That(next.GetPopulationPoolCapacity(rabbit), Is.EqualTo(50));
        }

        [Test]
        public void AuthoredCatalogValidationRejectsChangedTermsOrEffects()
        {
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            definition.ValidateAuthoredMap(definition.RabbitMap);
            var first = definition.RabbitMap.Nodes[0];
            var changed = new GenomeUpgradeNodeSnapshot(first.Id, first.DisplayName, first.Description,
                first.TargetSpecies, first.PrerequisiteNodeIds, first.RabbitDataPrice + 1,
                first.CapacityPoints, first.BranchName, first.Modifiers);
            var nodes = definition.RabbitMap.Nodes.Select(node => node.Id == first.Id ? changed : node);
            Assert.Throws<ArgumentException>(() => definition.ValidateAuthoredMap(
                new SpeciesGenomeMapSnapshot(definition.RabbitMap.SpeciesId, nodes)));
            CollectionAssert.AreEqual(new long[] { 10, 20, 30 }, definition.ConversionResearchPrices);
            Assert.That(definition.MaximumResearchLevel, Is.EqualTo(3));
            Assert.That(definition.ActiveCapacity, Is.EqualTo(8));
        }
    }
}
