using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace SaltyGame.Tests
{
    [TestFixture]
    public sealed class SpeciesUpgradeContractTests
    {
        static readonly GridPattern EmptyPattern = new GridPattern(new Vector2Int[0]);

        [Test]
        public void RegistryExposesStableDefinitionsForUpgradeTargets()
        {
            Assert.That(
                SpeciesAttributeRegistry.TryGet(SpeciesAttributeIds.MovementSpeed, out var definition),
                Is.True);
            Assert.That(definition.DisplayName, Is.EqualTo("Movement Speed"));
            Assert.That(definition.ValueKind, Is.EqualTo(SpeciesAttributeValueKind.Float));
            Assert.That(SpeciesAttributeRegistry.Contains(SpeciesAttributeIds.VisionRange), Is.True);
            Assert.That(SpeciesAttributeRegistry.Contains(SpeciesAttributeIds.AttackAmount), Is.True);
        }

        [Test]
        public void SnapshotAppliesMultipleSignedModifiersToOneSpecies()
        {
            var rules = CreateRules(
                movementSpeed: 1f,
                metabolism: 2,
                awareness: new SpeciesAwarenessRules(visionRange: 2, intelligence: 1));
            var snapshot = new SpeciesUpgradeSnapshot(
                "trailblazer-test",
                "Trailblazer Test",
                "Test upgrade",
                SpeciesIds.Herbivore,
                cost: 5,
                new[]
                {
                    new SpeciesUpgradeModifier(SpeciesAttributeIds.MovementSpeed, 0.5f),
                    new SpeciesUpgradeModifier(SpeciesAttributeIds.Metabolism, -1f),
                    new SpeciesUpgradeModifier(SpeciesAttributeIds.VisionRange, 1f),
                });

            var result = snapshot.Apply(rules);

            Assert.That(snapshot.TargetSpecies, Is.EqualTo(SpeciesIds.Herbivore));
            Assert.That(snapshot.Scope, Is.EqualTo(SpeciesUpgradeScope.PerRun));
            Assert.That(result.MovementSpeed, Is.EqualTo(1.5f));
            Assert.That(result.Metabolism, Is.EqualTo(1));
            Assert.That(result.Awareness.VisionRange, Is.EqualTo(3));
            Assert.That(result.BlockAmount, Is.EqualTo(rules.BlockAmount));
        }

        [Test]
        public void SnapshotFingerprintIncludesModifierOrderAndValues()
        {
            var first = new SpeciesUpgradeSnapshot(
                "ordered-test",
                "Ordered Test",
                "Test upgrade",
                SpeciesIds.Herbivore,
                5,
                new[]
                {
                    new SpeciesUpgradeModifier(SpeciesAttributeIds.MovementSpeed, 0.5f),
                    new SpeciesUpgradeModifier(SpeciesAttributeIds.Metabolism, -1f),
                });
            var second = new SpeciesUpgradeSnapshot(
                "ordered-test",
                "Ordered Test",
                "Test upgrade",
                SpeciesIds.Herbivore,
                5,
                new[]
                {
                    new SpeciesUpgradeModifier(SpeciesAttributeIds.Metabolism, -1f),
                    new SpeciesUpgradeModifier(SpeciesAttributeIds.MovementSpeed, 0.5f),
                });

            Assert.That(first.Fingerprint, Is.Not.EqualTo(second.Fingerprint));
        }

        [Test]
        public void SnapshotRejectsUnknownOrDuplicateAttributeIds()
        {
            Assert.Throws<ArgumentException>(() => new SpeciesUpgradeSnapshot(
                "unknown-test",
                "Unknown Test",
                "Test upgrade",
                SpeciesIds.Herbivore,
                0,
                new[] { new SpeciesUpgradeModifier("species.unknown", 1f) }));

            Assert.Throws<ArgumentException>(() => new SpeciesUpgradeSnapshot(
                "duplicate-test",
                "Duplicate Test",
                "Test upgrade",
                SpeciesIds.Herbivore,
                0,
                new[]
                {
                    new SpeciesUpgradeModifier(SpeciesAttributeIds.MovementSpeed, 0.5f),
                    new SpeciesUpgradeModifier(SpeciesAttributeIds.MovementSpeed, 0.25f),
                }));
        }

        [Test]
        public void SnapshotRejectsFractionalIntegerModifiers()
        {
            Assert.Throws<ArgumentException>(() => new SpeciesUpgradeSnapshot(
                "fractional-test",
                "Fractional Test",
                "Test upgrade",
                SpeciesIds.Herbivore,
                0,
                new[]
                {
                    new SpeciesUpgradeModifier(SpeciesAttributeIds.BlockAmount, 0.5f),
                }));
        }

        [Test]
        public void SnapshotRejectsAmbiguousPrerequisiteRelationships()
        {
            Assert.Throws<ArgumentException>(() => new SpeciesUpgradeSnapshot(
                "self-required",
                "Self Required",
                "Test upgrade",
                SpeciesIds.Herbivore,
                0,
                new[]
                {
                    new SpeciesUpgradeModifier(SpeciesAttributeIds.MovementSpeed, 0.5f),
                },
                prerequisiteUpgradeIds: new[] { "self-required" }));

            Assert.Throws<ArgumentException>(() => new SpeciesUpgradeSnapshot(
                "conflicting-links",
                "Conflicting Links",
                "Test upgrade",
                SpeciesIds.Herbivore,
                0,
                new[]
                {
                    new SpeciesUpgradeModifier(SpeciesAttributeIds.MovementSpeed, 0.5f),
                },
                prerequisiteUpgradeIds: new[] { "other" },
                excludedUpgradeIds: new[] { "other" }));

            Assert.Throws<ArgumentException>(() => new SpeciesUpgradeSnapshot(
                "self-excluded",
                "Self Excluded",
                "Test upgrade",
                SpeciesIds.Herbivore,
                0,
                new[]
                {
                    new SpeciesUpgradeModifier(SpeciesAttributeIds.MovementSpeed, 0.5f),
                },
                excludedUpgradeIds: new[] { "self-excluded" }));

            Assert.Throws<ArgumentException>(() => new SpeciesUpgradeSnapshot(
                "duplicate-prerequisite",
                "Duplicate Prerequisite",
                "Test upgrade",
                SpeciesIds.Herbivore,
                0,
                new[]
                {
                    new SpeciesUpgradeModifier(SpeciesAttributeIds.MovementSpeed, 0.5f),
                },
                prerequisiteUpgradeIds: new[] { "other", "other" }));
        }

        [Test]
        public void SnapshotRejectsEmptyMetadataCostsAndModifierValues()
        {
            Assert.Throws<ArgumentException>(() => new SpeciesUpgradeSnapshot(
                "",
                "Display",
                "Description",
                SpeciesIds.Herbivore,
                0,
                new[]
                {
                    new SpeciesUpgradeModifier(SpeciesAttributeIds.MovementSpeed, 0.5f),
                }));

            Assert.Throws<ArgumentException>(() => new SpeciesUpgradeSnapshot(
                "id",
                "",
                "Description",
                SpeciesIds.Herbivore,
                0,
                new[]
                {
                    new SpeciesUpgradeModifier(SpeciesAttributeIds.MovementSpeed, 0.5f),
                }));

            Assert.Throws<ArgumentException>(() => new SpeciesUpgradeSnapshot(
                "id",
                "Display",
                "",
                SpeciesIds.Herbivore,
                0,
                new[]
                {
                    new SpeciesUpgradeModifier(SpeciesAttributeIds.MovementSpeed, 0.5f),
                }));

            Assert.Throws<ArgumentOutOfRangeException>(() => new SpeciesUpgradeSnapshot(
                "negative-cost",
                "Negative Cost",
                "Test upgrade",
                SpeciesIds.Herbivore,
                -1,
                new[]
                {
                    new SpeciesUpgradeModifier(SpeciesAttributeIds.MovementSpeed, 0.5f),
                }));

            Assert.Throws<ArgumentException>(() => new SpeciesUpgradeSnapshot(
                "no-modifiers",
                "No Modifiers",
                "Test upgrade",
                SpeciesIds.Herbivore,
                0,
                Array.Empty<SpeciesUpgradeModifier>()));

            Assert.Throws<ArgumentOutOfRangeException>(() => new SpeciesUpgradeModifier(
                SpeciesAttributeIds.MovementSpeed,
                0f));
        }

        [Test]
        public void ProgressionRejectsInvalidRunUpgradeCombinationsWithoutMutation()
        {
            var progression = new SpeciesProgression(
                new SpeciesDefinition(
                    new SpeciesId("hare"),
                    CreateRules(movementSpeed: 1f, metabolism: 1, awareness: null)));
            var initialRules = progression.CurrentRules;

            var wrongSpecies = CreateSnapshot(
                "wrong-species",
                new SpeciesId("fox"),
                new SpeciesUpgradeModifier(SpeciesAttributeIds.MovementSpeed, 0.5f));
            Assert.That(progression.TryApplyRunUpgrade(wrongSpecies), Is.False);
            Assert.That(progression.CurrentRules, Is.SameAs(initialRules));
            Assert.That(progression.OrderedUpgradeIds, Is.Empty);

            var missingPrerequisite = CreateSnapshot(
                "requires-base",
                new SpeciesId("hare"),
                new SpeciesUpgradeModifier(SpeciesAttributeIds.MovementSpeed, 0.5f),
                prerequisiteUpgradeIds: new[] { "base" });
            Assert.That(progression.TryApplyRunUpgrade(missingPrerequisite), Is.False);
            Assert.That(progression.CurrentRules, Is.SameAs(initialRules));

            var baseUpgrade = CreateSnapshot(
                "base",
                new SpeciesId("hare"),
                new SpeciesUpgradeModifier(SpeciesAttributeIds.CrowdingTolerance, 1f));
            Assert.That(progression.TryApplyRunUpgrade(baseUpgrade), Is.True);

            var excludedByBase = CreateSnapshot(
                "excluded-by-base",
                new SpeciesId("hare"),
                new SpeciesUpgradeModifier(SpeciesAttributeIds.MovementSpeed, 0.5f),
                excludedUpgradeIds: new[] { "base" });
            Assert.That(progression.TryApplyRunUpgrade(excludedByBase), Is.False);
            Assert.That(progression.CurrentRules.MovementSpeed, Is.EqualTo(1f));
            Assert.That(progression.CurrentRules.CrowdingTolerance, Is.EqualTo(1));
            Assert.That(progression.OrderedUpgradeIds, Is.EqualTo(new[] { "base" }));
            Assert.That(progression.TryApplyRunUpgrade(baseUpgrade), Is.False);
        }

        [Test]
        public void ProgressionAppliesSingleSpeciesSnapshotAndPreservesOrder()
        {
            var progression = new SpeciesProgression(
                new SpeciesDefinition(SpeciesIds.Herbivore, CreateRules(movementSpeed: 1f, metabolism: 1, awareness: null)));
            var upgrade = new SpeciesUpgradeSnapshot(
                "snapshot-test",
                "Snapshot Test",
                "Test upgrade",
                SpeciesIds.Herbivore,
                5,
                new[]
                {
                    new SpeciesUpgradeModifier(SpeciesAttributeIds.MovementSpeed, 0.5f),
                });

            Assert.That(progression.TryApplyRunUpgrade(upgrade), Is.True);
            Assert.That(progression.CurrentRules.MovementSpeed, Is.EqualTo(1.5f));
            Assert.That(progression.OrderedUpgradeIds, Is.EqualTo(new[] { "snapshot-test" }));
            Assert.That(progression.TryApplyRunUpgrade(upgrade), Is.False);
        }

        [Test]
        public void RunnerCarriesOrderedUpgradeSnapshotsIntoTheRunResult()
        {
            var snapshot = new SpeciesUpgradeSnapshot(
                "result-test",
                "Result Test",
                "Test upgrade",
                SpeciesIds.Herbivore,
                0,
                new[]
                {
                    new SpeciesUpgradeModifier(SpeciesAttributeIds.MovementSpeed, 0.5f),
                });
            var data = new CellularSimData(
                1,
                1,
                new Dictionary<SpeciesId, float>(),
                SpeciesRuleDefaults.Create(),
                runDurationSeconds: 1f,
                stepInterval: 1f);
            var run = new SimulationRunState(
                new Grid<SpeciesCell>(1, 1, (_, __) => SpeciesCell.Empty),
                SpeciesIds.Herbivore,
                seed: 7,
                durationSeconds: 1f);

            var runner = new SpeciesSimulationRunner(
                run,
                data,
                upgradeLoadout: new[] { snapshot });

            var result = SimulationRunResults.Create(run);

            Assert.That(runner.Run.UpgradeLoadout, Has.Count.EqualTo(1));
            Assert.That(result.UpgradeLoadout[0], Is.SameAs(snapshot));
        }

        static SpeciesUpgradeSnapshot CreateSnapshot(
            string id,
            SpeciesId targetSpecies,
            SpeciesUpgradeModifier modifier,
            IEnumerable<string> prerequisiteUpgradeIds = null,
            IEnumerable<string> excludedUpgradeIds = null)
        {
            return new SpeciesUpgradeSnapshot(
                id,
                id,
                "Test upgrade",
                targetSpecies,
                0,
                new[] { modifier },
                prerequisiteUpgradeIds,
                excludedUpgradeIds);
        }

        static SpeciesRules CreateRules(
            float movementSpeed,
            int metabolism,
            SpeciesAwarenessRules awareness)
        {
            return new SpeciesRules(
                movementSpeed: movementSpeed,
                movementPattern: EmptyPattern,
                attackPattern: EmptyPattern,
                attackAmount: 0,
                blockPattern: EmptyPattern,
                blockAmount: 0,
                dietPattern: EmptyPattern,
                dietTarget: null,
                reproductionPattern: EmptyPattern,
                reproductionNeighborCount: 0,
                reproductionChance: 0f,
                metabolism: metabolism,
                awareness: awareness);
        }
    }
}
