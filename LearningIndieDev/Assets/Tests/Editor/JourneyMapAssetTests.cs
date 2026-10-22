using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;

namespace SaltyGame.EditorTests
{
    public sealed class JourneyMapAssetTests
    {
        const string MapPath =
            "Assets/Data/ProductionData/CellularSimulation/Species/Journey/ForestEdgeJourney.asset";
        const string ScenarioPath =
            "Assets/Data/ProductionData/CellularSimulation/Scenarios/ForestEdge.asset";

        [Test]
        public void ForestEdgeJourneyHasConnectedAuthoredConditionsAndVisibleFutureNodes()
        {
            var map = AssetDatabase.LoadAssetAtPath<JourneyMapAsset>(MapPath);
            var scenario = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(ScenarioPath);
            Assert.That(map, Is.Not.Null);
            Assert.That(scenario, Is.Not.Null);
            Assert.That(map.AppliesTo(scenario, new SpeciesId("hare")), Is.True);
            Assert.That(map.Nodes.Count, Is.EqualTo(31));
            Assert.That(map.StartNode, Is.Not.Null);
            Assert.That(map.StartNode.Row, Is.Zero);
            Assert.That(map.ValidateOverlayLayout(out var layoutError), Is.True, layoutError);
            Assert.That(map.NodePlacements.Count, Is.EqualTo(map.Nodes.Count));
            Assert.That(map.Nodes.Sum(node => node.NextNodes.Count), Is.EqualTo(43));
            var openings = map.Nodes.Where(node => node.Row == 0).ToArray();
            Assert.That(openings, Has.Length.EqualTo(3));
            Assert.That(openings, Does.Contain(map.StartNode));
            Assert.That(openings.All(node => node.Kind == JourneyNodeKind.Reward
                && node.PlayableInPrototype && node.RewardMutationIds.Count == 3), Is.True);
            Assert.That(openings.Select(LongestRouteFrom), Is.All.EqualTo(13));
            Assert.That(openings.SelectMany(ReachableNodesFrom).Distinct().Count(),
                Is.EqualTo(map.Nodes.Count));
            foreach (var stage in map.Nodes.GroupBy(node => node.Row))
            {
                Assert.That(stage.Select(node => node.Column).OrderBy(column => column),
                    Is.EqualTo(Enumerable.Range(0, stage.Count())),
                    $"Stage {stage.Key} has duplicate or missing lane indices.");
            }

            var ids = new HashSet<string>();
            var centers = new HashSet<UnityEngine.Vector2>();
            foreach (var node in map.Nodes)
            {
                Assert.That(node, Is.Not.Null);
                Assert.That(ids.Add(node.NodeId), Is.True, $"Duplicate node id {node.NodeId}");
                Assert.That(node.DisplayName, Is.Not.Empty);
                Assert.That(node.Description, Is.Not.Empty);
                Assert.That(map.TryGetNormalizedCenter(node, out var center), Is.True);
                Assert.That(centers.Add(center), Is.True, $"Duplicate visual position for {node.NodeId}");
                Assert.That(center.x, Is.InRange(0.04f, 0.95f));
                Assert.That(center.y, Is.InRange(0.14f, 0.79f));
                foreach (var next in node.NextNodes)
                {
                    Assert.That(next, Is.Not.Null);
                    Assert.That(map.GetNode(next.NodeId), Is.SameAs(next));
                    Assert.That(next.Row, Is.EqualTo(node.Row + 1));
                    Assert.That(map.TryGetNormalizedCenter(next, out var nextCenter), Is.True);
                    Assert.That(nextCenter.x, Is.GreaterThan(center.x),
                        $"The visual trail must advance from {node.NodeId} to {next.NodeId}.");
                }
            }

            Assert.That(centers.Max(center => center.x) - centers.Min(center => center.x),
                Is.GreaterThanOrEqualTo(0.89f));
            Assert.That(centers.Max(center => center.y) - centers.Min(center => center.y),
                Is.GreaterThanOrEqualTo(0.6f));
            foreach (var center in centers)
            {
                Assert.That(centers.Where(other => other != center)
                    .All(other => UnityEngine.Vector2.Distance(center, other) > 0.07f),
                    Is.True, $"Journey markers are crowded around {center}.");
            }

            Assert.That(openings.All(node => node.NextNodes.Count == 1), Is.True);
            var firstSimulations = openings.Select(node => node.NextNodes[0]).ToArray();
            Assert.That(firstSimulations.Distinct().Count(), Is.EqualTo(3));
            Assert.That(firstSimulations.All(node => node.Kind == JourneyNodeKind.Simulation
                && node.Row == 1 && node.PlayableInPrototype), Is.True);
            var conditions = firstSimulations.SelectMany(node => node.NextNodes).Distinct().ToArray();
            Assert.That(conditions, Has.Length.EqualTo(4));
            Assert.That(conditions.All(node => node.NextNodes.Count == 1), Is.True);
            Assert.That(conditions.Select(node => node.NextNodes[0]).Distinct().Count(), Is.EqualTo(2));
            var rules = scenario.CreateRuntimeData().SpeciesRules;
            foreach (var conditionNode in conditions)
            {
                Assert.That(firstSimulations.Any(node => map.CanChooseAfter(node, conditionNode)), Is.True);
                Assert.That(conditionNode.PlayableInPrototype, Is.True);
                if (conditionNode.Kind == JourneyNodeKind.Refresh || conditionNode.Kind == JourneyNodeKind.Event)
                {
                    Assert.DoesNotThrow(() => conditionNode.CreatePopulationEncounter());
                    continue;
                }
                Assert.That(conditionNode.Kind, Is.EqualTo(JourneyNodeKind.Condition));
                Assert.That(conditionNode.TryCreateCondition(out var condition, out var error),
                    Is.True, error);
                Assert.That(rules.ContainsKey(condition.TargetSpecies), Is.True);
                Assert.DoesNotThrow(() => condition.Apply(rules[condition.TargetSpecies]));
                Assert.That(conditionNode.NextNodes, Is.Not.Empty);
                Assert.That(conditionNode.NextNodes.All(next => next.Kind == JourneyNodeKind.Simulation
                    && next.PlayableInPrototype && next.Row == 3), Is.True);
            }

            Assert.That(map.Nodes.Count(node => node.Row > 3), Is.EqualTo(19));
            Assert.That(map.Nodes.Where(node => node.Row > 3)
                .All(node => !node.PlayableInPrototype), Is.True);
        }

