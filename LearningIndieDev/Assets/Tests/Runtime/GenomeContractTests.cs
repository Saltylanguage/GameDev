using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace SaltyGame.Tests
{
    [TestFixture]
    public sealed class GenomeContractTests
    {
        [Test]
        public void ProfileKeepsPermanentUnlocksSeparateFromActiveConfiguration()
        {
            var profile = new SpeciesGenomeProfile(
                SpeciesIds.Herbivore,
                new[] { "hare.guard", "hare.migrate" },
                new[] { "hare.guard" });

            Assert.That(profile.UnlockedNodeIds, Is.EqualTo(new[] { "hare.guard", "hare.migrate" }));
            Assert.That(profile.ActiveNodeIds, Is.EqualTo(new[] { "hare.guard" }));
            Assert.That(GenomeContract.ActiveCapacity, Is.EqualTo(8));
            Assert.That(profile.IsNodeUnlocked("hare.migrate"), Is.True);
            Assert.That(profile.IsNodeActive("hare.migrate"), Is.False);
        }

        [Test]
        public void ProfileRejectsActiveNodeThatIsNotUnlocked()
        {
            Assert.Throws<System.ArgumentException>(() => new SpeciesGenomeProfile(
                SpeciesIds.Herbivore,
                new[] { "hare.guard" },
                new[] { "hare.migrate" }));
        }

        [Test]
        public void ProfileSnapshotLaunchRequestAndRunKeepTheSameGenomeIdentity()
        {
            var profile = new ProfileSessionSnapshot(
                true,
                "profile-1",
                "Test Profile",
                new[]
                {
                    new SpeciesGenomeProfile(
                        SpeciesIds.Herbivore,
                        new[] { "hare.guard", "hare.migrate" },
                        new[] { "hare.migrate" }),
                    new SpeciesGenomeProfile(
                        SpeciesIds.Carnivore,
                        new[] { "fox.stalk" },
                        new[] { "fox.stalk" }),
                });
            var launch = new SimulationLaunchRequest(
                profile.ProfileId,
                "ForestEdge",
                SpeciesIds.Herbivore.Value,
                10100,
                activeGenomeSnapshot: profile.CreateGenomeSnapshot());
            var run = new SimulationRunState(
                new Grid<SpeciesCell>(1, 1),
                SpeciesIds.Herbivore,
                10100,
                1f,
                activeGenomeSnapshot: launch.ActiveGenomeSnapshot);

            Assert.That(launch.ActiveGenomeSnapshot.Fingerprint, Is.EqualTo(profile.CreateGenomeSnapshot().Fingerprint));
            Assert.That(run.ActiveGenomeSnapshot.Fingerprint, Is.EqualTo(launch.ActiveGenomeSnapshot.Fingerprint));
            Assert.That(run.ActiveGenomeSnapshot.GetSpeciesGenome(SpeciesIds.Carnivore).ActiveNodeIds,
                Is.EqualTo(new[] { "fox.stalk" }));
            Assert.That(run.ActiveGenomeSnapshot.GetSpeciesGenome(SpeciesIds.Plant).ActiveNodeIds,
                Is.Empty);
            Assert.That(SimulationRunResults.Create(run).ActiveGenomeSnapshot.Fingerprint,
                Is.EqualTo(run.ActiveGenomeSnapshot.Fingerprint));
        }

        [Test]
        public void GenomeSnapshotFingerprintDoesNotDependOnSpeciesOrNodeInputOrder()
        {
            var first = new GenomeSimulationSnapshot(new[]
            {
                new SpeciesGenomeSnapshot(SpeciesIds.Herbivore, new[] { "hare.migrate", "hare.guard" }),
                new SpeciesGenomeSnapshot(SpeciesIds.Carnivore, new[] { "fox.stalk" }),
            });
            var second = new GenomeSimulationSnapshot(new[]
            {
                new SpeciesGenomeSnapshot(SpeciesIds.Carnivore, new[] { "fox.stalk" }),
                new SpeciesGenomeSnapshot(SpeciesIds.Herbivore, new[] { "hare.guard", "hare.migrate" }),
            });

            Assert.That(first.Fingerprint, Is.EqualTo(second.Fingerprint));
        }

        [Test]
        public void GenomeCatalogSnapshotPreservesNodeTopologyWithoutAssetReferences()
        {
            var root = new GenomeUpgradeNodeSnapshot(
                "hare.root",
                "Root",
                "Root node.",
                new SpeciesId("hare"));
            var branch = new GenomeUpgradeNodeSnapshot(
                "hare.branch",
                "Branch",
                "Branch node.",
                new SpeciesId("hare"),
                new[] { "hare.root" });
            var map = new SpeciesGenomeMapSnapshot(
                new SpeciesId("hare"),
                new[] { root, branch });
            var catalog = new GenomeCatalogSnapshot(new[] { map });

            Assert.That(catalog.TryGetSpeciesMap(new SpeciesId("hare"), out var resolvedMap), Is.True);
            Assert.That(resolvedMap.Nodes, Is.EqualTo(new[] { root, branch }));
            Assert.That(resolvedMap.GetNode("hare.branch").PrerequisiteNodeIds,
                Is.EqualTo(new[] { "hare.root" }));
            Assert.That(catalog.Fingerprint, Is.Not.Empty);
        }

        [Test]
        public void GenomeMapRejectsMissingPrerequisitesAndCycles()
        {
            var missingPrerequisite = new GenomeUpgradeNodeSnapshot(
                "hare.branch",
                "Branch",
                "Branch node.",
                new SpeciesId("hare"),
                new[] { "hare.missing" });
            Assert.Throws<System.ArgumentException>(() => new SpeciesGenomeMapSnapshot(
                new SpeciesId("hare"),
                new[] { missingPrerequisite }));

            var first = new GenomeUpgradeNodeSnapshot(
                "hare.first",
                "First",
                "First node.",
                new SpeciesId("hare"),
                new[] { "hare.second" });
            var second = new GenomeUpgradeNodeSnapshot(
                "hare.second",
                "Second",
                "Second node.",
                new SpeciesId("hare"),
                new[] { "hare.first" });
            Assert.Throws<System.ArgumentException>(() => new SpeciesGenomeMapSnapshot(
                new SpeciesId("hare"),
                new[] { first, second }));
        }

        [Test]
        public void ProfileSessionPersistsGenomeConfiguration()
        {
            PlayerPrefs.DeleteKey(Helper_ProfileSession.StoreKey);
            PlayerPrefs.Save();
            GameObject firstObject = null;
            GameObject secondObject = null;
            try
            {
                firstObject = new GameObject("Genome Profile Session");
                var firstSession = firstObject.AddComponent<Helper_ProfileSession>();
                firstSession.CreateInitialProfile("Genome Test");
                Assert.That(firstSession.TrySetGenomeConfiguration(
                    SpeciesIds.Herbivore,
                    new[] { "hare.guard" },
                    new[] { "hare.guard" },
                    out var validationMessage), Is.True, validationMessage);

                secondObject = new GameObject("Reloaded Genome Profile Session");
                var secondSession = secondObject.AddComponent<Helper_ProfileSession>();

                Assert.That(secondSession.Current.HasLoadedProfile, Is.True);
                Assert.That(secondSession.Current.TryGetGenomeProfile(
                    SpeciesIds.Herbivore,
                    out var genomeProfile), Is.True);
                Assert.That(genomeProfile.ActiveNodeIds, Is.EqualTo(new[] { "hare.guard" }));
            }
            finally
            {
                if (firstObject != null)
                {
                    Object.DestroyImmediate(firstObject);
                }

                if (secondObject != null)
                {
                    Object.DestroyImmediate(secondObject);
                }

                PlayerPrefs.DeleteKey(Helper_ProfileSession.StoreKey);
                PlayerPrefs.Save();
            }
        }
    }
}
