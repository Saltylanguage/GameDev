using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
    public enum JourneyNodeKind
    {
        Simulation,
        Condition,
        Upgrade,
        Event,
        Finale,
        Reward,
        Refresh,
    }

    [CreateAssetMenu(menuName = "Salty Game/Journey/Node", fileName = "JourneyNode")]
    public sealed class JourneyNodeAsset : ScriptableObject
    {
        [SerializeField] string nodeId;
        [SerializeField] string displayName;
        [SerializeField, TextArea] string description;
        [SerializeField] JourneyNodeKind kind;
        [SerializeField, Min(0)] int row;
        [SerializeField, Min(0)] int column;
        [SerializeField] bool playableInPrototype;
        [SerializeField] SpeciesUpgradeAsset conditionUpgrade;
        [SerializeField] string[] rewardMutationIds = Array.Empty<string>();
        [SerializeField, Min(0)] int dataReward;
        [SerializeField] JourneyNodeAsset[] nextNodes = Array.Empty<JourneyNodeAsset>();
        [SerializeField, Min(1)] int refreshHares = 10;
        [SerializeField, Min(1)] int refreshPlants = 10;
        [SerializeField, Min(1)] int eventPlantGain = 10;
        [SerializeField, Min(1)] int eventPlantLoss = 5;
        [SerializeField, Range(0, 100)] int eventGainPercent = 50;

        public string NodeId => nodeId;
        public string DisplayName => displayName;
        public string Description => description;
        public JourneyNodeKind Kind => kind;
        public int Row => row;
        public int Column => column;
        public bool PlayableInPrototype => playableInPrototype;
        public IReadOnlyList<string> RewardMutationIds => rewardMutationIds;
        public int DataReward => dataReward;
        public IReadOnlyList<JourneyNodeAsset> NextNodes => nextNodes;

        public JourneyPopulationEncounter CreatePopulationEncounter()
        {
            if (!playableInPrototype || (kind != JourneyNodeKind.Refresh && kind != JourneyNodeKind.Event)
                || refreshHares < 1 || refreshPlants < 1 || eventPlantGain < 1 || eventPlantLoss < 1
                || eventGainPercent < 0 || eventGainPercent > 100)
                throw new InvalidOperationException($"Invalid population encounter '{nodeId}'.");
            return new JourneyPopulationEncounter(nodeId, displayName, description, kind,
                refreshHares, refreshPlants, eventPlantGain, eventPlantLoss, eventGainPercent);
        }

        public bool TryCreateCondition(out SpeciesUpgradeSnapshot condition, out string error)
        {
            condition = null;
            error = string.Empty;
            if (kind != JourneyNodeKind.Condition || !playableInPrototype)
            {
                error = "This journey node is not a playable condition.";
                return false;
            }

            if (conditionUpgrade == null)
            {
                error = "This journey condition has no authored effect.";
                return false;
            }

            return conditionUpgrade.TryCreateSnapshot(out condition, out error);
        }
    }

    // Resolved once when preparing a journey; later asset edits cannot change it.
    public sealed class JourneyPopulationEncounter
    {
        public JourneyPopulationEncounter(string id, string name, string description, JourneyNodeKind kind,
            int hares, int plants, int gain, int loss, int gainPercent)
        {
            Id = id; Name = name; Description = description; Kind = kind;
            Hares = hares; Plants = plants; Gain = gain; Loss = loss; GainPercent = gainPercent;
        }
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public JourneyNodeKind Kind { get; }
        public int Hares { get; }
        public int Plants { get; }
        public int Gain { get; }
        public int Loss { get; }
        public int GainPercent { get; }
        public int EventAmount(int runSeed)
        {
            unchecked
            {
                uint hash = 2166136261;
                foreach (var character in Id) hash = (hash ^ character) * 16777619;
                return new System.Random(runSeed ^ (int)hash).Next(100) < GainPercent ? Gain : -Loss;
            }
        }
    }
}
