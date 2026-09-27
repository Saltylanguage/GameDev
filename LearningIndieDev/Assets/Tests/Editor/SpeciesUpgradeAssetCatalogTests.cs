using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SaltyGame;
using UnityEditor;

namespace SaltyGame.EditorTests
{
    [TestFixture]
    [Category("Authoring")]
    public sealed class SpeciesUpgradeAssetCatalogTests
    {
        const string ProductionCatalogPath = "Assets/Data/ProductionData/CellularSimulation/Upgrades/Production";
        const string LargerMinimumLitterPath = ProductionCatalogPath + "/LargerMinimumLitter.asset";
        const string LargerMaximumLitterPath = ProductionCatalogPath + "/LargerMaximumLitter.asset";
        const string HareSpeciesPath = "Assets/Data/ProductionData/CellularSimulation/Species/hare.asset";

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
                LargerMinimumLitterPath,
                SpeciesUpgradeCatalog.LargerMinimumLitterId,
                "Hare: Larger Minimum Litter",
                5,
                canApplyAfterRunStart: true,
                new ExpectedModifier(SpeciesAttributeIds.LitterMinimum, 1f));
            AssertFixture(
                LargerMaximumLitterPath,
                SpeciesUpgradeCatalog.LargerMaximumLitterId,
                "Hare: Larger Maximum Litter",
                5,
                canApplyAfterRunStart: true,
                new ExpectedModifier(SpeciesAttributeIds.LitterMaximum, 1f));

            Assert.That(
                CreateSnapshot(LoadAsset(LargerMinimumLitterPath)).Fingerprint,
                Is.EqualTo(CreateSnapshot(LoadAsset(LargerMinimumLitterPath)).Fingerprint),
                "Resolving an unchanged asset must produce a deterministic fingerprint.");
        }

        [Test]
        public void ProductionHareUsesDoubledEnergyAndLitterMatingRules()
        {
            var hareAsset = AssetDatabase.LoadAssetAtPath<SpeciesDefinitionAsset>(HareSpeciesPath);
            Assert.That(hareAsset, Is.Not.Null);

            var rules = hareAsset.CreateRules();
            Assert.That(rules.StartingEnergy, Is.EqualTo(12));
            Assert.That(rules.MaximumEnergy, Is.EqualTo(48));
            Assert.That(rules.EnergyValue, Is.EqualTo(96));
            Assert.That(rules.ReproductionChance, Is.EqualTo(1f));
            Assert.That(rules.MatingEnergyThresholdFraction, Is.EqualTo(0.5f));
            Assert.That(rules.MatingEnergyCostFraction, Is.EqualTo(0.5f));
            Assert.That(rules.MaxReproductionGroupSize, Is.EqualTo(7));
            Assert.That(rules.DistributeMatingEnergyToOffspring, Is.True);
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
