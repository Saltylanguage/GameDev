using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SaltyGame
{
    /// <summary>
    /// Presentation model for the authored Genome catalog. It exposes only
    /// immutable snapshot data; unlock/activation commands belong to the
    /// profile feature pass and are intentionally absent for now.
    /// </summary>
    public sealed class GenomeLabViewModel : INotifyPropertyChanged
    {
        internal const float NodeWidth = 156f;
        internal const float NodeHeight = 112f;
        const float ColumnGap = 92f;
        const float RowGap = 28f;
        const float TreePadding = 24f;
        const float TreeHeaderHeight = 42f;
        const float ConnectorThickness = 4f;

        GenomeCatalogSnapshot catalog;
        readonly ProfileSessionSnapshot profile;
        SpeciesId? selectedSpecies;

        public GenomeLabViewModel(
            GenomeCatalogSnapshot catalog,
            ProfileSessionSnapshot profile = null,
            SpeciesId? selectedSpecies = null)
        {
            this.profile = profile;
            this.selectedSpecies = selectedSpecies;
            Nodes = new ObservableCollection<GenomeLabNodeViewModel>();
            GenomeTreeSegments = new ObservableCollection<GenomeLabTreeSegmentViewModel>();
            BindCatalog(catalog);
        }

        public ObservableCollection<GenomeLabNodeViewModel> Nodes { get; }
        public ObservableCollection<GenomeLabTreeSegmentViewModel> GenomeTreeSegments { get; }
        public GenomeLabNodeViewModel SelectedNode => Nodes.Count == 0 ? null : Nodes[0];
        public string SpeciesNameText { get; private set; }
        public string SpeciesIdText { get; private set; }
        public string ActiveGenomeCapacityText { get; private set; }
        public string CatalogStateText { get; private set; }
        public string NodeCountText => $"{Nodes.Count} NODE{(Nodes.Count == 1 ? string.Empty : "S")}";
        public float GenomeTreeWidth { get; private set; }
        public float GenomeTreeHeight { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void BindCatalog(GenomeCatalogSnapshot nextCatalog)
        {
            catalog = nextCatalog ?? GenomeCatalogSnapshot.Empty;
            var speciesMap = ResolveSpeciesMap();
            Nodes.Clear();
            GenomeTreeSegments.Clear();
            GenomeTreeWidth = 560f;
            GenomeTreeHeight = 300f;

            if (speciesMap == null)
            {
                SpeciesNameText = "NO GENOME MAP";
                SpeciesIdText = string.Empty;
                ActiveGenomeCapacityText = $"0 / {GenomeContract.ActiveCapacity}";
                CatalogStateText = catalog.SpeciesMaps.Count == 0
                    ? "NO AUTHORING SNAPSHOT"
                    : "SPECIES MAP NOT FOUND";
            }
            else
            {
                SpeciesNameText = speciesMap.SpeciesId.Value.ToUpperInvariant();
                SpeciesIdText = speciesMap.SpeciesId.Value;
                ActiveGenomeCapacityText = GetActiveCapacityText(speciesMap.SpeciesId);
                var fingerprintPrefix = catalog.Fingerprint.Length < 8
                    ? catalog.Fingerprint
                    : catalog.Fingerprint.Substring(0, 8);
                CatalogStateText = $"AUTHORING SNAPSHOT · {fingerprintPrefix.ToUpperInvariant()}";

                var nodeLayouts = CreateNodeLayouts(speciesMap, out var treeWidth, out var treeHeight);
                foreach (var node in speciesMap.Nodes)
                {
                    var layout = nodeLayouts[node.Id];
                    Nodes.Add(new GenomeLabNodeViewModel(node, profile, layout.Left, layout.Top));
                    AddTreeSegments(node, nodeLayouts);
                }

                GenomeTreeWidth = treeWidth;
                GenomeTreeHeight = treeHeight;
            }

            OnPropertyChanged(nameof(Nodes));
            OnPropertyChanged(nameof(GenomeTreeSegments));
            OnPropertyChanged(nameof(SelectedNode));
            OnPropertyChanged(nameof(SpeciesNameText));
            OnPropertyChanged(nameof(SpeciesIdText));
            OnPropertyChanged(nameof(ActiveGenomeCapacityText));
            OnPropertyChanged(nameof(CatalogStateText));
            OnPropertyChanged(nameof(NodeCountText));
            OnPropertyChanged(nameof(GenomeTreeWidth));
            OnPropertyChanged(nameof(GenomeTreeHeight));
        }

        public void SelectSpecies(string speciesId)
        {
            if (string.IsNullOrWhiteSpace(speciesId))
            {
                return;
            }

            var nextSpecies = new SpeciesId(speciesId);
            if (selectedSpecies.HasValue && selectedSpecies.Value == nextSpecies)
            {
                return;
            }

            selectedSpecies = nextSpecies;
            BindCatalog(catalog);
        }

        Dictionary<string, GenomeNodeLayout> CreateNodeLayouts(
            SpeciesGenomeMapSnapshot speciesMap,
            out float treeWidth,
            out float treeHeight)
        {
            var depths = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var node in speciesMap.Nodes)
            {
                GetNodeDepth(node.Id, speciesMap, depths);
            }

            var nextRowsByDepth = new Dictionary<int, int>();
            var layouts = new Dictionary<string, GenomeNodeLayout>(StringComparer.Ordinal);
            var maxDepth = 0;
            var maxRows = 0;
            foreach (var node in speciesMap.Nodes)
            {
                var depth = depths[node.Id];
                var row = nextRowsByDepth.TryGetValue(depth, out var nextRow)
                    ? nextRow
                    : 0;
                nextRowsByDepth[depth] = row + 1;
                maxDepth = Math.Max(maxDepth, depth);
                maxRows = Math.Max(maxRows, row + 1);

                layouts.Add(
                    node.Id,
                    new GenomeNodeLayout(
                        TreePadding + depth * (NodeWidth + ColumnGap),
                        TreeHeaderHeight + TreePadding + row * (NodeHeight + RowGap)));
            }

            treeWidth = TreePadding * 2f
                + (maxDepth + 1) * NodeWidth
                + maxDepth * ColumnGap;
            treeHeight = TreeHeaderHeight
                + TreePadding * 2f
                + maxRows * NodeHeight
                + Math.Max(0, maxRows - 1) * RowGap;
            return layouts;
        }

        int GetNodeDepth(
            string nodeId,
            SpeciesGenomeMapSnapshot speciesMap,
            IDictionary<string, int> depths)
        {
            if (depths.TryGetValue(nodeId, out var depth))
            {
                return depth;
            }

            var node = speciesMap.GetNode(nodeId);
            var maxPrerequisiteDepth = -1;
            foreach (var prerequisiteId in node.PrerequisiteNodeIds)
            {
                maxPrerequisiteDepth = Math.Max(
                    maxPrerequisiteDepth,
                    GetNodeDepth(prerequisiteId, speciesMap, depths));
            }

            depth = maxPrerequisiteDepth + 1;
            depths.Add(nodeId, depth);
            return depth;
        }

        void AddTreeSegments(
            GenomeUpgradeNodeSnapshot child,
            IReadOnlyDictionary<string, GenomeNodeLayout> layouts)
        {
            var childLayout = layouts[child.Id];
            foreach (var prerequisiteId in child.PrerequisiteNodeIds)
            {
                var parentLayout = layouts[prerequisiteId];
                var startX = parentLayout.Right;
                var endX = childLayout.Left;
                var midpointX = startX + (endX - startX) * 0.5f;
                var parentCenterY = parentLayout.CenterY;
                var childCenterY = childLayout.CenterY;
                var verticalTop = Math.Min(parentCenterY, childCenterY) - ConnectorThickness * 0.5f;
                var verticalHeight = Math.Max(
                    ConnectorThickness,
                    Math.Abs(childCenterY - parentCenterY));

                GenomeTreeSegments.Add(new GenomeLabTreeSegmentViewModel(
                    startX,
                    parentCenterY - ConnectorThickness * 0.5f,
                    midpointX - startX,
                    ConnectorThickness));
                GenomeTreeSegments.Add(new GenomeLabTreeSegmentViewModel(
                    midpointX - ConnectorThickness * 0.5f,
                    verticalTop,
                    ConnectorThickness,
                    verticalHeight));
                GenomeTreeSegments.Add(new GenomeLabTreeSegmentViewModel(
                    midpointX,
                    childCenterY - ConnectorThickness * 0.5f,
                    endX - midpointX,
                    ConnectorThickness));
            }
        }

        SpeciesGenomeMapSnapshot ResolveSpeciesMap()
        {
            if (selectedSpecies.HasValue)
            {
                return catalog.TryGetSpeciesMap(selectedSpecies.Value, out var requestedMap)
                    ? requestedMap
                    : null;
            }

            return catalog.SpeciesMaps.Count == 0
                ? null
                : catalog.SpeciesMaps[0];
        }

        string GetActiveCapacityText(SpeciesId speciesId)
        {
            if (profile == null || !profile.TryGetGenomeProfile(speciesId, out var speciesProfile))
            {
                return $"0 / {GenomeContract.ActiveCapacity}";
            }

            return $"{speciesProfile.ActiveNodeIds.Count} / {GenomeContract.ActiveCapacity}";
        }

        void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    public sealed class GenomeLabNodeViewModel
    {
        public GenomeLabNodeViewModel(
            GenomeUpgradeNodeSnapshot node,
            ProfileSessionSnapshot profile = null,
            float left = 0f,
            float top = 0f)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            NodeId = node.Id;
            DisplayName = node.DisplayName;
            Description = string.IsNullOrWhiteSpace(node.Description)
                ? "Genome node details pending effect contract."
                : node.Description;
            PrerequisiteText = node.PrerequisiteNodeIds.Count == 0
                ? "ROOT NODE"
                : $"REQUIRES · {string.Join(", ", node.PrerequisiteNodeIds)}";
            StateText = GetStateText(node, profile);
            Fingerprint = node.Fingerprint;
            Left = left;
            Top = top;
        }

        public string NodeId { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public string PrerequisiteText { get; }
        public string StateText { get; }
        public string Fingerprint { get; }
        public float Left { get; }
        public float Top { get; }

        static string GetStateText(GenomeUpgradeNodeSnapshot node, ProfileSessionSnapshot profile)
        {
            if (profile == null || !profile.TryGetGenomeProfile(node.TargetSpecies, out var speciesProfile))
            {
                return "CATALOG ENTRY";
            }

            if (speciesProfile.IsNodeActive(node.Id))
            {
                return "ACTIVE";
            }

            return speciesProfile.IsNodeUnlocked(node.Id)
                ? "UNLOCKED"
                : "LOCKED";
        }
    }

    public sealed class GenomeLabTreeSegmentViewModel
    {
        public GenomeLabTreeSegmentViewModel(float left, float top, float width, float height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }

        public float Left { get; }
        public float Top { get; }
        public float Width { get; }
        public float Height { get; }
    }

    sealed class GenomeNodeLayout
    {
        public GenomeNodeLayout(float left, float top)
        {
            Left = left;
            Top = top;
        }

        public float Left { get; }
        public float Top { get; }
        public float Right => Left + GenomeLabViewModel.NodeWidth;
        public float CenterY => Top + GenomeLabViewModel.NodeHeight * 0.5f;
    }
}
