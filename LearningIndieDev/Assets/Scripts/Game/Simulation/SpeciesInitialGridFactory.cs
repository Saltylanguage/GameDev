using System;

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
            var populationCount = 0;
            for (var y = 0; y < data.Height; y++)
            {
                for (var x = 0; x < data.Width; x++)
                {
                    var roll = random.NextDouble();
                    if (!TryGetInitialSpecies(roll, data, out var species))
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

                var species = GetInitialSpecies(random.NextDouble(), data);
                grid.SetCell(x, y, CreateCell(data, species));
                populationCount++;
            }

            return grid;
        }

        static SpeciesCell CreateCell(CellularSimData data, SpeciesId species)
        {
            var rules = data.SpeciesRules[species];
            return species == SpeciesIds.Plant
                ? SpeciesCell.FromTerrain(
                    data.TerrainDefinitions[TerrainIds.Grass],
                    rules.StartingFoodReserve,
                    SpeciesIds.Plant)
                : new SpeciesCell(
                    species,
                    energy: rules.StartingEnergy,
                    foodReserve: rules.StartingFoodReserve);
        }

        static SpeciesId GetInitialSpecies(double roll, CellularSimData data)
        {
            return TryGetInitialSpecies(roll, data, out var species)
                ? species
                : GetFallbackSpecies(data);
        }

        static bool TryGetInitialSpecies(
            double roll,
            CellularSimData data,
            out SpeciesId species)
        {
            var cumulativeProbability = 0d;
            foreach (var entry in data.StartingProbabilities)
            {
                cumulativeProbability += entry.Value;
                if (roll < cumulativeProbability)
                {
                    species = entry.Key;
                    return true;
                }
            }

            species = default;
            return false;
        }

        static SpeciesId GetFallbackSpecies(CellularSimData data)
        {
            foreach (var species in data.SpeciesRules.Keys)
            {
                return species;
            }

            throw new InvalidOperationException("Cellular simulation data must define at least one species.");
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
                        && ((species == SpeciesIds.Plant && neighbor.IsPlantResource)
                            || (species != SpeciesIds.Plant
                                && neighbor.IsCreature
                                && neighbor.SpeciesId == species)))
                    {
                        count++;
                    }
                }
            }

            return count;
        }
    }
}
