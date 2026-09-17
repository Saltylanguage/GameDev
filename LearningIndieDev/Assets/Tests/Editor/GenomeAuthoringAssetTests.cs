using NUnit.Framework;
using SaltyGame;
using System.Reflection;
using UnityEditor;

namespace SaltyGame.EditorTests
{
    [TestFixture]
    public sealed class GenomeAuthoringAssetTests
    {
        const string MapPath = "Assets/Data/CellularSimulation/Genome/HareGenome.asset";
        const string UpgradePath = "Assets/Data/CellularSimulation/Genome/HareGenome/GuardedBurrow.asset";

        [Test]
        public void HareGenomeAuthoringAssetsResolveToTheExpectedImmutableCatalog()
        {
            var mapAsset = AssetDatabase.LoadAssetAtPath<SpeciesGenomeMapAsset>(MapPath);
            var upgradeAsset = AssetDatabase.LoadAssetAtPath<GenomeUpgradeAsset>(UpgradePath);

            Assert.That(mapAsset, Is.Not.Null, $"Expected Genome map fixture at '{MapPath}'.");
            Assert.That(upgradeAsset, Is.Not.Null, $"Expected Genome upgrade fixture at '{UpgradePath}'.");
            Assert.That(mapAsset.TryCreateSnapshot(out var map, out var validationMessage), Is.True, validationMessage);
            Assert.That(map.SpeciesId.Value, Is.EqualTo("hare"));
            Assert.That(map.Nodes, Has.Count.EqualTo(5));
            Assert.That(map.Nodes[0].Id, Is.EqualTo("hare.guarded-burrow"));
            Assert.That(map.Nodes[0].DisplayName, Is.EqualTo("Guarded Burrow"));
            Assert.That(map.Nodes[0].PrerequisiteNodeIds, Is.Empty);
            Assert.That(map.Nodes[1].PrerequisiteNodeIds, Is.EqualTo(new[] { "hare.guarded-burrow" }));
            Assert.That(map.Nodes[3].PrerequisiteNodeIds,
                Is.EqualTo(new[] { "hare.swift-digging", "hare.keen-hearing" }));
            Assert.That(upgradeAsset.TryCreateSnapshot(out var upgrade, out validationMessage), Is.True, validationMessage);
            Assert.That(upgrade, Is.Not.Null);
            Assert.That(upgrade.Id, Is.EqualTo(map.Nodes[0].Id));
            Assert.That(upgrade.Fingerprint, Is.EqualTo(map.Nodes[0].Fingerprint));
        }

        [Test]
        public void GenomeCatalogProviderCapturesAssetFreeSnapshot()
        {
            var providerObject = new UnityEngine.GameObject("Genome Catalog Provider Test");
            try
            {
                var provider = providerObject.AddComponent<GenomeCatalogProvider>();
                var mapAsset = AssetDatabase.LoadAssetAtPath<SpeciesGenomeMapAsset>(MapPath);
                var serializedProvider = new SerializedObject(provider);
                serializedProvider.FindProperty("speciesMaps").arraySize = 1;
                serializedProvider.FindProperty("speciesMaps").GetArrayElementAtIndex(0).objectReferenceValue = mapAsset;
                serializedProvider.ApplyModifiedPropertiesWithoutUndo();

                Assert.That(provider.TryCapture(out var validationMessage), Is.True, validationMessage);
                Assert.That(provider.Snapshot.SpeciesMaps, Has.Count.EqualTo(1));
                Assert.That(provider.Snapshot.GetSpeciesMap(new SpeciesId("hare")).Nodes, Has.Count.EqualTo(5));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(providerObject);
            }
        }

