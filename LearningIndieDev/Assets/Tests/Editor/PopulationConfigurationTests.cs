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
        public void S4EditorFixtureIsTransientAndSupportsPausedSingleTicks()
        {
            var prefsKeys = new[] { "v3", "v4", "v5" }
                .Select(value => "SaltyGame.SpeciesSimulationPreview.DefaultSettings." + value).ToArray();
            var prefsBefore = prefsKeys.ToDictionary(key => key, key => PlayerPrefs.HasKey(key) ? PlayerPrefs.GetString(key) : null);
            var scenario = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(ScenarioPath);
            var assetBefore = System.IO.File.ReadAllText(ScenarioPath);
            var previewObject = new GameObject("S4 Fixture Test");
            previewObject.SetActive(false);
            try
            {
                var preview = previewObject.AddComponent<SpeciesSimulationPreview>();
                preview.ConfigureScenarioOptions(new[] { scenario });
                typeof(SpeciesSimulationPreview).GetMethod("Awake", PrivateInstance).Invoke(preview, null);
                var editor = AppDomain.CurrentDomain.GetAssemblies()
                    .Select(assembly => assembly.GetType("SaltyGame.EditorTools.SpeciesSimulationPreviewEditor"))
                    .First(type => type != null);
                var apply = editor.GetMethod("TryApplyS4Fixture");
                var arguments = new object[] { preview, 10100, null };
                Assert.That(apply.Invoke(null, arguments), Is.True, (string)arguments[2]);
                Assert.That(preview.PlayerSpecies.Value, Is.EqualTo("hare"));
                Assert.That(preview.BaseSeed, Is.EqualTo(10100));
                Assert.That(preview.StepInterval, Is.EqualTo(0.1f));
                Assert.That(preview.RunTicks, Is.EqualTo(600));
                Assert.That(preview.PhaseLengthTicks, Is.EqualTo(100));
                Assert.That(preview.ContinuousPhasesEnabled, Is.True);
                Assert.That(preview.CoupledSpeciesResponsesEnabled, Is.False);
                Assert.That(preview.RandomizeSeedOnStart, Is.False);
                preview.StartSimulation();
                var initial = preview.Run.PopulationHistory[0];
                Assert.That(initial.GetCount(new SpeciesId("plant")), Is.EqualTo(400));
                Assert.That(initial.GetCount(new SpeciesId("hare")), Is.EqualTo(20));
                Assert.That(initial.GetCount(new SpeciesId("fox")), Is.EqualTo(10));
                preview.PauseSimulation();
                Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
                Assert.That(preview.Run.Tick, Is.EqualTo(1));
                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Paused));
                Assert.That(apply.Invoke(null, arguments), Is.False);
                Assert.That(preview.Run.Tick, Is.EqualTo(1));
                foreach (var key in prefsKeys)
                    Assert.That(PlayerPrefs.HasKey(key) ? PlayerPrefs.GetString(key) : null, Is.EqualTo(prefsBefore[key]));
                Assert.That(System.IO.File.ReadAllText(ScenarioPath), Is.EqualTo(assetBefore));
            }
            finally
            {
                Object.DestroyImmediate(previewObject);
            }
        }

        [Test]
        public void BatchTrailblazerLoadoutMatchesProgressionWithoutFleeSpeed()
        {
            var runner = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType("SaltyGame.EditorTools.CellularSimulationExperimentRunner"))
                .First(type => type != null);
            var data = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(ScenarioPath).CreateRuntimeData();
            var ids = new[] { "faster-movement", "threat-exposure", "faster-movement", "threat-response", "faster-movement" };
            var apply = runner.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                .Single(method => method.Name == "ApplyLoadout" && method.GetParameters().Length == 4);
            var result = (CellularSimData)apply.Invoke(null, new object[] { data, "hare", ids, 0f });
            var hare = new SpeciesId("hare");
            var progression = new SpeciesProgression(new SpeciesDefinition(hare, data.SpeciesRules[hare]));
            foreach (var id in ids) Assert.That(progression.TryApplyFreeUpgrade(SpeciesUpgradeCatalog.Create(id)), Is.True);
            Assert.That(result.SpeciesRules[hare].MovementSpeed, Is.EqualTo(progression.CurrentRules.MovementSpeed));
            Assert.That(result.SpeciesRules[hare].FleeMovementSpeedBonus, Is.EqualTo(data.SpeciesRules[hare].FleeMovementSpeedBonus));
            var createSnapshots = runner.GetMethod("CreateLegacyUpgradeSnapshots", BindingFlags.Static | BindingFlags.NonPublic);
            var snapshots = (IReadOnlyList<SpeciesUpgradeSnapshot>)createSnapshots.Invoke(null, new object[] { ids, "hare", 0f });
            var snapshotApply = runner.GetMethod("ApplySnapshotLoadout", BindingFlags.Static | BindingFlags.NonPublic);
            var actualResult = (CellularSimData)snapshotApply.Invoke(null, new object[] { data, "hare", snapshots });
            Assert.That(actualResult.SpeciesRules[hare].MovementSpeed, Is.EqualTo(progression.CurrentRules.MovementSpeed));
            Assert.That(actualResult.SpeciesRules[hare].FleeMovementSpeedBonus, Is.EqualTo(data.SpeciesRules[hare].FleeMovementSpeedBonus));
            Assert.That(snapshots.Sum(snapshot => snapshot.PreContactAvoidanceChanceBonus), Is.EqualTo(progression.PreContactAvoidanceChance));
            var prediction = SaltyGame.EditorTools.SpeciesUpgradePredictionInputAdapter.CreateInput(snapshots);
            Assert.That(prediction.upgrades[1].preContactAvoidanceChanceBonus, Is.EqualTo(0.08f));
            Assert.That(prediction.upgrades[1].modifiers, Is.Empty);
            var serialization = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType("SaltyGame.EditorTools.SimulationReportSerialization"))
                .First(type => type != null);
            var records = (Array)serialization.GetMethod("CreateUpgradeLoadout").Invoke(null, new object[] { snapshots });
            var avoidanceRecord = records.GetValue(1);
            Assert.That(avoidanceRecord.GetType().GetField("preContactAvoidanceChanceBonus").GetValue(avoidanceRecord), Is.EqualTo(0.08f));
            StringAssert.Contains("preContactAvoidanceChanceBonus", JsonUtility.ToJson(avoidanceRecord));
            var invalid = Enumerable.Repeat("faster-movement", 11).ToArray();
            Assert.Throws<TargetInvocationException>(() => createSnapshots.Invoke(null, new object[] { invalid, "hare", 0f }));
            Assert.Throws<TargetInvocationException>(() => apply.Invoke(null, new object[] { data, "hare", invalid, 0f }));
            Assert.Throws<TargetInvocationException>(() => apply.Invoke(null, new object[] { data, "hare", new[] { "threat-exposure" }, 0.75f }));
        }

        [Test]
        public void WarrenBatchSnapshotsAndAuthoringCopiesPreserveCrowdingReduction()
        {
            var data = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(ScenarioPath).CreateRuntimeData();
            var hare = new SpeciesId("hare");
            var runner = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType("SaltyGame.EditorTools.CellularSimulationExperimentRunner"))
                .First(type => type != null);
            var ids = new[] { "tough-hide", "crowding-tolerance", "tough-hide", "crowding-tolerance", "tough-hide" };
            var snapshots = (IReadOnlyList<SpeciesUpgradeSnapshot>)runner
                .GetMethod("CreateLegacyUpgradeSnapshots", BindingFlags.Static | BindingFlags.NonPublic)
                .Invoke(null, new object[] { ids, "hare", 0f });
            var result = (CellularSimData)runner.GetMethod("ApplySnapshotLoadout", BindingFlags.Static | BindingFlags.NonPublic)
                .Invoke(null, new object[] { data, "hare", snapshots });
            var rules = result.SpeciesRules[hare];
            Assert.That(rules.CrowdingEnergyReduction, Is.EqualTo(0.2f));
            Assert.That(rules.BlockAmount, Is.EqualTo(data.SpeciesRules[hare].BlockAmount + 6));
            Assert.That(rules.CrowdingTolerance, Is.EqualTo(data.SpeciesRules[hare].CrowdingTolerance));
            Assert.That(result.Fingerprint, Is.Not.EqualTo(data.Fingerprint));
            var prediction = SaltyGame.EditorTools.SpeciesUpgradePredictionInputAdapter.CreateInput(snapshots);
            Assert.That(prediction.upgrades[1].modifiers[0].attributeId, Is.EqualTo(SpeciesAttributeIds.CrowdingEnergyReduction));
            Assert.That(prediction.upgrades[1].modifiers[0].signedValue, Is.EqualTo(0.1f));
            var metrics = new SpeciesSimulationMetrics();
            var record = typeof(SpeciesSimulationMetrics).GetMethod("Record", PrivateInstance);
            var parameters = record.GetParameters();
            var arguments = parameters.Select(parameter => parameter.DefaultValue).ToArray();
            arguments[0] = hare;
            arguments[Array.FindIndex(parameters, parameter => parameter.Name == "crowdingMetabolismTicks")] = 9;
            arguments[Array.FindIndex(parameters, parameter => parameter.Name == "crowdingEnergyLost")] = 7;
            record.Invoke(metrics, arguments);
            var serialization = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType("SaltyGame.EditorTools.SimulationReportSerialization"))
                .First(type => type != null);
            var activityRecords = (Array)serialization.GetMethod("CreateActivity")
                .Invoke(null, new object[] { metrics, new[] { hare } });
            var activityRecord = activityRecords.GetValue(0);
            Assert.That(activityRecord.GetType().GetField("crowdingMetabolismTicks").GetValue(activityRecord), Is.EqualTo(9));
            Assert.That(activityRecord.GetType().GetField("crowdingEnergyLost").GetValue(activityRecord), Is.EqualTo(7));
            StringAssert.Contains("crowdingEnergyLost", JsonUtility.ToJson(activityRecord));
            var asset = ScriptableObject.CreateInstance<CellularSimDataAsset>();
            var authored = Object.Instantiate(AssetDatabase.LoadAssetAtPath<SpeciesDefinitionAsset>(
                "Assets/Data/ProductionData/CellularSimulation/Species/hare.asset"));
            try
            {
                var definition = typeof(CellularSimDataAsset).GetNestedType("SpeciesDefinition", BindingFlags.NonPublic);
                var entries = result.SpeciesRules.ToArray();
                var array = Array.CreateInstance(definition, entries.Length);
                for (var index = 0; index < entries.Length; index++)
                    array.SetValue(definition.GetMethod("From", BindingFlags.Static | BindingFlags.NonPublic)
                        .Invoke(null, new object[] { entries[index].Key, 0.1f, entries[index].Value }), index);
                typeof(CellularSimDataAsset).GetField("species", PrivateInstance).SetValue(asset, array);
                Assert.That(asset.CreateRuntimeData().SpeciesRules[hare].CrowdingEnergyReduction, Is.EqualTo(0.2f));
                var serialized = new SerializedObject(authored);
                serialized.FindProperty("crowdingEnergyReduction").floatValue = 0.5f;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                Assert.That(authored.CreateRules().CrowdingEnergyReduction, Is.EqualTo(0.5f));
                StringAssert.Contains("crowdingEnergyReduction", JsonUtility.ToJson(asset));
            }
            finally { Object.DestroyImmediate(asset); Object.DestroyImmediate(authored); }
        }

        [Test]
        public void GardenersBatchLoadoutMatchesProgressionAndReportsDirectPlantingCounters()
        {
            var data = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(ScenarioPath).CreateRuntimeData();
            var hare = new SpeciesId("hare");
            var ids = new[] { "efficient-digestion", "seed-dispersal", "efficient-digestion", "seed-dispersal", "efficient-digestion" };
            var runner = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType("SaltyGame.EditorTools.CellularSimulationExperimentRunner"))
                .First(type => type != null);
            var create = runner.GetMethod("CreateLegacyUpgradeSnapshots", BindingFlags.Static | BindingFlags.NonPublic);
            var snapshots = (IReadOnlyList<SpeciesUpgradeSnapshot>)create.Invoke(null, new object[] { ids, "hare", 0f });
            var result = (CellularSimData)runner.GetMethod("ApplySnapshotLoadout", BindingFlags.Static | BindingFlags.NonPublic)
                .Invoke(null, new object[] { data, "hare", snapshots });
            var progression = new SpeciesProgression(new SpeciesDefinition(hare, data.SpeciesRules[hare]));
            foreach (var id in ids) Assert.That(progression.TryApplyFreeUpgrade(SpeciesUpgradeCatalog.Create(id)), Is.True);
            Assert.That(result.SpeciesRules[hare].SeedDropChance, Is.EqualTo(data.SpeciesRules[hare].SeedDropChance + 0.02f).Within(0.000001f));
            Assert.That(result.SpeciesRules[hare].SeedDropChance, Is.EqualTo(progression.CurrentRules.SeedDropChance));
            Assert.That(result.SpeciesRules[hare].DigestionEnergyBonus, Is.EqualTo(progression.CurrentRules.DigestionEnergyBonus));
            Assert.That(result.Fingerprint, Is.Not.EqualTo(data.Fingerprint));
            var prediction = SaltyGame.EditorTools.SpeciesUpgradePredictionInputAdapter.CreateInput(snapshots);
            Assert.That(prediction.upgrades[1].modifiers[0].attributeId, Is.EqualTo(SpeciesAttributeIds.SeedDropChance));
            Assert.That(prediction.upgrades[1].modifiers[0].signedValue, Is.EqualTo(0.01f));
            Assert.Throws<TargetInvocationException>(() => create.Invoke(null,
                new object[] { Enumerable.Repeat("seed-dispersal", 11).ToArray(), "hare", 0f }));
            var metrics = new SpeciesSimulationMetrics();
            var record = typeof(SpeciesSimulationMetrics).GetMethod("Record", PrivateInstance);
            var parameters = record.GetParameters();
            var arguments = parameters.Select(parameter => parameter.DefaultValue).ToArray();
            arguments[0] = hare;
            arguments[Array.FindIndex(parameters, parameter => parameter.Name == "seedDropAttempts")] = 9;
            arguments[Array.FindIndex(parameters, parameter => parameter.Name == "seedDropSuccesses")] = 2;
            arguments[Array.FindIndex(parameters, parameter => parameter.Name == "seedDropFoodCreated")] = 6.5f;
            arguments[Array.FindIndex(parameters, parameter => parameter.Name == "seedDropReserveSpent")] = 2;
            record.Invoke(metrics, arguments);
            var serialization = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType("SaltyGame.EditorTools.SimulationReportSerialization"))
                .First(type => type != null);
            var records = (Array)serialization.GetMethod("CreateActivity").Invoke(null, new object[] { metrics, new[] { hare } });
            var activity = records.GetValue(0);
            Assert.That(activity.GetType().GetField("seedDropAttempts").GetValue(activity), Is.EqualTo(9));
            Assert.That(activity.GetType().GetField("seedDropSuccesses").GetValue(activity), Is.EqualTo(2));
            Assert.That(activity.GetType().GetField("seedDropFoodCreated").GetValue(activity), Is.EqualTo(6.5f));
            Assert.That(activity.GetType().GetField("seedDropReserveSpent").GetValue(activity), Is.EqualTo(2));
            StringAssert.Contains("seedDropFoodCreated", JsonUtility.ToJson(activity));
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
