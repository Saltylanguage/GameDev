using System;

namespace SaltyGame
{
    public sealed class SpeciesExperimentalOptions
    {
        public const string BevExperimentalFeaturesId = "bev-experimental";

        public SpeciesExperimentalOptions(
            string featureId = "",
            int foxAttackCooldownTicks = 0,
            float preContactAvoidanceChance = 0f)
        {
            if (foxAttackCooldownTicks < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(foxAttackCooldownTicks),
                    foxAttackCooldownTicks,
                    "Fox attack cooldown ticks cannot be negative.");
            }

            if (foxAttackCooldownTicks > 0
                && !string.Equals(featureId, BevExperimentalFeaturesId, StringComparison.Ordinal))
            {
                throw new ArgumentException(
                    $"Fox attack cooldown requires the {BevExperimentalFeaturesId} feature bundle.",
                    nameof(featureId));
            }

            if (preContactAvoidanceChance < 0f || preContactAvoidanceChance > 1f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(preContactAvoidanceChance),
                    preContactAvoidanceChance,
                    "Pre-contact avoidance chance must be between zero and one.");
            }

            if (preContactAvoidanceChance > 0f
                && !string.Equals(featureId, BevExperimentalFeaturesId, StringComparison.Ordinal))
            {
                throw new ArgumentException(
                    $"Pre-contact avoidance requires the {BevExperimentalFeaturesId} feature bundle.",
                    nameof(featureId));
            }

            FeatureId = featureId ?? string.Empty;
            FoxAttackCooldownTicks = foxAttackCooldownTicks;
            PreContactAvoidanceChance = preContactAvoidanceChance;
        }

        public static SpeciesExperimentalOptions None { get; } = new SpeciesExperimentalOptions();

        public string FeatureId { get; }
        public int FoxAttackCooldownTicks { get; }
        public bool HasFoxAttackCooldown => FoxAttackCooldownTicks > 0;
        public float PreContactAvoidanceChance { get; }
        public bool HasPreContactAvoidance => PreContactAvoidanceChance > 0f;
        public bool UsesSplitCombatStats =>
            string.Equals(FeatureId, BevExperimentalFeaturesId, StringComparison.Ordinal);
        public bool UsesHerbivoreStatLine =>
            string.Equals(FeatureId, BevExperimentalFeaturesId, StringComparison.Ordinal);
    }
}
