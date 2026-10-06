using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;

namespace SaltyGame.EditorTests
{
    public sealed class JourneyMapAssetTests
    {
        const string MapPath =
            "Assets/Data/ProductionData/CellularSimulation/Journey/ForestEdgeJourney.asset";
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
            Assert.That(map.Nodes.Count, Is.EqualTo(18));
            Assert.That(map.StartNode, Is.Not.Null);
            Assert.That(map.StartNode.Row, Is.Zero);
            Assert.That(map.ValidateOverlayLayout(out var layoutError), Is.True, layoutError);
            Assert.That(map.NodePlacements.Count, Is.EqualTo(map.Nodes.Count));
            Assert.That(map.Nodes.Sum(node => node.NextNodes.Count), Is.EqualTo(23));
            Assert.That(LongestRouteFrom(map.StartNode), Is.EqualTo(12));
            Assert.That(map.Nodes.Count(node => node.NextNodes.Count > 1), Is.GreaterThanOrEqualTo(4));

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
                Assert.That(center.x, Is.InRange(0.05f, 0.95f));
                Assert.That(center.y, Is.InRange(0.2f, 0.85f));
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

            Assert.That(map.StartNode.Kind, Is.EqualTo(JourneyNodeKind.Reward));
            Assert.That(map.StartNode.RewardMutationIds.Count, Is.EqualTo(3));
            Assert.That(map.StartNode.NextNodes.Count, Is.EqualTo(1));
            var firstSimulation = map.StartNode.NextNodes[0];
            Assert.That(firstSimulation.Kind, Is.EqualTo(JourneyNodeKind.Simulation));
            Assert.That(firstSimulation.Row, Is.EqualTo(1));
            Assert.That(firstSimulation.PlayableInPrototype, Is.True);
            var conditions = firstSimulation.NextNodes.ToArray();
            Assert.That(conditions, Has.Length.EqualTo(2));
            var rules = scenario.CreateRuntimeData().SpeciesRules;
            foreach (var conditionNode in conditions)
            {
                Assert.That(map.CanChooseAfter(firstSimulation, conditionNode), Is.True);
                Assert.That(conditionNode.Kind, Is.EqualTo(JourneyNodeKind.Condition));
                Assert.That(conditionNode.PlayableInPrototype, Is.True);
                Assert.That(conditionNode.TryCreateCondition(out var condition, out var error),
                    Is.True, error);
                Assert.That(rules.ContainsKey(condition.TargetSpecies), Is.True);
                Assert.DoesNotThrow(() => condition.Apply(rules[condition.TargetSpecies]));
                Assert.That(conditionNode.NextNodes, Is.Not.Empty);
                Assert.That(conditionNode.NextNodes.All(next => next.Kind == JourneyNodeKind.Simulation
                    && next.PlayableInPrototype && next.Row == 3), Is.True);
            }

            Assert.That(map.Nodes.Count(node => node.Row > 3), Is.EqualTo(12));
            Assert.That(map.Nodes.Where(node => node.Row > 3)
                .All(node => !node.PlayableInPrototype), Is.True);
        }

        static int LongestRouteFrom(JourneyNodeAsset node)
        {
            return 1 + (node.NextNodes.Count == 0
                ? 0
                : node.NextNodes.Max(LongestRouteFrom));
        }
    }
}
