using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SaltyGame;
using UnityEditor;
using UnityEngine;

namespace SaltyGame.EditorTools
{
    /// <summary>Persists the most recent completed preview run for human and agent analysis.</summary>
    [InitializeOnLoad]
    public static class PlayModeSimulationResultLogger
    {
        const int ReportSchemaVersion = 4;
        const string JsonFileName = "playmode-last-run.json";
        const string MarkdownFileName = "playmode-last-run.md";

        static PlayModeSimulationResultLogger()
        {
            SpeciesSimulationPreview.RunCompleted += Save;
        }

        static void Save(SpeciesSimulationPreview preview, SimulationRunState run)
        {
            try
            {
                var projectRoot = Directory.GetParent(Application.dataPath).FullName;
                var artifactsDirectory = Path.Combine(projectRoot, "artifacts");
                Directory.CreateDirectory(artifactsDirectory);

                var report = CreateReport(preview, run);
                var jsonPath = Path.Combine(artifactsDirectory, JsonFileName);
                var markdownPath = Path.Combine(artifactsDirectory, MarkdownFileName);
                File.WriteAllText(jsonPath, JsonUtility.ToJson(report, true), new UTF8Encoding(false));
                File.WriteAllText(markdownPath, CreateMarkdown(report), new UTF8Encoding(false));
                Debug.Log($"[Salty] Saved last Play Mode simulation to {jsonPath}");
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        static PlayModeRunReport CreateReport(SpeciesSimulationPreview preview, SimulationRunState run)
        {
            var species = SimulationReportSerialization.GetSpecies(run.PopulationHistory);

            return new PlayModeRunReport
            {
                schemaVersion = ReportSchemaVersion,
                createdUtc = DateTime.UtcNow.ToString("O"),
                scenarioAssetPath = preview.SelectedScenario == null
                    ? string.Empty
                    : AssetDatabase.GetAssetPath(preview.SelectedScenario),
                scenarioName = preview.SelectedScenario == null
                    ? "Runtime Defaults"
                    : preview.SelectedScenario.name,
                playerSpeciesId = run.PlayerSpeciesId.Value,
                seed = run.Seed,
                ticks = run.Tick,
                durationSeconds = run.ElapsedSeconds,
                gridWidth = run.Cells.Width,
                gridHeight = run.Cells.Height,
                rulesetFingerprint = run.RulesetFingerprint,
                finalPlayerPopulation = SimulationRunResults.Create(run).PlayerPopulation,
                populationHistory = SimulationReportSerialization.CreatePopulationHistory(run.PopulationHistory, species),
                activity = SimulationReportSerialization.CreateActivity(run.Metrics, species),
                behavior = SimulationReportSerialization.CreateBehavior(run.Metrics, species),
                behaviorTransitions = SimulationReportSerialization.CreateBehaviorTransitions(run.Metrics),
                trackedBehavior = SimulationReportSerialization.CreateTrackedBehavior(run.Metrics, species),
                deathEvents = SimulationReportSerialization.CreateDeathEvents(run.Metrics),
            };
        }

        static string CreateMarkdown(PlayModeRunReport report)
        {
            var builder = new StringBuilder();
            builder.AppendLine("# Last Play Mode Simulation");
            builder.AppendLine();
            builder.AppendLine($"- Scenario: `{report.scenarioName}`");
            builder.AppendLine($"- Seed: `{report.seed}`");
            builder.AppendLine($"- Grid: `{report.gridWidth} x {report.gridHeight}`");
            builder.AppendLine($"- Duration: `{report.durationSeconds:0.###}s` / `{report.ticks}` ticks");
            builder.AppendLine($"- Player species: `{report.playerSpeciesId}`");
            builder.AppendLine($"- Final player population: `{report.finalPlayerPopulation}`");
            builder.AppendLine($"- Ruleset fingerprint: `{report.rulesetFingerprint}`");
            builder.AppendLine();
            builder.AppendLine("## Final populations");
            builder.AppendLine();
            builder.AppendLine("| Species | Final population |");
            builder.AppendLine("|---|---:|");
            var finalSnapshot = report.populationHistory[report.populationHistory.Length - 1];
            for (var index = 0; index < finalSnapshot.species.Length; index++)
            {
                var entry = finalSnapshot.species[index];
                builder.AppendLine($"| {entry.speciesId} | {entry.population} |");
            }

            builder.AppendLine();
            builder.AppendLine("## Activity");
            builder.AppendLine();
            builder.AppendLine("| Species | Births | Food | Movement | Kills | Deaths | Starvation | State transitions |");
            builder.AppendLine("|---|---:|---:|---:|---:|---:|---:|---:|");
            for (var index = 0; index < report.activity.Length; index++)
            {
                var entry = report.activity[index];
                builder.AppendLine(
                    $"| {entry.speciesId} | {entry.births} | {entry.foodConsumed:0.###} | "
                    + $"{entry.movementSteps} | {entry.combatKills} | {entry.deaths} | {entry.starvationDeaths} | "
                    + $"{entry.stateTransitions} |");
            }

            builder.AppendLine();
            builder.AppendLine("## Behavior states");
            builder.AppendLine();
            builder.AppendLine("| Species | State | Ticks |");
            builder.AppendLine("|---|---|---:|");
            for (var index = 0; index < report.behavior.Length; index++)
            {
                var entry = report.behavior[index];
                if (entry.ticks == 0)
                {
                    continue;
                }

                builder.AppendLine($"| {entry.speciesId} | {entry.state} | {entry.ticks} |");
            }

            builder.AppendLine();
            builder.AppendLine("## Tracked FSM transitions");
            builder.AppendLine();
            builder.AppendLine("| Species | Entity | Age | Position | Previous | Current |");
            builder.AppendLine("|---|---:|---:|---|---|---|");
            for (var index = 0; index < report.behaviorTransitions.Length; index++)
            {
                var entry = report.behaviorTransitions[index];
                builder.AppendLine(
                    $"| {entry.speciesId} | {entry.entityId} | {entry.age} | ({entry.x},{entry.y}) | "
                    + $"{entry.previousState} | {entry.currentState} |");
            }

            builder.AppendLine();
            builder.AppendLine("## Tracked FSM entities");
            builder.AppendLine();
            builder.AppendLine("| Species | Entity | Age | Position | State | State ticks |");
            builder.AppendLine("|---|---:|---:|---|---|---:|");
            for (var index = 0; index < report.trackedBehavior.Length; index++)
            {
                var entry = report.trackedBehavior[index];
                builder.AppendLine(
                    $"| {entry.speciesId} | {entry.entityId} | {entry.age} | ({entry.x},{entry.y}) | "
                    + $"{entry.state} | {entry.stateTicks} |");
            }

            builder.AppendLine();
            builder.AppendLine("## Death events");
            builder.AppendLine();
            builder.AppendLine("| Tick | Species | Entity | Creature | Age | Position | Cause |");
            builder.AppendLine("|---:|---|---:|---|---:|---|---|");
            for (var index = 0; index < report.deathEvents.Length; index++)
            {
                var entry = report.deathEvents[index];
                builder.AppendLine(
                    $"| {entry.tick} | {entry.speciesId} | {entry.entityId} | {entry.isCreature} | {entry.age} | "
                    + $"({entry.x},{entry.y}) | {entry.cause} |");
            }

            builder.AppendLine();
            builder.AppendLine("The JSON file beside this report contains the full per-tick population history, tracked entity transitions, and death events.");
            return builder.ToString();
        }

        [Serializable]
        sealed class PlayModeRunReport
        {
            public int schemaVersion;
            public string createdUtc;
            public string scenarioAssetPath;
            public string scenarioName;
            public string playerSpeciesId;
            public int seed;
            public int ticks;
            public float durationSeconds;
            public int gridWidth;
            public int gridHeight;
            public string rulesetFingerprint;
            public int finalPlayerPopulation;
            public SimulationPopulationSnapshotRecord[] populationHistory;
            public SimulationSpeciesActivityRecord[] activity;
            public SimulationSpeciesBehaviorRecord[] behavior;
            public SimulationSpeciesBehaviorTransitionRecord[] behaviorTransitions;
            public SimulationSpeciesTrackedBehaviorRecord[] trackedBehavior;
            public SimulationSpeciesDeathRecord[] deathEvents;
        }
    }
}
