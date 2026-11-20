using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using NUnit.Framework;
using UnityEngine;

namespace SaltyGame.EditorTests
{
    public sealed class CellSimPortableProvenanceTests
    {
        [Test]
        public void PortableManifestCoversEveryBatchSharedSource()
        {
            var projectRoot = Directory.GetParent(Application.dataPath).FullName;
            var projectDirectory = Path.Combine(projectRoot, "tools", "CellSim.Batch");
            var project = XDocument.Load(Path.Combine(projectDirectory, "CellSim.Batch.csproj"));
            var sharedSources = project.Descendants("SharedSource")
                .Select(element => (string)element.Attribute("Include"))
                .Select(include => Path.GetRelativePath(projectRoot,
                    Path.GetFullPath(Path.Combine(projectDirectory, include))).Replace('\\', '/'));
            var portableSources = GetPortableSourceFiles();

            foreach (var source in sharedSources)
            {
                Assert.That(portableSources, Does.Contain(source),
                    "CellSim.Batch compiles this source, so portable exports must fingerprint it: " + source);
            }
        }

        [Test]
        public void ChangingOnlyPopulationInterventionChangesPortableSourceHash()
        {
            const string target = "Assets/Scripts/Game/Simulation/PopulationIntervention.cs";
            Assert.That(GetPortableSourceFiles(), Does.Contain(target));

            var before = GetPortableSourceHash(path =>
                path == target ? "population source before" : "unchanged source");
            var after = GetPortableSourceHash(path =>
                path == target ? "population source after" : "unchanged source");

            Assert.That(after, Is.Not.EqualTo(before));
        }

        [Test]
        public void PortableRunDigestIncludesHiddenHareAndBurrowState()
        {
            var hare = new SpeciesId("hare");
            var fox = new SpeciesId("fox");
            var noMovement = new GridPattern(Array.Empty<UnityEngine.Vector2Int>());
            var hareRules = new SpeciesRules(movementSpeed: 0f, movementPattern: noMovement,
                attackPattern: noMovement, attackAmount: 0, blockPattern: noMovement, blockAmount: 0,
                dietPattern: noMovement, dietTarget: null, reproductionPattern: noMovement,
                reproductionNeighborCount: 0, reproductionChance: 0f, reproductionFoodRequired: 1,
                metabolism: 0, role: SpeciesRole.Herbivore);
            var foxRules = new SpeciesRules(movementSpeed: 0f, movementPattern: noMovement,
                attackPattern: GridPatternTemplates.CreateMooreRange(1), attackAmount: 100,
                blockPattern: noMovement, blockAmount: 0, dietPattern: noMovement, dietTarget: hare,
                reproductionPattern: noMovement, reproductionNeighborCount: 0,
                reproductionChance: 0f, reproductionFoodRequired: 1, metabolism: 0,
                role: SpeciesRole.Carnivore);
            var cells = new Grid<SpeciesCell>(7, 7);
            cells.SetCell(3, 3, new SpeciesCell(hare, energy: 48));
            var run = new SimulationRunState(cells, hare, seed: 75, durationSeconds: 100f);
            run.ConfigureContinuousPhases(10);
            var rules = new Dictionary<SpeciesId, SpeciesRules> { [hare] = hareRules, [fox] = foxRules };
            var burrowing = SpeciesUpgradeCatalog.ResolveFieldDataOrdinaryRewardPool(hare,
                    Array.Empty<SpeciesUpgradeSnapshot>(), id => id == SpeciesUpgradeCatalog.BurrowingId)
                .Single(upgrade => upgrade.Id == SpeciesUpgradeCatalog.BurrowingId);
            var simulation = new SpeciesSimulationRunner(run, rules, 0.1f,
                upgradeLoadout: new[] { burrowing });

            Assert.That(simulation.AdvanceOneTick(), Is.True);
            run.Cells.SetCell(4, 3, new SpeciesCell(fox, energy: 100));
            Assert.That(simulation.AdvanceOneTick(), Is.True);
            Assert.That(run.HiddenHareCount, Is.EqualTo(1));

            // Keep the visible grid identical while omitting simulation-owned
            // Burrow state; only the portable hidden-state contribution differs.
            var surfaceOnly = new SimulationRunState(run.Cells.Copy(), hare, seed: 75, durationSeconds: 100f);
            var withBurrowDigest = GetPortableRunStateDigest(run);
            var surfaceOnlyDigest = GetPortableRunStateDigest(surfaceOnly);

            Assert.That(withBurrowDigest, Is.Not.EqualTo(surfaceOnlyDigest));
        }

