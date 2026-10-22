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
        // Agreed starting values for the first scent comparison; tune from measured play.
        public const int ScentSampleIntervalTicks = 10;
        public const int BaseScentDurationTicks = 20;
        public const int BaseScentRange = 16;
        public const int DroppingScentDurationTicks = BaseScentDurationTicks * 2;
        public const int DroppingScentRange = BaseScentRange * 3 / 2;
        public const int SniffedTargetRange = DroppingScentRange * 3 / 2;

        public static bool TryFindDroppingScent(
            Grid<SpeciesCell> cells,
            int x,
            int y,
            SpeciesId preySpecies,
            out SpeciesPerceivedTarget target,
            out SpeciesScentMarker marker)
        {
            var bestDistance = int.MaxValue;
            target = default;
            marker = default;
            for (var targetY = 0; targetY < cells.Height; targetY++)
            {
                for (var targetX = 0; targetX < cells.Width; targetX++)
                {
                    var cell = cells.GetCell(targetX, targetY);
                    var scent = cell.ScentMarker;
                    var distance = cells.GetDistance(x, y, targetX, targetY);
                    if (!scent.IsActive || !scent.IsDropping
                        || scent.OwnerSpeciesId != preySpecies
                        || distance > DroppingScentRange || distance >= bestDistance)
                    {
                        continue;
                    }

                    bestDistance = distance;
                    marker = scent;
                    target = new SpeciesPerceivedTarget(
                        SpeciesMovementIntent.Food,
                        new Vector2Int(targetX, targetY),
                        cell);
                }
            }

            return bestDistance != int.MaxValue;
        }

        public static bool TryFindMovementScent(
            Grid<SpeciesCell> cells,
            int x,
            int y,
            SpeciesId preySpecies,
            out SpeciesPerceivedTarget target,
            out SpeciesScentMarker marker)
        {
            var bestLifetime = 0;
            var bestDistance = int.MaxValue;
            target = default;
            marker = default;
            for (var targetY = 0; targetY < cells.Height; targetY++)
            {
                for (var targetX = 0; targetX < cells.Width; targetX++)
                {
                    var cell = cells.GetCell(targetX, targetY);
                    var scent = cell.ScentMarker;
                    var distance = cells.GetDistance(x, y, targetX, targetY);
                    if (!scent.IsActive || scent.IsDropping
                        || scent.OwnerSpeciesId != preySpecies
                        || distance > BaseScentRange
                        || scent.TicksRemaining < bestLifetime
                        || (scent.TicksRemaining == bestLifetime && distance >= bestDistance))
                    {
                        continue;
                    }

                    bestLifetime = scent.TicksRemaining;
                    bestDistance = distance;
                    marker = scent;
                    target = new SpeciesPerceivedTarget(
                        SpeciesMovementIntent.Food,
                        new Vector2Int(targetX, targetY),
                        cell);
                }
            }

            return bestLifetime > 0;
        }

        public static bool TryFindSniffedTarget(
            Grid<SpeciesCell> cells,
            int x,
            int y,
            SpeciesId preySpecies,
            long entityId,
            out SpeciesPerceivedTarget target)
        {
            target = default;
            var bestLifetime = 0;
            var bestDistance = int.MaxValue;
            for (var targetY = 0; targetY < cells.Height; targetY++)
            {
                for (var targetX = 0; targetX < cells.Width; targetX++)
                {
                    var cell = cells.GetCell(targetX, targetY);
                    var scent = cell.ScentMarker;
                    var distance = cells.GetDistance(x, y, targetX, targetY);
                    if (!scent.IsActive || scent.IsDropping
                        || scent.OwnerSpeciesId != preySpecies
                        || scent.OwnerEntityId != entityId
                        || distance > SniffedTargetRange
                        || scent.TicksRemaining < bestLifetime
                        || (scent.TicksRemaining == bestLifetime && distance >= bestDistance))
                    {
                        continue;
                    }

                    bestLifetime = scent.TicksRemaining;
                    bestDistance = distance;
                    target = new SpeciesPerceivedTarget(
                        SpeciesMovementIntent.Food,
                        new Vector2Int(targetX, targetY),
                        cell);
                }
            }

            return bestLifetime > 0;
        }

        public static bool TryFindScentMemoryTarget(
            Grid<SpeciesCell> cells,
            SpeciesScentMemory memory,
            out SpeciesPerceivedTarget target)
        {
            target = default;
            if (!memory.IsActive || !cells.TryGetCell(memory.X, memory.Y, out var cell))
            {
                return false;
            }

            target = new SpeciesPerceivedTarget(
                SpeciesMovementIntent.Food,
                new Vector2Int(memory.X, memory.Y),
                cell);
            return true;
        }

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

        public static bool TryFindTrackedFoodTarget(
            Grid<SpeciesCell> cells,
            SpeciesCell hunter,
            SpeciesRules rules,
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

            if (!hunter.IsCreature
                || hunter.TrackingTargetEntityId <= 0
                || hunter.TrackingTicksRemaining <= 0
                || !rules.DietTargetId.HasValue)
            {
                target = default;
                return false;
            }

            for (var y = 0; y < cells.Height; y++)
            {
                for (var x = 0; x < cells.Width; x++)
                {
                    var candidate = cells.GetCell(x, y);
                    if (candidate.IsCreature
                        && candidate.EntityId == hunter.TrackingTargetEntityId
                        && IsDietTarget(candidate, rules.DietTargetId.Value))
                    {
                        target = new SpeciesPerceivedTarget(
                            SpeciesMovementIntent.Food,
                            new Vector2Int(x, y),
                            candidate);
                        return true;
                    }
                }
            }

            target = default;
            return false;
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
            foreach (var offset in cells.GetPattern(visionPattern).Offsets)
            {
                var targetX = x + offset.x;
                var targetY = y + offset.y;
                if (!cells.TryResolveCoordinates(ref targetX, ref targetY)
                    || !cells.TryGetCell(targetX, targetY, out var candidate)
                    || !candidate.IsCreature
                    || !rules.TryGetValue(candidate.SpeciesId, out var threatRules)
                    || threatRules.DietTargetId != species)
                {
                    continue;
                }

                var distance = cells.GetDistance(x, y, targetX, targetY);
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
            foreach (var offset in cells.GetPattern(speciesRules.Awareness.VisionPattern).Offsets)
            {
                var targetX = x + offset.x;
                var targetY = y + offset.y;
                if (!cells.TryResolveCoordinates(ref targetX, ref targetY)
                    || !cells.TryGetCell(targetX, targetY, out var candidate)
                    || !candidate.IsCreature
                    || !rules.TryGetValue(candidate.SpeciesId, out var threatRules)
                    || threatRules.DietTargetId != species
                    || !previousEntityPositions.TryGetValue(candidate.EntityId, out var previousThreat))
                {
                    continue;
                }

                var currentDistance = cells.GetDistance(x, y, targetX, targetY);
                var previousDistance = cells.GetDistance(previousSubject.x, previousSubject.y, previousThreat.x, previousThreat.y);
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
                        || !IsInAttackRange(cells, threatX, threatY, x, y, threatRules.AttackPattern))
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
            foreach (var offset in cells.GetPattern(visionPattern).Offsets)
            {
                var targetX = x + offset.x;
                var targetY = y + offset.y;
                if (!cells.TryResolveCoordinates(ref targetX, ref targetY)
                    || !cells.TryGetCell(targetX, targetY, out var candidate)
                    || (requireCreature
                        ? !candidate.IsCreature || candidate.SpeciesId != targetSpecies
                        : !IsDietTarget(candidate, targetSpecies)))
                {
                    continue;
                }

                var distance = cells.GetDistance(x, y, targetX, targetY);
                if (distance < bestDistance || (distance == bestDistance && random.Next(2) == 0))
                {
                    bestDistance = distance;
                    target = new SpeciesPerceivedTarget(intent, new Vector2Int(targetX, targetY), candidate);
                }
            }

            return bestDistance != int.MaxValue;
        }

        static bool IsInAttackRange(
            Grid<SpeciesCell> cells,
            int attackerX,
            int attackerY,
            int targetX,
            int targetY,
            GridPattern attackPattern)
        {
            foreach (var offset in cells.GetPattern(attackPattern).Offsets)
            {
                var candidateX = attackerX + offset.x;
                var candidateY = attackerY + offset.y;
                if (cells.TryResolveCoordinates(ref candidateX, ref candidateY)
                    && candidateX == targetX && candidateY == targetY)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
