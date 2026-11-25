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

        [TestCase(MapTopologyDefinition.FixedShape.Square, 11, 121)]
        [TestCase(MapTopologyDefinition.FixedShape.Circle, 11, 97)]
        [TestCase(MapTopologyDefinition.FixedShape.Oval, 16, 136)]
        [TestCase(MapTopologyDefinition.FixedShape.Diamond, 11, 61)]
        [TestCase(MapTopologyDefinition.FixedShape.Cross, 11, 57)]
        [TestCase(MapTopologyDefinition.FixedShape.ShortRectangle, 16, 176)]
        [TestCase(MapTopologyDefinition.FixedShape.LongRectangle, 22, 242)]
        public void AuthoredFixedShapeControlsGeometryWrappingPlacementAndBoardCells(
            MapTopologyDefinition.FixedShape shape, int expectedWidth, int expectedCellCount)
        {
            var scenario = Object.Instantiate(AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(ScenarioPath));
            try
            {
                var serialized = new SerializedObject(scenario);
                serialized.FindProperty("mapLayout.shape").intValue = (int)shape;
                serialized.FindProperty("height").intValue = 11;
                var species = serialized.FindProperty("species");
                for (var index = 0; index < species.arraySize; index++)
                {
                    species.GetArrayElementAtIndex(index).FindPropertyRelative("startingPopulation").intValue = 4;
                }
                serialized.ApplyModifiedPropertiesWithoutUndo();

                var data = scenario.CreateRuntimeData();
                var topology = data.Topology;
                Assert.That(data.Width, Is.EqualTo(expectedWidth));
                Assert.That(data.Height, Is.EqualTo(11));
                Assert.That(topology.CellCount, Is.EqualTo(expectedCellCount));
                Assert.That(scenario.CreateRuntimeData().Fingerprint, Is.EqualTo(data.Fingerprint));
                Assert.That(topology.IsPlayableCell(0, 0), Is.EqualTo(expectedCellCount == data.Width * data.Height));

                var playable = topology.PlayableCoordinates.ToArray();
                AssertOppositeSeams(topology, playable);

                var grid = SpeciesInitialGridFactory.Create(data, 17);
                var repeatedGrid = SpeciesInitialGridFactory.Create(scenario.CreateRuntimeData(), 17);
                var rabbit = new SpeciesId("rabbit");
                var run = new SimulationRunState(grid, rabbit, 17, data.RunDurationSeconds);
                var board = SimulationBoardSnapshot.Create(run, data.SpeciesRules, rabbit);
                foreach (var position in playable)
                {
                    var cell = grid.GetCell(position.x, position.y);
                    Assert.That(repeatedGrid.GetCell(position.x, position.y).SpeciesId, Is.EqualTo(cell.SpeciesId));
                }
                foreach (var startingPopulation in data.StartingPopulations)
                {
                    Assert.That(playable.Count(position =>
                    {
                        var cell = grid.GetCell(position.x, position.y);
                        return (cell.IsCreature && cell.SpeciesId == startingPopulation.Key)
                            || (cell.IsPlantResource && cell.ResourceSpeciesId == startingPopulation.Key);
                    }),
                        Is.EqualTo(startingPopulation.Value));
                }
                for (var y = 0; y < data.Height; y++)
                {
                    for (var x = 0; x < data.Width; x++)
                    {
                        var isPlayable = topology.IsPlayableCell(x, y);
                        Assert.That(grid.TryGetCell(x, y, out _), Is.EqualTo(isPlayable));
                        Assert.That(board.TryGetCell(x, y, out _), Is.EqualTo(isPlayable));
                    }
                }
                run.ConfigureContinuousPhases(1);
                run.Start();
                run.Advance(grid.Copy(), data.StepInterval);
                var runner = new SpeciesSimulationRunner(run, data.SpeciesRules, data.StepInterval);
                Assert.That(runner.CanChangeBoundaryPopulation(rabbit, 1, out _, out var reason), Is.True, reason);
                Assert.That(runner.CanChangeBoundaryPopulation(rabbit, 13, out _, out reason), Is.False);
                Assert.That(reason, Does.Contain("Species population limit"));

                var npcRun = new SimulationRunState(grid.Copy(), new SpeciesId("fox"), 17, data.RunDurationSeconds);
                npcRun.ConfigureContinuousPhases(1);
                npcRun.Start();
                npcRun.Advance(npcRun.Cells.Copy(), data.StepInterval);
                var npcRunner = new SpeciesSimulationRunner(npcRun, data.SpeciesRules, data.StepInterval);
                Assert.That(npcRunner.CanChangeBoundaryPopulation(rabbit, 1, out _, out reason), Is.True, reason);
            }
            finally
            {
                Object.DestroyImmediate(scenario);
            }
        }

        [Test]
        public void AuthoredIrregularLandRecreatesAConnectedCoastWithBothSeams()
        {
            var scenario = Object.Instantiate(AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(ScenarioPath));
            try
            {
                var serialized = new SerializedObject(scenario);
                serialized.FindProperty("mapLayout.shape").intValue = (int)MapTopologyDefinition.FixedShape.IrregularLand;
                serialized.FindProperty("mapLayout.mapSeed").intValue = 17;
                serialized.FindProperty("mapLayout.edgeRoughness").intValue = 65;
                serialized.FindProperty("mapLayout.interiorHole").boolValue = false;
                serialized.FindProperty("mapLayout.detachedIsland").boolValue = false;
                serialized.FindProperty("width").intValue = 45;
                serialized.FindProperty("height").intValue = 55;
                serialized.ApplyModifiedPropertiesWithoutUndo();

                var first = scenario.CreateRuntimeData();
                var second = scenario.CreateRuntimeData();
                var topology = (MapTopology)first.Topology;
                var land = topology.CopyCells();
                Assert.That((first.Width, first.Height), Is.EqualTo((45, 55)));
                Assert.That(land, Is.EqualTo(((MapTopology)second.Topology).CopyCells()));
                Assert.That(second.Fingerprint, Is.EqualTo(first.Fingerprint));
                Assert.That(topology.GenerationInputs, Does.StartWith("irregular-land-v2:"));
                Assert.That(topology.GenerationInputs, Does.Contain("seed=17"));
                Assert.That(land.Length, Is.LessThan(first.Width * first.Height));
                var bayRows = land.GroupBy(cell => cell.y)
                    .Where(row => row.Count() < row.Last().x - row.First().x + 1)
                    .Select(row => row.Key).ToHashSet();
                Assert.That(bayRows.Any(y => bayRows.Contains(y + 1) && bayRows.Contains(y + 2)), Is.True,
                    "The reference seed should have a bay running across several adjacent rows.");
                AssertCardinallyConnected(land);
                AssertOppositeSeams(topology, land);

                var grid = SpeciesInitialGridFactory.Create(first, 10100);
                var repeatedGrid = SpeciesInitialGridFactory.Create(second, 10100);
                foreach (var cell in land)
                {
                    Assert.That(repeatedGrid.GetCell(cell.x, cell.y).SpeciesId, Is.EqualTo(grid.GetCell(cell.x, cell.y).SpeciesId));
                }
                var outside = Enumerable.Range(0, first.Width).First(x => !topology.IsPlayableCell(x, 0));
                Assert.That(grid.TryGetCell(outside, 0, out _), Is.False);

                serialized.FindProperty("mapLayout.mapSeed").intValue = -23;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                Assert.That(((MapTopology)scenario.CreateRuntimeData().Topology).CopyCells(), Is.Not.EqualTo(land));
                serialized.FindProperty("mapLayout.mapSeed").intValue = 17;
                serialized.FindProperty("mapLayout.edgeRoughness").intValue = 0;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                Assert.That(((MapTopology)scenario.CreateRuntimeData().Topology).CopyCells(), Is.Not.EqualTo(land));
            }
            finally
            {
                Object.DestroyImmediate(scenario);
            }
        }

        [TestCase(68, 48, true)]
        [TestCase(16, 16, true)]
        [TestCase(15, 16, false)]
        [TestCase(2, 11, false)]
        [TestCase(1, 1, false)]
        public void IrregularLandInteriorHoleBlocksMovementAndKeepsLandConnected(int width, int height, bool hasHole)
        {
            var definition = new MapTopologyDefinition();
            JsonUtility.FromJsonOverwrite("{\"shape\":8,\"mapSeed\":-23,\"edgeRoughness\":100}", definition);
            var solid = (MapTopology)definition.Create(width, height);
            JsonUtility.FromJsonOverwrite("{\"interiorHole\":true}", definition);
            var topology = (MapTopology)definition.Create(width, height);
            var land = topology.CopyCells();
            AssertCardinallyConnected(land);

            if (!hasHole)
            {
                Assert.That(land, Is.EqualTo(solid.CopyCells()));
                return;
            }

            var center = new Vector2Int(width / 2, height / 2);
            var gap = solid.CopyCells().Except(land).ToHashSet();
            Assert.That(gap, Does.Contain(center));
            Assert.That(land.Except(solid.CopyCells()), Is.Empty);
            foreach (var cell in gap)
            {
                foreach (var direction in new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
                {
                    var shore = cell - direction;
                    if (topology.IsPlayableCell(shore.x, shore.y))
                    {
                        Assert.That(topology.TryResolveOffset(shore.x, shore.y, direction, out _), Is.False);
                    }
                    else
                    {
                        Assert.That(gap, Does.Contain(shore), "The hole must be enclosed by playable land.");
                    }
                }
            }
            AssertOppositeSeams(topology, land);
        }

        [Test]
        public void DetachedIslandSettingProducesRepeatableSeparateLandmasses()
        {
            var definition = JsonUtility.FromJson<MapTopologyDefinition>(
                "{\"shape\":8,\"mapSeed\":17,\"edgeRoughness\":65,\"interiorHole\":true}");
            var mainland = definition.Create(68, 48);
            JsonUtility.FromJsonOverwrite("{\"detachedIsland\":true}", definition);
            var topology = (MapTopology)definition.Create(68, 48);
            Assert.That(definition.Create(68, 48).Fingerprint, Is.EqualTo(topology.Fingerprint));
            Assert.That(topology.GenerationInputs, Does.Contain("island=1"));
            Assert.That(topology.CopyCells(), Is.Not.EqualTo(mainland.PlayableCoordinates.ToArray()));

            var land = topology.CopyCells();
            var occupiedColumns = land.Select(cell => cell.x).ToHashSet();
            var divide = Enumerable.Range(occupiedColumns.Min(), occupiedColumns.Max() - occupiedColumns.Min())
                .First(column => !occupiedColumns.Contains(column));
            var west = land.Where(cell => cell.x < divide).ToArray();
            var east = land.Where(cell => cell.x > divide).ToArray();
            AssertCardinallyConnected(west);
            AssertCardinallyConnected(east);
            Assert.That(west.Select(cell => cell.y).Intersect(east.Select(cell => cell.y)), Is.Not.Empty);
            AssertOppositeSeams(topology, west);
            AssertOppositeSeams(topology, east);
            Assert.That(topology.GetDistance(west[0].x, west[0].y, east[0].x, east[0].y), Is.EqualTo(int.MaxValue));
            Assert.That(topology.IsPlayableCell(25, 24), Is.False);
            Assert.That(definition.Create(23, 16).PlayableCoordinates.ToArray(), Is.Not.Empty);
            Assert.That(((MapTopology)definition.Create(23, 16)).GenerationInputs, Does.Not.Contain("island=1"));

            JsonUtility.FromJsonOverwrite("{\"mapSeed\":-23}", definition);
            Assert.That(definition.Create(68, 48).PlayableCoordinates.ToArray(), Is.Not.EqualTo(land));
        }

        private static void AssertCardinallyConnected(Vector2Int[] land)
        {
            Assert.That(land, Is.Not.Empty);
            var remaining = land.ToHashSet();
            var frontier = new Queue<Vector2Int>();
            var directions = new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            frontier.Enqueue(land[0]);
            remaining.Remove(land[0]);
            while (frontier.Count != 0)
            {
                var cell = frontier.Dequeue();
                foreach (var direction in directions)
                {
                    var next = cell + direction;
                    if (remaining.Remove(next)) frontier.Enqueue(next);
                }
            }
            Assert.That(remaining, Is.Empty);
        }

        private static void AssertOppositeSeams(IGridTopology topology, Vector2Int[] playable)
        {
            var rowEdges = playable.GroupBy(cell => cell.y)
                .Select(row => (First: row.First(), Last: row.Last(), Direction: Vector2Int.left));
            var columnEdges = playable.GroupBy(cell => cell.x)
                .Select(column => (First: column.First(), Last: column.Last(), Direction: Vector2Int.down));
            foreach (var edge in rowEdges.Concat(columnEdges))
            {
                Assert.That(topology.TryResolveOffset(edge.First.x, edge.First.y, edge.Direction, out var target), Is.True);
                Assert.That(target, Is.EqualTo(edge.Last));
                Assert.That(topology.TryResolveOffset(target.x, target.y, -edge.Direction, out var returned), Is.True);
                Assert.That(returned, Is.EqualTo(edge.First));
                if (edge.First != edge.Last)
                {
                    Assert.That(topology.GetDistance(edge.First.x, edge.First.y, target.x, target.y), Is.EqualTo(1));
                }
            }
        }

        [TestCase(0, 1), TestCase(3, 3)]
        public void LegacyPreviewDraftHealthMigratesWithoutChangingPositiveValues(int serializedHealth, int expectedHealth)
        {
            var root = new GameObject("Legacy preview draft test");
            root.SetActive(false);
            try
            {
                var preview = root.AddComponent<SpeciesSimulationPreview>();
                var draftMap = (System.Collections.IDictionary)typeof(SpeciesSimulationPreview)
                    .GetMethod("CreateRuleDrafts", PrivateInstance)
                    .Invoke(preview, new object[] { SpeciesRuleDefaults.Create() });
                foreach (var draft in draftMap.Values)
                    draft.GetType().GetField("MaximumHealth").SetValue(draft, serializedHealth);
                typeof(SpeciesSimulationPreview).GetField("ruleDrafts", PrivateInstance).SetValue(preview, draftMap);
                var rules = (IReadOnlyDictionary<SpeciesId, SpeciesRules>)typeof(SpeciesSimulationPreview)
                    .GetMethod("CreateRulesFromDrafts", PrivateInstance).Invoke(preview, null);
                foreach (var rule in rules.Values) Assert.That(rule.MaximumHealth, Is.EqualTo(expectedHealth));
            }
            finally { Object.DestroyImmediate(root); }
        }

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
        public void RabbitGenomeExperimentFlagPreservesAllocationOrderAndAllowsPhases()
        {
            var runner = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType("SaltyGame.EditorTools.CellularSimulationExperimentRunner"))
                .First(type => type != null);
            var optionsType = runner.GetNestedType("CommandLineOptions", BindingFlags.NonPublic);
            var parse = optionsType.GetMethod("Parse");
            var options = parse.Invoke(null, new object[] {
                new[] { "-rabbitGenomeNodeIds", "fertile-droppings, poc.rabbit.efficient-digestion",
                    "-phaseLengthTicks", "2", "-phaseUpgradeSchedule", "none;none" } });
            Assert.That((string[])optionsType.GetProperty("RabbitGenomeNodeIds")
                .GetValue(options), Is.EqualTo(new[] { "fertile-droppings", "poc.rabbit.efficient-digestion" }));

            foreach (var arguments in new[] {
                new[] { "-rabbitGenomeNodeIds", "fertile-droppings,fertile-droppings" },
                new[] { "-rabbitGenomeNodeIds", "fertile-droppings,,poc.rabbit.long-stride" },
                new[] { "-rabbitGenomeNodeIds", "fertile-droppings", "-attackOpportunityMode",
                    "paired-lockstep-diagnostic" },
            })
            {
                var error = Assert.Throws<TargetInvocationException>(() =>
                    parse.Invoke(null, new object[] { arguments }));
                Assert.That(error.InnerException, Is.TypeOf<ArgumentException>());
            }
        }

        const string PocGenomePath = "Assets/Data/TestData/Genome/RabbitSelectionPOC/RabbitSelectionPOC.asset";
        static Type ExperimentRunner => AppDomain.CurrentDomain.GetAssemblies()
            .Select(assembly => assembly.GetType("SaltyGame.EditorTools.CellularSimulationExperimentRunner"))
            .First(type => type != null);

        static CellularSimData GenomeFixtureData()
        {
            var data = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(ScenarioPath).CreateRuntimeData();
            var populations = data.SpeciesRules.Keys.ToDictionary(id => id, _ => 0);
            populations[data.SpeciesRules.First(entry => entry.Value.IsPlant).Key] = 20;
            populations[GenomeContract.RabbitSpeciesId] = 4;
            return data.WithGridSizeAndStartingPopulations(8, 8, populations).WithRunTicks(4, 0.1f);
        }

        static object FrozenGenomeFixture(CellularSimData data = null)
        {
            var snapshotType = ExperimentRunner.GetNestedType("PortableSnapshot");
            var snapshot = snapshotType.GetMethod("Create").Invoke(null, new object[] { data ?? GenomeFixtureData() });
            var map = AssetDatabase.LoadAssetAtPath<SpeciesGenomeMapAsset>(PocGenomePath).CreateSnapshot();
            var catalogType = ExperimentRunner.GetNestedType("GenomeCatalogData");
            snapshotType.GetField("rabbitGenomeCatalogPath").SetValue(snapshot, PocGenomePath);
            snapshotType.GetField("rabbitGenomeCatalog").SetValue(snapshot,
                catalogType.GetMethod("Create").Invoke(null, new object[] { map }));
            return snapshot;
        }

        static string[] GenomeArguments(string ids, params string[] additional)
        {
            return new[] { "-playerSpeciesId", "rabbit", "-rabbitGenomeCatalogPath", PocGenomePath,
                "-rabbitGenomeNodeIds", ids }.Concat(additional).ToArray();
        }

        static GenomeReportProbe GenomeHeader(object snapshot, string[] arguments)
        {
            var json = (string)ExperimentRunner.GetMethod("PortableReportHeader")
                .Invoke(null, new object[] { snapshot, arguments, 10100, 1 });
            return JsonUtility.FromJson<GenomeReportProbe>(json);
        }

        [Test]
        public void RabbitGenomeReportMatchesDesktopRulesAndRecordsResolvedValuesBeforeMutations()
        {
            var snapshot = FrozenGenomeFixture();
            var arguments = GenomeArguments("fertile-droppings,poc.rabbit.efficient-digestion",
                "-upgradeId", SpeciesUpgradeCatalog.EfficientDigestionId);
            var report = GenomeHeader(snapshot, arguments);
            var data = GenomeFixtureData();
            var catalog = new GenomeCatalogSnapshot(new[]
            {
                AssetDatabase.LoadAssetAtPath<SpeciesGenomeMapAsset>(PocGenomePath).CreateSnapshot(),
            });
            var profile = new ProfileSessionSnapshot(true, "fixture", "Fixture", new[]
            {
                new SpeciesGenomeProfile(GenomeContract.RabbitSpeciesId,
                    new[] { "fertile-droppings", "poc.rabbit.efficient-digestion" },
                    new[] { "fertile-droppings", "poc.rabbit.efficient-digestion" }),
            });
            var genome = profile.CreateGenomeSnapshot(catalog).IncludeSpecies(data.SpeciesRules.Keys);
            var effective = data;
            foreach (var entry in genome.ApplyToNaturalRules(data.SpeciesRules))
            {
                effective = effective.WithSpeciesRules(entry.Key, entry.Value);
            }
            effective = effective.WithSpeciesRules(GenomeContract.RabbitSpeciesId,
                SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.EfficientDigestionId)
                    .Apply(effective.SpeciesRules[GenomeContract.RabbitSpeciesId]));
            Assert.That(report.schemaVersion, Is.EqualTo(42));
            Assert.That(report.genomeFingerprint, Is.EqualTo(genome.Fingerprint));
            Assert.That(report.rulesetFingerprint, Is.EqualTo(effective.Fingerprint));
            Assert.That(report.genomeInput.fingerprint, Is.EqualTo(genome.Fingerprint));
            Assert.That(report.genomeInput.catalogFingerprint, Is.EqualTo(catalog.Fingerprint));
            Assert.That(report.genomeInput.capacityUsed, Is.EqualTo(2));
            Assert.That(report.genomeInput.capacityLimit, Is.EqualTo(8));
            Assert.That(report.genomeInput.orderedNodeIds, Is.EqualTo(new[] { "fertile-droppings", "poc.rabbit.efficient-digestion" }));
            Assert.That(report.genomeInput.nodes[1].modifiers[0].id, Is.EqualTo(SpeciesAttributeIds.DigestionEnergyBonus));
            Assert.That(report.genomeInput.nodes[1].modifiers[0].value, Is.EqualTo(0.25f));
            Assert.That(report.genomeInput.fertileDroppings.droppingScentDurationTicks, Is.EqualTo(40));
            Assert.That(report.genomeInput.fertileDroppings.sniffedTargetRange, Is.EqualTo(36));
        }

        [TestCase("unknown")]
        [TestCase("poc.rabbit.efficient-digestion")]
        [TestCase("poc.rabbit.efficient-digestion,fertile-droppings")]
        [TestCase("poc.rabbit.tough-hide,poc.rabbit.burrows-preview")]
        public void RabbitGenomeResearchRejectsUnknownMissingPrerequisitePreviewAndOverCapacityBuilds(string ids)
        {
            var error = Assert.Throws<TargetInvocationException>(() => GenomeHeader(FrozenGenomeFixture(), GenomeArguments(ids)));
            Assert.That(error.InnerException, Is.InstanceOf<ArgumentException>().Or.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public void RabbitGenomeScheduledLaunchMutationMatchesOneDesktopApplication()
        {
            var snapshot = FrozenGenomeFixture();
            var arguments = GenomeArguments("fertile-droppings,poc.rabbit.efficient-digestion",
                "-phaseLengthTicks", "2", "-phaseUpgradeSchedule", "faster-movement;faster-movement");
            var natural = GenomeFixtureData();
            var map = AssetDatabase.LoadAssetAtPath<SpeciesGenomeMapAsset>(PocGenomePath).CreateSnapshot();
            var genome = new GenomeSimulationSnapshot(new[]
            {
                new SpeciesGenomeSnapshot(GenomeContract.RabbitSpeciesId,
                    new[] { "fertile-droppings", "poc.rabbit.efficient-digestion" }),
            }).Resolve(new GenomeCatalogSnapshot(new[] { map })).IncludeSpecies(natural.SpeciesRules.Keys);
            var data = natural;
            foreach (var entry in genome.ApplyToNaturalRules(natural.SpeciesRules))
            {
                data = data.WithSpeciesRules(entry.Key, entry.Value);
            }
            var mutation = SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.FasterMovementId);
            data = data.WithSpeciesRules(GenomeContract.RabbitSpeciesId, mutation.Apply(data.SpeciesRules[GenomeContract.RabbitSpeciesId]));
            var run = new SimulationRunState(SpeciesInitialGridFactory.Create(data, 10100), GenomeContract.RabbitSpeciesId,
                10100, data.RunDurationSeconds, data.RunTicks, genome);
            run.ConfigureContinuousPhases(2);
            var runner = new SpeciesSimulationRunner(run, data);
            while (runner.AdvanceOneTick())
            {
                if (run.Status == SimulationRunStatus.AwaitingDecision)
                {
                    Assert.That(run.ContinueWithoutUpgrade(), Is.True);
                }
            }
            var expected = (string)ExperimentRunner.GetMethod("PortableStateDigest", BindingFlags.Static | BindingFlags.NonPublic)
                .Invoke(null, new object[] { run.Cells });
            var actualJson = (string)ExperimentRunner.GetMethod("RunPortableSeed")
                .Invoke(null, new object[] { snapshot, arguments, 10100, false, null });
            Assert.That(JsonUtility.FromJson<GenomeRunProbe>(actualJson).finalStateDigest, Is.EqualTo(expected));
        }

        [Test]
        public void RabbitGenomePortableSnapshotRoundTripsAndDetectsCatalogCorruption()
        {
            var snapshot = FrozenGenomeFixture();
            var json = JsonUtility.ToJson(snapshot);
            var restored = JsonUtility.FromJson(json, snapshot.GetType());
            var arguments = GenomeArguments("fertile-droppings,poc.rabbit.efficient-digestion");
            Assert.That(GenomeHeader(restored, arguments).genomeFingerprint, Is.EqualTo(GenomeHeader(snapshot, arguments).genomeFingerprint));
            var catalog = snapshot.GetType().GetField("rabbitGenomeCatalog").GetValue(restored);
            var nodes = (Array)catalog.GetType().GetField("nodes").GetValue(catalog);
            var node = nodes.GetValue(1);
            var modifiers = (Array)node.GetType().GetField("modifiers").GetValue(node);
            var modifier = modifiers.GetValue(0);
            modifier.GetType().GetField("value").SetValue(modifier, 999f);
            var error = Assert.Throws<TargetInvocationException>(() => GenomeHeader(restored, arguments));
            Assert.That(error.InnerException, Is.TypeOf<InvalidOperationException>());
            Assert.That(error.InnerException.Message, Does.Contain("fingerprint"));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void RabbitGenomeRemainsFrozenThroughScheduledPhasesAndMutationPolicies(bool policy)
        {
            var snapshot = FrozenGenomeFixture();
            var arguments = policy
                ? GenomeArguments("fertile-droppings,poc.rabbit.efficient-digestion", "-phaseLengthTicks", "2", "-mutationPolicy", "gardeners")
                : GenomeArguments("fertile-droppings,poc.rabbit.efficient-digestion", "-phaseLengthTicks", "2", "-phaseUpgradeSchedule", "none;none");
            var header = GenomeHeader(snapshot, arguments);
            var method = ExperimentRunner.GetMethod("RunPortableSeed");
            var compact = (string)method.Invoke(null, new object[] { snapshot, arguments, 10100, false, null });
            var detailed = (string)method.Invoke(null, new object[] { snapshot, arguments, 10100, true, null });
            var first = JsonUtility.FromJson<GenomeRunProbe>(compact);
            var second = JsonUtility.FromJson<GenomeRunProbe>(detailed);
            Assert.That(first.genomeFingerprint, Is.EqualTo(header.genomeFingerprint));
            Assert.That(second.genomeFingerprint, Is.EqualTo(header.genomeFingerprint));
            Assert.That(first.finalStateDigest, Is.EqualTo(second.finalStateDigest));
            Assert.That(first.ticks, Is.EqualTo(policy ? 12 : 4));
        }

        [TestCase("0")]
        [TestCase("-2")]
        [TestCase("3")]
        [TestCase("2,2")]
        [TestCase("4,2")]
        [TestCase("6")]
        [TestCase("2,")]
        public void FoxIntroductionScheduleRejectsInvalidBoundaries(string ticks)
        {
            var parse = ExperimentRunner.GetNestedType("CommandLineOptions", BindingFlags.NonPublic).GetMethod("Parse");
            var error = Assert.Throws<TargetInvocationException>(() => parse.Invoke(null, new object[] {
                new[] { "-phaseLengthTicks", "2", "-phaseUpgradeSchedule", "none;none;none", "-foxIntroductionTicks", ticks } }));
            Assert.That(error.InnerException, Is.TypeOf<ArgumentException>());
        }

        [Test]
        public void FoxIntroductionsRejectMissingPhasesAndPairedModes()
        {
            var parse = ExperimentRunner.GetNestedType("CommandLineOptions", BindingFlags.NonPublic).GetMethod("Parse");
            var error = Assert.Throws<TargetInvocationException>(() => parse.Invoke(null, new object[] {
                new[] { "-foxIntroductionTicks", "2" } }));
            Assert.That(error.InnerException, Is.TypeOf<ArgumentException>());
            foreach (var mode in new[] { "paired-lockstep-diagnostic", "PAIRED-LOCKSTEP-DIAGNOSTIC" })
            {
                var paired = Assert.Throws<TargetInvocationException>(() => parse.Invoke(null, new object[] {
                    new[] { "-phaseLengthTicks", "2", "-phaseUpgradeSchedule", "none;none;none",
                        "-foxIntroductionTicks", "2", "-attackOpportunityMode", mode } }));
                Assert.That(paired.InnerException, Is.TypeOf<ArgumentException>());
            }
        }

        [Test]
        public void PopulationGenomeLaunchAddsFiveOnceAndPortableReportPreservesCapacity()
        {
            var snapshot = FrozenGenomeFixture();
            var args = GenomeArguments("poc.rabbit.starting-population,poc.rabbit.population-pool",
                "-phaseLengthTicks", "2", "-phaseUpgradeSchedule", "none;none");
            var header = GenomeHeader(snapshot, args);
            Assert.That(header.genomeInput.orderedNodeIds, Is.EqualTo(new[] { "poc.rabbit.starting-population", "poc.rabbit.population-pool" }));
            var first = JsonUtility.FromJson<IntroductionRunProbe>((string)ExperimentRunner.GetMethod("RunPortableSeed")
                .Invoke(null, new object[] { snapshot, args, 10100, true, null }));
            var repeated = JsonUtility.FromJson<IntroductionRunProbe>((string)ExperimentRunner.GetMethod("RunPortableSeed")
                .Invoke(null, new object[] { snapshot, args, 10100, true, null }));
            Assert.That(first.populationHistory[0].species.First(p => p.speciesId == "rabbit").population, Is.EqualTo(9));
            Assert.That(first.populationPoolCapacity, Is.EqualTo(60));
            Assert.That(first.finalStateDigest, Is.EqualTo(repeated.finalStateDigest));
            Assert.That(first.populationHistory.Last().species.First(p => p.speciesId == "rabbit").population, Is.EqualTo(9));
        }

        [Test]
        public void ResearchFeedingAndFoxIntroductionsAreRecordedAndReplayInBothDetailModes()
        {
            var snapshot = FrozenGenomeFixture();
            var args = new[] { "-playerSpeciesId", "rabbit", "-phaseLengthTicks", "2",
                "-phaseUpgradeSchedule", "none;none;none", "-foxIntroductionTicks", "2,4",
                "-speciesStats", "rabbit:energy.forage-threshold=25" };
            var header = GenomeHeader(snapshot, args);
            Assert.That(header.resolvedRabbitForageThreshold, Is.EqualTo(25));
            Assert.That(header.researchSpeciesStats, Is.EqualTo("rabbit:energy.forage-threshold=25"));
            Assert.That(header.foxIntroductionTicks, Is.EqualTo(new[] { 2, 4 }));
            var method = ExperimentRunner.GetMethod("RunPortableSeed");
            var detailed = JsonUtility.FromJson<IntroductionRunProbe>((string)method.Invoke(null,
                new object[] { snapshot, args, 10100, true, null }));
            var compact = JsonUtility.FromJson<IntroductionRunProbe>((string)method.Invoke(null,
                new object[] { snapshot, args, 10100, false, null }));
            Assert.That(detailed.ticks, Is.EqualTo(6));
            Assert.That(detailed.finalStateDigest, Is.EqualTo(compact.finalStateDigest));
            Assert.That(detailed.researchMilestones.firstFeedingTick, Is.EqualTo(compact.researchMilestones.firstFeedingTick));
            Assert.That(detailed.researchMilestones.firstMatingTick, Is.EqualTo(compact.researchMilestones.firstMatingTick));
            Assert.That(detailed.researchMilestones.firstBirthTick, Is.EqualTo(-1));
            Assert.That(detailed.populationIntroductions.Length, Is.EqualTo(2));
            Assert.That(detailed.boundaryPopulationAdded.First(p => p.speciesId == "fox").population, Is.EqualTo(2));
            for (var index = 0; index < 2; index++)
            {
                var arrival = detailed.populationIntroductions[index];
                Assert.That(arrival.tick, Is.EqualTo((index + 1) * 2));
                Assert.That(arrival.speciesId, Is.EqualTo("fox"));
                Assert.That(arrival.amount, Is.EqualTo(1));
                Assert.That(arrival.populationAfter, Is.EqualTo(arrival.populationBefore + 1));
                Assert.That(arrival.cumulativeAdded, Is.EqualTo(index + 1));
                Assert.That(compact.populationIntroductions[index].tick, Is.EqualTo(arrival.tick));
            }
            Assert.That(detailed.populationHistory[1].species.First(p => p.speciesId == "fox").population, Is.Zero);
            Assert.That(detailed.populationHistory[2].species.First(p => p.speciesId == "fox").population, Is.EqualTo(1));
            Assert.That(detailed.populationHistory[4].species.First(p => p.speciesId == "fox").population, Is.EqualTo(2));
            Assert.That(detailed.populationHistory.Last().species.First(p => p.speciesId == "fox").population, Is.EqualTo(2));
        }

        [Test]
        public void EmptyResearchPhasesPreserveUnscheduledWorldState()
        {
            var snapshot = FrozenGenomeFixture();
            var method = ExperimentRunner.GetMethod("RunPortableSeed");
            var plain = new[] { "-playerSpeciesId", "rabbit", "-runTicks", "6" };
            var phased = new[] { "-playerSpeciesId", "rabbit", "-phaseLengthTicks", "2", "-phaseUpgradeSchedule", "none;none;none" };
            var first = JsonUtility.FromJson<GenomeRunProbe>((string)method.Invoke(null, new object[] { snapshot, plain, 10100, true, null }));
            var second = JsonUtility.FromJson<GenomeRunProbe>((string)method.Invoke(null, new object[] { snapshot, phased, 10100, true, null }));
            Assert.That(second.finalStateDigest, Is.EqualTo(first.finalStateDigest));
        }

        [Test]
        public void ScheduledEcologyObservesAfterRabbitExtinctionWhileGameplayStillStops()
        {
            var data = GenomeFixtureData();
            data = data.WithGridSizeAndStartingPopulations(8, 8,
                data.SpeciesRules.Keys.ToDictionary(id => id, _ => 0)).WithRunTicks(6, 0.1f);
            var run = new SimulationRunState(SpeciesInitialGridFactory.Create(data, 10100),
                GenomeContract.RabbitSpeciesId, 10100, data.RunDurationSeconds, 6);
            run.ConfigureContinuousPhases(2);
            var gameplay = new SpeciesSimulationRunner(run, data);
            Assert.That(gameplay.AdvanceOneTick(), Is.True);
            Assert.That(run.Status, Is.EqualTo(SimulationRunStatus.Complete));
            Assert.That(run.Tick, Is.EqualTo(1));

            var args = new[] { "-playerSpeciesId", "rabbit", "-phaseLengthTicks", "2",
                "-phaseUpgradeSchedule", "none;none;none", "-foxIntroductionTicks", "2,4" };
            var research = JsonUtility.FromJson<IntroductionRunProbe>((string)ExperimentRunner.GetMethod("RunPortableSeed")
                .Invoke(null, new object[] { FrozenGenomeFixture(data), args, 10100, true, null }));
            Assert.That(research.ticks, Is.EqualTo(6));
            Assert.That(research.populationIntroductions.Length, Is.EqualTo(2));
            Assert.That(research.populationHistory.Length, Is.EqualTo(7));
            Assert.That(research.populationHistory.All(p => p.species.First(s => s.speciesId == "rabbit").population == 0), Is.True);
            Assert.That(research.researchMilestones.firstBirthTick, Is.EqualTo(-1));
        }

        [Test]
        public void FoxIntroductionFailureStopsInsteadOfSkippingArrival()
        {
            var data = GenomeFixtureData();
            data = data.WithGridSizeAndStartingPopulations(1, 1,
                data.SpeciesRules.Keys.ToDictionary(id => id, id => id == GenomeContract.RabbitSpeciesId ? 1 : 0));
            var snapshot = FrozenGenomeFixture(data);
            var args = new[] { "-playerSpeciesId", "rabbit", "-phaseLengthTicks", "2",
                "-phaseUpgradeSchedule", "none;none", "-foxIntroductionTicks", "2" };
            var error = Assert.Throws<TargetInvocationException>(() => ExperimentRunner.GetMethod("RunPortableSeed")
                .Invoke(null, new object[] { snapshot, args, 10100, true, null }));
            Assert.That(error.InnerException, Is.TypeOf<InvalidOperationException>());
            Assert.That(error.InnerException.Message, Does.Contain("Could not introduce Fox"));
        }

        [Serializable] class IntroductionRunProbe
        {
            public int ticks;
            public int populationPoolCapacity;
            public PopulationSpeciesProbe[] boundaryPopulationAdded;
            public string finalStateDigest;
            public IntroductionProbe[] populationIntroductions;
            public PopulationProbe[] populationHistory;
            public MilestonesProbe researchMilestones;
        }
        [Serializable] class MilestonesProbe { public int firstFeedingTick, firstMatingTick, firstBirthTick; }
        [Serializable] class IntroductionProbe { public int tick, amount, populationBefore, populationAfter, cumulativeAdded; public string speciesId; }
        [Serializable] class PopulationProbe { public PopulationSpeciesProbe[] species; }
        [Serializable] class PopulationSpeciesProbe { public string speciesId; public int population; }

        [Serializable] class GenomeReportProbe
        {
            public int schemaVersion;
            public int resolvedRabbitForageThreshold;
            public string researchSpeciesStats;
            public int[] foxIntroductionTicks;
            public string genomeFingerprint, rulesetFingerprint;
            public GenomeInputProbe genomeInput;
        }
        [Serializable] class GenomeInputProbe
        {
            public string fingerprint, catalogFingerprint;
            public int capacityUsed, capacityLimit;
            public string[] orderedNodeIds;
            public GenomeNodeProbe[] nodes;
            public ScentProbe fertileDroppings;
        }
        [Serializable] class GenomeNodeProbe { public GenomeModifierProbe[] modifiers; }
        [Serializable] class GenomeModifierProbe { public string id; public float value; }
        [Serializable] class ScentProbe { public int droppingScentDurationTicks, sniffedTargetRange; }
        [Serializable] class GenomeRunProbe { public string genomeFingerprint, finalStateDigest; public int ticks; }

        [Test]
        public void ForestEdgeWrappingReachesRunsAndScenarioSwitches()
        {
            var forest = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(ScenarioPath);
            var bounded = Object.Instantiate(forest);
            var serialized = new SerializedObject(bounded);
            serialized.FindProperty("wrapEdges").boolValue = false;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            var previewObject = new GameObject("Wrapping Scenario Test");
            previewObject.SetActive(false);
            try
            {
                Assert.That(forest.CreateRuntimeData().WrapEdges, Is.True);
                Assert.That(bounded.CreateRuntimeData().WrapEdges, Is.False);
                Assert.That(ApplyOverrides().WrapEdges, Is.True);
                Assert.That(ApplyOverrides("-wrapEdges", "false").WrapEdges, Is.False);

                var preview = previewObject.AddComponent<SpeciesSimulationPreview>();
                preview.ConfigureScenarioOptions(new[] { forest, bounded });
                typeof(SpeciesSimulationPreview).GetMethod("Awake", PrivateInstance).Invoke(preview, null);
                Assert.That(preview.WrapEdges, Is.True);
                Assert.That(preview.Run.Cells.WrapEdges, Is.True);
                Assert.That(preview.TrySelectScenario(1, out var message), Is.True, message);
                Assert.That(preview.Run.Cells.WrapEdges, Is.False);
                Assert.That(preview.TrySelectScenario(0, out message), Is.True, message);
                Assert.That(preview.Run.Cells.WrapEdges, Is.True);
            }
            finally
            {
                Object.DestroyImmediate(previewObject);
                Object.DestroyImmediate(bounded);
            }
        }

        [Test]
        public void WrappingOverridesSurviveDataCopiesAndCheckpointsWithMatchedInitialCells()
        {
            var bounded = ApplyOverrides("-wrapEdges", "false", "-gridWidth", "36", "-gridHeight", "20", "-runTicks", "600",
                "-stepIntervalSeconds", "0.1", "-startingPopulations", "plant=400,rabbit=15,fox=10");
            var wrapped = ApplyOverrides("-wrapEdges", "true", "-gridWidth", "36", "-gridHeight", "20",
                "-runTicks", "600", "-stepIntervalSeconds", "0.1", "-startingPopulations", "plant=400,rabbit=15,fox=10");
            Assert.That(bounded.WrapEdges, Is.False);
            Assert.That(wrapped.WrapEdges, Is.True);
            Assert.That(wrapped.Fingerprint, Is.Not.EqualTo(bounded.Fingerprint));
            Assert.That(wrapped.Copy().WithRunTicks(600, 0.1f).WithGridSize(36, 20)
                .WithSpeciesRules(new SpeciesId("rabbit"), wrapped.SpeciesRules[new SpeciesId("rabbit")]).WrapEdges, Is.True);
            var initial = SpeciesInitialGridFactory.Create(wrapped, 10100);
            var original = SpeciesInitialGridFactory.Create(bounded, 10100);
            for (var y = 0; y < initial.Height; y++)
                for (var x = 0; x < initial.Width; x++)
                {
                    Assert.That(initial.GetCell(x, y).SpeciesId, Is.EqualTo(original.GetCell(x, y).SpeciesId));
                    Assert.That(initial.GetCell(x, y).TerrainId, Is.EqualTo(original.GetCell(x, y).TerrainId));
                }
            var run = new SimulationRunState(initial, new SpeciesId("rabbit"), 10100, wrapped.RunDurationSeconds);
            var runner = new SpeciesSimulationRunner(run, wrapped);
            run.ConfigureContinuousPhases(1);
            runner.Start();
            Assert.That(runner.AdvanceOneTick(), Is.True);
            Assert.That(SpeciesSimulationRunner.RestoreCheckpoint(runner.CreateCheckpoint(), wrapped).Run.Cells.WrapEdges, Is.True);
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
                Assert.That(preview.PlayerSpecies.Value, Is.EqualTo("rabbit"));
                Assert.That(preview.WrapEdges, Is.False);
                Assert.That(preview.TryApplyWrapEdges(true, out _), Is.True);
                Assert.That(preview.Run.Cells.WrapEdges, Is.True);
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
                Assert.That(initial.GetCount(new SpeciesId("rabbit")), Is.EqualTo(15));
                Assert.That(initial.GetCount(new SpeciesId("fox")), Is.EqualTo(10));
                preview.PauseSimulation();
                Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
                Assert.That(preview.Run.Tick, Is.EqualTo(1));
                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Paused));
                Assert.That(apply.Invoke(null, arguments), Is.False);
                Assert.That(preview.WrapEdges, Is.True);
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
        public void PreviewBuildsSimulationDataWithScenarioWrapping()
        {
            var scenario = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(ScenarioPath);
            var previewObject = new GameObject("Wrapping Preview Test");
            previewObject.SetActive(false);
            try
            {
                var preview = previewObject.AddComponent<SpeciesSimulationPreview>();
                preview.ConfigureScenarioOptions(new[] { scenario });
                typeof(SpeciesSimulationPreview).GetMethod("Awake", PrivateInstance).Invoke(preview, null);

                var data = (CellularSimData)typeof(SpeciesSimulationPreview)
                    .GetMethod("CreateSimulationData", PrivateInstance)
                    .Invoke(preview, null);

                Assert.That(preview.WrapEdges, Is.True);
                Assert.That(data.WrapEdges, Is.True);
            }
            finally
            {
                Object.DestroyImmediate(previewObject);
            }
        }

        [Test]
        public void PreviewWithoutAnAuthoredScenarioDefaultsToBoundedSimulationData()
        {
            var previewObject = new GameObject("Bounded Preview Test");
            previewObject.SetActive(false);
            try
            {
                var preview = previewObject.AddComponent<SpeciesSimulationPreview>();
                typeof(SpeciesSimulationPreview).GetMethod("Awake", PrivateInstance).Invoke(preview, null);

                var data = (CellularSimData)typeof(SpeciesSimulationPreview)
                    .GetMethod("CreateSimulationData", PrivateInstance)
                    .Invoke(preview, null);

                Assert.That(preview.WrapEdges, Is.False);
                Assert.That(data.WrapEdges, Is.False);
            }
            finally
            {
                Object.DestroyImmediate(previewObject);
            }
        }

        [Test]
        public void ScheduledAvoidanceOptionsFollowTheActiveSnapshots()
        {
            var runner = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType("SaltyGame.EditorTools.CellularSimulationExperimentRunner"))
                .First(type => type != null);
            var resolve = runner.GetMethod("GetScheduledExperimentalOptions", BindingFlags.Static | BindingFlags.NonPublic);
            var options = new SpeciesExperimentalOptions(SpeciesExperimentalOptions.BevExperimentalFeaturesId, 3, 0f, true);
            var rabbit = new SpeciesId("rabbit");
            var avoidance = SpeciesUpgradeCatalog.Create("threat-exposure").CreateSnapshot(rabbit);
            foreach (var level in new[] { 0, 1, 2, 0 })
            {
                var snapshots = Enumerable.Repeat(avoidance, level).ToArray();
                var actual = (SpeciesExperimentalOptions)resolve.Invoke(null, new object[] { options, snapshots, rabbit });
                Assert.That(actual.PreContactAvoidanceChance, Is.EqualTo(level * 0.08f));
                Assert.That(actual.FeatureId, Is.EqualTo(options.FeatureId));
                Assert.That(actual.FoxAttackCooldownTicks, Is.EqualTo(3));
                Assert.That(actual.CoupledSpeciesResponsesEnabled, Is.True);
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
            var result = (CellularSimData)apply.Invoke(null, new object[] { data, "rabbit", ids, 0f });
            var rabbit = new SpeciesId("rabbit");
            var progression = new SpeciesProgression(new SpeciesDefinition(rabbit, data.SpeciesRules[rabbit]));
            foreach (var id in ids) Assert.That(progression.TryApplyFreeUpgrade(SpeciesUpgradeCatalog.Create(id)), Is.True);
            Assert.That(result.SpeciesRules[rabbit].MovementSpeed, Is.EqualTo(progression.CurrentRules.MovementSpeed));
            Assert.That(result.SpeciesRules[rabbit].FleeMovementSpeedBonus, Is.EqualTo(data.SpeciesRules[rabbit].FleeMovementSpeedBonus));
            var createSnapshots = runner.GetMethod("CreateLegacyUpgradeSnapshots", BindingFlags.Static | BindingFlags.NonPublic);
            var snapshots = (IReadOnlyList<SpeciesUpgradeSnapshot>)createSnapshots.Invoke(null, new object[] { ids, "rabbit", 0f });
            var snapshotApply = runner.GetMethod("ApplySnapshotLoadout", BindingFlags.Static | BindingFlags.NonPublic);
            var actualResult = (CellularSimData)snapshotApply.Invoke(null, new object[] { data, "rabbit", snapshots });
            Assert.That(actualResult.SpeciesRules[rabbit].MovementSpeed, Is.EqualTo(progression.CurrentRules.MovementSpeed));
            Assert.That(actualResult.SpeciesRules[rabbit].FleeMovementSpeedBonus, Is.EqualTo(data.SpeciesRules[rabbit].FleeMovementSpeedBonus));
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
            Assert.Throws<TargetInvocationException>(() => createSnapshots.Invoke(null, new object[] { invalid, "rabbit", 0f }));
            Assert.Throws<TargetInvocationException>(() => apply.Invoke(null, new object[] { data, "rabbit", invalid, 0f }));
            Assert.Throws<TargetInvocationException>(() => apply.Invoke(null, new object[] { data, "rabbit", new[] { "threat-exposure" }, 0.75f }));
        }

        [Test]
        public void ScheduledAvoidanceOptionsFollowEachPhaseLoadout()
        {
            var runner = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType("SaltyGame.EditorTools.CellularSimulationExperimentRunner"))
                .First(type => type != null);
            var create = runner.GetMethod("GetScheduledExperimentalOptions", BindingFlags.Static | BindingFlags.NonPublic);
            var baseOptions = new SpeciesExperimentalOptions(SpeciesExperimentalOptions.BevExperimentalFeaturesId);
            var avoidance = SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.ThreatExposureId)
                .CreateSnapshot(new SpeciesId("rabbit"));
            SpeciesExperimentalOptions Resolve(params SpeciesUpgradeSnapshot[] loadout) =>
                (SpeciesExperimentalOptions)create.Invoke(null, new object[] { baseOptions, loadout, new SpeciesId("rabbit") });

            Assert.That(Resolve().PreContactAvoidanceChance, Is.Zero);
            Assert.That(Resolve(avoidance).PreContactAvoidanceChance, Is.EqualTo(0.08f).Within(0.00001f));
            Assert.That(Resolve(avoidance, avoidance).PreContactAvoidanceChance, Is.EqualTo(0.16f).Within(0.00001f));
            Assert.That(Resolve().PreContactAvoidanceChance, Is.Zero);
        }

        [Test]
        public void WarrenBatchSnapshotsAndAuthoringCopiesPreserveCrowdingReduction()
        {
            var data = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(ScenarioPath).CreateRuntimeData();
            var rabbit = new SpeciesId("rabbit");
            var runner = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType("SaltyGame.EditorTools.CellularSimulationExperimentRunner"))
                .First(type => type != null);
            var ids = new[] { "tough-hide", "crowding-tolerance", "tough-hide", "crowding-tolerance", "tough-hide" };
            var snapshots = (IReadOnlyList<SpeciesUpgradeSnapshot>)runner
                .GetMethod("CreateLegacyUpgradeSnapshots", BindingFlags.Static | BindingFlags.NonPublic)
                .Invoke(null, new object[] { ids, "rabbit", 0f });
            var result = (CellularSimData)runner.GetMethod("ApplySnapshotLoadout", BindingFlags.Static | BindingFlags.NonPublic)
                .Invoke(null, new object[] { data, "rabbit", snapshots });
            var rules = result.SpeciesRules[rabbit];
            Assert.That(rules.CrowdingEnergyReduction, Is.EqualTo(0.2f));
            Assert.That(rules.MaximumHealth, Is.EqualTo(data.SpeciesRules[rabbit].MaximumHealth + 6));
            Assert.That(rules.CrowdingTolerance, Is.EqualTo(data.SpeciesRules[rabbit].CrowdingTolerance));
            Assert.That(result.Fingerprint, Is.Not.EqualTo(data.Fingerprint));
            var prediction = SaltyGame.EditorTools.SpeciesUpgradePredictionInputAdapter.CreateInput(snapshots);
            Assert.That(prediction.upgrades[1].modifiers[0].attributeId, Is.EqualTo(SpeciesAttributeIds.CrowdingEnergyReduction));
            Assert.That(prediction.upgrades[1].modifiers[0].signedValue, Is.EqualTo(0.1f));
            var metrics = new SpeciesSimulationMetrics();
            var record = typeof(SpeciesSimulationMetrics).GetMethod("Record", PrivateInstance);
            var parameters = record.GetParameters();
            var arguments = parameters.Select(parameter => parameter.DefaultValue).ToArray();
            arguments[0] = rabbit;
            arguments[Array.FindIndex(parameters, parameter => parameter.Name == "crowdingMetabolismTicks")] = 9;
            arguments[Array.FindIndex(parameters, parameter => parameter.Name == "crowdingEnergyLost")] = 7;
            record.Invoke(metrics, arguments);
            var serialization = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType("SaltyGame.EditorTools.SimulationReportSerialization"))
                .First(type => type != null);
            var activityRecords = (Array)serialization.GetMethod("CreateActivity")
                .Invoke(null, new object[] { metrics, new[] { rabbit } });
            var activityRecord = activityRecords.GetValue(0);
            Assert.That(activityRecord.GetType().GetField("crowdingMetabolismTicks").GetValue(activityRecord), Is.EqualTo(9));
            Assert.That(activityRecord.GetType().GetField("crowdingEnergyLost").GetValue(activityRecord), Is.EqualTo(7));
            StringAssert.Contains("crowdingEnergyLost", JsonUtility.ToJson(activityRecord));
            var asset = ScriptableObject.CreateInstance<CellularSimDataAsset>();
            var authored = Object.Instantiate(AssetDatabase.LoadAssetAtPath<SpeciesDefinitionAsset>(
                "Assets/Data/ProductionData/CellularSimulation/Species/rabbit.asset"));
            try
            {
                var definition = typeof(CellularSimDataAsset).GetNestedType("SpeciesDefinition", BindingFlags.NonPublic);
                var entries = result.SpeciesRules.ToArray();
                var array = Array.CreateInstance(definition, entries.Length);
                for (var index = 0; index < entries.Length; index++)
                    array.SetValue(definition.GetMethod("From", BindingFlags.Static | BindingFlags.NonPublic)
                        .Invoke(null, new object[] { entries[index].Key, 0.1f, entries[index].Value }), index);
                typeof(CellularSimDataAsset).GetField("species", PrivateInstance).SetValue(asset, array);
                Assert.That(asset.CreateRuntimeData().SpeciesRules[rabbit].CrowdingEnergyReduction, Is.EqualTo(0.2f));
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
            var rabbit = new SpeciesId("rabbit");
            var ids = new[] { "efficient-digestion", "seed-dispersal", "efficient-digestion", "seed-dispersal", "efficient-digestion" };
            var runner = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType("SaltyGame.EditorTools.CellularSimulationExperimentRunner"))
                .First(type => type != null);
            var create = runner.GetMethod("CreateLegacyUpgradeSnapshots", BindingFlags.Static | BindingFlags.NonPublic);
            var snapshots = (IReadOnlyList<SpeciesUpgradeSnapshot>)create.Invoke(null, new object[] { ids, "rabbit", 0f });
            var result = (CellularSimData)runner.GetMethod("ApplySnapshotLoadout", BindingFlags.Static | BindingFlags.NonPublic)
                .Invoke(null, new object[] { data, "rabbit", snapshots });
            var progression = new SpeciesProgression(new SpeciesDefinition(rabbit, data.SpeciesRules[rabbit]));
            foreach (var id in ids) Assert.That(progression.TryApplyFreeUpgrade(SpeciesUpgradeCatalog.Create(id)), Is.True);
            Assert.That(result.SpeciesRules[rabbit].SeedDropChance, Is.EqualTo(data.SpeciesRules[rabbit].SeedDropChance + 0.02f).Within(0.000001f));
            Assert.That(result.SpeciesRules[rabbit].SeedDropChance, Is.EqualTo(progression.CurrentRules.SeedDropChance));
            Assert.That(result.SpeciesRules[rabbit].DigestionEnergyBonus, Is.EqualTo(progression.CurrentRules.DigestionEnergyBonus));
            Assert.That(result.Fingerprint, Is.Not.EqualTo(data.Fingerprint));
            var prediction = SaltyGame.EditorTools.SpeciesUpgradePredictionInputAdapter.CreateInput(snapshots);
            Assert.That(prediction.upgrades[1].modifiers[0].attributeId, Is.EqualTo(SpeciesAttributeIds.SeedDropChance));
            Assert.That(prediction.upgrades[1].modifiers[0].signedValue, Is.EqualTo(0.01f));
            Assert.Throws<TargetInvocationException>(() => create.Invoke(null,
                new object[] { Enumerable.Repeat("seed-dispersal", 11).ToArray(), "rabbit", 0f }));
            var metrics = new SpeciesSimulationMetrics();
            var record = typeof(SpeciesSimulationMetrics).GetMethod("Record", PrivateInstance);
            var parameters = record.GetParameters();
            var arguments = parameters.Select(parameter => parameter.DefaultValue).ToArray();
            arguments[0] = rabbit;
            arguments[Array.FindIndex(parameters, parameter => parameter.Name == "seedDropAttempts")] = 9;
            arguments[Array.FindIndex(parameters, parameter => parameter.Name == "seedDropSuccesses")] = 2;
            arguments[Array.FindIndex(parameters, parameter => parameter.Name == "seedDropFoodCreated")] = 6.5f;
            arguments[Array.FindIndex(parameters, parameter => parameter.Name == "seedDropReserveSpent")] = 2;
            record.Invoke(metrics, arguments);
            var serialization = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType("SaltyGame.EditorTools.SimulationReportSerialization"))
                .First(type => type != null);
            var records = (Array)serialization.GetMethod("CreateActivity").Invoke(null, new object[] { metrics, new[] { rabbit } });
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
                "-startingPopulations", "plant=40,rabbit=5,fox=2");
            Assert.That(data.Width, Is.EqualTo(10));
            Assert.That(data.Height, Is.EqualTo(10));
            Assert.That(data.StartingPopulations[new SpeciesId("plant")], Is.EqualTo(40));
            Assert.That(data.StartingPopulations[new SpeciesId("rabbit")], Is.EqualTo(5));
            Assert.That(data.StartingPopulations[new SpeciesId("fox")], Is.EqualTo(2));
            var authored = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(ScenarioPath).CreateRuntimeData();
            Assert.That(authored.StartingPopulations[new SpeciesId("plant")], Is.EqualTo(400));
            Assert.That(authored.StartingPopulations[new SpeciesId("rabbit")], Is.EqualTo(3));
            Assert.That(authored.StartingPopulations[new SpeciesId("fox")], Is.EqualTo(2));
        }

        [TestCase("rabbit=-1")]
        [TestCase("rabbit=5,rabbit=2")]
        [TestCase("unknown=1")]
        [TestCase("plant=101")]
        [TestCase("rabbit=abc")]
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
                serialized.FindProperty("species").GetArrayElementAtIndex(1).FindPropertyRelative("startingPopulation").intValue = 8;
                serialized.FindProperty("species").GetArrayElementAtIndex(2).FindPropertyRelative("startingPopulation").intValue = 12;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                var preview = previewObject.AddComponent<SpeciesSimulationPreview>();
                preview.ConfigureScenarioOptions(new[] { scenario });
                typeof(SpeciesSimulationPreview).GetMethod("Awake", PrivateInstance).Invoke(preview, null);
                typeof(SpeciesSimulationPreview).GetField("herbivoreStartingPopulation", PrivateInstance).SetValue(preview, 14);
                typeof(SpeciesSimulationPreview).GetField("carnivoreStartingPopulation", PrivateInstance).SetValue(preview, 13);
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
                Assert.That(preview.HerbivoreStartingPopulation, Is.EqualTo(explicitOverride ? 14 : 8));
                Assert.That(preview.CarnivoreStartingPopulation, Is.EqualTo(explicitOverride ? 13 : 12));
                var initial = preview.Run.PopulationHistory[0];
                Assert.That(initial.GetCount(new SpeciesId("rabbit")), Is.EqualTo(explicitOverride ? 14 : 8));
                Assert.That(initial.GetCount(new SpeciesId("fox")), Is.EqualTo(explicitOverride ? 13 : 12));
                preview.SaveCurrentSettingsAsDefault();
                typeof(SpeciesSimulationPreview).GetMethod("LoadSavedSettings", PrivateInstance).Invoke(preview, null);
                Assert.That(preview.HerbivoreStartingPopulation, Is.EqualTo(explicitOverride ? 14 : 8));
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
                var originalPopulations = scenario.CreateRuntimeData().StartingPopulations;
                var editedRabbits = checked(originalPopulations[new SpeciesId("rabbit")] + 5);
                var serialized = new SerializedObject(scenario);
                var population = serialized.FindProperty("species").GetArrayElementAtIndex(1).FindPropertyRelative("startingPopulation");
                population.intValue = editedRabbits;
                serialized.ApplyModifiedProperties();
                Undo.FlushUndoRecordObjects();
                Assert.That(scenario.CreateRuntimeData().StartingPopulations[new SpeciesId("rabbit")], Is.EqualTo(editedRabbits));
                Undo.PerformUndo();
                Assert.That(scenario.CreateRuntimeData().StartingPopulations[new SpeciesId("rabbit")],
                    Is.EqualTo(originalPopulations[new SpeciesId("rabbit")]));
                serialized.Update();
                population.intValue = editedRabbits;
                serialized.ApplyModifiedProperties();
                AssetDatabase.SaveAssetIfDirty(scenario);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                var loaded = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(path).CreateRuntimeData();
                Assert.That(loaded.StartingPopulations[new SpeciesId("rabbit")], Is.EqualTo(editedRabbits));
                Assert.That(loaded.StartingPopulations[new SpeciesId("fox")],
                    Is.EqualTo(originalPopulations[new SpeciesId("fox")]));
            }
            finally
            {
                Undo.ClearUndo(scenario);
                AssetDatabase.DeleteAsset(path);
            }
        }

        [TestCase(12, 12)]
        [TestCase(2000, 400)]
        public void FixedShapeUsesAuthoredDimensionsWithStalePreviewSettings(int savedPlants, int expectedPlants)
        {
            var keys = new[] { "v3", "v4", "v5" }
                .Select(version => "SaltyGame.SpeciesSimulationPreview.DefaultSettings." + version).ToArray();
            var previous = keys.ToDictionary(key => key, key => PlayerPrefs.HasKey(key) ? PlayerPrefs.GetString(key) : null);
            var scenario = Object.Instantiate(AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(ScenarioPath));
            var root = new GameObject("Fixed layout settings test");
            root.SetActive(false);
            try
            {
                var serialized = new SerializedObject(scenario);
                serialized.FindProperty("mapLayout.shape").intValue = (int)MapTopologyDefinition.FixedShape.Cross;
                serialized.FindProperty("height").intValue = 40;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                var data = scenario.CreateRuntimeData();
                foreach (var key in keys)
                {
                    PlayerPrefs.DeleteKey(key);
                }
                var preview = root.AddComponent<SpeciesSimulationPreview>();
                preview.ConfigureScenarioOptions(new[] { scenario });
                typeof(SpeciesSimulationPreview).GetMethod("Awake", PrivateInstance).Invoke(preview, null);
                typeof(SpeciesSimulationPreview).GetField("width", PrivateInstance).SetValue(preview, 36);
                typeof(SpeciesSimulationPreview).GetField("height", PrivateInstance).SetValue(preview, 20);
                typeof(SpeciesSimulationPreview).GetField("plantStartingPopulation", PrivateInstance).SetValue(preview, savedPlants);
                typeof(SpeciesSimulationPreview).GetField("startingPopulationOverrideEnabled", PrivateInstance).SetValue(preview, true);
                preview.SaveCurrentSettingsAsDefault();
                typeof(SpeciesSimulationPreview).GetMethod("ApplySelectedScenario", PrivateInstance).Invoke(preview, null);
                typeof(SpeciesSimulationPreview).GetMethod("LoadSavedSettings", PrivateInstance).Invoke(preview, null);
                preview.ResetToStart();

                Assert.That(preview.GridWidth, Is.EqualTo(data.Width));
                Assert.That(preview.GridHeight, Is.EqualTo(data.Height));
                Assert.That(preview.Run.Cells.Topology.Fingerprint, Is.EqualTo(data.Topology.Fingerprint));
                Assert.That(preview.PlantStartingPopulation, Is.EqualTo(expectedPlants));

                var resized = preview.TryApplyGlobalSettingsForTicksWithStartingPopulations(
                    "36", "20", "17", "0", "0", "20", "0.5", "0", "0", "0", false,
                    "12", "0", "0", out var resizeMessage);
                Assert.That(resized, Is.False);
                Assert.That(resizeMessage, Does.Contain("fixed dimensions"));
                var overCapacity = preview.TryApplyGlobalSettingsForTicksWithStartingPopulations(
                    "40", "40", "17", "0", "0", "20", "0.5", "0", "0", "0", false,
                    "1500", "0", "0", out var populationMessage);
                Assert.That(overCapacity, Is.False);
                Assert.That(populationMessage, Does.Contain("grid capacity"));
            }
            finally
            {
                Object.DestroyImmediate(root);
                Object.DestroyImmediate(scenario);
                foreach (var entry in previous)
                {
                    if (entry.Value == null)
                    {
                        PlayerPrefs.DeleteKey(entry.Key);
                    }
                    else
                    {
                        PlayerPrefs.SetString(entry.Key, entry.Value);
                    }
                }
                PlayerPrefs.Save();
            }
        }
    }
}
