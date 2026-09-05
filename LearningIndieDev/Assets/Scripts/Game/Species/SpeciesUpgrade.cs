using System;

namespace SaltyGame
{
    public enum SpeciesUpgradeType
    {
        MovementSpeed,
        AttackAmount,
        AttackModifier,
        DamageAmount,
        BlockAmount,
        DigestionEnergyBonus,
        CrowdingTolerance,
        FleeMovementSpeedBonus,
    }

    public sealed class SpeciesUpgrade
    {
        public SpeciesUpgrade(string id, int cost, SpeciesUpgradeType type, float value)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Upgrade id cannot be empty.", nameof(id));
            }

            if (cost < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(cost), cost, "Upgrade cost cannot be negative.");
            }

            if (value <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Upgrade value must be greater than zero.");
            }

            Id = id;
            Cost = cost;
            Type = type;
            Value = value;
        }

        public string Id { get; }
        public int Cost { get; }
        public SpeciesUpgradeType Type { get; }
        public float Value { get; }

        public SpeciesRules Apply(SpeciesRules rules)
        {
            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            var movementSpeed = rules.MovementSpeed;
            var attackAmount = rules.AttackAmount;
            var attackModifier = rules.AttackModifier;
            var damageAmount = rules.DamageAmount;
            var blockAmount = rules.BlockAmount;
            var digestionEnergyBonus = rules.DigestionEnergyBonus;
            var crowdingTolerance = rules.CrowdingTolerance;
            var fleeMovementSpeedBonus = rules.FleeMovementSpeedBonus;
            switch (Type)
            {
                case SpeciesUpgradeType.MovementSpeed:
                    movementSpeed += Value;
                    break;
                case SpeciesUpgradeType.AttackAmount:
                    attackAmount += (int)Value;
                    attackModifier += (int)Value;
                    damageAmount += (int)Value;
                    break;
                case SpeciesUpgradeType.AttackModifier:
                    attackModifier += (int)Value;
                    break;
                case SpeciesUpgradeType.DamageAmount:
                    damageAmount += (int)Value;
                    break;
                case SpeciesUpgradeType.BlockAmount:
                    blockAmount += (int)Value;
                    break;
                case SpeciesUpgradeType.DigestionEnergyBonus:
                    digestionEnergyBonus += (int)Value;
                    break;
                case SpeciesUpgradeType.CrowdingTolerance:
                    crowdingTolerance += (int)Value;
                    break;
                case SpeciesUpgradeType.FleeMovementSpeedBonus:
                    fleeMovementSpeedBonus += Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Type), Type, "Unknown upgrade type.");
            }

            return new SpeciesRules(
                movementSpeed,
                rules.MovementPattern,
                rules.AttackPattern,
                attackAmount,
                rules.BlockPattern,
                blockAmount,
                rules.DietPattern,
                rules.DietTargetId,
                rules.ReproductionPattern,
                rules.ReproductionNeighborCount,
                rules.ReproductionChance,
                rules.ReproductionFoodRequired,
                rules.MaxReproductionGroupSize,
                rules.StartingEnergy,
                rules.WiltChance,
                rules.CrowdingEnergyPenalty,
                rules.StartingFoodReserve,
                rules.SeedDropChance,
                rules.EnergyValue,
                rules.Metabolism,
                awareness: rules.Awareness,
                role: rules.Role,
                forageBelowEnergy: rules.ForageBelowEnergy,
                maximumEnergy: rules.MaximumEnergy,
                litterMinimum: rules.LitterMinimum,
                litterMaximum: rules.LitterMaximum,
                attackModifier: attackModifier,
                damageAmount: damageAmount,
                digestionEnergyBonus: digestionEnergyBonus,
                crowdingTolerance: crowdingTolerance,
                fleeMovementSpeedBonus: fleeMovementSpeedBonus);
        }
    }

    public static class SpeciesUpgradeCatalog
    {
        public const string FasterMovementId = "faster-movement";
        public const string StrongerAttackId = "stronger-attack";
        public const string StrongerAttackModifierId = "stronger-attack-modifier";
        public const string StrongerDamageId = "stronger-damage";
        public const string StrongerBlockId = "stronger-block";
        public const string StrongerBlockTwoId = "stronger-block-2";
        public const string ToughHideId = "tough-hide";
        public const int ToughHideMaxLevel = 10;

        public static int GetMaxLevel(string upgradeId)
        {
            return upgradeId == ToughHideId ? ToughHideMaxLevel
                : IsThreatExposureId(upgradeId) ? ThreatExposureMaxLevel : int.MaxValue;
        }
        public const string EfficientDigestionId = "efficient-digestion";
        public const string CrowdingToleranceId = "crowding-tolerance";
        public const string ThreatExposureId = "threat-exposure";
        public const string LegacyThreatResponseId = "threat-response";
        [Obsolete("Use ThreatExposureId.")]
        public const string ThreatResponseId = LegacyThreatResponseId;
        public const float ThreatExposureFleeSpeedBonus = 0.75f;
        public const float ThreatExposureAvoidanceChanceBonus = 0.08f;
        public const int ThreatExposureMaxLevel = 10;
        [Obsolete("Use ThreatExposureFleeSpeedBonus.")]
        public const float ThreatResponseFleeSpeedBonus = ThreatExposureFleeSpeedBonus;
        [Obsolete("Use ThreatExposureAvoidanceChanceBonus.")]
        public const float ThreatResponseAvoidanceChanceBonus = ThreatExposureAvoidanceChanceBonus;
        [Obsolete("Use ThreatExposureMaxLevel.")]
        public const int ThreatResponseMaxLevel = ThreatExposureMaxLevel;

        static readonly string[] ExperimentalHerbivoreUpgradeIds =
        {
            ToughHideId,
            EfficientDigestionId,
            CrowdingToleranceId,
            ThreatExposureId,
        };

        public static SpeciesUpgrade Create(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Upgrade id cannot be empty.", nameof(id));
            }

            switch (id)
            {
                case FasterMovementId:
                    return new SpeciesUpgrade(FasterMovementId, 5, SpeciesUpgradeType.MovementSpeed, 0.5f);
                case StrongerAttackId:
                    return new SpeciesUpgrade(StrongerAttackId, 5, SpeciesUpgradeType.AttackAmount, 1f);
                case StrongerAttackModifierId:
                    return new SpeciesUpgrade(StrongerAttackModifierId, 5, SpeciesUpgradeType.AttackModifier, 1f);
                case StrongerDamageId:
                    return new SpeciesUpgrade(StrongerDamageId, 5, SpeciesUpgradeType.DamageAmount, 1f);
                case StrongerBlockId:
                    return new SpeciesUpgrade(StrongerBlockId, 5, SpeciesUpgradeType.BlockAmount, 1f);
                case StrongerBlockTwoId:
                    return new SpeciesUpgrade(StrongerBlockTwoId, 5, SpeciesUpgradeType.BlockAmount, 2f);
                case ToughHideId:
                    return new SpeciesUpgrade(ToughHideId, 5, SpeciesUpgradeType.BlockAmount, 2f);
                case EfficientDigestionId:
                    return new SpeciesUpgrade(EfficientDigestionId, 5, SpeciesUpgradeType.DigestionEnergyBonus, 1f);
                case CrowdingToleranceId:
                    return new SpeciesUpgrade(CrowdingToleranceId, 5, SpeciesUpgradeType.CrowdingTolerance, 1f);
                case ThreatExposureId:
                case LegacyThreatResponseId:
                    return new SpeciesUpgrade(
                        ThreatExposureId,
                        5,
                        SpeciesUpgradeType.FleeMovementSpeedBonus,
                        ThreatExposureFleeSpeedBonus);
                default:
                    const string blockSweepPrefix = "stronger-block-";
                    if (id.StartsWith(blockSweepPrefix, StringComparison.Ordinal)
                        && int.TryParse(id.Substring(blockSweepPrefix.Length), out var blockBonus)
                        && blockBonus >= 3
                        && blockBonus <= 10)
                    {
                        return new SpeciesUpgrade(id, 5, SpeciesUpgradeType.BlockAmount, blockBonus);
                    }

                    throw new ArgumentException($"Unknown upgrade id '{id}'.", nameof(id));
            }
        }

        public static bool IsThreatExposureId(string id)
        {
            return string.Equals(id, ThreatExposureId, StringComparison.Ordinal)
                || string.Equals(id, LegacyThreatResponseId, StringComparison.Ordinal);
        }

        public static bool IsThreatExposureFleeLevel(int level)
        {
            if (level < 1 || level > ThreatExposureMaxLevel)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(level),
                    level,
                    $"Threat Exposure level must be between 1 and {ThreatExposureMaxLevel}.");
            }

            return level == 1;
        }

        public static float GetThreatExposureAvoidanceChance(int level)
        {
            if (level < 0 || level > ThreatExposureMaxLevel)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(level),
                    level,
                    $"Threat Exposure level must be between 0 and {ThreatExposureMaxLevel}.");
            }

            return level * ThreatExposureAvoidanceChanceBonus;
        }

        [Obsolete("Use IsThreatExposureFleeLevel.")]
        public static bool IsThreatResponseFleeLevel(int level)
        {
            return IsThreatExposureFleeLevel(level);
        }

        [Obsolete("Use GetThreatExposureAvoidanceChance.")]
        public static float GetThreatResponseAvoidanceChance(int level)
        {
            return GetThreatExposureAvoidanceChance(level);
        }

        public static string GetDisplayName(string id)
        {
            switch (id)
            {
                case FasterMovementId:
                    return "FASTER";
                case StrongerAttackId:
                    return "ATTACK";
                case StrongerBlockId:
                    return "BLOCK";
                case ToughHideId:
                    return "TOUGH HIDE";
                case EfficientDigestionId:
                    return "EFFICIENT DIGESTION";
                case CrowdingToleranceId:
                    return "CROWDING TOLERANCE";
                case ThreatExposureId:
                case LegacyThreatResponseId:
                    return "THREAT EXPOSURE";
                default:
                    return id?.ToUpperInvariant() ?? string.Empty;
            }
        }

        public static SpeciesUpgrade[] CreateExperimentalHerbivoreOffer(
            string continuingUpgradeId,
            int rotation,
            int seed)
        {
            if (rotation < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rotation), rotation, "Offer rotation cannot be negative.");
            }

            if (IsThreatExposureId(continuingUpgradeId))
            {
                continuingUpgradeId = ThreatExposureId;
            }

            var primaryIndex = Array.IndexOf(ExperimentalHerbivoreUpgradeIds, continuingUpgradeId);
            var seededValue = seed & int.MaxValue;
            var hasContinuingUpgrade = primaryIndex >= 0;
            if (!hasContinuingUpgrade)
            {
                primaryIndex = seededValue % ExperimentalHerbivoreUpgradeIds.Length;
            }

            var alternativeRotation = hasContinuingUpgrade
                ? rotation % (ExperimentalHerbivoreUpgradeIds.Length - 1)
                : (seededValue / ExperimentalHerbivoreUpgradeIds.Length)
                    % (ExperimentalHerbivoreUpgradeIds.Length - 1);
            var alternativeIndex = (primaryIndex + 1 + alternativeRotation)
                % ExperimentalHerbivoreUpgradeIds.Length;
            return new[]
            {
                Create(ExperimentalHerbivoreUpgradeIds[primaryIndex]),
                Create(ExperimentalHerbivoreUpgradeIds[alternativeIndex]),
            };
        }
    }
}
