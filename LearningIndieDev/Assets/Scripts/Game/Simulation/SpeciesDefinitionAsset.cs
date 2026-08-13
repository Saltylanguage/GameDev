using System;
using UnityEngine;

namespace SaltyGame
{
    /// <summary>
    /// Reusable, inspector-authored species definition. Category assets only
    /// supply the role; the shared rule surface lives here once.
    /// </summary>
    public abstract class SpeciesDefinitionAsset : ScriptableObject
    {
        [SerializeField] string id;
        [SerializeField, Range(0f, 1f)] float startingProbability;
        [SerializeField, Min(0f)] float movementSpeed;
        [SerializeField] Vector2Int[] movementPattern;
        [SerializeField] Vector2Int[] attackPattern;
        [SerializeField, Min(0)] int attackAmount;
        [SerializeField] Vector2Int[] blockPattern;
        [SerializeField, Min(0)] int blockAmount;
        [SerializeField] Vector2Int[] dietPattern;
        [SerializeField] string dietTargetId;
        [SerializeField] Vector2Int[] reproductionPattern;
        [SerializeField, Min(0)] int reproductionNeighborCount;
        [SerializeField, Range(0f, 1f)] float reproductionChance;
        [SerializeField, Min(0)] int reproductionFoodRequired;
        [SerializeField, Min(0)] int maxReproductionGroupSize;
        [SerializeField, Min(0)] int startingEnergy;
        [SerializeField, Range(0f, 1f)] float wiltChance;
        [SerializeField, Min(0)] int crowdingEnergyPenalty;
        [SerializeField, Min(0f)] float startingFoodReserve;
        [SerializeField, Range(0f, 1f)] float seedDropChance;
        [SerializeField, Min(0)] int energyValue;
        [SerializeField] int metabolism = 1;
        [SerializeField, Min(0)] int visionRange;
        [SerializeField, Min(0)] int intelligence;
        [SerializeField, Range(0f, 1f)] float alphaChance;
        [SerializeField, Min(0)] int alphaHealthBonus;
        [SerializeField, Min(0)] int alphaEnergyBonus;

        public SpeciesId Id => new SpeciesId(id);
        public float StartingProbability => startingProbability;
        public SpeciesRole Role => GetRole();

        protected abstract SpeciesRole GetRole();

        public SpeciesRules CreateRules()
        {
            return new SpeciesRules(
                movementSpeed,
                new GridPattern(movementPattern ?? Array.Empty<Vector2Int>()),
                new GridPattern(attackPattern ?? Array.Empty<Vector2Int>()),
                attackAmount,
                new GridPattern(blockPattern ?? Array.Empty<Vector2Int>()),
                blockAmount,
                new GridPattern(dietPattern ?? Array.Empty<Vector2Int>()),
                string.IsNullOrWhiteSpace(dietTargetId) ? (SpeciesId?)null : new SpeciesId(dietTargetId),
                new GridPattern(reproductionPattern ?? Array.Empty<Vector2Int>()),
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
                Role);
        }

        public bool TryCreateAlphaRule(out AlphaOffspringRule rule)
        {
            if (alphaChance <= 0f)
            {
                rule = null;
                return false;
            }

            rule = new AlphaOffspringRule(Id, alphaChance, alphaHealthBonus, alphaEnergyBonus);
            return true;
        }
    }

}
