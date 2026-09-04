using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SaltyGame.EditorTools
{
    /// <summary>
    /// Read-only validation and inspection for the explicit production upgrade catalog.
    /// </summary>
    public sealed class SpeciesUpgradeCatalogValidatorWindow : EditorWindow
    {
        const string ProductionCatalogPath = "Assets/Data/CellularSimulation/Upgrades/Production";

        sealed class CatalogEntry
        {
            public string Path;
            public SpeciesUpgradeAsset Asset;
            public SpeciesUpgradeSnapshot Snapshot;
            public string ValidationMessage;
            public bool HasDuplicateId;

            public bool IsValid => Snapshot != null && !HasDuplicateId;
        }

        readonly List<CatalogEntry> entries = new List<CatalogEntry>();
        Vector2 scrollPosition;
        string statusMessage = "Scan the production catalog to begin.";
        MessageType statusType = MessageType.Info;

        [MenuItem("Salty Game/Upgrades/Catalog Validator")]
        static void Open()
        {
            var window = GetWindow<SpeciesUpgradeCatalogValidatorWindow>();
            window.titleContent = new GUIContent("Upgrade Catalog Validator");
            window.minSize = new Vector2(620f, 420f);
            window.Show();
        }

        void OnEnable()
        {
            RefreshCatalog();
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Production upgrade catalog", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "This read-only tool validates assets under the explicit Production folder. "
                + "Fix assets in their Inspector; this window never rewrites them or changes runtime state.",
                MessageType.Info);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Folder", ProductionCatalogPath, EditorStyles.miniLabel);
            if (GUILayout.Button("Refresh", GUILayout.Width(90f)))
            {
                RefreshCatalog();
            }

            EditorGUILayout.EndHorizontal();

            var validCount = entries.Count(entry => entry.IsValid);
            var invalidCount = entries.Count - validCount;
            var duplicateCount = entries.Count(entry => entry.HasDuplicateId);
            EditorGUILayout.LabelField(
                $"Assets: {entries.Count}    Valid: {validCount}    Invalid: {invalidCount}    Duplicate IDs: {duplicateCount}",
                EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(statusMessage, statusType);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            foreach (var entry in entries)
            {
                DrawEntry(entry);
            }

            if (entries.Count == 0)
            {
                EditorGUILayout.HelpBox("No SpeciesUpgradeAsset files were found in the Production folder.", MessageType.Warning);
            }

            EditorGUILayout.EndScrollView();
        }

        void DrawEntry(CatalogEntry entry)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(entry.IsValid ? "VALID" : "ERROR", GUILayout.Width(48f));
            EditorGUILayout.LabelField(
                string.IsNullOrWhiteSpace(entry.Asset?.DisplayName) ? entry.Path : entry.Asset.DisplayName,
                EditorStyles.boldLabel);
            if (entry.Asset != null && GUILayout.Button("Ping", GUILayout.Width(50f)))
            {
                Selection.activeObject = entry.Asset;
                EditorGUIUtility.PingObject(entry.Asset);
            }

            EditorGUILayout.EndHorizontal();

            if (entry.Asset == null)
            {
                EditorGUILayout.LabelField(entry.Path, EditorStyles.miniLabel);
                EditorGUILayout.EndVertical();
                return;
            }

            EditorGUILayout.LabelField("ID", entry.Asset.UpgradeId);
            EditorGUILayout.LabelField("Target", entry.Asset.TargetSpeciesId);
            EditorGUILayout.LabelField("Cost", entry.Asset.Cost.ToString());

            if (entry.HasDuplicateId)
            {
                EditorGUILayout.HelpBox(
                    $"Stable ID '{entry.Asset.UpgradeId}' is used by more than one production asset.",
                    MessageType.Error);
            }

            if (!string.IsNullOrWhiteSpace(entry.ValidationMessage))
            {
                EditorGUILayout.HelpBox(entry.ValidationMessage, MessageType.Error);
            }

            if (entry.Snapshot != null)
            {
                var modifiers = string.Join(
                    ", ",
                    entry.Snapshot.Modifiers.Select(
                        modifier => $"{modifier.AttributeId} {modifier.SignedValue:+0.###;-0.###;0}"));
                EditorGUILayout.LabelField("Modifiers", modifiers);
                EditorGUILayout.LabelField("Fingerprint", entry.Snapshot.Fingerprint, EditorStyles.miniLabel);
            }

            EditorGUILayout.LabelField("Path", entry.Path, EditorStyles.miniLabel);
            EditorGUILayout.EndVertical();
        }

        void RefreshCatalog()
        {
            entries.Clear();
            var duplicateIds = new Dictionary<string, List<CatalogEntry>>(StringComparer.Ordinal);
            foreach (var guid in AssetDatabase.FindAssets("t:SpeciesUpgradeAsset", new[] { ProductionCatalogPath }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<SpeciesUpgradeAsset>(path);
                var entry = new CatalogEntry
                {
                    Path = path,
                    Asset = asset,
                };

                if (asset == null)
                {
                    entry.ValidationMessage = "The catalog search returned an asset that could not be loaded.";
                }
                else if (!asset.TryCreateSnapshot(out entry.Snapshot, out entry.ValidationMessage))
                {
                    entry.Snapshot = null;
                }

                entries.Add(entry);
                if (asset != null && !string.IsNullOrWhiteSpace(asset.UpgradeId))
                {
                    if (!duplicateIds.TryGetValue(asset.UpgradeId.Trim(), out var matchingEntries))
                    {
                        matchingEntries = new List<CatalogEntry>();
                        duplicateIds.Add(asset.UpgradeId.Trim(), matchingEntries);
                    }

                    matchingEntries.Add(entry);
                }
            }

            foreach (var matchingEntries in duplicateIds.Values)
            {
                if (matchingEntries.Count < 2)
                {
                    continue;
                }

                foreach (var entry in matchingEntries)
                {
                    entry.HasDuplicateId = true;
                }
            }

            entries.Sort((left, right) => string.Compare(left.Path, right.Path, StringComparison.Ordinal));
            var invalidCount = entries.Count(entry => !entry.IsValid);
            statusMessage = entries.Count == 0
                ? "No production upgrade assets found."
                : invalidCount == 0
                    ? $"Catalog scan passed: {entries.Count} production asset(s) are valid."
                    : $"Catalog scan found {invalidCount} invalid production asset(s).";
            statusType = invalidCount == 0 ? MessageType.Info : MessageType.Error;
            Repaint();
        }
    }
}
