using System;

namespace SaltyGame
{
    public sealed class CaveGenerationSettings
    {
        public CaveGenerationSettings(
            int width,
            int height,
            float initialWallProbability = 0.45f,
            int simulationSteps = 5,
            int wallNeighborThreshold = 5)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), width, "Cave width must be greater than zero.");
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), height, "Cave height must be greater than zero.");
            }

            if (initialWallProbability < 0f || initialWallProbability > 1f)
            {
                throw new ArgumentOutOfRangeException(nameof(initialWallProbability), initialWallProbability, "Initial wall probability must be between zero and one.");
            }

            if (simulationSteps < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(simulationSteps), simulationSteps, "Simulation steps cannot be negative.");
            }

            if (wallNeighborThreshold < 0 || wallNeighborThreshold > CaveCell.Neighborhood.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(wallNeighborThreshold), wallNeighborThreshold, "Wall neighbor threshold must fit within the cave neighborhood.");
            }

            Width = width;
            Height = height;
            InitialWallProbability = initialWallProbability;
            SimulationSteps = simulationSteps;
            WallNeighborThreshold = wallNeighborThreshold;
        }

        public int Width { get; }
        public int Height { get; }
        public float InitialWallProbability { get; }
        public int SimulationSteps { get; }
        public int WallNeighborThreshold { get; }
    }
}
