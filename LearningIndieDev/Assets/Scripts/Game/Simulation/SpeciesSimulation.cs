using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
    public static class SpeciesSimulation
    {
        public static Grid<SpeciesCell> Step(
            Grid<SpeciesCell> source,
            IReadOnlyDictionary<SpeciesArchetype, SpeciesRules> rules,
            int seed,
            int maxPopulation = 0)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            var next = source.Copy();
            var random = new System.Random(seed);
            ResolveAttacks(source, next, rules);
            ResolveMovement(source, next, rules);
            ResolveMetabolism(next, rules);
            ResolveStarvation(next, rules);
            ResolveCrowdingStress(next, rules);
            ResolveSeedDrops(next, rules, random);
            ResolveWilt(next, rules, random);
            ResolveReproduction(next, rules, random);
            ResolvePopulationLimit(next, maxPopulation, random);
            return next;
        }

        static void ResolvePopulationLimit(Grid<SpeciesCell> next, int maxPopulation, System.Random random)
        {
            if (maxPopulation <= 0)
            {
                return;
            }

            var occupied = new List<int>();
            for (var y = 0; y < next.Height; y++)
            {
                for (var x = 0; x < next.Width; x++)
                {
                    if (next.GetCell(x, y).IsCreature || next.GetCell(x, y).IsPlantResource)
                    {
                        occupied.Add(GetIndex(next, x, y));
                    }
                }
            }

            while (occupied.Count > maxPopulation)
            {
                var removeIndex = random.Next(occupied.Count);
                var cellIndex = occupied[removeIndex];
                occupied.RemoveAt(removeIndex);
                var x = cellIndex % next.Width;
                var y = cellIndex / next.Width;
                var cell = next.GetCell(x, y);
                next.SetCell(x, y, cell.IsPlantResource
                    ? cell.WithoutPlantResource()
                    : cell.WithoutEntity());
            }
        }

        static void ResolveAttacks(
            Grid<SpeciesCell> source,
            Grid<SpeciesCell> next,
            IReadOnlyDictionary<SpeciesArchetype, SpeciesRules> rules)
        {
            for (var y = 0; y < source.Height; y++)
            {
                for (var x = 0; x < source.Width; x++)
                {
                    var attacker = source.GetCell(x, y);
                    if (!attacker.IsCreature
                        || !rules.TryGetValue(attacker.Species, out var attackerRules)
                        || attackerRules.AttackAmount <= 0
                        || !next.GetCell(x, y).IsCreature)
                    {
                        continue;
                    }

                    foreach (var offset in attackerRules.AttackPattern.Offsets)
                    {
                        var targetX = x + offset.x;
                        var targetY = y + offset.y;
                        if (!source.TryGetCell(targetX, targetY, out var target)
                            || !attackerRules.DietTarget.HasValue
                            || !IsDietTarget(target, attackerRules.DietTarget.Value))
                        {
                            continue;
                        }

                        var currentTarget = target;
                        if (target.IsCreature
                            && (!next.TryGetCell(targetX, targetY, out currentTarget)
                                || !currentTarget.IsCreature))
                        {
                            continue;
                        }

                        var damage = attackerRules.AttackAmount;
                        if (rules.TryGetValue(target.Species, out var targetRules)
                            && ContainsOffset(targetRules.BlockPattern, new Vector2Int(-offset.x, -offset.y)))
                        {
                            damage = Math.Max(0, damage - targetRules.BlockAmount);
                        }

                        var currentAttacker = next.GetCell(x, y);
                        if (!currentAttacker.IsCreature || currentAttacker.Species != attacker.Species)
                        {
                            break;
                        }

                        if (target.IsPlantResource)
                        {
                            TryFeedOnPlant(
                                next,
                                targetX,
                                targetY,
                                x,
                                y,
                                currentAttacker,
                                attackerRules,
                                rules.TryGetValue(SpeciesArchetype.Plant, out var plantRules)
                                    ? plantRules.EnergyValue
                                    : 1);
                            break;
                        }

                        if (damage > 0 && currentTarget.IsCreature)
                        {
                            var remainingHealth = currentTarget.Health - damage;
                            next.SetCell(targetX, targetY, remainingHealth > 0
                                ? currentTarget.WithEntity(currentTarget.Species, remainingHealth, currentTarget.Energy, currentTarget.Age, currentTarget.FoodEaten, currentTarget.FoodReserve)
                                : currentTarget.WithoutEntity());

                            if (remainingHealth <= 0
                                && attackerRules.StartingEnergy > 0
                                && currentAttacker.IsCreature
                                && currentAttacker.Species == attacker.Species)
                            {
                                next.SetCell(x, y, CreateFedCell(
                                    currentAttacker,
                                    attackerRules,
                                    rules.TryGetValue(target.Species, out var foodRules)
                                        ? foodRules.EnergyValue
                                        : 0));
                            }
                        }

                        break;
                    }
                }
            }
        }

        static void ResolveMovement(
            Grid<SpeciesCell> source,
            Grid<SpeciesCell> next,
            IReadOnlyDictionary<SpeciesArchetype, SpeciesRules> rules)
        {
            var movementPasses = 1;
            foreach (var speciesRules in rules.Values)
            {
                movementPasses = Math.Max(movementPasses, (int)Math.Ceiling(speciesRules.MovementSpeed));
            }

            for (var pass = 0; pass < movementPasses; pass++)
            {
                var movementSource = pass == 0 ? source : next.Copy();
                ResolveMovementPass(movementSource, next, rules, pass);
            }
        }

        static void ResolveMovementPass(
            Grid<SpeciesCell> source,
            Grid<SpeciesCell> next,
            IReadOnlyDictionary<SpeciesArchetype, SpeciesRules> rules,
            int movementPass)
        {
            var moved = new bool[source.Count];
            var claimed = new bool[source.Count];
            var plantEnergyValue = rules.TryGetValue(SpeciesArchetype.Plant, out var plantRules)
                ? plantRules.EnergyValue
                : 1;

            for (var y = 0; y < source.Height; y++)
            {
                for (var x = 0; x < source.Width; x++)
                {
                    var sourceIndex = GetIndex(source, x, y);
                    var sourceCell = source.GetCell(x, y);
                    var currentCell = next.GetCell(x, y);
                    if (moved[sourceIndex]
                        || !sourceCell.IsCreature
                        || !currentCell.IsCreature
                        || !rules.TryGetValue(sourceCell.Species, out var speciesRules)
                        || speciesRules.MovementSpeed <= movementPass
                        || currentCell.Species != sourceCell.Species)
                    {
                        continue;
                    }

                    if (speciesRules.DietTarget.HasValue
                        && currentCell.FoodReserve <= sourceCell.FoodReserve
                        && TryMove(
                            source,
                            next,
                            x,
                            y,
                            currentCell,
                            speciesRules,
                            speciesRules.DietPattern,
                            plantEnergyValue,
                            requireDietTarget: true,
                            moved,
                            claimed))
                    {
                        continue;
                    }

                    if (speciesRules.ReproductionNeighborCount > 0
                        && CountPatternSpeciesNeighbors(
                            source,
                            x,
                            y,
                            sourceCell.Species,
                            speciesRules.ReproductionPattern,
                            excludeX: -1,
                            excludeY: -1) < speciesRules.ReproductionNeighborCount
                        && TryMoveTowardMate(
                            source,
                            next,
                            x,
                            y,
                            currentCell,
                            speciesRules,
                            moved,
                            claimed))
                    {
                        continue;
                    }

                    TryMove(
                        source,
                        next,
                        x,
                        y,
                        currentCell,
                        speciesRules,
                        speciesRules.MovementPattern,
                        plantEnergyValue,
                        requireDietTarget: false,
                        moved,
                        claimed);
                }
            }
        }

        static bool TryMove(
            Grid<SpeciesCell> source,
            Grid<SpeciesCell> next,
            int x,
            int y,
            SpeciesCell cell,
            SpeciesRules speciesRules,
            GridPattern pattern,
            int plantEnergyValue,
            bool requireDietTarget,
            bool[] moved,
            bool[] claimed)
        {
            var bestX = -1;
            var bestY = -1;
            var bestCrowding = int.MaxValue;

            foreach (var offset in pattern.Offsets)
            {
                var targetX = x + offset.x;
                var targetY = y + offset.y;
                if (!source.IsInBounds(targetX, targetY))
                {
                    continue;
                }

                var targetIndex = GetIndex(source, targetX, targetY);
                if (claimed[targetIndex])
                {
                    continue;
                }

                var sourceTarget = source.GetCell(targetX, targetY);
                var isDietTarget = speciesRules.DietTarget.HasValue
                    && IsDietTarget(sourceTarget, speciesRules.DietTarget.Value);
                if (requireDietTarget && !isDietTarget)
                {
                    continue;
                }

                var nextTarget = next.GetCell(targetX, targetY);
                if (nextTarget.IsCreature && !isDietTarget)
                {
                    continue;
                }

                if (isDietTarget && sourceTarget.IsCreature)
                {
                    continue;
                }

                if (requireDietTarget)
                {
                    bestX = targetX;
                    bestY = targetY;
                    break;
                }

                var crowding = CountNearbySpecies(source, targetX, targetY, cell.Species, x, y);
                if (speciesRules.MaxReproductionGroupSize > 0
                    && crowding + 1 > speciesRules.MaxReproductionGroupSize)
                {
                    continue;
                }

                if (crowding < bestCrowding)
                {
                    bestX = targetX;
                    bestY = targetY;
                    bestCrowding = crowding;
                }
            }

            if (bestX < 0)
            {
                return false;
            }

            var bestTarget = source.GetCell(bestX, bestY);
            var currentTarget = next.GetCell(bestX, bestY);
            var bestIsDietTarget = speciesRules.DietTarget.HasValue
                && IsDietTarget(bestTarget, speciesRules.DietTarget.Value)
                && !bestTarget.IsCreature;
            var bestIndex = GetIndex(source, bestX, bestY);

            if (bestIsDietTarget && bestTarget.Species == SpeciesArchetype.Plant)
            {
                if (!TryFeedOnPlant(next, bestX, bestY, x, y, cell, speciesRules, plantEnergyValue))
                {
                    return false;
                }

                moved[GetIndex(source, x, y)] = true;
                return true;
            }

            next.SetCell(x, y, source.GetCell(x, y).WithoutEntity());
            next.SetCell(bestX, bestY, currentTarget.WithEntity(
                cell.Species,
                cell.Health,
                cell.Energy,
                cell.Age,
                cell.FoodEaten,
                cell.FoodReserve));
            moved[GetIndex(source, x, y)] = true;
            moved[bestIndex] = true;
            claimed[bestIndex] = true;
            return true;
        }

        static bool TryMoveTowardMate(
            Grid<SpeciesCell> source,
            Grid<SpeciesCell> next,
            int x,
            int y,
            SpeciesCell cell,
            SpeciesRules speciesRules,
            bool[] moved,
            bool[] claimed)
        {
            foreach (var offset in speciesRules.MovementPattern.Offsets)
            {
                var targetX = x + offset.x;
                var targetY = y + offset.y;
                if (!source.IsInBounds(targetX, targetY))
                {
                    continue;
                }

                var targetIndex = GetIndex(source, targetX, targetY);
                if (claimed[targetIndex]
                    || source.GetCell(targetX, targetY).IsCreature
                    || next.GetCell(targetX, targetY).IsCreature)
                {
                    continue;
                }

                var sameSpeciesNeighbors = CountPatternSpeciesNeighbors(
                    source,
                    targetX,
                    targetY,
                    cell.Species,
                    speciesRules.ReproductionPattern,
                    excludeX: x,
                    excludeY: y);
                if (sameSpeciesNeighbors < speciesRules.ReproductionNeighborCount
                    || (speciesRules.MaxReproductionGroupSize > 0
                        && sameSpeciesNeighbors + 1 > speciesRules.MaxReproductionGroupSize))
                {
                    continue;
                }

                next.SetCell(x, y, source.GetCell(x, y).WithoutEntity());
                next.SetCell(targetX, targetY, next.GetCell(targetX, targetY).WithEntity(
                    cell.Species,
                    cell.Health,
                    cell.Energy,
                    cell.Age,
                    cell.FoodEaten,
                    cell.FoodReserve));
                moved[GetIndex(source, x, y)] = true;
                moved[targetIndex] = true;
                claimed[targetIndex] = true;
                return true;
            }

            return false;
        }

        static void ResolveStarvation(
            Grid<SpeciesCell> next,
            IReadOnlyDictionary<SpeciesArchetype, SpeciesRules> rules)
        {
            for (var y = 0; y < next.Height; y++)
            {
                for (var x = 0; x < next.Width; x++)
                {
                    var cell = next.GetCell(x, y);
                    if (!cell.IsCreature
                        || !rules.TryGetValue(cell.Species, out var speciesRules)
                        || speciesRules.Metabolism <= 0)
                    {
                        continue;
                    }

                    var remainingEnergy = cell.Energy - speciesRules.Metabolism;
                    next.SetCell(x, y, remainingEnergy > 0
                        ? cell.WithEntity(cell.Species, cell.Health, remainingEnergy, cell.Age, cell.FoodEaten, cell.FoodReserve)
                        : cell.WithoutEntity());
                }
            }
        }

        static void ResolveMetabolism(
            Grid<SpeciesCell> next,
            IReadOnlyDictionary<SpeciesArchetype, SpeciesRules> rules)
        {
            for (var y = 0; y < next.Height; y++)
            {
                for (var x = 0; x < next.Width; x++)
                {
                    var cell = next.GetCell(x, y);
                    if (!cell.IsPlantResource
                        || !rules.TryGetValue(SpeciesArchetype.Plant, out var plantRules)
                        || plantRules.Metabolism >= 0)
                    {
                        continue;
                    }

                    var grownEnergy = cell.IsGrass
                        ? cell.TerrainEnergy - plantRules.Metabolism
                        : cell.FoodReserve - plantRules.Metabolism;
                    next.SetCell(x, y, cell.IsGrass
                        ? cell.WithTerrainEnergy(grownEnergy)
                        : new SpeciesCell(
                            cell.Species,
                            cell.Health,
                            cell.Energy,
                            cell.Age,
                            cell.FoodEaten,
                            grownEnergy));
                }
            }
        }

        static void ResolveCrowdingStress(
            Grid<SpeciesCell> next,
            IReadOnlyDictionary<SpeciesArchetype, SpeciesRules> rules)
        {
            for (var y = 0; y < next.Height; y++)
            {
                for (var x = 0; x < next.Width; x++)
                {
                    var cell = next.GetCell(x, y);
                    if (!cell.IsCreature
                        || !rules.TryGetValue(cell.Species, out var speciesRules)
                        || speciesRules.MaxReproductionGroupSize <= 0
                        || speciesRules.CrowdingEnergyPenalty <= 0)
                    {
                        continue;
                    }

                    var groupSize = CountPatternSpeciesNeighbors(
                        next,
                        x,
                        y,
                        cell.Species,
                        speciesRules.ReproductionPattern,
                        excludeX: -1,
                        excludeY: -1) + 1;
                    var excessMembers = groupSize - speciesRules.MaxReproductionGroupSize;
                    if (excessMembers <= 0)
                    {
                        continue;
                    }

                    var remainingEnergy = cell.Energy - excessMembers * speciesRules.CrowdingEnergyPenalty;
                    next.SetCell(x, y, remainingEnergy > 0
                        ? cell.WithEntity(cell.Species, cell.Health, remainingEnergy, cell.Age, cell.FoodEaten, cell.FoodReserve)
                        : cell.WithoutEntity());
                }
            }
        }

        static void ResolveWilt(
            Grid<SpeciesCell> next,
            IReadOnlyDictionary<SpeciesArchetype, SpeciesRules> rules,
            System.Random random)
        {
            for (var y = 0; y < next.Height; y++)
            {
                for (var x = 0; x < next.Width; x++)
                {
                    var cell = next.GetCell(x, y);
                    if (!cell.IsPlantResource
                        || !rules.TryGetValue(cell.Species, out var speciesRules)
                        || speciesRules.WiltChance <= 0f
                        || random.NextDouble() > speciesRules.WiltChance)
                    {
                        continue;
                    }

                    next.SetCell(x, y, cell.WithoutPlantResource());
                }
            }
        }

        static void ResolveSeedDrops(
            Grid<SpeciesCell> next,
            IReadOnlyDictionary<SpeciesArchetype, SpeciesRules> rules,
            System.Random random)
        {
            if (!rules.TryGetValue(SpeciesArchetype.Plant, out var plantRules)
                || plantRules.StartingFoodReserve <= 0f)
            {
                return;
            }

            for (var y = 0; y < next.Height; y++)
            {
                for (var x = 0; x < next.Width; x++)
                {
                    var cell = next.GetCell(x, y);
                    if (!cell.IsCreature
                        || !rules.TryGetValue(cell.Species, out var speciesRules)
                        || speciesRules.SeedDropChance <= 0f
                        || cell.FoodReserve <= 0f
                        || random.NextDouble() > speciesRules.SeedDropChance
                        || speciesRules.MovementPattern.Count == 0)
                    {
                        continue;
                    }

                    var startOffset = random.Next(speciesRules.MovementPattern.Count);
                    for (var offsetIndex = 0; offsetIndex < speciesRules.MovementPattern.Count; offsetIndex++)
                    {
                        var offset = speciesRules.MovementPattern.Offsets[
                            (startOffset + offsetIndex) % speciesRules.MovementPattern.Count];
                        var seedX = x + offset.x;
                        var seedY = y + offset.y;
                        if (!next.IsInBounds(seedX, seedY)
                            || next.GetCell(seedX, seedY).IsCreature
                            || next.GetCell(seedX, seedY).IsPlantResource)
                        {
                            continue;
                        }

                        next.SetCell(seedX, seedY, SpeciesCell.Grass(plantRules.StartingFoodReserve));
                        break;
                    }
                }
            }
        }

        static void ResolveReproduction(
            Grid<SpeciesCell> next,
            IReadOnlyDictionary<SpeciesArchetype, SpeciesRules> rules,
            System.Random random)
        {
            var source = next.Copy();
            var claimed = new bool[source.Count];

            for (var y = 0; y < source.Height; y++)
            {
                for (var x = 0; x < source.Width; x++)
                {
                    var parent = source.GetCell(x, y);
                    if ((!parent.IsCreature && !parent.IsPlantResource)
                        || !rules.TryGetValue(parent.Species, out var speciesRules)
                        || (!next.GetCell(x, y).IsCreature && !next.GetCell(x, y).IsPlantResource))
                    {
                        continue;
                    }

                    var currentParent = next.GetCell(x, y);
                    if (currentParent.Species != parent.Species
                        || GetReproductionEnergy(currentParent) <= speciesRules.ReproductionFoodRequired
                        || speciesRules.ReproductionChance <= 0f)
                    {
                        continue;
                    }

                    var sameSpeciesNeighbors = 0;
                    if (speciesRules.ReproductionNeighborCount > 0
                        || speciesRules.MaxReproductionGroupSize > 0)
                    {
                        foreach (var offset in speciesRules.ReproductionPattern.Offsets)
                        {
                            if (source.TryGetCell(x + offset.x, y + offset.y, out var neighbor)
                                && IsSameSpecies(neighbor, parent.Species))
                            {
                                sameSpeciesNeighbors++;
                            }
                        }
                    }

                    if (sameSpeciesNeighbors < speciesRules.ReproductionNeighborCount
                        || (speciesRules.MaxReproductionGroupSize > 0
                            && sameSpeciesNeighbors + 1 >= speciesRules.MaxReproductionGroupSize))
                    {
                        continue;
                    }

                    foreach (var offset in speciesRules.ReproductionPattern.Offsets)
                    {
                        var childX = x + offset.x;
                        var childY = y + offset.y;
                        if (!source.IsInBounds(childX, childY))
                        {
                            continue;
                        }

                        var childIndex = GetIndex(source, childX, childY);
                        if (claimed[childIndex]
                            || next.GetCell(childX, childY).IsCreature
                            || next.GetCell(childX, childY).IsPlantResource)
                        {
                            continue;
                        }

                        if (random.NextDouble() <= speciesRules.ReproductionChance)
                        {
                            next.SetCell(childX, childY, parent.Species == SpeciesArchetype.Plant
                                ? SpeciesCell.Grass(speciesRules.StartingFoodReserve)
                                : new SpeciesCell(
                                    parent.Species,
                                    health: 1,
                                    energy: speciesRules.StartingEnergy));
                            next.SetCell(x, y, ConsumeReproductionEnergy(
                                currentParent,
                                speciesRules.ReproductionFoodRequired));
                            claimed[childIndex] = true;
                        }

                        break;
                    }
                }
            }
        }

        static bool ContainsOffset(GridPattern pattern, Vector2Int offset)
        {
            foreach (var candidate in pattern.Offsets)
            {
                if (candidate == offset)
                {
                    return true;
                }
            }

            return false;
        }

        static SpeciesCell CreateFedCell(
            SpeciesCell cell,
            SpeciesRules rules,
            int energyValue,
            float foodAmount = 1f)
        {
            return cell.WithEntity(
                cell.Species,
                cell.Health,
                cell.Energy + energyValue,
                cell.Age,
                cell.FoodEaten + 1,
                cell.FoodReserve + foodAmount);
        }

        static bool TryFeedOnPlant(
            Grid<SpeciesCell> next,
            int plantX,
            int plantY,
            int eaterX,
            int eaterY,
            SpeciesCell eater,
            SpeciesRules eaterRules,
            int energyValue)
        {
            var plant = next.GetCell(plantX, plantY);
            if (!plant.IsPlantResource)
            {
                return false;
            }

            var availableEnergy = plant.IsGrass ? plant.TerrainEnergy : plant.FoodReserve;
            if (availableEnergy <= 0f)
            {
                return false;
            }

            var consumedEnergy = Math.Min(1f, availableEnergy);
            next.SetCell(eaterX, eaterY, CreateFedCell(eater, eaterRules, energyValue, consumedEnergy));
            var remainingEnergy = availableEnergy - consumedEnergy;
            next.SetCell(plantX, plantY, plant.IsGrass
                ? remainingEnergy > 0f
                    ? plant.WithTerrainEnergy(remainingEnergy)
                    : plant.WithoutPlantResource()
                : remainingEnergy > 0f
                    ? new SpeciesCell(
                        plant.Species,
                        plant.Health,
                        plant.Energy,
                        plant.Age,
                        plant.FoodEaten,
                        remainingEnergy)
                    : plant.WithoutEntity());
            return true;
        }

        static bool IsDietTarget(SpeciesCell cell, SpeciesArchetype target)
        {
            return target == SpeciesArchetype.Plant
                ? cell.IsPlantResource && !cell.IsCreature
                : cell.IsCreature && cell.Species == target;
        }

        static bool IsSameSpecies(SpeciesCell cell, SpeciesArchetype species)
        {
            return species == SpeciesArchetype.Plant
                ? cell.IsPlantResource
                : cell.IsCreature && cell.Species == species;
        }

        static int GetReproductionEnergy(SpeciesCell cell)
        {
            return cell.IsCreature
                ? cell.Energy
                : (int)(cell.IsGrass ? cell.TerrainEnergy : cell.FoodReserve);
        }

        static SpeciesCell ConsumeReproductionEnergy(SpeciesCell cell, int amount)
        {
            if (cell.IsCreature)
            {
                return cell.WithEntity(
                    cell.Species,
                    cell.Health,
                    cell.Energy - amount,
                    cell.Age,
                    cell.FoodEaten,
                    cell.FoodReserve);
            }

            var remaining = Math.Max(0f, (cell.IsGrass ? cell.TerrainEnergy : cell.FoodReserve) - amount);
            return cell.IsGrass
                ? cell.WithTerrainEnergy(remaining)
                : new SpeciesCell(
                    cell.Species,
                    cell.Health,
                    cell.Energy,
                    cell.Age,
                    cell.FoodEaten,
                    remaining);
        }

        static int CountNearbySpecies(
            Grid<SpeciesCell> grid,
            int x,
            int y,
            SpeciesArchetype species,
            int excludeX,
            int excludeY)
        {
            var count = 0;
            for (var offsetY = -1; offsetY <= 1; offsetY++)
            {
                for (var offsetX = -1; offsetX <= 1; offsetX++)
                {
                    var neighborX = x + offsetX;
                    var neighborY = y + offsetY;
                    if ((neighborX == x && neighborY == y)
                        || (neighborX == excludeX && neighborY == excludeY))
                    {
                        continue;
                    }

                    if (grid.TryGetCell(neighborX, neighborY, out var neighbor)
                        && IsSameSpecies(neighbor, species))
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        static int CountPatternSpeciesNeighbors(
            Grid<SpeciesCell> grid,
            int x,
            int y,
            SpeciesArchetype species,
            GridPattern pattern,
            int excludeX,
            int excludeY)
        {
            var count = 0;
            foreach (var offset in pattern.Offsets)
            {
                var neighborX = x + offset.x;
                var neighborY = y + offset.y;
                if (neighborX == excludeX && neighborY == excludeY)
                {
                    continue;
                }

                if (grid.TryGetCell(neighborX, neighborY, out var neighbor)
                    && IsSameSpecies(neighbor, species))
                {
                    count++;
                }
            }

            return count;
        }

        static int GetIndex<T>(Grid<T> grid, int x, int y)
        {
            return x + y * grid.Width;
        }
    }
}
