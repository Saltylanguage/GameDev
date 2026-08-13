using UnityEditor;
using UnityEngine;

namespace SaltyGame.EditorTools
{
    [CustomEditor(typeof(SaltyGame.SpeciesSimulationPreview))]
    public sealed class SpeciesSimulationPreviewEditor : Editor
    {
        SerializedProperty scenarioOptions;
        SerializedProperty selectedScenarioIndex;

        void OnEnable()
        {
            scenarioOptions = serializedObject.FindProperty("scenarioOptions");
            selectedScenarioIndex = serializedObject.FindProperty("selectedScenarioIndex");
        }

        public override void OnInspectorGUI()
        {
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
    }
}
