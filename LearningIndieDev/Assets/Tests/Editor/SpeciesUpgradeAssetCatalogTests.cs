using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SaltyGame;
using UnityEditor;

namespace SaltyGame.EditorTests
{
    [TestFixture]
    public sealed class SpeciesUpgradeAssetCatalogTests
    {
        const string ProductionCatalogPath = "Assets/Data/CellularSimulation/Upgrades/Production";
        const string CarefulSowingPath = ProductionCatalogPath + "/Gardeners_CarefulSowing.asset";
        const string SeedPouchesPath = ProductionCatalogPath + "/Gardeners_SeedPouches.asset";
        const string FarSightPath = ProductionCatalogPath + "/Trailblazer_FarSight.asset";
        const string TrailblazerPath = ProductionCatalogPath + "/Trailblazer_LongStride.asset";
        const string GuardedBurrowPath = ProductionCatalogPath + "/Warren_GuardedBurrow.asset";
        const string RoomToBreedPath = ProductionCatalogPath + "/Warren_RoomToBreed.asset";
        const string FamilialBondPath = ProductionCatalogPath + "/FamilialBond_LargeLitters.asset";

        [Test]
        public void EveryProductionUpgradeAssetResolvesToAValidSnapshot()
        {
            var assets = LoadProductionAssets();

            Assert.That(assets, Is.Not.Empty, "The production upgrade catalog has no ScriptableObject assets.");
            foreach (var asset in assets)
            {
                Assert.That(
                    asset.TryCreateSnapshot(out var snapshot, out var validationMessage),
                    Is.True,
                    $"Production upgrade '{asset.name}' is invalid: {validationMessage}");
                Assert.That(snapshot, Is.Not.Null);
                Assert.That(snapshot.RegistryFingerprint, Is.Not.Empty);
                Assert.That(snapshot.Fingerprint, Is.Not.Empty);
            }
        }

        [Test]
        public void ProductionUpgradeStableIdsAreUnique()
        {
            var assets = LoadProductionAssets();
            var ids = new HashSet<string>(StringComparer.Ordinal);

            foreach (var asset in assets)
            {
                Assert.That(asset.UpgradeId, Is.Not.Null.And.Not.Empty, $"Asset '{asset.name}' has no stable ID.");
                Assert.That(ids.Add(asset.UpgradeId), Is.True, $"Duplicate production upgrade ID '{asset.UpgradeId}'.");
            }
        }

        [Test]
        public void ProductionCatalogFixturesMatchTheirAcceptanceMatrix()
        {
            AssertFixture(
                TrailblazerPath,
                "trailblazer-long-stride",
                "Trailblazer: Long Stride",
                5,
                canApplyAfterRunStart: true,
                new ExpectedModifier(SpeciesAttributeIds.MovementSpeed, 0.5f),
                new ExpectedModifier(SpeciesAttributeIds.ReproductionNeighborCount, 1f));
            AssertFixture(
                FarSightPath,
                "trailblazer-far-sight",
                "Trailblazer: Far Sight",
                8,
                canApplyAfterRunStart: true,
                new ExpectedModifier(SpeciesAttributeIds.VisionRange, 1f),
                new ExpectedModifier(SpeciesAttributeIds.Metabolism, 1f));
            AssertFixture(
                GuardedBurrowPath,
                "warren-guarded-burrow",
                "Warren: Guarded Burrow",
                7,
                canApplyAfterRunStart: true,
                new ExpectedModifier(SpeciesAttributeIds.BlockAmount, 2f),
                new ExpectedModifier(SpeciesAttributeIds.MovementSpeed, -0.25f));
            AssertFixture(
                RoomToBreedPath,
                "warren-room-to-breed",
                "Warren: Room to Breed",
                9,
                canApplyAfterRunStart: true,
                new ExpectedModifier(SpeciesAttributeIds.MaxReproductionGroupSize, 1f),
                new ExpectedModifier(SpeciesAttributeIds.CrowdingEnergyPenalty, -1f),
                new ExpectedModifier(SpeciesAttributeIds.Metabolism, 1f));
            AssertFixture(
                SeedPouchesPath,
                "gardeners-seed-pouches",
                "Gardeners: Seed Pouches",
                6,
                canApplyAfterRunStart: false,
                new ExpectedModifier(SpeciesAttributeIds.StartingFoodReserve, 2f),
                new ExpectedModifier(SpeciesAttributeIds.StartingEnergy, -2f));
            AssertFixture(
                CarefulSowingPath,
                "gardeners-careful-sowing",
                "Gardeners: Careful Sowing",
                8,
                canApplyAfterRunStart: true,
                new ExpectedModifier(SpeciesAttributeIds.SeedDropChance, 0.1f),
                new ExpectedModifier(SpeciesAttributeIds.MovementSpeed, -0.25f));
            AssertFixture(
                FamilialBondPath,
                "familial-bond-large-litters",
                "Familial Bond: Large Litters",
                10,
                canApplyAfterRunStart: true,
                new ExpectedModifier(SpeciesAttributeIds.CrowdingTolerance, 3f));

            Assert.That(
                CreateSnapshot(LoadAsset(TrailblazerPath)).Fingerprint,
                Is.EqualTo(CreateSnapshot(LoadAsset(TrailblazerPath)).Fingerprint),
                "Resolving an unchanged asset must produce a deterministic fingerprint.");
        }

