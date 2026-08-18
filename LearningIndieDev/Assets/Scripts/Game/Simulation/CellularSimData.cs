using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SaltyGame
{
    public sealed class CellularSimData
    {
        readonly IReadOnlyDictionary<SpeciesId, float> startingProbabilities;
        readonly IReadOnlyDictionary<SpeciesId, SpeciesRules> speciesRules;
        readonly IReadOnlyDictionary<SpeciesId, int> startingPopulations;
        readonly IReadOnlyDictionary<TerrainId, TerrainDefinition> terrainDefinitions;
        readonly IReadOnlyDictionary<SpeciesId, AlphaOffspringRule> alphaOffspringRules;

        public CellularSimData(
            int width,
            int height,
            IReadOnlyDictionary<SpeciesId, float> startingProbabilities,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> speciesRules,
            float runDurationSeconds,
            float stepInterval,
            int maxPopulation = 0,
            int minPopulation = 0,
            IReadOnlyDictionary<TerrainId, TerrainDefinition> terrainDefinitions = null,
            IReadOnlyDictionary<SpeciesId, AlphaOffspringRule> alphaOffspringRules = null,
            IReadOnlyDictionary<SpeciesId, int> startingPopulations = null)
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

            var copiedStartingPopulations = new Dictionary<SpeciesId, int>();
            if (startingPopulations != null)
            {
                foreach (var entry in startingPopulations)
                {
                    if (!copiedRules.ContainsKey(entry.Key))
                    {
                        throw new ArgumentException(
                            $"Starting population is configured for undefined species {entry.Key}.",
                            nameof(startingPopulations));
                    }

                    if (entry.Value < 0)
                    {
                        throw new ArgumentOutOfRangeException(
                            nameof(startingPopulations),
                            entry.Value,
                            "Starting populations cannot be negative.");
                    }

                    copiedStartingPopulations.Add(entry.Key, entry.Value);
                }
            }

            var totalStartingPopulation = 0L;
            foreach (var entry in copiedStartingPopulations)
            {
                totalStartingPopulation += entry.Value;
            }

            if (totalStartingPopulation > (long)width * height)
            {
                throw new ArgumentException(
                    "Configured starting populations cannot exceed the grid capacity.",
                    nameof(startingPopulations));
            }

            if (maxPopulation > 0 && totalStartingPopulation > maxPopulation)
            {
                throw new ArgumentException(
                    "Configured starting populations cannot exceed the scenario maximum population.",
                    nameof(startingPopulations));
            }

            var configuredTerrainDefinitions = terrainDefinitions ?? TerrainDefaults.Create();
            var copiedTerrainDefinitions = new Dictionary<TerrainId, TerrainDefinition>(
                configuredTerrainDefinitions.Count);
            foreach (var entry in configuredTerrainDefinitions)
            {
                if (!entry.Key.IsValid)
                {
                    throw new ArgumentException("Terrain definitions cannot use an empty terrain id.", nameof(terrainDefinitions));
                }

                if (entry.Value == null)
                {
                    throw new ArgumentException("Terrain definitions cannot contain null values.", nameof(terrainDefinitions));
                }

                if (entry.Key != entry.Value.Id)
                {
                    throw new ArgumentException(
                        $"Terrain definition key {entry.Key} does not match definition id {entry.Value.Id}.",
                        nameof(terrainDefinitions));
                }

                copiedTerrainDefinitions.Add(entry.Key, entry.Value);
            }

            if (!copiedTerrainDefinitions.ContainsKey(TerrainIds.Bare)
                || !copiedTerrainDefinitions.ContainsKey(TerrainIds.Grass))
            {
                throw new ArgumentException(
                    "Terrain definitions must include the bare and grass terrain ids.",
                    nameof(terrainDefinitions));
            }

            var copiedAlphaOffspringRules = new Dictionary<SpeciesId, AlphaOffspringRule>();
            if (alphaOffspringRules != null)
            {
                foreach (var entry in alphaOffspringRules)
                {
                    if (entry.Value == null
                        || entry.Key != entry.Value.Species
                        || !copiedRules.ContainsKey(entry.Key))
                    {
                        throw new ArgumentException(
                            "Alpha offspring rules must target a configured species and match their dictionary key.",
                            nameof(alphaOffspringRules));
                    }

                    copiedAlphaOffspringRules.Add(entry.Key, entry.Value);
                }
            }

            Width = width;
            Height = height;
            this.startingProbabilities = new ReadOnlyDictionary<SpeciesId, float>(copiedProbabilities);
            this.speciesRules = new ReadOnlyDictionary<SpeciesId, SpeciesRules>(copiedRules);
            this.startingPopulations = new ReadOnlyDictionary<SpeciesId, int>(copiedStartingPopulations);
            this.terrainDefinitions = new ReadOnlyDictionary<TerrainId, TerrainDefinition>(copiedTerrainDefinitions);
            this.alphaOffspringRules = new ReadOnlyDictionary<SpeciesId, AlphaOffspringRule>(copiedAlphaOffspringRules);
            RunDurationSeconds = runDurationSeconds;
            StepInterval = stepInterval;
            MaxPopulation = maxPopulation;
            MinPopulation = minPopulation;
            Fingerprint = CellularSimDataFingerprint.Create(this);
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
            int minPopulation = 0,
            IReadOnlyDictionary<TerrainId, TerrainDefinition> terrainDefinitions = null)
            : this(
                width,
                height,
                SpeciesIdConversions.FromLegacy(startingProbabilities),
                speciesRules,
                runDurationSeconds,
                stepInterval,
                maxPopulation,
                minPopulation,
                terrainDefinitions)
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
            int minPopulation = 0,
            IReadOnlyDictionary<TerrainId, TerrainDefinition> terrainDefinitions = null)
            : this(
                width,
                height,
                SpeciesIdConversions.FromLegacy(startingProbabilities),
                SpeciesIdConversions.FromLegacy(speciesRules),
                runDurationSeconds,
                stepInterval,
                maxPopulation,
                minPopulation,
                terrainDefinitions)
        {
        }

        public int Width { get; }
        public int Height { get; }
        public float RunDurationSeconds { get; }
        public float StepInterval { get; }
        public int MaxPopulation { get; }
        public int MinPopulation { get; }
        public string Fingerprint { get; }
        public IReadOnlyDictionary<SpeciesId, float> StartingProbabilities => startingProbabilities;
        public IReadOnlyDictionary<SpeciesId, int> StartingPopulations => startingPopulations;
        public IReadOnlyDictionary<SpeciesId, SpeciesRules> SpeciesRules => speciesRules;
        public IReadOnlyDictionary<TerrainId, TerrainDefinition> TerrainDefinitions => terrainDefinitions;
        public IReadOnlyDictionary<SpeciesId, AlphaOffspringRule> AlphaOffspringRules => alphaOffspringRules;

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
            return CreateUpdated(startingProbabilities, updatedRules, alphaOffspringRules, startingPopulations);
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
            return CreateUpdated(updatedProbabilities, updatedRules, alphaOffspringRules, startingPopulations);
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
            var updatedAlphaRules = Copy(alphaOffspringRules);
            updatedAlphaRules.Remove(species);
            var updatedStartingPopulations = Copy(startingPopulations);
            updatedStartingPopulations.Remove(species);
            return CreateUpdated(updatedProbabilities, updatedRules, updatedAlphaRules, updatedStartingPopulations);
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
            return CreateUpdated(updatedProbabilities, speciesRules, alphaOffspringRules, startingPopulations);
        }

        public CellularSimData WithAlphaOffspringRule(AlphaOffspringRule rule)
        {
            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            if (!speciesRules.ContainsKey(rule.Species))
            {
                throw new InvalidOperationException(
                    $"Cannot add an alpha offspring rule for undefined species {rule.Species}.");
            }

            var updatedAlphaRules = Copy(alphaOffspringRules);
            updatedAlphaRules[rule.Species] = rule;
            return CreateUpdated(startingProbabilities, speciesRules, updatedAlphaRules, startingPopulations);
        }

        public CellularSimData WithoutAlphaOffspringRule(SpeciesId species)
        {
            var updatedAlphaRules = Copy(alphaOffspringRules);
            return updatedAlphaRules.Remove(species)
                ? CreateUpdated(startingProbabilities, speciesRules, updatedAlphaRules, startingPopulations)
                : this;
        }

        public CellularSimData Copy()
        {
            return CreateUpdated(startingProbabilities, speciesRules, alphaOffspringRules, startingPopulations);
        }

        public CellularSimData WithRunWindow(float runDurationSeconds, float stepInterval)
        {
            return new CellularSimData(
                Width,
                Height,
                startingProbabilities,
                speciesRules,
                runDurationSeconds,
                stepInterval,
                MaxPopulation,
                MinPopulation,
                terrainDefinitions,
                alphaOffspringRules,
                startingPopulations);
        }

        public CellularSimData WithGridSize(int width, int height)
        {
            return new CellularSimData(
                width,
                height,
                startingProbabilities,
                speciesRules,
                RunDurationSeconds,
                StepInterval,
                MaxPopulation,
                MinPopulation,
                terrainDefinitions,
                alphaOffspringRules,
                startingPopulations);
        }

        CellularSimData CreateUpdated(
            IReadOnlyDictionary<SpeciesId, float> updatedProbabilities,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> updatedRules,
            IReadOnlyDictionary<SpeciesId, AlphaOffspringRule> updatedAlphaOffspringRules,
            IReadOnlyDictionary<SpeciesId, int> updatedStartingPopulations)
        {
            return new CellularSimData(
                Width,
                Height,
                updatedProbabilities,
                updatedRules,
                RunDurationSeconds,
                StepInterval,
                MaxPopulation,
                MinPopulation,
                terrainDefinitions,
                updatedAlphaOffspringRules,
                updatedStartingPopulations);
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
