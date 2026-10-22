using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;

namespace SaltyGame.EditorTests
{
    public sealed class FieldDataSpeciesEffectsTests
    {
        [TestCase(false)]
        [TestCase(true)]
        public void SignedOppositesNetAgainstBaseRulesBeforeClamping(bool reverse)
        {
            var baseRules = SpeciesRuleDefaults.Create()[SpeciesIds.Herbivore];
            var effects = new[]
            {
                Effect("stride-plus", "long-stride", SpeciesAttributeIds.MovementSpeed, 1.5f),
                Effect("stride-minus", "long-stride", SpeciesAttributeIds.MovementSpeed, -1.5f),
                Effect("hide-plus", "reinforced-hide", SpeciesAttributeIds.BlockAmount, 99f),
                Effect("hide-minus", "reinforced-hide", SpeciesAttributeIds.BlockAmount, -99f),
                Effect("digestion-plus", "deep-digestion", SpeciesAttributeIds.DigestionEnergyBonus, 99f),
                Effect("digestion-minus", "deep-digestion", SpeciesAttributeIds.DigestionEnergyBonus, -99f),
                Effect("seed-plus", "abundant-seedfall", SpeciesAttributeIds.SeedDropChance, 99f),
                Effect("seed-minus", "abundant-seedfall", SpeciesAttributeIds.SeedDropChance, -99f),
                Effect("crowding-plus", "warren-comfort", SpeciesAttributeIds.CrowdingEnergyReduction, 99f),
                Effect("crowding-minus", "warren-comfort", SpeciesAttributeIds.CrowdingEnergyReduction, -99f),
                Effect("instinct-plus", "sharper-instinct", SpeciesAttributeIds.PreContactAvoidanceChance, 0.9f),
                Effect("instinct-minus", "sharper-instinct", SpeciesAttributeIds.PreContactAvoidanceChance, -0.9f),
            };
            if (reverse) System.Array.Reverse(effects);

            Assert.That(FieldDataSpeciesEffects.TryResolve(baseRules, 0.5f, effects,
                out var effective, out var avoidance, out var fingerprint, out var error), Is.True, error);
            Assert.That(RuntimeTargets(effective, avoidance), Is.EqualTo(RuntimeTargets(baseRules, 0.5f)));
            Assert.That(fingerprint, Has.Length.EqualTo(64));
        }

        [Test]
        public void SignedEffectsRespectFinalAttributeBoundsAndAvoidanceRegistryMapping()
        {
            var baseRules = SpeciesRuleDefaults.Create()[SpeciesIds.Herbivore];
            var effects = new[]
            {
                Effect("stride-minus", "long-stride", SpeciesAttributeIds.MovementSpeed, -99f),
                Effect("hide-minus", "reinforced-hide", SpeciesAttributeIds.BlockAmount, -99f),
                Effect("digestion-minus", "deep-digestion", SpeciesAttributeIds.DigestionEnergyBonus, -0.3f),
                Effect("warren-minus", "warren-comfort", SpeciesAttributeIds.CrowdingEnergyReduction, -0.3f),
                Effect("seed-plus", "abundant-seedfall", SpeciesAttributeIds.SeedDropChance, 99f),
                Effect("instinct-plus", "sharper-instinct", SpeciesAttributeIds.PreContactAvoidanceChance, 0.24f),
                Effect("instinct-minus", "sharper-instinct", SpeciesAttributeIds.PreContactAvoidanceChance, -0.48f),
            };

            Assert.That(SpeciesAttributeRegistry.Contains(SpeciesAttributeIds.PreContactAvoidanceChance), Is.True);
            Assert.That(FieldDataSpeciesEffects.TryResolve(baseRules, 0.1f, effects,
                out var effective, out var avoidance, out _, out var error), Is.True, error);
            Assert.That(effective.MovementSpeed, Is.Zero);
            Assert.That(effective.BlockAmount, Is.Zero);
            Assert.That(effective.DigestionEnergyBonus, Is.Zero);
            Assert.That(effective.CrowdingEnergyReduction, Is.Zero);
            Assert.That(effective.SeedDropChance, Is.EqualTo(1f));
            Assert.That(avoidance, Is.Zero);
        }

