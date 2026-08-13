using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
    /// <summary>Scenario-level authoring asset composed from reusable species assets.</summary>
    [CreateAssetMenu(menuName = "Salty Game/Cellular Simulation Scenario", fileName = "Scenario")]
    public sealed class ScenarioDefinitionAsset : ScriptableObject
    {
        [Serializable]
        public sealed class SpeciesEntry
        {
            [SerializeField] SpeciesDefinitionAsset definition;
            [SerializeField, Range(0f, 1f)] float startingProbability;

            public SpeciesEntry(SpeciesDefinitionAsset definition, float startingProbability)
            {
                if (startingProbability < 0f || startingProbability > 1f)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(startingProbability),
                        startingProbability,
                        "Starting probability must be between zero and one.");
                }

                this.definition = definition;
                this.startingProbability = startingProbability;
            }

            public SpeciesDefinitionAsset Definition => definition;
            public float StartingProbability => startingProbability;
        }

        [SerializeField, Min(1)] int width = 32;
        [SerializeField, Min(1)] int height = 20;
        [SerializeField, Min(0.01f)] float runDurationSeconds = 20f;
        [SerializeField, Min(0.01f)] float stepInterval = 0.1f;
        [SerializeField, Min(0)] int maxPopulation;
        [SerializeField, Min(0)] int minPopulation;
        [SerializeField] SpeciesEntry[] species = Array.Empty<SpeciesEntry>();

        public IReadOnlyList<SpeciesEntry> Species => species;

        public CellularSimData CreateRuntimeData()
        {
            if (species == null || species.Length == 0)
            {
                throw new InvalidOperationException("Scenario must define at least one species.");
            }

            var probabilities = new Dictionary<SpeciesId, float>(species.Length);
            var rules = new Dictionary<SpeciesId, SpeciesRules>(species.Length);
            var alphaRules = new Dictionary<SpeciesId, AlphaOffspringRule>();
            foreach (var entry in species)
            {
                if (entry == null || entry.Definition == null)
                {
                    throw new InvalidOperationException("Scenario cannot contain an empty species definition.");
                }

                var definition = entry.Definition;
                var id = definition.Id;
                probabilities.Add(id, entry.StartingProbability);
                rules.Add(id, definition.CreateRules());
                if (definition.TryCreateAlphaRule(out var alphaRule))
                {
                    alphaRules.Add(id, alphaRule);
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
    }
}
