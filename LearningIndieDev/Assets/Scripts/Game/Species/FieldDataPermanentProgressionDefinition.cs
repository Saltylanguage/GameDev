using System;
using System.Collections.Generic;
using System.Linq;

namespace SaltyGame
{
    /// <summary>The approved first FD2 catalog and economy, shared by authoring and domain callers.</summary>
    public sealed class FieldDataPermanentProgressionDefinition
    {
        public const string Version = "field-data-permanent-v1";
        public const int FieldDataPerSpeciesData = 10;
        public const int ResearchMultiplierNumerator = 11;
        public const int ResearchMultiplierDenominator = 10;
        public const long SuccessResearchData = 10;
        public const long HareLicensePrice = 10;
        public const long NodePrice = 5;
        public const int NodeCapacity = 1;
        public const int MaximumActiveCapacity = 8;
        public const int PopulationReservesMaximumRank = 5;
        public const int BaseHarePurchaseAllowance = 10;
        public const long ResetResearchDataFee = 20;
        public const string HareMapPath = "Assets/Data/TestData/Genome/HareGenome.asset";
        public const string BurrowingNodeId = "hare.burrowing";
        public const string SharedBurrowsNodeId = "hare.shared-burrows";
        public const string ExpandedBurrowsNodeId = "hare.expanded-burrows";
        public const string BornBurrowerNodeId = "hare.born-burrower";

        public static bool HasProvisionalResearchPrice(string id) =>
            id == "hare.seed-dispersal" || id == "hare.crowding-tolerance" || id == "hare.reproductive-drive"
            || id == "hare.escape-instinct" || id == "hare.increased-minimum-litter" || id == "hare.larger-litters"
            || id == BurrowingNodeId || id == SharedBurrowsNodeId || id == ExpandedBurrowsNodeId || id == BornBurrowerNodeId;

        public static long PopulationReservesResearchCost(int rank)
        {
            if (rank < 1 || rank > PopulationReservesMaximumRank)
                throw new ArgumentOutOfRangeException(nameof(rank));
            return rank * 5L;
        }

        public static int HarePurchaseAllowance(int equippedRank)
        {
            if (equippedRank < 0 || equippedRank > PopulationReservesMaximumRank)
                throw new ArgumentOutOfRangeException(nameof(equippedRank));
            return BaseHarePurchaseAllowance + equippedRank * 5;
        }

        /// <summary>Freeze owned research independently of the equipped loadout.</summary>
        public static Func<string, bool> CaptureMutationResearch(ProfileSessionSnapshot profile, SpeciesId species)
        {
            if (species.Value != "hare") return id => true;
            var owned = new HashSet<string>(StringComparer.Ordinal);
            if (profile != null && profile.TryGetGenomeProfile(species, out var genome))
            {
                owned.UnionWith(genome.UnlockedNodeIds);
                // Prototype purchases were stored as allocations rather than unlock IDs.
                owned.UnionWith(genome.Allocations.Select(allocation => allocation.NodeId));
            }
            return id =>
            {
                string[] research;
                switch (id)
                {
                    case SpeciesUpgradeCatalog.ToughHideId: research = new[] { "hare.guarded-burrow", "poc.hare.tough-hide" }; break;
                    case SpeciesUpgradeCatalog.FasterMovementId: research = new[] { "hare.swift-digging", "poc.hare.long-stride" }; break;
                    case SpeciesUpgradeCatalog.KeenSensesId:
                    case SpeciesUpgradeCatalog.LongVisionId: research = new[] { "hare.keen-hearing", "poc.hare.long-vision", "heightened-senses" }; break;
                    case SpeciesUpgradeCatalog.EfficientDigestionId: research = new[] { "hare.safe-foraging", "poc.hare.efficient-digestion" }; break;
                    case SpeciesUpgradeCatalog.SeedDispersalId: research = new[] { "hare.seed-dispersal", "seed-dispersal", "fertile-droppings" }; break;
                    case SpeciesUpgradeCatalog.BurrowingId: return owned.Contains(BurrowingNodeId);
                    case SpeciesUpgradeCatalog.SharedBurrowsId: return owned.Contains(SharedBurrowsNodeId);
                    case SpeciesUpgradeCatalog.ExpandedBurrowsId: return owned.Contains(ExpandedBurrowsNodeId);
                    case SpeciesUpgradeCatalog.CrowdingToleranceId: research = new[] { "hare.crowding-tolerance", "crowding-tolerance", "high-crowding" }; break;
                    case SpeciesUpgradeCatalog.ReproductiveDriveId: research = new[] { "hare.reproductive-drive", "reproductive-drive" }; break;
                    case SpeciesUpgradeCatalog.LargerMinimumLitterId: research = new[] { "hare.increased-minimum-litter", "increased-minimum-litter" }; break;
                    case SpeciesUpgradeCatalog.LargerMaximumLitterId: research = new[] { "hare.larger-litters", "poc.hare.larger-litters" }; break;
                    case SpeciesUpgradeCatalog.ThreatExposureId:
                    case SpeciesUpgradeCatalog.LegacyThreatResponseId: research = new[] { "hare.escape-instinct", "escape-instinct" }; break;
                    default: return false;
                }
                return research.Any(owned.Contains);
            };
        }

