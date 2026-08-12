using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SaltyGame;
using UnityEditor;
using UnityEngine;

namespace SaltyGame.EditorTools
{
    /// <summary>
    /// Produces repeatable, machine-readable simulation runs from a scenario asset or the current defaults.
    /// Intended for Unity batch mode through tools/Run-CellularExperiment.ps1.
    /// </summary>
    public static class CellularSimulationExperimentRunner
    {
        const int ReportSchemaVersion = 1;
        const int DefaultSeedStart = 1;
        const int DefaultSeedCount = 20;
        const string DefaultPlayerSpeciesId = "herbivore";
        const string ScenarioPathArgument = "-scenarioPath";
        const string SeedStartArgument = "-seedStart";
        const string SeedCountArgument = "-seedCount";
        const string PlayerSpeciesArgument = "-playerSpeciesId";
        const string OutputPathArgument = "-outputPath";

        public static void RunFromCommandLine()
        {
            var options = CommandLineOptions.Parse(Environment.GetCommandLineArgs());
            var outputPath = GetRequiredOutputPath(options.OutputPath);
            var data = LoadSimulationData(options.ScenarioPath, out var temporaryAsset);

            try
            {
                var report = CreateReport(data, options, outputPath);
                File.WriteAllText(outputPath, JsonUtility.ToJson(report, true), new UTF8Encoding(false));
                Debug.Log($"[Salty] Wrote {options.SeedCount} seeded cellular simulation runs to {outputPath}");
            }
            finally
            {
                if (temporaryAsset != null)
                {
                    UnityEngine.Object.DestroyImmediate(temporaryAsset);
                }
            }
        }

        static ExperimentReport CreateReport(CellularSimData data, CommandLineOptions options, string outputPath)
        {
            var playerSpecies = new SpeciesId(options.PlayerSpeciesId);
            if (!data.SpeciesRules.ContainsKey(playerSpecies))
            {
                throw new ArgumentException(
                    $"Scenario does not define the requested player species '{options.PlayerSpeciesId}'.",
                    PlayerSpeciesArgument);
            }

            var species = GetSortedSpecies(data.SpeciesRules);
            var runs = new ExperimentRun[options.SeedCount];
            for (var index = 0; index < options.SeedCount; index++)
            {
                runs[index] = RunSimulation(data, playerSpecies, options.SeedStart + index, species);
            }

            return new ExperimentReport
            {
                schemaVersion = ReportSchemaVersion,
                createdUtc = DateTime.UtcNow.ToString("O"),
                scenarioAssetPath = options.ScenarioPath ?? string.Empty,
                outputPath = outputPath,
                rulesetFingerprint = data.Fingerprint,
                playerSpeciesId = playerSpecies.Value,
                seedStart = options.SeedStart,
                seedCount = options.SeedCount,
                gridWidth = data.Width,
                gridHeight = data.Height,
                runDurationSeconds = data.RunDurationSeconds,
                stepIntervalSeconds = data.StepInterval,
                runs = runs,
                finalPopulationSummary = CreateFinalPopulationSummary(runs, species),
            };
        }

        static ExperimentRun RunSimulation(
            CellularSimData data,
            SpeciesId playerSpecies,
            int seed,
            IReadOnlyList<SpeciesId> species)
        {
            var initialGrid = SpeciesInitialGridFactory.Create(data, seed);
            var run = new SimulationRunState(initialGrid, playerSpecies, seed, data.RunDurationSeconds);
            var runner = new SpeciesSimulationRunner(run, data);
            while (runner.AdvanceOneTick())
            {
            }

            var result = SimulationRunResults.Create(run);
            return new ExperimentRun
            {
                seed = seed,
                ticks = result.Ticks,
                durationSeconds = result.DurationSeconds,
                playerPopulation = result.PlayerPopulation,
                currencyEarned = result.CurrencyEarned,
                populationHistory = CreatePopulationHistory(run.PopulationHistory, species),
            };
        }

