using System;
using System.Collections.Generic;

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
        VisionRange,
        ForageBelowEnergy,
        ReproductionChance,
        TrackingPersistenceSteps,
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

            if (value <= 0f || float.IsNaN(value) || float.IsInfinity(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Upgrade value must be finite and greater than zero.");
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

        public SpeciesUpgradeSnapshot CreateSnapshot(SpeciesId targetSpecies)
        {
            var modifiers = new List<SpeciesUpgradeModifier>();
            switch (Type)
            {
                case SpeciesUpgradeType.MovementSpeed:
                    modifiers.Add(new SpeciesUpgradeModifier(SpeciesAttributeIds.MovementSpeed, Value));
                    break;
                case SpeciesUpgradeType.AttackAmount:
                    modifiers.Add(new SpeciesUpgradeModifier(SpeciesAttributeIds.AttackAmount, Value));
                    modifiers.Add(new SpeciesUpgradeModifier(SpeciesAttributeIds.AttackModifier, Value));
                    modifiers.Add(new SpeciesUpgradeModifier(SpeciesAttributeIds.DamageAmount, Value));
                    break;
                case SpeciesUpgradeType.AttackModifier:
                    modifiers.Add(new SpeciesUpgradeModifier(SpeciesAttributeIds.AttackModifier, Value));
                    break;
                case SpeciesUpgradeType.DamageAmount:
                    modifiers.Add(new SpeciesUpgradeModifier(SpeciesAttributeIds.DamageAmount, Value));
                    break;
                case SpeciesUpgradeType.BlockAmount:
                    modifiers.Add(new SpeciesUpgradeModifier(SpeciesAttributeIds.BlockAmount, Value));
                    break;
                case SpeciesUpgradeType.ReproductionChance:
                    modifiers.Add(new SpeciesUpgradeModifier(SpeciesAttributeIds.ReproductionChance, Value));
                    break;
                case SpeciesUpgradeType.DigestionEnergyBonus:
                    modifiers.Add(new SpeciesUpgradeModifier(SpeciesAttributeIds.DigestionEnergyBonus, Value));
                    break;
                case SpeciesUpgradeType.CrowdingTolerance:
                    modifiers.Add(new SpeciesUpgradeModifier(SpeciesAttributeIds.CrowdingTolerance, Value));
                    break;
                case SpeciesUpgradeType.FleeMovementSpeedBonus:
                    modifiers.Add(new SpeciesUpgradeModifier(SpeciesAttributeIds.FleeMovementSpeedBonus, Value));
                    break;
                case SpeciesUpgradeType.VisionRange:
                    modifiers.Add(new SpeciesUpgradeModifier(SpeciesAttributeIds.VisionRange, Value));
                    break;
                case SpeciesUpgradeType.ForageBelowEnergy:
                    modifiers.Add(new SpeciesUpgradeModifier(SpeciesAttributeIds.ForageBelowEnergy, Value));
                    break;
                case SpeciesUpgradeType.TrackingPersistenceSteps:
                    modifiers.Add(new SpeciesUpgradeModifier(SpeciesAttributeIds.TrackingPersistenceSteps, Value));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Type), Type, "Unknown upgrade type.");
            }

            return new SpeciesUpgradeSnapshot(
                Id,
                SpeciesUpgradeCatalog.GetDisplayName(Id),
                "Legacy upgrade converted to the run upgrade contract.",
                targetSpecies,
                Cost,
                modifiers);
        }

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
            var reproductionChance = rules.ReproductionChance;
            var digestionEnergyBonus = rules.DigestionEnergyBonus;
            var crowdingTolerance = rules.CrowdingTolerance;
            var fleeMovementSpeedBonus = rules.FleeMovementSpeedBonus;
            var visionRange = rules.Awareness.VisionRange;
            var intelligence = rules.Awareness.Intelligence;
            var forageBelowEnergy = rules.ForageBelowEnergy;
            var trackingPersistenceSteps = rules.TrackingPersistenceSteps;
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
                case SpeciesUpgradeType.ReproductionChance:
                    reproductionChance += Value;
                    break;
                case SpeciesUpgradeType.DigestionEnergyBonus:
                    digestionEnergyBonus += Value;
                    break;
                case SpeciesUpgradeType.CrowdingTolerance:
                    crowdingTolerance += (int)Value;
                    break;
                case SpeciesUpgradeType.FleeMovementSpeedBonus:
                    fleeMovementSpeedBonus += Value;
                    break;
                case SpeciesUpgradeType.VisionRange:
                    visionRange += (int)Value;
                    break;
                case SpeciesUpgradeType.ForageBelowEnergy:
                    forageBelowEnergy += (int)Value;
                    break;
                case SpeciesUpgradeType.TrackingPersistenceSteps:
                    trackingPersistenceSteps += (int)Value;
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
                reproductionChance,
                rules.ReproductionFoodRequired,
                rules.MaxReproductionGroupSize,
                rules.StartingEnergy,
                rules.WiltChance,
                rules.CrowdingEnergyPenalty,
                rules.StartingFoodReserve,
                rules.SeedDropChance,
                rules.EnergyValue,
                rules.Metabolism,
                awareness: new SpeciesAwarenessRules(visionRange, intelligence),
                role: rules.Role,
                forageBelowEnergy: forageBelowEnergy,
                maximumEnergy: rules.MaximumEnergy,
                litterMinimum: rules.LitterMinimum,
                litterMaximum: rules.LitterMaximum,
                attackModifier: attackModifier,
                damageAmount: damageAmount,
                digestionEnergyBonus: digestionEnergyBonus,
                crowdingTolerance: crowdingTolerance,
                fleeMovementSpeedBonus: fleeMovementSpeedBonus,
                trackingPersistenceSteps: trackingPersistenceSteps);
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
        public const string EfficientDigestionId = "efficient-digestion";
        public const int EfficientDigestionMaxLevel = 10;
        public const float EfficientDigestionBonusPerLevel = 0.1f;
        public const string CrowdingToleranceId = "crowding-tolerance";
        public const int CrowdingToleranceMaxLevel = 10;
        public const int CrowdingToleranceBonusPerLevel = 1;
        public const string ReproductiveDriveId = "reproductive-drive";
        public const int ReproductiveDriveMaxLevel = 10;
        public const float ReproductiveDriveChancePerLevel = 0.005f;
        public const string KeenSensesId = "keen-senses";
        public const int KeenSensesMaxLevel = 10;
        public const float KeenSensesTrackingStepsPerLevel = 1f;
        public const string RelentlessPursuitId = "relentless-pursuit";
        public const int RelentlessPursuitMaxLevel = 10;
        public const float RelentlessPursuitMovementSpeedPerLevel = 0.15f;
        public const string PiercingBiteId = "piercing-bite";
        public const int PiercingBiteMaxLevel = 10;
        public const float PiercingBiteAttackModifierPerLevel = 1f;
        public const string HuntUrgencyId = "hunt-urgency";
        public const int HuntUrgencyMaxLevel = 10;
        public const float HuntUrgencyForageBelowEnergyPerLevel = 1f;
        public const string BroodDriveId = "brood-drive";
        public const int BroodDriveMaxLevel = 10;
        public const float BroodDriveChancePerLevel = 0.01f;

        public static int GetMaxLevel(string upgradeId)
        {
            return upgradeId == ToughHideId ? ToughHideMaxLevel
                : upgradeId == EfficientDigestionId ? EfficientDigestionMaxLevel
                : upgradeId == CrowdingToleranceId ? CrowdingToleranceMaxLevel
                : upgradeId == ReproductiveDriveId ? ReproductiveDriveMaxLevel
                : upgradeId == KeenSensesId ? KeenSensesMaxLevel
                : upgradeId == RelentlessPursuitId ? RelentlessPursuitMaxLevel
                : upgradeId == PiercingBiteId ? PiercingBiteMaxLevel
                : upgradeId == HuntUrgencyId ? HuntUrgencyMaxLevel
                : upgradeId == BroodDriveId ? BroodDriveMaxLevel
                : IsThreatExposureId(upgradeId) ? ThreatExposureMaxLevel : int.MaxValue;
        }
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
            ReproductiveDriveId,
            ThreatExposureId,
        };

        static readonly string[] ExperimentalPredatorUpgradeIds =
        {
            RelentlessPursuitId,
            PiercingBiteId,
            HuntUrgencyId,
            BroodDriveId,
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
                    return new SpeciesUpgrade(
                        EfficientDigestionId,
                        5,
                        SpeciesUpgradeType.DigestionEnergyBonus,
                        EfficientDigestionBonusPerLevel);
                case CrowdingToleranceId:
                    return new SpeciesUpgrade(
                        CrowdingToleranceId,
                        5,
                        SpeciesUpgradeType.CrowdingTolerance,
                        CrowdingToleranceBonusPerLevel);
                case ReproductiveDriveId:
                    return new SpeciesUpgrade(
                        ReproductiveDriveId,
                        5,
                        SpeciesUpgradeType.ReproductionChance,
                        ReproductiveDriveChancePerLevel);
                case KeenSensesId:
                    return new SpeciesUpgrade(
                        KeenSensesId,
                        5,
                        SpeciesUpgradeType.TrackingPersistenceSteps,
                        KeenSensesTrackingStepsPerLevel);
                case RelentlessPursuitId:
                    return new SpeciesUpgrade(
                        RelentlessPursuitId,
                        5,
                        SpeciesUpgradeType.MovementSpeed,
                        RelentlessPursuitMovementSpeedPerLevel);
                case PiercingBiteId:
                    return new SpeciesUpgrade(
                        PiercingBiteId,
                        5,
                        SpeciesUpgradeType.AttackModifier,
                        PiercingBiteAttackModifierPerLevel);
                case HuntUrgencyId:
                    return new SpeciesUpgrade(
                        HuntUrgencyId,
                        5,
                        SpeciesUpgradeType.ForageBelowEnergy,
                        HuntUrgencyForageBelowEnergyPerLevel);
                case BroodDriveId:
                    return new SpeciesUpgrade(
                        BroodDriveId,
                        5,
                        SpeciesUpgradeType.ReproductionChance,
                        BroodDriveChancePerLevel);
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
                case ReproductiveDriveId:
                    return "REPRODUCTIVE DRIVE";
                case KeenSensesId:
                    return "KEEN SENSES";
                case RelentlessPursuitId:
                    return "RELENTLESS PURSUIT";
                case PiercingBiteId:
                    return "PIERCING BITE";
                case HuntUrgencyId:
                    return "HUNT URGENCY";
                case BroodDriveId:
                    return "BROOD DRIVE";
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
            if (IsThreatExposureId(continuingUpgradeId))
            {
                continuingUpgradeId = ThreatExposureId;
            }

            return CreateExperimentalOffer(
                continuingUpgradeId,
                rotation,
                seed,
                ExperimentalHerbivoreUpgradeIds);
        }

        public static bool TryGetCoupledResponse(
            SpeciesId selectedSpecies,
            string selectedUpgradeId,
            out SpeciesId responderSpecies,
            out string responderUpgradeId)
        {
            responderSpecies = default;
            responderUpgradeId = string.Empty;
            var isHare = selectedSpecies == SpeciesIds.Herbivore
                || string.Equals(selectedSpecies.Value, "hare", StringComparison.Ordinal);
            var isFox = selectedSpecies == SpeciesIds.Carnivore
                || string.Equals(selectedSpecies.Value, "fox", StringComparison.Ordinal);
            if (isHare)
            {
                responderSpecies = string.Equals(selectedSpecies.Value, "hare", StringComparison.Ordinal)
                    ? new SpeciesId("fox")
                    : SpeciesIds.Carnivore;
                switch (selectedUpgradeId)
                {
                    case ToughHideId:
                        responderUpgradeId = PiercingBiteId;
                        break;
                    case ThreatExposureId:
                    case LegacyThreatResponseId:
                        responderUpgradeId = RelentlessPursuitId;
                        break;
                    case EfficientDigestionId:
                        responderUpgradeId = HuntUrgencyId;
                        break;
                    case ReproductiveDriveId:
                    case CrowdingToleranceId:
                        responderUpgradeId = BroodDriveId;
                        break;
                }
            }
            else if (isFox)
            {
                responderSpecies = string.Equals(selectedSpecies.Value, "fox", StringComparison.Ordinal)
                    ? new SpeciesId("hare")
                    : SpeciesIds.Herbivore;
                switch (selectedUpgradeId)
                {
                    case PiercingBiteId:
                        responderUpgradeId = ToughHideId;
                        break;
                    case RelentlessPursuitId:
                        responderUpgradeId = ThreatExposureId;
                        break;
                    case HuntUrgencyId:
                        responderUpgradeId = EfficientDigestionId;
                        break;
                    case BroodDriveId:
                        responderUpgradeId = ReproductiveDriveId;
                        break;
                }
            }

            return !string.IsNullOrEmpty(responderUpgradeId);
        }

        public static SpeciesUpgrade[] CreateExperimentalPredatorOffer(
            string continuingUpgradeId,
            int rotation,
            int seed)
        {
            return CreateExperimentalOffer(
                continuingUpgradeId,
                rotation,
                seed,
                ExperimentalPredatorUpgradeIds);
        }

        static SpeciesUpgrade[] CreateExperimentalOffer(
            string continuingUpgradeId,
            int rotation,
            int seed,
            string[] upgradeIds)
        {
            if (rotation < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rotation), rotation, "Offer rotation cannot be negative.");
            }

            var primaryIndex = Array.IndexOf(upgradeIds, continuingUpgradeId);
            var seededValue = seed & int.MaxValue;
            var hasContinuingUpgrade = primaryIndex >= 0;
            if (!hasContinuingUpgrade)
            {
                primaryIndex = seededValue % upgradeIds.Length;
            }

            var alternativeRotation = hasContinuingUpgrade
                ? rotation % (upgradeIds.Length - 1)
                : (seededValue / upgradeIds.Length) % (upgradeIds.Length - 1);
            var alternativeIndex = (primaryIndex + 1 + alternativeRotation) % upgradeIds.Length;
            return new[]
            {
                Create(upgradeIds[primaryIndex]),
                Create(upgradeIds[alternativeIndex]),
            };
        }
    }
}
