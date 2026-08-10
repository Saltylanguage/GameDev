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
                    reproductionNeighborCount: 1),
                [SpeciesArchetype.Herbivore] = new SpeciesRules(
                    movementSpeed: 1f,
                    movementPattern: CardinalPattern,
                    attackPattern: CardinalPattern,
                    attackAmount: 1,
                    blockPattern: CardinalPattern,
                    blockAmount: 0,
                    dietPattern: CardinalPattern,
                    dietTarget: SpeciesArchetype.Plant,
                    reproductionPattern: CardinalPattern,
                    reproductionNeighborCount: 1),
                [SpeciesArchetype.Carnivore] = new SpeciesRules(
                    movementSpeed: 1f,
                    movementPattern: MoorePattern,
                    attackPattern: MoorePattern,
                    attackAmount: 2,
                    blockPattern: CardinalPattern,
                    blockAmount: 0,
                    dietPattern: MoorePattern,
                    dietTarget: SpeciesArchetype.Herbivore,
                    reproductionPattern: MoorePattern,
                    reproductionNeighborCount: 1),
            };
        }
    }
}