        static ExperimentPopulationSnapshot[] CreatePopulationHistory(
            IReadOnlyList<SpeciesPopulationSnapshot> populationHistory,
            IReadOnlyList<SpeciesId> species)
        {
            var snapshots = new ExperimentPopulationSnapshot[populationHistory.Count];
            for (var index = 0; index < snapshots.Length; index++)
            {
                var source = populationHistory[index];
                var counts = new ExperimentSpeciesPopulation[species.Count];
                for (var speciesIndex = 0; speciesIndex < species.Count; speciesIndex++)
                {
                    var speciesId = species[speciesIndex];
                    counts[speciesIndex] = new ExperimentSpeciesPopulation
                    {
                        speciesId = speciesId.Value,
                        population = source.GetCount(speciesId),
                    };
                }

                snapshots[index] = new ExperimentPopulationSnapshot
                {
                    tick = source.Tick,
                    empty = source.Empty,
                    species = counts,
                };
            }

            return snapshots;
        }

        static ExperimentPopulationSummary[] CreateFinalPopulationSummary(
            IReadOnlyList<ExperimentRun> runs,
            IReadOnlyList<SpeciesId> species)
        {
            var summary = new ExperimentPopulationSummary[species.Count];
            for (var speciesIndex = 0; speciesIndex < species.Count; speciesIndex++)
            {
                var minimum = int.MaxValue;
                var maximum = int.MinValue;
                var total = 0;
                var extinctRuns = 0;
                for (var runIndex = 0; runIndex < runs.Count; runIndex++)
                {
                    var finalPopulation = GetFinalPopulation(runs[runIndex], speciesIndex);
                    minimum = Math.Min(minimum, finalPopulation);
                    maximum = Math.Max(maximum, finalPopulation);
                    total += finalPopulation;
                    if (finalPopulation == 0)
                    {
                        extinctRuns++;
                    }
                }

                summary[speciesIndex] = new ExperimentPopulationSummary
                {
                    speciesId = species[speciesIndex].Value,
                    minimumFinalPopulation = minimum,
                    maximumFinalPopulation = maximum,
                    averageFinalPopulation = runs.Count == 0 ? 0f : (float)total / runs.Count,
                    extinctFinalRuns = extinctRuns,
                    finalExtinctionRate = runs.Count == 0 ? 0f : (float)extinctRuns / runs.Count,
                };
            }

            return summary;
        }

        static int GetFinalPopulation(ExperimentRun run, int speciesIndex)
        {
            var history = run.populationHistory;
            if (history == null || history.Length == 0)
            {
                return 0;
            }

            var species = history[history.Length - 1].species;
            return species != null && speciesIndex < species.Length ? species[speciesIndex].population : 0;
        }

        static CellularSimData LoadSimulationData(string scenarioPath, out CellularSimDataAsset temporaryAsset)
        {
            temporaryAsset = null;
            if (!string.IsNullOrWhiteSpace(scenarioPath))
            {
                if (!scenarioPath.StartsWith("Assets/", StringComparison.Ordinal))
                {
                    throw new ArgumentException("Scenario paths must be Unity project paths beginning with 'Assets/'.", ScenarioPathArgument);
                }

                var asset = AssetDatabase.LoadAssetAtPath<CellularSimDataAsset>(scenarioPath);
                if (asset == null)
                {
                    throw new FileNotFoundException($"No CellularSimDataAsset exists at '{scenarioPath}'.", scenarioPath);
                }

                return asset.CreateRuntimeData();
            }

            temporaryAsset = ScriptableObject.CreateInstance<CellularSimDataAsset>();
            return temporaryAsset.CreateRuntimeData();
        }

        static string GetRequiredOutputPath(string outputPath)
        {
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                throw new ArgumentException("An output path is required.", OutputPathArgument);
            }

