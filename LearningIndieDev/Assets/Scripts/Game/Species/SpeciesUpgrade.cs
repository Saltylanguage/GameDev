using System;

namespace SaltyGame
{
    public enum SpeciesUpgradeType
    {
        MovementSpeed,
        AttackAmount,
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
            var blockAmount = rules.BlockAmount;
            switch (Type)
            {
                case SpeciesUpgradeType.MovementSpeed:
                    movementSpeed += Value;
                    break;
                case SpeciesUpgradeType.AttackAmount:
                    attackAmount += (int)Value;
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
                litterMaximum: rules.LitterMaximum);
        }
    }
}
