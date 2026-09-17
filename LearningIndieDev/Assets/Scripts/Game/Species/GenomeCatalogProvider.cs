using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
    /// <summary>
    /// Unity composition boundary for Genome authoring assets. Consumers only
    /// receive the captured immutable catalog snapshot.
    /// </summary>
    public sealed class GenomeCatalogProvider : MonoBehaviour
    {
        [SerializeField] SpeciesGenomeMapAsset[] speciesMaps = Array.Empty<SpeciesGenomeMapAsset>();

        readonly List<SpeciesGenomeMapAsset> subscribedMaps = new List<SpeciesGenomeMapAsset>();
        readonly List<GenomeUpgradeAsset> subscribedUpgrades = new List<GenomeUpgradeAsset>();

        public GenomeCatalogSnapshot Snapshot { get; private set; } = GenomeCatalogSnapshot.Empty;
        public string ValidationMessage { get; private set; } = string.Empty;
        public event Action<GenomeCatalogSnapshot> SnapshotChanged;

        void OnEnable()
        {
            RebindAuthoringEvents();
            if (!TryCapture(out var validationMessage))
            {
                Debug.LogError($"[Salty] Genome catalog capture failed: {validationMessage}", this);
            }
        }

        void OnDisable()
        {
            UnbindAuthoringEvents();
        }

        void OnValidate()
        {
            RebindAuthoringEvents();
            TryCapture(out _);
        }

        void RebindAuthoringEvents()
        {
            UnbindAuthoringEvents();
            foreach (var speciesMap in speciesMaps ?? Array.Empty<SpeciesGenomeMapAsset>())
            {
                if (speciesMap == null)
                {
                    continue;
                }

                speciesMap.AuthoringChanged += HandleAuthoringChanged;
                subscribedMaps.Add(speciesMap);
                foreach (var upgrade in speciesMap.Upgrades)
                {
                    if (upgrade == null)
                    {
                        continue;
                    }

                    upgrade.AuthoringChanged += HandleAuthoringChanged;
                    subscribedUpgrades.Add(upgrade);
                }
            }
        }

        void UnbindAuthoringEvents()
        {
            foreach (var speciesMap in subscribedMaps)
            {
                speciesMap.AuthoringChanged -= HandleAuthoringChanged;
            }

            foreach (var upgrade in subscribedUpgrades)
            {
                upgrade.AuthoringChanged -= HandleAuthoringChanged;
            }

            subscribedMaps.Clear();
            subscribedUpgrades.Clear();
        }

        void HandleAuthoringChanged()
        {
            RebindAuthoringEvents();
            TryCapture(out _);
        }

        public bool TryCapture(out string validationMessage)
        {
            var previousFingerprint = Snapshot.Fingerprint;
            try
            {
                var mapSnapshots = new List<SpeciesGenomeMapSnapshot>();
                foreach (var speciesMap in speciesMaps ?? Array.Empty<SpeciesGenomeMapAsset>())
                {
                    if (speciesMap == null)
                    {
                        throw new InvalidOperationException("Genome catalog map entries cannot be null.");
                    }

                    if (!speciesMap.TryCreateSnapshot(out var mapSnapshot, out var mapValidationMessage))
                    {
                        throw new InvalidOperationException(
                            $"Species Genome map '{speciesMap.name}' is invalid: {mapValidationMessage}");
                    }

                    mapSnapshots.Add(mapSnapshot);
                }

                Snapshot = new GenomeCatalogSnapshot(mapSnapshots);
                ValidationMessage = string.Empty;
                validationMessage = string.Empty;
                NotifyIfChanged(previousFingerprint);
                return true;
            }
            catch (Exception exception) when (
                exception is ArgumentException
                || exception is InvalidOperationException)
            {
                Snapshot = GenomeCatalogSnapshot.Empty;
                ValidationMessage = exception.Message;
                validationMessage = ValidationMessage;
                NotifyIfChanged(previousFingerprint);
                return false;
            }
        }

        void NotifyIfChanged(string previousFingerprint)
        {
            if (!string.Equals(previousFingerprint, Snapshot.Fingerprint, StringComparison.Ordinal))
            {
                SnapshotChanged?.Invoke(Snapshot);
            }
        }
    }
}
