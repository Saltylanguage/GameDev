using System;
using System.Collections.Generic;

namespace SaltyGame
{
    public static class SpeciesInitialGridFactory
    {
        public static Grid<SpeciesCell> Create(CellularSimData data, int runSeed)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var random = new Random(runSeed);
            var grid = new Grid<SpeciesCell>(data.Width, data.Height);
            var startingSpecies = GetSortedSpecies(data.StartingProbabilities);
            var fallbackSpecies = GetSortedSpecies(data.SpeciesRules);
            var populationCount = 0;
            for (var y = 0; y < data.Height; y++)
            {
                for (var x = 0; x < data.Width; x++)
                {
                    var roll = random.NextDouble();
                    if (!TryGetInitialSpecies(roll, data, startingSpecies, out var species))
                    {
                        continue;
                    }

                    if (data.MaxPopulation > 0 && populationCount >= data.MaxPopulation)
                    {
                        continue;
                    }

                    var sameSpeciesNeighbors = CountNearbySpecies(grid, x, y, species);
                    var clumpPenalty = sameSpeciesNeighbors > 2
                        ? 0.9d
                        : sameSpeciesNeighbors > 0 ? 0.65d : 0d;
                    if (random.NextDouble() < clumpPenalty)
                    {
                        continue;
                    }

                    grid.SetCell(x, y, CreateCell(data, species));
                    populationCount++;
                }
            }

            var minimumPopulation = Math.Min(data.MinPopulation, grid.Count);
            if (data.MaxPopulation > 0)
            {
                minimumPopulation = Math.Min(minimumPopulation, data.MaxPopulation);
            }

            var attempts = 0;
            while (populationCount < minimumPopulation && attempts++ < grid.Count * 4)
            {
                var index = random.Next(grid.Count);
                var x = index % data.Width;
                var y = index / data.Width;
                var cell = grid.GetCell(x, y);
                if (cell.IsCreature || cell.IsPlantResource)
                {
                    continue;
                }

                var species = GetInitialSpecies(random.NextDouble(), data, startingSpecies, fallbackSpecies);
                grid.SetCell(x, y, CreateCell(data, species));
                populationCount++;
            }

            return grid;
        }

        static SpeciesCell CreateCell(CellularSimData data, SpeciesId species)
        {
            var rules = data.SpeciesRules[species];
            return (data.SpeciesRules[species].IsPlant || species == SpeciesIds.Plant)
                ? SpeciesCell.FromTerrain(
                    data.TerrainDefinitions[TerrainIds.Grass],
                    rules.StartingFoodReserve,
                    species)
                : new SpeciesCell(
                    species,
                    energy: rules.MaximumEnergy > 0
                        ? Math.Min(rules.MaximumEnergy, rules.StartingEnergy)
                        : rules.StartingEnergy,
                    foodReserve: rules.StartingFoodReserve);
        }

        static SpeciesId GetInitialSpecies(
            double roll,
            CellularSimData data,
            IReadOnlyList<SpeciesId> startingSpecies,
            IReadOnlyList<SpeciesId> fallbackSpecies)
        {
            return TryGetInitialSpecies(roll, data, startingSpecies, out var species)
                ? species
                : GetFallbackSpecies(data, startingSpecies, fallbackSpecies);
        }

        static bool TryGetInitialSpecies(
            double roll,
            CellularSimData data,
            IReadOnlyList<SpeciesId> startingSpecies,
            out SpeciesId species)
        {
            var cumulativeProbability = 0d;
            foreach (var speciesId in startingSpecies)
            {
                cumulativeProbability += data.StartingProbabilities[speciesId];
                if (roll < cumulativeProbability)
                {
                    species = speciesId;
                    return true;
                }
            }

            species = default;
            return false;
        }

        static SpeciesId GetFallbackSpecies(
            CellularSimData data,
            IReadOnlyList<SpeciesId> startingSpecies,
            IReadOnlyList<SpeciesId> fallbackSpecies)
        {
            var fallback = default(SpeciesId);
            var highestProbability = float.MinValue;
            foreach (var species in startingSpecies)
            {
                var probability = data.StartingProbabilities[species];
                if (probability > highestProbability)
                {
                    fallback = species;
                    highestProbability = probability;
                }
            }

            if (fallback.IsValid)
            {
                return fallback;
            }

            if (fallbackSpecies.Count > 0)
            {
                return fallbackSpecies[0];
            }

            throw new InvalidOperationException("Cellular simulation data must define at least one species.");
        }

        static List<SpeciesId> GetSortedSpecies<T>(IReadOnlyDictionary<SpeciesId, T> definitions)
        {
            var species = new List<SpeciesId>(definitions.Keys);
            species.Sort((left, right) => string.CompareOrdinal(left.Value, right.Value));
            return species;
        }

        static int CountNearbySpecies(Grid<SpeciesCell> grid, int x, int y, SpeciesId species)
        {
            var count = 0;
            for (var offsetY = -1; offsetY <= 1; offsetY++)
            {
                for (var offsetX = -1; offsetX <= 1; offsetX++)
                {
                    if (offsetX == 0 && offsetY == 0)
                    {
                        continue;
                    }

                    if (grid.TryGetCell(x + offsetX, y + offsetY, out var neighbor)
                        && ((neighbor.IsPlantResource || neighbor.IsCreature)
                            && neighbor.SpeciesId == species))
                    {
                        count++;
                    }
                }
            }

            return count;
        }
    }
}
