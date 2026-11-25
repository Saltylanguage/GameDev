using System;

namespace SaltyGame
{
    public static class GridSimulation
    {
        public static Grid<T> Step<T>(Grid<T> source, Func<Grid<T>, int, int, T> nextCell)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (nextCell == null)
            {
                throw new ArgumentNullException(nameof(nextCell));
            }

            var next = new Grid<T>(source.Width, source.Height, source.Topology);
            foreach (var position in source.Topology.PlayableCoordinates)
                next.SetCell(position.x, position.y, nextCell(source, position.x, position.y));
            return next;
        }
    }
}
