using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaltyGame.EditorTests
{
    [Category("Tooling")]
    public sealed class PopulationConfigurationTests
    {
        const string ScenarioPath = "Assets/Data/ProductionData/CellularSimulation/Scenarios/ForestEdge.asset";
        const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;

        static CellularSimData ApplyOverrides(params string[] arguments)
        {
            var runner = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType("SaltyGame.EditorTools.CellularSimulationExperimentRunner"))
                .First(type => type != null);
            var optionsType = runner.GetNestedType("CommandLineOptions", BindingFlags.NonPublic);
            var options = optionsType.GetMethod("Parse").Invoke(null, new object[] { arguments });
            var data = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(ScenarioPath).CreateRuntimeData();
            return (CellularSimData)runner.GetMethod("ApplyOverrides", BindingFlags.Static | BindingFlags.NonPublic)
                .Invoke(null, new[] { (object)data, options });
        }

        [Test]
        public void StartingPopulationsAndGridOverridesAreAppliedTogether()
        {
            var data = ApplyOverrides("-gridWidth", "10", "-gridHeight", "10",
                "-startingPopulations", "plant=40,hare=5,fox=2");
            Assert.That(data.Width, Is.EqualTo(10));
            Assert.That(data.Height, Is.EqualTo(10));
            Assert.That(data.StartingPopulations[new SpeciesId("plant")], Is.EqualTo(40));
            Assert.That(data.StartingPopulations[new SpeciesId("hare")], Is.EqualTo(5));
            Assert.That(data.StartingPopulations[new SpeciesId("fox")], Is.EqualTo(2));
            var authored = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(ScenarioPath).CreateRuntimeData();
            Assert.That(authored.StartingPopulations[new SpeciesId("hare")], Is.EqualTo(55));
            Assert.That(authored.StartingPopulations[new SpeciesId("fox")], Is.EqualTo(35));
        }

        [TestCase("hare=-1")]
        [TestCase("hare=5,hare=2")]
        [TestCase("unknown=1")]
        [TestCase("plant=101")]
        [TestCase("hare=abc")]
        public void InvalidStartingPopulationsAreRejected(string populations)
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                ApplyOverrides("-gridWidth", "10", "-gridHeight", "10", "-startingPopulations", populations));
            Assert.That(exception.InnerException, Is.InstanceOf<ArgumentException>());
        }

        [TestCase(true, "v5")]
        [TestCase(true, "v4")]
        [TestCase(false, "v5")]
        public void SavedPopulationOverridesArePreservedAndDefaultsFollowTheScenario(bool explicitOverride, string version)
        {
            var keys = new[] { "v3", "v4", "v5" }
                .Select(value => "SaltyGame.SpeciesSimulationPreview.DefaultSettings." + value).ToArray();
            var previous = keys.ToDictionary(key => key, key => PlayerPrefs.HasKey(key) ? PlayerPrefs.GetString(key) : null);
            var scenario = Object.Instantiate(AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(ScenarioPath));
            scenario.name = "ForestEdge";
            var previewObject = new GameObject("Population Settings Test");
            previewObject.SetActive(false);
            try
            {
                foreach (var key in keys) PlayerPrefs.DeleteKey(key);
                var serialized = new SerializedObject(scenario);
                serialized.FindProperty("species").GetArrayElementAtIndex(1).FindPropertyRelative("startingPopulation").intValue = 25;
                serialized.FindProperty("species").GetArrayElementAtIndex(2).FindPropertyRelative("startingPopulation").intValue = 15;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                var preview = previewObject.AddComponent<SpeciesSimulationPreview>();
                preview.ConfigureScenarioOptions(new[] { scenario });
                typeof(SpeciesSimulationPreview).GetMethod("Awake", PrivateInstance).Invoke(preview, null);
                typeof(SpeciesSimulationPreview).GetField("herbivoreStartingPopulation", PrivateInstance).SetValue(preview, 55);
                typeof(SpeciesSimulationPreview).GetField("carnivoreStartingPopulation", PrivateInstance).SetValue(preview, 35);
                typeof(SpeciesSimulationPreview).GetField("startingPopulationOverrideEnabled", PrivateInstance).SetValue(preview, explicitOverride);
                preview.SaveCurrentSettingsAsDefault();
                var saved = PlayerPrefs.GetString(keys[2]);
                // This is the pre-cleanup JSON shape: no migration marker.
                Assert.That(saved.Contains("populationPresetMigrationApplied"), Is.False);
                if (version == "v4")
                {
                    PlayerPrefs.SetString(keys[1], saved);
                    PlayerPrefs.DeleteKey(keys[2]);
                }
                typeof(SpeciesSimulationPreview).GetMethod("ApplySelectedScenario", PrivateInstance).Invoke(preview, null);
                typeof(SpeciesSimulationPreview).GetMethod("LoadSavedSettings", PrivateInstance).Invoke(preview, null);
                preview.ResetToStart();
                Assert.That(preview.HerbivoreStartingPopulation, Is.EqualTo(explicitOverride ? 55 : 25));
                Assert.That(preview.CarnivoreStartingPopulation, Is.EqualTo(explicitOverride ? 35 : 15));
                var initial = preview.Run.PopulationHistory[0];
                Assert.That(initial.GetCount(new SpeciesId("hare")), Is.EqualTo(explicitOverride ? 55 : 25));
                Assert.That(initial.GetCount(new SpeciesId("fox")), Is.EqualTo(explicitOverride ? 35 : 15));
                preview.SaveCurrentSettingsAsDefault();
                typeof(SpeciesSimulationPreview).GetMethod("LoadSavedSettings", PrivateInstance).Invoke(preview, null);
                Assert.That(preview.HerbivoreStartingPopulation, Is.EqualTo(explicitOverride ? 55 : 25));
            }
            finally
            {
                Object.DestroyImmediate(previewObject);
                Object.DestroyImmediate(scenario);
                foreach (var entry in previous)
                {
                    if (entry.Value == null) PlayerPrefs.DeleteKey(entry.Key);
                    else PlayerPrefs.SetString(entry.Key, entry.Value);
                }
                PlayerPrefs.Save();
            }
        }

        [Test]
        public void ScenarioPopulationEditsSupportUndoSaveAndReload()
        {
            var path = "Assets/PopulationConfigurationTest-" + Guid.NewGuid().ToString("N") + ".asset";
            Assert.That(AssetDatabase.CopyAsset(ScenarioPath, path), Is.True);
            var scenario = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(path);
            try
            {
                Undo.IncrementCurrentGroup();
                var serialized = new SerializedObject(scenario);
                var population = serialized.FindProperty("species").GetArrayElementAtIndex(1).FindPropertyRelative("startingPopulation");
                population.intValue = 25;
                serialized.ApplyModifiedProperties();
                Undo.FlushUndoRecordObjects();
                Assert.That(scenario.CreateRuntimeData().StartingPopulations[new SpeciesId("hare")], Is.EqualTo(25));
                Undo.PerformUndo();
                Assert.That(scenario.CreateRuntimeData().StartingPopulations[new SpeciesId("hare")], Is.EqualTo(55));
                serialized.Update();
                population.intValue = 25;
                serialized.ApplyModifiedProperties();
                AssetDatabase.SaveAssetIfDirty(scenario);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                var loaded = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(path).CreateRuntimeData();
                Assert.That(loaded.StartingPopulations[new SpeciesId("hare")], Is.EqualTo(25));
                Assert.That(loaded.StartingPopulations[new SpeciesId("fox")], Is.EqualTo(35));
            }
            finally
            {
                Undo.ClearUndo(scenario);
                AssetDatabase.DeleteAsset(path);
            }
        }
    }
}
