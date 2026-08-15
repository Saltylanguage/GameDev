using NUnit.Framework;
using UnityEngine;

namespace SaltyGame.Tests
{
    public sealed class SpeciesSimulationPlayerSpeciesSelectionTests
    {
        GameObject root;

        [SetUp]
        public void SetUp()
        {
            root = new GameObject("Species Simulation Player Species Test");
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(root);
        }

        [Test]
        public void LegacyRosterOffersOnlyCreatureSpeciesForPlayerSelection()
        {
            var preview = root.AddComponent<SpeciesSimulationPreview>();

            Assert.That(preview.RosterSpecies, Is.EqualTo(new[]
            {
                SpeciesIds.Plant,
                SpeciesIds.Herbivore,
                SpeciesIds.Carnivore,
            }));
            Assert.That(preview.PlayableSpecies, Is.EqualTo(new[]
            {
                SpeciesIds.Herbivore,
                SpeciesIds.Carnivore,
            }));
        }

        [Test]
        public void PlayerSelectionRejectsPlantsAndPreparesTheSelectedCreature()
        {
            var preview = root.AddComponent<SpeciesSimulationPreview>();

            Assert.That(preview.TrySetPlayerSpecies(SpeciesIds.Plant.Value, out var plantMessage), Is.False);
            Assert.That(plantMessage, Does.Contain("not playable"));

            Assert.That(preview.TrySetPlayerSpecies(SpeciesIds.Carnivore.Value, out var selectionMessage), Is.True);
            Assert.That(selectionMessage, Does.Contain(SpeciesIds.Carnivore.Value));
            Assert.That(preview.PlayerSpecies, Is.EqualTo(SpeciesIds.Carnivore));
            Assert.That(preview.Run.PlayerSpeciesId, Is.EqualTo(SpeciesIds.Carnivore));
        }
    }
}
