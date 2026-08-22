using System;

namespace SaltyGame
{
    public sealed class SpeciesRules
    {
        public SpeciesRules(
            float movementSpeed,
            GridPattern movementPattern,
            GridPattern attackPattern,
            int attackAmount,
            GridPattern blockPattern,
            int blockAmount,
            GridPattern dietPattern,
            SpeciesId? dietTarget,
            GridPattern reproductionPattern,
            int reproductionNeighborCount,
            float reproductionChance = 0.5f,
            int reproductionFoodRequired = 0,
            int maxReproductionGroupSize = 0,
            int startingEnergy = 0,
            float wiltChance = 0f,
            int crowdingEnergyPenalty = 0,
            float startingFoodReserve = 0f,
            float seedDropChance = 0f,
            int energyValue = 0,
            int metabolism = 1,
            SpeciesAwarenessRules awareness = null,
            SpeciesRole role = SpeciesRole.Herbivore,
            int forageBelowEnergy = 0,
            int maximumEnergy = 0,
            int litterMinimum = 1,
            int litterMaximum = 1,
            int? attackModifier = null,
            int? damageAmount = null)
        {
            if (movementSpeed < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(movementSpeed), movementSpeed, "Movement speed cannot be negative.");
            }

            if (attackAmount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(attackAmount), attackAmount, "Attack amount cannot be negative.");
            }

            if (attackModifier.HasValue && attackModifier.Value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(attackModifier), attackModifier, "Attack modifier cannot be negative.");
            }

            if (damageAmount.HasValue && damageAmount.Value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(damageAmount), damageAmount, "Damage amount cannot be negative.");
            }

            if (blockAmount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockAmount), blockAmount, "Block amount cannot be negative.");
            }

            if (reproductionNeighborCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(reproductionNeighborCount), reproductionNeighborCount, "Reproduction neighbor count cannot be negative.");
            }

            if (reproductionChance < 0f || reproductionChance > 1f)
            {
                throw new ArgumentOutOfRangeException(nameof(reproductionChance), reproductionChance, "Reproduction chance must be between zero and one.");
            }

            if (reproductionFoodRequired < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(reproductionFoodRequired), reproductionFoodRequired, "Reproduction food requirement cannot be negative.");
            }

            if (maxReproductionGroupSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxReproductionGroupSize), maxReproductionGroupSize, "Maximum reproduction group size cannot be negative.");
            }

            if (startingEnergy < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startingEnergy), startingEnergy, "Starting energy cannot be negative.");
            }

            if (forageBelowEnergy < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(forageBelowEnergy), forageBelowEnergy, "Forage energy threshold cannot be negative.");
            }

            if (maximumEnergy < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumEnergy), maximumEnergy, "Maximum energy cannot be negative.");
            }

            if (litterMinimum < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(litterMinimum), litterMinimum, "Minimum litter size must be at least one.");
            }

            if (litterMaximum < litterMinimum)
            {
                throw new ArgumentOutOfRangeException(nameof(litterMaximum), litterMaximum, "Maximum litter size cannot be less than the minimum.");
            }

            if (wiltChance < 0f || wiltChance > 1f)
            {
                throw new ArgumentOutOfRangeException(nameof(wiltChance), wiltChance, "Wilt chance must be between zero and one.");
            }

            if (crowdingEnergyPenalty < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(crowdingEnergyPenalty), crowdingEnergyPenalty, "Crowding energy penalty cannot be negative.");
            }

            if (startingFoodReserve < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(startingFoodReserve), startingFoodReserve, "Starting food reserve cannot be negative.");
            }

            if (seedDropChance < 0f || seedDropChance > 1f)
            {
                throw new ArgumentOutOfRangeException(nameof(seedDropChance), seedDropChance, "Seed drop chance must be between zero and one.");
            }

            if (energyValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(energyValue), energyValue, "Energy value cannot be negative.");
            }

            MovementSpeed = movementSpeed;
            MovementPattern = movementPattern ?? throw new ArgumentNullException(nameof(movementPattern));
            AttackPattern = attackPattern ?? throw new ArgumentNullException(nameof(attackPattern));
            AttackAmount = attackAmount;
            AttackModifier = attackModifier ?? attackAmount;
            DamageAmount = damageAmount ?? attackAmount;
            BlockPattern = blockPattern ?? throw new ArgumentNullException(nameof(blockPattern));
            BlockAmount = blockAmount;
            DietPattern = dietPattern ?? throw new ArgumentNullException(nameof(dietPattern));
            DietTargetId = dietTarget;
            ReproductionPattern = reproductionPattern ?? throw new ArgumentNullException(nameof(reproductionPattern));
            ReproductionNeighborCount = reproductionNeighborCount;
            ReproductionChance = reproductionChance;
            ReproductionFoodRequired = reproductionFoodRequired;
            MaxReproductionGroupSize = maxReproductionGroupSize;
            StartingEnergy = startingEnergy;
            ForageBelowEnergy = forageBelowEnergy;
            WiltChance = wiltChance;
            CrowdingEnergyPenalty = crowdingEnergyPenalty;
            StartingFoodReserve = startingFoodReserve;
            SeedDropChance = seedDropChance;
            EnergyValue = energyValue;
            Metabolism = metabolism;
            Awareness = awareness ?? SpeciesAwarenessRules.None;
            Role = role;
            MaximumEnergy = maximumEnergy;
            LitterMinimum = litterMinimum;
            LitterMaximum = litterMaximum;
        }

        public float MovementSpeed { get; }
        public GridPattern MovementPattern { get; }
        public GridPattern AttackPattern { get; }
        public int AttackAmount { get; }
        public int AttackModifier { get; }
        public int DamageAmount { get; }
        public GridPattern BlockPattern { get; }
        public int BlockAmount { get; }
        public GridPattern DietPattern { get; }
        public SpeciesId? DietTargetId { get; }
        [Obsolete("Use DietTargetId instead.")]
        public SpeciesArchetype? DietTarget => DietTargetId.HasValue
            ? SpeciesId.ToLegacyArchetype(DietTargetId.Value)
            : (SpeciesArchetype?)null;
        public GridPattern ReproductionPattern { get; }
        public int ReproductionNeighborCount { get; }
        public float ReproductionChance { get; }
        public int ReproductionFoodRequired { get; }
        public int MaxReproductionGroupSize { get; }
        public int StartingEnergy { get; }
        public int ForageBelowEnergy { get; }
        public float WiltChance { get; }
        public int CrowdingEnergyPenalty { get; }
        public int CrowdingCost => CrowdingEnergyPenalty;
        public float StartingFoodReserve { get; }
        public float SeedDropChance { get; }
        public int EnergyValue { get; }
        public int Metabolism { get; }
        public SpeciesAwarenessRules Awareness { get; }
        public SpeciesRole Role { get; }
        public bool IsPlant => Role == SpeciesRole.Plant;
        public int MaximumEnergy { get; }
        public int LitterMinimum { get; }
        public int LitterMaximum { get; }
    }
}
