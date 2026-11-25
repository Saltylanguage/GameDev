using NUnit.Framework;
using UnityEditor;
using System.Collections.Generic;

namespace SaltyGame.EditorTests
{
    [TestFixture]
    [Category("Authoring")]
    public sealed class FieldDataPermanentProgressionAssetTests
    {
        [Test]
        public void AuthoredCatalogMatchesTheSingleRuntimeDefinition()
        {
            var map = AssetDatabase.LoadAssetAtPath<SpeciesGenomeMapAsset>(
                FieldDataPermanentProgressionDefinition.RabbitMapPath);
            Assert.That(map, Is.Not.Null);
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            Assert.DoesNotThrow(() => definition.ValidateAuthoredMap(map.CreateSnapshot()));
            Assert.That(map.Upgrades, Has.Length.EqualTo(definition.RabbitMap.Nodes.Count));
            foreach (var node in map.Upgrades)
            {
                Assert.That(node.UpgradeId, Does.StartWith("rabbit."));
                Assert.That(AssetDatabase.GetAssetPath(node),
                    Does.StartWith("Assets/Data/TestData/Genome/RabbitGenome/"));
            }
        }

        [Test]
        public void AuthoredCatalogUsesTheApprovedPricesRanksAndPrerequisiteArrows()
        {
            var map = AssetDatabase.LoadAssetAtPath<SpeciesGenomeMapAsset>(
                FieldDataPermanentProgressionDefinition.RabbitMapPath).CreateSnapshot();
            var expected = new Dictionary<string, (long price, int ranks, string[] prerequisites)>
            {
                [FieldDataPermanentProgressionDefinition.SprintNodeId] = (10, 1, System.Array.Empty<string>()),
                ["rabbit.crowding-tolerance"] = (10, 1, System.Array.Empty<string>()),
                [FieldDataPermanentProgressionDefinition.BurrowingNodeId] = (10, 1, System.Array.Empty<string>()),
                ["rabbit.guarded-burrow"] = (10, 1, System.Array.Empty<string>()),
                ["rabbit.keen-hearing"] = (20, 1, new[] { FieldDataPermanentProgressionDefinition.SprintNodeId }),
                ["rabbit.swift-digging"] = (20, 1, new[] { "rabbit.keen-hearing" }),
                ["rabbit.safe-foraging"] = (20, 1, new[] { "rabbit.guarded-burrow" }),
                ["rabbit.reproductive-drive"] = (20, 1, new[] { "rabbit.crowding-tolerance" }),
                [FieldDataPermanentProgressionDefinition.SharedBurrowsNodeId] = (20, 1, new[] { FieldDataPermanentProgressionDefinition.BurrowingNodeId }),
                ["rabbit.escape-instinct"] = (30, 1, new[] { "rabbit.keen-hearing" }),
                ["rabbit.seed-dispersal"] = (30, 1, new[] { "rabbit.escape-instinct", "rabbit.reproductive-drive" }),
                [FieldDataPermanentProgressionDefinition.GestationUpgradeNodeId] = (30, 1, new[] { "rabbit.seed-dispersal" }),
                [FieldDataPermanentProgressionDefinition.ExpandedBurrowsNodeId] = (30, 1,
                    new[] { FieldDataPermanentProgressionDefinition.GestationUpgradeNodeId, FieldDataPermanentProgressionDefinition.SharedBurrowsNodeId }),
                [FieldDataPermanentProgressionDefinition.PopulationReservesNodeId] = (30, 1,
                    new[] { FieldDataPermanentProgressionDefinition.ExpandedBurrowsNodeId, "rabbit.safe-foraging" }),
                [FieldDataPermanentProgressionDefinition.LitterUpgradeNodeId] = (40, 3, new[] { "rabbit.seed-dispersal" }),
                [FieldDataPermanentProgressionDefinition.FoundingRabbitsNodeId] = (40, 3,
                    new[] { FieldDataPermanentProgressionDefinition.GestationUpgradeNodeId }),
                [FieldDataPermanentProgressionDefinition.BornBurrowerNodeId] = (40, 1,
                    new[] { FieldDataPermanentProgressionDefinition.ExpandedBurrowsNodeId }),
            };

            foreach (var pair in expected)
            {
                var node = map.GetNode(pair.Key);
                Assert.That(node, Is.Not.Null, pair.Key);
                Assert.That(node.RabbitDataPrice, Is.EqualTo(pair.Value.price), pair.Key);
                Assert.That(node.MaximumRanks, Is.EqualTo(pair.Value.ranks), pair.Key);
                CollectionAssert.AreEquivalent(pair.Value.prerequisites, node.PrerequisiteNodeIds, pair.Key);
            }
            for (var rank = 1; rank <= FieldDataPermanentProgressionDefinition.PopulationReservesMaximumRank; rank++)
                Assert.That(FieldDataPermanentProgressionDefinition.PopulationReservesResearchCost(rank), Is.EqualTo(30));
        }
    }
}
