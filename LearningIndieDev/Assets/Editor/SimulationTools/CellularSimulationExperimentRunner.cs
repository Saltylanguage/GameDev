using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
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
        const int ReportSchemaVersion = 18;
        const int DefaultSeedStart = 1;
        const int DefaultSeedCount = 20;
        const string DefaultPlayerSpeciesId = "herbivore";
        const string ScenarioPathArgument = "-scenarioPath";
        const string SeedStartArgument = "-seedStart";
        const string SeedCountArgument = "-seedCount";
        const string PlayerSpeciesArgument = "-playerSpeciesId";
        const string UpgradeIdArgument = "-upgradeId";
        const string DefaultUpgradeId = "none";
        const string GridWidthArgument = "-gridWidth";
        const string GridHeightArgument = "-gridHeight";
        const string RunDurationArgument = "-runDurationSeconds";
        const string StepIntervalArgument = "-stepIntervalSeconds";
        const string OutputPathArgument = "-outputPath";
        const string CombatModeArgument = "-combatMode";
        const string DefaultCombatMode = "legacy-fixed-damage";
        const string AttackOpportunityModeArgument = "-attackOpportunityMode";
        const string DefaultAttackOpportunityMode = "natural";
        const string ExperimentalFeaturesArgument = "-experimentalFeatures";
        const string FoxAttackCooldownTicksArgument = "-foxAttackCooldownTicks";

        [MenuItem("Salty Game/Simulation/Run FSM Test Harness")]
        public static void RunFsmTestHarness()
        {
            var scenarioPath = "Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset";
            var scenario = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(scenarioPath);
            if (scenario == null)
            {
                throw new FileNotFoundException($"FSM test scenario was not found at '{scenarioPath}'.");
            }

            var playerSpecies = new SpeciesId("hare");
            var predatorSpecies = new SpeciesId("fox");
            var criteria = new SimulationTestCriteria(
                minimumFinalPlayerRatio: 1.2f,
                maximumFinalPlayerRatio: 1.5f,
                expectedInitialPlayerPopulation: 22,
                maximumAllowedFinalExtinctions: 0,
                minimumPlayerStateTransitions: 1,
                expectedInitialPopulations: new Dictionary<SpeciesId, int>
                {
                    [playerSpecies] = 22,
                    [predatorSpecies] = 4,
                });
            var testCase = new SimulationTestCase(
                "Forest Edge FSM balance",
                scenario.CreateRuntimeData(),
                playerSpecies,
                seedStart: 1001,
                seedCount: 20,
                criteria);
            var report = SimulationTestHarness.Run(testCase);
            var projectRoot = Directory.GetParent(Application.dataPath).FullName;
            var artifactsPath = Path.Combine(projectRoot, "artifacts");
            Directory.CreateDirectory(artifactsPath);
            var jsonPath = Path.Combine(artifactsPath, "fsm-test-report.json");
            var markdownPath = Path.Combine(artifactsPath, "fsm-test-report.md");
            File.WriteAllText(jsonPath, JsonUtility.ToJson(CreateHarnessReport(report), true), new UTF8Encoding(false));
            File.WriteAllText(markdownPath, CreateHarnessMarkdown(report), new UTF8Encoding(false));
            Debug.Log($"[Salty] FSM test harness completed: {report.PassedRuns}/{report.Runs.Count} runs passed. Report: {markdownPath}");
        }

        public static void RunFromCommandLine()
        {
            try
            {
                var options = CommandLineOptions.Parse(Environment.GetCommandLineArgs());
                var outputPath = GetRequiredOutputPath(options.OutputPath);
                var data = ApplyOverrides(
                    LoadSimulationData(options.ScenarioPath, out var temporaryAsset),
                    options);
                var experimentalOptions = GetExperimentalOptions(options);

                try
                {
                    var report = options.AttackOpportunityMode == SpeciesAttackOpportunityMode.PairedLockstepDiagnostic
                        ? CreatePairedReport(data, options, outputPath, experimentalOptions)
                        : CreateReport(ApplyLoadout(data, options), options, outputPath, experimentalOptions);
                    File.WriteAllText(
                        outputPath,
                        SerializeReport(
                            report,
                            experimentalOptions.UsesHerbivoreStatLine
                                && data.SpeciesRules[new SpeciesId(options.PlayerSpeciesId)].Role == SpeciesRole.Herbivore),
                        new UTF8Encoding(false));
                    WriteCsv(report, GetSortedSpecies(data.SpeciesRules), report.csvOutputPath);
                    Debug.Log($"[Salty] Wrote {options.SeedCount} seeded cellular simulation runs to {outputPath} and {report.csvOutputPath}");
                }
                finally
                {
                    if (temporaryAsset != null)
                    {
                        UnityEngine.Object.DestroyImmediate(temporaryAsset);
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                if (Application.isBatchMode)
                {
                    EditorApplication.Exit(1);
                    return;
                }

                throw;
            }

            if (Application.isBatchMode)
            {
                EditorApplication.Exit(0);
            }
        }

        static ExperimentReport CreateReport(
            CellularSimData data,
            CommandLineOptions options,
            string outputPath,
            SpeciesExperimentalOptions experimentalOptions)
        {
            var playerSpecies = new SpeciesId(options.PlayerSpeciesId);
            if (!data.SpeciesRules.ContainsKey(playerSpecies))
            {
                throw new ArgumentException(
                    $"Scenario does not define the requested player species '{options.PlayerSpeciesId}'.",
                    PlayerSpeciesArgument);
            }

            var species = GetSortedSpecies(data.SpeciesRules);
            var upgrade = GetOptionalUpgrade(options.UpgradeId);
            var orderedLoadout = upgrade == null ? new string[0] : new[] { upgrade.Id };
            var runs = new ExperimentRun[options.SeedCount];
            for (var index = 0; index < options.SeedCount; index++)
            {
                runs[index] = RunSimulation(
                    data,
                    playerSpecies,
                    options.SeedStart + index,
                    species,
                    options.CombatResolutionMode,
                    options.AttackOpportunityMode,
                    experimentalOptions);
            }

            return new ExperimentReport
            {
                schemaVersion = ReportSchemaVersion,
                createdUtc = DateTime.UtcNow.ToString("O"),
                scenarioAssetPath = options.ScenarioPath ?? string.Empty,
                outputPath = outputPath,
                csvOutputPath = Path.ChangeExtension(outputPath, ".csv"),
                rulesetFingerprint = data.Fingerprint,
                runProvenanceFingerprint = CellularSimDataFingerprint.CreateRun(
                    data.Fingerprint,
                    options.CombatResolutionMode,
                    options.AttackOpportunityMode,
                    experimentalOptions,
                    orderedLoadout),
                playerSpeciesId = playerSpecies.Value,
                upgradeId = options.UpgradeId,
                upgradeType = upgrade == null ? string.Empty : upgrade.Type.ToString(),
                upgradeValue = upgrade == null ? 0f : upgrade.Value,
                orderedLoadout = orderedLoadout,
                combatResolutionMode = options.CombatResolutionMode.ToString(),
                attackOpportunityMode = options.AttackOpportunityMode.ToString(),
                experimentalFeatures = experimentalOptions.FeatureId,
                foxAttackCooldownTicks = experimentalOptions.FoxAttackCooldownTicks,
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
            IReadOnlyList<SpeciesId> species,
            SpeciesCombatResolutionMode combatResolutionMode,
            SpeciesAttackOpportunityMode attackOpportunityMode,
            SpeciesExperimentalOptions experimentalOptions)
        {
            var initialGrid = SpeciesInitialGridFactory.Create(data, seed);
            var run = new SimulationRunState(initialGrid, playerSpecies, seed, data.RunDurationSeconds);
            var runner = new SpeciesSimulationRunner(
                run,
                data,
                combatResolutionMode,
                attackOpportunityMode,
                experimentalOptions);
            while (runner.AdvanceOneTick())
            {
            }

            return CreateExperimentRun(
                run,
                species,
                new ExperimentOpportunityControl
                {
                    scheduled = run.Metrics.ControlledOpportunityScheduled,
                    eligible = run.Metrics.ControlledOpportunityEligible,
                    unfulfilledNoTarget = run.Metrics.ControlledOpportunityUnfulfilledNoTarget,
                    unfulfilledInvalidated = run.Metrics.ControlledOpportunityUnfulfilledInvalidated,
                },
                playerSpecies,
                experimentalOptions.UsesHerbivoreStatLine
                    && data.SpeciesRules[playerSpecies].Role == SpeciesRole.Herbivore);
        }

        static ExperimentReport CreatePairedReport(
            CellularSimData data,
            CommandLineOptions options,
            string outputPath,
            SpeciesExperimentalOptions experimentalOptions)
        {
            if (!string.Equals(options.UpgradeId, DefaultUpgradeId, StringComparison.Ordinal)
                && !string.Equals(options.UpgradeId, "stronger-block-2", StringComparison.Ordinal))
            {
                throw new ArgumentException(
                    "Paired lockstep diagnostic mode requires upgradeId 'none' or 'stronger-block-2'.",
                    UpgradeIdArgument);
            }

            var baselineData = ApplyLoadout(data, options.PlayerSpeciesId, DefaultUpgradeId);
            var blockPlusTwoData = ApplyLoadout(data, options.PlayerSpeciesId, "stronger-block-2");
            var playerSpecies = new SpeciesId(options.PlayerSpeciesId);
            var species = GetSortedSpecies(data.SpeciesRules);
            var selectedRuns = new ExperimentRun[options.SeedCount];
            for (var index = 0; index < options.SeedCount; index++)
            {
                var paired = RunPairedSimulation(
                    baselineData,
                    blockPlusTwoData,
                    playerSpecies,
                    options.SeedStart + index,
                    species,
                    options.CombatResolutionMode,
                    experimentalOptions);
                selectedRuns[index] = string.Equals(options.UpgradeId, DefaultUpgradeId, StringComparison.Ordinal)
                    ? paired.Baseline
                    : paired.BlockPlusTwo;
            }

            var selectedData = string.Equals(options.UpgradeId, DefaultUpgradeId, StringComparison.Ordinal)
                ? baselineData
                : blockPlusTwoData;
            var orderedLoadout = options.UpgradeId == DefaultUpgradeId
                ? new string[0]
                : new[] { options.UpgradeId };
            return new ExperimentReport
            {
                schemaVersion = ReportSchemaVersion,
                createdUtc = DateTime.UtcNow.ToString("O"),
                scenarioAssetPath = options.ScenarioPath ?? string.Empty,
                outputPath = outputPath,
                csvOutputPath = Path.ChangeExtension(outputPath, ".csv"),
                rulesetFingerprint = selectedData.Fingerprint,
                runProvenanceFingerprint = CellularSimDataFingerprint.CreateRun(
                    selectedData.Fingerprint,
                    options.CombatResolutionMode,
                    options.AttackOpportunityMode,
                    experimentalOptions,
                    orderedLoadout),
                playerSpeciesId = playerSpecies.Value,
                upgradeId = options.UpgradeId,
                upgradeType = GetOptionalUpgrade(options.UpgradeId)?.Type.ToString() ?? string.Empty,
                upgradeValue = GetOptionalUpgrade(options.UpgradeId)?.Value ?? 0f,
                orderedLoadout = orderedLoadout,
                combatResolutionMode = options.CombatResolutionMode.ToString(),
                attackOpportunityMode = options.AttackOpportunityMode.ToString(),
                experimentalFeatures = experimentalOptions.FeatureId,
                foxAttackCooldownTicks = experimentalOptions.FoxAttackCooldownTicks,
                seedStart = options.SeedStart,
                seedCount = options.SeedCount,
                gridWidth = selectedData.Width,
                gridHeight = selectedData.Height,
                runDurationSeconds = selectedData.RunDurationSeconds,
                stepIntervalSeconds = selectedData.StepInterval,
                runs = selectedRuns,
                finalPopulationSummary = CreateFinalPopulationSummary(selectedRuns, species),
            };
        }

        static PairedExperimentRuns RunPairedSimulation(
            CellularSimData baselineData,
            CellularSimData blockPlusTwoData,
            SpeciesId playerSpecies,
            int seed,
            IReadOnlyList<SpeciesId> species,
            SpeciesCombatResolutionMode combatResolutionMode,
            SpeciesExperimentalOptions experimentalOptions)
        {
            var initialGrid = SpeciesInitialGridFactory.Create(baselineData, seed);
            var baselineRun = new SimulationRunState(initialGrid.Copy(), playerSpecies, seed, baselineData.RunDurationSeconds);
            var blockPlusTwoRun = new SimulationRunState(initialGrid.Copy(), playerSpecies, seed, blockPlusTwoData.RunDurationSeconds);
            var runner = new SpeciesPairedSimulationRunner(
                baselineRun,
                baselineData,
                blockPlusTwoRun,
                blockPlusTwoData,
                combatResolutionMode,
                experimentalOptions);
            while (runner.AdvanceOneTick())
            {
            }

            return new PairedExperimentRuns
            {
                Baseline = CreateExperimentRun(
                    baselineRun,
                    species,
                    CreatePairedOpportunityControl(runner.OpportunityControl),
                    playerSpecies,
                    experimentalOptions.UsesHerbivoreStatLine
                        && baselineData.SpeciesRules[playerSpecies].Role == SpeciesRole.Herbivore),
                BlockPlusTwo = CreateExperimentRun(
                    blockPlusTwoRun,
                    species,
                    CreatePairedOpportunityControl(runner.OpportunityControl),
                    playerSpecies,
                    experimentalOptions.UsesHerbivoreStatLine
                        && blockPlusTwoData.SpeciesRules[playerSpecies].Role == SpeciesRole.Herbivore),
            };
        }

        static ExperimentOpportunityControl CreatePairedOpportunityControl(
            SpeciesPairedOpportunityControl control)
        {
            return new ExperimentOpportunityControl
            {
                scheduled = control.Scheduled,
                baselineValid = control.BaselineValid,
                blockPlusTwoValid = control.BlockPlusTwoValid,
                commonValid = control.CommonValid,
                baselineOnly = control.BaselineOnly,
                blockPlusTwoOnly = control.BlockPlusTwoOnly,
                baselineCandidateCount = control.BaselineCandidateCount,
                blockPlusTwoCandidateCount = control.BlockPlusTwoCandidateCount,
                commonCandidateCount = control.CommonCandidateCount,
                unionCandidateCount = control.UnionCandidateCount,
                pairedAttempts = control.PairedAttempts,
                pairedMismatches = control.PairedMismatches,
                unfulfilledInvalidated = control.Invalidated,
                pairedOpportunityIds = new List<string>(control.PairedOpportunityIds).ToArray(),
                opportunityAudit = new List<SpeciesPairedOpportunityObservation>(control.OpportunityObservations).ToArray(),
            };
        }

        static ExperimentRun CreateExperimentRun(
            SimulationRunState run,
            IReadOnlyList<SpeciesId> species,
            ExperimentOpportunityControl opportunityControl,
            SpeciesId statSpecies,
            bool includeHerbivoreStatLine)
        {
            var result = SimulationRunResults.Create(run);
            return new ExperimentRun
            {
                seed = run.Seed,
                ticks = result.Ticks,
                durationSeconds = result.DurationSeconds,
                playerPopulation = result.PlayerPopulation,
                currencyEarned = result.CurrencyEarned,
                populationHistory = SimulationReportSerialization.CreatePopulationHistory(run.PopulationHistory, species),
                activity = SimulationReportSerialization.CreateActivity(run.Metrics, species),
                behavior = SimulationReportSerialization.CreateBehavior(run.Metrics, species),
                behaviorTransitions = SimulationReportSerialization.CreateBehaviorTransitions(run.Metrics),
                trackedBehavior = SimulationReportSerialization.CreateTrackedBehavior(run.Metrics, species),
                deathEvents = SimulationReportSerialization.CreateDeathEvents(run.Metrics),
                combatRolls = SimulationReportSerialization.CreateCombatRolls(run.Metrics),
                combatCooldownSuppressions = SimulationReportSerialization.CreateCombatCooldownSuppressions(run.Metrics),
                herbivoreStatLine = includeHerbivoreStatLine
                    ? SimulationReportSerialization.CreateHerbivoreStatLine(run, statSpecies)
                    : null,
                opportunityControl = opportunityControl,
            };
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

        static CellularSimData ApplyOverrides(CellularSimData data, CommandLineOptions options)
        {
            if (options.GridWidth == 0 && options.GridHeight == 0)
            {
                return options.RunDurationSeconds == 0f && options.StepIntervalSeconds == 0f
                    ? data
                    : data.WithRunWindow(
                        options.RunDurationSeconds == 0f ? data.RunDurationSeconds : options.RunDurationSeconds,
                        options.StepIntervalSeconds == 0f ? data.StepInterval : options.StepIntervalSeconds);
            }

            return new CellularSimData(
                options.GridWidth == 0 ? data.Width : options.GridWidth,
                options.GridHeight == 0 ? data.Height : options.GridHeight,
                data.StartingProbabilities,
                data.SpeciesRules,
                options.RunDurationSeconds == 0f ? data.RunDurationSeconds : options.RunDurationSeconds,
                options.StepIntervalSeconds == 0f ? data.StepInterval : options.StepIntervalSeconds,
                data.MaxPopulation,
                data.MinPopulation,
                data.TerrainDefinitions,
                data.AlphaOffspringRules,
                data.StartingPopulations);
        }

        static SpeciesExperimentalOptions GetExperimentalOptions(CommandLineOptions options)
        {
            if (!string.IsNullOrEmpty(options.ExperimentalFeatures)
                && !string.Equals(
                    options.ExperimentalFeatures,
                    SpeciesExperimentalOptions.BevExperimentalFeaturesId,
                    StringComparison.Ordinal))
            {
                throw new ArgumentException(
                    $"'{ExperimentalFeaturesArgument}' must be empty or '{SpeciesExperimentalOptions.BevExperimentalFeaturesId}'.",
                    ExperimentalFeaturesArgument);
            }

            var experimentalOptions = new SpeciesExperimentalOptions(
                options.ExperimentalFeatures,
                options.FoxAttackCooldownTicks);
            if (experimentalOptions.HasFoxAttackCooldown
                && options.CombatResolutionMode != SpeciesCombatResolutionMode.OpposedRoll)
            {
                throw new ArgumentException(
                    $"'{FoxAttackCooldownTicksArgument}' requires '{CombatModeArgument}' opposed-roll.",
                    FoxAttackCooldownTicksArgument);
            }

            if (experimentalOptions.HasFoxAttackCooldown
                && options.AttackOpportunityMode == SpeciesAttackOpportunityMode.PairedLockstepDiagnostic)
            {
                throw new ArgumentException(
                    $"'{FoxAttackCooldownTicksArgument}' is not supported by paired lockstep diagnostic mode.",
                    FoxAttackCooldownTicksArgument);
            }

            return experimentalOptions;
        }

        static CellularSimData ApplyLoadout(CellularSimData data, CommandLineOptions options)
        {
            return ApplyLoadout(data, options.PlayerSpeciesId, options.UpgradeId);
        }

        static CellularSimData ApplyLoadout(
            CellularSimData data,
            string playerSpeciesId,
            string upgradeId)
        {
            var upgrade = GetOptionalUpgrade(upgradeId);
            if (upgrade == null)
            {
                return data;
            }

            var playerSpecies = new SpeciesId(playerSpeciesId);
            if (!data.SpeciesRules.TryGetValue(playerSpecies, out var rules))
            {
                throw new ArgumentException(
                    $"Scenario does not define the requested player species '{playerSpeciesId}'.",
                    PlayerSpeciesArgument);
            }

            return data.WithSpeciesRules(playerSpecies, upgrade.Apply(rules));
        }

        static SpeciesUpgrade GetOptionalUpgrade(string upgradeId)
        {
            return string.IsNullOrWhiteSpace(upgradeId)
                || string.Equals(upgradeId, DefaultUpgradeId, StringComparison.Ordinal)
                ? null
                : SpeciesUpgradeCatalog.Create(upgradeId);
        }

        static void WriteCsv(
            ExperimentReport report,
            IReadOnlyList<SpeciesId> species,
            string outputPath)
        {
            var csv = new StringBuilder();
            var first = true;
            AppendCsvField(csv, "scenarioAssetPath", ref first);
            AppendCsvField(csv, "playerSpeciesId", ref first);
            AppendCsvField(csv, "seed", ref first);
            AppendCsvField(csv, "gridWidth", ref first);
            AppendCsvField(csv, "gridHeight", ref first);
            AppendCsvField(csv, "ticks", ref first);
            AppendCsvField(csv, "durationSeconds", ref first);
            AppendCsvField(csv, "playerPopulation", ref first);
            AppendCsvField(csv, "currencyEarned", ref first);
            for (var speciesIndex = 0; speciesIndex < species.Count; speciesIndex++)
            {
                AppendCsvField(csv, $"{species[speciesIndex].Value}_finalPopulation", ref first);
            }

            csv.AppendLine();
            for (var runIndex = 0; runIndex < report.runs.Length; runIndex++)
            {
                var run = report.runs[runIndex];
                first = true;
                AppendCsvField(csv, report.scenarioAssetPath, ref first);
                AppendCsvField(csv, report.playerSpeciesId, ref first);
                AppendCsvField(csv, run.seed, ref first);
                AppendCsvField(csv, report.gridWidth, ref first);
                AppendCsvField(csv, report.gridHeight, ref first);
                AppendCsvField(csv, run.ticks, ref first);
                AppendCsvField(csv, run.durationSeconds, ref first);
                AppendCsvField(csv, run.playerPopulation, ref first);
                AppendCsvField(csv, run.currencyEarned, ref first);
                for (var speciesIndex = 0; speciesIndex < species.Count; speciesIndex++)
                {
                    AppendCsvField(csv, GetFinalPopulation(run, speciesIndex), ref first);
                }

                csv.AppendLine();
            }

            File.WriteAllText(outputPath, csv.ToString(), new UTF8Encoding(true));
        }

        static FsmHarnessReport CreateHarnessReport(SimulationTestReport report)
        {
            var runs = new FsmHarnessRun[report.Runs.Count];
            for (var index = 0; index < runs.Length; index++)
            {
                var source = report.Runs[index];
                runs[index] = new FsmHarnessRun
                {
                    seed = source.Seed,
                    initialPlayerPopulation = source.InitialPlayerPopulation,
                    finalPlayerPopulation = source.FinalPlayerPopulation,
                    peakPlayerPopulation = source.PeakPlayerPopulation,
                    playerStateTransitions = source.PlayerStateTransitions,
                    passed = source.Passed,
                    failures = new List<string>(source.Failures).ToArray(),
                };
            }

            return new FsmHarnessReport
            {
                schemaVersion = 1,
                createdUtc = DateTime.UtcNow.ToString("O"),
                testName = report.TestCase.Name,
                rulesetFingerprint = report.TestCase.Data.Fingerprint,
                passed = report.Passed,
                passedRuns = report.PassedRuns,
                failedRuns = report.FailedRuns,
                aggregateFailures = new List<string>(report.Failures).ToArray(),
                runs = runs,
            };
        }

        static string CreateHarnessMarkdown(SimulationTestReport report)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"# {report.TestCase.Name}");
            builder.AppendLine();
            builder.AppendLine($"- Passed: `{report.Passed}`");
            builder.AppendLine($"- Runs: `{report.PassedRuns}/{report.Runs.Count}`");
            builder.AppendLine($"- Ruleset fingerprint: `{report.TestCase.Data.Fingerprint}`");
            builder.AppendLine();
            builder.AppendLine("| Seed | Initial | Final | Peak | State transitions | Result | Failure |");
            builder.AppendLine("|---:|---:|---:|---:|---:|---|---|");
            foreach (var run in report.Runs)
            {
                var failure = run.Failures.Count == 0
                    ? string.Empty
                    : string.Join("; ", run.Failures).Replace("|", "\\|");
                builder.AppendLine(
                    $"| {run.Seed} | {run.InitialPlayerPopulation} | {run.FinalPlayerPopulation} | "
                    + $"{run.PeakPlayerPopulation} | {run.PlayerStateTransitions} | {(run.Passed ? "PASS" : "FAIL")} | {failure} |");
            }

            if (report.Failures.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("## Aggregate failures");
                foreach (var failure in report.Failures)
                {
                    builder.AppendLine($"- {failure}");
                }
            }

            return builder.ToString();
        }

        static void AppendCsvField(StringBuilder csv, string value, ref bool first)
        {
            if (!first)
            {
                csv.Append(',');
            }

            csv.Append('"').Append((value ?? string.Empty).Replace("\"", "\"\""));
            csv.Append('"');
            first = false;
        }

        static void AppendCsvField(StringBuilder csv, int value, ref bool first)
        {
            AppendCsvField(csv, value.ToString(CultureInfo.InvariantCulture), ref first);
        }

        static void AppendCsvField(StringBuilder csv, float value, ref bool first)
        {
            AppendCsvField(csv, value.ToString(CultureInfo.InvariantCulture), ref first);
        }

        static CellularSimData LoadSimulationData(string scenarioPath, out UnityEngine.Object temporaryAsset)
        {
            temporaryAsset = null;
            if (!string.IsNullOrWhiteSpace(scenarioPath))
            {
                if (!scenarioPath.StartsWith("Assets/", StringComparison.Ordinal))
                {
                    throw new ArgumentException("Scenario paths must be Unity project paths beginning with 'Assets/'.", ScenarioPathArgument);
                }

                var scenario = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(scenarioPath);
                if (scenario != null)
                {
                    return scenario.CreateRuntimeData();
                }

                var asset = AssetDatabase.LoadAssetAtPath<CellularSimDataAsset>(scenarioPath);
                if (asset == null)
                {
                    throw new FileNotFoundException($"No supported simulation scenario exists at '{scenarioPath}'.", scenarioPath);
                }

                return asset.CreateRuntimeData();
            }

            temporaryAsset = ScriptableObject.CreateInstance<CellularSimDataAsset>();
            return ((CellularSimDataAsset)temporaryAsset).CreateRuntimeData();
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
            public string UpgradeId { get; private set; }
            public SpeciesCombatResolutionMode CombatResolutionMode { get; private set; }
            public SpeciesAttackOpportunityMode AttackOpportunityMode { get; private set; }
            public string ExperimentalFeatures { get; private set; }
            public int FoxAttackCooldownTicks { get; private set; }
            public int GridWidth { get; private set; }
            public int GridHeight { get; private set; }
            public float RunDurationSeconds { get; private set; }
            public float StepIntervalSeconds { get; private set; }
            public string OutputPath { get; private set; }

            public static CommandLineOptions Parse(string[] arguments)
            {
                return new CommandLineOptions
                {
                    ScenarioPath = GetOptionalValue(arguments, ScenarioPathArgument),
                    SeedStart = GetIntValue(arguments, SeedStartArgument, DefaultSeedStart, allowZero: true, allowSigned: true),
                    SeedCount = GetIntValue(arguments, SeedCountArgument, DefaultSeedCount, allowZero: false),
                    PlayerSpeciesId = GetOptionalValue(arguments, PlayerSpeciesArgument) ?? DefaultPlayerSpeciesId,
                    UpgradeId = GetOptionalValue(arguments, UpgradeIdArgument) ?? DefaultUpgradeId,
                    CombatResolutionMode = ParseCombatResolutionMode(
                        GetOptionalValue(arguments, CombatModeArgument) ?? DefaultCombatMode),
                    AttackOpportunityMode = ParseAttackOpportunityMode(
                        GetOptionalValue(arguments, AttackOpportunityModeArgument) ?? DefaultAttackOpportunityMode),
                    ExperimentalFeatures = GetOptionalValue(arguments, ExperimentalFeaturesArgument) ?? string.Empty,
                    FoxAttackCooldownTicks = GetIntValue(
                        arguments,
                        FoxAttackCooldownTicksArgument,
                        0,
                        allowZero: true),
                    GridWidth = GetIntValue(arguments, GridWidthArgument, 0, allowZero: true),
                    GridHeight = GetIntValue(arguments, GridHeightArgument, 0, allowZero: true),
                    RunDurationSeconds = GetFloatValue(arguments, RunDurationArgument),
                    StepIntervalSeconds = GetFloatValue(arguments, StepIntervalArgument),
                    OutputPath = GetOptionalValue(arguments, OutputPathArgument),
                };
            }

            static SpeciesCombatResolutionMode ParseCombatResolutionMode(string value)
            {
                if (string.Equals(value, "legacy-fixed-damage", StringComparison.OrdinalIgnoreCase))
                {
                    return SpeciesCombatResolutionMode.LegacyFixedDamage;
                }

                if (string.Equals(value, "opposed-roll", StringComparison.OrdinalIgnoreCase))
                {
                    return SpeciesCombatResolutionMode.OpposedRoll;
                }

                throw new ArgumentException(
                    $"'{CombatModeArgument}' must be 'legacy-fixed-damage' or 'opposed-roll'.",
                    CombatModeArgument);
            }

            static SpeciesAttackOpportunityMode ParseAttackOpportunityMode(string value)
            {
                if (string.Equals(value, "natural", StringComparison.OrdinalIgnoreCase))
                {
                    return SpeciesAttackOpportunityMode.Natural;
                }

                if (string.Equals(value, "fixed-rate-diagnostic", StringComparison.OrdinalIgnoreCase))
                {
                    return SpeciesAttackOpportunityMode.FixedRateDiagnostic;
                }

                if (string.Equals(value, "paired-lockstep-diagnostic", StringComparison.OrdinalIgnoreCase))
                {
                    return SpeciesAttackOpportunityMode.PairedLockstepDiagnostic;
                }

                throw new ArgumentException(
                    $"'{AttackOpportunityModeArgument}' must be 'natural', 'fixed-rate-diagnostic', or 'paired-lockstep-diagnostic'.",
                    AttackOpportunityModeArgument);
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

            static int GetIntValue(
                IReadOnlyList<string> arguments,
                string name,
                int defaultValue,
                bool allowZero,
                bool allowSigned = false)
            {
                var value = GetOptionalValue(arguments, name);
                if (string.IsNullOrWhiteSpace(value))
                {
                    return defaultValue;
                }

                if (!int.TryParse(value, out var parsed)
                    || (!allowSigned && parsed < 0)
                    || (!allowZero && parsed == 0)
                    || (!allowSigned && parsed > 1000000))
                {
                    throw new ArgumentException(
                        allowSigned
                            ? $"'{name}' must be a signed 32-bit integer{(allowZero ? string.Empty : " other than zero")}."
                            : $"'{name}' must be an integer between {(allowZero ? 0 : 1)} and 1000000.",
                        name);
                }

                return parsed;
            }

            static float GetFloatValue(IReadOnlyList<string> arguments, string name)
            {
                var value = GetOptionalValue(arguments, name);
                if (string.IsNullOrWhiteSpace(value))
                {
                    return 0f;
                }

                if (!float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)
                    || float.IsNaN(parsed)
                    || float.IsInfinity(parsed)
                    || parsed <= 0f
                    || parsed > 1000000f)
                {
                    throw new ArgumentException(
                        $"'{name}' must be a finite number greater than zero and no greater than 1000000.",
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
            public string csvOutputPath;
            public string rulesetFingerprint;
            public string runProvenanceFingerprint;
            public string playerSpeciesId;
            public string upgradeId;
            public string upgradeType;
            public float upgradeValue;
            public string[] orderedLoadout;
            public string combatResolutionMode;
            public string attackOpportunityMode;
            public string experimentalFeatures;
            public int foxAttackCooldownTicks;
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
            public SimulationPopulationSnapshotRecord[] populationHistory;
            public SimulationSpeciesActivityRecord[] activity;
            public SimulationSpeciesBehaviorRecord[] behavior;
            public SimulationSpeciesBehaviorTransitionRecord[] behaviorTransitions;
            public SimulationSpeciesTrackedBehaviorRecord[] trackedBehavior;
            public SimulationSpeciesDeathRecord[] deathEvents;
            public SimulationSpeciesCombatRollRecord[] combatRolls;
            public SimulationSpeciesCombatCooldownSuppressionRecord[] combatCooldownSuppressions;
            public SimulationHerbivoreStatLineRecord herbivoreStatLine;
            public ExperimentOpportunityControl opportunityControl;
        }

        static string SerializeReport(ExperimentReport report, bool includeHerbivoreStatLine)
        {
            var json = JsonUtility.ToJson(report, true);
            return includeHerbivoreStatLine
                ? json
                : Regex.Replace(json, @"\s*""herbivoreStatLine"":\s*\{[^{}]*\},?", string.Empty);
        }

        [Serializable]
        sealed class ExperimentOpportunityControl
        {
            public int scheduled;
            public int eligible;
            public int unfulfilledNoTarget;
            public int unfulfilledInvalidated;
            public int baselineValid;
            public int blockPlusTwoValid;
            public int commonValid;
            public int baselineOnly;
            public int blockPlusTwoOnly;
            public int baselineCandidateCount;
            public int blockPlusTwoCandidateCount;
            public int commonCandidateCount;
            public int unionCandidateCount;
            public int pairedAttempts;
            public int pairedMismatches;
            public string[] pairedOpportunityIds;
            public SpeciesPairedOpportunityObservation[] opportunityAudit;
        }

        sealed class PairedExperimentRuns
        {
            public ExperimentRun Baseline;
            public ExperimentRun BlockPlusTwo;
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

        [Serializable]
        sealed class FsmHarnessReport
        {
            public int schemaVersion;
            public string createdUtc;
            public string testName;
            public string rulesetFingerprint;
            public bool passed;
            public int passedRuns;
            public int failedRuns;
            public string[] aggregateFailures;
            public FsmHarnessRun[] runs;
        }

        [Serializable]
        sealed class FsmHarnessRun
        {
            public int seed;
            public int initialPlayerPopulation;
            public int finalPlayerPopulation;
            public int peakPlayerPopulation;
            public int playerStateTransitions;
            public bool passed;
            public string[] failures;
        }
    }
}
