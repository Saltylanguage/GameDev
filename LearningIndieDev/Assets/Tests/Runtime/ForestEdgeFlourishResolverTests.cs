using System.Collections.Generic;
using NUnit.Framework;

namespace SaltyGame.Tests
{
    public sealed class ForestEdgeFlourishResolverTests
    {
        [Test]
        public void FlourishesStayOnFullGrassAndRemainStableUnderAnimals()
        {
            var grass = SpeciesCell.Grass(96);
            var fullGrass = SimulationCellSnapshot.Create(grass, TerrainTileResolver.FullMask);
            var animalOnGrass = SimulationCellSnapshot.Create(
                grass.WithEntity(new SpeciesId("hare"), 1, 12, 0, 0, 0f), TerrainTileResolver.FullMask);
            var variants = new HashSet<int>();
            var decorated = 0;
            for (var y = 0; y < 20; y++)
            {
                for (var x = 0; x < 36; x++)
                {
                    var index = ForestEdgeFlourishResolver.Resolve(fullGrass, x, y);
                    Assert.That(ForestEdgeFlourishResolver.Resolve(animalOnGrass, x, y), Is.EqualTo(index));
                    Assert.That(index, Is.InRange(-1, ForestEdgeFlourishResolver.SpriteNames.Count - 1));
                    if (index >= 0) { variants.Add(index); decorated++; }
                    Assert.That(ForestEdgeFlourishResolver.Resolve(
                        SimulationCellSnapshot.Create(grass, 127), x, y), Is.EqualTo(-1));
                    Assert.That(ForestEdgeFlourishResolver.Resolve(
                        SimulationCellSnapshot.Create(SpeciesCell.Empty, 255), x, y), Is.EqualTo(-1));
                }
            }
            Assert.That(ForestEdgeFlourishResolver.SpriteNames.Count, Is.EqualTo(10));
            Assert.That(variants.Count, Is.EqualTo(ForestEdgeFlourishResolver.SpriteNames.Count));
            Assert.That(decorated, Is.InRange(180, 300));
        }
    }
}
