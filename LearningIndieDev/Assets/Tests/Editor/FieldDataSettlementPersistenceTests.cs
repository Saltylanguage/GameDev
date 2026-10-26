using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaltyGame.Tests
{
    [TestFixture]
    [Category("Simulation")]
    public sealed class FieldDataSettlementPersistenceTests
    {
        sealed class Storage : IProfileSessionStorage
        {
            public Dictionary<string, string> Cached = new Dictionary<string, string>();
            public Dictionary<string, string> Durable = new Dictionary<string, string>();
            public readonly List<string> Writes = new List<string>();
            public int SaveCalls;
            public int FailOnSave = -1;
            public bool PersistThenThrow;
            public bool HasKey(string key) => Cached.ContainsKey(key);
            public string GetString(string key) => Cached.TryGetValue(key, out var value) ? value : string.Empty;
            public void SetString(string key, string value) => Cached[key] = value;
            public void DeleteKey(string key) => Cached.Remove(key);
            public void Save()
            {
                SaveCalls++;
                Writes.Add(GetString(Helper_ProfileSession.StoreKey));
                if (SaveCalls == FailOnSave)
                {
                    if (PersistThenThrow) Durable = new Dictionary<string, string>(Cached);
                    throw new IOException("Simulated profile storage failure.");
                }
                Durable = new Dictionary<string, string>(Cached);
            }
            public Storage Restart() => new Storage
            {
                Cached = new Dictionary<string, string>(Durable),
                Durable = new Dictionary<string, string>(Durable),
            };
        }

        readonly List<GameObject> objects = new List<GameObject>();
        readonly SpeciesId hare = new SpeciesId("hare");
        Storage storage;
        Helper_ProfileSession session;
        string profileId;

        [SetUp]
        public void SetUp()
        {
            storage = new Storage();
            session = NewSession(storage);
            profileId = session.CreateProfile("Settlement fixture").ProfileId;
            session.AddRabbitData(7);
            session.AddResearchData(30);
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var instance in objects) Object.DestroyImmediate(instance);
            objects.Clear();
        }

        Helper_ProfileSession NewSession(Storage source)
        {
            var instance = new GameObject("Isolated settlement persistence");
            instance.SetActive(false);
            objects.Add(instance);
            var result = instance.AddComponent<Helper_ProfileSession>();
            result.InitializeStorage(source);
            result.SetPlaytimeTracking(false);
            return result;
        }

        bool Settle(string id, out FieldDataSettlementReceipt receipt, out string error,
            long leftover = 100, int level = 1, FieldDataJourneyFlowState state = FieldDataJourneyFlowState.Succeeded)
        {
            return session.TrySettleFieldDataJourney(profileId, id, state, hare, leftover, level, out receipt, out error);
        }

        [Test]
        public void DepositsAndReceiptShareOneSavedSnapshotAndRepeatedIdNeverWritesOrPaysAgain()
        {
            var start = storage.Writes.Count;
            Assert.That(Settle("success-1", out var receipt, out var error), Is.True, error);
            AssertPaid(session, 18, 40, 1);
            Assert.That(session.Current.PendingSettlement, Is.Null);
            Assert.That(storage.Writes.Count - start, Is.EqualTo(2));
            var pending = JObject.Parse(storage.Writes[start])["profiles"][0];
            Assert.That((long)pending["researchData"], Is.EqualTo(30));
            Assert.That((long)pending["speciesDataWallets"][0]["balance"], Is.EqualTo(7));
            Assert.That(pending["settlementReceipts"].Count(), Is.Zero);
            Assert.That((string)pending["pendingSettlement"]["journeyId"], Is.EqualTo("success-1"));
            var paid = JObject.Parse(storage.Writes[start + 1])["profiles"][0];
            Assert.That((long)paid["researchData"], Is.EqualTo(40));
            Assert.That((long)paid["speciesDataWallets"][0]["balance"], Is.EqualTo(18));
            Assert.That(paid["settlementReceipts"].Count(), Is.EqualTo(1));
            Assert.That(paid["pendingSettlement"].Type, Is.EqualTo(JTokenType.Null));
            var writes = storage.SaveCalls;
            Assert.That(Settle("success-1", out var prior, out error, 999, 3), Is.True, error);
            Assert.That(prior, Is.SameAs(receipt));
            Assert.That(storage.SaveCalls, Is.EqualTo(writes));
            AssertPaid(session, 18, 40, 1);
            Assert.That(Settle("success-2", out _, out error, 0, 0), Is.True, error);
            AssertPaid(session, 18, 50, 2);
        }

        [TestCase(1, false)]
        [TestCase(2, false)]
        [TestCase(2, true)]
        public void SaveFailureNeverPublishesPaidBalancesAndRetryUsesFrozenSuccess(int failingWrite, bool persistThenThrow)
        {
            storage.FailOnSave = storage.SaveCalls + failingWrite;
            storage.PersistThenThrow = persistThenThrow;
            var publishedPaid = false;
            session.SnapshotChanged += value => publishedPaid |= value.SettlementReceipts.Count > 0;
            Assert.That(Settle("retry", out var receipt, out var error), Is.False);
            Assert.That(receipt, Is.Null);
            StringAssert.Contains("pending", error);
            AssertPaid(session, 7, 30, 0);
            Assert.That(publishedPaid, Is.False);
            Assert.That(session.Current.PendingSettlement.JourneyId, Is.EqualTo("retry"));
            Assert.That(Settle("different", out _, out _), Is.False);
            storage.FailOnSave = -1;
            Assert.That(Settle("retry", out receipt, out error, 9999, 3), Is.True, error);
            Assert.That(receipt.LeftoverFieldData, Is.EqualTo(100));
            Assert.That(receipt.ConversionResearchLevel, Is.EqualTo(1));
            AssertPaid(session, 18, 40, 1);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void RestartAfterUncertainFinalSaveFindsEitherPendingOrPaidAndStillPaysOnce(bool persisted)
        {
            storage.FailOnSave = storage.SaveCalls + 2;
            storage.PersistThenThrow = persisted;
            Assert.That(Settle("restart", out _, out _), Is.False);
            var rebootStorage = storage.Restart();
            var reboot = NewSession(rebootStorage);
            Assert.That(reboot.Current.ProfileId, Is.EqualTo(profileId));
            if (!persisted)
            {
                AssertPaid(reboot, 7, 30, 0);
                Assert.That(reboot.Current.PendingSettlement.JourneyId, Is.EqualTo("restart"));
                Assert.That(reboot.TryRetryPendingFieldDataSettlement(profileId, out _, out var retryError), Is.True, retryError);
            }
            Assert.That(reboot.TrySettleFieldDataJourney(profileId, "restart", FieldDataJourneyFlowState.Succeeded,
                hare, 999, 3, out var receipt, out var error), Is.True, error);
            Assert.That(receipt.LeftoverFieldData, Is.EqualTo(100));
            AssertPaid(reboot, 18, 40, 1);
        }

        [Test]
        public void ReloadCannotDiscardTheFirstUnpersistedSuccessWhileStorageStillFails()
        {
            storage.FailOnSave = storage.SaveCalls + 1;
            Assert.That(Settle("pending", out _, out _), Is.False);
            storage.FailOnSave = storage.SaveCalls + 1;
            session.Reload();
            Assert.That(session.Current.PendingSettlement.JourneyId, Is.EqualTo("pending"));
            AssertPaid(session, 7, 30, 0);
            Assert.That(session.LoadWarning, Is.Not.Empty);
            storage.FailOnSave = -1;
            Assert.That(session.TryRetryPendingFieldDataSettlement(profileId, out _, out var error), Is.True, error);
            session.Reload();
            AssertPaid(session, 18, 40, 1);
        }

        [Test]
        public void IdentityIsScopedToItsProfileAndSwitchingProfilesCannotReceiveAnotherProfilesDeposit()
        {
            var otherId = session.CreateProfile("Other").ProfileId;
            Assert.That(Settle("shared-id", out _, out _), Is.False);
            Assert.That(session.SelectProfile(profileId), Is.True);
            Assert.That(Settle("shared-id", out _, out var error), Is.True, error);
            Assert.That(session.SelectProfile(otherId), Is.True);
            Assert.That(session.Current.SettlementReceipts, Is.Empty);
            Assert.That(session.TrySettleFieldDataJourney(otherId, "shared-id", FieldDataJourneyFlowState.Succeeded,
                hare, 0, 0, out _, out error), Is.True, error);
            AssertPaid(session, 0, 10, 1);
            Assert.That(session.SelectProfile(profileId), Is.True);
            AssertPaid(session, 18, 40, 1);
        }

        [Test]
        public void PendingNotificationReentryAndProfileSwitchCannotPayTheWrongProfile()
        {
            var otherId = session.CreateProfile("Other").ProfileId;
            session.SelectProfile(profileId);
            var reentered = true;
            session.SnapshotChanged += value =>
            {
                if (value.PendingSettlement == null) return;
                reentered = Settle("reentry", out _, out _);
                session.SelectProfile(otherId);
            };
            Assert.That(Settle("reentry", out _, out var error), Is.False);
            StringAssert.Contains("profile changed", error);
            Assert.That(reentered, Is.False);
            AssertPaid(session, 0, 0, 0);
        }

        [TestCase(FieldDataJourneyFlowState.Failed, 100L, 1)]
        [TestCase(FieldDataJourneyFlowState.Running, 100L, 1)]
        [TestCase(FieldDataJourneyFlowState.Succeeded, -1L, 1)]
        [TestCase(FieldDataJourneyFlowState.Succeeded, 100L, 4)]
        public void RejectedJourneysDoNotWriteOrChangeAnyPermanentState(FieldDataJourneyFlowState state, long fd, int level)
        {
            var writes = storage.SaveCalls;
            Assert.That(Settle("rejected", out var receipt, out _, fd, level, state), Is.False);
            Assert.That(receipt, Is.Null);
            Assert.That(session.Current.PendingSettlement, Is.Null);
            Assert.That(storage.SaveCalls, Is.EqualTo(writes));
            AssertPaid(session, 7, 30, 0);
        }

        [Test]
        public void LegacyCompletionCannotBePaidAgainAndReceiptHistorySurvivesEveryProfileCopy()
        {
            session.RecordSuccessfulJourney("legacy");
            Assert.That(Settle("legacy", out _, out _), Is.False);
            Assert.That(Settle("receipt", out var receipt, out var error), Is.True, error);
            var snapshot = session.Current;
            var copies = new[] { snapshot.WithLoaded(false), snapshot.WithCurrency(999), snapshot.WithTime(5, 10),
                snapshot.WithUnlockedSpecies(new SpeciesId("fox")), snapshot.WithUnlockedGenome(hare),
                snapshot.WithUnlockedFeature("test"), snapshot.WithGenomeProfile(new SpeciesGenomeProfile(hare)),
                snapshot.WithGenomeEconomy(snapshot.RabbitData, snapshot.ResearchData, snapshot.CompletedRuns) };
            foreach (var copy in copies)
                Assert.That(copy.SettlementReceipts.Single(), Is.SameAs(receipt));
            session.Reload();
            Assert.That(session.Current.SettlementReceipts.Single().SpeciesDataAfter, Is.EqualTo(receipt.SpeciesDataAfter));
        }

        [TestCase("missing")]
        [TestCase("duplicate")]
        [TestCase("arithmetic")]
        [TestCase("version")]
        [TestCase("pending")]
        public void CorruptedJournalIsPreservedAndRejectedRatherThanLosingDuplicateProtection(string corruption)
        {
            Assert.That(Settle("safe", out _, out var error), Is.True, error);
            var json = JObject.Parse(storage.GetString(Helper_ProfileSession.StoreKey));
            var profile = json["profiles"][0];
            if (corruption == "missing") ((JObject)profile).Remove("settlementReceipts");
            if (corruption == "duplicate") ((JArray)profile["settlementReceipts"]).Add(profile["settlementReceipts"][0].DeepClone());
            if (corruption == "arithmetic") profile["settlementReceipts"][0]["speciesDataAfter"] = 900;
            if (corruption == "version") profile["settlementReceipts"][0]["definitionVersion"] = "unknown-version";
            if (corruption == "pending") profile["pendingSettlement"] = new JObject { ["journeyId"] = "safe", ["speciesId"] = "hare" };
            var original = json.ToString();
            storage.SetString(Helper_ProfileSession.StoreKey, original);
            storage.SetString(Helper_ProfileSession.RecoveryStoreKey, "existing recovery");
            session.Reload();
            Assert.That(session.HasProfiles, Is.False);
            Assert.That(session.LoadWarning, Is.Not.Empty);
            Assert.That(storage.GetString(Helper_ProfileSession.StoreKey), Is.EqualTo(original));
            Assert.That(storage.GetString(Helper_ProfileSession.RecoveryStoreKey), Is.EqualTo("existing recovery"));
            session.CreateProfile("Recovered");
            StringAssert.Contains("migrationSourceJson", storage.GetString(Helper_ProfileSession.StoreKey));
            Assert.That((string)JObject.Parse(storage.GetString(Helper_ProfileSession.StoreKey))["migrationSourceJson"], Is.EqualTo(original));
        }

        [Test]
        public void PendingSettlementPreservesOtherWalletsAndUsesCurrentBalancesWithoutChangingItsFrozenAward()
        {
            var json = JObject.Parse(storage.GetString(Helper_ProfileSession.StoreKey));
            ((JArray)json["profiles"][0]["speciesDataWallets"]).Add(new JObject { ["speciesId"] = "fox", ["balance"] = 400 });
            storage.SetString(Helper_ProfileSession.StoreKey, json.ToString());
            session.Reload();
            storage.FailOnSave = storage.SaveCalls + 2;
            Assert.That(Settle("pending-change", out _, out _), Is.False);
            storage.FailOnSave = -1;
            session.AddResearchData(5);
            session.AddRabbitData(2);
            Assert.That(session.Current.PendingSettlement.LeftoverFieldData, Is.EqualTo(100));
            Assert.That(session.TryRetryPendingFieldDataSettlement(profileId, out var receipt, out var error), Is.True, error);
            Assert.That(receipt.SpeciesDataBefore, Is.EqualTo(9));
            Assert.That(receipt.SpeciesDataDeposit, Is.EqualTo(11));
            AssertPaid(session, 20, 45, 1);
            Assert.That(session.Current.SpeciesDataWallets[new SpeciesId("fox")], Is.EqualTo(400));
            session.Reload();
            AssertPaid(session, 20, 45, 1);
        }

        [Test]
        public void MissingIdentityUnsupportedSpeciesAndOverflowDoNotStartATransaction()
        {
            var saves = storage.SaveCalls;
            foreach (var id in new[] { null, "", " " })
                Assert.That(Settle(id, out _, out _), Is.False);
            Assert.That(session.TrySettleFieldDataJourney(profileId, "unsupported", FieldDataJourneyFlowState.Succeeded,
                new SpeciesId("fox"), 100, 1, out _, out _), Is.False);
            session.AddResearchData(long.MaxValue - session.Current.ResearchData);
            saves++;
            Assert.That(Settle("overflow", out _, out _), Is.False);
            Assert.That(session.Current.PendingSettlement, Is.Null);
            Assert.That(session.Current.SettlementReceipts, Is.Empty);
            Assert.That(storage.SaveCalls, Is.EqualTo(saves));
            Assert.That(session.Current.RabbitData, Is.EqualTo(7));
        }

        [Test]
        public void VersionFourMigrationPreservesWalletsHistoryAndTheEarlierRawMigrationArchive()
        {
            const string old = "{\"version\":4,\"profiles\":[{\"id\":\"old\",\"name\":\"Old\",\"researchData\":20,\"speciesDataWallets\":[{\"speciesId\":\"hare\",\"balance\":7}],\"completedRuns\":1,\"completedRunIds\":[\"legacy\"]}],\"lastLoadedProfileId\":\"old\",\"migrationSourceJson\":\"earlier raw save\"}";
            storage.SetString(Helper_ProfileSession.StoreKey, old);
            session.Reload();
            AssertPaid(session, 7, 20, 0);
            Assert.That(session.Current.CompletedRuns, Is.EqualTo(1));
            Assert.That(session.Current.CompletedRunIds.Single(), Is.EqualTo("legacy"));
            Assert.That((int)JObject.Parse(storage.GetString(Helper_ProfileSession.StoreKey))["version"], Is.EqualTo(5));
            Assert.That((string)JObject.Parse(storage.GetString(Helper_ProfileSession.StoreKey))["migrationSourceJson"], Is.EqualTo("earlier raw save"));
        }

        [Test]
        public void ProcessReloadCheckpoint()
        {
            var phase = Environment.GetEnvironmentVariable("FD2_SETTLEMENT_RELOAD_PHASE");
            var path = Environment.GetEnvironmentVariable("FD2_SETTLEMENT_RELOAD_PATH");
            if (phase == "read")
            {
                storage.SetString(Helper_ProfileSession.StoreKey, File.ReadAllText(path));
                storage.Save();
                session.Reload();
                profileId = session.Current.ProfileId;
                var saves = storage.SaveCalls;
                Assert.That(Settle("process-reload", out var prior, out var readError, 9999, 3), Is.True, readError);
                Assert.That(prior.LeftoverFieldData, Is.EqualTo(100));
                AssertPaid(session, 18, 40, 1);
                Assert.That(storage.SaveCalls, Is.EqualTo(saves));
                return;
            }
            Assert.That(Settle("process-reload", out _, out var error), Is.True, error);
            if (phase == "write") File.WriteAllText(path, storage.Durable[Helper_ProfileSession.StoreKey]);
            var reopened = NewSession(storage.Restart());
            AssertPaid(reopened, 18, 40, 1);
        }

        static void AssertPaid(Helper_ProfileSession target, long speciesData, long researchData, int receiptCount)
        {
            Assert.That(target.Current.RabbitData, Is.EqualTo(speciesData));
            Assert.That(target.Current.ResearchData, Is.EqualTo(researchData));
            Assert.That(target.Current.SettlementReceipts.Count, Is.EqualTo(receiptCount));
        }
    }
}
