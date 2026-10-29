using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Noesis;

namespace SaltyGame
{
    /// <summary>
    /// Presentation model for the authored Genome catalog and profile actions.
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
        const string AllBranches = "All branches";

        GenomeCatalogSnapshot catalog;
        ProfileSessionSnapshot profile;
        readonly Helper_ProfileSession profileSession;
        readonly SpeciesId? selectedSpecies;
        string selectedNodeId;
        string selectedBranch;

        public GenomeLabViewModel(
            GenomeCatalogSnapshot catalog,
            ProfileSessionSnapshot profile = null,
            SpeciesId? selectedSpecies = null,
            Helper_ProfileSession profileSession = null)
        {
            this.profile = profile;
            this.selectedSpecies = selectedSpecies;
            this.profileSession = profileSession;
            PurchaseLicenseCommand = new DelegateCommand(PurchaseLicense);
            PurchaseNodeCommand = new DelegateCommand(PurchaseNode);
            ResetGenomeCommand = new DelegateCommand(ResetGenome);
            Nodes = new ObservableCollection<GenomeLabNodeViewModel>();
            GenomeTreeSegments = new ObservableCollection<GenomeLabTreeSegmentViewModel>();
            BranchNames = new ObservableCollection<string>();
            BindCatalog(catalog);
        }

        public ObservableCollection<GenomeLabNodeViewModel> Nodes { get; }
        public ObservableCollection<GenomeLabTreeSegmentViewModel> GenomeTreeSegments { get; }
        public GenomeLabNodeViewModel SelectedNode => Nodes.FirstOrDefault(node => node.NodeId == selectedNodeId);
        public ObservableCollection<string> BranchNames { get; }
        public IReadOnlyList<bool> CapacitySlots { get; private set; } = Array.Empty<bool>();
        public string SelectedBranch
        {
            get => selectedBranch;
            set
            {
                if (value == selectedBranch || !BranchNames.Contains(value))
                {
                    return;
                }
                selectedBranch = value;
                actionResult = null;
                BindCatalog(catalog);
            }
        }
        public DelegateCommand PurchaseLicenseCommand { get; }
        public DelegateCommand PurchaseNodeCommand { get; }
        public DelegateCommand ResetGenomeCommand { get; }
        public string RabbitDataBalanceText => (profile?.RabbitData ?? 0).ToString();
        public string ResearchDataBalanceText => (profile?.ResearchData ?? 0).ToString();
        public string CompletedRunsText => (profile?.CompletedRuns ?? 0).ToString();
        public string LicenseOfferText => $"TREE ACCESS · {GenomeContract.RabbitTreeLicensePrice} DATA";
        public string NodeOfferText => SelectedNode == null
            ? "NO NODE SELECTED"
            : !SelectedNode.IsPurchasable ? "PREVIEW ONLY"
            : profile != null && profile.TryGetGenomeProfile(new SpeciesId("hare"), out var genome)
                && genome.IsNodeActive(SelectedNode.NodeId)
                ? GetSelectedNodeRank(genome) < SelectedNode.MaximumRanks
                    ? $"UPGRADE · RANK {GetSelectedNodeRank(genome) + 1}/{SelectedNode.MaximumRanks} / {SelectedNode.RabbitDataPrice} DATA +1 POINT"
                    : $"RANK {GetSelectedNodeRank(genome)}/{SelectedNode.MaximumRanks} · {GetSelectedNodePoints(genome)} POINTS"
                : $"ACTIVATE · {SelectedNode.RabbitDataPrice} DATA / {SelectedNode.CapacityPoints} POINTS";
        public string ResetOfferText => $"RESET · {GenomeContract.RabbitResetResearchDataFee} RESEARCH DATA";
        string actionResult;
        public string ActionStatusText => !string.IsNullOrEmpty(actionResult) ? actionResult
            : GetPurchaseBlockReason() ?? "Ready to activate for the next journey.";
        public bool CanPurchaseLicense => profileSession != null && profile != null
            && profile.HasLoadedProfile && profile.GeneLabUnlocked
            && SpeciesIdText == GenomeContract.RabbitSpeciesId.Value
            && !HasRabbitLicense && profile.RabbitData >= GenomeContract.RabbitTreeLicensePrice;
        public bool CanPurchaseNode => GetPurchaseBlockReason() == null;

