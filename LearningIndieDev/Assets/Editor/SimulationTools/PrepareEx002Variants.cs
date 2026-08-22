#if UNITY_EDITOR
using System;
using System.IO;
using SaltyGame;
using UnityEditor;
using UnityEngine;

namespace SaltyGame.EditorTools
{
    /// <summary>Creates the versioned, single-mechanism scenario arms for EXP-002.</summary>
    public static class PrepareEx002Variants
    {
        const string SpeciesRoot = "Assets/Data/CellularSimulation/Species";
        const string SpeciesVariantRoot = SpeciesRoot + "/EX002";
        const string VariantRoot = "Assets/Data/CellularSimulation/Scenarios/EX002";
        const string PlantPath = SpeciesRoot + "/plant.asset";
        const string HerbivorePath = SpeciesRoot + "/herbivore.asset";
        const string CarnivorePath = SpeciesRoot + "/carnivore.asset";

        public static void Create()
        {
            EnsureFolder("Assets/Data/CellularSimulation/Scenarios");
            EnsureFolder(SpeciesVariantRoot);
            EnsureFolder(VariantRoot);

            var energyRelief = CopySpecies(
                HerbivorePath,
                SpeciesVariantRoot + "/EX002_HerbivoreEnergyRelief.asset",
                "startingEnergy",
                12);
            var predationRelief = CopySpecies(
                CarnivorePath,
                SpeciesVariantRoot + "/EX002_PredationRelief.asset",
                "attackAmount",
                0);

            CreateScenario(
                VariantRoot + "/EX002_HerbivoreEnergyRelief.asset",
                energyRelief,
                AssetDatabase.LoadAssetAtPath<SpeciesDefinitionAsset>(CarnivorePath));
            CreateScenario(
                VariantRoot + "/EX002_PredationRelief.asset",
                AssetDatabase.LoadAssetAtPath<SpeciesDefinitionAsset>(HerbivorePath),
                predationRelief);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Salty] Prepared EXP-002 versioned intervention scenarios.");
        }

        static SpeciesDefinitionAsset CopySpecies(
            string sourcePath,
            string destinationPath,
            string changedField,
            int changedValue)
        {
            AssetDatabase.DeleteAsset(destinationPath);
            if (!AssetDatabase.CopyAsset(sourcePath, destinationPath))
            {
                throw new IOException($"Could not copy '{sourcePath}' to '{destinationPath}'.");
            }

            var asset = AssetDatabase.LoadAssetAtPath<SpeciesDefinitionAsset>(destinationPath);
            var serialized = new SerializedObject(asset);
            serialized.FindProperty(changedField).intValue = changedValue;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            asset.name = Path.GetFileNameWithoutExtension(destinationPath);
            EditorUtility.SetDirty(asset);
            return asset;
        }

        static void CreateScenario(
            string path,
            SpeciesDefinitionAsset herbivore,
            SpeciesDefinitionAsset carnivore)
        {
            AssetDatabase.DeleteAsset(path);
            var scenario = ScriptableObject.CreateInstance<ScenarioDefinitionAsset>();
            AssetDatabase.CreateAsset(scenario, path);
            var serialized = new SerializedObject(scenario);
            serialized.FindProperty("width").intValue = 32;
            serialized.FindProperty("height").intValue = 20;
            serialized.FindProperty("runDurationSeconds").floatValue = 20f;
            serialized.FindProperty("stepInterval").floatValue = 0.1f;
            serialized.FindProperty("maxPopulation").intValue = 0;
            serialized.FindProperty("minPopulation").intValue = 0;
            var species = serialized.FindProperty("species");
            species.arraySize = 3;
            SetEntry(species.GetArrayElementAtIndex(0), PlantPath, 0.4f);
            SetEntry(species.GetArrayElementAtIndex(1), herbivore, 0.02f);
            SetEntry(species.GetArrayElementAtIndex(2), carnivore, 0.007f);
            serialized.ApplyModifiedPropertiesWithoutUndo();
            scenario.name = Path.GetFileNameWithoutExtension(path);
            EditorUtility.SetDirty(scenario);
        }

        static void SetEntry(SerializedProperty entry, string assetPath, float probability)
        {
            SetEntry(
                entry,
                AssetDatabase.LoadAssetAtPath<SpeciesDefinitionAsset>(assetPath),
                probability);
        }

        static void SetEntry(SerializedProperty entry, SpeciesDefinitionAsset definition, float probability)
        {
            entry.FindPropertyRelative("definition").objectReferenceValue = definition;
            entry.FindPropertyRelative("startingProbability").floatValue = probability;
            entry.FindPropertyRelative("startingPopulation").intValue = 0;
        }

        static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            var parent = Path.GetDirectoryName(path).Replace('\\', '/');
            var name = Path.GetFileName(path);
            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
#endif
