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
            [SerializeField, Min(0)] int startingPopulation;

            public SpeciesEntry(
                SpeciesDefinitionAsset definition,
                float startingProbability,
                int startingPopulation = 0)
            {
                if (startingProbability < 0f || startingProbability > 1f)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(startingProbability),
                        startingProbability,
                        "Starting probability must be between zero and one.");
                }

                if (startingPopulation < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(startingPopulation),
                        startingPopulation,
                        "Starting population cannot be negative.");
                }

                this.definition = definition;
                this.startingProbability = startingProbability;
                this.startingPopulation = startingPopulation;
            }

            public SpeciesDefinitionAsset Definition => definition;
            public float StartingProbability => startingProbability;
            public int StartingPopulation => startingPopulation;
        }

        [SerializeField, Min(1)] int width = 32;
        [SerializeField, Min(1)] int height = 20;
        [SerializeField, Min(0.01f)] float runDurationSeconds = 20f;
        [SerializeField, Min(0.01f)] float stepInterval = 0.1f;
        [SerializeField, Min(0)]
        [Tooltip("Exact ticks per run. Zero keeps the legacy duration field; the default 20 seconds at a 0.1 second step is 200 ticks.")]
        int runTicks;
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
            var startingPopulations = new Dictionary<SpeciesId, int>();
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
                if (entry.StartingPopulation > 0)
                {
                    startingPopulations.Add(id, entry.StartingPopulation);
                }
                rules.Add(id, definition.CreateRules());
                if (definition.TryCreateAlphaRule(out var alphaRule))
                {
                    alphaRules.Add(id, alphaRule);
                }
            }

            var effectiveRunDurationSeconds = runTicks > 0
                ? (float)(runTicks * (double)stepInterval)
                : runDurationSeconds;
            return new CellularSimData(
                width,
                height,
                probabilities,
                rules,
                effectiveRunDurationSeconds,
                stepInterval,
                maxPopulation,
                minPopulation,
                alphaOffspringRules: alphaRules,
                startingPopulations: startingPopulations);
        }
    }
}
