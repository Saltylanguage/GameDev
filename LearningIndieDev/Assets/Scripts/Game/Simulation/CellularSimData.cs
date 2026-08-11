using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SaltyGame
{
    public sealed class CellularSimData
    {
        readonly IReadOnlyDictionary<SpeciesArchetype, float> startingProbabilities;
        readonly IReadOnlyDictionary<SpeciesArchetype, SpeciesRules> speciesRules;

        public CellularSimData(
            int width,
            int height,
            IReadOnlyDictionary<SpeciesArchetype, float> startingProbabilities,
            IReadOnlyDictionary<SpeciesArchetype, SpeciesRules> speciesRules,
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

            var copiedProbabilities = new Dictionary<SpeciesArchetype, float>(startingProbabilities.Count);
            foreach (var entry in startingProbabilities)
            {
                if (entry.Value < 0f || entry.Value > 1f)
                {
                    throw new ArgumentOutOfRangeException(nameof(startingProbabilities), entry.Value, "Starting probabilities must be between zero and one.");
                }

                copiedProbabilities.Add(entry.Key, entry.Value);
            }

            var copiedRules = new Dictionary<SpeciesArchetype, SpeciesRules>(speciesRules.Count);
            foreach (var entry in speciesRules)
            {
                if (entry.Value == null)
                {
                    throw new ArgumentException("Species rules cannot contain null values.", nameof(speciesRules));
                }

                copiedRules.Add(entry.Key, entry.Value);
            }

            foreach (var entry in copiedRules)
            {
                if (entry.Value.DietTarget.HasValue && !copiedRules.ContainsKey(entry.Value.DietTarget.Value))
                {
                    throw new ArgumentException(
                        $"Species {entry.Key} targets missing diet species {entry.Value.DietTarget.Value}.",
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
            this.startingProbabilities = new ReadOnlyDictionary<SpeciesArchetype, float>(copiedProbabilities);
            this.speciesRules = new ReadOnlyDictionary<SpeciesArchetype, SpeciesRules>(copiedRules);
            RunDurationSeconds = runDurationSeconds;
            StepInterval = stepInterval;
            MaxPopulation = maxPopulation;
            MinPopulation = minPopulation;
        }

        public int Width { get; }
        public int Height { get; }
        public float RunDurationSeconds { get; }
        public float StepInterval { get; }
        public int MaxPopulation { get; }
        public int MinPopulation { get; }
        public IReadOnlyDictionary<SpeciesArchetype, float> StartingProbabilities => startingProbabilities;
        public IReadOnlyDictionary<SpeciesArchetype, SpeciesRules> SpeciesRules => speciesRules;

        public bool TryGetStartingProbability(SpeciesArchetype species, out float probability)
        {
            return startingProbabilities.TryGetValue(species, out probability);
        }

        public CellularSimData WithSpeciesRules(SpeciesArchetype species, SpeciesRules rules)
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
            SpeciesArchetype species,
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

        public CellularSimData WithoutSpecies(SpeciesArchetype species)
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

        public CellularSimData WithStartingProbability(SpeciesArchetype species, float probability)
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
            IReadOnlyDictionary<SpeciesArchetype, float> updatedProbabilities,
            IReadOnlyDictionary<SpeciesArchetype, SpeciesRules> updatedRules)
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

        static Dictionary<SpeciesArchetype, TValue> Copy<TValue>(
            IReadOnlyDictionary<SpeciesArchetype, TValue> source)
        {
            var copy = new Dictionary<SpeciesArchetype, TValue>(source.Count);
            foreach (var entry in source)
            {
                copy.Add(entry.Key, entry.Value);
            }

            return copy;
        }
    }
}
