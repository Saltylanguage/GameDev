using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
    /// <summary>Inspector-authored template that creates an immutable runtime ruleset.</summary>
    [CreateAssetMenu(menuName = "Salty Game/Cellular Simulation Data", fileName = "CellularSimData")]
    public sealed class CellularSimDataAsset : ScriptableObject
    {
        [SerializeField, Min(1)] int width = 32;
        [SerializeField, Min(1)] int height = 20;
        [SerializeField, Min(0.01f)] float runDurationSeconds = 20f;
        [SerializeField, Min(0.01f)] float stepInterval = 0.1f;
        [SerializeField, Min(0)] int maxPopulation;
        [SerializeField, Min(0)] int minPopulation;
        [SerializeField] SpeciesDefinition[] species = CreateDefaultSpecies();

        public CellularSimData CreateRuntimeData()
        {
            if (species == null || species.Length == 0)
            {
                throw new InvalidOperationException("Cellular simulation data must define at least one species.");
            }

            var probabilities = new Dictionary<SpeciesId, float>(species.Length);
            var rules = new Dictionary<SpeciesId, SpeciesRules>(species.Length);
            var alphaRules = new Dictionary<SpeciesId, AlphaOffspringRule>();
            foreach (var definition in species)
            {
                if (definition == null)
                {
                    throw new InvalidOperationException("Cellular simulation data cannot contain an empty species definition.");
                }

                var speciesId = new SpeciesId(definition.id);
                probabilities.Add(speciesId, definition.startingProbability);
                rules.Add(speciesId, definition.CreateRules());
                if (definition.alphaChance > 0f)
                {
                    alphaRules.Add(speciesId, new AlphaOffspringRule(
                        speciesId,
                        definition.alphaChance,
                        definition.alphaHealthBonus,
                        definition.alphaEnergyBonus));
                }
            }

            return new CellularSimData(
                width,
                height,
                probabilities,
                rules,
                runDurationSeconds,
                stepInterval,
                maxPopulation,
                minPopulation,
                alphaOffspringRules: alphaRules);
        }

        void Reset()
        {
            species = CreateDefaultSpecies();
        }

        static SpeciesDefinition[] CreateDefaultSpecies()
        {
            var defaults = SpeciesRuleDefaults.Create();
            return new[]
            {
                SpeciesDefinition.From(SpeciesIds.Plant, 0.4f, defaults[SpeciesIds.Plant]),
                SpeciesDefinition.From(SpeciesIds.Herbivore, 0.16f, defaults[SpeciesIds.Herbivore]),
                SpeciesDefinition.From(SpeciesIds.Carnivore, 0.04f, defaults[SpeciesIds.Carnivore]),
            };
        }

        [Serializable]
        sealed class SpeciesDefinition
        {
            [SerializeField] internal string id;
            [SerializeField, Range(0f, 1f)] internal float startingProbability;
            [SerializeField, Min(0f)] internal float movementSpeed;
            [SerializeField] internal Vector2Int[] movementPattern;
            [SerializeField] internal Vector2Int[] attackPattern;
            [SerializeField, Min(0)] internal int attackAmount;
            [SerializeField] internal Vector2Int[] blockPattern;
            [SerializeField, Min(0)] internal int blockAmount;
            [SerializeField] internal Vector2Int[] dietPattern;
            [SerializeField] internal string dietTargetId;
            [SerializeField] internal Vector2Int[] reproductionPattern;
            [SerializeField, Min(0)] internal int reproductionNeighborCount;
            [SerializeField, Range(0f, 1f)] internal float reproductionChance;
            [SerializeField, Min(0)] internal int reproductionFoodRequired;
            [SerializeField, Min(0)] internal int maxReproductionGroupSize;
            [SerializeField, Min(0)] internal int startingEnergy;
            [SerializeField, Range(0f, 1f)] internal float wiltChance;
            [SerializeField, Min(0)] internal int crowdingEnergyPenalty;
            [SerializeField, Min(0f)] internal float startingFoodReserve;
            [SerializeField, Range(0f, 1f)] internal float seedDropChance;
            [SerializeField, Min(0)] internal int energyValue;
            [SerializeField] internal int metabolism = 1;
            [Header("Awareness")]
            [SerializeField, Min(0)] internal int visionRange;
            [SerializeField, Min(0)] internal int intelligence;
            [Header("Alpha Offspring")]
            [SerializeField, Range(0f, 1f)] internal float alphaChance;
            [SerializeField, Min(0)] internal int alphaHealthBonus;
            [SerializeField, Min(0)] internal int alphaEnergyBonus;
            [SerializeField] internal SpeciesRole role;

            internal SpeciesRules CreateRules()
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
                    role);
            }

            internal static SpeciesDefinition From(SpeciesId species, float probability, SpeciesRules rules)
            {
                return new SpeciesDefinition
                {
                    id = species.Value,
                    startingProbability = probability,
                    movementSpeed = rules.MovementSpeed,
                    movementPattern = Copy(rules.MovementPattern),
                    attackPattern = Copy(rules.AttackPattern),
                    attackAmount = rules.AttackAmount,
                    blockPattern = Copy(rules.BlockPattern),
                    blockAmount = rules.BlockAmount,
                    dietPattern = Copy(rules.DietPattern),
                    dietTargetId = rules.DietTargetId?.Value,
                    reproductionPattern = Copy(rules.ReproductionPattern),
                    reproductionNeighborCount = rules.ReproductionNeighborCount,
                    reproductionChance = rules.ReproductionChance,
                    reproductionFoodRequired = rules.ReproductionFoodRequired,
                    maxReproductionGroupSize = rules.MaxReproductionGroupSize,
                    startingEnergy = rules.StartingEnergy,
                    wiltChance = rules.WiltChance,
                    crowdingEnergyPenalty = rules.CrowdingEnergyPenalty,
                    startingFoodReserve = rules.StartingFoodReserve,
                    seedDropChance = rules.SeedDropChance,
                    energyValue = rules.EnergyValue,
                    metabolism = rules.Metabolism,
                    visionRange = rules.Awareness.VisionRange,
                    intelligence = rules.Awareness.Intelligence,
                    role = rules.Role,
                };
            }

            static Vector2Int[] Copy(GridPattern pattern)
            {
                var offsets = new Vector2Int[pattern.Count];
                for (var index = 0; index < offsets.Length; index++)
                {
                    offsets[index] = pattern.Offsets[index];
                }

                return offsets;
            }
        }
    }
}
