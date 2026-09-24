using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SaltyGame.EditorTools
{
    /// <summary>
    /// Card-based authoring view for the species definition assets.
    /// </summary>
    public sealed class SpeciesCatalogWindow : EditorWindow
    {
        const string CatalogPath = "Assets/Data/CellularSimulation/Species";
        const string SpeciesArtPath = "Assets/Art/Species";
        const float CardMinimumWidth = 330f;
        const float CardMaximumWidth = 430f;
        const float CardGap = 8f;
        const float IconSize = 68f;

        static readonly string[] RoleFilters = { "All", "Plants", "Herbivores", "Carnivores" };

        sealed class CatalogEntry
        {
            public string Path;
            public SpeciesDefinitionAsset Asset;
            public SerializedObject SerializedObject;
            public Texture2D Icon;
            public string IconKey;
            public bool ShowAdvanced;
        }

        readonly List<CatalogEntry> entries = new List<CatalogEntry>();
        readonly List<string> speciesArtPaths = new List<string>();
        readonly Dictionary<string, int> idCounts = new Dictionary<string, int>(StringComparer.Ordinal);

        Vector2 scrollPosition;
        string searchText = string.Empty;
        int roleFilter;

        GUIStyle titleStyle;
        GUIStyle subtitleStyle;
        GUIStyle cardStyle;
        GUIStyle cardTitleStyle;
        GUIStyle roleBadgeStyle;
        GUIStyle sectionStyle;
        GUIStyle fallbackIconStyle;
        GUIStyle pathStyle;

        [MenuItem("Salty Game/Simulation/Species Catalog")]
        static void Open()
        {
            var window = GetWindow<SpeciesCatalogWindow>();
            window.titleContent = new GUIContent("Species Catalog");
            window.minSize = new Vector2(720f, 520f);
            window.Show();
        }

        void OnEnable()
        {
            Undo.undoRedoPerformed += HandleUndoRedo;
            RefreshCatalog();
        }

        void OnDisable()
        {
            Undo.undoRedoPerformed -= HandleUndoRedo;
        }

        void OnProjectChange()
        {
            RefreshCatalog();
        }

        void OnGUI()
        {
            EnsureStyles();
            UpdateIdCounts();
            DrawCatalogHeader();
            DrawToolbar();
            DrawSummary();
            DrawCatalogCards();
        }

        void DrawCatalogHeader()
        {
            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("Species Catalog", titleStyle);
            EditorGUILayout.LabelField(
                "Compare and tune the authored species definitions without opening each asset separately.",
                subtitleStyle);
            EditorGUILayout.Space(6f);
        }

        void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.Label("Search", GUILayout.Width(44f));
                searchText = GUILayout.TextField(searchText, EditorStyles.toolbarSearchField, GUILayout.MinWidth(108f));

                if (!string.IsNullOrEmpty(searchText)
                    && GUILayout.Button("Clear", EditorStyles.toolbarButton, GUILayout.Width(42f)))
                {
                    searchText = string.Empty;
                    GUI.FocusControl(null);
                }

                GUILayout.Space(8f);
                roleFilter = GUILayout.Toolbar(roleFilter, RoleFilters, EditorStyles.toolbarButton, GUILayout.Width(280f));
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Collapse", EditorStyles.toolbarButton, GUILayout.Width(62f)))
                {
                    SetAdvancedVisibility(false);
                }

                if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(58f)))
                {
                    RefreshCatalog();
                }

                if (GUILayout.Button("Save Assets", EditorStyles.toolbarButton, GUILayout.Width(78f)))
                {
                    AssetDatabase.SaveAssets();
                }
            }
        }

        void DrawSummary()
        {
            var visibleCount = 0;
            var plantCount = 0;
            var herbivoreCount = 0;
            var carnivoreCount = 0;
            var invalidCount = 0;

            foreach (var entry in entries)
            {
                if (entry.Asset == null)
                {
                    invalidCount++;
                    continue;
                }

                switch (entry.Asset.Role)
                {
                    case SpeciesRole.Plant:
                        plantCount++;
                        break;
                    case SpeciesRole.Herbivore:
                        herbivoreCount++;
                        break;
                    case SpeciesRole.Carnivore:
                        carnivoreCount++;
                        break;
                }

                if (IsVisible(entry))
                {
                    visibleCount++;
                }

                if (!TryGetId(entry, out var id) || string.IsNullOrWhiteSpace(id) || IsDuplicateId(id))
                {
                    invalidCount++;
                }
            }

            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                GUILayout.Label($"{visibleCount} shown", EditorStyles.boldLabel, GUILayout.Width(72f));
                GUILayout.Label($"{entries.Count} total", GUILayout.Width(62f));
                GUILayout.Label($"Plants  {plantCount}", GUILayout.Width(72f));
                GUILayout.Label($"Herbivores  {herbivoreCount}", GUILayout.Width(96f));
                GUILayout.Label($"Carnivores  {carnivoreCount}", GUILayout.Width(96f));
                GUILayout.FlexibleSpace();

                if (invalidCount > 0)
                {
                    GUILayout.Label($"{invalidCount} need attention", EditorStyles.miniBoldLabel);
                }
                else
                {
                    GUILayout.Label("Catalog IDs look healthy", EditorStyles.centeredGreyMiniLabel);
                }
            }
        }

        void DrawCatalogCards()
        {
            var visibleEntries = GetVisibleEntries();
            if (visibleEntries.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    entries.Count == 0
                        ? $"No SpeciesDefinitionAsset files were found under {CatalogPath}."
                        : "No species match the current search and role filter.",
                    MessageType.Info);
                return;
            }

            var availableWidth = Mathf.Max(CardMinimumWidth, position.width - 30f);
            var columnCount = Mathf.Max(1, Mathf.FloorToInt((availableWidth + CardGap) / (CardMinimumWidth + CardGap)));
            var cardWidth = Mathf.Min(
                CardMaximumWidth,
                (availableWidth - CardGap * (columnCount - 1)) / columnCount);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            for (var rowStart = 0; rowStart < visibleEntries.Count; rowStart += columnCount)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    for (var column = 0; column < columnCount; column++)
                    {
                        var entryIndex = rowStart + column;
                        if (entryIndex < visibleEntries.Count)
                        {
                            DrawSpeciesCard(visibleEntries[entryIndex], cardWidth);
                        }
                        else
                        {
                            GUILayout.Space(cardWidth);
                        }

                        if (column < columnCount - 1)
                        {
                            GUILayout.Space(CardGap);
                        }
                    }

                    GUILayout.FlexibleSpace();
                }

                GUILayout.Space(CardGap);
            }

            EditorGUILayout.EndScrollView();
        }

        void DrawSpeciesCard(CatalogEntry entry, float cardWidth)
        {
            if (entry.Asset == null || entry.SerializedObject == null)
            {
                return;
            }

            entry.SerializedObject.UpdateIfRequiredOrScript();
            var idProperty = entry.SerializedObject.FindProperty("id");
            var id = idProperty == null ? entry.Asset.name : idProperty.stringValue;

            using (new EditorGUILayout.VerticalScope(cardStyle, GUILayout.Width(cardWidth)))
            {
                DrawCardHeader(entry, id);
                DrawValidation(entry, id);

                DrawSectionLabel("AT A GLANCE");
                DrawProperty(idProperty, "Species ID");

                if (entry.Asset.Role == SpeciesRole.Plant)
                {
                    DrawPlantOverview(entry.SerializedObject, cardWidth);
                }
                else
                {
                    DrawAnimalOverview(entry.SerializedObject, cardWidth);
                }

                DrawPatternSummary(entry.SerializedObject);
                EditorGUILayout.Space(3f);

                entry.ShowAdvanced = EditorGUILayout.Foldout(
                    entry.ShowAdvanced,
                    "Advanced settings",
                    true,
                    EditorStyles.foldoutHeader);

                if (entry.ShowAdvanced)
                {
                    DrawAdvancedSettings(entry.SerializedObject, entry.Asset.Role);
                }

                EditorGUILayout.Space(2f);
                EditorGUILayout.LabelField(entry.Path, pathStyle);
            }

            if (entry.SerializedObject.ApplyModifiedProperties())
            {
                UpdateEntryIcon(entry, idProperty == null ? entry.Asset.name : idProperty.stringValue);
                Repaint();
            }
        }

        void DrawCardHeader(CatalogEntry entry, string id)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var iconRect = GUILayoutUtility.GetRect(
                    IconSize,
                    IconSize,
                    GUILayout.Width(IconSize),
                    GUILayout.Height(IconSize));
                DrawSpeciesIcon(iconRect, entry, id);

                using (new EditorGUILayout.VerticalScope(GUILayout.MinHeight(IconSize)))
                {
                    EditorGUILayout.LabelField(GetDisplayName(id, entry.Asset.name), cardTitleStyle);

                    var badgeRect = GUILayoutUtility.GetRect(96f, 20f, GUILayout.Width(96f), GUILayout.Height(20f));
                    EditorGUI.DrawRect(badgeRect, GetRoleColor(entry.Asset.Role));
                    GUI.Label(badgeRect, entry.Asset.Role.ToString().ToUpperInvariant(), roleBadgeStyle);

                    GUILayout.FlexibleSpace();
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("Ping", GUILayout.Width(48f)))
                        {
                            EditorGUIUtility.PingObject(entry.Asset);
                        }

                        if (GUILayout.Button("Inspect", GUILayout.Width(58f)))
                        {
                            Selection.activeObject = entry.Asset;
                        }
                    }
                }
            }
        }

        void DrawSpeciesIcon(Rect rect, CatalogEntry entry, string id)
        {
            EditorGUI.DrawRect(rect, GetRoleColor(entry.Asset.Role) * 0.55f);
            var innerRect = new Rect(rect.x + 3f, rect.y + 3f, rect.width - 6f, rect.height - 6f);
            if (entry.Icon != null)
            {
                GUI.DrawTexture(innerRect, entry.Icon, ScaleMode.ScaleToFit, true);
            }
            else
            {
                var fallback = string.IsNullOrWhiteSpace(id) ? "?" : id.Trim().Substring(0, 1).ToUpperInvariant();
                GUI.Label(innerRect, fallback, fallbackIconStyle);
            }
        }

        void DrawValidation(CatalogEntry entry, string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                EditorGUILayout.HelpBox("Species ID is required.", MessageType.Error);
                return;
            }

            if (IsDuplicateId(id))
            {
                EditorGUILayout.HelpBox($"Species ID '{id.Trim()}' is duplicated.", MessageType.Error);
            }

            var startingEnergy = Find(entry.SerializedObject, "startingEnergy");
            var maximumEnergy = Find(entry.SerializedObject, "maximumEnergy");
            if (startingEnergy != null
                && maximumEnergy != null
                && maximumEnergy.intValue > 0
                && startingEnergy.intValue > maximumEnergy.intValue)
            {
                EditorGUILayout.HelpBox("Starting energy is above the energy cap.", MessageType.Warning);
            }

            var litterMinimum = Find(entry.SerializedObject, "litterMinimum");
            var litterMaximum = Find(entry.SerializedObject, "litterMaximum");
            if (litterMinimum != null && litterMaximum != null && litterMaximum.intValue < litterMinimum.intValue)
            {
                EditorGUILayout.HelpBox("Maximum litter size is below the minimum.", MessageType.Warning);
            }
        }

        void DrawPlantOverview(SerializedObject serializedObject, float cardWidth)
        {
            DrawPropertyPair(serializedObject, "startingEnergy", "Start Energy", "energyValue", "Food Value", cardWidth);
            DrawPropertyPair(serializedObject, "metabolism", "Metabolism", "energyLossIntervalTicks", "Loss Interval", cardWidth);
            DrawPropertyPair(serializedObject, "startingFoodReserve", "Food Reserve", "crowdingEnergyPenalty", "Crowding", cardWidth);
            DrawPropertyPair(serializedObject, "reproductionChance", "Reproduction", "reproductionNeighborCount", "Neighbors", cardWidth);
            DrawPropertyPair(serializedObject, "wiltChance", "Wilt Chance", "seedDropChance", "Seed Drop", cardWidth);
        }

        void DrawAnimalOverview(SerializedObject serializedObject, float cardWidth)
        {
            DrawPropertyPair(serializedObject, "movementSpeed", "Movement", "visionRange", "Vision", cardWidth);
            DrawPropertyPair(serializedObject, "startingEnergy", "Start Energy", "maximumEnergy", "Energy Cap", cardWidth);
            DrawPropertyPair(serializedObject, "metabolism", "Metabolism", "energyLossIntervalTicks", "Loss Interval", cardWidth);
            DrawPropertyPair(serializedObject, "attackAmount", "Attack", "blockAmount", "Block", cardWidth);
            DrawPropertyPair(serializedObject, "dietTargetId", "Diet", "energyValue", "Food Value", cardWidth);
            DrawPropertyPair(serializedObject, "forageBelowEnergy", "Forage Below", "reproductionChance", "Reproduction", cardWidth);
            DrawPropertyPair(serializedObject, "litterMinimum", "Litter Min", "litterMaximum", "Litter Max", cardWidth);
        }

        void DrawPatternSummary(SerializedObject serializedObject)
        {
            var movementCount = GetArraySize(serializedObject, "movementPattern");
            var attackCount = GetArraySize(serializedObject, "attackPattern");
            var dietCount = GetArraySize(serializedObject, "dietPattern");
            var reproductionCount = GetArraySize(serializedObject, "reproductionPattern");
            EditorGUILayout.LabelField(
                $"Pattern cells   Move {movementCount}   Attack {attackCount}   Diet {dietCount}   Reproduce {reproductionCount}",
                EditorStyles.centeredGreyMiniLabel);
        }

        void DrawAdvancedSettings(SerializedObject serializedObject, SpeciesRole role)
        {
            if (role == SpeciesRole.Plant)
            {
                DrawSectionLabel("ADDITIONAL SHARED STATS");
                DrawProperty(Find(serializedObject, "movementSpeed"), "Movement Speed");
                DrawProperty(Find(serializedObject, "visionRange"), "Vision Range");
                DrawProperty(Find(serializedObject, "maximumEnergy"), "Maximum Energy");
                DrawProperty(Find(serializedObject, "attackAmount"), "Attack Amount");
                DrawProperty(Find(serializedObject, "blockAmount"), "Block Amount");
                DrawProperty(Find(serializedObject, "dietTargetId"), "Diet Target ID");
                DrawProperty(Find(serializedObject, "forageBelowEnergy"), "Forage Below Energy");
                DrawProperty(Find(serializedObject, "litterMinimum"), "Minimum Litter Size");
                DrawProperty(Find(serializedObject, "litterMaximum"), "Maximum Litter Size");
            }

            DrawSectionLabel("PATTERNS");
            DrawProperty(Find(serializedObject, "movementPattern"), "Movement Pattern", includeChildren: true);
            DrawProperty(Find(serializedObject, "attackPattern"), "Attack Pattern", includeChildren: true);
            DrawProperty(Find(serializedObject, "blockPattern"), "Block Pattern", includeChildren: true);
            DrawProperty(Find(serializedObject, "dietPattern"), "Diet Pattern", includeChildren: true);
            DrawProperty(Find(serializedObject, "reproductionPattern"), "Reproduction Pattern", includeChildren: true);

            DrawSectionLabel("FEEDING & ENERGY");
            DrawProperty(Find(serializedObject, "foragesUntilFull"), "Forages Until Full");
            DrawProperty(Find(serializedObject, "forageThresholdFraction"), "Forage Threshold Fraction");
            DrawProperty(Find(serializedObject, "matingEnergyThresholdFraction"), "Mating Threshold Fraction");
            DrawProperty(Find(serializedObject, "matingEnergyCostFraction"), "Mating Cost Fraction");

            DrawSectionLabel("REPRODUCTION & ECOLOGY");
            DrawProperty(Find(serializedObject, "reproductionNeighborCount"), "Required Neighbors");
            DrawProperty(Find(serializedObject, "reproductionFoodRequired"), "Food Required");
            DrawProperty(Find(serializedObject, "maxReproductionGroupSize"), "Maximum Group Size");
            DrawProperty(Find(serializedObject, "crowdingEnergyPenalty"), "Crowding Energy Penalty");
            DrawProperty(Find(serializedObject, "startingFoodReserve"), "Starting Food Reserve");
            DrawProperty(Find(serializedObject, "wiltChance"), "Wilt Chance");
            DrawProperty(Find(serializedObject, "seedDropChance"), "Seed Drop Chance");

            DrawSectionLabel("BEHAVIOR");
            DrawProperty(Find(serializedObject, "intelligence"), "Intelligence");
            DrawProperty(Find(serializedObject, "behaviorStateRules"), "State Overrides", includeChildren: true);

            DrawSectionLabel("ALPHA OFFSPRING");
            DrawProperty(Find(serializedObject, "alphaChance"), "Alpha Chance");
            DrawProperty(Find(serializedObject, "alphaHealthBonus"), "Health Bonus");
            DrawProperty(Find(serializedObject, "alphaEnergyBonus"), "Energy Bonus");
        }

        void DrawPropertyPair(
            SerializedObject serializedObject,
            string leftName,
            string leftLabel,
            string rightName,
            string rightLabel,
            float cardWidth)
        {
            var left = Find(serializedObject, leftName);
            var right = Find(serializedObject, rightName);
            if (left == null && right == null)
            {
                return;
            }

            var columnWidth = Mathf.Max(130f, (cardWidth - 34f) * 0.5f);
            var previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = Mathf.Clamp(columnWidth * 0.53f, 68f, 96f);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (left != null)
                {
                    EditorGUILayout.PropertyField(left, new GUIContent(leftLabel), GUILayout.Width(columnWidth));
                }

                if (right != null)
                {
                    EditorGUILayout.PropertyField(right, new GUIContent(rightLabel), GUILayout.Width(columnWidth));
                }
            }

            EditorGUIUtility.labelWidth = previousLabelWidth;
        }

        void DrawProperty(SerializedProperty property, string label, bool includeChildren = false)
        {
            if (property != null)
            {
                EditorGUILayout.PropertyField(property, new GUIContent(label), includeChildren);
            }
        }

        void DrawSectionLabel(string label)
        {
            EditorGUILayout.Space(5f);
            EditorGUILayout.LabelField(label, sectionStyle);
        }

        List<CatalogEntry> GetVisibleEntries()
        {
            var visibleEntries = new List<CatalogEntry>();
            foreach (var entry in entries)
            {
                if (IsVisible(entry))
                {
                    visibleEntries.Add(entry);
                }
            }

            return visibleEntries;
        }

        bool IsVisible(CatalogEntry entry)
        {
            if (entry.Asset == null || !MatchesRoleFilter(entry.Asset.Role))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(searchText))
            {
                return true;
            }

            var query = searchText.Trim();
            var id = TryGetId(entry, out var currentId) ? currentId : string.Empty;
            return ContainsIgnoreCase(id, query)
                || ContainsIgnoreCase(entry.Asset.name, query)
                || ContainsIgnoreCase(entry.Asset.Role.ToString(), query);
        }

        bool MatchesRoleFilter(SpeciesRole role)
        {
            return roleFilter == 0
                || (roleFilter == 1 && role == SpeciesRole.Plant)
                || (roleFilter == 2 && role == SpeciesRole.Herbivore)
                || (roleFilter == 3 && role == SpeciesRole.Carnivore);
        }

        void RefreshCatalog()
        {
            var advancedByPath = new Dictionary<string, bool>(StringComparer.Ordinal);
            foreach (var entry in entries)
            {
                advancedByPath[entry.Path] = entry.ShowAdvanced;
            }

            entries.Clear();
            CacheSpeciesArtPaths();

            var guids = AssetDatabase.FindAssets("t:SpeciesDefinitionAsset", new[] { CatalogPath });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<SpeciesDefinitionAsset>(path);
                if (asset == null)
                {
                    continue;
                }

                var entry = new CatalogEntry
                {
                    Path = path,
                    Asset = asset,
                    SerializedObject = new SerializedObject(asset),
                    ShowAdvanced = advancedByPath.TryGetValue(path, out var wasExpanded) && wasExpanded,
                };

                TryGetId(entry, out var id);
                UpdateEntryIcon(entry, id);
                entries.Add(entry);
            }

            entries.Sort(CompareEntries);
            Repaint();
        }

        void CacheSpeciesArtPaths()
        {
            speciesArtPaths.Clear();
            foreach (var guid in AssetDatabase.FindAssets("t:Texture2D", new[] { SpeciesArtPath }))
            {
                speciesArtPaths.Add(AssetDatabase.GUIDToAssetPath(guid));
            }

            speciesArtPaths.Sort(StringComparer.Ordinal);
        }

        void UpdateEntryIcon(CatalogEntry entry, string id)
        {
            entry.IconKey = id ?? string.Empty;
            entry.Icon = ResolveSpeciesIcon(entry.IconKey, entry.Asset.Role);
        }

        Texture2D ResolveSpeciesIcon(string id, SpeciesRole role)
        {
            var searchKey = NormalizeIconKey(id);
            if (searchKey == "hare")
            {
                searchKey = "rabbit";
            }
            else if (searchKey == "herbivore")
            {
                searchKey = "rabbit";
            }
            else if (searchKey == "carnivore")
            {
                searchKey = "wolf";
            }

            if (string.IsNullOrEmpty(searchKey) || role == SpeciesRole.Plant)
            {
                return null;
            }

            string bestPath = null;
            var bestScore = int.MinValue;
            foreach (var path in speciesArtPaths)
            {
                var fileName = NormalizeIconKey(Path.GetFileNameWithoutExtension(path));
                var score = ScoreIconPath(fileName, path, searchKey);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestPath = path;
                }
            }

            return bestScore > 0 ? AssetDatabase.LoadAssetAtPath<Texture2D>(bestPath) : null;
        }

        static int ScoreIconPath(string fileName, string path, string searchKey)
        {
            var score = 0;
            if (fileName == searchKey)
            {
                score = 120;
            }
            else if (fileName.EndsWith(searchKey, StringComparison.Ordinal))
            {
                score = 100;
            }
            else if (fileName.Contains(searchKey))
            {
                score = 70;
            }

            if (score == 0)
            {
                return 0;
            }

            if (path.IndexOf("/64/", StringComparison.Ordinal) >= 0)
            {
                score += 20;
            }

            if (path.IndexOf("/Standardized/", StringComparison.Ordinal) >= 0)
            {
                score += 10;
            }

            return score;
        }

        void UpdateIdCounts()
        {
            idCounts.Clear();
            foreach (var entry in entries)
            {
                if (!TryGetId(entry, out var id) || string.IsNullOrWhiteSpace(id))
                {
                    continue;
                }

                id = id.Trim();
                idCounts[id] = idCounts.TryGetValue(id, out var count) ? count + 1 : 1;
            }
        }

        bool IsDuplicateId(string id)
        {
            return !string.IsNullOrWhiteSpace(id)
                && idCounts.TryGetValue(id.Trim(), out var count)
                && count > 1;
        }

        static bool TryGetId(CatalogEntry entry, out string id)
        {
            id = string.Empty;
            if (entry.SerializedObject == null)
            {
                return false;
            }

            entry.SerializedObject.UpdateIfRequiredOrScript();
            var idProperty = entry.SerializedObject.FindProperty("id");
            if (idProperty == null)
            {
                return false;
            }

            id = idProperty.stringValue;
            return true;
        }

        void SetAdvancedVisibility(bool visible)
        {
            foreach (var entry in entries)
            {
                entry.ShowAdvanced = visible;
            }
        }

        void HandleUndoRedo()
        {
            foreach (var entry in entries)
            {
                if (TryGetId(entry, out var id) && !string.Equals(id, entry.IconKey, StringComparison.Ordinal))
                {
                    UpdateEntryIcon(entry, id);
                }
            }

            Repaint();
        }

        void EnsureStyles()
        {
            if (titleStyle != null)
            {
                return;
            }

            titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 21,
                fixedHeight = 28f,
            };
            subtitleStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 12,
                normal = { textColor = EditorGUIUtility.isProSkin ? new Color(0.72f, 0.76f, 0.80f) : new Color(0.30f, 0.34f, 0.38f) },
            };
            cardStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(11, 11, 11, 9),
                margin = new RectOffset(0, 0, 0, 0),
            };
            cardTitleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 15,
                clipping = TextClipping.Clip,
            };
            roleBadgeStyle = new GUIStyle(EditorStyles.miniBoldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white },
            };
            sectionStyle = new GUIStyle(EditorStyles.miniBoldLabel)
            {
                normal = { textColor = EditorGUIUtility.isProSkin ? new Color(0.58f, 0.78f, 0.88f) : new Color(0.18f, 0.42f, 0.54f) },
            };
            fallbackIconStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 28,
                normal = { textColor = Color.white },
            };
            pathStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                clipping = TextClipping.Clip,
            };
        }

        static int CompareEntries(CatalogEntry left, CatalogEntry right)
        {
            var roleComparison = left.Asset.Role.CompareTo(right.Asset.Role);
            if (roleComparison != 0)
            {
                return roleComparison;
            }

            TryGetId(left, out var leftId);
            TryGetId(right, out var rightId);
            return string.Compare(leftId, rightId, StringComparison.OrdinalIgnoreCase);
        }

        static SerializedProperty Find(SerializedObject serializedObject, string propertyName)
        {
            return serializedObject == null ? null : serializedObject.FindProperty(propertyName);
        }

        static int GetArraySize(SerializedObject serializedObject, string propertyName)
        {
            var property = Find(serializedObject, propertyName);
            return property != null && property.isArray ? property.arraySize : 0;
        }

        static string GetDisplayName(string id, string fallback)
        {
            var value = string.IsNullOrWhiteSpace(id) ? fallback : id.Trim();
            if (string.IsNullOrEmpty(value))
            {
                return "Unnamed Species";
            }

            return char.ToUpperInvariant(value[0]) + value.Substring(1);
        }

        static bool ContainsIgnoreCase(string value, string query)
        {
            return !string.IsNullOrEmpty(value)
                && value.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        static string NormalizeIconKey(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return value.Trim()
                .Replace("_", string.Empty)
                .Replace("-", string.Empty)
                .Replace(" ", string.Empty)
                .ToLowerInvariant();
        }

        static Color GetRoleColor(SpeciesRole role)
        {
            switch (role)
            {
                case SpeciesRole.Plant:
                    return new Color(0.24f, 0.55f, 0.31f, 1f);
                case SpeciesRole.Herbivore:
                    return new Color(0.23f, 0.49f, 0.67f, 1f);
                case SpeciesRole.Carnivore:
                    return new Color(0.66f, 0.30f, 0.27f, 1f);
                default:
                    return new Color(0.40f, 0.40f, 0.40f, 1f);
            }
        }
    }
}