        [Test]
        public void ShapeTopologyAndGenerationInputsSurvivePortableJsonAndChangeReplayIdentity()
        {
            var cells = new[] { new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1), new Vector2Int(1, 0), new Vector2Int(1, 2) };
            var edges = new[] { new MapBoundaryConnection(cells[0], Vector2Int.left, cells[2]),
                new MapBoundaryConnection(cells[3], Vector2Int.down, cells[4]) };
            var topology = new MapTopology(3, 3, cells, edges, "cross;seed=17");
            var authored = JsonUtility.FromJson<MapTopologyDefinition>(JsonUtility.ToJson(new MapTopologyDefinition(topology)));
            Assert.That(authored.Create(3, 3).Fingerprint, Is.EqualTo(topology.Fingerprint));
            var none = new GridPattern(Array.Empty<Vector2Int>());
            var rules = new SpeciesRules(0f, none, none, 0, none, 0, none, null, none, 0, 0f, 0);
            var data = new CellularSimData(3, 3, new Dictionary<SpeciesId, float>(),
                new Dictionary<SpeciesId, SpeciesRules> { [SpeciesIds.Herbivore] = rules }, 10f, 0.1f, topology: topology);
            var snapshotType = GetPortableRunnerType().GetNestedType("PortableSnapshot", BindingFlags.Public);
            var snapshot = snapshotType.GetMethod("Create").Invoke(null, new object[] { data });
            var json = JsonUtility.ToJson(snapshot);
            var deserialized = JsonUtility.FromJson(json, snapshotType);
            var restored = (CellularSimData)snapshotType.GetMethod("Restore").Invoke(deserialized, null);
            Assert.That(restored.Fingerprint, Is.EqualTo(data.Fingerprint));
            Assert.That(restored.Topology.Fingerprint, Is.EqualTo(topology.Fingerprint));
            Assert.That(restored.Topology.TryResolveOffset(0, 1, Vector2Int.left, out var target), Is.True);
            Assert.That(target, Is.EqualTo(new Vector2Int(2, 1)));
            var first = new SimulationRunState(SpeciesInitialGridFactory.Create(data, 17), SpeciesIds.Herbivore, 17, 10f);
            var changedData = data.WithTopology(new MapTopology(3, 3, cells, edges, "cross;seed=18"));
            var changed = new SimulationRunState(SpeciesInitialGridFactory.Create(changedData, 17), SpeciesIds.Herbivore, 17, 10f);
            Assert.That(GetPortableRunStateDigest(first), Is.Not.EqualTo(GetPortableRunStateDigest(changed)));
            snapshotType.GetField("mapCells").SetValue(deserialized, null);
            Assert.Throws<TargetInvocationException>(() => snapshotType.GetMethod("Restore").Invoke(deserialized, null));
        }

        [Test]
        public void GeneratedIslandsKeepTopologyAndFinalStateThroughPortableReplay()
        {
            var definition = JsonUtility.FromJson<MapTopologyDefinition>(
                "{\"shape\":8,\"mapSeed\":17,\"edgeRoughness\":65,\"interiorHole\":true,\"detachedIsland\":true}");
            var topology = definition.Create(68, 48);
            var hare = new SpeciesId("hare");
            const string scenarioPath = "Assets/Data/ProductionData/CellularSimulation/Scenarios/ForestEdge.asset";
            var scenario = UnityEditor.AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(scenarioPath);
            var data = new CellularSimData(68, 48, new Dictionary<SpeciesId, float>(), scenario.CreateRuntimeData().SpeciesRules,
                2f, 0.2f, startingPopulations: new Dictionary<SpeciesId, int> { [hare] = 10 }, topology: topology);
            var runner = GetPortableRunnerType();
            var snapshotType = runner.GetNestedType("PortableSnapshot", BindingFlags.Public);
            var snapshot = snapshotType.GetMethod("Create").Invoke(null, new object[] { data });
            var exported = JsonUtility.FromJson(JsonUtility.ToJson(snapshot), snapshotType);
            var restored = (CellularSimData)snapshotType.GetMethod("Restore").Invoke(exported, null);
            Assert.That(restored.Fingerprint, Is.EqualTo(data.Fingerprint));
            Assert.That(restored.Topology.Fingerprint, Is.EqualTo(topology.Fingerprint));

            var arguments = new[] { "-playerSpeciesId", hare.Value, "-runTicks", "10" };
            var run = runner.GetMethod("RunPortableSeed");
            var original = (string)run.Invoke(null, new object[] { snapshot, arguments, 10100, false, data });
            var replay = (string)run.Invoke(null, new object[] { exported, arguments, 10100, false, null });
            Assert.That(original, Does.Contain("\"spatial\":{"));
            var originalDigest = JsonUtility.FromJson<ReplayDigest>(original).finalStateDigest;
            Assert.That(originalDigest, Is.Not.Null.And.Not.Empty);
            Assert.That(JsonUtility.FromJson<ReplayDigest>(replay).finalStateDigest, Is.EqualTo(originalDigest));
        }

        [Serializable]
        private sealed class ReplayDigest
        {
            public string finalStateDigest = null;
        }

        static Type GetPortableRunnerType()
        {
            var type = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType(
                    "SaltyGame.EditorTools.CellularSimulationExperimentRunner", false))
                .FirstOrDefault(candidate => candidate != null);
            Assert.That(type, Is.Not.Null, "The Editor portable runner must be loaded for these tests.");
            return type;
        }

        static string[] GetPortableSourceFiles() => (string[])GetPortableRunnerType()
            .GetField("PortableSourceFiles", BindingFlags.Public | BindingFlags.Static)
            .GetValue(null);

        static string GetPortableSourceHash(Func<string, string> readSource) => (string)GetPortableRunnerType()
            .GetMethod("PortableSourceHash", BindingFlags.Public | BindingFlags.Static)
            .Invoke(null, new object[] { readSource });

        static string GetPortableRunStateDigest(SimulationRunState run) => (string)GetPortableRunnerType()
            .GetMethod("PortableRunStateDigest", BindingFlags.Public | BindingFlags.Static)
            .Invoke(null, new object[] { run });
    }
}