        readonly IReadOnlyList<long> conversionResearchPrices = Array.AsReadOnly(new long[] { 10, 20, 30 });

        FieldDataPermanentProgressionDefinition(SpeciesGenomeMapSnapshot hareMap)
        {
            HareMap = hareMap;
            Catalog = new GenomeCatalogSnapshot(new[] { hareMap });
        }

        public SpeciesGenomeMapSnapshot HareMap { get; }
        public GenomeCatalogSnapshot Catalog { get; }
        public IReadOnlyList<long> ConversionResearchPrices => conversionResearchPrices;
        public int MaximumResearchLevel => conversionResearchPrices.Count;
        public int ActiveCapacity => MaximumActiveCapacity;

        public static FieldDataPermanentProgressionDefinition CreateStarter()
        {
            var hare = new SpeciesId("hare");
            var nodes = new[]
            {
                Node("hare.guarded-burrow", "Tough Hide", "Unlocks Tough Hide as a journey Mutation."),
                Node("hare.keen-hearing", "Heightened Senses", "Unlocks Long Vision and Keen Senses journey Mutations.",
                    "hare.guarded-burrow"),
                Node("hare.swift-digging", "Long Stride", "Unlocks Faster Movement as a journey Mutation.",
                    "hare.guarded-burrow"),
                Node("hare.safe-foraging", "Efficient Digestion", "Unlocks Efficient Digestion as a journey Mutation.",
                    "hare.keen-hearing", "hare.swift-digging"),
                new GenomeUpgradeNodeSnapshot("hare.warren-network", "Population Reserves",
                    "Raises purchased Hare allowance by 5 per rank for each simulation node.", hare,
                    new[] { "hare.guarded-burrow", "hare.safe-foraging" }, NodePrice, NodeCapacity, "FD2"),
                new GenomeUpgradeNodeSnapshot("hare.founding-hares", "Founding Hares",
                    "+5 Hares at the start of a new journey. A larger colony consumes more food. Does not refill each round.",
                    hare, rabbitDataPrice: NodePrice, capacityPoints: NodeCapacity, branchName: "FD2",
                    modifiers: new[] { new SpeciesUpgradeModifier(SpeciesAttributeIds.StartingPopulation, 5) }),
                Node("hare.seed-dispersal", "Seed Dispersal", "Unlocks Seed Dispersal as a journey Mutation. Research cost and path are test values."),
                Node("hare.crowding-tolerance", "Crowding Tolerance", "Unlocks Crowding Tolerance as a journey Mutation. Research cost and path are test values."),
                Node("hare.reproductive-drive", "Reproductive Drive", "Unlocks Reproductive Drive as a journey Mutation. Not offered at 100% reproduction chance. Cost and path are test values."),
                Node("hare.escape-instinct", "Escape Instinct", "Unlocks Escape Instinct as a journey Mutation. Research cost and path are test values."),
                Node("hare.increased-minimum-litter", "Increased Minimum Litter", "Unlocks Increased Minimum Litter as a journey Mutation. Research cost and path are test values."),
                Node("hare.larger-litters", "Larger Litters", "Unlocks Larger Litters as a journey Mutation. Research cost and path are test values."),
                Node(BurrowingNodeId, "Burrowing", "Unlocks Burrowing as a journey Mutation. Research cost and path are test values."),
                Node(SharedBurrowsNodeId, "Shared Burrows", "Unlocks Shared Burrows as a journey Mutation. Research cost and path are test values.", BurrowingNodeId),
                Node(ExpandedBurrowsNodeId, "Expanded Burrows", "Unlocks five journey ranks of Expanded Burrows. Research cost and path are test values.", BurrowingNodeId),
                Node(BornBurrowerNodeId, "Born Burrower", "Equip to start the journey with Burrowing. Planned exclusion is Open-ground Sprint. Research cost is a test value.", BurrowingNodeId),
            };
            return new FieldDataPermanentProgressionDefinition(new SpeciesGenomeMapSnapshot(hare, nodes));
        }