        string GetPurchaseBlockReason()
        {
            var node = SelectedNode;
            if (profileSession == null || profile == null || !profile.HasLoadedProfile)
            {
                return "Select a profile to use Gene Lab.";
            }
            if (!profile.GeneLabUnlocked)
            {
                return $"Complete 3 successful journeys to unlock Gene Lab ({profile.CompletedRuns}/3).";
            }
            if (SpeciesIdText != GenomeContract.RabbitSpeciesId.Value)
            {
                return "Only Rabbit Genome allocation is available in this playtest.";
            }
            if (!HasRabbitLicense)
            {
                return "Unlock the Rabbit Genome tree first.";
            }
            if (node == null)
            {
                return "Select a Genome node.";
            }
            if (!node.IsPurchasable)
            {
                return "Preview only. This node's mechanic is not implemented.";
            }
            profile.TryGetGenomeProfile(GenomeContract.RabbitSpeciesId, out var genome);
            var isActive = genome != null && genome.IsNodeActive(node.NodeId);
            var currentPoints = isActive ? GetSelectedNodePoints(genome) : 0;
            if (isActive && currentPoints >= node.MaximumCapacityPoints)
                return "This Genome node is at maximum rank.";
            var missing = node.PrerequisiteNodeIds.Where(id => genome == null || !genome.IsNodeActive(id))
                .Select(id => ResolveSpeciesMap().GetNode(id).DisplayName).ToArray();
            if (missing.Length > 0)
            {
                return $"Activate first: {string.Join(", ", missing)}.";
            }
            var free = GenomeContract.ActiveCapacity - (genome?.CapacityUsed ?? 0);
            if (free < (isActive ? 1 : node.CapacityPoints))
            {
                var needed = isActive ? 1 : node.CapacityPoints;
                return $"Needs {needed} capacity; {free} available. Reset to reallocate.";
            }
            if (profile.RabbitData < node.RabbitDataPrice)
            {
                return $"Needs {node.RabbitDataPrice} Rabbit Data; you have {profile.RabbitData}.";
            }
            return null;
        }
        public bool CanResetGenome
        {
            get
            {
                if (profileSession == null || profile == null || !HasRabbitLicense
                    || SpeciesIdText != GenomeContract.RabbitSpeciesId.Value
                    || profile.ResearchData < GenomeContract.RabbitResetResearchDataFee)
                {
                    return false;
                }
                return profile.TryGetGenomeProfile(new SpeciesId("hare"), out var genome)
                    && genome.ActiveNodeIds.Count > 0;
            }
        }
        bool HasRabbitLicense => profile != null
            && profile.UnlockedGenomeSpeciesIds.Contains(new SpeciesId("hare"));
        public string SpeciesNameText { get; private set; }
        public string SpeciesIdText { get; private set; }
        public string ActiveGenomeCapacityText { get; private set; }
        public string CatalogStateText { get; private set; }
        public string NodeCountText => $"{Nodes.Count} NODE{(Nodes.Count == 1 ? string.Empty : "S")}";
        public float GenomeTreeWidth { get; private set; }
        public float GenomeTreeHeight { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void BindProfile(ProfileSessionSnapshot nextProfile)
        {
            profile = nextProfile;
            actionResult = null;
            BindCatalog(catalog);
        }

        void PurchaseLicense(object _)
        {
            if (!CanPurchaseLicense)
            {
                return;
            }
            var succeeded = profileSession.TryPurchaseRabbitGenomeLicense(
                GenomeContract.RabbitTreeLicensePrice, out var reason);
            BindProfile(profileSession.Current);
            SetActionResult(succeeded ? "Rabbit Genome unlocked." : reason);
        }

        void PurchaseNode(object _)
        {
            if (!CanPurchaseNode)
            {
                return;
            }
            var node = SelectedNode;
            var isActive = profile.TryGetGenomeProfile(GenomeContract.RabbitSpeciesId, out var genome)
                && genome.IsNodeActive(node.NodeId);
            var succeeded = isActive
                ? profileSession.TryUpgradeRabbitGenomeNode(ResolveSpeciesMap(), node.NodeId, out var reason)
                : profileSession.TryPurchaseRabbitGenomeNode(ResolveSpeciesMap(), node.NodeId,
                    node.RabbitDataPrice, node.CapacityPoints, out reason);
            BindProfile(profileSession.Current);
            SetActionResult(succeeded ? $"{node.DisplayName} updated for the next journey." : reason);
        }

        int GetSelectedNodeRank(SpeciesGenomeProfile genome) =>
            genome.TryGetAllocation(SelectedNode.NodeId, out var allocation)
                ? SelectedNode.GetRank(allocation.CapacityPoints) : 1;

        int GetSelectedNodePoints(SpeciesGenomeProfile genome) =>
            genome.TryGetAllocation(SelectedNode.NodeId, out var allocation)
                ? allocation.CapacityPoints : SelectedNode.CapacityPoints;

        void ResetGenome(object _)
        {
            if (!CanResetGenome)
            {
                return;
            }
            var succeeded = profileSession.TryResetRabbitGenome(
                GenomeContract.RabbitResetResearchDataFee, out var reason);
            BindProfile(profileSession.Current);
            SetActionResult(succeeded ? "Rabbit Genome reset; node Rabbit Data refunded." : reason);
        }

        void SetActionResult(string message)
        {
            actionResult = message;
            OnPropertyChanged(nameof(ActionStatusText));
        }

        void SelectNode(string nodeId)
        {
            selectedNodeId = nodeId;
            actionResult = null;
            foreach (var node in Nodes)
            {
                node.SetSelected(node.NodeId == selectedNodeId);
            }
            OnPropertyChanged(nameof(SelectedNode));
            OnPropertyChanged(nameof(NodeOfferText));
            OnPropertyChanged(nameof(CanPurchaseNode));
            OnPropertyChanged(nameof(ActionStatusText));
        }

        public void BindCatalog(GenomeCatalogSnapshot nextCatalog)
        {
            catalog = nextCatalog ?? GenomeCatalogSnapshot.Empty;
            var speciesMap = ResolveSpeciesMap();
            var branches = new[] { AllBranches }.Concat((speciesMap?.Nodes ?? Array.Empty<GenomeUpgradeNodeSnapshot>())
                .Select(node => node.BranchName).Where(name => !string.IsNullOrEmpty(name)).Distinct()).ToArray();
            if (!BranchNames.SequenceEqual(branches))
            {
                BranchNames.Clear();
                foreach (var branch in branches)
                {
                    BranchNames.Add(branch);
                }
            }
            if (!BranchNames.Contains(selectedBranch))
            {
                selectedBranch = branches.Length > 1 ? branches[1] : AllBranches;
            }
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
                CatalogStateText = $"{speciesMap.Nodes.Count} GENOME OPTION{(speciesMap.Nodes.Count == 1 ? string.Empty : "S")}";

                var visibleNodes = speciesMap.Nodes.Where(node => selectedBranch == AllBranches
                    || node.BranchName == selectedBranch).ToArray();
                var nodeLayouts = CreateNodeLayouts(speciesMap, visibleNodes, out var treeWidth, out var treeHeight);
                foreach (var node in visibleNodes)
                {
                    var layout = nodeLayouts[node.Id];
                    Nodes.Add(new GenomeLabNodeViewModel(node, profile, layout.Left, layout.Top, SelectNode,
                        prerequisiteText: string.Join(", ", node.PrerequisiteNodeIds.Select(id => speciesMap.GetNode(id).DisplayName))));
                    AddTreeSegments(node, nodeLayouts);
                }

                GenomeTreeWidth = treeWidth;
                GenomeTreeHeight = treeHeight;
            }

            if (!Nodes.Any(node => node.NodeId == selectedNodeId))
            {
                selectedNodeId = Nodes.FirstOrDefault()?.NodeId;
            }
            foreach (var node in Nodes)
            {
                node.SetSelected(node.NodeId == selectedNodeId);
            }
            var capacityUsed = profile != null && speciesMap != null && profile.TryGetGenomeProfile(speciesMap.SpeciesId, out var speciesProfile)
                ? speciesProfile.CapacityUsed : 0;
            CapacitySlots = Enumerable.Range(0, GenomeContract.ActiveCapacity).Select(index => index < capacityUsed).ToArray();

            OnPropertyChanged(nameof(Nodes));
            OnPropertyChanged(nameof(BranchNames));
            OnPropertyChanged(nameof(SelectedBranch));
            OnPropertyChanged(nameof(CapacitySlots));
            OnPropertyChanged(nameof(GenomeTreeSegments));
            OnPropertyChanged(nameof(SelectedNode));
            OnPropertyChanged(nameof(SpeciesNameText));
            OnPropertyChanged(nameof(SpeciesIdText));
            OnPropertyChanged(nameof(ActiveGenomeCapacityText));
            OnPropertyChanged(nameof(CatalogStateText));
            OnPropertyChanged(nameof(NodeCountText));
            OnPropertyChanged(nameof(GenomeTreeWidth));
            OnPropertyChanged(nameof(GenomeTreeHeight));
            OnPropertyChanged(nameof(RabbitDataBalanceText));
            OnPropertyChanged(nameof(ResearchDataBalanceText));
            OnPropertyChanged(nameof(CompletedRunsText));
            OnPropertyChanged(nameof(CanPurchaseLicense));
            OnPropertyChanged(nameof(CanResetGenome));
            OnPropertyChanged(nameof(ActionStatusText));
            OnPropertyChanged(nameof(NodeOfferText));
            OnPropertyChanged(nameof(CanPurchaseNode));
        }

