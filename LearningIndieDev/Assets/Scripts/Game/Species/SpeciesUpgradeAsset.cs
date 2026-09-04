using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
    [CreateAssetMenu(menuName = "Salty Game/Upgrades/Species Per-Run Upgrade", fileName = "SpeciesUpgrade")]
    public sealed class SpeciesUpgradeAsset : ScriptableObject
    {
        [Serializable]
        sealed class ModifierEntry
        {
            [SerializeField] string attributeId;
            [SerializeField] float signedValue;

            public SpeciesUpgradeModifier CreateModifier()
            {
                return new SpeciesUpgradeModifier(attributeId, signedValue);
            }
        }

        [SerializeField] string upgradeId;
        [SerializeField] string displayName;
        [SerializeField, TextArea] string description;
        [SerializeField] string targetSpeciesId;
        [SerializeField, Min(0)] int cost;
        [SerializeField] ModifierEntry[] modifiers = Array.Empty<ModifierEntry>();
        [SerializeField] string[] prerequisiteUpgradeIds = Array.Empty<string>();
        [SerializeField] string[] excludedUpgradeIds = Array.Empty<string>();

        public string UpgradeId => upgradeId;
        public string DisplayName => displayName;
        public string Description => description;
        public string TargetSpeciesId => targetSpeciesId;
        public int Cost => cost;

        public bool TryCreateSnapshot(out SpeciesUpgradeSnapshot snapshot, out string validationMessage)
        {
            try
            {
                var modifierValues = new List<SpeciesUpgradeModifier>();
                foreach (var entry in modifiers ?? Array.Empty<ModifierEntry>())
                {
                    if (entry == null)
                    {
                        throw new InvalidOperationException("Upgrade modifier entries cannot be null.");
                    }

                    modifierValues.Add(entry.CreateModifier());
                }

                snapshot = new SpeciesUpgradeSnapshot(
                    upgradeId,
                    displayName,
                    description,
                    new SpeciesId(targetSpeciesId),
                    cost,
                    modifierValues,
                    prerequisiteUpgradeIds,
                    excludedUpgradeIds);
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

        public SpeciesUpgradeSnapshot CreateSnapshot()
        {
            if (!TryCreateSnapshot(out var snapshot, out var validationMessage))
            {
                throw new InvalidOperationException(
                    $"Species upgrade asset '{name}' is invalid: {validationMessage}");
            }

            return snapshot;
        }
    }
}
