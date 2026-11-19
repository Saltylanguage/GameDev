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
        public const long RabbitLicensePrice = 10;
        public const long NodePrice = 5;
        public const int NodeCapacity = 1;
        public const int MaximumActiveCapacity = 8;
        public const int PopulationReservesMaximumRank = 5;
        public const int BaseRabbitPurchaseAllowance = 10;
        public const long ResetResearchDataFee = 20;
        public const string RabbitMapPath = "Assets/Data/TestData/Genome/RabbitGenome.asset";
        public const string BurrowingNodeId = "rabbit.burrowing";
        public const string SharedBurrowsNodeId = "rabbit.shared-burrows";
        public const string ExpandedBurrowsNodeId = "rabbit.expanded-burrows";
        public const string BornBurrowerNodeId = "rabbit.born-burrower";

        public static bool HasProvisionalResearchPrice(string id) =>
            id == "rabbit.seed-dispersal" || id == "rabbit.crowding-tolerance" || id == "rabbit.reproductive-drive"
            || id == "rabbit.escape-instinct" || id == "rabbit.increased-minimum-litter" || id == "rabbit.larger-litters"
            || id == BurrowingNodeId || id == SharedBurrowsNodeId || id == ExpandedBurrowsNodeId || id == BornBurrowerNodeId;

        public static long PopulationReservesResearchCost(int rank)
        {
            if (rank < 1 || rank > PopulationReservesMaximumRank)
                throw new ArgumentOutOfRangeException(nameof(rank));
            return rank * 5L;
        }

        public static int RabbitPurchaseAllowance(int equippedRank)
        {
            if (equippedRank < 0 || equippedRank > PopulationReservesMaximumRank)
                throw new ArgumentOutOfRangeException(nameof(equippedRank));
            return BaseRabbitPurchaseAllowance + equippedRank * 5;
        }

        /// <summary>Freeze owned research independently of the equipped loadout.</summary>
        public static Func<string, bool> CaptureMutationResearch(ProfileSessionSnapshot profile, SpeciesId species)
        {
            if (species.Value != "rabbit") return id => true;
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
                    case SpeciesUpgradeCatalog.ToughHideId: research = new[] { "rabbit.guarded-burrow", "poc.rabbit.tough-hide" }; break;
                    case SpeciesUpgradeCatalog.FasterMovementId: research = new[] { "rabbit.swift-digging", "poc.rabbit.long-stride" }; break;
                    case SpeciesUpgradeCatalog.KeenSensesId:
                    case SpeciesUpgradeCatalog.LongVisionId: research = new[] { "rabbit.keen-hearing", "poc.rabbit.long-vision", "heightened-senses" }; break;
                    case SpeciesUpgradeCatalog.EfficientDigestionId: research = new[] { "rabbit.safe-foraging", "poc.rabbit.efficient-digestion" }; break;
                    case SpeciesUpgradeCatalog.SeedDispersalId: research = new[] { "rabbit.seed-dispersal", "seed-dispersal", "fertile-droppings" }; break;
                    case SpeciesUpgradeCatalog.BurrowingId: return owned.Contains(BurrowingNodeId);
                    case SpeciesUpgradeCatalog.SharedBurrowsId: return owned.Contains(SharedBurrowsNodeId);
                    case SpeciesUpgradeCatalog.ExpandedBurrowsId: return owned.Contains(ExpandedBurrowsNodeId);
                    case SpeciesUpgradeCatalog.CrowdingToleranceId: research = new[] { "rabbit.crowding-tolerance", "crowding-tolerance", "high-crowding" }; break;
                    case SpeciesUpgradeCatalog.ReproductiveDriveId: research = new[] { "rabbit.reproductive-drive", "reproductive-drive" }; break;
                    case SpeciesUpgradeCatalog.LargerMinimumLitterId: research = new[] { "rabbit.increased-minimum-litter", "increased-minimum-litter" }; break;
                    case SpeciesUpgradeCatalog.LargerMaximumLitterId: research = new[] { "rabbit.larger-litters", "poc.rabbit.larger-litters" }; break;
                    case SpeciesUpgradeCatalog.QuickEscapeId:
                    case SpeciesUpgradeCatalog.ThreatExposureId:
                    case SpeciesUpgradeCatalog.LegacyThreatResponseId: research = new[] { "rabbit.escape-instinct", "escape-instinct" }; break;
                    default: return false;
                }
                return research.Any(owned.Contains);
            };
        }

        readonly IReadOnlyList<long> conversionResearchPrices = Array.AsReadOnly(new long[] { 10, 20, 30 });

        FieldDataPermanentProgressionDefinition(SpeciesGenomeMapSnapshot rabbitMap)
        {
            RabbitMap = rabbitMap;
            Catalog = new GenomeCatalogSnapshot(new[] { rabbitMap });
        }

        public SpeciesGenomeMapSnapshot RabbitMap { get; }
        public GenomeCatalogSnapshot Catalog { get; }
        public IReadOnlyList<long> ConversionResearchPrices => conversionResearchPrices;
        public int MaximumResearchLevel => conversionResearchPrices.Count;
        public int ActiveCapacity => MaximumActiveCapacity;

        public static FieldDataPermanentProgressionDefinition CreateStarter()
        {
            var rabbit = new SpeciesId("rabbit");
            var nodes = new[]
            {
                Node("rabbit.guarded-burrow", "Tough Hide", "Unlocks Tough Hide outside Forest Edge and in Field Data. Forest Edge offers it without research."),
                Node("rabbit.keen-hearing", "Heightened Senses", "Unlocks Long Vision and Keen Senses journey Mutations.",
                    "rabbit.guarded-burrow"),
                Node("rabbit.swift-digging", "Long Stride", "Unlocks Faster Movement as a journey Mutation.",
                    "rabbit.guarded-burrow"),
                Node("rabbit.safe-foraging", "Efficient Digestion", "Unlocks Efficient Digestion as a journey Mutation.",
                    "rabbit.keen-hearing", "rabbit.swift-digging"),
                new GenomeUpgradeNodeSnapshot("rabbit.warren-network", "Population Reserves",
                    "Raises purchased Rabbit allowance by 5 per rank for each simulation node.", rabbit,
                    new[] { "rabbit.guarded-burrow", "rabbit.safe-foraging" }, NodePrice, NodeCapacity, "FD2"),
                new GenomeUpgradeNodeSnapshot("rabbit.founding-rabbits", "Founding Rabbits",
                    "+5 Rabbits at the start of a new journey. A larger colony consumes more food. Does not refill each round.",
                    rabbit, rabbitDataPrice: NodePrice, capacityPoints: NodeCapacity, branchName: "FD2",
                    modifiers: new[] { new SpeciesUpgradeModifier(SpeciesAttributeIds.StartingPopulation, 5) }),
                Node("rabbit.seed-dispersal", "Seed Dispersal", "Unlocks Seed Dispersal as a journey Mutation. Research cost and path are test values."),
                Node("rabbit.crowding-tolerance", "Crowding Tolerance", "Unlocks Crowding Tolerance as a journey Mutation. Research cost and path are test values."),
                Node("rabbit.reproductive-drive", "Reproductive Drive", "Unlocks Reproductive Drive outside Forest Edge and in Field Data. Forest Edge offers it without research. Not offered at 100% reproduction chance. Cost and path are test values."),
                Node("rabbit.escape-instinct", "Escape Instinct", "Unlocks Escape Instinct outside Forest Edge, Quick Escape, and Field Data offers. Forest Edge offers Escape Instinct without research. Research cost and path are test values."),
                Node("rabbit.increased-minimum-litter", "Increased Minimum Litter", "Unlocks Increased Minimum Litter as a journey Mutation. Research cost and path are test values."),
                Node("rabbit.larger-litters", "Larger Litters", "Unlocks Larger Litters as a journey Mutation. Research cost and path are test values."),
                Node(BurrowingNodeId, "Burrowing", "Unlocks Burrowing as a journey Mutation. Research cost and path are test values."),
                Node(SharedBurrowsNodeId, "Shared Burrows", "Unlocks Shared Burrows as a journey Mutation. Research cost and path are test values.", BurrowingNodeId),
                Node(ExpandedBurrowsNodeId, "Expanded Burrows", "Unlocks five journey ranks of Expanded Burrows. Research cost and path are test values.", BurrowingNodeId),
                Node(BornBurrowerNodeId, "Born Burrower", "Equip to start the journey with Burrowing. Planned exclusion is Open-ground Sprint. Research cost is a test value.", BurrowingNodeId),
            };
            return new FieldDataPermanentProgressionDefinition(new SpeciesGenomeMapSnapshot(rabbit, nodes));
        }

        static GenomeUpgradeNodeSnapshot Node(string id, string name, string description,
            params string[] prerequisites)
        {
            return new GenomeUpgradeNodeSnapshot(id, name, description, new SpeciesId("rabbit"),
                prerequisites, NodePrice, NodeCapacity, "FD2");
        }

        public void ValidateAuthoredMap(SpeciesGenomeMapSnapshot authoredMap)
        {
            if (authoredMap == null || authoredMap.Fingerprint != RabbitMap.Fingerprint)
            {
                throw new ArgumentException("The authored FD2 Rabbit catalog does not match " + Version + ".",
                    nameof(authoredMap));
            }
        }

        /// <summary>FD2 prerequisites are owned nodes; the POC resolver retains its active-parent rule.</summary>
        public GenomeSimulationSnapshot FreezeOwnedLoadout(SpeciesGenomeProfile profile)
        {
            if (profile != null && profile.SpeciesId != RabbitMap.SpeciesId)
            {
                throw new ArgumentException("The first FD2 catalog supports Rabbit only.", nameof(profile));
            }

            var activeIds = profile?.ActiveNodeIds ?? Array.Empty<string>();
            var resolvedNodes = new List<GenomeUpgradeNodeSnapshot>();
            var capacity = 0;
            foreach (var id in activeIds)
            {
                if (!RabbitMap.TryGetNode(id, out var node) || !profile.IsNodeUnlocked(id))
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
                new SpeciesGenomeSnapshot(RabbitMap.SpeciesId, activeIds, resolvedNodes, Catalog.Fingerprint,
                    populationReservesRank: profile != null && profile.IsNodeActive("rabbit.warren-network") ? profile.PopulationReservesRank : 0),
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

                RequireOwnedPrerequisites(RabbitMap.GetNode(prerequisite), profile);
            }
        }
    }
}