        [Test]
        public void EffectFingerprintsAreStableAndDuplicateInstancesAreRejected()
        {
            var baseRules = SpeciesRuleDefaults.Create()[SpeciesIds.Herbivore];
            var effect = Effect("one", "long-stride", SpeciesAttributeIds.MovementSpeed, 1.5f);
            Assert.That(FieldDataSpeciesEffects.TryResolve(baseRules, 0f, new[] { effect },
                out _, out _, out var first, out var error), Is.True, error);
            Assert.That(FieldDataSpeciesEffects.TryResolve(baseRules, 0f, new[] { effect },
                out _, out _, out var second, out error), Is.True, error);
            Assert.That(second, Is.EqualTo(first));
            Assert.That(FieldDataSpeciesEffects.TryResolve(baseRules, 0f, new[] { effect, effect },
                out _, out _, out _, out error), Is.False);
            Assert.That(error, Does.Contain("unique instance IDs"));
        }

        [Test]
        public void EveryApprovedVariantChangesOnlyItsRegisteredRuntimeTarget()
        {
            var asset = AssetDatabase.LoadAssetAtPath<FieldDataJourneyDefinitionAsset>(
                "Assets/Data/TestData/FieldDataStarterDefinition.asset");
            Assert.That(asset.TryResolve(out var definition, out var error), Is.True, error);
            var baseRules = SpeciesRuleDefaults.Create()[SpeciesIds.Herbivore];
            foreach (var modifier in new[]
            {
                new SpeciesUpgradeModifier(SpeciesAttributeIds.MovementSpeed, 3f),
                new SpeciesUpgradeModifier(SpeciesAttributeIds.BlockAmount, 20f),
                new SpeciesUpgradeModifier(SpeciesAttributeIds.DigestionEnergyBonus, 0.5f),
                new SpeciesUpgradeModifier(SpeciesAttributeIds.SeedDropChance, 0.5f),
                new SpeciesUpgradeModifier(SpeciesAttributeIds.CrowdingEnergyReduction, 0.5f),
            })
                baseRules = SpeciesAttributeRegistry.Apply(baseRules, modifier);
            var before = RuntimeTargets(baseRules, 0.5f);
            var variantCount = 0;
            foreach (var offer in definition.SignedSpeciesOffers)
            {
                variantCount++;
                Assert.That(SpeciesAttributeRegistry.Contains(offer.AttributeId), Is.True, offer.Id);
                Assert.That(FieldDataSpeciesEffects.TryResolve(baseRules, 0.5f,
                    new[] { new FieldDataSpeciesEffectInstance("test-" + offer.Id, offer) },
                    out var effective, out var avoidance, out _, out error), Is.True, error);
                var after = RuntimeTargets(effective, avoidance);
                foreach (var target in before)
                {
                    var expected = target.Value + (target.Key == offer.AttributeId ? offer.SignedAmount : 0f);
                    Assert.That(after[target.Key], Is.EqualTo(expected).Within(0.000001f),
                        offer.Id + " must affect only " + offer.AttributeId);
                }
            }
            Assert.That(variantCount, Is.EqualTo(12));
        }

        static Dictionary<string, float> RuntimeTargets(SpeciesRules rules, float avoidance)
        {
            return new Dictionary<string, float>
            {
                [SpeciesAttributeIds.MovementSpeed] = rules.MovementSpeed,
                [SpeciesAttributeIds.BlockAmount] = rules.BlockAmount,
                [SpeciesAttributeIds.DigestionEnergyBonus] = rules.DigestionEnergyBonus,
                [SpeciesAttributeIds.SeedDropChance] = rules.SeedDropChance,
                [SpeciesAttributeIds.CrowdingEnergyReduction] = rules.CrowdingEnergyReduction,
                [SpeciesAttributeIds.PreContactAvoidanceChance] = avoidance,
            };
        }

        static FieldDataSpeciesEffectInstance Effect(string id, string family, string attribute, float amount)
        {
            return new FieldDataSpeciesEffectInstance(id,
                new FieldDataSignedSpeciesOffer(id, family, "hare", attribute, amount, 30, 1));
        }
    }
}
