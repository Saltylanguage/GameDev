using System;
using System.Collections.Generic;

namespace SaltyGame
{
    public enum SimulationRunStatus
    {
        Ready,
        Running,
        Paused,
        Complete,
    }

    public readonly struct SpeciesPopulationSnapshot
    {
        public SpeciesPopulationSnapshot(int tick, int plants, int herbivores, int carnivores, int empty)
        {
            Tick = tick;
            Plants = plants;
            Herbivores = herbivores;
            Carnivores = carnivores;
            Empty = empty;
        }

        public int Tick { get; }
        public int Plants { get; }
        public int Herbivores { get; }
        public int Carnivores { get; }
        public int Empty { get; }

        public static SpeciesPopulationSnapshot Create(Grid<SpeciesCell> cells, int tick)
        {
            var plants = 0;
            var herbivores = 0;
            var carnivores = 0;
            var empty = 0;
            for (var y = 0; y < cells.Height; y++)
            {
                for (var x = 0; x < cells.Width; x++)
                {
                    var cell = cells.GetCell(x, y);
                    if (!cell.IsPlantResource && !cell.IsCreature)
                    {
                        empty++;
                        continue;
                    }

                    if (cell.IsPlantResource)
                    {
                        plants++;
                    }

                    if (!cell.IsCreature)
                    {
                        if (!cell.IsPlantResource)
                        {
                            empty++;
                        }

                        continue;
                    }

                    if (cell.SpeciesId == SpeciesIds.Herbivore)
                    {
                        herbivores++;
                    }
                    else if (cell.SpeciesId == SpeciesIds.Carnivore)
                    {
                        carnivores++;
                    }
                }
            }

            return new SpeciesPopulationSnapshot(
                tick,
                plants,
                herbivores,
                carnivores,
                empty);
        }
    }

    public sealed class SimulationRunState
    {
        readonly Grid<SpeciesCell> initialCells;
        readonly List<SpeciesPopulationSnapshot> populationHistory;

        public SimulationRunState(
            Grid<SpeciesCell> cells,
            SpeciesId playerSpecies,
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

            initialCells = cells.Copy();
            Cells = cells;
            PlayerSpeciesId = playerSpecies;
            Seed = seed;
            DurationSeconds = durationSeconds;
            Status = SimulationRunStatus.Ready;
            populationHistory = new List<SpeciesPopulationSnapshot>
            {
                SpeciesPopulationSnapshot.Create(cells, tick: 0),
            };
            PopulationHistory = populationHistory;
        }

        public Grid<SpeciesCell> Cells { get; private set; }
        public SpeciesId PlayerSpeciesId { get; }

        [Obsolete("Use PlayerSpeciesId instead.")]
        public SpeciesArchetype PlayerSpecies => SpeciesId.ToLegacyArchetype(PlayerSpeciesId);
        public int Seed { get; }
        public float DurationSeconds { get; }
        public float ElapsedSeconds { get; private set; }
        public int Tick { get; private set; }
        public SimulationRunStatus Status { get; private set; }
        public IReadOnlyList<SpeciesPopulationSnapshot> PopulationHistory { get; }

        public void Start()
        {
            if (Status != SimulationRunStatus.Ready)
            {
                throw new InvalidOperationException("Only a ready run can be started.");
            }

            Status = SimulationRunStatus.Running;
        }

        public void Pause()
        {
            if (Status != SimulationRunStatus.Running)
            {
                throw new InvalidOperationException("Only a running simulation can be paused.");
            }

            Status = SimulationRunStatus.Paused;
        }

        public void Resume()
        {
            if (Status != SimulationRunStatus.Paused)
            {
                throw new InvalidOperationException("Only a paused simulation can be resumed.");
            }

            Status = SimulationRunStatus.Running;
        }

        public void Restart()
        {
            if (Status != SimulationRunStatus.Running && Status != SimulationRunStatus.Paused)
            {
                throw new InvalidOperationException("Only a running or paused simulation can be restarted.");
            }

            Cells = initialCells.Copy();
            ElapsedSeconds = 0f;
            Tick = 0;
            Status = SimulationRunStatus.Ready;
            populationHistory.Clear();
            populationHistory.Add(SpeciesPopulationSnapshot.Create(Cells, tick: 0));
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
            populationHistory.Add(SpeciesPopulationSnapshot.Create(nextCells, Tick));

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
                    if (run.Cells.GetCell(x, y).IsCreature
                        && run.Cells.GetCell(x, y).SpeciesId == run.PlayerSpeciesId)
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
