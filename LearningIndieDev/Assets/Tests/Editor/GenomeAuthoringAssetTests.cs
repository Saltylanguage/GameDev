using NUnit.Framework;
using SaltyGame;
using System.Reflection;
using UnityEditor;

namespace SaltyGame.EditorTests
{
    [TestFixture]
    public sealed class GenomeAuthoringAssetTests
    {
        [Test]
        public void GenomeCatalogProviderCapturesAssetFreeSnapshot()
        {
            var upgrade = UnityEngine.ScriptableObject.CreateInstance<GenomeUpgradeAsset>();
            var map = UnityEngine.ScriptableObject.CreateInstance<SpeciesGenomeMapAsset>();
            var providerObject = new UnityEngine.GameObject("Genome Catalog Provider Test");
            try
            {
                ConfigureUpgrade(upgrade, "hare.test", "Test Node");
                ConfigureMap(map, "hare", upgrade);
                var provider = providerObject.AddComponent<GenomeCatalogProvider>();
                var serializedProvider = new SerializedObject(provider);
                serializedProvider.FindProperty("speciesMaps").arraySize = 1;
                serializedProvider.FindProperty("speciesMaps").GetArrayElementAtIndex(0).objectReferenceValue = map;
                serializedProvider.ApplyModifiedPropertiesWithoutUndo();

                Assert.That(provider.TryCapture(out var validationMessage), Is.True, validationMessage);
                Assert.That(provider.Snapshot.SpeciesMaps, Has.Count.EqualTo(1));
                Assert.That(provider.Snapshot.GetSpeciesMap(new SpeciesId("hare")).Nodes, Has.Count.EqualTo(1));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(providerObject);
                UnityEngine.Object.DestroyImmediate(map);
                UnityEngine.Object.DestroyImmediate(upgrade);
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
                providerObject.SetActive(true);
                typeof(GenomeCatalogProvider)
                    .GetMethod("OnEnable", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(provider, null);
                Assert.That(provider.TryCapture(out var initialValidationMessage), Is.True, initialValidationMessage);
                var originalFingerprint = provider.Snapshot.Fingerprint;
                var changeCount = 0;
                provider.SnapshotChanged += _ => changeCount++;
                Assert.That(changeCount, Is.Zero);
                Assert.That(provider.Snapshot.GetSpeciesMap(new SpeciesId("hare")).Nodes[0].DisplayName,
                    Is.EqualTo("Dynamic Node"));

                var upgradeSerialized = new SerializedObject(upgrade);
                upgradeSerialized.FindProperty("displayName").stringValue = "Updated Node";
                upgradeSerialized.ApplyModifiedPropertiesWithoutUndo();
                typeof(GenomeUpgradeAsset)
                    .GetMethod("OnValidate", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(upgrade, null);

                Assert.That(changeCount, Is.EqualTo(1));
                Assert.That(provider.Snapshot.Fingerprint, Is.Not.EqualTo(originalFingerprint));
                Assert.That(provider.Snapshot.GetSpeciesMap(new SpeciesId("hare")).Nodes[0].DisplayName,
                    Is.EqualTo("Updated Node"));

                var mapSerialized = new SerializedObject(map);
                mapSerialized.FindProperty("upgrades").arraySize = 2;
                mapSerialized.FindProperty("upgrades").GetArrayElementAtIndex(1).objectReferenceValue = secondUpgrade;
                mapSerialized.ApplyModifiedPropertiesWithoutUndo();
                typeof(SpeciesGenomeMapAsset)
                    .GetMethod("OnValidate", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(map, null);

                Assert.That(changeCount, Is.EqualTo(2));
                Assert.That(provider.Snapshot.GetSpeciesMap(new SpeciesId("hare")).Nodes, Has.Count.EqualTo(2));
                Assert.That(provider.TryCapture(out var unchangedValidationMessage), Is.True, unchangedValidationMessage);
                Assert.That(changeCount, Is.EqualTo(2));
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
