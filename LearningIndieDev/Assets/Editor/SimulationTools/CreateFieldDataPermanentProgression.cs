using System;
using System.Collections.Generic;
using UnityEditor;

namespace SaltyGame.EditorTools
{
    public static class CreateFieldDataPermanentProgression
    {
        static readonly string[] NodeFiles =
        {
            "GuardedBurrow", "KeenHearing", "SwiftDigging", "SafeForaging", "WarrenNetwork",
        };

        [MenuItem("Salty Game/Genome/Apply Approved FD2 Starter Definition")]
        public static void ApplyApprovedDefinition()
        {
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            var map = AssetDatabase.LoadAssetAtPath<SpeciesGenomeMapAsset>(
                FieldDataPermanentProgressionDefinition.HareMapPath);
            if (map == null)
            {
                throw new InvalidOperationException("The existing FD2 Hare map is required.");
            }

            var assets = new List<GenomeUpgradeAsset>();
            for (var index = 0; index < NodeFiles.Length; index++)
            {
                var path = "Assets/Data/TestData/Genome/HareGenome/" + NodeFiles[index] + ".asset";
                var asset = AssetDatabase.LoadAssetAtPath<GenomeUpgradeAsset>(path);
                if (asset == null) throw new InvalidOperationException("Missing existing FD2 node: " + path);
                assets.Add(asset);
            }

            Undo.RecordObjects(assets.ToArray(), "Apply approved FD2 definition");
            Undo.RecordObject(map, "Apply approved FD2 catalog");
            for (var index = 0; index < assets.Count; index++)
            {
                var node = definition.HareMap.GetNode(assets[index].UpgradeId);
                var serialized = new SerializedObject(assets[index]);
                serialized.FindProperty("upgradeId").stringValue = node.Id;
                serialized.FindProperty("displayName").stringValue = node.DisplayName;
                serialized.FindProperty("description").stringValue = node.Description;
                serialized.FindProperty("targetSpeciesId").stringValue = node.TargetSpecies.Value;
                serialized.FindProperty("rabbitDataPrice").longValue = node.RabbitDataPrice;
                serialized.FindProperty("capacityPoints").intValue = node.CapacityPoints;
                serialized.FindProperty("branchName").stringValue = node.BranchName;
                var prerequisites = serialized.FindProperty("prerequisiteNodeIds");
                prerequisites.arraySize = node.PrerequisiteNodeIds.Count;
                for (var parent = 0; parent < prerequisites.arraySize; parent++)
                {
                    prerequisites.GetArrayElementAtIndex(parent).stringValue = node.PrerequisiteNodeIds[parent];
                }
                var modifiers = serialized.FindProperty("modifiers");
                modifiers.arraySize = node.Modifiers.Count;
                for (var modifier = 0; modifier < modifiers.arraySize; modifier++)
                {
                    var entry = modifiers.GetArrayElementAtIndex(modifier);
                    entry.FindPropertyRelative("attributeId").stringValue = node.Modifiers[modifier].AttributeId;
                    entry.FindPropertyRelative("signedValue").floatValue = node.Modifiers[modifier].SignedValue;
                }
                serialized.ApplyModifiedProperties();
            }

            var serializedMap = new SerializedObject(map);
            serializedMap.FindProperty("speciesId").stringValue = definition.HareMap.SpeciesId.Value;
            var upgrades = serializedMap.FindProperty("upgrades");
            upgrades.arraySize = assets.Count;
            for (var index = 0; index < assets.Count; index++)
            {
                upgrades.GetArrayElementAtIndex(index).objectReferenceValue = assets[index];
            }
            serializedMap.ApplyModifiedProperties();
            definition.ValidateAuthoredMap(map.CreateSnapshot());
            AssetDatabase.SaveAssets();
        }

    }
}
