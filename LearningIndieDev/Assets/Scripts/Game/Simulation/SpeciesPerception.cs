using System;
using UnityEngine;

namespace SaltyGame
{
    public enum SpeciesMovementIntent
    {
        Wander,
        Food,
        Mate,
    }

    public readonly struct SpeciesPerceivedTarget
    {
        public SpeciesPerceivedTarget(SpeciesMovementIntent intent, Vector2Int location, SpeciesCell cell)
        {
            Intent = intent;
            Location = location;
            Cell = cell;
        }

        public SpeciesMovementIntent Intent { get; }
        public Vector2Int Location { get; }
        public SpeciesCell Cell { get; }
    }

    public static class SpeciesPerception
    {
        public static bool TryFindFoodTarget(
            Grid<SpeciesCell> cells,
            int x,
            int y,
            SpeciesRules rules,
            System.Random random,
            out SpeciesPerceivedTarget target)
        {
            if (rules == null || !rules.DietTargetId.HasValue)
            {
                target = default;
                return false;
            }

            return TryFindTarget(
                cells,
                x,
                y,
                rules.Awareness.VisionPattern,
                rules.DietTargetId.Value,
                SpeciesMovementIntent.Food,
                requireCreature: false,
                random,
                out target);
        }

        public static bool TryFindMateTarget(
            Grid<SpeciesCell> cells,
            int x,
            int y,
            SpeciesId species,
            SpeciesRules rules,
            System.Random random,
            out SpeciesPerceivedTarget target)
        {
            return TryFindTarget(
                cells,
                x,
                y,
                rules.Awareness.VisionPattern,
                species,
                SpeciesMovementIntent.Mate,
                requireCreature: true,
                random,
                out target);
        }

        public static bool IsDietTarget(SpeciesCell cell, SpeciesId target)
        {
            return target == SpeciesIds.Plant
                ? cell.IsPlantResource && !cell.IsCreature
                : cell.IsCreature && cell.SpeciesId == target;
        }

        static bool TryFindTarget(
            Grid<SpeciesCell> cells,
            int x,
            int y,
            GridPattern visionPattern,
            SpeciesId targetSpecies,
            SpeciesMovementIntent intent,
            bool requireCreature,
            System.Random random,
            out SpeciesPerceivedTarget target)
        {
            if (cells == null)
            {
                throw new ArgumentNullException(nameof(cells));
            }

            if (visionPattern == null)
            {
                throw new ArgumentNullException(nameof(visionPattern));
            }

            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            var bestDistance = int.MaxValue;
            target = default;
            foreach (var offset in visionPattern.Offsets)
            {
                var targetX = x + offset.x;
                var targetY = y + offset.y;
                if (!cells.TryGetCell(targetX, targetY, out var candidate)
                    || (requireCreature
                        ? !candidate.IsCreature || candidate.SpeciesId != targetSpecies
                        : !IsDietTarget(candidate, targetSpecies)))
                {
                    continue;
                }

                var distance = Math.Max(Math.Abs(offset.x), Math.Abs(offset.y));
                if (distance < bestDistance || (distance == bestDistance && random.Next(2) == 0))
                {
                    bestDistance = distance;
                    target = new SpeciesPerceivedTarget(intent, new Vector2Int(targetX, targetY), candidate);
                }
            }

            return bestDistance != int.MaxValue;
        }
    }
}
