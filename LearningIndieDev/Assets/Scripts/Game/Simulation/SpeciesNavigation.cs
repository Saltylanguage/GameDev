using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
    public static class SpeciesNavigation
    {
        public static bool TryFindNextStep(
            Grid<SpeciesCell> cells,
            Vector2Int start,
            Vector2Int target,
            GridPattern movementPattern,
            GridPattern interactionPattern,
            System.Random random,
            out Vector2Int nextStep)
        {
            if (cells == null)
            {
                throw new ArgumentNullException(nameof(cells));
            }

            if (!cells.IsPlayableCell(start.x, start.y))
            {
                throw new ArgumentOutOfRangeException(nameof(start));
            }

            if (movementPattern == null)
            {
                throw new ArgumentNullException(nameof(movementPattern));
            }

            if (interactionPattern == null)
            {
                throw new ArgumentNullException(nameof(interactionPattern));
            }

            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            var startIndex = GetIndex(cells, start.x, start.y);
            var previous = new int[checked(cells.Width * cells.Height)];
            Array.Fill(previous, -1);
            previous[startIndex] = startIndex;
            var queue = new Queue<int>();
            queue.Enqueue(startIndex);
            var initialPattern = cells.GetPattern(start.x, start.y, movementPattern);
            var offsetStart = initialPattern.Count == 0 ? 0 : random.Next(initialPattern.Count);

            while (queue.Count > 0)
            {
                var currentIndex = queue.Dequeue();
                var currentX = currentIndex % cells.Width;
                var currentY = currentIndex / cells.Width;
                if (currentIndex != startIndex
                    && IsInInteractionRange(cells, currentX, currentY, target, interactionPattern))
                {
                    nextStep = ReconstructFirstStep(previous, startIndex, currentIndex, cells.Width);
                    return true;
                }

                var currentPattern = cells.GetPattern(currentX, currentY, movementPattern);
                for (var offsetIndex = 0; offsetIndex < currentPattern.Count; offsetIndex++)
                {
                    var offset = currentPattern.Offsets[(offsetStart + offsetIndex) % currentPattern.Count];
                    if (offset.x == 0 && offset.y == 0)
                    {
                        continue;
                    }

                    var nextXResolved = cells.TryResolveOffset(currentX, currentY, offset, out var nextX, out var nextY);
                    if (!nextXResolved)
                    {
                        continue;
                    }

                    var nextIndex = GetIndex(cells, nextX, nextY);
                    var nextCell = cells.GetCell(nextX, nextY);
                    if (previous[nextIndex] >= 0
                        || !nextCell.IsPassable
                        || nextCell.IsCreature)
                    {
                        continue;
                    }

                    previous[nextIndex] = currentIndex;
                    queue.Enqueue(nextIndex);
                }
            }

            nextStep = default;
            return false;
        }

        static bool IsInInteractionRange(Grid<SpeciesCell> cells, int x, int y, Vector2Int target, GridPattern interactionPattern)
        {
            foreach (var offset in cells.GetPattern(x, y, interactionPattern).Offsets)
            {
                var targetXResolved = cells.TryResolveOffset(x, y, offset, out var targetX, out var targetY);
                if (targetXResolved
                    && targetX == target.x && targetY == target.y)
                {
                    return true;
                }
            }

            return false;
        }

        static Vector2Int ReconstructFirstStep(int[] previous, int startIndex, int currentIndex, int width)
        {
            while (previous[currentIndex] != startIndex)
            {
                currentIndex = previous[currentIndex];
            }

            return new Vector2Int(currentIndex % width, currentIndex / width);
        }

        static int GetIndex<T>(Grid<T> grid, int x, int y)
        {
            return x + y * grid.Width;
        }
    }
}
