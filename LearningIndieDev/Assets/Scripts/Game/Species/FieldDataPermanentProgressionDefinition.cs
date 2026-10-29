using System;
using System.Collections.Generic;

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
        public const int NodeCapacity = 2;
        public const int MaximumActiveCapacity = 8;
        public const long ResetResearchDataFee = 20;
        public const string HareMapPath = "Assets/Data/TestData/Genome/HareGenome.asset";

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
                Node("hare.guarded-burrow", "Tough Hide", "Block +1 while active.",
                    SpeciesAttributeIds.BlockAmount, 1),
                Node("hare.keen-hearing", "Long Vision", "Vision range +2 tiles while active.",
                    SpeciesAttributeIds.VisionRange, 2, "hare.guarded-burrow"),
                Node("hare.swift-digging", "Long Stride", "Movement speed +0.25 while active.",
                    SpeciesAttributeIds.MovementSpeed, 0.25f, "hare.guarded-burrow"),
                Node("hare.safe-foraging", "Efficient Digestion", "Digestion energy bonus +0.25 while active.",
                    SpeciesAttributeIds.DigestionEnergyBonus, 0.25f, "hare.keen-hearing", "hare.swift-digging"),
                Node("hare.warren-network", "Population Reserves",
                    "Ordinary replenishment capacity +10 while active. Does not spawn Hares or limit births.",
                    SpeciesAttributeIds.PopulationPoolCapacity, 10, "hare.guarded-burrow", "hare.safe-foraging"),
            };
            return new FieldDataPermanentProgressionDefinition(new SpeciesGenomeMapSnapshot(hare, nodes));
        }

        static GenomeUpgradeNodeSnapshot Node(string id, string name, string description,
            string attribute, float amount, params string[] prerequisites)
        {
            return new GenomeUpgradeNodeSnapshot(id, name, description, new SpeciesId("hare"),
                prerequisites, NodePrice, NodeCapacity, "FD2",
                new[] { new SpeciesUpgradeModifier(attribute, amount) });
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
                capacity = checked(capacity + node.CapacityPoints);
                if (capacity > ActiveCapacity)
                {
                    throw new InvalidOperationException("The FD2 loadout exceeds active capacity.");
                }

                resolvedNodes.Add(node);
            }

            return new GenomeSimulationSnapshot(new[]
            {
                new SpeciesGenomeSnapshot(HareMap.SpeciesId, activeIds, resolvedNodes, Catalog.Fingerprint),
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
