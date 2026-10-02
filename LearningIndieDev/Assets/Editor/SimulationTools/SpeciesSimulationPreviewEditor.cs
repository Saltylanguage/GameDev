using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace SaltyGame.EditorTools
{
    [CustomEditor(typeof(SaltyGame.SpeciesSimulationPreview))]
    public sealed class SpeciesSimulationPreviewEditor : Editor
    {
        SerializedProperty scenarioOptions;
        SerializedProperty selectedScenarioIndex;
        int fixtureSeed = 10100;
        string runtimeMessage;

        void OnEnable()
        {
            scenarioOptions = serializedObject.FindProperty("scenarioOptions");
            selectedScenarioIndex = serializedObject.FindProperty("selectedScenarioIndex");
        }

        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                DrawRuntimeControls((SpeciesSimulationPreview)target);
                return;
            }

            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "scenarioOptions", "selectedScenarioIndex");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Authored Scenario", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(scenarioOptions, new GUIContent("Available Scenarios"), true);

            var options = new string[scenarioOptions.arraySize + 1];
            options[0] = "Legacy Defaults";
            for (var index = 0; index < scenarioOptions.arraySize; index++)
            {
                var reference = scenarioOptions.GetArrayElementAtIndex(index).objectReferenceValue;
                options[index + 1] = reference == null ? $"<Missing Scenario {index + 1}>" : reference.name;
            }

            var popupValue = Mathf.Clamp(selectedScenarioIndex.intValue + 1, 0, options.Length - 1);
            var selectedValue = EditorGUILayout.Popup("Selected Scenario", popupValue, options);
            selectedScenarioIndex.intValue = selectedValue - 1;
            if (selectedScenarioIndex.intValue >= 0
                && selectedScenarioIndex.intValue < scenarioOptions.arraySize
                && scenarioOptions.GetArrayElementAtIndex(selectedScenarioIndex.intValue).objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Assign a ScenarioDefinitionAsset to the selected list slot.", MessageType.Warning);
            }

            serializedObject.ApplyModifiedProperties();
        }

        void DrawRuntimeControls(SpeciesSimulationPreview preview)
        {
            EditorGUILayout.LabelField("Manual Simulation Testing", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("S4 developer fixture: Forest Edge, Hare, 36x20, 400/20/10, 0.1-second steps, six 100-tick phases. Coupled responses off. Does not save a preset or change scenario assets.", MessageType.Info);
            using (new EditorGUI.DisabledScope(!preview.SettingsEditable))
            {
                fixtureSeed = EditorGUILayout.IntField("Fixture Seed", fixtureSeed);
                if (GUILayout.Button("Apply S4 Fixture"))
                {
                    TryApplyS4Fixture(preview, fixtureSeed, out runtimeMessage);
                }
            }

            var run = preview.Run;
            EditorGUILayout.LabelField("Scenario / Species", $"{preview.SelectedScenario?.name ?? "Legacy defaults"} / {preview.PlayerSpecies.Value}");
            EditorGUILayout.LabelField("Seed / Randomization", $"{run?.Seed ?? preview.BaseSeed} / {preview.RandomizeSeedOnStart}");
            EditorGUILayout.LabelField("Grid / Step", $"{preview.GridWidth}x{preview.GridHeight} / {preview.StepInterval.ToString("0.###", CultureInfo.InvariantCulture)} seconds");
            EditorGUILayout.LabelField("Phase Length / Total Phases", $"{preview.PhaseLengthTicks} / {preview.ContinuousPhaseCount}");
            EditorGUILayout.LabelField("Experimental / Coupled Responses", $"{preview.BevExperimentalFeaturesEnabled} / {preview.CoupledSpeciesResponsesEnabled}");
            EditorGUILayout.LabelField("State / Tick", $"{preview.State} / {run?.Tick ?? 0}");
            if (run != null && run.PopulationHistory.Count > 0)
            {
                var initial = run.PopulationHistory[0];
                EditorGUILayout.LabelField("Actual Tick-Zero Plants / Hares / Foxes", $"{initial.GetCount(new SpeciesId("plant"))} / {initial.GetCount(new SpeciesId("hare"))} / {initial.GetCount(new SpeciesId("fox"))}");
                EditorGUILayout.LabelField("Rules Fingerprint", run.RulesetFingerprint);
                if (preview.ActiveSpeciesRules.TryGetValue(preview.PlayerSpecies, out var playerRules))
                {
                    EditorGUILayout.LabelField("Extra Crowding Cost Remaining", $"{((1f - playerRules.CrowdingEnergyReduction) * 100f).ToString("0.#", CultureInfo.InvariantCulture)}% of original surcharge");
                    var activity = run.Metrics.GetActivity(preview.PlayerSpecies);
                    EditorGUILayout.LabelField("Crowded Animal-Ticks / Energy Lost", $"{activity.CrowdingMetabolismTicks} / {activity.CrowdingEnergyLost}");
                    EditorGUILayout.LabelField("Planting Chance Per Eligible Tick", $"{(playerRules.SeedDropChance * 100f).ToString("0.#", CultureInfo.InvariantCulture)}%");
                    EditorGUILayout.LabelField("Planting Attempts / Plants Created", $"{activity.SeedDropAttempts} / {activity.SeedDropSuccesses}");
                    EditorGUILayout.LabelField("Plant Food Created / Stored Food Spent", $"{activity.SeedDropFoodCreated.ToString("0.###", CultureInfo.InvariantCulture)} / {activity.SeedDropReserveSpent}");
                }
                for (var index = 0; index < run.UpgradeAcquisitionTimeline.Count; index++)
                {
                    var acquisition = run.UpgradeAcquisitionTimeline[index];
                    EditorGUILayout.LabelField($"Mutation {index + 1} at tick {acquisition.EffectiveTick}", acquisition.Snapshot.DisplayName);
                }
            }
            EditorGUILayout.LabelField("Genome Fingerprint", preview.ActiveGenomeSnapshot.Fingerprint);

            using (new EditorGUI.DisabledScope(run == null || run.Status != SimulationRunStatus.Ready))
                if (GUILayout.Button("Start")) preview.StartSimulation();
            using (new EditorGUI.DisabledScope(run == null || run.Status != SimulationRunStatus.Running))
                if (GUILayout.Button("Pause")) preview.PauseSimulation();
            using (new EditorGUI.DisabledScope(run == null || run.Status != SimulationRunStatus.Paused))
            {
                if (GUILayout.Button("Advance One Simulation Tick")) preview.AdvanceOneTickWhilePaused();
                if (GUILayout.Button("Resume")) preview.ResumeSimulation();
            }
            if (GUILayout.Button("Reset to Start")) preview.ResetToStart();
            if (run?.Status == SimulationRunStatus.AwaitingDecision)
                EditorGUILayout.HelpBox("Choose a Mutation or Skip in the Game view before continuing.", MessageType.Info);
            if (!string.IsNullOrEmpty(runtimeMessage))
                EditorGUILayout.HelpBox(runtimeMessage, MessageType.Info);
            Repaint();
        }

        public static bool TryApplyS4Fixture(SpeciesSimulationPreview preview, int seed, out string message)
        {
            message = "The S4 fixture can only be applied before a session starts.";
            if (preview == null || !preview.SettingsEditable) return false;
            var scenarioIndex = -1;
            for (var index = 0; index < preview.ScenarioOptions.Count; index++)
            {
                if (AssetDatabase.GetAssetPath(preview.ScenarioOptions[index]) == "Assets/Data/ProductionData/CellularSimulation/Scenarios/ForestEdge.asset")
                {
                    scenarioIndex = index;
                    break;
                }
            }
            if (scenarioIndex < 0)
            {
                message = "Add the production Forest Edge scenario to this preview's scenario list before applying the fixture.";
                return false;
            }
            return preview.TrySelectScenario(scenarioIndex, out message)
                && preview.TrySetPlayerSpecies("hare", out message)
                && preview.TryApplyGlobalSettingsForTicksWithStartingPopulations(
                    "36", "20", seed.ToString(CultureInfo.InvariantCulture), "720", "0", "600", "0.1",
                    "0", "0", "0", false, "400", "20", "10", out message)
                && preview.TryApplyExperimentalFeatures(true, false, "0", out message)
                && preview.TryApplyContinuousPhases(true, "100", out message);
        }
    }
}
