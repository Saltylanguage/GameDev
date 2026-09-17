using System;
using UnityEngine;

namespace SaltyGame
{
    [CreateAssetMenu(menuName = "Salty Game/Genome/Genome Upgrade", fileName = "GenomeUpgrade")]
    public sealed class GenomeUpgradeAsset : ScriptableObject
    {
        [SerializeField] string upgradeId;
        [SerializeField] string displayName;
        [SerializeField, TextArea] string description;
        [SerializeField] string targetSpeciesId;
        [SerializeField] string[] prerequisiteNodeIds = Array.Empty<string>();

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
                snapshot = new GenomeUpgradeNodeSnapshot(
                    upgradeId,
                    displayName,
                    description,
                    new SpeciesId(targetSpeciesId),
                    prerequisiteNodeIds);
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
