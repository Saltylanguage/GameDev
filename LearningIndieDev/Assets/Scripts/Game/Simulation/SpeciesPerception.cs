using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
    public enum SpeciesMovementIntent
    {
        Wander,
        Food,
        Mate,
        Flee,
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
        public static IReadOnlyDictionary<long, Vector2Int> BuildEntityPositionIndex(
            Grid<SpeciesCell> cells)
        {
            if (cells == null)
            {
                throw new ArgumentNullException(nameof(cells));
            }

            var positions = new Dictionary<long, Vector2Int>();
            for (var y = 0; y < cells.Height; y++)
            {
                for (var x = 0; x < cells.Width; x++)
                {
                    var cell = cells.GetCell(x, y);
                    if (cell.IsCreature && cell.EntityId > 0)
                    {
                        positions[cell.EntityId] = new Vector2Int(x, y);
                    }
                }
            }

            return positions;
        }

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

        public static bool TryFindThreatTarget(
            Grid<SpeciesCell> cells,
            int x,
            int y,
            SpeciesId species,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules,
            System.Random random,
            out SpeciesPerceivedTarget target)
        {
            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            if (!rules.TryGetValue(species, out var speciesRules))
            {
                target = default;
                return false;
            }

            var visionPattern = speciesRules.Awareness.VisionPattern;
            var bestDistance = int.MaxValue;
            target = default;
            foreach (var offset in visionPattern.Offsets)
            {
                var targetX = x + offset.x;
                var targetY = y + offset.y;
                if (!cells.TryGetCell(targetX, targetY, out var candidate)
                    || !candidate.IsCreature
                    || !rules.TryGetValue(candidate.SpeciesId, out var threatRules)
                    || threatRules.DietTargetId != species)
                {
                    continue;
                }

                var distance = Math.Max(Math.Abs(offset.x), Math.Abs(offset.y));
                if (distance < bestDistance || (distance == bestDistance && random.Next(2) == 0))
                {
                    bestDistance = distance;
                    target = new SpeciesPerceivedTarget(
                        SpeciesMovementIntent.Flee,
                        new Vector2Int(targetX, targetY),
                        candidate);
                }
            }

            return bestDistance != int.MaxValue;
        }

        public static bool TryFindApproachingThreatTarget(
            Grid<SpeciesCell> cells,
            int x,
            int y,
            SpeciesCell subject,
            SpeciesId species,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules,
            IReadOnlyDictionary<long, Vector2Int> previousEntityPositions,
            System.Random random,
            out SpeciesPerceivedTarget target)
        {
            if (cells == null)
            {
                throw new ArgumentNullException(nameof(cells));
            }

            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            if (previousEntityPositions == null)
            {
                throw new ArgumentNullException(nameof(previousEntityPositions));
            }

            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            if (!rules.TryGetValue(species, out var speciesRules))
            {
                target = default;
                return false;
            }

            if (!previousEntityPositions.TryGetValue(subject.EntityId, out var previousSubject))
            {
                target = default;
                return false;
            }

            var bestDistance = int.MaxValue;
            target = default;
            foreach (var offset in speciesRules.Awareness.VisionPattern.Offsets)
            {
                var targetX = x + offset.x;
                var targetY = y + offset.y;
                if (!cells.TryGetCell(targetX, targetY, out var candidate)
                    || !candidate.IsCreature
                    || !rules.TryGetValue(candidate.SpeciesId, out var threatRules)
                    || threatRules.DietTargetId != species
                    || !previousEntityPositions.TryGetValue(candidate.EntityId, out var previousThreat))
                {
                    continue;
                }

                var currentDistance = Math.Max(
                    Math.Abs(targetX - x),
                    Math.Abs(targetY - y));
                var previousDistance = Math.Max(
                    Math.Abs(previousThreat.x - previousSubject.x),
                    Math.Abs(previousThreat.y - previousSubject.y));
                if (currentDistance >= previousDistance)
                {
                    continue;
                }

                if (currentDistance < bestDistance
                    || (currentDistance == bestDistance && random.Next(2) == 0))
                {
                    bestDistance = currentDistance;
                    target = new SpeciesPerceivedTarget(
                        SpeciesMovementIntent.Flee,
                        new Vector2Int(targetX, targetY),
                        candidate);
                }
            }

            return bestDistance != int.MaxValue;
        }

        public static bool IsSafeFromThreats(
            Grid<SpeciesCell> cells,
            int x,
            int y,
            SpeciesId species,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules)
        {
            if (cells == null)
            {
                throw new ArgumentNullException(nameof(cells));
            }

            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            for (var threatY = 0; threatY < cells.Height; threatY++)
            {
                for (var threatX = 0; threatX < cells.Width; threatX++)
                {
                    var threat = cells.GetCell(threatX, threatY);
                    if (!threat.IsCreature
                        || !rules.TryGetValue(threat.SpeciesId, out var threatRules)
                        || threatRules.DietTargetId != species
                        || !IsInAttackRange(threatX, threatY, x, y, threatRules.AttackPattern))
                    {
                        continue;
                    }

                    return false;
                }
            }

            return true;
        }

        public static bool IsDietTarget(SpeciesCell cell, SpeciesId target)
        {
            return cell.IsPlantResource
                ? !cell.IsCreature && cell.SpeciesId == target
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

        static bool IsInAttackRange(
            int attackerX,
            int attackerY,
            int targetX,
            int targetY,
            GridPattern attackPattern)
        {
            foreach (var offset in attackPattern.Offsets)
            {
                if (attackerX + offset.x == targetX
                    && attackerY + offset.y == targetY)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
