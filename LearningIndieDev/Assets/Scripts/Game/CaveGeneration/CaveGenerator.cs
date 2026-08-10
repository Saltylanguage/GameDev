using System;
using UnityEngine;

namespace SaltyGame
{
    public static class CaveGenerator
    {
        public static Grid<CaveCell> Generate(CaveGenerationSettings settings, int seed)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            var random = new System.Random(seed);
            var cave = new Grid<CaveCell>(settings.Width, settings.Height, (x, y) =>
                new CaveCell(IsBoundary(x, y, settings.Width, settings.Height)
                    || random.NextDouble() < settings.InitialWallProbability));

            for (var step = 0; step < settings.SimulationSteps; step++)
            {
                cave = SimulateStep(cave, CaveCell.Neighborhood, settings.WallNeighborThreshold);
            }

            return cave;
        }

        public static Grid<CaveCell> SimulateStep(
            Grid<CaveCell> source,
            GridPattern neighborhood,
            int wallNeighborThreshold)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (neighborhood == null)
            {
                throw new ArgumentNullException(nameof(neighborhood));
            }

            if (wallNeighborThreshold < 0 || wallNeighborThreshold > neighborhood.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(wallNeighborThreshold), wallNeighborThreshold, "Wall neighbor threshold must fit within the supplied neighborhood.");
            }

            return GridSimulation.Step(source, (cave, x, y) =>
            {
                var neighboringWalls = CountNeighboringWalls(cave, neighborhood, x, y);
                return new CaveCell(neighboringWalls >= wallNeighborThreshold);
            });
        }

        static int CountNeighboringWalls(Grid<CaveCell> cave, GridPattern neighborhood, int x, int y)
        {
            var wallCount = 0;
            foreach (Vector2Int offset in neighborhood.Offsets)
            {
                var neighborX = x + offset.x;
                var neighborY = y + offset.y;
                if (!cave.IsInBounds(neighborX, neighborY) || cave.GetCell(neighborX, neighborY).IsWall)
                {
                    wallCount++;
                }
            }

            return wallCount;
        }

        static bool IsBoundary(int x, int y, int width, int height)
        {
            return x == 0 || y == 0 || x == width - 1 || y == height - 1;
        }
    }
}
