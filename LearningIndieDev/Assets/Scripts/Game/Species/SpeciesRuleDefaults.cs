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

        public static IReadOnlyDictionary<SpeciesId, SpeciesRules> Create()
        {
            return new Dictionary<SpeciesId, SpeciesRules>
            {
                [SpeciesIds.Plant] = new SpeciesRules(
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
                    metabolism: -1,
                    awareness: SpeciesAwarenessRules.None),
                [SpeciesIds.Herbivore] = new SpeciesRules(
                    movementSpeed: 1.5f,
                    movementPattern: CardinalPattern,
                    attackPattern: CardinalPattern,
                    attackAmount: 1,
                    blockPattern: CardinalPattern,
                    blockAmount: 0,
                    dietPattern: MoorePattern,
                    dietTarget: SpeciesIds.Plant,
                    reproductionPattern: MoorePattern,
                    reproductionNeighborCount: 1,
                    reproductionChance: 0.5f,
                    reproductionFoodRequired: 1,
                    maxReproductionGroupSize: 4,
                    startingEnergy: 12,
                    crowdingEnergyPenalty: 1,
                    seedDropChance: 0.05f,
                    energyValue: 4,
                    metabolism: 1,
                    awareness: new SpeciesAwarenessRules(visionRange: 5, intelligence: 1)),
                [SpeciesIds.Carnivore] = new SpeciesRules(
                    movementSpeed: 1.5f,
                    movementPattern: MoorePattern,
                    attackPattern: MoorePattern,
                    attackAmount: 2,
                    blockPattern: CardinalPattern,
                    blockAmount: 0,
                    dietPattern: MoorePattern,
                    dietTarget: SpeciesIds.Herbivore,
                    reproductionPattern: CardinalPattern,
                    reproductionNeighborCount: 1,
                    reproductionChance: 0.4f,
                    reproductionFoodRequired: 1,
                    maxReproductionGroupSize: 3,
                    startingEnergy: 18,
                    crowdingEnergyPenalty: 1,
                    energyValue: 8,
                    metabolism: 1,
                    awareness: new SpeciesAwarenessRules(visionRange: 4, intelligence: 1)),
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
