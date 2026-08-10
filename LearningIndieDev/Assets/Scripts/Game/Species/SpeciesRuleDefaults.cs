using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
    public static class SpeciesRuleDefaults
    {
        static readonly GridPattern CardinalPattern = new GridPattern(new[]
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left,
        });

        static readonly GridPattern MoorePattern = new GridPattern(new[]
        {
            new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1),
            new Vector2Int(-1, 0), new Vector2Int(1, 0),
            new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1),
        });

        public static IReadOnlyDictionary<SpeciesArchetype, SpeciesRules> Create()
        {
            return new Dictionary<SpeciesArchetype, SpeciesRules>
            {
                [SpeciesArchetype.Plant] = new SpeciesRules(
                    movementSpeed: 0f,
                    movementPattern: CardinalPattern,
                    attackPattern: CardinalPattern,
                    attackAmount: 0,
                    blockPattern: CardinalPattern,
                    blockAmount: 0,
                    dietPattern: CardinalPattern,
                    dietTarget: null,
                    reproductionPattern: CardinalPattern,
                    reproductionNeighborCount: 0,
                    reproductionChance: 0.1f,
                    startingEnergy: 0,
                    wiltChance: 0.003f,
                    startingFoodReserve: 3.25f,
                    energyValue: 1,
                    metabolism: -1),
                [SpeciesArchetype.Herbivore] = new SpeciesRules(
                    movementSpeed: 1.5f,
                    movementPattern: CardinalPattern,
                    attackPattern: CardinalPattern,
                    attackAmount: 1,
                    blockPattern: CardinalPattern,
                    blockAmount: 0,
                    dietPattern: MoorePattern,
                    dietTarget: SpeciesArchetype.Plant,
                    reproductionPattern: MoorePattern,
                    reproductionNeighborCount: 1,
                    reproductionChance: 0.5f,
                    reproductionFoodRequired: 1,
                    maxReproductionGroupSize: 4,
                    startingEnergy: 12,
                    crowdingEnergyPenalty: 1,
                    seedDropChance: 0.05f,
                    energyValue: 4,
                    metabolism: 1),
                [SpeciesArchetype.Carnivore] = new SpeciesRules(
                    movementSpeed: 1.5f,
                    movementPattern: MoorePattern,
                    attackPattern: MoorePattern,
                    attackAmount: 2,
                    blockPattern: CardinalPattern,
                    blockAmount: 0,
                    dietPattern: MoorePattern,
                    dietTarget: SpeciesArchetype.Herbivore,
                    reproductionPattern: CardinalPattern,
                    reproductionNeighborCount: 1,
                    reproductionChance: 0.4f,
                    reproductionFoodRequired: 1,
                    maxReproductionGroupSize: 3,
                    startingEnergy: 18,
                    crowdingEnergyPenalty: 1,
                    energyValue: 8,
                    metabolism: 1),
            };
        }

        public static GridPattern CreateCardinalPattern()
        {
            return CardinalPattern;
        }

        public static GridPattern CreateMoorePattern()
        {
            return MoorePattern;
        }
    }
}