            var projectRoot = Directory.GetParent(Application.dataPath).FullName;
            var artifactsRoot = Path.GetFullPath(Path.Combine(projectRoot, "artifacts"));
            var normalizedOutputPath = Path.GetFullPath(outputPath);
            var artifactsPrefix = artifactsRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;
            if (!normalizedOutputPath.StartsWith(artifactsPrefix, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    $"Output path must stay under '{artifactsRoot}'.",
                    OutputPathArgument);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(normalizedOutputPath));
            return normalizedOutputPath;
        }

        static List<SpeciesId> GetSortedSpecies(IReadOnlyDictionary<SpeciesId, SpeciesRules> definitions)
        {
            var species = new List<SpeciesId>(definitions.Keys);
            species.Sort((left, right) => string.CompareOrdinal(left.Value, right.Value));
            return species;
        }

        sealed class CommandLineOptions
        {
            public string ScenarioPath { get; private set; }
            public int SeedStart { get; private set; }
            public int SeedCount { get; private set; }
            public string PlayerSpeciesId { get; private set; }
            public string OutputPath { get; private set; }

            public static CommandLineOptions Parse(string[] arguments)
            {
                return new CommandLineOptions
                {
                    ScenarioPath = GetOptionalValue(arguments, ScenarioPathArgument),
                    SeedStart = GetIntValue(arguments, SeedStartArgument, DefaultSeedStart, allowZero: true),
                    SeedCount = GetIntValue(arguments, SeedCountArgument, DefaultSeedCount, allowZero: false),
                    PlayerSpeciesId = GetOptionalValue(arguments, PlayerSpeciesArgument) ?? DefaultPlayerSpeciesId,
                    OutputPath = GetOptionalValue(arguments, OutputPathArgument),
                };
            }

            static string GetOptionalValue(IReadOnlyList<string> arguments, string name)
            {
                for (var index = 0; index < arguments.Count - 1; index++)
                {
                    if (string.Equals(arguments[index], name, StringComparison.Ordinal))
                    {
                        return arguments[index + 1];
                    }
                }

                return null;
            }

            static int GetIntValue(IReadOnlyList<string> arguments, string name, int defaultValue, bool allowZero)
            {
                var value = GetOptionalValue(arguments, name);
                if (string.IsNullOrWhiteSpace(value))
                {
                    return defaultValue;
                }

                if (!int.TryParse(value, out var parsed)
                    || parsed < 0
                    || (!allowZero && parsed == 0)
                    || parsed > 10000)
                {
                    throw new ArgumentException(
                        $"'{name}' must be an integer between {(allowZero ? 0 : 1)} and 10000.",
                        name);
                }

                return parsed;
            }
        }

        [Serializable]
        sealed class ExperimentReport
        {
            public int schemaVersion;
            public string createdUtc;
            public string scenarioAssetPath;
            public string outputPath;
            public string rulesetFingerprint;
            public string playerSpeciesId;
            public int seedStart;
            public int seedCount;
            public int gridWidth;
            public int gridHeight;
            public float runDurationSeconds;
            public float stepIntervalSeconds;
            public ExperimentRun[] runs;
            public ExperimentPopulationSummary[] finalPopulationSummary;
        }

        [Serializable]
        sealed class ExperimentRun
        {
            public int seed;
            public int ticks;
            public float durationSeconds;
            public int playerPopulation;
            public int currencyEarned;
            public ExperimentPopulationSnapshot[] populationHistory;
        }

        [Serializable]
        sealed class ExperimentPopulationSnapshot
        {
            public int tick;
            public int empty;
            public ExperimentSpeciesPopulation[] species;
        }

        [Serializable]
        sealed class ExperimentSpeciesPopulation
        {
            public string speciesId;
            public int population;
        }

        [Serializable]
        sealed class ExperimentPopulationSummary
        {
            public string speciesId;
            public int minimumFinalPopulation;
            public int maximumFinalPopulation;
            public float averageFinalPopulation;
            public int extinctFinalRuns;
            public float finalExtinctionRate;
        }
    }
}
