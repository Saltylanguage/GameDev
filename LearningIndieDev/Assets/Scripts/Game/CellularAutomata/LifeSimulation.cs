using System;

namespace SaltyGame
{
    public static class LifeSimulation
    {
        public static Grid<LifeCell> CreateRandom(int width, int height, int seed, float lifeProbability)
        {
            if (lifeProbability < 0f || lifeProbability > 0.8f)
            {
                throw new ArgumentOutOfRangeException(nameof(lifeProbability));
            }

            var random = new Random(seed);
            return new Grid<LifeCell>(width, height, (_, _) =>
            {
                var roll = random.NextDouble();
                if (roll < 0.02)
                {
                    return new LifeCell(LifeCell.State.Fire, 100f);
                }

                if (roll < 0.2)
                {
                    return new LifeCell(LifeCell.State.Plant);
                }

                return new LifeCell(roll < 0.2 + lifeProbability
                    ? LifeCell.State.Life
                    : LifeCell.State.Empty);
            });
        }

        public static Grid<LifeCell> Step(Grid<LifeCell> source, GridPattern neighborhood, int seed)
        {
            if (neighborhood == null)
            {
                throw new ArgumentNullException(nameof(neighborhood));
            }

            var random = new Random(seed);
            return GridSimulation.Step(source, (grid, x, y) =>
                StepCell(grid, neighborhood, x, y, random.NextDouble()));
        }

        static LifeCell StepCell(
            Grid<LifeCell> grid,
            GridPattern neighborhood,
            int x,
            int y,
            double randomValue)
        {
            var cell = grid.GetCell(x, y);
            var lifeNeighbors = 0;
            var touchesFire = false;
            var totalHeat = cell.Temperature;
            var samples = 1;

            foreach (var offset in neighborhood.Offsets)
            {
                if (!grid.TryGetCell(x + offset.x, y + offset.y, out var neighbor))
                {
                    continue;
                }

                lifeNeighbors += neighbor.CurrentState == LifeCell.State.Life ? 1 : 0;
                touchesFire |= neighbor.CurrentState == LifeCell.State.Fire;
                totalHeat += neighbor.Temperature;
                samples++;
            }

            var temperature = totalHeat / samples;
            switch (cell.CurrentState)
            {
                case LifeCell.State.Fire:
                    return new LifeCell(LifeCell.State.Empty, 100f);
                case LifeCell.State.Plant:
                    return new LifeCell(touchesFire ? LifeCell.State.Fire : LifeCell.State.Plant, temperature);
                case LifeCell.State.Life:
                    return new LifeCell(lifeNeighbors == 2 || lifeNeighbors == 3
                        ? LifeCell.State.Life
                        : LifeCell.State.Empty, temperature);
                default:
                    if (lifeNeighbors == 3)
                    {
                        return new LifeCell(LifeCell.State.Life, temperature);
                    }

                    return new LifeCell(randomValue < 0.01
                        ? LifeCell.State.Plant
                        : LifeCell.State.Empty, temperature);
            }
        }
    }
}
