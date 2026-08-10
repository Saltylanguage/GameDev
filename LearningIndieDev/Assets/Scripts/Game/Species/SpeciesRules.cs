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
            SpeciesArchetype? dietTarget,
            GridPattern reproductionPattern,
            int reproductionNeighborCount)
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

            MovementSpeed = movementSpeed;
            MovementPattern = movementPattern ?? throw new ArgumentNullException(nameof(movementPattern));
            AttackPattern = attackPattern ?? throw new ArgumentNullException(nameof(attackPattern));
            AttackAmount = attackAmount;
            BlockPattern = blockPattern ?? throw new ArgumentNullException(nameof(blockPattern));
            BlockAmount = blockAmount;
            DietPattern = dietPattern ?? throw new ArgumentNullException(nameof(dietPattern));
            DietTarget = dietTarget;
            ReproductionPattern = reproductionPattern ?? throw new ArgumentNullException(nameof(reproductionPattern));
            ReproductionNeighborCount = reproductionNeighborCount;
        }

        public float MovementSpeed { get; }
        public GridPattern MovementPattern { get; }
        public GridPattern AttackPattern { get; }
        public int AttackAmount { get; }
        public GridPattern BlockPattern { get; }
        public int BlockAmount { get; }
        public GridPattern DietPattern { get; }
        public SpeciesArchetype? DietTarget { get; }
        public GridPattern ReproductionPattern { get; }
        public int ReproductionNeighborCount { get; }
    }
}
