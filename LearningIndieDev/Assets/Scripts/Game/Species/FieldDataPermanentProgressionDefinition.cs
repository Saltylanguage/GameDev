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
        public const string SprintNodeId = "hare.open-ground-sprint";
        public const string GestationUpgradeNodeId = "hare.gestation-upgrade";
        public const string LitterUpgradeNodeId = "hare.larger-litters";
        public const string FoundingHaresNodeId = "hare.founding-hares";
        public const string PopulationReservesNodeId = "hare.warren-network";

        public static bool HasProvisionalResearchPrice(string id) => id == "hare.increased-minimum-litter";

        public static long PopulationReservesResearchCost(int rank)
        {
            if (rank < 1 || rank > PopulationReservesMaximumRank)
                throw new ArgumentOutOfRangeException(nameof(rank));
            // The approved catalog diagram prices every Population Reserves rank at 30 Hare Data.
            return 30L;
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
            return id => GetMutationResearchNodeIds(id).Any(owned.Contains);
        }

        /// <summary>Journey Mutations require both owned research and its active Genome node.</summary>
        public static Func<string, bool> CaptureMutationEquipment(ProfileSessionSnapshot profile, SpeciesId species)
        {
            if (species.Value != "hare" || profile == null) return id => true;
            var active = new HashSet<string>(StringComparer.Ordinal);
            if (profile.TryGetGenomeProfile(species, out var genome))
                active.UnionWith(genome.ActiveNodeIds);
            var hasBornBurrowerStarter = active.Contains(BornBurrowerNodeId);
            return id =>
            {
                if (id == SpeciesUpgradeCatalog.OpenGroundSprintId && hasBornBurrowerStarter) return false;
                return GetMutationResearchNodeIds(id).Any(active.Contains);
            };
        }

        static string[] GetMutationResearchNodeIds(string mutationId)
        {
            switch (mutationId)
            {
                case SpeciesUpgradeCatalog.ToughHideId: return new[] { "hare.guarded-burrow", "poc.hare.tough-hide" };
                case SpeciesUpgradeCatalog.FasterMovementId: return new[] { "hare.swift-digging", "poc.hare.long-stride" };
                case SpeciesUpgradeCatalog.KeenSensesId:
                case SpeciesUpgradeCatalog.LongVisionId: return new[] { "hare.keen-hearing", "poc.hare.long-vision", "heightened-senses" };
                case SpeciesUpgradeCatalog.EfficientDigestionId: return new[] { "hare.safe-foraging", "poc.hare.efficient-digestion" };
                case SpeciesUpgradeCatalog.SeedDispersalId: return new[] { "hare.seed-dispersal", "seed-dispersal", "fertile-droppings" };
                case SpeciesUpgradeCatalog.BurrowingId: return new[] { BurrowingNodeId };
                case SpeciesUpgradeCatalog.SharedBurrowsId: return new[] { SharedBurrowsNodeId };
                case SpeciesUpgradeCatalog.ExpandedBurrowsId: return new[] { ExpandedBurrowsNodeId };
                case SpeciesUpgradeCatalog.FasterGestationId: return new[] { GestationUpgradeNodeId };
                case SpeciesUpgradeCatalog.OpenGroundSprintId: return new[] { SprintNodeId };
                case SpeciesUpgradeCatalog.CrowdingToleranceId: return new[] { "hare.crowding-tolerance", "crowding-tolerance", "high-crowding" };
                case SpeciesUpgradeCatalog.ReproductiveDriveId: return new[] { "hare.reproductive-drive", "reproductive-drive" };
                case SpeciesUpgradeCatalog.LargerMinimumLitterId: return new[] { LitterUpgradeNodeId, "hare.increased-minimum-litter", "increased-minimum-litter" };
                case SpeciesUpgradeCatalog.LargerMaximumLitterId: return new[] { LitterUpgradeNodeId, "poc.hare.larger-litters" };
                case SpeciesUpgradeCatalog.ThreatExposureId:
                case SpeciesUpgradeCatalog.LegacyThreatResponseId: return new[] { "hare.escape-instinct", "escape-instinct" };
                default: return Array.Empty<string>();
            }
        }

        public static Func<string, int> CaptureMutationRankLimit(ProfileSessionSnapshot profile, SpeciesId species)
        {
            var researched = CaptureMutationResearch(profile, species);
            var litterRank = 0;
            if (species.Value == "hare" && profile != null && profile.TryGetGenomeProfile(species, out var genome))
                litterRank = genome.LitterUpgradeRank;
            return id =>
            {
                if (!researched(id)) return 0;
                if (species.Value == "hare"
                    && (id == SpeciesUpgradeCatalog.LargerMinimumLitterId
                        || id == SpeciesUpgradeCatalog.LargerMaximumLitterId))
                    return Math.Min(
                        SpeciesUpgradeCatalog.GetFieldDataRewardMaxLevel(species, id),
                        litterRank > 0 ? litterRank : 1);
                return SpeciesUpgradeCatalog.GetFieldDataRewardMaxLevel(species, id);
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
                Node("hare.guarded-burrow", "Tough Hide", "Unlocks Tough Hide as a journey Mutation.", 10),
                Node(SprintNodeId, "Open-ground Sprint", "Unlocks Open-ground Sprint as a journey Mutation; grants a provisional +0.5 movement speed on exposed ground. Current Bare and Grass terrain are exposed.", 10),
                Node("hare.keen-hearing", "Heightened Senses", "Unlocks Long Vision and Keen Senses journey Mutations.", 20,
                    SprintNodeId),
                Node("hare.swift-digging", "Long Stride", "Unlocks Faster Movement as a journey Mutation.",
                    20, "hare.keen-hearing"),
                Node("hare.safe-foraging", "Efficient Digestion", "Unlocks Efficient Digestion as a journey Mutation.",
                    20, "hare.guarded-burrow"),
                Node("hare.crowding-tolerance", "Crowding Tolerance", "Unlocks Crowding Tolerance as a journey Mutation.", 10),
                Node("hare.reproductive-drive", "Reproductive Drive", "Unlocks Reproductive Drive as a journey Mutation.", 20,
                    "hare.crowding-tolerance"),
                Node(BurrowingNodeId, "Burrowing", "Unlocks Burrowing as a journey Mutation.", 10),
                Node(SharedBurrowsNodeId, "Shared Burrows", "Unlocks Shared Burrows as a journey Mutation.", 20,
                    BurrowingNodeId),
                Node("hare.escape-instinct", "Escape Instinct", "Unlocks Escape Instinct as a journey Mutation.", 30,
                    "hare.keen-hearing"),
                Node("hare.seed-dispersal", "Seed Dispersal", "Unlocks Seed Dispersal as a journey Mutation.", 30,
                    "hare.escape-instinct", "hare.reproductive-drive"),
                Node(GestationUpgradeNodeId, "Gestation Upgrade", "Unlocks Faster Gestation as a journey Mutation.", 30,
                    "hare.seed-dispersal"),
                Node(ExpandedBurrowsNodeId, "Expanded Burrows", "Unlocks five journey ranks of Expanded Burrows.", 30,
                    GestationUpgradeNodeId, SharedBurrowsNodeId),
                new GenomeUpgradeNodeSnapshot(PopulationReservesNodeId, "Population Reserves",
                    "Raises purchased Hare allowance by 5 per researched rank for each simulation node.", hare,
                    new[] { ExpandedBurrowsNodeId, "hare.safe-foraging" }, 30, NodeCapacity, "FD2"),
                new GenomeUpgradeNodeSnapshot(FoundingHaresNodeId, "Founding Hares",
                    "+5 Hares at the start of a new journey. Research up to three ranks; does not refill each round.",
                    hare, new[] { GestationUpgradeNodeId }, 40, NodeCapacity, "FD2",
                    new[] { new SpeciesUpgradeModifier(SpeciesAttributeIds.StartingPopulation, 5) }, maximumRanks: 3),
                Node(LitterUpgradeNodeId, "Litter Upgrade", "Unlocks Increased Minimum Litter and three ranks of Larger Litters.",
                    40, "hare.seed-dispersal", maximumRanks: 3),
                Node(BornBurrowerNodeId, "Born Burrower", "Equip to start the journey with Burrowing. Excludes Open-ground Sprint.", 40,
                    ExpandedBurrowsNodeId),
                // Kept as an unavailable legacy ID so existing saves remain readable; the Litter Upgrade node owns both choices.
                new GenomeUpgradeNodeSnapshot("hare.increased-minimum-litter", "Increased Minimum Litter (legacy)",
                    "Legacy research ID. New profiles unlock both litter Mutations through Litter Upgrade.", hare,
                    rabbitDataPrice: 0, capacityPoints: 0, branchName: "Legacy"),
            };
            return new FieldDataPermanentProgressionDefinition(new SpeciesGenomeMapSnapshot(hare, nodes));
        }

        static GenomeUpgradeNodeSnapshot Node(string id, string name, string description, long price,
            params string[] prerequisites)
        {
            return new GenomeUpgradeNodeSnapshot(id, name, description, new SpeciesId("hare"),
                prerequisites, price, NodeCapacity, "FD2");
        }

        static GenomeUpgradeNodeSnapshot Node(string id, string name, string description, long price,
            string prerequisite, int maximumRanks)
        {
            return new GenomeUpgradeNodeSnapshot(id, name, description, new SpeciesId("hare"),
                new[] { prerequisite }, price, NodeCapacity, "FD2", maximumRanks: maximumRanks);
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

                // The purchase boundary enforces prerequisite arrows. Keep already-owned
                // legacy loadouts launchable when their old path predates the current catalog.
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
                    populationReservesRank: profile != null && profile.IsNodeActive("hare.warren-network") ? profile.PopulationReservesRank : 0,
                    foundingHaresRank: profile != null && profile.IsNodeActive(FoundingHaresNodeId) ? profile.FoundingHaresRank : 0),
            });
        }

    }
}
