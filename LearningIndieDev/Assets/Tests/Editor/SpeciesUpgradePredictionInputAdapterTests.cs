using NUnit.Framework;
using SaltyGame;
using SaltyGame.EditorTools;
using UnityEditor;

namespace SaltyGame.EditorTests
{
    [TestFixture]
    public sealed class SpeciesUpgradePredictionInputAdapterTests
    {
        [Test]
        public void ResolvePreservesRequestedOrderAndAuthoredValues()
        {
            var snapshots = SpeciesUpgradePredictionInputAdapter.Resolve(new[]
            {
                "trailblazer-far-sight",
                "trailblazer-long-stride",
            });

            Assert.That(snapshots, Has.Length.EqualTo(2));
            Assert.That(snapshots[0].Id, Is.EqualTo("trailblazer-far-sight"));
            Assert.That(snapshots[0].TargetSpecies.Value, Is.EqualTo("hare"));
            Assert.That(snapshots[0].Modifiers[0].AttributeId, Is.EqualTo(SpeciesAttributeIds.VisionRange));
            Assert.That(snapshots[0].Modifiers[0].SignedValue, Is.EqualTo(1f));
            Assert.That(snapshots[1].Id, Is.EqualTo("trailblazer-long-stride"));
        }

        [Test]
        public void CreateInputSerializesContractValuesAndOrderedIdentity()
        {
            var snapshots = SpeciesUpgradePredictionInputAdapter.Resolve(new[]
            {
                "trailblazer-long-stride",
                "warren-guarded-burrow",
            });
            var input = SpeciesUpgradePredictionInputAdapter.CreateInput(snapshots);
            var json = SpeciesUpgradePredictionInputAdapter.Serialize(snapshots);

            Assert.That(input.schemaVersion, Is.EqualTo(SpeciesUpgradePredictionInputAdapter.SchemaVersion));
            Assert.That(input.contractVersion, Is.EqualTo(SpeciesUpgradeSnapshot.ContractVersion));
            Assert.That(input.registryFingerprint, Is.EqualTo(SpeciesAttributeRegistry.Fingerprint));
            Assert.That(input.orderedUpgradeIds, Is.EqualTo(new[]
            {
                "trailblazer-long-stride",
                "warren-guarded-burrow",
            }));
            Assert.That(input.upgrades[0].order, Is.EqualTo(0));
            Assert.That(input.upgrades[1].order, Is.EqualTo(1));
            Assert.That(input.upgrades[0].fingerprint, Is.EqualTo(snapshots[0].Fingerprint));
            Assert.That(input.upgrades[1].modifiers, Has.Length.EqualTo(2));
            StringAssert.Contains("orderedLoadoutFingerprint", json);
            StringAssert.Contains("movement.speed", json);
            StringAssert.Contains("-0.25", json);
        }

        [Test]
        public void CreateInputFromAssetsUsesTheSameResolvedSnapshots()
        {
            var assets = new[]
            {
                AssetDatabase.LoadAssetAtPath<SpeciesUpgradeAsset>(
                    "Assets/Data/CellularSimulation/Upgrades/Production/Trailblazer_LongStride.asset"),
                AssetDatabase.LoadAssetAtPath<SpeciesUpgradeAsset>(
                    "Assets/Data/CellularSimulation/Upgrades/Production/Warren_GuardedBurrow.asset"),
            };

            var input = SpeciesUpgradePredictionInputAdapter.CreateInputFromAssets(assets);

            Assert.That(input.orderedUpgradeIds, Is.EqualTo(new[]
            {
                "trailblazer-long-stride",
                "warren-guarded-burrow",
            }));
            Assert.That(input.upgrades[0].fingerprint, Is.EqualTo(assets[0].CreateSnapshot().Fingerprint));
        }

        [Test]
        public void Ex007ResearchFixturesPreserveLegacyInterventionValues()
        {
            var assets = new[]
            {
                AssetDatabase.LoadAssetAtPath<SpeciesUpgradeAsset>(
                    "Assets/Data/CellularSimulation/Upgrades/Research/EX-007/FasterMovement.asset"),
                AssetDatabase.LoadAssetAtPath<SpeciesUpgradeAsset>(
                    "Assets/Data/CellularSimulation/Upgrades/Research/EX-007/CrowdingTolerance.asset"),
            };

            var input = SpeciesUpgradePredictionInputAdapter.CreateInputFromAssets(assets);

            Assert.That(input.orderedUpgradeIds, Is.EqualTo(new[]
            {
                "faster-movement",
                "crowding-tolerance",
            }));
            Assert.That(input.upgrades[0].modifiers[0].attributeId, Is.EqualTo(SpeciesAttributeIds.MovementSpeed));
            Assert.That(input.upgrades[0].modifiers[0].signedValue, Is.EqualTo(0.5f));
            Assert.That(input.upgrades[1].modifiers[0].attributeId, Is.EqualTo(SpeciesAttributeIds.CrowdingTolerance));
            Assert.That(input.upgrades[1].modifiers[0].signedValue, Is.EqualTo(1f));
        }

        [Test]
        public void ResolveCanLoadExplicitResearchFixtureCatalog()
        {
            var snapshots = SpeciesUpgradePredictionInputAdapter.Resolve(
                new[] { "faster-movement", "crowding-tolerance" },
                "Assets/Data/CellularSimulation/Upgrades/Research/EX-007");
            var input = SpeciesUpgradePredictionInputAdapter.CreateInput(
                snapshots,
                "Assets/Data/CellularSimulation/Upgrades/Research/EX-007");

            Assert.That(snapshots, Has.Length.EqualTo(2));
            Assert.That(snapshots[0].Id, Is.EqualTo("faster-movement"));
            Assert.That(snapshots[1].Id, Is.EqualTo("crowding-tolerance"));
            Assert.That(input.sourceCatalogPath, Is.EqualTo(
                "Assets/Data/CellularSimulation/Upgrades/Research/EX-007"));
        }

        [Test]
        public void InvalidCatalogPathIsRejected()
        {
            Assert.That(
                SpeciesUpgradePredictionInputAdapter.TryResolve(
                    new[] { "faster-movement" },
                    "Packages/ResearchFixtures",
                    out _,
                    out var validationMessage),
                Is.False);
            StringAssert.Contains("Assets", validationMessage);
        }

        [Test]
        public void OrderedLoadoutFingerprintChangesWhenOrderChanges()
        {
            var forward = SpeciesUpgradePredictionInputAdapter.Resolve(new[]
            {
                "trailblazer-long-stride",
                "warren-guarded-burrow",
            });
            var reverse = SpeciesUpgradePredictionInputAdapter.Resolve(new[]
            {
                "warren-guarded-burrow",
                "trailblazer-long-stride",
            });

            Assert.That(
                SpeciesUpgradeLoadoutFingerprint.Create(forward),
                Is.Not.EqualTo(SpeciesUpgradeLoadoutFingerprint.Create(reverse)));
        }

        [Test]
        public void UnknownAndDuplicateIdsAreRejected()
        {
            Assert.That(
                SpeciesUpgradePredictionInputAdapter.TryResolve(
                    new[] { "not-a-production-upgrade" },
                    out _,
                    out var unknownMessage),
                Is.False);
            StringAssert.Contains("not-a-production-upgrade", unknownMessage);

            Assert.That(
                SpeciesUpgradePredictionInputAdapter.TryResolve(
                    new[] { "trailblazer-long-stride", "trailblazer-long-stride" },
                    out _,
                    out var duplicateMessage),
                Is.False);
            StringAssert.Contains("may only appear once", duplicateMessage);
        }
    }
}