        sealed class ExpectedModifier
        {
            public ExpectedModifier(string attributeId, float signedValue)
            {
                AttributeId = attributeId;
                SignedValue = signedValue;
            }

            public string AttributeId { get; }
            public float SignedValue { get; }
        }

        static void AssertFixture(
            string path,
            string id,
            string displayName,
            int cost,
            bool canApplyAfterRunStart,
            params ExpectedModifier[] expectedModifiers)
        {
            var snapshot = CreateSnapshot(LoadAsset(path));
            Assert.That(snapshot.Id, Is.EqualTo(id));
            Assert.That(snapshot.DisplayName, Is.EqualTo(displayName));
            Assert.That(snapshot.TargetSpecies.Value, Is.EqualTo("hare"));
            Assert.That(snapshot.Cost, Is.EqualTo(cost));
            Assert.That(snapshot.Scope, Is.EqualTo(SpeciesUpgradeScope.PerRun));
            Assert.That(snapshot.PrerequisiteUpgradeIds, Is.Empty);
            Assert.That(snapshot.ExcludedUpgradeIds, Is.Empty);
            Assert.That(snapshot.CanApplyAfterRunStart, Is.EqualTo(canApplyAfterRunStart));
            Assert.That(snapshot.Modifiers, Has.Count.EqualTo(expectedModifiers.Length));

            foreach (var expected in expectedModifiers)
            {
                AssertModifier(snapshot, expected.AttributeId, expected.SignedValue);
            }
        }

        static SpeciesUpgradeSnapshot CreateSnapshot(SpeciesUpgradeAsset asset)
        {
            Assert.That(
                asset.TryCreateSnapshot(out var snapshot, out var validationMessage),
                Is.True,
                $"Asset '{asset.name}' is invalid: {validationMessage}");
            return snapshot;
        }

        static SpeciesUpgradeAsset LoadAsset(string path)
        {
            var asset = AssetDatabase.LoadAssetAtPath<SpeciesUpgradeAsset>(path);
            Assert.That(asset, Is.Not.Null, $"Expected upgrade fixture at '{path}'.");
            return asset;
        }

        static IReadOnlyList<SpeciesUpgradeAsset> LoadProductionAssets()
        {
            var assets = new List<SpeciesUpgradeAsset>();
            foreach (var guid in AssetDatabase.FindAssets("t:SpeciesUpgradeAsset", new[] { ProductionCatalogPath }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<SpeciesUpgradeAsset>(path);
                Assert.That(asset, Is.Not.Null, $"Could not load upgrade asset at '{path}'.");
                assets.Add(asset);
            }

            return assets
                .OrderBy(asset => asset.UpgradeId, StringComparer.Ordinal)
                .ToArray();
        }

        static void AssertModifier(SpeciesUpgradeSnapshot snapshot, string attributeId, float signedValue)
        {
            var matches = snapshot.Modifiers
                .Where(entry => entry.AttributeId == attributeId)
                .ToArray();
            Assert.That(matches, Has.Length.EqualTo(1), $"Expected modifier '{attributeId}' was not authored exactly once.");
            var modifier = matches[0];
            Assert.That(modifier.SignedValue, Is.EqualTo(signedValue).Within(0.0001f));
        }
    }
}