        [Test]
        public void GenomeCatalogProviderRefreshesWhenAnUpgradeAssetChanges()
        {
            var upgrade = UnityEngine.ScriptableObject.CreateInstance<GenomeUpgradeAsset>();
            var secondUpgrade = UnityEngine.ScriptableObject.CreateInstance<GenomeUpgradeAsset>();
            var map = UnityEngine.ScriptableObject.CreateInstance<SpeciesGenomeMapAsset>();
            var providerObject = new UnityEngine.GameObject("Genome Catalog Change Test");
            providerObject.SetActive(false);
            try
            {
                ConfigureUpgrade(upgrade, "hare.dynamic", "Dynamic Node");
                ConfigureUpgrade(secondUpgrade, "hare.second", "Second Node");
                ConfigureMap(map, "hare", upgrade);

                var provider = providerObject.AddComponent<GenomeCatalogProvider>();
                typeof(GenomeCatalogProvider)
                    .GetField("speciesMaps", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(provider, new[] { map });
                var changeCount = 0;
                provider.SnapshotChanged += _ => changeCount++;

                providerObject.SetActive(true);

                Assert.That(changeCount, Is.EqualTo(1));
                var originalFingerprint = provider.Snapshot.Fingerprint;
                Assert.That(provider.Snapshot.GetSpeciesMap(new SpeciesId("hare")).Nodes[0].DisplayName,
                    Is.EqualTo("Dynamic Node"));

                var upgradeSerialized = new SerializedObject(upgrade);
                upgradeSerialized.FindProperty("displayName").stringValue = "Updated Node";
                upgradeSerialized.ApplyModifiedPropertiesWithoutUndo();

                Assert.That(changeCount, Is.EqualTo(2));
                Assert.That(provider.Snapshot.Fingerprint, Is.Not.EqualTo(originalFingerprint));
                Assert.That(provider.Snapshot.GetSpeciesMap(new SpeciesId("hare")).Nodes[0].DisplayName,
                    Is.EqualTo("Updated Node"));

                var mapSerialized = new SerializedObject(map);
                mapSerialized.FindProperty("upgrades").arraySize = 2;
                mapSerialized.FindProperty("upgrades").GetArrayElementAtIndex(1).objectReferenceValue = secondUpgrade;
                mapSerialized.ApplyModifiedPropertiesWithoutUndo();

                Assert.That(changeCount, Is.EqualTo(3));
                Assert.That(provider.Snapshot.GetSpeciesMap(new SpeciesId("hare")).Nodes, Has.Count.EqualTo(2));
                Assert.That(provider.TryCapture(out var unchangedValidationMessage), Is.True, unchangedValidationMessage);
                Assert.That(changeCount, Is.EqualTo(3));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(providerObject);
                UnityEngine.Object.DestroyImmediate(map);
                UnityEngine.Object.DestroyImmediate(upgrade);
                UnityEngine.Object.DestroyImmediate(secondUpgrade);
            }
        }

        static void ConfigureUpgrade(GenomeUpgradeAsset upgrade, string id, string displayName)
        {
            var serializedUpgrade = new SerializedObject(upgrade);
            serializedUpgrade.FindProperty("upgradeId").stringValue = id;
            serializedUpgrade.FindProperty("displayName").stringValue = displayName;
            serializedUpgrade.FindProperty("description").stringValue = "Test genome node.";
            serializedUpgrade.FindProperty("targetSpeciesId").stringValue = "hare";
            serializedUpgrade.FindProperty("prerequisiteNodeIds").arraySize = 0;
            serializedUpgrade.ApplyModifiedPropertiesWithoutUndo();
        }

        static void ConfigureMap(
            SpeciesGenomeMapAsset map,
            string speciesId,
            GenomeUpgradeAsset upgrade)
        {
            var serializedMap = new SerializedObject(map);
            serializedMap.FindProperty("speciesId").stringValue = speciesId;
            serializedMap.FindProperty("upgrades").arraySize = 1;
            serializedMap.FindProperty("upgrades").GetArrayElementAtIndex(0).objectReferenceValue = upgrade;
            serializedMap.ApplyModifiedPropertiesWithoutUndo();
        }

    }
}
