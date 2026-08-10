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
            ResolveAttacks(source, next, rules);
            ResolveMovement(source, next, rules);
            ResolveReproduction(source, next, rules, seed);
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
                        || attackerRules.AttackAmount <= 0)
                    {
                        continue;
                    }

                    foreach (var offset in attackerRules.AttackPattern.Offsets)
                    {
                        var targetX = x + offset.x;
                        var targetY = y + offset.y;
                        if (!source.TryGetCell(targetX, targetY, out var target)
                            || !target.IsOccupied
                            || target.Species == attacker.Species)
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
                            var remainingHealth = target.Health - damage;
                            next.SetCell(targetX, targetY, remainingHealth > 0
                                ? new SpeciesCell(target.Species, remainingHealth, target.Energy, target.Age)
                                : SpeciesCell.Empty);
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
                    var cell = source.GetCell(x, y);
                    if (moved[sourceIndex]
                        || !cell.IsOccupied
                        || !rules.TryGetValue(cell.Species, out var speciesRules)
                        || speciesRules.MovementSpeed <= 0f
                        || !next.GetCell(x, y).IsOccupied)
                    {
                        continue;
                    }

                    if (speciesRules.DietTarget.HasValue
                        && TryMove(
                            source,
                            next,
                            x,
                            y,
                            cell,
                            speciesRules,
                            speciesRules.DietPattern,
                            requireDietTarget: true,
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
                        cell,
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

                next.SetCell(x, y, SpeciesCell.Empty);
                next.SetCell(targetX, targetY, cell);
                moved[GetIndex(source, x, y)] = true;
                moved[targetIndex] = true;
                claimed[targetIndex] = true;
                return true;
            }

            return false;
        }

        static void ResolveReproduction(
            Grid<SpeciesCell> source,
            Grid<SpeciesCell> next,
            IReadOnlyDictionary<SpeciesArchetype, SpeciesRules> rules,
            int seed)
        {
            var random = new System.Random(seed);
            var claimed = new bool[source.Count];

            for (var y = 0; y < source.Height; y++)
            {
                for (var x = 0; x < source.Width; x++)
                {
                    var parent = source.GetCell(x, y);
                    if (!parent.IsOccupied
                        || !rules.TryGetValue(parent.Species, out var speciesRules)
                        || speciesRules.ReproductionNeighborCount <= 0
                        || !next.GetCell(x, y).IsOccupied)
                    {
                        continue;
                    }

                    var sameSpeciesNeighbors = 0;
                    foreach (var offset in speciesRules.ReproductionPattern.Offsets)
                    {
                        if (source.TryGetCell(x + offset.x, y + offset.y, out var neighbor)
                            && neighbor.IsOccupied
                            && neighbor.Species == parent.Species)
                        {
                            sameSpeciesNeighbors++;
                        }
                    }

                    if (sameSpeciesNeighbors < speciesRules.ReproductionNeighborCount)
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

                        if (random.NextDouble() <= 0.5d)
                        {
                            next.SetCell(childX, childY, new SpeciesCell(parent.Species));
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

        static int GetIndex<T>(Grid<T> grid, int x, int y)
        {
            return x + y * grid.Width;
        }
    }
}
