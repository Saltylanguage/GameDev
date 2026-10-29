using System;
using System.Collections.Generic;
using System.Linq;

namespace SaltyGame
{
    /// <summary>Profile-owned Gene Lab workflow and quotes; transactions remain in the profile API.</summary>
    public sealed class FieldDataGenomeLab
    {
        readonly Helper_ProfileSession session;
        readonly string profileId;
        readonly FieldDataPermanentProgressionDefinition definition = FieldDataPermanentProgressionDefinition.CreateStarter();
        readonly HashSet<string> selection = new HashSet<string>(StringComparer.Ordinal);
        ProfileSessionSnapshot profile;
        SpeciesGenomeProfile genome;
        long generation;
        string activeIdentity;

        public FieldDataGenomeLab(Helper_ProfileSession session)
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            if (!session.Current.HasLoadedProfile) throw new ArgumentException("Gene Lab requires a loaded profile.");
            profileId = session.Current.ProfileId;
            Bind(session.Current);
        }

        public GenomeCatalogSnapshot Catalog => definition.Catalog;
        public string Status { get; private set; }
        public void ClearStatus() { Status = null; }
        public bool HasLicense => profile.UnlockedGenomeSpeciesIds.Contains(definition.HareMap.SpeciesId);
        public int CapacityUsed => Capacity(genome?.ActiveNodeIds ?? Array.Empty<string>());
        public int SelectedCapacity => Capacity(selection);
        public string CapacityText => $"{CapacityUsed} / {definition.ActiveCapacity}";
        public string SelectionText => $"Selection: {SelectedCapacity} / {definition.ActiveCapacity} points. Apply once from an empty loadout.";
        public string ResearchText => $"CONVERSION RESEARCH {profile.ConversionResearchLevel} / {definition.MaximumResearchLevel}";
        public string ResearchOffer => profile.ConversionResearchLevel >= definition.MaximumResearchLevel ? "RESEARCH MAXIMUM"
            : $"UPGRADE · {definition.ConversionResearchPrices[profile.ConversionResearchLevel]} RD";
        public string ResearchReason => OwnerReason ?? (profile.ConversionResearchLevel >= definition.MaximumResearchLevel
            ? "Maximum conversion research owned." : SpendQuote(profile.ResearchData,
                definition.ConversionResearchPrices[profile.ConversionResearchLevel], "Research Data"));
        public bool CanResearch => OwnerReason == null && profile.ConversionResearchLevel < definition.MaximumResearchLevel
            && profile.ResearchData >= definition.ConversionResearchPrices[profile.ConversionResearchLevel];
        public string LicenseOffer => HasLicense ? "HARE PATH OWNED" : $"UNLOCK HARE · {FieldDataPermanentProgressionDefinition.HareLicensePrice} HARE DATA";
        public string LicenseReason => OwnerReason ?? (HasLicense ? "Permanent Hare path license retained."
            : SpendQuote(HareData, FieldDataPermanentProgressionDefinition.HareLicensePrice, "Hare Data"));
        public bool CanLicense => OwnerReason == null && !HasLicense && HareData >= FieldDataPermanentProgressionDefinition.HareLicensePrice;
        public string ResetOffer => $"RESET · {FieldDataPermanentProgressionDefinition.ResetResearchDataFee} RD · NO REFUND";
        public string ResetReason => OwnerReason ?? (!HasLicense ? "Unlock the Hare path first."
            : genome == null || genome.ActiveNodeIds.Count == 0 ? "No active loadout to reset."
            : SpendQuote(profile.ResearchData, FieldDataPermanentProgressionDefinition.ResetResearchDataFee, "Research Data")
                + " Ownership stays; active nodes clear. Each replacement needs another reset.");
        public bool CanReset => OwnerReason == null && HasLicense && genome != null && genome.ActiveNodeIds.Count > 0
            && profile.ResearchData >= FieldDataPermanentProgressionDefinition.ResetResearchDataFee;
        public long HareData => profile.SpeciesDataWallets.TryGetValue(definition.HareMap.SpeciesId, out var balance) ? balance : 0;
        string OwnerReason => profile == null || !profile.HasLoadedProfile || profile.ProfileId != profileId
            || !session.Current.HasLoadedProfile || session.Current.ProfileId != profileId
                ? "Reopen Gene Lab for the currently selected profile."
                : genome?.Allocations.Count > 0 ? "Refundable POC allocations are preserved. Reopen the legacy Gene Lab to reset them." : null;

        public void Bind(ProfileSessionSnapshot snapshot)
        {
            profile = snapshot ?? ProfileSessionSnapshot.Empty;
            genome = null;
            profile?.TryGetGenomeProfile(definition.HareMap.SpeciesId, out genome);
            var identity = string.Join("|", genome?.ActiveNodeIds ?? Array.Empty<string>());
            if (generation != (genome?.ResetGeneration ?? 0) || activeIdentity != identity || snapshot?.ProfileId != profileId)
            {
                selection.Clear();
                generation = genome?.ResetGeneration ?? 0;
                activeIdentity = identity;
            }
        }

