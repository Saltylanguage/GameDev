using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
        static readonly IReadOnlyDictionary<SpeciesId, int> EmptyCounts =
            new ReadOnlyDictionary<SpeciesId, int>(new Dictionary<SpeciesId, int>());
        readonly IReadOnlyDictionary<SpeciesId, int> counts;

        public SpeciesPopulationSnapshot(
            int tick,
            IReadOnlyDictionary<SpeciesId, int> counts,
            int empty)
        {
            if (counts == null)
            {
                throw new ArgumentNullException(nameof(counts));
            }

            if (empty < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(empty), empty, "Empty cell count cannot be negative.");
            }

            var copiedCounts = new Dictionary<SpeciesId, int>(counts.Count);
            foreach (var entry in counts)
            {
                if (!entry.Key.IsValid)
                {
                    throw new ArgumentException("Population counts cannot use an empty species id.", nameof(counts));
                }

                if (entry.Value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(counts), entry.Value, "Population counts cannot be negative.");
                }

                copiedCounts.Add(entry.Key, entry.Value);
            }

            Tick = tick;
            this.counts = new ReadOnlyDictionary<SpeciesId, int>(copiedCounts);
            Empty = empty;
        }

        [Obsolete("Use the species-id keyed constructor instead.")]
        public SpeciesPopulationSnapshot(int tick, int plants, int herbivores, int carnivores, int empty)
            : this(
                tick,
                new Dictionary<SpeciesId, int>
                {
                    [SpeciesIds.Plant] = plants,
                    [SpeciesIds.Herbivore] = herbivores,
                    [SpeciesIds.Carnivore] = carnivores,
                },
                empty)
        {
        }

        public int Tick { get; }
        public IReadOnlyDictionary<SpeciesId, int> Counts => counts ?? EmptyCounts;
        public int Plants => GetCount(SpeciesIds.Plant);
        public int Herbivores => GetCount(SpeciesIds.Herbivore);
        public int Carnivores => GetCount(SpeciesIds.Carnivore);
        public int Empty { get; }

        public int GetCount(SpeciesId species)
        {
            return counts != null && counts.TryGetValue(species, out var count) ? count : 0;
        }

        public static SpeciesPopulationSnapshot Create(Grid<SpeciesCell> cells, int tick)
        {
            if (cells == null)
            {
                throw new ArgumentNullException(nameof(cells));
            }

            var counts = new Dictionary<SpeciesId, int>();
            var empty = 0;
            for (var y = 0; y < cells.Height; y++)
            {
                for (var x = 0; x < cells.Width; x++)
                {
                    var cell = cells.GetCell(x, y);
                    var hasPopulation = false;
                    if (cell.IsCreature)
                    {
                        AddCount(counts, cell.SpeciesId);
                        hasPopulation = true;
                    }

                    if (cell.IsPlantResource)
                    {
                        var resourceSpecies = cell.ResourceSpeciesId.IsValid
                            ? cell.ResourceSpeciesId
                            : cell.SpeciesId.IsValid ? cell.SpeciesId : SpeciesIds.Plant;
                        AddCount(counts, resourceSpecies);
                        hasPopulation = true;
                    }

                    if (!hasPopulation)
                    {
                        empty++;
                    }
                }
            }

            return new SpeciesPopulationSnapshot(tick, counts, empty);
        }

        static void AddCount(Dictionary<SpeciesId, int> counts, SpeciesId species)
        {
            counts.TryGetValue(species, out var count);
            counts[species] = count + 1;
        }
    }

    public sealed class SimulationRunState
    {
        readonly Grid<SpeciesCell> initialCells;
        readonly List<SpeciesPopulationSnapshot> populationHistory;
        readonly SpeciesSimulationMetrics metrics;

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
            metrics = new SpeciesSimulationMetrics();
            Metrics = metrics;
        }

        public Grid<SpeciesCell> Cells { get; private set; }
        public SpeciesId PlayerSpeciesId { get; }

        [Obsolete("Use PlayerSpeciesId instead.")]
        public SpeciesArchetype PlayerSpecies => SpeciesId.ToLegacyArchetype(PlayerSpeciesId);
        public int Seed { get; }
        public string RulesetFingerprint { get; private set; }
        public float DurationSeconds { get; }
        public float ElapsedSeconds { get; private set; }
        public int Tick { get; private set; }
        public SimulationRunStatus Status { get; private set; }
        public IReadOnlyList<SpeciesPopulationSnapshot> PopulationHistory { get; }
        public SpeciesSimulationMetrics Metrics { get; }

        internal void SetRulesetFingerprint(string fingerprint)
        {
            if (string.IsNullOrWhiteSpace(fingerprint))
            {
                throw new ArgumentException("Ruleset fingerprint cannot be empty.", nameof(fingerprint));
            }

            RulesetFingerprint = fingerprint;
        }

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
            metrics.Clear();
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
        public SimulationRunResult(
            int ticks,
            float durationSeconds,
            int playerPopulation,
            int currencyEarned,
            string rulesetFingerprint = null)
        {
            Ticks = ticks;
            DurationSeconds = durationSeconds;
            PlayerPopulation = playerPopulation;
            CurrencyEarned = currencyEarned;
            RulesetFingerprint = rulesetFingerprint;
        }

        public int Ticks { get; }
        public float DurationSeconds { get; }
        public int PlayerPopulation { get; }
        public int CurrencyEarned { get; }
        public string RulesetFingerprint { get; }
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
                playerPopulation,
                run.RulesetFingerprint);
        }
    }
}
