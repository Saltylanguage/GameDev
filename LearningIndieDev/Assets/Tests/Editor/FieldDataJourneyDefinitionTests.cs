using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace SaltyGame.EditorTests
{
    public sealed class FieldDataJourneyDefinitionTests
    {
        const string DefinitionPath = "Assets/Data/TestData/FieldDataStarterDefinition.asset";

        [Test]
        public void HareRewardPoolUsesOnlyTheSixHareSkillsAndPreservesAuthoredSkillDefinitions()
        {
            var hare = new SpeciesId("hare");
            var fallback = SpeciesUpgradeCatalog.ResolveFieldDataOrdinaryRewardPool(hare, null);
            Assert.That(fallback.Select(upgrade => upgrade.Id), Is.EqualTo(new[]
            {
                SpeciesUpgradeCatalog.ToughHideId, SpeciesUpgradeCatalog.EfficientDigestionId,
                SpeciesUpgradeCatalog.SeedDispersalId, SpeciesUpgradeCatalog.CrowdingToleranceId,
                SpeciesUpgradeCatalog.ReproductiveDriveId, SpeciesUpgradeCatalog.KeenSensesId,
            }));
            Assert.That(fallback.All(upgrade => upgrade.TargetSpecies == hare), Is.True);
            var authored = new[]
            {
                fallback[2], fallback[0],
                SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.StrongerAttackId).CreateSnapshot(hare),
            };
            var resolved = SpeciesUpgradeCatalog.ResolveFieldDataOrdinaryRewardPool(hare, authored);
            Assert.That(resolved.Select(upgrade => upgrade.Id), Is.EqualTo(fallback.Select(upgrade => upgrade.Id)));
            Assert.That(resolved[2], Is.SameAs(authored[0]));
            Assert.That(resolved[0], Is.SameAs(authored[1]));
            Assert.That(SpeciesUpgradeCatalog.ResolveFieldDataOrdinaryRewardPool(hare,
                new[] { SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.FasterMovementId).CreateSnapshot(new SpeciesId("fox")) })
                .Select(upgrade => upgrade.Id), Is.EqualTo(fallback.Select(upgrade => upgrade.Id)));
        }

        [Test]
        public void HareReproductiveDriveCannotExceedTheProbabilityBounds()
        {
            var rules = SpeciesRuleDefaults.Create()[SpeciesIds.Herbivore];
            var capped = SpeciesAttributeRegistry.Apply(rules, new SpeciesUpgradeModifier(
                SpeciesAttributeIds.ReproductionChance, 1f - rules.ReproductionChance));
            var upgrade = SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.ReproductiveDriveId);
            Assert.That(upgrade.Apply(capped).ReproductionChance, Is.EqualTo(1f));
            Assert.That(upgrade.CreateSnapshot(new SpeciesId("hare")).Apply(capped).ReproductionChance, Is.EqualTo(1f));
            Assert.That(SpeciesAttributeRegistry.Apply(capped, new SpeciesUpgradeModifier(
                SpeciesAttributeIds.ReproductionChance, -2f)).ReproductionChance, Is.Zero);
        }

        [Test]
        public void StarterDefinitionResolvesImmutableApprovedRules()
        {
            var asset = AssetDatabase.LoadAssetAtPath<FieldDataJourneyDefinitionAsset>(DefinitionPath);
            Assert.That(asset, Is.Not.Null);
            Assert.That(asset.TryResolve(out var snapshot, out var error), Is.True, error);
            Assert.That(snapshot.Version, Is.EqualTo(FieldDataJourneyDefinition.CurrentVersion));
            Assert.That(snapshot.Fingerprint, Has.Length.EqualTo(64));
            Assert.That(snapshot.ScenarioId, Is.EqualTo("ForestEdge"));
            Assert.That(snapshot.SelectedSpeciesId, Is.EqualTo("hare"));
            Assert.That(snapshot.InitialFieldData, Is.Zero);
            Assert.That(snapshot.Route.Count, Is.EqualTo(5));
            Assert.That(snapshot.Route.Select(node => node.Kind), Is.EqualTo(new[]
            {
                JourneyNodeKind.Simulation, JourneyNodeKind.Shop, JourneyNodeKind.Simulation,
                JourneyNodeKind.Shop, JourneyNodeKind.Simulation,
            }));
            Assert.That(snapshot.RoundLength, Is.EqualTo(100));
            Assert.That(snapshot.RoundsPerNode, Is.EqualTo(6));
            Assert.That(snapshot.NodeClearFieldData, Is.EqualTo(10));
            Assert.That(snapshot.FreeChoicesPerRewardEvent, Is.EqualTo(3));
            Assert.That(snapshot.FirstRoundChoice, Is.EqualTo(1));
            Assert.That(snapshot.LastRoundChoice, Is.EqualTo(5));
            Assert.That(snapshot.NodeRewardChoiceCount, Is.EqualTo(2));
            Assert.That(snapshot.OffersPerShop, Is.EqualTo(5));
            Assert.That(FieldDataJourneyDefinition.SpeciesOfferCost, Is.EqualTo(30));
            Assert.That(snapshot.SpeciesOfferCopyLimit, Is.EqualTo(3));
            Assert.That(snapshot.PopulationControls.Count, Is.EqualTo(6));
            Assert.That(snapshot.IncomeOffers.Count, Is.EqualTo(3));
            Assert.That(snapshot.SignedSpeciesOffers, Has.Count.EqualTo(12));
            Assert.That(snapshot.PopulationControls.Select(control => control.Amount),
                Is.EqualTo(new[] { 1, -1, 1, -1, 5, -5 }));
            Assert.That(snapshot.PopulationControls.Select(control => control.Cost),
                Is.All.EqualTo(10));
            Assert.That(snapshot.IncomeOffers[0].Sources,
                Is.EqualTo(FieldDataIncomeSource.Shop));
            Assert.That(snapshot.IncomeOffers[0].ShopCost, Is.EqualTo(20));
            Assert.That(snapshot.IncomeOffers[1].Sources,
                Is.EqualTo(FieldDataIncomeSource.Shop));
            Assert.That(snapshot.IncomeOffers[1].ShopCost, Is.EqualTo(30));
            Assert.That(snapshot.IncomeOffers[1].FirstShopVisit, Is.EqualTo(1));
            Assert.That(snapshot.IncomeOffers[2].ShopCost, Is.EqualTo(50));
            Assert.That(snapshot.IncomeOffers[2].FirstShopVisit, Is.EqualTo(2));
            Assert.That(snapshot.ReorderPricing.BaseRandomCost, Is.EqualTo(10));
            Assert.That(snapshot.ReorderPricing.IncreaseNumerator, Is.EqualTo(11));
            Assert.That(snapshot.ReorderPricing.IncreaseDenominator, Is.EqualTo(10));
            Assert.That(snapshot.ReorderPricing.ChosenMultiplier, Is.EqualTo(10));
        }

        [Test]
        public void StarterDefinitionRejectsIncomeModifiersOutsideShops()
        {
            var clone = LoadClone();
            try
            {
                var serialized = new SerializedObject(clone);
                var offers = serialized.FindProperty("incomeOffers");
                for (var index = 0; index < offers.arraySize; index++)
                {
                    var sources = offers.GetArrayElementAtIndex(index).FindPropertyRelative("sources");
                    foreach (var invalid in new[] { FieldDataIncomeSource.RoundReward, FieldDataIncomeSource.NodeReward })
                    {
                        sources.intValue = (int)(FieldDataIncomeSource.Shop | invalid);
                        serialized.ApplyModifiedPropertiesWithoutUndo();
                        Assert.That(clone.TryResolve(out _, out _), Is.False,
                            "Income offer " + index + " must not allow " + invalid);
                    }
                    sources.intValue = (int)FieldDataIncomeSource.Shop;
                    serialized.ApplyModifiedPropertiesWithoutUndo();
                }
                Assert.That(clone.TryResolve(out _, out var error), Is.True, error);
            }
            finally
            {
                Object.DestroyImmediate(clone);
            }
        }

        [Test]
        public void PortableDefinitionRoundTripsAndRejectsTamperedContents()
        {
            var asset = AssetDatabase.LoadAssetAtPath<FieldDataJourneyDefinitionAsset>(DefinitionPath);
            Assert.That(asset, Is.Not.Null);
            Assert.That(asset.TryResolve(out var definition, out var error), Is.True, error);

            var portable = FieldDataPortableDefinition.Create(definition);
            Assert.That(portable.TryRestore(out var restored, out error), Is.True, error);
            Assert.That(restored.Fingerprint, Is.EqualTo(definition.Fingerprint));
            Assert.That(restored.Route.Select(node => node.Kind), Is.EqualTo(definition.Route.Select(node => node.Kind)));
            Assert.That(restored.IncomeOffers.Select(offer => offer.Id), Is.EqualTo(definition.IncomeOffers.Select(offer => offer.Id)));
            Assert.That(restored.SignedSpeciesOffers.Select(offer => offer.Id), Is.EqualTo(definition.SignedSpeciesOffers.Select(offer => offer.Id)));

            portable.incomeOffers[0].value = "999";
            Assert.That(portable.TryRestore(out restored, out error), Is.False);
            Assert.That(restored, Is.Null);
            Assert.That(error, Does.Contain("fingerprint"));
        }

        [Test]
        public void PortableDefinitionRejectsUnknownSchemaVersion()
        {
            var asset = AssetDatabase.LoadAssetAtPath<FieldDataJourneyDefinitionAsset>(DefinitionPath);
            Assert.That(asset.TryResolve(out var definition, out var error), Is.True, error);
            var portable = FieldDataPortableDefinition.Create(definition);
            portable.schemaVersion++;

            Assert.That(portable.TryRestore(out var restored, out error), Is.False);
            Assert.That(restored, Is.Null);
            Assert.That(error, Does.Contain("unsupported"));
        }

        [Test]
        public void RouteOrderAndConnectionsAreValidatedBeforeResolution()
        {
            var clone = LoadClone();
            try
            {
                var serialized = new SerializedObject(clone);
                serialized.FindProperty("route").GetArrayElementAtIndex(1)
                    .FindPropertyRelative("kind").intValue = (int)JourneyNodeKind.Condition;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                Assert.That(clone.TryResolve(out var result, out var error), Is.False);
                Assert.That(result, Is.Null);
                Assert.That(error, Does.Contain("Simulation → Shop"));
            }
            finally
            {
                Object.DestroyImmediate(clone);
            }
        }

        [Test]
        public void InvalidPopulationCostAndDuplicateIdsAreRejected()
        {
            var clone = LoadClone();
            try
            {
                var serialized = new SerializedObject(clone);
                var controls = serialized.FindProperty("populationControls");
                controls.GetArrayElementAtIndex(0).FindPropertyRelative("cost").intValue = 0;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                Assert.That(clone.TryResolve(out _, out var costError), Is.False);
                Assert.That(costError, Does.Contain("10-FD"));

                serialized = new SerializedObject(clone);
                controls = serialized.FindProperty("populationControls");
                controls.GetArrayElementAtIndex(0).FindPropertyRelative("cost").intValue = 10;
                controls.GetArrayElementAtIndex(1).FindPropertyRelative("id").stringValue = "add-hares";
                serialized.ApplyModifiedPropertiesWithoutUndo();
                Assert.That(clone.TryResolve(out _, out var idError), Is.False);
                Assert.That(idError, Does.Contain("unique"));
            }
            finally
            {
                Object.DestroyImmediate(clone);
            }
        }

        [Test]
        public void UnsupportedSignedSpeciesTargetsCannotEnterResolvedRun()
        {
            var clone = LoadClone();
            try
            {
                var serialized = new SerializedObject(clone);
                var offers = serialized.FindProperty("signedSpeciesOffers");
                offers.arraySize = 1;
                var offer = offers.GetArrayElementAtIndex(0);
                offer.FindPropertyRelative("id").stringValue = "long-stride-plus";
                offer.FindPropertyRelative("familyId").stringValue = "long-stride";
                offer.FindPropertyRelative("targetSpeciesId").stringValue = "fox";
                offer.FindPropertyRelative("attributeId").stringValue = "movement.speed";
                offer.FindPropertyRelative("signedAmount").floatValue = 1.5f;
                offer.FindPropertyRelative("cost").intValue = 30;
                serialized.ApplyModifiedPropertiesWithoutUndo();

                Assert.That(clone.TryResolve(out var result, out var error), Is.False);
                Assert.That(result, Is.Null);
                Assert.That(error, Does.Contain("Signed Hare offer"));
            }
            finally
            {
                Object.DestroyImmediate(clone);
            }
        }

        [Test]
        public void ActiveSnapshotFingerprintDoesNotChangeWhenAssetIsEdited()
        {
            var asset = AssetDatabase.LoadAssetAtPath<FieldDataJourneyDefinitionAsset>(DefinitionPath);
            var clone = Object.Instantiate(asset);
            try
            {
                Assert.That(asset.TryResolve(out var first, out var error), Is.True, error);
                var originalFingerprint = first.Fingerprint;
                var serialized = new SerializedObject(clone);
                serialized.FindProperty("incomeOffers").GetArrayElementAtIndex(0)
                    .FindPropertyRelative("displayName").stringValue = "Test label change";
                serialized.ApplyModifiedPropertiesWithoutUndo();

                Assert.That(clone.TryResolve(out var edited, out error), Is.True, error);
                Assert.That(edited.Fingerprint, Is.Not.EqualTo(originalFingerprint));
                Assert.That(first.Fingerprint, Is.EqualTo(originalFingerprint));
            }
            finally
            {
                Object.DestroyImmediate(clone);
            }
        }

        [Test]
        public void ExistingJourneyNodeKindSerializedValuesStayStable()
        {
            Assert.That((int)FieldDataIncomeOperation.AddPerHare, Is.EqualTo(0));
            Assert.That((int)FieldDataIncomeOperation.MultiplyPopulationPayout, Is.EqualTo(1));
            Assert.That((int)JourneyNodeKind.Simulation, Is.EqualTo(0));
            Assert.That((int)JourneyNodeKind.Condition, Is.EqualTo(1));
            Assert.That((int)JourneyNodeKind.Upgrade, Is.EqualTo(2));
            Assert.That((int)JourneyNodeKind.Event, Is.EqualTo(3));
            Assert.That((int)JourneyNodeKind.Finale, Is.EqualTo(4));
            Assert.That((int)JourneyNodeKind.Reward, Is.EqualTo(5));
            Assert.That((int)JourneyNodeKind.Shop, Is.EqualTo(6));
        }

        [Test]
        public void ExistingJourneyMapRemainsAuthoredWithoutTheFieldDataFixture()
        {
            var map = AssetDatabase.LoadAssetAtPath<JourneyMapAsset>(
                "Assets/Data/ProductionData/CellularSimulation/Species/Journey/ForestEdgeJourney.asset");
            Assert.That(map, Is.Not.Null);
            Assert.That(map.Nodes.Count, Is.EqualTo(29));
            Assert.That(map.ValidateOverlayLayout(out var error), Is.True, error);
            Assert.That(map.Nodes.All(node => node.Kind != JourneyNodeKind.Shop), Is.True);
        }

        static FieldDataJourneyDefinitionAsset LoadClone()
        {
            return Object.Instantiate(
                AssetDatabase.LoadAssetAtPath<FieldDataJourneyDefinitionAsset>(DefinitionPath));
        }
    }
}
