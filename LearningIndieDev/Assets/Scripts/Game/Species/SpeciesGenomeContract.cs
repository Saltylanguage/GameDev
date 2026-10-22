using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace SaltyGame
{
    /// <summary>
    /// Permanent Genome selection, approved purchase terms, and frozen additive stat effects.
    /// Bespoke abilities remain explicit simulation rules.
    /// </summary>
    public static class GenomeContract
    {
        public const string Version = "species-genome-v1";
        public const int ActiveCapacity = 8;
        public const int BasePopulationPoolCapacity = 50;
        public static readonly SpeciesId RabbitSpeciesId = new SpeciesId("hare");
        public const long RabbitTreeLicensePrice = 1;
        public const long RabbitResetResearchDataFee = 1;
        public const long RabbitDataPerSuccessfulJourney = 1;
        public const long ResearchDataPerSuccessfulJourney = 1;
    }

    /// <summary>Receipt for one resettable active Genome purchase.</summary>
    public sealed class GenomeNodeAllocation
    {
        public GenomeNodeAllocation(string nodeId, long rabbitDataSpent, int capacityPoints)
        {
            if (string.IsNullOrWhiteSpace(nodeId))
            {
                throw new ArgumentException("A Genome node id is required.", nameof(nodeId));
            }

            if (rabbitDataSpent <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rabbitDataSpent));
            }

            if (capacityPoints < 1 || capacityPoints > GenomeContract.ActiveCapacity)
            {
                throw new ArgumentOutOfRangeException(nameof(capacityPoints));
            }

            NodeId = nodeId.Trim();
            RabbitDataSpent = rabbitDataSpent;
            CapacityPoints = capacityPoints;
        }

        public string NodeId { get; }
        public long RabbitDataSpent { get; }
        public int CapacityPoints { get; }
    }

    /// <summary>
    /// Persistent profile state for one species. UnlockedNodeIds preserves legacy
    /// permanent unlocks; purchased allocations are resettable.
    /// </summary>
    public sealed class SpeciesGenomeProfile
    {
        readonly IReadOnlyList<string> unlockedNodeIds;
        readonly IReadOnlyList<string> activeNodeIds;
        readonly IReadOnlyList<GenomeNodeAllocation> allocations;

        public SpeciesGenomeProfile(
            SpeciesId speciesId,
            IEnumerable<string> unlockedNodeIds = null,
            IEnumerable<string> activeNodeIds = null,
            IEnumerable<GenomeNodeAllocation> allocations = null)
        {
            if (!speciesId.IsValid)
            {
                throw new ArgumentException("Genome species id is required.", nameof(speciesId));
            }

            SpeciesId = speciesId;
            this.unlockedNodeIds = CopyIds(unlockedNodeIds, nameof(unlockedNodeIds));
            this.activeNodeIds = CopyIds(activeNodeIds, nameof(activeNodeIds));
            var copiedAllocations = new List<GenomeNodeAllocation>();
            var allocatedIds = new HashSet<string>(StringComparer.Ordinal);
            var capacityUsed = 0;
            if (allocations != null)
            {
                foreach (var allocation in allocations)
                {
                    if (allocation == null || !allocatedIds.Add(allocation.NodeId))
                    {
                        throw new ArgumentException("Genome allocations must be non-null and unique.", nameof(allocations));
                    }

                    capacityUsed = checked(capacityUsed + allocation.CapacityPoints);
                    copiedAllocations.Add(allocation);
                }
            }

            this.allocations = new ReadOnlyCollection<GenomeNodeAllocation>(copiedAllocations);

            var unlocked = new HashSet<string>(this.unlockedNodeIds, StringComparer.Ordinal);
            foreach (var nodeId in this.activeNodeIds)
            {
                if (!unlocked.Contains(nodeId) && !allocatedIds.Contains(nodeId))
                {
                    throw new ArgumentException(
                        $"Active Genome node '{nodeId}' is neither a legacy unlock nor an allocation.",
                        nameof(activeNodeIds));
                }
            }

            foreach (var allocation in copiedAllocations)
            {
                if (!Contains(this.activeNodeIds, allocation.NodeId))
                {
                    throw new ArgumentException("Every purchased Genome allocation must be active.", nameof(allocations));
                }
            }

            // Legacy selections predate priced capacity records. Count each as one
            // point for new purchases without rejecting existing saves on load.
            foreach (var nodeId in this.activeNodeIds)
            {
                if (!allocatedIds.Contains(nodeId))
                {
                    capacityUsed = checked(capacityUsed + 1);
                }
            }

            if (copiedAllocations.Count > 0 && capacityUsed > GenomeContract.ActiveCapacity)
            {
                throw new ArgumentException("Purchased Genome allocations exceed active capacity.", nameof(allocations));
            }

            CapacityUsed = capacityUsed;
        }

        public SpeciesId SpeciesId { get; }
        public IReadOnlyList<string> UnlockedNodeIds => unlockedNodeIds;
        public IReadOnlyList<string> ActiveNodeIds => activeNodeIds;
        public IReadOnlyList<GenomeNodeAllocation> Allocations => allocations;
        public int CapacityUsed { get; }

        public bool IsNodeUnlocked(string nodeId)
        {
            return !string.IsNullOrWhiteSpace(nodeId)
                && Contains(unlockedNodeIds, nodeId.Trim());
        }

        public bool IsNodeActive(string nodeId)
        {
            return !string.IsNullOrWhiteSpace(nodeId)
                && Contains(activeNodeIds, nodeId.Trim());
        }

        public SpeciesGenomeProfile WithActiveNodeIds(IEnumerable<string> nextActiveNodeIds)
        {
            return new SpeciesGenomeProfile(SpeciesId, unlockedNodeIds, nextActiveNodeIds, allocations);
        }

        public SpeciesGenomeSnapshot CreateSnapshot()
        {
            return new SpeciesGenomeSnapshot(SpeciesId, activeNodeIds);
        }

        static IReadOnlyList<string> CopyIds(IEnumerable<string> ids, string parameterName)
        {
            var copied = new List<string>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            if (ids != null)
            {
                foreach (var id in ids)
                {
                    if (string.IsNullOrWhiteSpace(id))
                    {
                        throw new ArgumentException("Genome node ids cannot be empty.", parameterName);
                    }

                    var normalized = id.Trim();
                    if (!seen.Add(normalized))
                    {
                        throw new ArgumentException(
                            $"Genome node id '{normalized}' may only appear once.",
                            parameterName);
                    }

                    copied.Add(normalized);
                }
            }

            return new ReadOnlyCollection<string>(copied);
        }

        static bool Contains(IReadOnlyList<string> ids, string value)
        {
            for (var index = 0; index < ids.Count; index++)
            {
                if (string.Equals(ids[index], value, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Immutable active configuration for one species at simulation launch.
    /// </summary>
    public sealed class SpeciesGenomeSnapshot
    {
        readonly IReadOnlyList<string> activeNodeIds;
        readonly IReadOnlyList<GenomeUpgradeNodeSnapshot> resolvedNodes;

        public SpeciesGenomeSnapshot(SpeciesId speciesId, IEnumerable<string> activeNodeIds = null,
            IEnumerable<GenomeUpgradeNodeSnapshot> resolvedNodes = null, string catalogFingerprint = null)
        {
            if (!speciesId.IsValid)
            {
                throw new ArgumentException("Genome species id is required.", nameof(speciesId));
            }

            SpeciesId = speciesId;
            var copied = new List<string>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            if (activeNodeIds != null)
            {
                foreach (var nodeId in activeNodeIds)
                {
                    if (string.IsNullOrWhiteSpace(nodeId))
                    {
                        throw new ArgumentException("Active Genome node ids cannot be empty.", nameof(activeNodeIds));
                    }

                    var normalized = nodeId.Trim();
                    if (!seen.Add(normalized))
                    {
                        throw new ArgumentException(
                            $"Active Genome node id '{normalized}' may only appear once.",
                            nameof(activeNodeIds));
                    }

                    copied.Add(normalized);
                }
            }

            this.activeNodeIds = new ReadOnlyCollection<string>(copied);
            var nodes = new List<GenomeUpgradeNodeSnapshot>(resolvedNodes ?? Array.Empty<GenomeUpgradeNodeSnapshot>());
            if (resolvedNodes != null)
            {
                if (nodes.Count != copied.Count)
                {
                    throw new ArgumentException("Resolved Genome nodes must match the complete active allocation.");
                }
                for (var index = 0; index < nodes.Count; index++)
                {
                    if (nodes[index] == null || nodes[index].TargetSpecies != speciesId || nodes[index].Id != copied[index])
                    {
                        throw new ArgumentException("Resolved Genome nodes must match species and allocation order.");
                    }
                }
            }
            this.resolvedNodes = new ReadOnlyCollection<GenomeUpgradeNodeSnapshot>(nodes);
            CatalogFingerprint = catalogFingerprint ?? string.Empty;
            Fingerprint = CreateFingerprint();
        }

        public SpeciesId SpeciesId { get; }
        public IReadOnlyList<string> ActiveNodeIds => activeNodeIds;
        public IReadOnlyList<GenomeUpgradeNodeSnapshot> ResolvedNodes => resolvedNodes;
        public string CatalogFingerprint { get; }
        public string Fingerprint { get; }

        public SpeciesRules ApplyToNaturalRules(SpeciesRules naturalRules)
        {
            if (naturalRules == null)
            {
                throw new ArgumentNullException(nameof(naturalRules));
            }
            if (resolvedNodes.Count != activeNodeIds.Count)
            {
                // Older identity-only callers can still use the explicit Fertile Droppings ability.
                foreach (var id in activeNodeIds)
                {
                    if (SpeciesId != GenomeContract.RabbitSpeciesId || id != SpeciesSimulation.FertileDroppingsNodeId)
                    {
                        throw new InvalidOperationException($"Genome node '{id}' must be resolved against the catalog before launch.");
                    }
                }
            }
            var result = naturalRules;
            foreach (var node in resolvedNodes)
            {
                result = node.Apply(result);
            }
            return result;
        }

        public int GetPopulationModifier(string attributeId)
        {
            if (!SpeciesAttributeRegistry.TryGet(attributeId, out var definition) || !definition.IsPopulationAttribute)
                throw new ArgumentException("A Genome population attribute is required.", nameof(attributeId));
            // Identity-only legacy snapshots have no effects; launch rule composition validates resolution.
            var total = 0;
            foreach (var node in resolvedNodes)
                foreach (var modifier in node.Modifiers)
                    if (modifier.AttributeId == attributeId)
                        total = checked(total + (int)modifier.SignedValue);
            return total;
        }

        public bool HasActiveNode(string nodeId)
        {
            if (string.IsNullOrWhiteSpace(nodeId))
            {
                return false;
            }

            foreach (var activeNodeId in activeNodeIds)
            {
                if (string.Equals(activeNodeId, nodeId, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        string CreateFingerprint()
        {
            var canonical = new StringBuilder(128);
            Append(canonical, GenomeContract.Version);
            Append(canonical, SpeciesId.Value);

            var orderedNodeIds = new List<string>(activeNodeIds);
            orderedNodeIds.Sort(StringComparer.Ordinal);
            foreach (var nodeId in orderedNodeIds)
            {
                Append(canonical, nodeId);
            }

            if (resolvedNodes.Count > 0 || CatalogFingerprint.Length > 0)
            {
                Append(canonical, "genome-stat-effects-v1");
                Append(canonical, SpeciesAttributeRegistry.Fingerprint);
                Append(canonical, CatalogFingerprint);
                foreach (var node in resolvedNodes)
                {
                    Append(canonical, node.Fingerprint);
                }
            }

            return Sha256(canonical.ToString());
        }

        static string Sha256(string value)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
                var result = new StringBuilder(bytes.Length * 2);
                foreach (var byteValue in bytes)
                {
                    result.Append(byteValue.ToString("x2", CultureInfo.InvariantCulture));
                }

                return result.ToString();
            }
        }

        static void Append(StringBuilder builder, string value)
        {
            builder.Append(value?.Length ?? 0).Append(':').Append(value).Append(';');
        }
    }

    /// <summary>
    /// Immutable per-run active Genome state for all configured species.
    /// </summary>
    public sealed class GenomeSimulationSnapshot
    {
        readonly IReadOnlyDictionary<SpeciesId, SpeciesGenomeSnapshot> speciesGenomes;

        public static GenomeSimulationSnapshot Empty { get; } =
            new GenomeSimulationSnapshot(Array.Empty<SpeciesGenomeSnapshot>());

        public GenomeSimulationSnapshot(IEnumerable<SpeciesGenomeSnapshot> speciesSnapshots)
        {
            var copied = new List<SpeciesGenomeSnapshot>();
            var seenSpecies = new HashSet<SpeciesId>();
            if (speciesSnapshots != null)
            {
                foreach (var snapshot in speciesSnapshots)
                {
                    if (snapshot == null)
                    {
                        throw new ArgumentException(
                            "Genome snapshots cannot contain null entries.",
                            nameof(speciesSnapshots));
                    }

                    if (!seenSpecies.Add(snapshot.SpeciesId))
                    {
                        throw new ArgumentException(
                            $"Genome snapshot for species '{snapshot.SpeciesId}' appears more than once.",
                            nameof(speciesSnapshots));
                    }

                    copied.Add(snapshot);
                }
            }

            copied.Sort((left, right) => string.CompareOrdinal(
                left.SpeciesId.Value,
                right.SpeciesId.Value));

            var dictionary = new Dictionary<SpeciesId, SpeciesGenomeSnapshot>();
            foreach (var snapshot in copied)
            {
                dictionary.Add(snapshot.SpeciesId, snapshot);
            }

            speciesGenomes = new ReadOnlyDictionary<SpeciesId, SpeciesGenomeSnapshot>(dictionary);
            Fingerprint = CreateFingerprint(copied);
        }

        public IReadOnlyDictionary<SpeciesId, SpeciesGenomeSnapshot> SpeciesGenomes => speciesGenomes;
        public string Fingerprint { get; }

        public int GetPopulationPoolCapacity(SpeciesId species)
        {
            var capacity = checked(GenomeContract.BasePopulationPoolCapacity
                + GetSpeciesGenome(species).GetPopulationModifier(SpeciesAttributeIds.PopulationPoolCapacity));
            if (capacity < 1) throw new InvalidOperationException("Population pool capacity must be positive.");
            return capacity;
        }

        public CellularSimData ApplyToStartingPopulations(CellularSimData natural)
        {
            if (natural == null) throw new ArgumentNullException(nameof(natural));
            var populations = new Dictionary<SpeciesId, int>();
            foreach (var entry in natural.StartingPopulations) populations.Add(entry.Key, entry.Value);
            var changed = false;
            foreach (var species in natural.SpeciesRules.Keys)
            {
                var bonus = GetSpeciesGenome(species).GetPopulationModifier(SpeciesAttributeIds.StartingPopulation);
                if (bonus == 0) continue;
                if (!populations.TryGetValue(species, out var starting))
                    throw new InvalidOperationException($"Starting-population Genome requires an explicit starting count for '{species}'.");
                populations[species] = checked(starting + bonus);
                changed = true;
            }
            return changed ? natural.WithGridSizeAndStartingPopulations(natural.Width, natural.Height, populations) : natural;
        }

        /// <summary>Freeze authored values in allocation order, before any run rules are built.</summary>
        public GenomeSimulationSnapshot Resolve(GenomeCatalogSnapshot catalog)
        {
            if (catalog == null)
            {
                throw new ArgumentNullException(nameof(catalog));
            }
            var snapshots = new List<SpeciesGenomeSnapshot>();
            foreach (var genome in speciesGenomes.Values)
            {
                var nodes = new List<GenomeUpgradeNodeSnapshot>();
                var map = catalog.GetSpeciesMap(genome.SpeciesId);
                foreach (var id in genome.ActiveNodeIds)
                {
                    if (map == null || !map.TryGetNode(id, out var node))
                    {
                        throw new InvalidOperationException($"Active Genome node '{id}' is missing from the '{genome.SpeciesId}' catalog.");
                    }
                    foreach (var prerequisite in node.PrerequisiteNodeIds)
                    {
                        if (!genome.HasActiveNode(prerequisite))
                        {
                            throw new InvalidOperationException($"Active Genome node '{id}' requires '{prerequisite}'.");
                        }
                    }
                    nodes.Add(node);
                }
                snapshots.Add(new SpeciesGenomeSnapshot(genome.SpeciesId, genome.ActiveNodeIds, nodes, catalog.Fingerprint));
            }
            return new GenomeSimulationSnapshot(snapshots);
        }

        /// <summary>Compose every participating species from natural rules exactly once, before Mutations.</summary>
        public IReadOnlyDictionary<SpeciesId, SpeciesRules> ApplyToNaturalRules(
            IReadOnlyDictionary<SpeciesId, SpeciesRules> naturalRules)
        {
            if (naturalRules == null)
            {
                throw new ArgumentNullException(nameof(naturalRules));
            }
            var result = new Dictionary<SpeciesId, SpeciesRules>();
            foreach (var entry in naturalRules)
            {
                GetPopulationPoolCapacity(entry.Key);
                result.Add(entry.Key, GetSpeciesGenome(entry.Key).ApplyToNaturalRules(entry.Value));
            }
            return new ReadOnlyDictionary<SpeciesId, SpeciesRules>(result);
        }

        public bool TryGetSpeciesGenome(SpeciesId speciesId, out SpeciesGenomeSnapshot snapshot)
        {
            return speciesGenomes.TryGetValue(speciesId, out snapshot);
        }

        public GenomeSimulationSnapshot IncludeSpecies(IEnumerable<SpeciesId> speciesIds)
        {
            var snapshots = new List<SpeciesGenomeSnapshot>(speciesGenomes.Values);
            var included = new HashSet<SpeciesId>(speciesGenomes.Keys);
            if (speciesIds != null)
            {
                foreach (var speciesId in speciesIds)
                {
                    if (!speciesId.IsValid)
                    {
                        throw new ArgumentException("Genome species id is required.", nameof(speciesIds));
                    }

                    if (included.Add(speciesId))
                    {
                        snapshots.Add(new SpeciesGenomeSnapshot(speciesId));
                    }
                }
            }

            return new GenomeSimulationSnapshot(snapshots);
        }

        public SpeciesGenomeSnapshot GetSpeciesGenome(SpeciesId speciesId)
        {
            return speciesGenomes.TryGetValue(speciesId, out var snapshot)
                ? snapshot
                : new SpeciesGenomeSnapshot(speciesId);
        }

        public static GenomeSimulationSnapshot Create(IEnumerable<SpeciesGenomeProfile> profiles)
        {
            var snapshots = new List<SpeciesGenomeSnapshot>();
            if (profiles != null)
            {
                foreach (var profile in profiles)
                {
                    if (profile == null)
                    {
                        throw new ArgumentException(
                            "Genome profiles cannot contain null entries.",
                            nameof(profiles));
                    }

                    snapshots.Add(profile.CreateSnapshot());
                }
            }

            return new GenomeSimulationSnapshot(snapshots);
        }

        static string CreateFingerprint(IReadOnlyList<SpeciesGenomeSnapshot> snapshots)
        {
            var canonical = new StringBuilder(256);
            Append(canonical, GenomeContract.Version);
            foreach (var snapshot in snapshots)
            {
                Append(canonical, snapshot.SpeciesId.Value);
                Append(canonical, snapshot.Fingerprint);
            }

            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(canonical.ToString()));
                var result = new StringBuilder(bytes.Length * 2);
                foreach (var byteValue in bytes)
                {
                    result.Append(byteValue.ToString("x2", CultureInfo.InvariantCulture));
                }

                return result.ToString();
            }
        }

        static void Append(StringBuilder builder, string value)
        {
            builder.Append(value?.Length ?? 0).Append(':').Append(value).Append(';');
        }
    }

    /// <summary>
    /// Asset-free authored node: ordered additive stat modifiers plus catalog terms.
    /// Abilities such as Fertile Droppings remain explicit simulation rules keyed by ID.
    /// </summary>
    public sealed class GenomeUpgradeNodeSnapshot
    {
        public const string ContractVersion = "genome-node-catalog-v3";

        readonly IReadOnlyList<string> prerequisiteNodeIds;
        readonly IReadOnlyList<SpeciesUpgradeModifier> modifiers;

        public GenomeUpgradeNodeSnapshot(
            string id,
            string displayName,
            string description,
            SpeciesId targetSpecies,
            IEnumerable<string> prerequisiteNodeIds = null,
            long rabbitDataPrice = 0,
            int capacityPoints = 0,
            string branchName = null,
            IEnumerable<SpeciesUpgradeModifier> modifiers = null)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Genome node id cannot be empty.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Genome node display name cannot be empty.", nameof(displayName));
            }

            if (!targetSpecies.IsValid)
            {
                throw new ArgumentException("Genome node target species is required.", nameof(targetSpecies));
            }

            if (rabbitDataPrice < 0 || capacityPoints < 0
                || (rabbitDataPrice == 0) != (capacityPoints == 0)
                || capacityPoints > GenomeContract.ActiveCapacity)
            {
                throw new ArgumentException("A purchasable Genome node needs a positive Rabbit Data price and valid capacity cost.");
            }

            Id = id.Trim();
            DisplayName = displayName.Trim();
            Description = description == null ? string.Empty : description.Trim();
            TargetSpecies = targetSpecies;
            RabbitDataPrice = rabbitDataPrice;
            CapacityPoints = capacityPoints;
            BranchName = branchName == null ? string.Empty : branchName.Trim();
            this.prerequisiteNodeIds = CopyIds(prerequisiteNodeIds, nameof(prerequisiteNodeIds));
            var copiedModifiers = new List<SpeciesUpgradeModifier>();
            var attributes = new HashSet<string>(StringComparer.Ordinal);
            foreach (var modifier in modifiers ?? Array.Empty<SpeciesUpgradeModifier>())
            {
                SpeciesAttributeRegistry.ValidateModifier(modifier);
                if (!attributes.Add(modifier.AttributeId))
                {
                    throw new ArgumentException($"Attribute '{modifier.AttributeId}' may only appear once per Genome node.");
                }
                copiedModifiers.Add(modifier);
            }
            this.modifiers = new ReadOnlyCollection<SpeciesUpgradeModifier>(copiedModifiers);

            foreach (var prerequisiteId in this.prerequisiteNodeIds)
            {
                if (string.Equals(prerequisiteId, Id, StringComparison.Ordinal))
                {
                    throw new ArgumentException(
                        $"Genome node '{Id}' cannot require itself.",
                        nameof(prerequisiteNodeIds));
                }
            }

            Fingerprint = CreateFingerprint();
        }

        public string Id { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public SpeciesId TargetSpecies { get; }
        public long RabbitDataPrice { get; }
        public int CapacityPoints { get; }
        public bool IsPurchasable => RabbitDataPrice > 0;
        public string BranchName { get; }
        public IReadOnlyList<string> PrerequisiteNodeIds => prerequisiteNodeIds;
        public IReadOnlyList<SpeciesUpgradeModifier> Modifiers => modifiers;
        public string Fingerprint { get; }

        public SpeciesRules Apply(SpeciesRules rules)
        {
            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }
            if (modifiers.Count == 0
                && (TargetSpecies != GenomeContract.RabbitSpeciesId || Id != SpeciesSimulation.FertileDroppingsNodeId))
            {
                throw new InvalidOperationException($"Genome node '{Id}' has no implemented stat or ability effect.");
            }
            foreach (var modifier in modifiers)
            {
                if (!SpeciesAttributeRegistry.TryGet(modifier.AttributeId, out var definition) || !definition.IsPopulationAttribute)
                    rules = SpeciesAttributeRegistry.Apply(rules, modifier);
            }
            return rules;
        }

        string CreateFingerprint()
        {
            var canonical = new StringBuilder(192);
            Append(canonical, ContractVersion);
            Append(canonical, Id);
            Append(canonical, DisplayName);
            Append(canonical, Description);
            Append(canonical, TargetSpecies.Value);
            Append(canonical, RabbitDataPrice.ToString(CultureInfo.InvariantCulture));
            Append(canonical, CapacityPoints.ToString(CultureInfo.InvariantCulture));
            Append(canonical, BranchName);
            Append(canonical, SpeciesAttributeRegistry.Fingerprint);
            foreach (var modifier in modifiers)
            {
                Append(canonical, modifier.AttributeId);
                Append(canonical, modifier.SignedValue.ToString("R", CultureInfo.InvariantCulture));
            }

            var orderedPrerequisites = new List<string>(prerequisiteNodeIds);
            orderedPrerequisites.Sort(StringComparer.Ordinal);
            foreach (var prerequisiteId in orderedPrerequisites)
            {
                Append(canonical, prerequisiteId);
            }

            return Sha256(canonical.ToString());
        }

        static IReadOnlyList<string> CopyIds(IEnumerable<string> ids, string parameterName)
        {
            var copied = new List<string>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            if (ids != null)
            {
                foreach (var id in ids)
                {
                    if (string.IsNullOrWhiteSpace(id))
                    {
                        throw new ArgumentException("Genome prerequisite ids cannot be empty.", parameterName);
                    }

                    var normalized = id.Trim();
                    if (!seen.Add(normalized))
                    {
                        throw new ArgumentException(
                            $"Genome prerequisite id '{normalized}' may only appear once.",
                            parameterName);
                    }

                    copied.Add(normalized);
                }
            }

            return new ReadOnlyCollection<string>(copied);
        }

        static string Sha256(string value)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
                var result = new StringBuilder(bytes.Length * 2);
                foreach (var byteValue in bytes)
                {
                    result.Append(byteValue.ToString("x2", CultureInfo.InvariantCulture));
                }

                return result.ToString();
            }
        }

        static void Append(StringBuilder builder, string value)
        {
            builder.Append(value?.Length ?? 0).Append(':').Append(value).Append(';');
        }
    }

    /// <summary>
    /// Immutable topology for one species Genome map. Authored order is
    /// preserved for presentation, while fingerprinting remains order stable.
    /// </summary>
    public sealed class SpeciesGenomeMapSnapshot
    {
        readonly IReadOnlyList<GenomeUpgradeNodeSnapshot> nodes;
        readonly IReadOnlyDictionary<string, GenomeUpgradeNodeSnapshot> nodesById;

        public SpeciesGenomeMapSnapshot(
            SpeciesId speciesId,
            IEnumerable<GenomeUpgradeNodeSnapshot> nodes)
        {
            if (!speciesId.IsValid)
            {
                throw new ArgumentException("Genome map species id is required.", nameof(speciesId));
            }

            if (nodes == null)
            {
                throw new ArgumentNullException(nameof(nodes));
            }

            var copied = new List<GenomeUpgradeNodeSnapshot>();
            var byId = new Dictionary<string, GenomeUpgradeNodeSnapshot>(StringComparer.Ordinal);
            foreach (var node in nodes)
            {
                if (node == null)
                {
                    throw new ArgumentException("Genome map nodes cannot contain null entries.", nameof(nodes));
                }

                if (node.TargetSpecies != speciesId)
                {
                    throw new ArgumentException(
                        $"Genome node '{node.Id}' targets '{node.TargetSpecies}', not map species '{speciesId}'.",
                        nameof(nodes));
                }

                if (byId.ContainsKey(node.Id))
                {
                    throw new ArgumentException(
                        $"Genome node id '{node.Id}' appears more than once in species map '{speciesId}'.",
                        nameof(nodes));
                }

                byId.Add(node.Id, node);
                copied.Add(node);
            }

            foreach (var node in copied)
            {
                foreach (var prerequisiteId in node.PrerequisiteNodeIds)
                {
                    if (!byId.ContainsKey(prerequisiteId))
                    {
                        throw new ArgumentException(
                            $"Genome node '{node.Id}' requires missing node '{prerequisiteId}'.",
                            nameof(nodes));
                    }
                }
            }

            ValidateAcyclic(copied, byId);
            SpeciesId = speciesId;
            this.nodes = new ReadOnlyCollection<GenomeUpgradeNodeSnapshot>(copied);
            nodesById = new ReadOnlyDictionary<string, GenomeUpgradeNodeSnapshot>(byId);
            Fingerprint = CreateFingerprint();
        }

        public SpeciesId SpeciesId { get; }
        public IReadOnlyList<GenomeUpgradeNodeSnapshot> Nodes => nodes;
        public string Fingerprint { get; }

        public bool TryGetNode(string nodeId, out GenomeUpgradeNodeSnapshot node)
        {
            return nodesById.TryGetValue(nodeId, out node);
        }

        public GenomeUpgradeNodeSnapshot GetNode(string nodeId)
        {
            return nodesById.TryGetValue(nodeId, out var node)
                ? node
                : null;
        }

        string CreateFingerprint()
        {
            var canonical = new StringBuilder(256);
            Append(canonical, GenomeUpgradeNodeSnapshot.ContractVersion);
            Append(canonical, SpeciesId.Value);

            var orderedNodes = new List<GenomeUpgradeNodeSnapshot>(nodes);
            orderedNodes.Sort((left, right) => string.CompareOrdinal(left.Id, right.Id));
            foreach (var node in orderedNodes)
            {
                Append(canonical, node.Fingerprint);
            }

            return Sha256(canonical.ToString());
        }

        static void ValidateAcyclic(
            IReadOnlyList<GenomeUpgradeNodeSnapshot> nodes,
            IReadOnlyDictionary<string, GenomeUpgradeNodeSnapshot> nodesById)
        {
            var visiting = new HashSet<string>(StringComparer.Ordinal);
            var visited = new HashSet<string>(StringComparer.Ordinal);
            foreach (var node in nodes)
            {
                Visit(node, nodesById, visiting, visited);
            }
        }

        static void Visit(
            GenomeUpgradeNodeSnapshot node,
            IReadOnlyDictionary<string, GenomeUpgradeNodeSnapshot> nodesById,
            ISet<string> visiting,
            ISet<string> visited)
        {
            if (visited.Contains(node.Id))
            {
                return;
            }

            if (!visiting.Add(node.Id))
            {
                throw new ArgumentException(
                    $"Genome map contains a prerequisite cycle at node '{node.Id}'.",
                    nameof(nodesById));
            }

            foreach (var prerequisiteId in node.PrerequisiteNodeIds)
            {
                Visit(nodesById[prerequisiteId], nodesById, visiting, visited);
            }

            visiting.Remove(node.Id);
            visited.Add(node.Id);
        }

        static string Sha256(string value)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
                var result = new StringBuilder(bytes.Length * 2);
                foreach (var byteValue in bytes)
                {
                    result.Append(byteValue.ToString("x2", CultureInfo.InvariantCulture));
                }

                return result.ToString();
            }
        }

        static void Append(StringBuilder builder, string value)
        {
            builder.Append(value?.Length ?? 0).Append(':').Append(value).Append(';');
        }
    }

    /// <summary>
    /// Immutable catalog snapshot keyed by stable species identity. This is
    /// the boundary between Unity authoring assets and UI/runtime consumers.
    /// </summary>
    public sealed class GenomeCatalogSnapshot
    {
        readonly IReadOnlyList<SpeciesGenomeMapSnapshot> speciesMaps;
        readonly IReadOnlyDictionary<SpeciesId, SpeciesGenomeMapSnapshot> speciesMapsById;

        public static GenomeCatalogSnapshot Empty { get; } =
            new GenomeCatalogSnapshot(Array.Empty<SpeciesGenomeMapSnapshot>());

        public GenomeCatalogSnapshot(IEnumerable<SpeciesGenomeMapSnapshot> speciesMaps)
        {
            if (speciesMaps == null)
            {
                throw new ArgumentNullException(nameof(speciesMaps));
            }

            var copied = new List<SpeciesGenomeMapSnapshot>();
            var byId = new Dictionary<SpeciesId, SpeciesGenomeMapSnapshot>();
            foreach (var speciesMap in speciesMaps)
            {
                if (speciesMap == null)
                {
                    throw new ArgumentException("Genome species maps cannot contain null entries.", nameof(speciesMaps));
                }

                if (byId.ContainsKey(speciesMap.SpeciesId))
                {
                    throw new ArgumentException(
                        $"Genome map for species '{speciesMap.SpeciesId}' appears more than once.",
                        nameof(speciesMaps));
                }

                byId.Add(speciesMap.SpeciesId, speciesMap);
                copied.Add(speciesMap);
            }

            copied.Sort((left, right) => string.CompareOrdinal(
                left.SpeciesId.Value,
                right.SpeciesId.Value));
            this.speciesMaps = new ReadOnlyCollection<SpeciesGenomeMapSnapshot>(copied);
            speciesMapsById = new ReadOnlyDictionary<SpeciesId, SpeciesGenomeMapSnapshot>(byId);
            Fingerprint = CreateFingerprint();
        }

        public IReadOnlyList<SpeciesGenomeMapSnapshot> SpeciesMaps => speciesMaps;
        public string Fingerprint { get; }

        public bool TryGetSpeciesMap(SpeciesId speciesId, out SpeciesGenomeMapSnapshot speciesMap)
        {
            return speciesMapsById.TryGetValue(speciesId, out speciesMap);
        }

        public SpeciesGenomeMapSnapshot GetSpeciesMap(SpeciesId speciesId)
        {
            return speciesMapsById.TryGetValue(speciesId, out var speciesMap)
                ? speciesMap
                : null;
        }

        string CreateFingerprint()
        {
            var canonical = new StringBuilder(256);
            Append(canonical, GenomeUpgradeNodeSnapshot.ContractVersion);
            foreach (var speciesMap in speciesMaps)
            {
                Append(canonical, speciesMap.SpeciesId.Value);
                Append(canonical, speciesMap.Fingerprint);
            }

            return Sha256(canonical.ToString());
        }

        static string Sha256(string value)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
                var result = new StringBuilder(bytes.Length * 2);
                foreach (var byteValue in bytes)
                {
                    result.Append(byteValue.ToString("x2", CultureInfo.InvariantCulture));
                }

                return result.ToString();
            }
        }

        static void Append(StringBuilder builder, string value)
        {
            builder.Append(value?.Length ?? 0).Append(':').Append(value).Append(';');
        }
    }
}
