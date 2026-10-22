using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace SaltyGame
{
    /// <summary>A signed, journey-local species shop effect that remains separate from ordinary progression.</summary>
    public sealed class FieldDataSpeciesEffectInstance
    {
        public FieldDataSpeciesEffectInstance(string instanceId, FieldDataSignedSpeciesOffer offer)
        {
            if (string.IsNullOrWhiteSpace(instanceId)) throw new ArgumentException("Effect instance ID is required.", nameof(instanceId));
            if (string.IsNullOrWhiteSpace(offer.Id) || string.IsNullOrWhiteSpace(offer.FamilyId)
                || string.IsNullOrWhiteSpace(offer.TargetSpeciesId) || !SpeciesAttributeRegistry.Contains(offer.AttributeId)
                || float.IsNaN(offer.SignedAmount) || float.IsInfinity(offer.SignedAmount) || offer.SignedAmount == 0f)
                throw new ArgumentException("A valid signed species offer is required.", nameof(offer));

            InstanceId = instanceId.Trim();
            OfferId = offer.Id;
            FamilyId = offer.FamilyId;
            TargetSpeciesId = offer.TargetSpeciesId;
            AttributeId = offer.AttributeId;
            SignedAmount = offer.SignedAmount;
        }

        public string InstanceId { get; }
        public string OfferId { get; }
        public string FamilyId { get; }
        public string TargetSpeciesId { get; }
        public string AttributeId { get; }
        public float SignedAmount { get; }
    }

    public static class FieldDataSpeciesEffects
    {
        public const int FamilyCopyLimit = 3;
        public const string ResolverVersion = "field-data-species-effects-v1";

        public static bool TryResolve(
            SpeciesRules baseRules,
            float baseAvoidanceChance,
            IEnumerable<FieldDataSpeciesEffectInstance> effects,
            out SpeciesRules effectiveRules,
            out float effectiveAvoidanceChance,
            out string fingerprint,
            out string error)
        {
            effectiveRules = null;
            effectiveAvoidanceChance = 0f;
            fingerprint = string.Empty;
            error = null;
            if (baseRules == null)
            {
                error = "Base species rules are required.";
                return false;
            }
            if (float.IsNaN(baseAvoidanceChance) || float.IsInfinity(baseAvoidanceChance)
                || baseAvoidanceChance < 0f || baseAvoidanceChance > 1f)
            {
                error = "Base avoidance chance must be between zero and one.";
                return false;
            }

            var totals = new Dictionary<string, float>(StringComparer.Ordinal);
            var canonical = new StringBuilder(ResolverVersion).Append('|').Append(SpeciesAttributeRegistry.Fingerprint);
            var seenInstances = new HashSet<string>(StringComparer.Ordinal);
            foreach (var effect in effects ?? Array.Empty<FieldDataSpeciesEffectInstance>())
            {
                if (effect == null || !seenInstances.Add(effect.InstanceId))
                {
                    error = "Effect entries must be non-null and have unique instance IDs.";
                    return false;
                }
                if (!string.Equals(effect.TargetSpeciesId, "hare", StringComparison.Ordinal))
                {
                    error = "The first Field Data effect resolver supports Hare targets only.";
                    return false;
                }
                totals.TryGetValue(effect.AttributeId, out var total);
                var next = total + effect.SignedAmount;
                if (float.IsNaN(next) || float.IsInfinity(next))
                {
                    error = "The net signed effect is outside the supported numeric range.";
                    return false;
                }
                totals[effect.AttributeId] = next;
                canonical.Append('|').Append(effect.InstanceId).Append(':').Append(effect.OfferId)
                    .Append(':').Append(effect.FamilyId).Append(':').Append(effect.TargetSpeciesId)
                    .Append(':').Append(effect.AttributeId).Append(':')
                    .Append(effect.SignedAmount.ToString("R", CultureInfo.InvariantCulture));
            }

            var result = baseRules;
            var avoidance = baseAvoidanceChance;
            foreach (var entry in totals)
            {
                var amount = entry.Value;
                if (Math.Abs(amount) < 0.000001f) continue;
                if (entry.Key == SpeciesAttributeIds.PreContactAvoidanceChance)
                {
                    avoidance = Clamp01(avoidance + amount);
                    continue;
                }

                if (!SpeciesAttributeRegistry.TryGet(entry.Key, out var definition))
                {
                    error = "The signed effect references an unknown attribute.";
                    return false;
                }

                var baseValue = ReadAttribute(result, definition.Target);
                var targetValue = baseValue + amount;
                if (definition.Target == SpeciesAttributeTarget.MovementSpeed
                    || definition.Target == SpeciesAttributeTarget.BlockAmount
                    || definition.Target == SpeciesAttributeTarget.DigestionEnergyBonus)
                    targetValue = Math.Max(0f, targetValue);
                else if (definition.Target == SpeciesAttributeTarget.SeedDropChance
                    || definition.Target == SpeciesAttributeTarget.CrowdingEnergyReduction)
                    targetValue = Clamp01(targetValue);

                var boundedDelta = targetValue - baseValue;
                if (Math.Abs(boundedDelta) >= 0.000001f)
                    result = SpeciesAttributeRegistry.Apply(result, new SpeciesUpgradeModifier(entry.Key, boundedDelta));
            }

            effectiveRules = result;
            effectiveAvoidanceChance = avoidance;
            fingerprint = Hash(canonical.ToString());
            return true;
        }

        static float ReadAttribute(SpeciesRules rules, SpeciesAttributeTarget target)
        {
            switch (target)
            {
                case SpeciesAttributeTarget.MovementSpeed: return rules.MovementSpeed;
                case SpeciesAttributeTarget.BlockAmount: return rules.BlockAmount;
                case SpeciesAttributeTarget.DigestionEnergyBonus: return rules.DigestionEnergyBonus;
                case SpeciesAttributeTarget.SeedDropChance: return rules.SeedDropChance;
                case SpeciesAttributeTarget.CrowdingEnergyReduction: return rules.CrowdingEnergyReduction;
                default: throw new InvalidOperationException("Unsupported Field Data shop effect target: " + target);
            }
        }

        static float Clamp01(float value) => Math.Max(0f, Math.Min(1f, value));

        static string Hash(string value)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(value));
                var result = new StringBuilder(bytes.Length * 2);
                foreach (var item in bytes) result.Append(item.ToString("x2", CultureInfo.InvariantCulture));
                return result.ToString();
            }
        }
    }
}