        [Test]
        public void PopulationEncounterFreezesAuthoredValuesAndUsesAStableSeededRoll()
        {
            var asset = UnityEngine.ScriptableObject.CreateInstance<JourneyNodeAsset>();
            try
            {
                UnityEngine.JsonUtility.FromJsonOverwrite("{\"nodeId\":\"fixture\",\"displayName\":\"Fixture\",\"kind\":3,\"playableInPrototype\":true,\"eventGainPercent\":100}", asset);
                var frozen = asset.CreatePopulationEncounter();
                UnityEngine.JsonUtility.FromJsonOverwrite("{\"eventGainPercent\":0,\"eventPlantLoss\":8}", asset);
                Assert.That(frozen.EventAmount(123), Is.EqualTo(10));
                Assert.That(asset.CreatePopulationEncounter().EventAmount(123), Is.EqualTo(-8));
                var ordinary = new JourneyPopulationEncounter("seed-cache", "Cache", "", JourneyNodeKind.Event, 10, 10, 10, 5, 50);
                var outcomes = Enumerable.Range(0, 50).Select(ordinary.EventAmount).ToArray();
                Assert.That(outcomes, Has.Member(10));
                Assert.That(outcomes, Has.Member(-5));
                Assert.That(Enumerable.Range(0, 50).Select(ordinary.EventAmount), Is.EqualTo(outcomes));
            }
            finally { UnityEngine.Object.DestroyImmediate(asset); }
        }

        static int LongestRouteFrom(JourneyNodeAsset node)
        {
            return 1 + (node.NextNodes.Count == 0
                ? 0
                : node.NextNodes.Max(LongestRouteFrom));
        }

        static HashSet<JourneyNodeAsset> ReachableNodesFrom(JourneyNodeAsset start)
        {
            var reached = new HashSet<JourneyNodeAsset>();
            var pending = new Stack<JourneyNodeAsset>();
            pending.Push(start);
            while (pending.Count > 0)
            {
                var node = pending.Pop();
                if (!reached.Add(node))
                {
                    continue;
                }

                foreach (var next in node.NextNodes)
                {
                    pending.Push(next);
                }
            }

            return reached;
        }
    }
}
