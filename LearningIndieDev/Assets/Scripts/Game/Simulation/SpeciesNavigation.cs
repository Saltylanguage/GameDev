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

            if (!cells.IsInBounds(start.x, start.y))
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
            var previous = new int[cells.Count];
            Array.Fill(previous, -1);
            previous[startIndex] = startIndex;
            var queue = new Queue<int>();
            queue.Enqueue(startIndex);
            var offsetStart = movementPattern.Count == 0 ? 0 : random.Next(movementPattern.Count);

            while (queue.Count > 0)
            {
                var currentIndex = queue.Dequeue();
                var currentX = currentIndex % cells.Width;
                var currentY = currentIndex / cells.Width;
                if (currentIndex != startIndex
                    && IsInInteractionRange(currentX, currentY, target, interactionPattern))
                {
                    nextStep = ReconstructFirstStep(previous, startIndex, currentIndex, cells.Width);
                    return true;
                }

                for (var offsetIndex = 0; offsetIndex < movementPattern.Count; offsetIndex++)
                {
                    var offset = movementPattern.Offsets[(offsetStart + offsetIndex) % movementPattern.Count];
                    if (offset.x == 0 && offset.y == 0)
                    {
                        continue;
                    }

                    var nextX = currentX + offset.x;
                    var nextY = currentY + offset.y;
                    if (!cells.IsInBounds(nextX, nextY))
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

        static bool IsInInteractionRange(int x, int y, Vector2Int target, GridPattern interactionPattern)
        {
            foreach (var offset in interactionPattern.Offsets)
            {
                if (x + offset.x == target.x && y + offset.y == target.y)
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
