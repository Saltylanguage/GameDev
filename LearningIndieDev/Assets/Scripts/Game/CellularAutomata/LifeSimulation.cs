using System;

namespace SaltyGame
{
    public static class LifeSimulation
    {
        public static Grid<LifeCell> CreateRandom(int width, int height, int seed, float aliveProbability)
        {
            if (aliveProbability < 0f || aliveProbability > 1f)
            {
                throw new ArgumentOutOfRangeException(nameof(aliveProbability));
            }

            var random = new Random(seed);
            return new Grid<LifeCell>(width, height, (_, _) =>
                new LifeCell(random.NextDouble() < aliveProbability));
        }

        public static Grid<LifeCell> Step(Grid<LifeCell> source, GridPattern neighborhood)
        {
            if (neighborhood == null)
            {
                throw new ArgumentNullException(nameof(neighborhood));
            }

            return GridSimulation.Step(source, (grid, x, y) =>
            {
                var aliveNeighbors = 0;
                foreach (var offset in neighborhood.Offsets)
                {
                    if (grid.TryGetCell(x + offset.x, y + offset.y, out var neighbor) && neighbor.IsAlive)
                    {
                        aliveNeighbors++;
                    }
                }

                var isAlive = grid.GetCell(x, y).IsAlive;
                return new LifeCell(aliveNeighbors == 3 || (isAlive && aliveNeighbors == 2));
            });
        }
    }
}
