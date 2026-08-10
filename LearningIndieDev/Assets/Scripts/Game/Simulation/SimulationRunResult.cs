using System;

namespace SaltyGame
{
    public enum SimulationRunStatus
    {
        Ready,
        Running,
        Complete,
    }

    public sealed class SimulationRunState
    {
        public SimulationRunState(
            Grid<SpeciesCell> cells,
            SpeciesArchetype playerSpecies,
            int seed,
            float durationSeconds)
        {
            if (cells == null)
            {
                throw new ArgumentNullException(nameof(cells));
            }

            if (durationSeconds <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(durationSeconds), durationSeconds, "Run duration must be greater than zero.");
            }

            Cells = cells;
            PlayerSpecies = playerSpecies;
            Seed = seed;
            DurationSeconds = durationSeconds;
            Status = SimulationRunStatus.Ready;
        }

        public Grid<SpeciesCell> Cells { get; private set; }
        public SpeciesArchetype PlayerSpecies { get; }
        public int Seed { get; }
        public float DurationSeconds { get; }
        public float ElapsedSeconds { get; private set; }
        public int Tick { get; private set; }
        public SimulationRunStatus Status { get; private set; }

        public void Start()
        {
            if (Status != SimulationRunStatus.Ready)
            {
                throw new InvalidOperationException("Only a ready run can be started.");
            }

            Status = SimulationRunStatus.Running;
        }

        public void Advance(Grid<SpeciesCell> nextCells, float stepSeconds)
        {
            if (Status != SimulationRunStatus.Running)
            {
                throw new InvalidOperationException("Only a running simulation can advance.");
            }

            if (nextCells == null)
            {
                throw new ArgumentNullException(nameof(nextCells));
            }

            if (stepSeconds <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(stepSeconds), stepSeconds, "Simulation step must be greater than zero.");
            }

            if (nextCells.Width != Cells.Width || nextCells.Height != Cells.Height)
            {
                throw new ArgumentException("The next grid must match the current grid dimensions.", nameof(nextCells));
            }

            Cells = nextCells;
            ElapsedSeconds = Math.Min(DurationSeconds, ElapsedSeconds + stepSeconds);
            Tick++;

            if (ElapsedSeconds >= DurationSeconds)
            {
                Status = SimulationRunStatus.Complete;
            }
        }
    }

    public readonly struct SimulationRunResult
    {
        public SimulationRunResult(int ticks, float durationSeconds, int playerPopulation, int currencyEarned)
        {
            Ticks = ticks;
            DurationSeconds = durationSeconds;
            PlayerPopulation = playerPopulation;
            CurrencyEarned = currencyEarned;
        }

        public int Ticks { get; }
        public float DurationSeconds { get; }
        public int PlayerPopulation { get; }
        public int CurrencyEarned { get; }
    }

    public static class SimulationRunResults
    {
        public static SimulationRunResult Create(SimulationRunState run)
        {
            var playerPopulation = 0;
            for (var y = 0; y < run.Cells.Height; y++)
            {
                for (var x = 0; x < run.Cells.Width; x++)
                {
                    if (run.Cells.GetCell(x, y).IsOccupied
                        && run.Cells.GetCell(x, y).Species == run.PlayerSpecies)
                    {
                        playerPopulation++;
                    }
                }
            }

            return new SimulationRunResult(
                run.Tick,
                run.ElapsedSeconds,
                playerPopulation,
                playerPopulation);
        }
    }
}