        static GenomeUpgradeNodeSnapshot Node(string id, string name, string description,
            params string[] prerequisites)
        {
            return new GenomeUpgradeNodeSnapshot(id, name, description, new SpeciesId("hare"),
                prerequisites, NodePrice, NodeCapacity, "FD2");
        }

        public void ValidateAuthoredMap(SpeciesGenomeMapSnapshot authoredMap)
        {
            if (authoredMap == null || authoredMap.Fingerprint != HareMap.Fingerprint)
            {
                throw new ArgumentException("The authored FD2 Hare catalog does not match " + Version + ".",
                    nameof(authoredMap));
            }
        }

        /// <summary>FD2 prerequisites are owned nodes; the POC resolver retains its active-parent rule.</summary>
        public GenomeSimulationSnapshot FreezeOwnedLoadout(SpeciesGenomeProfile profile)
        {
            if (profile != null && profile.SpeciesId != HareMap.SpeciesId)
            {
                throw new ArgumentException("The first FD2 catalog supports Hare only.", nameof(profile));
            }

            var activeIds = profile?.ActiveNodeIds ?? Array.Empty<string>();
            var resolvedNodes = new List<GenomeUpgradeNodeSnapshot>();
            var capacity = 0;
            foreach (var id in activeIds)
            {
                if (!HareMap.TryGetNode(id, out var node) || !profile.IsNodeUnlocked(id))
                {
                    throw new InvalidOperationException("Active FD2 node must be owned and in the catalog: " + id);
                }

                RequireOwnedPrerequisites(node, profile);
                capacity = checked(capacity + 1);
                if (capacity > ActiveCapacity)
                {
                    throw new InvalidOperationException("The FD2 loadout exceeds active capacity.");
                }

                resolvedNodes.Add(node);
            }

            return new GenomeSimulationSnapshot(new[]
            {
                new SpeciesGenomeSnapshot(HareMap.SpeciesId, activeIds, resolvedNodes, Catalog.Fingerprint,
                    populationReservesRank: profile != null && profile.IsNodeActive("hare.warren-network") ? profile.PopulationReservesRank : 0),
            });
        }

        void RequireOwnedPrerequisites(GenomeUpgradeNodeSnapshot node, SpeciesGenomeProfile profile)
        {
            foreach (var prerequisite in node.PrerequisiteNodeIds)
            {
                if (!profile.IsNodeUnlocked(prerequisite))
                {
                    throw new InvalidOperationException("The FD2 loadout requires owned prerequisite: " + prerequisite);
                }

                RequireOwnedPrerequisites(HareMap.GetNode(prerequisite), profile);
            }
        }
    }
}
