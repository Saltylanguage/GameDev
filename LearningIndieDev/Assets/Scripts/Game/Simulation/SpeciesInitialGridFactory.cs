using System;
using System.Collections.Generic;

namespace SaltyGame
{
    public static class SpeciesInitialGridFactory
    {
        static readonly GridPattern NearbyPattern = GridPatternTemplates.CreateMooreRange(1);
        public static Grid<SpeciesCell> Create(CellularSimData data, int runSeed)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var random = new Random(runSeed);
            var grid = new Grid<SpeciesCell>(data.Width, data.Height, data.WrapEdges);
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

                    // Explicit starting populations are placed after the weighted
                    // pass so their counts remain deterministic for authored scenarios.
                    if (data.StartingPopulations.ContainsKey(species))
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

            PlaceExplicitStartingPopulations(data, grid, random, ref populationCount);

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

            StaggerInitialPlantGrowth(data, grid, random);

            return grid;
        }

        static void StaggerInitialPlantGrowth(
            CellularSimData data,
            Grid<SpeciesCell> grid,
            Random random)
        {
            foreach (var terrain in data.TerrainDefinitions.Values)
            {
                if (terrain.GrowthIntervalSeconds <= 0f)
                {
                    continue;
                }

                var intervalTicks = GetGrowthIntervalTicks(
                    terrain.GrowthIntervalSeconds,
                    data.StepInterval);
                for (var y = 0; y < grid.Height; y++)
                {
                    for (var x = 0; x < grid.Width; x++)
                    {
                        var cell = grid.GetCell(x, y);
                        var resourceSpecies = cell.ResourceSpeciesId.IsValid
                            ? cell.ResourceSpeciesId
                            : cell.SpeciesId;
                        if (!cell.IsPlantResource
                            || cell.TerrainId != terrain.Id
                            || !data.SpeciesRules.TryGetValue(resourceSpecies, out var speciesRules)
                            || !speciesRules.IsPlant)
                        {
                            continue;
                        }

                        // Starting plants represent an established patch, so give
                        // them different points in the growth cycle rather than
                        // letting the entire initial patch spread at once.
                        grid.SetCell(
                            x,
                            y,
                            cell.WithTerrainResourceGrowthElapsedTicks(random.Next(intervalTicks)));
                    }
                }
            }
        }

        static int GetGrowthIntervalTicks(float growthIntervalSeconds, float stepIntervalSeconds)
        {
            var tickCount = Math.Ceiling(
                (double)growthIntervalSeconds / stepIntervalSeconds - 0.000001d);
            if (tickCount >= int.MaxValue)
            {
                return int.MaxValue;
            }

            return Math.Max(1, (int)tickCount);
        }

        static void PlaceExplicitStartingPopulations(
            CellularSimData data,
            Grid<SpeciesCell> grid,
            Random random,
            ref int populationCount)
        {
            if (data.StartingPopulations.Count == 0)
            {
                return;
            }

            var indices = CreateShuffledIndices(grid.Count, random);
            foreach (var species in GetSortedSpecies(data.StartingPopulations))
            {
                var requested = data.StartingPopulations[species];
                if (!data.SpeciesRules.TryGetValue(species, out var rules))
                {
                    throw new InvalidOperationException(
                        $"Starting population is configured for undefined species {species}.");
                }

                var placed = 0;
                for (var index = 0; index < indices.Length && placed < requested; index++)
                {
                    if (data.MaxPopulation > 0 && populationCount >= data.MaxPopulation)
                    {
                        break;
                    }

                    var cellIndex = indices[index];
                    var x = cellIndex % grid.Width;
                    var y = cellIndex / grid.Width;
                    var cell = grid.GetCell(x, y);
                    if (cell.IsCreature || (cell.IsOccupied && !cell.IsTerrainResource))
                    {
                        continue;
                    }

                    if (rules.IsPlant || species == SpeciesIds.Plant)
                    {
                        grid.SetCell(
                            x,
                            y,
                            SpeciesCell.FromTerrain(
                                data.TerrainDefinitions[TerrainIds.Grass],
                                rules.StartingFoodReserve,
                                species));
                        placed++;
                        populationCount++;
                        continue;
                    }

                    var energy = rules.MaximumEnergy > 0
                        ? Math.Min(rules.MaximumEnergy, rules.StartingEnergy)
                        : rules.StartingEnergy;
                    grid.SetCell(x, y, cell.WithEntity(
                        species,
                        health: 1,
                        energy,
                        age: 0,
                        foodEaten: 0,
                        foodReserve: 0f));
                    placed++;
                    populationCount++;
                }

                if (placed < requested)
                {
                    throw new InvalidOperationException(
                        $"Could not place the configured starting population for {species}. "
                        + $"Requested {requested}, placed {placed}.");
                }
            }
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
            foreach (var offset in grid.GetPattern(NearbyPattern).Offsets)
            {
                if (grid.TryGetCell(x + offset.x, y + offset.y, out var neighbor)
                    && ((neighbor.IsPlantResource || neighbor.IsCreature)
                        && neighbor.SpeciesId == species))
                {
                    count++;
                }
            }

            return count;
        }

        static int[] CreateShuffledIndices(int count, Random random)
        {
            var indices = new int[count];
            for (var index = 0; index < count; index++)
            {
                indices[index] = index;
            }

            for (var index = count - 1; index > 0; index--)
            {
                var swapIndex = random.Next(index + 1);
                var value = indices[index];
                indices[index] = indices[swapIndex];
                indices[swapIndex] = value;
            }

            return indices;
        }
    }
}
