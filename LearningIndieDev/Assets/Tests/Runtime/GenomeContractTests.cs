using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace SaltyGame.Tests
{
    [TestFixture]
    [Category("Simulation")]
    public sealed class GenomeContractTests
    {
        bool hadProfileSave;
        string originalProfileSave;
        bool hadRecoverySave;
        string originalRecoverySave;

        [SetUp]
        public void PreserveProfileSave()
        {
            hadProfileSave = PlayerPrefs.HasKey(Helper_ProfileSession.StoreKey);
            originalProfileSave = PlayerPrefs.GetString(Helper_ProfileSession.StoreKey);
            hadRecoverySave = PlayerPrefs.HasKey(Helper_ProfileSession.RecoveryStoreKey);
            originalRecoverySave = PlayerPrefs.GetString(Helper_ProfileSession.RecoveryStoreKey);
        }

        [TearDown]
        public void RestoreProfileSave()
        {
            RestoreKey(Helper_ProfileSession.StoreKey, hadProfileSave, originalProfileSave);
            RestoreKey(Helper_ProfileSession.RecoveryStoreKey, hadRecoverySave, originalRecoverySave);
            PlayerPrefs.Save();
        }

        static void RestoreKey(string key, bool existed, string value)
        {
            if (existed)
            {
                PlayerPrefs.SetString(key, value);
            }
            else
            {
                PlayerPrefs.DeleteKey(key);
            }
        }

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
                secondSession.Reload();

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

        [Test]
        public void ProfileSessionKeepsCurrencyProgressAndTimeSeparatePerProfile()
        {
            PlayerPrefs.DeleteKey(Helper_ProfileSession.StoreKey);
            PlayerPrefs.Save();
            GameObject firstObject = null;
            GameObject secondObject = null;
            try
            {
                firstObject = new GameObject("First Profile Session");
                var firstSession = firstObject.AddComponent<Helper_ProfileSession>();
                firstSession.SetPlaytimeTracking(false);
                var firstProfileId = firstSession.CreateProfile("First").ProfileId;
                firstSession.AddCurrency(100);
                Assert.That(firstSession.TrySpendCurrency(25), Is.True);
                Assert.That(firstSession.TrySpendCurrency(100), Is.False);
                firstSession.UnlockSpecies(new SpeciesId("fox"));
                firstSession.UnlockGenome(new SpeciesId("hare"));
                firstSession.UnlockFeature("test-feature");
                firstSession.UnlockFeature("test-feature");
                Assert.That(firstSession.TrySetGenomeConfiguration(
                    new SpeciesId("hare"),
                    new[] { "hare.guard" },
                    new[] { "hare.guard" },
                    out var validationMessage), Is.True, validationMessage);
                firstSession.RecordSimulationTicks(600);
                firstSession.RecordPlayedMilliseconds(7200000);
                firstSession.Flush();

                var secondProfileId = firstSession.CreateProfile("Second").ProfileId;
                firstSession.AddCurrency(9);
                firstSession.RecordSimulationTicks(10);
                firstSession.RecordPlayedMilliseconds(1800000);
                firstSession.Flush();

                secondObject = new GameObject("Reloaded Profile Session");
                var reloaded = secondObject.AddComponent<Helper_ProfileSession>();
                reloaded.Reload();
                reloaded.SetPlaytimeTracking(false);
                Assert.That(reloaded.Profiles.Count, Is.EqualTo(2));
                Assert.That(reloaded.Current.ProfileId, Is.EqualTo(secondProfileId));
                Assert.That(reloaded.Current.Currency, Is.EqualTo(9));
                Assert.That(reloaded.Current.SimulationTicks, Is.EqualTo(10));
                Assert.That(reloaded.Current.HoursPlayed, Is.EqualTo(0.5d));
                Assert.That(reloaded.Current.UnlockedFeatureIds, Is.Empty);

                Assert.That(reloaded.SelectProfile(firstProfileId), Is.True);
                Assert.That(reloaded.Current.Currency, Is.EqualTo(75));
                Assert.That(reloaded.Current.SimulationTicks, Is.EqualTo(600));
                Assert.That(reloaded.Current.HoursPlayed, Is.EqualTo(2d));
                Assert.That(reloaded.Current.UnlockedSpeciesIds, Does.Contain(new SpeciesId("hare")));
                Assert.That(reloaded.Current.UnlockedSpeciesIds, Does.Contain(new SpeciesId("fox")));
                Assert.That(reloaded.Current.UnlockedGenomeSpeciesIds, Does.Contain(new SpeciesId("hare")));
                Assert.That(reloaded.Current.UnlockedFeatureIds,
                    Is.EqualTo(new[] { "test-feature" }));
                Assert.That(reloaded.Current.TryGetGenomeProfile(new SpeciesId("hare"),
                    out var genomeProfile), Is.True);
                Assert.That(genomeProfile.UnlockedNodeIds, Is.EqualTo(new[] { "hare.guard" }));
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

        [Test]
        public void ProfileSessionLoadsLegacyProfileWithoutLosingItsIdentity()
        {
            PlayerPrefs.SetString(Helper_ProfileSession.StoreKey,
                "{\"profiles\":[{\"id\":\"legacy\",\"name\":\"Legacy\",\"genomeProfiles\":[]}],\"lastLoadedProfileId\":\"legacy\"}");
            PlayerPrefs.Save();
            GameObject sessionObject = null;
            try
            {
                sessionObject = new GameObject("Legacy Profile Session");
                var session = sessionObject.AddComponent<Helper_ProfileSession>();
                session.Reload();
                session.SetPlaytimeTracking(false);
                Assert.That(session.Current.ProfileId, Is.EqualTo("legacy"));
                Assert.That(session.Current.Currency, Is.Zero);
                Assert.That(session.Current.SimulationTicks, Is.Zero);
                Assert.That(session.Current.PlayTimeMilliseconds, Is.Zero);
                Assert.That(session.Current.UnlockedSpeciesIds,
                    Does.Contain(new SpeciesId("hare")));
                Assert.That(session.Current.UnlockedFeatureIds, Is.Empty);
            }
            finally
            {
                if (sessionObject != null)
                {
                    Object.DestroyImmediate(sessionObject);
                }

                PlayerPrefs.DeleteKey(Helper_ProfileSession.StoreKey);
                PlayerPrefs.Save();
            }
        }

        [Test]
        public void ProfileSessionPreservesUnreadableSaveForRecovery()
        {
            const string invalidJson = "not a profile store";
            PlayerPrefs.SetString(Helper_ProfileSession.StoreKey, invalidJson);
            PlayerPrefs.DeleteKey(Helper_ProfileSession.RecoveryStoreKey);
            PlayerPrefs.Save();
            GameObject sessionObject = null;
            try
            {
                sessionObject = new GameObject("Corrupt Profile Session");
                var session = sessionObject.AddComponent<Helper_ProfileSession>();
                session.Reload();
                session.SetPlaytimeTracking(false);
                Assert.That(session.HasProfiles, Is.False);
                Assert.That(session.LoadWarning, Is.Not.Empty);
                Assert.That(PlayerPrefs.GetString(Helper_ProfileSession.RecoveryStoreKey),
                    Is.EqualTo(invalidJson));
                Assert.That(session.CreateProfile("New Profile").HasLoadedProfile, Is.True);
                Assert.That(PlayerPrefs.GetString(Helper_ProfileSession.RecoveryStoreKey),
                    Is.EqualTo(invalidJson));
            }
            finally
            {
                if (sessionObject != null)
                {
                    Object.DestroyImmediate(sessionObject);
                }

                PlayerPrefs.DeleteKey(Helper_ProfileSession.StoreKey);
                PlayerPrefs.DeleteKey(Helper_ProfileSession.RecoveryStoreKey);
                PlayerPrefs.Save();
            }
        }

        [Test]
        public void ProfileSessionKeepsValidProfileWhenAnotherRecordIsInvalid()
        {
            const string partiallyInvalidJson =
                "{\"version\":1,\"profiles\":[{\"id\":\"valid\",\"name\":\"Kept\",\"currency\":7},"
                + "{\"id\":\"invalid\",\"name\":\"Discarded\",\"currency\":-1}],"
                + "\"lastLoadedProfileId\":\"valid\"}";
            PlayerPrefs.SetString(Helper_ProfileSession.StoreKey, partiallyInvalidJson);
            PlayerPrefs.Save();
            GameObject sessionObject = null;
            try
            {
                sessionObject = new GameObject("Partial Profile Session");
                var session = sessionObject.AddComponent<Helper_ProfileSession>();
                session.Reload();
                session.SetPlaytimeTracking(false);

                Assert.That(session.Current.ProfileId, Is.EqualTo("valid"));
                Assert.That(session.Current.Currency, Is.EqualTo(7));
                Assert.That(session.Profiles.Count, Is.EqualTo(1));
                Assert.That(session.LoadWarning, Is.Not.Empty);
                Assert.That(PlayerPrefs.GetString(Helper_ProfileSession.RecoveryStoreKey),
                    Is.EqualTo(partiallyInvalidJson));
            }
            finally
            {
                if (sessionObject != null)
                {
                    Object.DestroyImmediate(sessionObject);
                }
            }
        }
    }
}
