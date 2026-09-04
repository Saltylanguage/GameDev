using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace SaltyGame
{
    public enum SpeciesAttributeValueKind
    {
        Integer,
        Float,
    }

    public readonly struct SpeciesAttributeDefinition
    {
        internal SpeciesAttributeDefinition(
            string id,
            string displayName,
            SpeciesAttributeValueKind valueKind,
            SpeciesAttributeTarget target)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Attribute id cannot be empty.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Attribute display name cannot be empty.", nameof(displayName));
            }

            Id = id.Trim();
            DisplayName = displayName.Trim();
            ValueKind = valueKind;
            Target = target;
        }

        public string Id { get; }
        public string DisplayName { get; }
        public SpeciesAttributeValueKind ValueKind { get; }
        internal SpeciesAttributeTarget Target { get; }
    }

    public enum SpeciesAttributeTarget
    {
        MovementSpeed,
        AttackAmount,
        AttackModifier,
        DamageAmount,
        BlockAmount,
        ReproductionNeighborCount,
        ReproductionChance,
        ReproductionFoodRequired,
        MaxReproductionGroupSize,
        StartingEnergy,
        ForageBelowEnergy,
        WiltChance,
        CrowdingEnergyPenalty,
        StartingFoodReserve,
        SeedDropChance,
        EnergyValue,
        Metabolism,
        VisionRange,
        Intelligence,
        MaximumEnergy,
        LitterMinimum,
        LitterMaximum,
        DigestionEnergyBonus,
        CrowdingTolerance,
        FleeMovementSpeedBonus,
    }

    public static class SpeciesAttributeIds
    {
        public const string MovementSpeed = "movement.speed";
        public const string AttackAmount = "combat.attack-amount";
        public const string AttackModifier = "combat.attack-modifier";
        public const string DamageAmount = "combat.damage";
        public const string BlockAmount = "combat.block";
        public const string ReproductionNeighborCount = "reproduction.neighbor-count";
        public const string ReproductionChance = "reproduction.chance";
        public const string ReproductionFoodRequired = "reproduction.food-required";
        public const string MaxReproductionGroupSize = "reproduction.group-size";
        public const string StartingEnergy = "energy.starting";
        public const string ForageBelowEnergy = "energy.forage-threshold";
        public const string WiltChance = "resource.wilt-chance";
        public const string CrowdingEnergyPenalty = "crowding.energy-penalty";
        public const string StartingFoodReserve = "resource.starting-food-reserve";
        public const string SeedDropChance = "resource.seed-drop-chance";
        public const string EnergyValue = "energy.value";
        public const string Metabolism = "energy.metabolism";
        public const string VisionRange = "awareness.vision-range";
        public const string Intelligence = "awareness.intelligence";
        public const string MaximumEnergy = "energy.maximum";
        public const string LitterMinimum = "reproduction.litter-minimum";
        public const string LitterMaximum = "reproduction.litter-maximum";
        public const string DigestionEnergyBonus = "digestion.energy-bonus";
        public const string CrowdingTolerance = "crowding.tolerance";
        public const string FleeMovementSpeedBonus = "flee.movement-speed-bonus";
    }

    public static class SpeciesAttributeRegistry
    {
        static readonly IReadOnlyDictionary<string, SpeciesAttributeDefinition> definitions =
            CreateDefinitions();
        static readonly IReadOnlyList<SpeciesAttributeDefinition> allDefinitions =
            CreateDefinitionList(definitions);
        static readonly string registryFingerprint = CreateFingerprint(definitions);

        public const string Version = "species-attribute-registry-v1";
        public static string Fingerprint => registryFingerprint;
        public static IReadOnlyList<SpeciesAttributeDefinition> All => allDefinitions;

        public static bool Contains(string attributeId)
        {
            return !string.IsNullOrWhiteSpace(attributeId)
                && definitions.ContainsKey(attributeId.Trim());
        }

        public static bool TryGet(string attributeId, out SpeciesAttributeDefinition definition)
        {
            definition = default;
            return !string.IsNullOrWhiteSpace(attributeId)
                && definitions.TryGetValue(attributeId.Trim(), out definition);
        }

        internal static void ValidateModifier(SpeciesUpgradeModifier modifier)
        {
            if (!TryGet(modifier.AttributeId, out var definition))
            {
                throw new ArgumentException(
                    $"Unknown species attribute id '{modifier.AttributeId}'.",
                    nameof(modifier));
            }

            var value = modifier.SignedValue;
            if (definition.ValueKind == SpeciesAttributeValueKind.Integer
                && (value < int.MinValue
                    || value > int.MaxValue
                    || Math.Abs(value - (float)Math.Round(value)) > 0.0001f))
            {
                throw new ArgumentException(
                    $"Attribute '{modifier.AttributeId}' requires a whole-number modifier.",
                    nameof(modifier));
            }
        }

        public static SpeciesRules Apply(SpeciesRules rules, SpeciesUpgradeModifier modifier)
        {
            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            ValidateModifier(modifier);
            var definition = definitions[modifier.AttributeId.Trim()];
            var value = modifier.SignedValue;

            var movementSpeed = rules.MovementSpeed;
            var attackAmount = rules.AttackAmount;
            var attackModifier = rules.AttackModifier;
            var damageAmount = rules.DamageAmount;
            var blockAmount = rules.BlockAmount;
            var reproductionNeighborCount = rules.ReproductionNeighborCount;
            var reproductionChance = rules.ReproductionChance;
            var reproductionFoodRequired = rules.ReproductionFoodRequired;
            var maxReproductionGroupSize = rules.MaxReproductionGroupSize;
            var startingEnergy = rules.StartingEnergy;
            var forageBelowEnergy = rules.ForageBelowEnergy;
            var wiltChance = rules.WiltChance;
            var crowdingEnergyPenalty = rules.CrowdingEnergyPenalty;
            var startingFoodReserve = rules.StartingFoodReserve;
            var seedDropChance = rules.SeedDropChance;
            var energyValue = rules.EnergyValue;
            var metabolism = rules.Metabolism;
            var visionRange = rules.Awareness.VisionRange;
            var intelligence = rules.Awareness.Intelligence;
            var maximumEnergy = rules.MaximumEnergy;
            var litterMinimum = rules.LitterMinimum;
            var litterMaximum = rules.LitterMaximum;
            var digestionEnergyBonus = rules.DigestionEnergyBonus;
            var crowdingTolerance = rules.CrowdingTolerance;
            var fleeMovementSpeedBonus = rules.FleeMovementSpeedBonus;

            checked
            {
                switch (definition.Target)
                {
                case SpeciesAttributeTarget.MovementSpeed:
                    movementSpeed += value;
                    break;
                case SpeciesAttributeTarget.AttackAmount:
                    attackAmount += (int)value;
                    break;
                case SpeciesAttributeTarget.AttackModifier:
                    attackModifier += (int)value;
                    break;
                case SpeciesAttributeTarget.DamageAmount:
                    damageAmount += (int)value;
                    break;
                case SpeciesAttributeTarget.BlockAmount:
                    blockAmount += (int)value;
                    break;
                case SpeciesAttributeTarget.ReproductionNeighborCount:
                    reproductionNeighborCount += (int)value;
                    break;
                case SpeciesAttributeTarget.ReproductionChance:
                    reproductionChance += value;
                    break;
                case SpeciesAttributeTarget.ReproductionFoodRequired:
                    reproductionFoodRequired += (int)value;
                    break;
                case SpeciesAttributeTarget.MaxReproductionGroupSize:
                    maxReproductionGroupSize += (int)value;
                    break;
                case SpeciesAttributeTarget.StartingEnergy:
                    startingEnergy += (int)value;
                    break;
                case SpeciesAttributeTarget.ForageBelowEnergy:
                    forageBelowEnergy += (int)value;
                    break;
                case SpeciesAttributeTarget.WiltChance:
                    wiltChance += value;
                    break;
                case SpeciesAttributeTarget.CrowdingEnergyPenalty:
                    crowdingEnergyPenalty += (int)value;
                    break;
                case SpeciesAttributeTarget.StartingFoodReserve:
                    startingFoodReserve += value;
                    break;
                case SpeciesAttributeTarget.SeedDropChance:
                    seedDropChance += value;
                    break;
                case SpeciesAttributeTarget.EnergyValue:
                    energyValue += (int)value;
                    break;
                case SpeciesAttributeTarget.Metabolism:
                    metabolism += (int)value;
                    break;
                case SpeciesAttributeTarget.VisionRange:
                    visionRange += (int)value;
                    break;
                case SpeciesAttributeTarget.Intelligence:
                    intelligence += (int)value;
                    break;
                case SpeciesAttributeTarget.MaximumEnergy:
                    maximumEnergy += (int)value;
                    break;
                case SpeciesAttributeTarget.LitterMinimum:
                    litterMinimum += (int)value;
                    break;
                case SpeciesAttributeTarget.LitterMaximum:
                    litterMaximum += (int)value;
                    break;
                case SpeciesAttributeTarget.DigestionEnergyBonus:
                    digestionEnergyBonus += (int)value;
                    break;
                case SpeciesAttributeTarget.CrowdingTolerance:
                    crowdingTolerance += (int)value;
                    break;
                case SpeciesAttributeTarget.FleeMovementSpeedBonus:
                    fleeMovementSpeedBonus += value;
                    break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(definition.Target), definition.Target, "Unknown species attribute target.");
                }
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
                reproductionNeighborCount,
                reproductionChance,
                reproductionFoodRequired,
                maxReproductionGroupSize,
                startingEnergy,
                wiltChance,
                crowdingEnergyPenalty,
                startingFoodReserve,
                seedDropChance,
                energyValue,
                metabolism,
                new SpeciesAwarenessRules(visionRange, intelligence),
                rules.Role,
                forageBelowEnergy,
                maximumEnergy,
                litterMinimum,
                litterMaximum,
                attackModifier,
                damageAmount,
                digestionEnergyBonus,
                crowdingTolerance,
                fleeMovementSpeedBonus);
        }

        static IReadOnlyDictionary<string, SpeciesAttributeDefinition> CreateDefinitions()
        {
            var values = new[]
            {
                Definition(SpeciesAttributeIds.MovementSpeed, "Movement Speed", SpeciesAttributeValueKind.Float, SpeciesAttributeTarget.MovementSpeed),
                Definition(SpeciesAttributeIds.AttackAmount, "Attack Amount", SpeciesAttributeValueKind.Integer, SpeciesAttributeTarget.AttackAmount),
                Definition(SpeciesAttributeIds.AttackModifier, "Attack Modifier", SpeciesAttributeValueKind.Integer, SpeciesAttributeTarget.AttackModifier),
                Definition(SpeciesAttributeIds.DamageAmount, "Damage Amount", SpeciesAttributeValueKind.Integer, SpeciesAttributeTarget.DamageAmount),
                Definition(SpeciesAttributeIds.BlockAmount, "Block Amount", SpeciesAttributeValueKind.Integer, SpeciesAttributeTarget.BlockAmount),
                Definition(SpeciesAttributeIds.ReproductionNeighborCount, "Reproduction Neighbor Count", SpeciesAttributeValueKind.Integer, SpeciesAttributeTarget.ReproductionNeighborCount),
                Definition(SpeciesAttributeIds.ReproductionChance, "Reproduction Chance", SpeciesAttributeValueKind.Float, SpeciesAttributeTarget.ReproductionChance),
                Definition(SpeciesAttributeIds.ReproductionFoodRequired, "Reproduction Food Required", SpeciesAttributeValueKind.Integer, SpeciesAttributeTarget.ReproductionFoodRequired),
                Definition(SpeciesAttributeIds.MaxReproductionGroupSize, "Reproduction Group Size", SpeciesAttributeValueKind.Integer, SpeciesAttributeTarget.MaxReproductionGroupSize),
                Definition(SpeciesAttributeIds.StartingEnergy, "Starting Energy", SpeciesAttributeValueKind.Integer, SpeciesAttributeTarget.StartingEnergy),
                Definition(SpeciesAttributeIds.ForageBelowEnergy, "Forage Energy Threshold", SpeciesAttributeValueKind.Integer, SpeciesAttributeTarget.ForageBelowEnergy),
                Definition(SpeciesAttributeIds.WiltChance, "Wilt Chance", SpeciesAttributeValueKind.Float, SpeciesAttributeTarget.WiltChance),
                Definition(SpeciesAttributeIds.CrowdingEnergyPenalty, "Crowding Energy Penalty", SpeciesAttributeValueKind.Integer, SpeciesAttributeTarget.CrowdingEnergyPenalty),
                Definition(SpeciesAttributeIds.StartingFoodReserve, "Starting Food Reserve", SpeciesAttributeValueKind.Float, SpeciesAttributeTarget.StartingFoodReserve),
                Definition(SpeciesAttributeIds.SeedDropChance, "Seed Drop Chance", SpeciesAttributeValueKind.Float, SpeciesAttributeTarget.SeedDropChance),
                Definition(SpeciesAttributeIds.EnergyValue, "Energy Value", SpeciesAttributeValueKind.Integer, SpeciesAttributeTarget.EnergyValue),
                Definition(SpeciesAttributeIds.Metabolism, "Metabolism", SpeciesAttributeValueKind.Integer, SpeciesAttributeTarget.Metabolism),
                Definition(SpeciesAttributeIds.VisionRange, "Vision Range", SpeciesAttributeValueKind.Integer, SpeciesAttributeTarget.VisionRange),
                Definition(SpeciesAttributeIds.Intelligence, "Intelligence", SpeciesAttributeValueKind.Integer, SpeciesAttributeTarget.Intelligence),
                Definition(SpeciesAttributeIds.MaximumEnergy, "Maximum Energy", SpeciesAttributeValueKind.Integer, SpeciesAttributeTarget.MaximumEnergy),
                Definition(SpeciesAttributeIds.LitterMinimum, "Minimum Litter", SpeciesAttributeValueKind.Integer, SpeciesAttributeTarget.LitterMinimum),
                Definition(SpeciesAttributeIds.LitterMaximum, "Maximum Litter", SpeciesAttributeValueKind.Integer, SpeciesAttributeTarget.LitterMaximum),
                Definition(SpeciesAttributeIds.DigestionEnergyBonus, "Digestion Energy Bonus", SpeciesAttributeValueKind.Integer, SpeciesAttributeTarget.DigestionEnergyBonus),
                Definition(SpeciesAttributeIds.CrowdingTolerance, "Crowding Tolerance", SpeciesAttributeValueKind.Integer, SpeciesAttributeTarget.CrowdingTolerance),
                Definition(SpeciesAttributeIds.FleeMovementSpeedBonus, "Flee Movement Speed Bonus", SpeciesAttributeValueKind.Float, SpeciesAttributeTarget.FleeMovementSpeedBonus),
            };

            var result = new Dictionary<string, SpeciesAttributeDefinition>(StringComparer.Ordinal);
            foreach (var definition in values)
            {
                if (result.ContainsKey(definition.Id))
                {
                    throw new InvalidOperationException($"Duplicate species attribute id '{definition.Id}'.");
                }

                result.Add(definition.Id, definition);
            }

            return result;
        }

        static string CreateFingerprint(IReadOnlyDictionary<string, SpeciesAttributeDefinition> source)
        {
            var entries = new List<SpeciesAttributeDefinition>(source.Values);
            entries.Sort((left, right) => string.CompareOrdinal(left.Id, right.Id));
            var canonical = new StringBuilder(1024);
            Append(canonical, Version);
            foreach (var entry in entries)
            {
                Append(canonical, entry.Id);
                Append(canonical, entry.DisplayName);
                Append(canonical, (int)entry.ValueKind);
                Append(canonical, (int)entry.Target);
            }

            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(canonical.ToString()));
                var result = new StringBuilder(bytes.Length * 2);
                foreach (var value in bytes)
                {
                    result.Append(value.ToString("x2", CultureInfo.InvariantCulture));
                }

                return result.ToString();
            }
        }

        static IReadOnlyList<SpeciesAttributeDefinition> CreateDefinitionList(
            IReadOnlyDictionary<string, SpeciesAttributeDefinition> source)
        {
            var entries = new List<SpeciesAttributeDefinition>(source.Values);
            entries.Sort((left, right) => string.CompareOrdinal(left.Id, right.Id));
            return entries.AsReadOnly();
        }

        static void Append(StringBuilder builder, string value)
        {
            builder.Append(value?.Length ?? 0).Append(':').Append(value).Append(';');
        }

        static void Append(StringBuilder builder, int value)
        {
            Append(builder, value.ToString(CultureInfo.InvariantCulture));
        }

        static SpeciesAttributeDefinition Definition(
            string id,
            string displayName,
            SpeciesAttributeValueKind valueKind,
            SpeciesAttributeTarget target)
        {
            return new SpeciesAttributeDefinition(id, displayName, valueKind, target);
        }
    }
}
