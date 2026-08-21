using System;

namespace SaltyGame
{
    public sealed class SpeciesExperimentalOptions
    {
        public const string BevExperimentalFeaturesId = "bev-experimental";

        public SpeciesExperimentalOptions(string featureId = "", int foxAttackCooldownTicks = 0)
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

            FeatureId = featureId ?? string.Empty;
            FoxAttackCooldownTicks = foxAttackCooldownTicks;
        }

        public static SpeciesExperimentalOptions None { get; } = new SpeciesExperimentalOptions();

        public string FeatureId { get; }
        public int FoxAttackCooldownTicks { get; }
        public bool HasFoxAttackCooldown => FoxAttackCooldownTicks > 0;
    }
}
