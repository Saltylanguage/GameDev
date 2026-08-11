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
            int metabolism = 1)
        {
            if (movementSpeed < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(movementSpeed), movementSpeed, "Movement speed cannot be negative.");
            }

            if (attackAmount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(attackAmount), attackAmount, "Attack amount cannot be negative.");
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
            WiltChance = wiltChance;
            CrowdingEnergyPenalty = crowdingEnergyPenalty;
            StartingFoodReserve = startingFoodReserve;
            SeedDropChance = seedDropChance;
            EnergyValue = energyValue;
            Metabolism = metabolism;
        }

        public float MovementSpeed { get; }
        public GridPattern MovementPattern { get; }
        public GridPattern AttackPattern { get; }
        public int AttackAmount { get; }
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
        public float WiltChance { get; }
        public int CrowdingEnergyPenalty { get; }
        public int CrowdingCost => CrowdingEnergyPenalty;
        public float StartingFoodReserve { get; }
        public float SeedDropChance { get; }
        public int EnergyValue { get; }
        public int Metabolism { get; }
    }
}