        Dictionary<string, GenomeNodeLayout> CreateNodeLayouts(
            SpeciesGenomeMapSnapshot speciesMap,
            IEnumerable<GenomeUpgradeNodeSnapshot> visibleNodes,
            out float treeWidth,
            out float treeHeight)
        {
            var depths = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var node in visibleNodes)
            {
                GetNodeDepth(node.Id, speciesMap, depths);
            }

            var nextRowsByDepth = new Dictionary<int, int>();
            var layouts = new Dictionary<string, GenomeNodeLayout>(StringComparer.Ordinal);
            var maxDepth = 0;
            var maxRows = 0;
            foreach (var node in visibleNodes)
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
                if (!layouts.TryGetValue(prerequisiteId, out var parentLayout))
                {
                    continue;
                }
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

            return $"{speciesProfile.CapacityUsed} / {GenomeContract.ActiveCapacity}";
        }

        void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    public sealed class GenomeLabNodeViewModel : INotifyPropertyChanged
    {
        readonly GenomeUpgradeNodeSnapshot node;

        public GenomeLabNodeViewModel(
            GenomeUpgradeNodeSnapshot node,
            ProfileSessionSnapshot profile = null,
            float left = 0f,
            float top = 0f,
            Action<string> select = null,
            string prerequisiteText = null)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            this.node = node;
            NodeId = node.Id;
            DisplayName = node.DisplayName;
            Description = string.IsNullOrWhiteSpace(node.Description)
                ? "Genome node details pending effect contract."
                : node.Description;
            PrerequisiteText = node.PrerequisiteNodeIds.Count == 0
                ? "ROOT NODE"
                : $"REQUIRES · {prerequisiteText ?? string.Join(", ", node.PrerequisiteNodeIds)}";
            StateText = GetStateText(node, profile);
            RabbitDataPrice = node.RabbitDataPrice;
            CapacityPoints = node.CapacityPoints;
            IsPurchasable = node.IsPurchasable;
            PrerequisiteNodeIds = node.PrerequisiteNodeIds;
            BranchText = string.IsNullOrEmpty(node.BranchName) ? "GENOME NODE" : $"{node.BranchName.ToUpperInvariant()} · GENOME";
            BiomeImpactText = node.Id == "fertile-droppings"
                ? "Droppings may regrow grass but leave scent that hungry predators can follow."
                : node.Modifiers.Any(modifier => modifier.AttributeId == SpeciesAttributeIds.PopulationPoolCapacity)
                    ? "Raises player replenishment capacity. Natural births can still exceed the pool and consume more food."
                : node.Modifiers.Any(modifier => modifier.AttributeId == SpeciesAttributeIds.StartingPopulation)
                    ? "More Hares at launch increase food demand and encounters with predators."
                : node.Modifiers.Count > 0
                    ? "Applies to every Hare next journey. Playtest values; ecology balance is not established."
                    : "Preview only. This mechanic is not available yet.";
            Fingerprint = node.Fingerprint;
            Left = left;
            Top = top;
            SelectCommand = new DelegateCommand(() => select?.Invoke(NodeId));
        }

        public string NodeId { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public string PrerequisiteText { get; }
        public string StateText { get; }
        public long RabbitDataPrice { get; }
        public int CapacityPoints { get; }
        public int MaximumRanks => node.MaximumRanks;
        public int MaximumCapacityPoints => node.MaximumCapacityPoints;
        public int GetRank(int allocationCapacityPoints) => node.GetRank(allocationCapacityPoints);
        public bool IsPurchasable { get; }
        public IReadOnlyList<string> PrerequisiteNodeIds { get; }
        public string BranchText { get; }
        public string BiomeImpactText { get; }
        public string Fingerprint { get; }
        public float Left { get; }
        public float Top { get; }
        public DelegateCommand SelectCommand { get; }
        public bool IsSelected { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;

        internal void SetSelected(bool selected)
        {
            if (IsSelected == selected)
            {
                return;
            }
            IsSelected = selected;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
        }

        static string GetStateText(GenomeUpgradeNodeSnapshot node, ProfileSessionSnapshot profile)
        {
            if (profile != null && profile.TryGetGenomeProfile(node.TargetSpecies, out var speciesProfile)
                && speciesProfile.IsNodeActive(node.Id))
            {
                return "ACTIVE";
            }
            if (!node.IsPurchasable)
            {
                return "PREVIEW";
            }
            if (profile == null || !profile.HasLoadedProfile || !profile.GeneLabUnlocked
                || !profile.UnlockedGenomeSpeciesIds.Contains(node.TargetSpecies))
            {
                return "LOCKED";
            }
            profile.TryGetGenomeProfile(node.TargetSpecies, out var genome);
            return node.PrerequisiteNodeIds.Any(id => genome == null || !genome.IsNodeActive(id))
                ? "REQUIRES NODE" : "AVAILABLE";
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
