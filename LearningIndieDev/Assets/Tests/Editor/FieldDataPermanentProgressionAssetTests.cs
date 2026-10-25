using NUnit.Framework;
using UnityEditor;

namespace SaltyGame.EditorTests
{
    [TestFixture]
    [Category("Authoring")]
    public sealed class FieldDataPermanentProgressionAssetTests
    {
        [Test]
        public void ApprovedFiveNodeAssetCatalogMatchesTheSingleRuntimeDefinition()
        {
            var map = AssetDatabase.LoadAssetAtPath<SpeciesGenomeMapAsset>(
                FieldDataPermanentProgressionDefinition.HareMapPath);
            Assert.That(map, Is.Not.Null);
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            Assert.DoesNotThrow(() => definition.ValidateAuthoredMap(map.CreateSnapshot()));
            Assert.That(map.Upgrades, Has.Length.EqualTo(5));
            foreach (var node in map.Upgrades)
            {
                Assert.That(node.UpgradeId, Does.StartWith("hare."));
                Assert.That(AssetDatabase.GetAssetPath(node),
                    Does.StartWith("Assets/Data/TestData/Genome/HareGenome/"));
            }
        }
    }
}
