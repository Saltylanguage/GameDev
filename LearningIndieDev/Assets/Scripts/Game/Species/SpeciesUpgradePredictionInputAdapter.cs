using System;
using System.Collections.Generic;
using System.Linq;
using SaltyGame;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SaltyGame.EditorTools
{
    /// <summary>
    /// Converts ordered upgrade snapshots into the research prediction-input
    /// contract. AssetDatabase resolution is editor-only; consumers receive
    /// snapshots and never retain live Scriptable Object references.
    /// </summary>
    public static class SpeciesUpgradePredictionInputAdapter
    {
        public const string ProductionCatalogPath = "Assets/Data/CellularSimulation/Upgrades/Production";
        public const string SchemaVersion = "species-upgrade-prediction-input-v1";

        public static SpeciesUpgradeSnapshot[] Resolve(IReadOnlyList<string> orderedUpgradeIds)
        {
            return Resolve(orderedUpgradeIds, ProductionCatalogPath);
        }

        public static SpeciesUpgradeSnapshot[] Resolve(
            IReadOnlyList<string> orderedUpgradeIds,
            string catalogPath)
        {
            if (!TryResolve(orderedUpgradeIds, catalogPath, out var snapshots, out var validationMessage))
            {
                throw new ArgumentException(validationMessage, nameof(orderedUpgradeIds));
            }

            return snapshots;
        }

        public static bool TryResolve(
            IReadOnlyList<string> orderedUpgradeIds,
            out SpeciesUpgradeSnapshot[] snapshots,
            out string validationMessage)
        {
            return TryResolve(
                orderedUpgradeIds,
                ProductionCatalogPath,
                out snapshots,
                out validationMessage);
        }

        public static bool TryResolve(
            IReadOnlyList<string> orderedUpgradeIds,
            string catalogPath,
            out SpeciesUpgradeSnapshot[] snapshots,
            out string validationMessage)
        {
            snapshots = Array.Empty<SpeciesUpgradeSnapshot>();
            validationMessage = string.Empty;
            if (orderedUpgradeIds == null || orderedUpgradeIds.Count == 0)
            {
                return true;
            }

#if UNITY_EDITOR
            Dictionary<string, SpeciesUpgradeSnapshot> catalog;
            try
            {
                catalog = LoadCatalog(catalogPath);
            }
            catch (Exception exception) when (exception is ArgumentException || exception is InvalidOperationException)
            {
                validationMessage = exception.Message;
                return false;
            }

            var seen = new HashSet<string>(StringComparer.Ordinal);
            var resolved = new List<SpeciesUpgradeSnapshot>(orderedUpgradeIds.Count);
            for (var index = 0; index < orderedUpgradeIds.Count; index++)
            {
                var id = orderedUpgradeIds[index]?.Trim();
                if (string.IsNullOrWhiteSpace(id) || string.Equals(id, "none", StringComparison.Ordinal))
                {
                    validationMessage = $"Upgrade entry {index + 1} must contain a production upgrade ID.";
                    return false;
                }

                if (!seen.Add(id))
                {
                    validationMessage = $"Upgrade ID '{id}' may only appear once in an ordered loadout.";
                    return false;
                }

                if (!catalog.TryGetValue(id, out var snapshot))
                {
                    validationMessage =
                        $"Upgrade ID '{id}' was not found in the catalog '{catalogPath}'.";
                    return false;
                }

                resolved.Add(snapshot);
            }

            snapshots = resolved.ToArray();
            return true;
#else
            validationMessage = "Stable-ID asset resolution is editor-only; provide resolved snapshots instead.";
            return false;
#endif
        }

        public static SpeciesUpgradeSnapshot[] ResolveAssets(
            IReadOnlyList<SpeciesUpgradeAsset> orderedAssets)
        {
            if (orderedAssets == null || orderedAssets.Count == 0)
            {
                return Array.Empty<SpeciesUpgradeSnapshot>();
            }

            var snapshots = new SpeciesUpgradeSnapshot[orderedAssets.Count];
            var seen = new HashSet<string>(StringComparer.Ordinal);
            for (var index = 0; index < orderedAssets.Count; index++)
            {
                var asset = orderedAssets[index]
                    ?? throw new ArgumentException("Prediction assets cannot contain null entries.", nameof(orderedAssets));
                if (!asset.TryCreateSnapshot(out var snapshot, out var validationMessage))
                {
                    throw new ArgumentException(
                        $"Upgrade asset '{asset.name}' is invalid: {validationMessage}",
                        nameof(orderedAssets));
                }

                if (!seen.Add(snapshot.Id))
                {
                    throw new ArgumentException(
                        $"Upgrade ID '{snapshot.Id}' may only appear once in an ordered loadout.",
                        nameof(orderedAssets));
                }

                snapshots[index] = snapshot;
            }

            return snapshots;
        }

        public static SpeciesUpgradePredictionInput CreateInput(
            IReadOnlyList<SpeciesUpgradeSnapshot> orderedSnapshots)
        {
            return CreateInput(orderedSnapshots, string.Empty);
        }

        public static SpeciesUpgradePredictionInput CreateInput(
            IReadOnlyList<SpeciesUpgradeSnapshot> orderedSnapshots,
            string sourceCatalogPath)
        {
            orderedSnapshots = orderedSnapshots ?? Array.Empty<SpeciesUpgradeSnapshot>();
            var records = new SpeciesUpgradePredictionRecord[orderedSnapshots.Count];
            var orderedIds = new string[orderedSnapshots.Count];
            for (var index = 0; index < orderedSnapshots.Count; index++)
            {
                var snapshot = orderedSnapshots[index]
                    ?? throw new ArgumentException("Prediction inputs cannot contain null snapshots.", nameof(orderedSnapshots));
                orderedIds[index] = snapshot.Id;
                records[index] = new SpeciesUpgradePredictionRecord
                {
                    order = index,
                    upgradeId = snapshot.Id,
                    displayName = snapshot.DisplayName,
                    description = snapshot.Description,
                    targetSpeciesId = snapshot.TargetSpecies.Value,
                    scope = snapshot.Scope.ToString(),
                    cost = snapshot.Cost,
                    contractVersion = SpeciesUpgradeSnapshot.ContractVersion,
                    registryFingerprint = snapshot.RegistryFingerprint,
                    fingerprint = snapshot.Fingerprint,
                    prerequisiteUpgradeIds = snapshot.PrerequisiteUpgradeIds.ToArray(),
                    excludedUpgradeIds = snapshot.ExcludedUpgradeIds.ToArray(),
                    modifiers = snapshot.Modifiers
                        .Select(modifier => new SpeciesUpgradePredictionModifierRecord
                        {
                            attributeId = modifier.AttributeId,
                            signedValue = modifier.SignedValue,
                        })
                        .ToArray(),
                };
            }

            return new SpeciesUpgradePredictionInput
            {
                schemaVersion = SchemaVersion,
                contractVersion = SpeciesUpgradeSnapshot.ContractVersion,
                sourceCatalogPath = sourceCatalogPath ?? string.Empty,
                registryFingerprint = orderedSnapshots.Count == 0
                    ? SpeciesAttributeRegistry.Fingerprint
                    : orderedSnapshots[0].RegistryFingerprint,
                orderedLoadoutFingerprint = SpeciesUpgradeLoadoutFingerprint.Create(orderedSnapshots),
                orderedUpgradeIds = orderedIds,
                upgrades = records,
            };
        }

        public static string Serialize(IReadOnlyList<SpeciesUpgradeSnapshot> orderedSnapshots)
        {
            return JsonUtility.ToJson(CreateInput(orderedSnapshots), true);
        }

        public static string Serialize(
            IReadOnlyList<SpeciesUpgradeSnapshot> orderedSnapshots,
            string sourceCatalogPath)
        {
            return JsonUtility.ToJson(CreateInput(orderedSnapshots, sourceCatalogPath), true);
        }

        public static SpeciesUpgradePredictionInput CreateInputFromAssets(
            IReadOnlyList<SpeciesUpgradeAsset> orderedAssets)
        {
            return CreateInput(ResolveAssets(orderedAssets));
        }

        public static SpeciesUpgradePredictionInput CreateInputFromAssets(
            IReadOnlyList<SpeciesUpgradeAsset> orderedAssets,
            string sourceCatalogPath)
        {
            return CreateInput(ResolveAssets(orderedAssets), sourceCatalogPath);
        }

        public static string SerializeAssets(IReadOnlyList<SpeciesUpgradeAsset> orderedAssets)
        {
            return Serialize(ResolveAssets(orderedAssets));
        }

        public static string SerializeAssets(
            IReadOnlyList<SpeciesUpgradeAsset> orderedAssets,
            string sourceCatalogPath)
        {
            return Serialize(ResolveAssets(orderedAssets), sourceCatalogPath);
        }

#if UNITY_EDITOR
        static Dictionary<string, SpeciesUpgradeSnapshot> LoadCatalog(string catalogPath)
        {
            if (string.IsNullOrWhiteSpace(catalogPath))
            {
                throw new ArgumentException("Upgrade catalog path cannot be empty.", nameof(catalogPath));
            }

            catalogPath = catalogPath.Trim().Replace('\\', '/');
            if (!catalogPath.StartsWith("Assets/", StringComparison.Ordinal)
                || catalogPath.IndexOf("..", StringComparison.Ordinal) >= 0)
            {
                throw new ArgumentException(
                    $"Upgrade catalog path must stay inside the Unity Assets folder: '{catalogPath}'.",
                    nameof(catalogPath));
            }

            if (!AssetDatabase.IsValidFolder(catalogPath))
            {
                throw new ArgumentException(
                    $"Upgrade catalog folder was not found: '{catalogPath}'.",
                    nameof(catalogPath));
            }

            var catalog = new Dictionary<string, SpeciesUpgradeSnapshot>(StringComparer.Ordinal);
            var assetPaths = AssetDatabase.FindAssets("t:SpeciesUpgradeAsset", new[] { catalogPath })
                .Select(AssetDatabase.GUIDToAssetPath)
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToArray();
            foreach (var assetPath in assetPaths)
            {
                var asset = AssetDatabase.LoadAssetAtPath<SpeciesUpgradeAsset>(assetPath);
                if (asset == null)
                {
                    throw new InvalidOperationException($"Could not load upgrade asset at '{assetPath}'.");
                }

                if (!asset.TryCreateSnapshot(out var snapshot, out var validationMessage))
                {
                    throw new InvalidOperationException(
                        $"Upgrade '{asset.name}' is invalid: {validationMessage}");
                }

                if (catalog.ContainsKey(snapshot.Id))
                {
                    throw new ArgumentException(
                        $"Duplicate upgrade ID '{snapshot.Id}' was found in '{catalogPath}'.");
                }

                catalog.Add(snapshot.Id, snapshot);
            }

            return catalog;
        }
#endif
    }

    [Serializable]
    public sealed class SpeciesUpgradePredictionInput
    {
        public string schemaVersion;
        public string contractVersion;
        public string sourceCatalogPath;
        public string registryFingerprint;
        public string orderedLoadoutFingerprint;
        public string[] orderedUpgradeIds;
        public SpeciesUpgradePredictionRecord[] upgrades;
    }

    [Serializable]
    public sealed class SpeciesUpgradePredictionRecord
    {
        public int order;
        public string upgradeId;
        public string displayName;
        public string description;
        public string targetSpeciesId;
        public string scope;
        public int cost;
        public string contractVersion;
        public string registryFingerprint;
        public string fingerprint;
        public string[] prerequisiteUpgradeIds;
        public string[] excludedUpgradeIds;
        public SpeciesUpgradePredictionModifierRecord[] modifiers;
    }

    [Serializable]
    public sealed class SpeciesUpgradePredictionModifierRecord
    {
        public string attributeId;
        public float signedValue;
    }
}
