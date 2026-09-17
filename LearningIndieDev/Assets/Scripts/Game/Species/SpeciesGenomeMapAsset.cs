using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
    [CreateAssetMenu(menuName = "Salty Game/Genome/Species Genome Map", fileName = "SpeciesGenomeMap")]
    public sealed class SpeciesGenomeMapAsset : ScriptableObject
    {
        [SerializeField] string speciesId;
        [SerializeField] GenomeUpgradeAsset[] upgrades = Array.Empty<GenomeUpgradeAsset>();

        public string SpeciesId => speciesId;
        public GenomeUpgradeAsset[] Upgrades => upgrades ?? Array.Empty<GenomeUpgradeAsset>();

        public event Action AuthoringChanged;

        void OnValidate()
        {
            AuthoringChanged?.Invoke();
        }

        public bool TryCreateSnapshot(out SpeciesGenomeMapSnapshot snapshot, out string validationMessage)
        {
            try
            {
                var nodeSnapshots = new List<GenomeUpgradeNodeSnapshot>();
                foreach (var upgrade in Upgrades)
                {
                    if (upgrade == null)
                    {
                        throw new InvalidOperationException("Genome map upgrade entries cannot be null.");
                    }

                    if (!upgrade.TryCreateSnapshot(out var nodeSnapshot, out var nodeValidationMessage))
                    {
                        throw new InvalidOperationException(
                            $"Genome upgrade '{upgrade.name}' is invalid: {nodeValidationMessage}");
                    }

                    nodeSnapshots.Add(nodeSnapshot);
                }

                snapshot = new SpeciesGenomeMapSnapshot(
                    new SpeciesId(speciesId),
                    nodeSnapshots);
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

        public SpeciesGenomeMapSnapshot CreateSnapshot()
        {
            if (!TryCreateSnapshot(out var snapshot, out var validationMessage))
            {
                throw new InvalidOperationException(
                    $"Species Genome map asset '{name}' is invalid: {validationMessage}");
            }

            return snapshot;
        }
    }
}
