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

namespace SaltyGame
{
    public sealed class SimulationTestCriteria
    {
        public SimulationTestCriteria(
            float minimumFinalPlayerRatio = 0f,
            float maximumFinalPlayerRatio = float.MaxValue,
            int expectedInitialPlayerPopulation = -1,
            int maximumAllowedFinalExtinctions = int.MaxValue,
            int minimumPlayerStateTransitions = 0,
            IReadOnlyDictionary<SpeciesId, int> expectedInitialPopulations = null)
        {
            if (minimumFinalPlayerRatio < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(minimumFinalPlayerRatio));
            }

            if (maximumFinalPlayerRatio < minimumFinalPlayerRatio)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumFinalPlayerRatio));
            }

            if (expectedInitialPlayerPopulation < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(expectedInitialPlayerPopulation));
            }

            if (maximumAllowedFinalExtinctions < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumAllowedFinalExtinctions));
            }

            if (minimumPlayerStateTransitions < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minimumPlayerStateTransitions));
            }

            var initialPopulations = new Dictionary<SpeciesId, int>();
            if (expectedInitialPopulations != null)
            {
                foreach (var entry in expectedInitialPopulations)
                {
                    if (!entry.Key.IsValid)
                    {
                        throw new ArgumentException("Expected initial populations must use valid species IDs.", nameof(expectedInitialPopulations));
                    }

                    if (entry.Value < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(expectedInitialPopulations), entry.Value, "Expected populations cannot be negative.");
                    }

                    initialPopulations.Add(entry.Key, entry.Value);
                }
            }

            MinimumFinalPlayerRatio = minimumFinalPlayerRatio;
            MaximumFinalPlayerRatio = maximumFinalPlayerRatio;
            ExpectedInitialPlayerPopulation = expectedInitialPlayerPopulation;
            MaximumAllowedFinalExtinctions = maximumAllowedFinalExtinctions;
            MinimumPlayerStateTransitions = minimumPlayerStateTransitions;
            ExpectedInitialPopulations = new ReadOnlyDictionary<SpeciesId, int>(initialPopulations);
        }

        public float MinimumFinalPlayerRatio { get; }
        public float MaximumFinalPlayerRatio { get; }
        public int ExpectedInitialPlayerPopulation { get; }
        public int MaximumAllowedFinalExtinctions { get; }
        public int MinimumPlayerStateTransitions { get; }
        public IReadOnlyDictionary<SpeciesId, int> ExpectedInitialPopulations { get; }
    }

    public sealed class SimulationTestCase
    {
        public SimulationTestCase(
            string name,
            CellularSimData data,
            SpeciesId playerSpecies,
            int seedStart,
            int seedCount,
            SimulationTestCriteria criteria)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Test case name cannot be empty.", nameof(name));
            }

            Data = data ?? throw new ArgumentNullException(nameof(data));
            if (!playerSpecies.IsValid || !data.SpeciesRules.ContainsKey(playerSpecies))
            {
                throw new ArgumentException("Test player species must be configured in the scenario.", nameof(playerSpecies));
            }

            if (seedCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(seedCount));
            }

            Criteria = criteria ?? throw new ArgumentNullException(nameof(criteria));
            Name = name;
            PlayerSpecies = playerSpecies;
            SeedStart = seedStart;
            SeedCount = seedCount;
        }

        public string Name { get; }
        public CellularSimData Data { get; }
        public SpeciesId PlayerSpecies { get; }
        public int SeedStart { get; }
        public int SeedCount { get; }
        public SimulationTestCriteria Criteria { get; }
    }

    public sealed class SimulationTestRunReport
    {
        internal SimulationTestRunReport(
            int seed,
            int initialPlayerPopulation,
            int finalPlayerPopulation,
            int peakPlayerPopulation,
            int playerStateTransitions,
            IReadOnlyList<string> failures)
        {
            Seed = seed;
            InitialPlayerPopulation = initialPlayerPopulation;
            FinalPlayerPopulation = finalPlayerPopulation;
            PeakPlayerPopulation = peakPlayerPopulation;
            PlayerStateTransitions = playerStateTransitions;
            Failures = failures;
        }

        public int Seed { get; }
        public int InitialPlayerPopulation { get; }
        public int FinalPlayerPopulation { get; }
        public int PeakPlayerPopulation { get; }
        public int PlayerStateTransitions { get; }
        public IReadOnlyList<string> Failures { get; }
        public bool Passed => Failures.Count == 0;
    }

    public sealed class SimulationTestReport
    {
        internal SimulationTestReport(
            SimulationTestCase testCase,
            IReadOnlyList<SimulationTestRunReport> runs,
            IReadOnlyList<string> failures)
        {
            TestCase = testCase;
            Runs = runs;
            Failures = failures;
        }

        public SimulationTestCase TestCase { get; }
        public IReadOnlyList<SimulationTestRunReport> Runs { get; }
        public IReadOnlyList<string> Failures { get; }
        public int PassedRuns => CountRuns(true);
        public int FailedRuns => CountRuns(false);
        public bool Passed => Failures.Count == 0 && FailedRuns == 0;

        int CountRuns(bool passed)
        {
            var count = 0;
            foreach (var run in Runs)
            {
                if (run.Passed == passed)
                {
                    count++;
                }
            }

            return count;
        }
    }

    public static class SimulationTestHarness
    {
        public static SimulationTestReport Run(SimulationTestCase testCase)
        {
            if (testCase == null)
            {
                throw new ArgumentNullException(nameof(testCase));
            }

            var runs = new List<SimulationTestRunReport>(testCase.SeedCount);
            var aggregateFailures = new List<string>();
            var finalExtinctions = 0;
            for (var index = 0; index < testCase.SeedCount; index++)
            {
                var seed = testCase.SeedStart + index;
                var initialGrid = SpeciesInitialGridFactory.Create(testCase.Data, seed);
                var run = new SimulationRunState(
                    initialGrid,
                    testCase.PlayerSpecies,
                    seed,
                    testCase.Data.RunDurationSeconds);
                var runner = new SpeciesSimulationRunner(run, testCase.Data);
                while (runner.AdvanceOneTick())
                {
                }

                var initialPlayerPopulation = run.PopulationHistory[0].GetCount(testCase.PlayerSpecies);
                var finalPlayerPopulation = run.PopulationHistory[run.PopulationHistory.Count - 1]
                    .GetCount(testCase.PlayerSpecies);
                var peakPlayerPopulation = 0;
                foreach (var snapshot in run.PopulationHistory)
                {
                    peakPlayerPopulation = Math.Max(
                        peakPlayerPopulation,
                        snapshot.GetCount(testCase.PlayerSpecies));
                }

                var failures = new List<string>();
                if (testCase.Criteria.ExpectedInitialPlayerPopulation >= 0
                    && initialPlayerPopulation != testCase.Criteria.ExpectedInitialPlayerPopulation)
                {
                    failures.Add(
                        $"Expected {testCase.Criteria.ExpectedInitialPlayerPopulation} initial "
                        + $"{testCase.PlayerSpecies}, got {initialPlayerPopulation}.");
                }

                foreach (var expected in testCase.Criteria.ExpectedInitialPopulations)
                {
                    var actual = run.PopulationHistory[0].GetCount(expected.Key);
                    if (actual != expected.Value)
                    {
                        failures.Add(
                            $"Expected {expected.Value} initial {expected.Key}, got {actual}.");
                    }
                }

                var ratio = initialPlayerPopulation == 0
                    ? 0f
                    : (float)finalPlayerPopulation / initialPlayerPopulation;
                if (ratio < testCase.Criteria.MinimumFinalPlayerRatio
                    || ratio > testCase.Criteria.MaximumFinalPlayerRatio)
                {
                    failures.Add(
                        $"Final {testCase.PlayerSpecies} ratio {ratio:0.###} is outside "
                        + $"[{testCase.Criteria.MinimumFinalPlayerRatio:0.###}, "
                        + $"{testCase.Criteria.MaximumFinalPlayerRatio:0.###}].");
                }

                var stateTransitions = run.Metrics.GetStateTransitions(testCase.PlayerSpecies);
                if (stateTransitions < testCase.Criteria.MinimumPlayerStateTransitions)
                {
                    failures.Add(
                        $"Expected at least {testCase.Criteria.MinimumPlayerStateTransitions} "
                        + $"state transitions for {testCase.PlayerSpecies}, got {stateTransitions}.");
                }

                if (finalPlayerPopulation == 0)
                {
                    finalExtinctions++;
                }

                runs.Add(new SimulationTestRunReport(
                    seed,
                    initialPlayerPopulation,
                    finalPlayerPopulation,
                    peakPlayerPopulation,
                    stateTransitions,
                    new ReadOnlyCollection<string>(failures)));
            }

            if (finalExtinctions > testCase.Criteria.MaximumAllowedFinalExtinctions)
            {
                aggregateFailures.Add(
                    $"Final extinction count {finalExtinctions} exceeds allowed "
                    + $"maximum {testCase.Criteria.MaximumAllowedFinalExtinctions}.");
            }

            return new SimulationTestReport(
                testCase,
                new ReadOnlyCollection<SimulationTestRunReport>(runs),
                new ReadOnlyCollection<string>(aggregateFailures));
        }
    }
}
