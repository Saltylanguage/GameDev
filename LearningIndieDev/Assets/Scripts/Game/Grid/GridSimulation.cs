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

            return new Grid<T>(source.Width, source.Height, (x, y) => nextCell(source, x, y));
        }
    }
}
