using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace SaltyGame
{
    /// <summary>
    /// Stable contract boundary for permanent Genome state. Node effects and
    /// authored costs are intentionally not inferred until their catalog is
    /// approved; this slice carries stable identities and active selections.
    /// </summary>
    public static class GenomeContract
    {
        public const string Version = "species-genome-v1";
        public const int ActiveCapacity = 8;
    }

    /// <summary>
    /// Persistent profile state for one species. Unlocked nodes are permanent;
    /// active nodes are the reassignable configuration for the next run.
    /// </summary>
    public sealed class SpeciesGenomeProfile
    {
        readonly IReadOnlyList<string> unlockedNodeIds;
        readonly IReadOnlyList<string> activeNodeIds;

        public SpeciesGenomeProfile(
            SpeciesId speciesId,
            IEnumerable<string> unlockedNodeIds = null,
            IEnumerable<string> activeNodeIds = null)
        {
            if (!speciesId.IsValid)
            {
                throw new ArgumentException("Genome species id is required.", nameof(speciesId));
            }

            SpeciesId = speciesId;
            this.unlockedNodeIds = CopyIds(unlockedNodeIds, nameof(unlockedNodeIds));
            this.activeNodeIds = CopyIds(activeNodeIds, nameof(activeNodeIds));

            var unlocked = new HashSet<string>(this.unlockedNodeIds, StringComparer.Ordinal);
            foreach (var nodeId in this.activeNodeIds)
            {
                if (!unlocked.Contains(nodeId))
                {
                    throw new ArgumentException(
                        $"Active Genome node '{nodeId}' is not permanently unlocked.",
                        nameof(activeNodeIds));
                }
            }
        }

        public SpeciesId SpeciesId { get; }
        public IReadOnlyList<string> UnlockedNodeIds => unlockedNodeIds;
        public IReadOnlyList<string> ActiveNodeIds => activeNodeIds;

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
            return new SpeciesGenomeProfile(SpeciesId, unlockedNodeIds, nextActiveNodeIds);
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

        public SpeciesGenomeSnapshot(SpeciesId speciesId, IEnumerable<string> activeNodeIds = null)
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
            Fingerprint = CreateFingerprint();
        }

        public SpeciesId SpeciesId { get; }
        public IReadOnlyList<string> ActiveNodeIds => activeNodeIds;
        public string Fingerprint { get; }

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
    /// Asset-free description of one authored Genome node. This is catalog
    /// metadata only; gameplay effects and costs are intentionally deferred.
    /// </summary>
    public sealed class GenomeUpgradeNodeSnapshot
    {
        public const string ContractVersion = "genome-node-catalog-v1";

        readonly IReadOnlyList<string> prerequisiteNodeIds;

        public GenomeUpgradeNodeSnapshot(
            string id,
            string displayName,
            string description,
            SpeciesId targetSpecies,
            IEnumerable<string> prerequisiteNodeIds = null)
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

            Id = id.Trim();
            DisplayName = displayName.Trim();
            Description = description == null ? string.Empty : description.Trim();
            TargetSpecies = targetSpecies;
            this.prerequisiteNodeIds = CopyIds(prerequisiteNodeIds, nameof(prerequisiteNodeIds));

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
        public IReadOnlyList<string> PrerequisiteNodeIds => prerequisiteNodeIds;
        public string Fingerprint { get; }

        string CreateFingerprint()
        {
            var canonical = new StringBuilder(192);
            Append(canonical, ContractVersion);
            Append(canonical, Id);
            Append(canonical, DisplayName);
            Append(canonical, Description);
            Append(canonical, TargetSpecies.Value);

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
