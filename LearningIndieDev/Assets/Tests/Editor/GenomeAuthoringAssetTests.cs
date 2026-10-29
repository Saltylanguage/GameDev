using NUnit.Framework;
using SaltyGame;
using System.Reflection;
using UnityEditor;

namespace SaltyGame.EditorTests
{
    [TestFixture]
    [Category("Authoring")]
    public sealed class GenomeAuthoringAssetTests
    {
        [Test]
        public void AuthoredGenomeModifiersUseRegistryAndFreezeValuesAcrossCatalogEdits()
        {
            var upgrade = UnityEngine.ScriptableObject.CreateInstance<GenomeUpgradeAsset>();
            var map = UnityEngine.ScriptableObject.CreateInstance<SpeciesGenomeMapAsset>();
            var providerObject = new UnityEngine.GameObject("Genome Stats Authoring Test");
            try
            {
                ConfigureUpgrade(upgrade, "hare.test", "Test Node");
                ConfigureMap(map, "hare", upgrade);
                var serialized = new SerializedObject(upgrade);
                var modifiers = serialized.FindProperty("modifiers");
                modifiers.arraySize = 1;
                modifiers.GetArrayElementAtIndex(0).FindPropertyRelative("attributeId").stringValue = SpeciesAttributeIds.DigestionEnergyBonus;
                modifiers.GetArrayElementAtIndex(0).FindPropertyRelative("signedValue").floatValue = 2;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                var provider = providerObject.AddComponent<GenomeCatalogProvider>();
                typeof(GenomeCatalogProvider).GetField("speciesMaps", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(provider, new[] { map });
                Assert.That(provider.TryCapture(out var reason), Is.True, reason);
                var selection = new GenomeSimulationSnapshot(new[]
                {
                    new SpeciesGenomeSnapshot(new SpeciesId("hare"), new[] { "hare.test" }),
                });
                var frozen = selection.Resolve(provider.Snapshot);
                modifiers.GetArrayElementAtIndex(0).FindPropertyRelative("signedValue").floatValue = 3;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                Assert.That(provider.TryCapture(out reason), Is.True, reason);
                var next = selection.Resolve(provider.Snapshot);
                Assert.That(next.Fingerprint, Is.Not.EqualTo(frozen.Fingerprint));
                Assert.That(frozen.GetSpeciesGenome(new SpeciesId("hare")).ResolvedNodes[0].Modifiers[0].SignedValue, Is.EqualTo(2));
                Assert.That(next.GetSpeciesGenome(new SpeciesId("hare")).ResolvedNodes[0].Modifiers[0].SignedValue, Is.EqualTo(3));

                modifiers.GetArrayElementAtIndex(0).FindPropertyRelative("attributeId").stringValue = "unknown.stat";
                serialized.ApplyModifiedPropertiesWithoutUndo();
                Assert.That(upgrade.TryCreateSnapshot(out _, out reason), Is.False);
                Assert.That(reason, Does.Contain("unknown.stat"));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(providerObject);
                UnityEngine.Object.DestroyImmediate(map);
                UnityEngine.Object.DestroyImmediate(upgrade);
            }
        }

        [Test]
        public void PocCatalogOffersOnlyImplementedEffectsAndKeepsUnfinishedAbilitiesAsPreviews()
        {
            var asset = AssetDatabase.LoadAssetAtPath<SpeciesGenomeMapAsset>(
                "Assets/Data/TestData/Genome/RabbitSelectionPOC/RabbitSelectionPOC.asset");
            Assert.That(asset, Is.Not.Null);
            var map = asset.CreateSnapshot();
            var scenario = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(
                "Assets/Data/ProductionData/CellularSimulation/Scenarios/ForestEdge.asset").CreateRuntimeData();
            var natural = scenario.SpeciesRules[GenomeContract.RabbitSpeciesId];
            var purchasable = 0;
            var previews = 0;
            foreach (var node in map.Nodes)
            {
                if (node.IsPurchasable)
                {
                    Assert.DoesNotThrow(() => node.Apply(natural), $"Offered node '{node.Id}' must have an implemented effect.");
                    purchasable++;
                }
                else
                {
                    Assert.That(node.CapacityPoints, Is.Zero);
                    Assert.That(node.Modifiers, Is.Empty);
                    Assert.Throws<System.InvalidOperationException>(() => node.Apply(natural));
                    previews++;
                }
            }
            Assert.That(purchasable, Is.EqualTo(8));
            Assert.That(previews, Is.EqualTo(1));
        }

        [Test]
        public void ProductionFertileDroppingsOfferIsAuthoredAsGardenerRoot()
        {
            var map = AssetDatabase.LoadAssetAtPath<SpeciesGenomeMapAsset>(
                "Assets/Data/Genome/HareGenome.asset");
            Assert.That(map, Is.Not.Null);
            Assert.That(map.TryCreateSnapshot(out var snapshot, out var reason), Is.True, reason);
            Assert.That(snapshot.Nodes, Has.Count.EqualTo(11));
            var node = snapshot.GetNode("fertile-droppings");
            Assert.That(node.Id, Is.EqualTo("fertile-droppings"));
            Assert.That(node.BranchName, Is.EqualTo("Gardener"));
            Assert.That(node.PrerequisiteNodeIds, Is.Empty);
            Assert.That(node.RabbitDataPrice, Is.EqualTo(1));
            Assert.That(node.CapacityPoints, Is.EqualTo(1));
            var gestation = snapshot.GetNode(GenomeContract.FastGestationNodeId);
            Assert.That(gestation, Is.Not.Null);
            Assert.That(gestation.BranchName, Is.EqualTo("Trailblazer"));
            Assert.That(gestation.RabbitDataPrice, Is.EqualTo(1));
            Assert.That(gestation.CapacityPoints, Is.EqualTo(2));
            Assert.That(gestation.MaximumRanks, Is.EqualTo(3));

            var minimumLitter = snapshot.GetNode("increased-minimum-litter");
            Assert.That(minimumLitter.BranchName, Is.EqualTo("Warren"));
            Assert.That(minimumLitter.PrerequisiteNodeIds, Is.Empty);
            Assert.That(minimumLitter.RabbitDataPrice, Is.EqualTo(1));
            Assert.That(minimumLitter.CapacityPoints, Is.EqualTo(1));
            Assert.That(minimumLitter.MaximumRanks, Is.EqualTo(1));
            var scenario = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(
                "Assets/Data/ProductionData/CellularSimulation/Scenarios/ForestEdge.asset").CreateRuntimeData();
            var natural = scenario.SpeciesRules[GenomeContract.RabbitSpeciesId];
            var upgraded = minimumLitter.Apply(natural);
            Assert.That(upgraded.LitterMinimum, Is.EqualTo(natural.LitterMinimum + 1));
            Assert.That(upgraded.LitterMaximum, Is.EqualTo(natural.LitterMaximum));

            var longStride = snapshot.GetNode("poc.hare.long-stride");
            var longVision = snapshot.GetNode("poc.hare.long-vision");
            var toughHide = snapshot.GetNode("poc.hare.tough-hide");
            var largerLitters = snapshot.GetNode("poc.hare.larger-litters");
            var efficientDigestion = snapshot.GetNode("poc.hare.efficient-digestion");
            var foundingHares = snapshot.GetNode("poc.hare.starting-population");
            var populationReserves = snapshot.GetNode("poc.hare.population-pool");
            var highCrowding = snapshot.GetNode("high-crowding");
            Assert.That(longStride.BranchName, Is.EqualTo("Trailblazer"));
            Assert.That(longVision.PrerequisiteNodeIds, Does.Contain(longStride.Id));
            Assert.That(toughHide.BranchName, Is.EqualTo("Warren"));
            Assert.That(largerLitters.PrerequisiteNodeIds, Does.Contain(toughHide.Id));
            Assert.That(efficientDigestion.PrerequisiteNodeIds, Does.Contain(node.Id));
            Assert.That(highCrowding.BranchName, Is.EqualTo("Trailblazer"));
            Assert.That(highCrowding.RabbitDataPrice, Is.EqualTo(1));
            Assert.That(highCrowding.CapacityPoints, Is.EqualTo(2));
            Assert.That(highCrowding.MaximumRanks, Is.EqualTo(1));
            Assert.That(highCrowding.Apply(natural).CrowdingEnergyReduction,
                Is.EqualTo(natural.CrowdingEnergyReduction + 0.1f));
            Assert.That(foundingHares, Is.Not.Null);
            Assert.That(populationReserves, Is.Not.Null);
            Assert.That(longStride.Apply(natural).MovementSpeed, Is.EqualTo(natural.MovementSpeed + 0.25f));
            Assert.That(longVision.Apply(natural).Awareness.VisionRange,
                Is.EqualTo(natural.Awareness.VisionRange + 2));
            Assert.That(toughHide.Apply(natural).BlockAmount, Is.EqualTo(natural.BlockAmount + 1));
            Assert.That(largerLitters.Apply(natural).LitterMaximum, Is.EqualTo(natural.LitterMaximum + 1));
            Assert.That(efficientDigestion.Apply(natural).DigestionEnergyBonus,
                Is.EqualTo(natural.DigestionEnergyBonus + 0.25f));
        }

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
