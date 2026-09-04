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
        const string TrailblazerPath = ProductionCatalogPath + "/Trailblazer_LongStride.asset";
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
        public void FirstCatalogFixturesMatchTheirDeclaredContract()
        {
            var trailblazer = LoadAsset(TrailblazerPath);
            var familialBond = LoadAsset(FamilialBondPath);

            var trailblazerSnapshot = CreateSnapshot(trailblazer);
            Assert.That(trailblazerSnapshot.Id, Is.EqualTo("trailblazer-long-stride"));
            Assert.That(trailblazerSnapshot.DisplayName, Is.EqualTo("Trailblazer: Long Stride"));
            Assert.That(trailblazerSnapshot.TargetSpecies.Value, Is.EqualTo("hare"));
            Assert.That(trailblazerSnapshot.Cost, Is.EqualTo(5));
            AssertModifier(trailblazerSnapshot, SpeciesAttributeIds.MovementSpeed, 0.5f);
            AssertModifier(trailblazerSnapshot, SpeciesAttributeIds.ReproductionNeighborCount, 1f);
            Assert.That(trailblazerSnapshot.Modifiers, Has.Count.EqualTo(2));

            var familialBondSnapshot = CreateSnapshot(familialBond);
            Assert.That(familialBondSnapshot.Id, Is.EqualTo("familial-bond-large-litters"));
            Assert.That(familialBondSnapshot.DisplayName, Is.EqualTo("Familial Bond: Large Litters"));
            Assert.That(familialBondSnapshot.TargetSpecies.Value, Is.EqualTo("hare"));
            Assert.That(familialBondSnapshot.Cost, Is.EqualTo(10));
            AssertModifier(familialBondSnapshot, SpeciesAttributeIds.CrowdingTolerance, 3f);
            Assert.That(familialBondSnapshot.Modifiers, Has.Count.EqualTo(1));

            Assert.That(
                CreateSnapshot(trailblazer).Fingerprint,
                Is.EqualTo(trailblazerSnapshot.Fingerprint),
                "Resolving an unchanged asset must produce a deterministic fingerprint.");
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
            var modifier = snapshot.Modifiers.SingleOrDefault(entry => entry.AttributeId == attributeId);
            Assert.That(modifier.AttributeId, Is.EqualTo(attributeId));
            Assert.That(modifier.SignedValue, Is.EqualTo(signedValue).Within(0.0001f));
        }
    }
}
