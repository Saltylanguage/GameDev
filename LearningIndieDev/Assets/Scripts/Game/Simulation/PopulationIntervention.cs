using System;
using System.Collections.Generic;

namespace SaltyGame
{
    public readonly struct PopulationIntervention
    {
        public PopulationIntervention(SpeciesId species, int amount, string source, int tick, int phase, int seed)
        {
            Species = species;
            Amount = amount;
            Source = source;
            Tick = tick;
            Phase = phase;
            Seed = seed;
        }

        public SpeciesId Species { get; }
        public int Amount { get; }
        public string Source { get; }
        public int Tick { get; }
        public int Phase { get; }
        public int Seed { get; }

        // Preview and commit share this planner. No random state or entity IDs are
        // consumed by validation; a rejected action never changes the source grid.
        internal static bool TryPlan(SimulationRunState run, SpeciesId species, SpeciesRules rules,
            int requested, int maximumPopulation, TerrainDefinition plantTerrain, out int amount, out List<(int x, int y)> candidates,
            out string reason)
        {
            amount = requested;
            candidates = new List<(int x, int y)>();
            reason = string.Empty;
            if (run.Status != SimulationRunStatus.AwaitingDecision)
                reason = "Available only at a round boundary.";
            else if (requested == 0 || requested == int.MinValue)
                reason = "Choose a non-zero population change.";
            else if (requested > 0 && species == run.PlayerSpeciesId)
            {
                amount = Math.Min(requested, run.PopulationPoolSpace);
                if (amount == 0) reason = "Population pool full.";
            }
            if (reason.Length > 0) return false;
            if (amount > 0 && rules.Role == SpeciesRole.Plant
                && (rules.StartingFoodReserve <= 0f || !plantTerrain.ProvidesResource))
            {
                reason = "Plant configuration has no usable food resource.";
                return false;
            }

            var snapshot = SpeciesPopulationSnapshot.Create(run.Cells, run.Tick);
            if (amount < 0 && species == run.PlayerSpeciesId && snapshot.GetCount(species) + amount < 1)
            {
                reason = "Keep at least one of your species.";
                return false;
            }
            var total = 0;
            foreach (var count in snapshot.Counts.Values) total += count;
            if (amount > 0 && maximumPopulation > 0 && total + amount > maximumPopulation)
            {
                reason = "World population limit reached.";
                return false;
            }
            for (var y = 0; y < run.Cells.Height; y++)
            for (var x = 0; x < run.Cells.Width; x++)
            {
                var cell = run.Cells.GetCell(x, y);
                var plant = rules.Role == SpeciesRole.Plant;
                var matches = plant
                    ? cell.IsPlantResource && (cell.ResourceSpeciesId.IsValid ? cell.ResourceSpeciesId : cell.SpeciesId) == species
                    : cell.IsCreature && cell.SpeciesId == species;
                // Planting uses bare, empty ground. It never overwrites another
                // terrain type, an existing resource, a creature, or its state.
                var free = plant
                    ? !cell.IsOccupied && !cell.IsPlantResource && cell.TerrainId == TerrainIds.Bare
                    : !cell.IsOccupied && cell.IsPassable;
                if (amount < 0 ? matches : free) candidates.Add((x, y));
            }
            if (candidates.Count < Math.Abs(amount))
            {
                reason = amount > 0 ? "Not enough suitable space." : "Not enough population to remove.";
                return false;
            }
            return true;
        }

        internal static Grid<SpeciesCell> Apply(Grid<SpeciesCell> source, SpeciesId species,
            SpeciesRules rules, int amount, List<(int x, int y)> candidates, int seed, TerrainDefinition plantTerrain)
        {
            var next = source.Copy();
            var random = new Random(seed);
            for (var i = 0; i < Math.Abs(amount); i++)
            {
                var selected = random.Next(i, candidates.Count);
                (candidates[i], candidates[selected]) = (candidates[selected], candidates[i]);
                var position = candidates[i];
                var cell = next.GetCell(position.x, position.y);
                next.SetCell(position.x, position.y, amount < 0
                    ? rules.Role == SpeciesRole.Plant ? cell.WithoutPlantResource() : cell.WithoutEntity()
                    : rules.Role == SpeciesRole.Plant
                        ? SpeciesCell.FromTerrain(plantTerrain, rules.StartingFoodReserve, species)
                            .WithScentMarker(cell.ScentMarker)
                        : cell.WithEntity(species, 1, rules.MaximumEnergy > 0 ? Math.Min(rules.StartingEnergy, rules.MaximumEnergy) : rules.StartingEnergy, 0, 0, 0f));
            }
            return next;
        }
    }

    public readonly struct FoodPressureSnapshot
    {
        public FoodPressureSnapshot(SimulationRunState run, SpeciesId foodSpecies)
        {
            Plants = SpeciesPopulationSnapshot.Create(run.Cells, run.Tick).GetCount(foodSpecies);
            var opening = run.PopulationHistory[0];
            foreach (var sample in run.PopulationHistory)
                if (sample.Tick <= run.PhaseStartTick) opening = sample;
            Change = Plants - opening.GetCount(foodSpecies);
            StarvationDeaths = run.Metrics.GetActivity(run.PlayerSpeciesId).StarvationDeaths;
        }
        public int Plants { get; }
        public int Change { get; }
        public int StarvationDeaths { get; }
        public string Status => Plants == 0 ? "FOOD EXHAUSTED" : Change < 0 ? "FOOD DECLINING" : "FOOD AVAILABLE";
    }
}
