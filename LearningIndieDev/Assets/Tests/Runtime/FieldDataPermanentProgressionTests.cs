using System;
using System.Linq;
using NUnit.Framework;

namespace SaltyGame.Tests
{
    [TestFixture]
    [Category("Simulation")]
    public sealed class FieldDataPermanentProgressionTests
    {
        [Test]
        public void EveryOwnedLoadoutWithinCapacityCanUseTheFinalNodeWithoutActiveParents()
        {
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            var nodes = definition.HareMap.Nodes;
            var owned = nodes.Select(node => node.Id).ToArray();
            var validCount = 0;
            var terminalCount = 0;
            for (var mask = 0; mask < 32; mask++)
            {
                var active = nodes.Where((node, index) => (mask & (1 << index)) != 0)
                    .Select(node => node.Id).ToArray();
                var profile = new SpeciesGenomeProfile(new SpeciesId("hare"), owned, active);
                if (active.Length == 5)
                {
                    Assert.Throws<InvalidOperationException>(() => definition.FreezeOwnedLoadout(profile));
                    continue;
                }

                var frozen = definition.FreezeOwnedLoadout(profile);
                CollectionAssert.AreEqual(active, frozen.GetSpeciesGenome(new SpeciesId("hare")).ActiveNodeIds);
                validCount++;
                if (active.Contains("hare.warren-network")) terminalCount++;
            }

            Assert.That(validCount, Is.EqualTo(31));
            Assert.That(terminalCount, Is.EqualTo(15));
        }

        [Test]
        public void FrozenOwnedLoadoutAppliesOnlyActiveEffectsAndKeepsThePocResolverStrict()
        {
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            var hare = new SpeciesId("hare");
            var active = new[] { "hare.keen-hearing", "hare.swift-digging", "hare.safe-foraging", "hare.warren-network" };
            var profile = new SpeciesGenomeProfile(hare, definition.HareMap.Nodes.Select(node => node.Id), active);
            var frozen = definition.FreezeOwnedLoadout(profile);
            var natural = SpeciesRuleDefaults.Create()[SpeciesIds.Herbivore];
            var effective = frozen.GetSpeciesGenome(hare).ApplyToNaturalRules(natural);
            Assert.That(effective.BlockAmount, Is.EqualTo(natural.BlockAmount));
            Assert.That(effective.Awareness.VisionRange, Is.EqualTo(natural.Awareness.VisionRange + 2));
            Assert.That(effective.MovementSpeed, Is.EqualTo(natural.MovementSpeed + 0.25f));
            Assert.That(effective.DigestionEnergyBonus, Is.EqualTo(natural.DigestionEnergyBonus + 0.25f));
            Assert.That(frozen.GetPopulationPoolCapacity(hare), Is.EqualTo(60));
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
