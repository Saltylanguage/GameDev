using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
    [CreateAssetMenu(menuName = "Salty Game/Genome/Genome Upgrade", fileName = "GenomeUpgrade")]
    public sealed class GenomeUpgradeAsset : ScriptableObject
    {
        [Serializable]
        sealed class ModifierEntry
        {
            [SerializeField] string attributeId;
            [SerializeField] float signedValue;

            public SpeciesUpgradeModifier CreateModifier() => new SpeciesUpgradeModifier(attributeId, signedValue);
        }

        [SerializeField] string upgradeId;
        [SerializeField] string displayName;
        [SerializeField, TextArea] string description;
        [SerializeField] string targetSpeciesId;
        [SerializeField] string[] prerequisiteNodeIds = Array.Empty<string>();
        [SerializeField] long rabbitDataPrice;
        [SerializeField] int capacityPoints;
        [SerializeField, Min(1)] int maximumRanks = 1;
        [SerializeField] string branchName;
        [SerializeField] ModifierEntry[] modifiers = Array.Empty<ModifierEntry>();

        public string UpgradeId => upgradeId;
        public string DisplayName => displayName;
        public string Description => description;
        public string TargetSpeciesId => targetSpeciesId;
        public string[] PrerequisiteNodeIds => prerequisiteNodeIds ?? Array.Empty<string>();

        public event Action AuthoringChanged;

        void OnValidate()
        {
            AuthoringChanged?.Invoke();
        }

        public bool TryCreateSnapshot(out GenomeUpgradeNodeSnapshot snapshot, out string validationMessage)
        {
            try
            {
                var modifierValues = new List<SpeciesUpgradeModifier>();
                foreach (var entry in modifiers ?? Array.Empty<ModifierEntry>())
                {
                    if (entry == null)
                    {
                        throw new InvalidOperationException("Genome modifier entries cannot be null.");
                    }
                    modifierValues.Add(entry.CreateModifier());
                }
                snapshot = new GenomeUpgradeNodeSnapshot(
                    upgradeId,
                    displayName,
                    description,
                    new SpeciesId(targetSpeciesId),
                    prerequisiteNodeIds,
                    rabbitDataPrice,
                    capacityPoints,
                    branchName,
                    modifierValues,
                    maximumRanks);
                validationMessage = string.Empty;
                return true;
            }
            catch (Exception exception) when (
                exception is ArgumentException
                || exception is InvalidOperationException)
            {
                snapshot = null;
                validationMessage = exception.Message;
                return false;
            }
        }

        public GenomeUpgradeNodeSnapshot CreateSnapshot()
        {
            if (!TryCreateSnapshot(out var snapshot, out var validationMessage))
            {
                throw new InvalidOperationException(
                    $"Genome upgrade asset '{name}' is invalid: {validationMessage}");
            }

            return snapshot;
        }
    }
}
