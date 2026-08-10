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
            int seed)
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
            ResolveStarvation(next, rules);
            ResolveWilt(next, rules, random);
            ResolveReproduction(source, next, rules, random);
            return next;
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
                    if (!attacker.IsOccupied
                        || !rules.TryGetValue(attacker.Species, out var attackerRules)
                        || attackerRules.AttackAmount <= 0
                        || !next.GetCell(x, y).IsOccupied)
                    {
                        continue;
                    }

                    foreach (var offset in attackerRules.AttackPattern.Offsets)
                    {
                        var targetX = x + offset.x;
                        var targetY = y + offset.y;
                        if (!source.TryGetCell(targetX, targetY, out var target)
                            || !target.IsOccupied
                            || target.Species == attacker.Species
                            || !attackerRules.DietTarget.HasValue
                            || target.Species != attackerRules.DietTarget.Value
                            || !next.TryGetCell(targetX, targetY, out var currentTarget)
                            || !currentTarget.IsOccupied)
                        {
                            continue;
                        }

                        var damage = attackerRules.AttackAmount;
                        if (rules.TryGetValue(target.Species, out var targetRules)
                            && ContainsOffset(targetRules.BlockPattern, new Vector2Int(-offset.x, -offset.y)))
                        {
                            damage = Math.Max(0, damage - targetRules.BlockAmount);
                        }

                        if (damage > 0)
                        {
                            var remainingHealth = currentTarget.Health - damage;
                            next.SetCell(targetX, targetY, remainingHealth > 0
                                ? new SpeciesCell(currentTarget.Species, remainingHealth, currentTarget.Energy, currentTarget.Age, currentTarget.FoodEaten)
                                : SpeciesCell.Empty);

                            if (remainingHealth <= 0
                                && attackerRules.StartingEnergy > 0
                                && next.TryGetCell(x, y, out var currentAttacker)
                                && currentAttacker.IsOccupied
                                && currentAttacker.Species == attacker.Species)
                            {
                                next.SetCell(x, y, CreateFedCell(currentAttacker, attackerRules));
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
            var moved = new bool[source.Count];
            var claimed = new bool[source.Count];

            for (var y = 0; y < source.Height; y++)
            {
                for (var x = 0; x < source.Width; x++)
                {
                    var sourceIndex = GetIndex(source, x, y);
                    var sourceCell = source.GetCell(x, y);
                    var currentCell = next.GetCell(x, y);
                    if (moved[sourceIndex]
                        || !sourceCell.IsOccupied
                        || !currentCell.IsOccupied
                        || !rules.TryGetValue(sourceCell.Species, out var speciesRules)
                        || speciesRules.MovementSpeed <= 0f
                        || currentCell.Species != sourceCell.Species)
                    {
                        continue;
                    }

                    if (speciesRules.DietTarget.HasValue
                        && TryMove(
                            source,
                            next,
                            x,
                            y,
                            currentCell,
                            speciesRules,
                            speciesRules.DietPattern,
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
                    && sourceTarget.IsOccupied
                    && sourceTarget.Species == speciesRules.DietTarget.Value;
                if (requireDietTarget && !isDietTarget)
                {
                    continue;
                }

                var nextTarget = next.GetCell(targetX, targetY);
                if (nextTarget.IsOccupied && !isDietTarget)
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
            var bestIsDietTarget = speciesRules.DietTarget.HasValue
                && bestTarget.IsOccupied
                && bestTarget.Species == speciesRules.DietTarget.Value;
            var bestIndex = GetIndex(source, bestX, bestY);
            next.SetCell(x, y, SpeciesCell.Empty);
            next.SetCell(bestX, bestY, bestIsDietTarget
                ? CreateFedCell(cell, speciesRules)
                : cell);
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
                    || source.GetCell(targetX, targetY).IsOccupied
                    || next.GetCell(targetX, targetY).IsOccupied)
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

                next.SetCell(x, y, SpeciesCell.Empty);
                next.SetCell(targetX, targetY, cell);
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
                    if (!cell.IsOccupied
                        || !rules.TryGetValue(cell.Species, out var speciesRules)
                        || !speciesRules.DietTarget.HasValue
                        || speciesRules.StartingEnergy <= 0)
                    {
                        continue;
                    }

                    var remainingEnergy = cell.Energy - 1;
                    next.SetCell(x, y, remainingEnergy > 0
                        ? new SpeciesCell(cell.Species, cell.Health, remainingEnergy, cell.Age, cell.FoodEaten)
                        : SpeciesCell.Empty);
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
                    if (!cell.IsOccupied
                        || !rules.TryGetValue(cell.Species, out var speciesRules)
                        || speciesRules.WiltChance <= 0f
                        || random.NextDouble() > speciesRules.WiltChance)
                    {
                        continue;
                    }

                    next.SetCell(x, y, SpeciesCell.Empty);
                }
            }
        }

        static void ResolveReproduction(
            Grid<SpeciesCell> source,
            Grid<SpeciesCell> next,
            IReadOnlyDictionary<SpeciesArchetype, SpeciesRules> rules,
            System.Random random)
        {
            var claimed = new bool[source.Count];

            for (var y = 0; y < source.Height; y++)
            {
                for (var x = 0; x < source.Width; x++)
                {
                    var parent = source.GetCell(x, y);
                    if (!parent.IsOccupied
                        || !rules.TryGetValue(parent.Species, out var speciesRules)
                        || !next.GetCell(x, y).IsOccupied)
                    {
                        continue;
                    }

                    if (parent.FoodEaten < speciesRules.ReproductionFoodRequired)
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
                                && neighbor.IsOccupied
                                && neighbor.Species == parent.Species)
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
                        if (claimed[childIndex] || next.GetCell(childX, childY).IsOccupied)
                        {
                            continue;
                        }

                        if (random.NextDouble() <= speciesRules.ReproductionChance)
                        {
                            next.SetCell(childX, childY, new SpeciesCell(
                                parent.Species,
                                health: 1,
                                energy: speciesRules.StartingEnergy));
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

        static SpeciesCell CreateFedCell(SpeciesCell cell, SpeciesRules rules)
        {
            return new SpeciesCell(
                cell.Species,
                cell.Health,
                rules.StartingEnergy,
                cell.Age,
                cell.FoodEaten + 1);
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
                        && neighbor.IsOccupied
                        && neighbor.Species == species)
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
                    && neighbor.IsOccupied
                    && neighbor.Species == species)
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
