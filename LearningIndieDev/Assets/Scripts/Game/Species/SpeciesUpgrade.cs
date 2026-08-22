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
                damageAmount: damageAmount);
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
    }
}