        int Capacity(IEnumerable<string> ids) => ids.Sum(id => definition.HareMap.TryGetNode(id, out var node) ? node.CapacityPoints : 0);
        static string SpendQuote(long balance, long cost, string currency) => balance < cost
            ? $"Needs {cost} {currency}; you have {balance}."
            : $"{currency}: {balance} -> {balance - cost}.";
        public bool IsOwned(string id) => genome?.IsNodeUnlocked(id) == true;
        public bool IsSelected(string id) => selection.Contains(id);
        public string NodeState(string id) => genome?.IsNodeActive(id) == true ? "OWNED · ACTIVE"
            : IsOwned(id) ? IsSelected(id) ? "OWNED · SELECTED" : "OWNED · INACTIVE"
            : !HasLicense ? "PATH LOCKED" : "NOT OWNED";
        bool HasUnappliedReset => genome != null && genome.ResetGeneration > 0 && genome.ActiveNodeIds.Count == 0;
        public string NodeOffer(string id) => IsOwned(id) ? "PERMANENTLY OWNED"
            : HasUnappliedReset ? "APPLY REPLACEMENT FIRST"
            : $"BUY + ACTIVATE · {FieldDataPermanentProgressionDefinition.NodePrice} HARE DATA";
        public string NodeReason(string id)
        {
            if (OwnerReason != null) return OwnerReason;
            if (!HasLicense) return "Unlock the Hare path first.";
            if (!definition.HareMap.TryGetNode(id ?? string.Empty, out var node)) return "Select a Hare Genome node.";
            if (IsOwned(id)) return genome.ActiveNodeIds.Count == 0
                ? "Permanently owned. Add or remove it in the selection below; applying is free."
                : "Permanently owned. Change the active loadout through a paid reset.";
            if (HasUnappliedReset) return "Apply your replacement selection before buying a new node. Buying also activates it.";
            var missing = node.PrerequisiteNodeIds.Where(parent => !IsOwned(parent))
                .Select(parent => definition.HareMap.GetNode(parent).DisplayName).ToArray();
            if (missing.Length > 0) return "Own first: " + string.Join(", ", missing) + ".";
            var active = (genome?.ActiveNodeIds ?? Array.Empty<string>()).Concat(new[] { id });
            try { definition.FreezeOwnedLoadout(new SpeciesGenomeProfile(definition.HareMap.SpeciesId,
                (genome?.UnlockedNodeIds ?? Array.Empty<string>()).Concat(new[] { id }), active)); }
            catch (InvalidOperationException error) { return error.Message; }
            return SpendQuote(HareData, node.RabbitDataPrice, "Hare Data");
        }
        public bool CanBuyNode(string id) => OwnerReason == null && HasLicense && !IsOwned(id) && !HasUnappliedReset
            && definition.HareMap.TryGetNode(id ?? string.Empty, out var node) && HareData >= node.RabbitDataPrice
            && node.PrerequisiteNodeIds.All(IsOwned) && CapacityUsed + node.CapacityPoints <= definition.ActiveCapacity;
        public string ArrangementReason => OwnerReason ?? (!HasLicense ? "Unlock the Hare path first."
            : genome == null || genome.ActiveNodeIds.Count > 0 ? "Pay for a reset before choosing a replacement loadout."
            : SelectedCapacity > definition.ActiveCapacity ? "Selection exceeds 8 points; remove a node."
            : "Select owned nodes, then apply the whole selection for free. Applying locks this loadout until another paid reset.");
        public bool CanToggle(string id) => OwnerReason == null && HasLicense && genome != null
            && genome.ActiveNodeIds.Count == 0 && IsOwned(id);
        public bool CanApply => OwnerReason == null && HasLicense && genome != null && genome.ActiveNodeIds.Count == 0
            && selection.Count > 0 && SelectedCapacity <= definition.ActiveCapacity;

        public bool Research()
        {
            if (!CanResearch) return Reject(ResearchReason);
            var level = profile.ConversionResearchLevel;
            return Complete(session.TryPurchaseConversionResearch(profileId, level, out var error), error, "Conversion research saved.");
        }
        public bool License()
        {
            if (!CanLicense) return Reject(LicenseReason);
            return Complete(session.TryPurchaseHareGenomeLicense(profileId, out var error), error, "Permanent Hare path license saved.");
        }
        public bool BuyNode(string id)
        {
            if (!CanBuyNode(id)) return Reject(NodeReason(id));
            return Complete(session.TryPurchaseHareGenomeNode(profileId, id, out var error), error, "Node owned and active for the next journey.");
        }
        public bool Reset()
        {
            if (!CanReset) return Reject(ResetReason);
            return Complete(session.TryResetGenomeLoadout(profileId, definition.HareMap.SpeciesId, generation, out var error), error,
                "Reset saved. Ownership retained, no refund. Select owned nodes and apply once for free.");
        }
        public bool Toggle(string id)
        {
            if (!CanToggle(id)) return Reject(ArrangementReason);
            if (!selection.Remove(id)) selection.Add(id);
            Status = null;
            return true;
        }
        public bool Apply()
        {
            if (!CanApply) return Reject(ArrangementReason);
            var ordered = definition.HareMap.Nodes.Where(node => selection.Contains(node.Id)).Select(node => node.Id).ToArray();
            return Complete(session.TryArrangeGenomeLoadout(profileId, definition.HareMap.SpeciesId, generation, ordered, out var error), error,
                "Loadout saved for the next journey. Another replacement requires a paid reset.");
        }
        bool Reject(string reason) { Status = reason; return false; }
        bool Complete(bool success, string error, string message)
        {
            Bind(session.Current);
            Status = success ? message : error;
            return success;
        }
    }
}
