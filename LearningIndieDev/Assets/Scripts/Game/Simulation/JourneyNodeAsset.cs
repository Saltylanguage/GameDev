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
}
