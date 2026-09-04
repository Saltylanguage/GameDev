using UnityEditor;
using UnityEngine;

namespace SaltyGame.EditorTools
{
    [CustomEditor(typeof(SpeciesUpgradeAsset))]
    public sealed class SpeciesUpgradeAssetEditor : Editor
    {
        SerializedProperty modifiers;

        void OnEnable()
        {
            modifiers = serializedObject.FindProperty("modifiers");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "modifiers");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Attribute Modifiers", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "V1 modifiers are signed additive values. Each upgrade targets one species and each attribute may appear once.",
                MessageType.Info);

            modifiers.arraySize = Mathf.Max(
                0,
                EditorGUILayout.IntField("Modifier Count", modifiers.arraySize));
            for (var index = 0; index < modifiers.arraySize; index++)
            {
                DrawModifier(modifiers.GetArrayElementAtIndex(index), index);
            }

            serializedObject.ApplyModifiedProperties();
            DrawValidationMessage();
        }

        static void DrawModifier(SerializedProperty modifier, int index)
        {
            var attributeId = modifier.FindPropertyRelative("attributeId");
            var signedValue = modifier.FindPropertyRelative("signedValue");
            var definition = SpeciesAttributeRegistry.TryGet(attributeId.stringValue, out var resolved)
                ? resolved
                : default(SpeciesAttributeDefinition);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField($"Modifier {index + 1}", EditorStyles.boldLabel);

            var definitions = SpeciesAttributeRegistry.All;
            var optionOffset = 0;
            var options = new string[definitions.Count + 1];
            if (string.IsNullOrWhiteSpace(attributeId.stringValue))
            {
                options[0] = "<Select attribute>";
            }
            else if (!string.IsNullOrEmpty(attributeId.stringValue))
            {
                options[0] = $"<Unknown: {attributeId.stringValue}>";
            }
            else
            {
                options[0] = "<Select attribute>";
            }

            for (var definitionIndex = 0; definitionIndex < definitions.Count; definitionIndex++)
            {
                options[definitionIndex + 1] = definitions[definitionIndex].DisplayName
                    + $" ({definitions[definitionIndex].Id})";
                if (definitions[definitionIndex].Id == attributeId.stringValue)
                {
                    optionOffset = definitionIndex + 1;
                }
            }

            var selectedIndex = EditorGUILayout.Popup("Attribute", optionOffset, options);
            if (selectedIndex > 0)
            {
                attributeId.stringValue = definitions[selectedIndex - 1].Id;
                definition = definitions[selectedIndex - 1];
            }

            if (string.IsNullOrEmpty(definition.Id) || definition.ValueKind == SpeciesAttributeValueKind.Float)
            {
                signedValue.floatValue = EditorGUILayout.FloatField("Signed Value", signedValue.floatValue);
            }
            else
            {
                signedValue.floatValue = EditorGUILayout.IntField("Signed Value", Mathf.RoundToInt(signedValue.floatValue));
            }

            if (!string.IsNullOrEmpty(definition.Id))
            {
                EditorGUILayout.LabelField("Value Kind", definition.ValueKind.ToString());
            }

            EditorGUILayout.EndVertical();
        }

        void DrawValidationMessage()
        {
            var asset = (SpeciesUpgradeAsset)target;
            if (asset.TryCreateSnapshot(out _, out var validationMessage))
            {
                EditorGUILayout.HelpBox(
                    "Valid per-run upgrade contract. The immutable snapshot will be used at runtime.",
                    MessageType.Info);
            }
            else if (!string.IsNullOrWhiteSpace(validationMessage))
            {
                EditorGUILayout.HelpBox(validationMessage, MessageType.Error);
            }
        }
    }
}
