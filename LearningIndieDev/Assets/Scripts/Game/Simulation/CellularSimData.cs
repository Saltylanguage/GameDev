using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SaltyGame
{
    public sealed class CellularSimData
    {
        readonly IReadOnlyDictionary<SpeciesId, float> startingProbabilities;
        readonly IReadOnlyDictionary<SpeciesId, SpeciesRules> speciesRules;

        public CellularSimData(
            int width,
            int height,
            IReadOnlyDictionary<SpeciesId, float> startingProbabilities,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> speciesRules,
            float runDurationSeconds,
            float stepInterval,
            int maxPopulation = 0,
            int minPopulation = 0)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), width, "Grid width must be greater than zero.");
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), height, "Grid height must be greater than zero.");
            }

            if (startingProbabilities == null)
            {
                throw new ArgumentNullException(nameof(startingProbabilities));
            }

            if (speciesRules == null)
            {
                throw new ArgumentNullException(nameof(speciesRules));
            }

            if (runDurationSeconds <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(runDurationSeconds), runDurationSeconds, "Run duration must be greater than zero.");
            }

            if (stepInterval <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(stepInterval), stepInterval, "Simulation step must be greater than zero.");
            }

            if (maxPopulation < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxPopulation), maxPopulation, "Maximum population cannot be negative.");
            }

            if (minPopulation < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minPopulation), minPopulation, "Minimum population cannot be negative.");
            }

            var copiedProbabilities = new Dictionary<SpeciesId, float>(startingProbabilities.Count);
            foreach (var entry in startingProbabilities)
            {
                if (!entry.Key.IsValid)
                {
                    throw new ArgumentException("Starting probabilities cannot use an empty species id.", nameof(startingProbabilities));
                }

                if (entry.Value < 0f || entry.Value > 1f)
                {
                    throw new ArgumentOutOfRangeException(nameof(startingProbabilities), entry.Value, "Starting probabilities must be between zero and one.");
                }

                copiedProbabilities.Add(entry.Key, entry.Value);
            }

            var copiedRules = new Dictionary<SpeciesId, SpeciesRules>(speciesRules.Count);
            foreach (var entry in speciesRules)
            {
                if (!entry.Key.IsValid)
                {
                    throw new ArgumentException("Species rules cannot use an empty species id.", nameof(speciesRules));
                }

                if (entry.Value == null)
                {
                    throw new ArgumentException("Species rules cannot contain null values.", nameof(speciesRules));
                }

                copiedRules.Add(entry.Key, entry.Value);
            }

            foreach (var entry in copiedRules)
            {
                if (entry.Value.DietTargetId.HasValue && !copiedRules.ContainsKey(entry.Value.DietTargetId.Value))
                {
                    throw new ArgumentException(
                        $"Species {entry.Key} targets missing diet species {entry.Value.DietTargetId.Value}.",
                        nameof(speciesRules));
                }
            }

            foreach (var entry in copiedProbabilities)
            {
                if (!copiedRules.ContainsKey(entry.Key))
                {
                    throw new ArgumentException(
                        $"Starting probability is configured for undefined species {entry.Key}.",
                        nameof(startingProbabilities));
                }
            }

            Width = width;
            Height = height;
            this.startingProbabilities = new ReadOnlyDictionary<SpeciesId, float>(copiedProbabilities);
            this.speciesRules = new ReadOnlyDictionary<SpeciesId, SpeciesRules>(copiedRules);
            RunDurationSeconds = runDurationSeconds;
            StepInterval = stepInterval;
            MaxPopulation = maxPopulation;
            MinPopulation = minPopulation;
        }

        [Obsolete("Use the SpeciesId-keyed constructor instead.")]
        public CellularSimData(
            int width,
            int height,
            IReadOnlyDictionary<SpeciesArchetype, float> startingProbabilities,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> speciesRules,
            float runDurationSeconds,
            float stepInterval,
            int maxPopulation = 0,
            int minPopulation = 0)
            : this(
                width,
                height,
                SpeciesIdConversions.FromLegacy(startingProbabilities),
                speciesRules,
                runDurationSeconds,
                stepInterval,
                maxPopulation,
                minPopulation)
        {
        }

        [Obsolete("Use the SpeciesId-keyed constructor instead.")]
        public CellularSimData(
            int width,
            int height,
            IReadOnlyDictionary<SpeciesArchetype, float> startingProbabilities,
            IReadOnlyDictionary<SpeciesArchetype, SpeciesRules> speciesRules,
            float runDurationSeconds,
            float stepInterval,
            int maxPopulation = 0,
            int minPopulation = 0)
            : this(
                width,
                height,
                SpeciesIdConversions.FromLegacy(startingProbabilities),
                SpeciesIdConversions.FromLegacy(speciesRules),
                runDurationSeconds,
                stepInterval,
                maxPopulation,
                minPopulation)
        {
        }

        public int Width { get; }
        public int Height { get; }
        public float RunDurationSeconds { get; }
        public float StepInterval { get; }
        public int MaxPopulation { get; }
        public int MinPopulation { get; }
        public IReadOnlyDictionary<SpeciesId, float> StartingProbabilities => startingProbabilities;
        public IReadOnlyDictionary<SpeciesId, SpeciesRules> SpeciesRules => speciesRules;

        public bool TryGetStartingProbability(SpeciesId species, out float probability)
        {
            return startingProbabilities.TryGetValue(species, out probability);
        }

        public CellularSimData WithSpeciesRules(SpeciesId species, SpeciesRules rules)
        {
            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            var updatedRules = Copy(speciesRules);
            updatedRules[species] = rules;
            return CreateUpdated(startingProbabilities, updatedRules);
        }

        public CellularSimData WithSpecies(
            SpeciesId species,
            SpeciesRules rules,
            float startingProbability)
        {
            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            var updatedRules = Copy(speciesRules);
            updatedRules[species] = rules;
            var updatedProbabilities = Copy(startingProbabilities);
            updatedProbabilities[species] = startingProbability;
            return CreateUpdated(updatedProbabilities, updatedRules);
        }

        public CellularSimData WithoutSpecies(SpeciesId species)
        {
            var updatedRules = Copy(speciesRules);
            if (!updatedRules.Remove(species))
            {
                return this;
            }

            var updatedProbabilities = Copy(startingProbabilities);
            updatedProbabilities.Remove(species);
            return CreateUpdated(updatedProbabilities, updatedRules);
        }

        public CellularSimData WithStartingProbability(SpeciesId species, float probability)
        {
            if (!speciesRules.ContainsKey(species))
            {
                throw new InvalidOperationException(
                    $"Cannot set a starting probability for undefined species {species}. Add the species rules first.");
            }

            var updatedProbabilities = Copy(startingProbabilities);
            updatedProbabilities[species] = probability;
            return CreateUpdated(updatedProbabilities, speciesRules);
        }

        public CellularSimData Copy()
        {
            return CreateUpdated(startingProbabilities, speciesRules);
        }

        CellularSimData CreateUpdated(
            IReadOnlyDictionary<SpeciesId, float> updatedProbabilities,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> updatedRules)
        {
            return new CellularSimData(
                Width,
                Height,
                updatedProbabilities,
                updatedRules,
                RunDurationSeconds,
                StepInterval,
                MaxPopulation,
                MinPopulation);
        }

        static Dictionary<SpeciesId, TValue> Copy<TValue>(
            IReadOnlyDictionary<SpeciesId, TValue> source)
        {
            var copy = new Dictionary<SpeciesId, TValue>(source.Count);
            foreach (var entry in source)
            {
                copy.Add(entry.Key, entry.Value);
            }

            return copy;
        }
    }
}
