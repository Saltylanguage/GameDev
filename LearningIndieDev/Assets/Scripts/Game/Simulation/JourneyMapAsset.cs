using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
    [Serializable]
    public struct JourneyNodePlacement
    {
        [SerializeField] JourneyNodeAsset node;
        [SerializeField] Vector2 normalizedCenter;

        public JourneyNodeAsset Node => node;
        public Vector2 NormalizedCenter => normalizedCenter;
    }

    [CreateAssetMenu(menuName = "Salty Game/Journey/Map", fileName = "JourneyMap")]
    public sealed class JourneyMapAsset : ScriptableObject
    {
        [SerializeField] string scenarioId = "ForestEdge";
        [SerializeField] string playerSpeciesId = "hare";
        [SerializeField] JourneyNodeAsset startNode;
        [SerializeField] JourneyNodeAsset[] nodes = Array.Empty<JourneyNodeAsset>();
        [SerializeField] JourneyNodePlacement[] nodePlacements = Array.Empty<JourneyNodePlacement>();

        public string ScenarioId => scenarioId;
        public string PlayerSpeciesId => playerSpeciesId;
        public JourneyNodeAsset StartNode => startNode;
        public IReadOnlyList<JourneyNodeAsset> Nodes => nodes;
        public IReadOnlyList<JourneyNodePlacement> NodePlacements => nodePlacements;

        public bool TryGetNormalizedCenter(JourneyNodeAsset node, out Vector2 center)
        {
            if (node == null)
            {
                center = default;
                return false;
            }

            foreach (var placement in nodePlacements ?? Array.Empty<JourneyNodePlacement>())
            {
                if (placement.Node == node)
                {
                    center = placement.NormalizedCenter;
                    return true;
                }
            }

            center = default;
            return false;
        }

        public bool ValidateOverlayLayout(out string error)
        {
            var knownNodes = new HashSet<JourneyNodeAsset>(nodes ?? Array.Empty<JourneyNodeAsset>());
            if (nodes == null || nodes.Length == 0 || knownNodes.Count != nodes.Length
                || knownNodes.Contains(null) || startNode == null || !knownNodes.Contains(startNode))
            {
                error = "The journey map needs a unique node list containing its start node.";
                return false;
            }

            foreach (var node in nodes)
            {
                foreach (var next in node.NextNodes ?? Array.Empty<JourneyNodeAsset>())
                {
                    if (next == null || !knownNodes.Contains(next))
                    {
                        error = $"The route from {node.NodeId} points outside this journey map.";
                        return false;
                    }
                }
            }

            var placedNodes = new HashSet<JourneyNodeAsset>();
            foreach (var placement in nodePlacements ?? Array.Empty<JourneyNodePlacement>())
            {
                if (placement.Node == null || !knownNodes.Contains(placement.Node)
                    || !placedNodes.Add(placement.Node))
                {
                    error = "The journey overlay has a missing, foreign, or duplicate node placement.";
                    return false;
                }

                var center = placement.NormalizedCenter;
                if (float.IsNaN(center.x) || float.IsNaN(center.y)
                    || float.IsInfinity(center.x) || float.IsInfinity(center.y)
                    || center.x < 0f || center.x > 1f
                    || center.y < 0f || center.y > 1f)
                {
                    error = $"The journey overlay position for {placement.Node.NodeId} must be within 0–1.";
                    return false;
                }
            }

            if (placedNodes.Count != knownNodes.Count)
            {
                error = "Every journey node needs one overlay placement.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        public bool CanChooseAfter(JourneyNodeAsset previous, JourneyNodeAsset candidate)
        {
            if (previous == null || candidate == null || GetNode(candidate.NodeId) != candidate)
            {
                return false;
            }

            foreach (var next in previous.NextNodes)
            {
                if (next == candidate)
                {
                    return true;
                }
            }

            return false;
        }

        public bool AppliesTo(ScenarioDefinitionAsset scenario, SpeciesId playerSpecies)
        {
            return scenario != null
                && string.Equals(scenario.name, scenarioId, StringComparison.OrdinalIgnoreCase)
                && string.Equals(playerSpecies.Value, playerSpeciesId, StringComparison.OrdinalIgnoreCase);
        }

        public JourneyNodeAsset GetNode(string nodeId)
        {
            if (string.IsNullOrEmpty(nodeId))
            {
                return null;
            }

            foreach (var node in nodes ?? Array.Empty<JourneyNodeAsset>())
            {
                if (node != null && string.Equals(node.NodeId, nodeId, StringComparison.Ordinal))
                {
                    return node;
                }
            }

            return null;
        }
    }
}
