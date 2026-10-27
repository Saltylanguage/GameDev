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
            session.AddResearchData(40);
            Assert.That(session.TryPurchaseConversionResearch(profileId, 0, out var error), Is.True, error);
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
            Assert.That(Settle("success-2", out _, out error, 0, 1), Is.True, error);
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
            {
                Assert.That(copy.SettlementReceipts.Single(), Is.SameAs(receipt));
                Assert.That(copy.ConversionResearchLevel, Is.EqualTo(1));
            }
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
            Assert.That((int)JObject.Parse(storage.GetString(Helper_ProfileSession.StoreKey))["version"], Is.EqualTo(6));
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
                Assert.That(session.Current.ConversionResearchLevel, Is.EqualTo(1));
                Assert.That(storage.SaveCalls, Is.EqualTo(saves));
                return;
            }
            Assert.That(Settle("process-reload", out _, out var error), Is.True, error);
            if (phase == "write") File.WriteAllText(path, storage.Durable[Helper_ProfileSession.StoreKey]);
            var reopened = NewSession(storage.Restart());
            AssertPaid(reopened, 18, 40, 1);
        }

        void StartResearch(long balance, int level = 0)
        {
            storage = new Storage();
            session = NewSession(storage);
            profileId = session.CreateProfile("Conversion research fixture").ProfileId;
            session.AddRabbitData(7);
            var earlierCosts = level == 0 ? 0 : level == 1 ? 10 : level == 2 ? 30 : 60;
            session.AddResearchData(balance + earlierCosts);
            for (var current = 0; current < level; current++)
                Assert.That(session.TryPurchaseConversionResearch(profileId, current, out var error), Is.True, error);
        }

        [TestCase(0, 0L, false, 0L)]
        [TestCase(0, 9L, false, 0L)]
        [TestCase(0, 10L, true, 10L)]
        [TestCase(0, 11L, true, 10L)]
        [TestCase(1, 19L, false, 0L)]
        [TestCase(1, 20L, true, 20L)]
        [TestCase(2, 29L, false, 0L)]
        [TestCase(2, 30L, true, 30L)]
        [TestCase(3, 100L, false, 0L)]
        public void ResearchChargesTheApprovedPriceAndPersistsLevelAndDeductionTogether(int level, long balance, bool succeeds, long price)
        {
            StartResearch(balance, level);
            var saves = storage.SaveCalls;
            Assert.That(session.TryPurchaseConversionResearch(profileId, level, out var error), Is.EqualTo(succeeds), error);
            Assert.That(session.Current.ConversionResearchLevel, Is.EqualTo(level + (succeeds ? 1 : 0)));
            Assert.That(session.Current.ResearchData, Is.EqualTo(balance - price));
            Assert.That(session.Current.RabbitData, Is.EqualTo(7));
            Assert.That(session.Current.SettlementReceipts, Is.Empty);
            Assert.That(session.Current.CompletedRuns, Is.Zero);
            Assert.That(storage.SaveCalls - saves, Is.EqualTo(succeeds ? 1 : 0));
            if (succeeds)
            {
                var saved = JObject.Parse(storage.Writes.Last())["profiles"][0];
                Assert.That((int)saved["conversionResearchLevel"], Is.EqualTo(level + 1));
                Assert.That((long)saved["researchData"], Is.EqualTo(balance - price));
            }
            var reboot = NewSession(storage.Restart());
            Assert.That(reboot.Current.ConversionResearchLevel, Is.EqualTo(session.Current.ConversionResearchLevel));
            Assert.That(reboot.Current.ResearchData, Is.EqualTo(balance - price));
        }

        [Test]
        public void ResearchRejectsStaleCallbacksWrongProfilesAndReentrantRequestsWithoutAnotherCharge()
        {
            StartResearch(60);
            var saves = storage.SaveCalls;
            Assert.That(session.TryPurchaseConversionResearch("another-profile", 0, out _), Is.False);
            Assert.That(session.TryPurchaseConversionResearch(profileId, -1, out _), Is.False);
            Assert.That(session.TryPurchaseConversionResearch(profileId, 1, out _), Is.False);
            Assert.That(storage.SaveCalls, Is.EqualTo(saves));
            var reentered = true;
            session.SnapshotChanged += value => reentered = session.TryPurchaseConversionResearch(profileId,
                value.ConversionResearchLevel, out _);
            Assert.That(session.TryPurchaseConversionResearch(profileId, 0, out var error), Is.True, error);
            Assert.That(reentered, Is.False);
            Assert.That(session.TryPurchaseConversionResearch(profileId, 0, out _), Is.False);
            Assert.That(session.Current.ConversionResearchLevel, Is.EqualTo(1));
            Assert.That(session.Current.ResearchData, Is.EqualTo(50));
            Assert.That(storage.SaveCalls, Is.EqualTo(saves + 1));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void ResearchFailurePublishesNoPurchaseAndInMemoryRetryChargesOnce(bool persistThenThrow)
        {
            StartResearch(60);
            storage.FailOnSave = storage.SaveCalls + 1;
            storage.PersistThenThrow = persistThenThrow;
            var published = false;
            session.SnapshotChanged += _ => published = true;
            Assert.That(session.TryPurchaseConversionResearch(profileId, 0, out var error), Is.False);
            StringAssert.Contains("could not be saved", error);
            Assert.That(published, Is.False);
            Assert.That(session.Current.ConversionResearchLevel, Is.Zero);
            Assert.That(session.Current.ResearchData, Is.EqualTo(60));
            storage.FailOnSave = -1;
            Assert.That(session.TryPurchaseConversionResearch(profileId, 0, out error), Is.True, error);
            Assert.That(session.Current.ConversionResearchLevel, Is.EqualTo(1));
            Assert.That(session.Current.ResearchData, Is.EqualTo(50));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void ResearchRestartAfterAnUncertainSaveCannotBuyTheSameLevelTwice(bool persisted)
        {
            StartResearch(60);
            storage.FailOnSave = storage.SaveCalls + 1;
            storage.PersistThenThrow = persisted;
            Assert.That(session.TryPurchaseConversionResearch(profileId, 0, out _), Is.False);
            var rebootStorage = storage.Restart();
            var reboot = NewSession(rebootStorage);
            Assert.That(reboot.TryPurchaseConversionResearch(profileId, 0, out _), Is.EqualTo(!persisted));
            Assert.That(reboot.Current.ConversionResearchLevel, Is.EqualTo(1));
            Assert.That(reboot.Current.ResearchData, Is.EqualTo(50));
            var saves = rebootStorage.SaveCalls;
            Assert.That(reboot.TryPurchaseConversionResearch(profileId, 0, out _), Is.False);
            Assert.That(rebootStorage.SaveCalls, Is.EqualTo(saves));
        }

        [Test]
        public void ResearchOnlyAffectsFutureSettlementsAndNeverRepricesPendingOrCompletedReceipts()
        {
            StartResearch(1000);
            Assert.That(session.TrySettleFieldDataJourney(profileId, "before-research", FieldDataJourneyFlowState.Succeeded,
                hare, 1000, out var original, out var error), Is.True, error);
            Assert.That(original.ConversionResearchLevel, Is.Zero);
            Assert.That(original.SpeciesDataDeposit, Is.EqualTo(100));
            Assert.That(session.TryPurchaseConversionResearch(profileId, 0, out error), Is.True, error);
            storage.FailOnSave = storage.SaveCalls + 2;
            Assert.That(session.TrySettleFieldDataJourney(profileId, "pending-at-one", FieldDataJourneyFlowState.Succeeded,
                hare, 1000, out _, out _), Is.False);
            storage.FailOnSave = -1;
            Assert.That(session.TryPurchaseConversionResearch(profileId, 1, out error), Is.True, error);
            Assert.That(session.Current.PendingSettlement.ConversionResearchLevel, Is.EqualTo(1));
            Assert.That(session.TryRetryPendingFieldDataSettlement(profileId, out var pending, out error), Is.True, error);
            Assert.That(pending.ConversionResearchLevel, Is.EqualTo(1));
            Assert.That(pending.SpeciesDataDeposit, Is.EqualTo(110));
            Assert.That(session.TrySettleFieldDataJourney(profileId, "after-research", FieldDataJourneyFlowState.Succeeded,
                hare, 1000, out var after, out error), Is.True, error);
            Assert.That(after.ConversionResearchLevel, Is.EqualTo(2));
            Assert.That(after.SpeciesDataDeposit, Is.EqualTo(121));
            Assert.That(session.Current.RabbitData, Is.EqualTo(338));
            Assert.That(session.Current.ResearchData, Is.EqualTo(1000));
            session.Reload();
            Assert.That(session.Current.ConversionResearchLevel, Is.EqualTo(2));
            Assert.That(session.Current.SettlementReceipts[0].ConversionResearchLevel, Is.Zero);
            Assert.That(session.Current.SettlementReceipts[0].SpeciesDataDeposit, Is.EqualTo(100));
            Assert.That(session.Current.SettlementReceipts[1].SpeciesDataDeposit, Is.EqualTo(110));
            Assert.That(session.TrySettleFieldDataJourney(profileId, "before-research", FieldDataJourneyFlowState.Succeeded,
                hare, 9999, out var prior, out error), Is.True, error);
            Assert.That(prior.SpeciesDataDeposit, Is.EqualTo(100));
        }

        [Test]
        public void ResearchIsSharedWithinOneProfileAndNeverChargesASpeciesWalletOrAnotherProfile()
        {
            StartResearch(60);
            var json = JObject.Parse(storage.GetString(Helper_ProfileSession.StoreKey));
            ((JArray)json["profiles"][0]["speciesDataWallets"]).Add(new JObject { ["speciesId"] = "fox", ["balance"] = 400 });
            storage.SetString(Helper_ProfileSession.StoreKey, json.ToString());
            session.Reload();
            Assert.That(session.TryPurchaseConversionResearch(profileId, 0, out var error), Is.True, error);
            Assert.That(session.Current.SpeciesDataWallets[new SpeciesId("fox")], Is.EqualTo(400));
            Assert.That(session.Current.RabbitData, Is.EqualTo(7));
            var other = session.CreateProfile("Independent research").ProfileId;
            Assert.That(session.Current.ConversionResearchLevel, Is.Zero);
            Assert.That(session.TryPurchaseConversionResearch(profileId, 1, out _), Is.False);
            session.SelectProfile(profileId);
            Assert.That(session.Current.ConversionResearchLevel, Is.EqualTo(1));
            Assert.That(session.Current.ResearchData, Is.EqualTo(50));
        }

        [Test]
        public void ResearchSettlementCannotClaimAnUnpurchasedLevel()
        {
            StartResearch(100);
            var saves = storage.SaveCalls;
            Assert.That(session.TrySettleFieldDataJourney(profileId, "spoofed", FieldDataJourneyFlowState.Succeeded,
                hare, 100, 3, out _, out _), Is.False);
            Assert.That(session.Current.PendingSettlement, Is.Null);
            Assert.That(storage.SaveCalls, Is.EqualTo(saves));
            Assert.That(session.TrySettleFieldDataJourney(profileId, "legitimate", FieldDataJourneyFlowState.Succeeded,
                hare, 100, out var receipt, out var error), Is.True, error);
            Assert.That(receipt.ConversionResearchLevel, Is.Zero);
            Assert.That(receipt.SpeciesDataDeposit, Is.EqualTo(10));
        }

        [TestCase(-1)]
        [TestCase(4)]
        public void ResearchSnapshotRejectsOutOfRangeLevels(int level)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ProfileSessionSnapshot(true, "id", "Invalid research",
                conversionResearchLevel: level));
        }

        [TestCase("missing")]
        [TestCase("null")]
        [TestCase("negative")]
        [TestCase("over-cap")]
        public void ResearchCorruptLevelIsPreservedForRecoveryInsteadOfResettingPurchases(string corruption)
        {
            StartResearch(60);
            var json = JObject.Parse(storage.GetString(Helper_ProfileSession.StoreKey));
            var profile = (JObject)json["profiles"][0];
            if (corruption == "missing") profile.Remove("conversionResearchLevel");
            if (corruption == "null") profile["conversionResearchLevel"] = null;
            if (corruption == "negative") profile["conversionResearchLevel"] = -1;
            if (corruption == "over-cap") profile["conversionResearchLevel"] = 4;
            var original = json.ToString();
            storage.SetString(Helper_ProfileSession.StoreKey, original);
            storage.SetString(Helper_ProfileSession.RecoveryStoreKey, "prior recovery");
            session.Reload();
            Assert.That(session.HasProfiles, Is.False);
            Assert.That(session.LoadWarning, Is.Not.Empty);
            Assert.That(storage.GetString(Helper_ProfileSession.StoreKey), Is.EqualTo(original));
            Assert.That(storage.GetString(Helper_ProfileSession.RecoveryStoreKey), Is.EqualTo("prior recovery"));
        }

        [Test]
        public void ResearchVersionFiveMigrationDefaultsToZeroWithoutChangingAnOldReceipt()
        {
            Assert.That(Settle("old-receipt", out _, out var error), Is.True, error);
            var json = JObject.Parse(storage.GetString(Helper_ProfileSession.StoreKey));
            json["version"] = 5;
            ((JObject)json["profiles"][0]).Remove("conversionResearchLevel");
            storage.SetString(Helper_ProfileSession.StoreKey, json.ToString());
            session.Reload();
            Assert.That(session.Current.ConversionResearchLevel, Is.Zero);
            Assert.That(session.Current.SettlementReceipts.Single().ConversionResearchLevel, Is.EqualTo(1));
            AssertPaid(session, 18, 40, 1);
            Assert.That(session.TrySettleFieldDataJourney(profileId, "old-receipt", FieldDataJourneyFlowState.Succeeded,
                hare, 9999, out var prior, out error), Is.True, error);
            Assert.That(prior.SpeciesDataDeposit, Is.EqualTo(11));
            AssertPaid(session, 18, 40, 1);
        }

        [Test]
        public void ResearchProcessReloadCheckpoint()
        {
            var phase = Environment.GetEnvironmentVariable("FD2_RESEARCH_RELOAD_PHASE");
            var path = Environment.GetEnvironmentVariable("FD2_RESEARCH_RELOAD_PATH");
            if (phase == "read")
            {
                storage.SetString(Helper_ProfileSession.StoreKey, File.ReadAllText(path));
                storage.Save();
                session.Reload();
                profileId = session.Current.ProfileId;
                Assert.That(session.Current.ConversionResearchLevel, Is.EqualTo(1));
                Assert.That(session.Current.ResearchData, Is.EqualTo(50));
                var saves = storage.SaveCalls;
                Assert.That(session.TryPurchaseConversionResearch(profileId, 0, out _), Is.False);
                Assert.That(storage.SaveCalls, Is.EqualTo(saves));
                Assert.That(session.TryPurchaseConversionResearch(profileId, 1, out var error), Is.True, error);
                Assert.That(session.Current.ConversionResearchLevel, Is.EqualTo(2));
                Assert.That(session.Current.ResearchData, Is.EqualTo(30));
                return;
            }
            StartResearch(60);
            Assert.That(session.TryPurchaseConversionResearch(profileId, 0, out var writeError), Is.True, writeError);
            if (phase == "write") File.WriteAllText(path, storage.Durable[Helper_ProfileSession.StoreKey]);
            var reopened = NewSession(storage.Restart());
            Assert.That(reopened.Current.ConversionResearchLevel, Is.EqualTo(1));
            Assert.That(reopened.Current.ResearchData, Is.EqualTo(50));
        }

        void StartLicense(long hareData, long foxData = 400, bool includeHareWallet = true)
        {
            StartResearch(30, 1);
            var json = JObject.Parse(storage.GetString(Helper_ProfileSession.StoreKey));
            var wallets = (JArray)json["profiles"][0]["speciesDataWallets"];
            wallets.Clear();
            if (includeHareWallet) wallets.Add(new JObject { ["speciesId"] = "hare", ["balance"] = hareData });
            wallets.Add(new JObject { ["speciesId"] = "fox", ["balance"] = foxData });
            storage.SetString(Helper_ProfileSession.StoreKey, json.ToString());
            storage.Save();
            session.Reload();
            session.UnlockGenome(new SpeciesId("fox"));
        }

        [TestCase(0L, false, 0L)]
        [TestCase(9L, false, 9L)]
        [TestCase(10L, true, 0L)]
        [TestCase(11L, true, 1L)]
        [TestCase(long.MaxValue, true, long.MaxValue - 10)]
        public void HareLicenseChargesExactlyTenOnceAndGrantsAccessWithoutNodeEffects(long before, bool succeeds, long after)
        {
            StartLicense(before);
            var saves = storage.SaveCalls;
            var fingerprint = session.Current.CreateGenomeSnapshot().Fingerprint;
            Assert.That(session.Current.GeneLabUnlocked, Is.False);
            Assert.That(session.TryPurchaseHareGenomeLicense(profileId, out var error), Is.EqualTo(succeeds), error);
            Assert.That(session.Current.UnlockedGenomeSpeciesIds.Contains(hare), Is.EqualTo(succeeds));
            Assert.That(session.Current.UnlockedGenomeSpeciesIds.Contains(new SpeciesId("fox")), Is.True);
            Assert.That(session.Current.RabbitData, Is.EqualTo(after));
            Assert.That(session.Current.SpeciesDataWallets[new SpeciesId("fox")], Is.EqualTo(400));
            Assert.That(session.Current.ResearchData, Is.EqualTo(30));
            Assert.That(session.Current.ConversionResearchLevel, Is.EqualTo(1));
            Assert.That(session.Current.CreateGenomeSnapshot().Fingerprint, Is.EqualTo(fingerprint));
            Assert.That(session.Current.GenomeProfiles, Is.Empty);
            Assert.That(session.Current.CompletedRuns, Is.Zero);
            Assert.That(storage.SaveCalls - saves, Is.EqualTo(succeeds ? 1 : 0));
            if (succeeds)
            {
                var saved = JObject.Parse(storage.Writes.Last())["profiles"][0];
                Assert.That(saved["unlockedGenomeSpeciesIds"].Values<string>(), Does.Contain("hare"));
                Assert.That((long)saved["speciesDataWallets"][0]["balance"], Is.EqualTo(after));
                saves = storage.SaveCalls;
                Assert.That(session.TryPurchaseHareGenomeLicense(profileId, out _), Is.False);
                Assert.That(storage.SaveCalls, Is.EqualTo(saves));
                var reopened = NewSession(storage.Restart());
                Assert.That(reopened.Current.UnlockedGenomeSpeciesIds, Does.Contain(hare));
                Assert.That(reopened.Current.RabbitData, Is.EqualTo(after));
                Assert.That(reopened.Current.GenomeProfiles, Is.Empty);
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public void HareLicenseCannotBeFundedByOtherSpeciesOrResearchData(bool includeHareWallet)
        {
            StartLicense(0, long.MaxValue, includeHareWallet);
            session.AddResearchData(long.MaxValue - session.Current.ResearchData);
            var original = storage.GetString(Helper_ProfileSession.StoreKey);
            var saves = storage.SaveCalls;
            Assert.That(session.TryPurchaseHareGenomeLicense(profileId, out var error), Is.False);
            StringAssert.Contains("Hare Data", error);
            Assert.That(session.Current.UnlockedGenomeSpeciesIds, Has.No.Member(hare));
            Assert.That(session.Current.SpeciesDataWallets[new SpeciesId("fox")], Is.EqualTo(long.MaxValue));
            Assert.That(session.Current.ResearchData, Is.EqualTo(long.MaxValue));
            Assert.That(storage.GetString(Helper_ProfileSession.StoreKey), Is.EqualTo(original));
            Assert.That(storage.SaveCalls, Is.EqualTo(saves));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void HareLicenseFailurePublishesNoUnlockAndRetryChargesOnce(bool persistThenThrow)
        {
            StartLicense(10);
            storage.FailOnSave = storage.SaveCalls + 1;
            storage.PersistThenThrow = persistThenThrow;
            var notified = false;
            session.SnapshotChanged += _ => notified = true;
            Assert.That(session.TryPurchaseHareGenomeLicense(profileId, out var error), Is.False);
            StringAssert.Contains("could not be saved", error);
            Assert.That(notified, Is.False);
            Assert.That(session.Current.UnlockedGenomeSpeciesIds, Has.No.Member(hare));
            Assert.That(session.Current.RabbitData, Is.EqualTo(10));
            storage.FailOnSave = -1;
            Assert.That(session.TryPurchaseHareGenomeLicense(profileId, out error), Is.True, error);
            Assert.That(session.Current.UnlockedGenomeSpeciesIds, Does.Contain(hare));
            Assert.That(session.Current.RabbitData, Is.Zero);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void HareLicenseRestartAfterUncertainSaveCannotPayTwice(bool persisted)
        {
            StartLicense(10);
            storage.FailOnSave = storage.SaveCalls + 1;
            storage.PersistThenThrow = persisted;
            Assert.That(session.TryPurchaseHareGenomeLicense(profileId, out _), Is.False);
            var rebootStorage = storage.Restart();
            var reboot = NewSession(rebootStorage);
            Assert.That(reboot.TryPurchaseHareGenomeLicense(profileId, out _), Is.EqualTo(!persisted));
            Assert.That(reboot.Current.UnlockedGenomeSpeciesIds, Does.Contain(hare));
            Assert.That(reboot.Current.RabbitData, Is.Zero);
            var saves = rebootStorage.SaveCalls;
            Assert.That(reboot.TryPurchaseHareGenomeLicense(profileId, out _), Is.False);
            Assert.That(rebootStorage.SaveCalls, Is.EqualTo(saves));
        }

        [Test]
        public void HareLicenseRequiresItsLoadedProfileAndRejectsReentry()
        {
            StartLicense(10);
            var originalId = profileId;
            var otherId = session.CreateProfile("Other license owner").ProfileId;
            var saves = storage.SaveCalls;
            Assert.That(session.TryPurchaseHareGenomeLicense(originalId, out _), Is.False);
            Assert.That(storage.SaveCalls, Is.EqualTo(saves));
            session.SelectProfile(originalId);
            var reentered = true;
            session.SnapshotChanged += snapshot => reentered = session.TryPurchaseHareGenomeLicense(originalId, out _);
            Assert.That(session.TryPurchaseHareGenomeLicense(originalId, out var error), Is.True, error);
            Assert.That(reentered, Is.False);
            session.SelectProfile(otherId);
            Assert.That(session.Current.UnlockedGenomeSpeciesIds, Has.No.Member(hare));
            Assert.That(session.Current.RabbitData, Is.Zero);
        }

        [Test]
        public void HareLicenseRecognizesLegacyOwnershipWithoutChargingAgain()
        {
            StartLicense(10);
            session.UnlockGenome(hare);
            var saves = storage.SaveCalls;
            Assert.That(session.TryPurchaseHareGenomeLicense(profileId, out _), Is.False);
            Assert.That(session.Current.RabbitData, Is.EqualTo(10));
            Assert.That(storage.SaveCalls, Is.EqualTo(saves));
        }

        [Test]
        public void HareLicensePreservesExistingGenomesResearchReceiptsAndPendingSuccess()
        {
            StartLicense(20);
            Assert.That(session.TrySetGenomeConfiguration(hare, new[] { "hare.guarded-burrow" },
                new[] { "hare.guarded-burrow" }, out var error), Is.True, error);
            Assert.That(Settle("history-before-license", out var history, out error), Is.True, error);
            storage.FailOnSave = storage.SaveCalls + 2;
            Assert.That(Settle("pending-before-license", out _, out _), Is.False);
            storage.FailOnSave = -1;
            var before = session.Current.CreateGenomeSnapshot().Fingerprint;
            Assert.That(session.TryPurchaseHareGenomeLicense(profileId, out error), Is.True, error);
            Assert.That(session.Current.RabbitData, Is.EqualTo(21));
            Assert.That(session.Current.ResearchData, Is.EqualTo(40));
            Assert.That(session.Current.ConversionResearchLevel, Is.EqualTo(1));
            Assert.That(session.Current.SettlementReceipts.Single(), Is.SameAs(history));
            Assert.That(session.Current.PendingSettlement.JourneyId, Is.EqualTo("pending-before-license"));
            Assert.That(session.Current.CreateGenomeSnapshot().Fingerprint, Is.EqualTo(before));
            session.Reload();
            Assert.That(session.Current.UnlockedGenomeSpeciesIds, Does.Contain(hare));
            Assert.That(session.Current.CreateGenomeSnapshot().Fingerprint, Is.EqualTo(before));
            Assert.That(session.TryRetryPendingFieldDataSettlement(profileId, out _, out error), Is.True, error);
            Assert.That(session.Current.RabbitData, Is.EqualTo(32));
            Assert.That(session.Current.ResearchData, Is.EqualTo(50));
            Assert.That(session.Current.SpeciesDataWallets[new SpeciesId("fox")], Is.EqualTo(400));
        }

        [Test]
        public void HareLicenseCanUseDataFromTheFirstSuccessfulJourneyWithoutTheLegacyThreeRunGate()
        {
            profileId = session.CreateProfile("Fresh first journey").ProfileId;
            Assert.That(session.Current.RabbitData, Is.Zero);
            Assert.That(session.Current.ResearchData, Is.Zero);
            Assert.That(session.Current.ConversionResearchLevel, Is.Zero);
            Assert.That(session.TrySettleFieldDataJourney(profileId, "first-success", FieldDataJourneyFlowState.Succeeded,
                hare, 100, out _, out var error), Is.True, error);
            Assert.That(session.Current.CompletedRuns, Is.EqualTo(1));
            Assert.That(session.Current.GeneLabUnlocked, Is.False);
            Assert.That(session.Current.RabbitData, Is.EqualTo(10));
            Assert.That(session.TryPurchaseHareGenomeLicense(profileId, out error), Is.True, error);
            Assert.That(session.Current.RabbitData, Is.Zero);
            Assert.That(session.Current.ResearchData, Is.EqualTo(10));
            Assert.That(session.Current.GenomeProfiles, Is.Empty);
        }

        [Test]
        public void HareLicenseProcessReloadCheckpoint()
        {
            var phase = Environment.GetEnvironmentVariable("FD2_LICENSE_RELOAD_PHASE");
            var path = Environment.GetEnvironmentVariable("FD2_LICENSE_RELOAD_PATH");
            if (phase == "read")
            {
                storage.SetString(Helper_ProfileSession.StoreKey, File.ReadAllText(path));
                storage.Save();
                session.Reload();
                profileId = session.Current.ProfileId;
                Assert.That(session.Current.UnlockedGenomeSpeciesIds, Does.Contain(hare));
                Assert.That(session.Current.RabbitData, Is.Zero);
                var saves = storage.SaveCalls;
                Assert.That(session.TryPurchaseHareGenomeLicense(profileId, out _), Is.False);
                Assert.That(storage.SaveCalls, Is.EqualTo(saves));
                Assert.That(session.Current.GenomeProfiles, Is.Empty);
                return;
            }
            StartLicense(10);
            Assert.That(session.TryPurchaseHareGenomeLicense(profileId, out var error), Is.True, error);
            if (phase == "write") File.WriteAllText(path, storage.Durable[Helper_ProfileSession.StoreKey]);
            var reopened = NewSession(storage.Restart());
            Assert.That(reopened.Current.UnlockedGenomeSpeciesIds, Does.Contain(hare));
            Assert.That(reopened.Current.RabbitData, Is.Zero);
        }

        void StartNodes(long balance = 40)
        {
            StartLicense(balance);
            session.UnlockGenome(hare);
        }

        [TestCase(0L, false)]
        [TestCase(4L, false)]
        [TestCase(5L, true)]
        [TestCase(6L, true)]
        [TestCase(long.MaxValue, true)]
        public void HareNodePurchaseChargesFiveAndPersistsPermanentOwnershipWithActivation(long balance, bool succeeds)
        {
            StartNodes(balance);
            var saves = storage.SaveCalls;
            var before = session.Current;
            Assert.That(session.TryPurchaseHareGenomeNode(profileId, "hare.guarded-burrow", out var error), Is.EqualTo(succeeds), error);
            if (!succeeds)
            {
                Assert.That(session.Current, Is.SameAs(before));
                Assert.That(storage.SaveCalls, Is.EqualTo(saves));
                return;
            }
            Assert.That(storage.SaveCalls, Is.EqualTo(saves + 1));
            var paid = JObject.Parse(storage.Writes.Last())["profiles"][0];
            Assert.That((long)paid["speciesDataWallets"].Single(wallet => (string)wallet["speciesId"] == "hare")["balance"], Is.EqualTo(balance - 5));
            Assert.That(paid["genomeProfiles"][0]["unlockedNodeIds"].Values<string>(), Is.EqualTo(new[] { "hare.guarded-burrow" }));
            Assert.That(paid["genomeProfiles"][0]["activeNodeIds"].Values<string>(), Is.EqualTo(new[] { "hare.guarded-burrow" }));
            Assert.That(paid["genomeProfiles"][0]["allocations"], Is.Empty);
            var reopened = NewSession(storage.Restart());
            Assert.That(reopened.Current.RabbitData, Is.EqualTo(balance - 5));
            Assert.That(reopened.Current.TryGetGenomeProfile(hare, out var genome), Is.True);
            Assert.That(genome.IsNodeUnlocked("hare.guarded-burrow"), Is.True);
            Assert.That(genome.IsNodeActive("hare.guarded-burrow"), Is.True);
            Assert.That(genome.Allocations, Is.Empty);
            Assert.That(reopened.Current.SpeciesDataWallets[new SpeciesId("fox")], Is.EqualTo(400));
            Assert.That(reopened.Current.ResearchData, Is.EqualTo(30));
            Assert.That(reopened.Current.ConversionResearchLevel, Is.EqualTo(1));
        }

        [TestCase("hare.guarded-burrow", SpeciesAttributeIds.BlockAmount, 1f)]
        [TestCase("hare.keen-hearing", SpeciesAttributeIds.VisionRange, 2f)]
        [TestCase("hare.swift-digging", SpeciesAttributeIds.MovementSpeed, 0.25f)]
        [TestCase("hare.safe-foraging", SpeciesAttributeIds.DigestionEnergyBonus, 0.25f)]
        [TestCase("hare.warren-network", SpeciesAttributeIds.PopulationPoolCapacity, 10f)]
        public void HareNodePurchaseUsesOwnedInactivePrerequisitesAndExactCatalogEffects(string id, string attribute, float amount)
        {
            StartNodes(5);
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            var owned = definition.HareMap.Nodes.Select(node => node.Id).Where(nodeId => nodeId != id).ToArray();
            Assert.That(session.TrySetGenomeConfiguration(hare, owned, Array.Empty<string>(), out var error), Is.True, error);
            Assert.That(session.TryPurchaseHareGenomeNode(profileId, id, out error), Is.True, error);
            session.Reload();
            session.Current.TryGetGenomeProfile(hare, out var genome);
            Assert.That(genome.UnlockedNodeIds.Count, Is.EqualTo(5));
            Assert.That(genome.ActiveNodeIds, Is.EqualTo(new[] { id }));
            var frozen = definition.FreezeOwnedLoadout(genome).GetSpeciesGenome(hare);
            Assert.That(frozen.CatalogFingerprint, Is.EqualTo(definition.Catalog.Fingerprint));
            Assert.That(frozen.ResolvedNodes.Single().Fingerprint, Is.EqualTo(definition.HareMap.GetNode(id).Fingerprint));
            Assert.That(frozen.ResolvedNodes.Single().Modifiers.Single().AttributeId, Is.EqualTo(attribute));
            Assert.That(frozen.ResolvedNodes.Single().Modifiers.Single().SignedValue, Is.EqualTo(amount));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("fox.guarded-burrow")]
        [TestCase("hare.unknown")]
        public void HareNodePurchaseRejectsUnknownAndOtherSpeciesIdsWithoutWriting(string id)
        {
            StartNodes();
            var before = session.Current;
            var saves = storage.SaveCalls;
            Assert.That(session.TryPurchaseHareGenomeNode(profileId, id, out var error), Is.False);
            Assert.That(error, Is.Not.Empty);
            Assert.That(session.Current, Is.SameAs(before));
            Assert.That(storage.SaveCalls, Is.EqualTo(saves));
        }

        [TestCase("hare.keen-hearing", "")]
        [TestCase("hare.safe-foraging", "hare.guarded-burrow,hare.keen-hearing")]
        [TestCase("hare.warren-network", "hare.guarded-burrow,hare.safe-foraging")]
        public void HareNodePurchaseRequiresAllOwnedAncestors(string id, string ownedIds)
        {
            StartNodes();
            Assert.That(session.TrySetGenomeConfiguration(hare, ownedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                Array.Empty<string>(), out var error), Is.True, error);
            var before = session.Current;
            var saves = storage.SaveCalls;
            Assert.That(session.TryPurchaseHareGenomeNode(profileId, id, out error), Is.False);
            StringAssert.Contains("prerequisite", error);
            Assert.That(session.Current, Is.SameAs(before));
            Assert.That(storage.SaveCalls, Is.EqualTo(saves));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void HareNodePurchaseNeverSpendsOtherWalletsOrResearchData(bool includeEmptyHareWallet)
        {
            StartLicense(0, long.MaxValue, includeEmptyHareWallet);
            session.UnlockGenome(hare);
            session.AddResearchData(long.MaxValue - session.Current.ResearchData);
            var before = session.Current;
            var saves = storage.SaveCalls;
            Assert.That(session.TryPurchaseHareGenomeNode(profileId, "hare.guarded-burrow", out _), Is.False);
            Assert.That(session.Current, Is.SameAs(before));
            Assert.That(storage.SaveCalls, Is.EqualTo(saves));
        }

        [Test]
        public void HareNodePurchaseNeedsLicenseAndMatchingLoadedProfileAndRejectsNotificationReentry()
        {
            StartLicense(40);
            var before = session.Current;
            var saves = storage.SaveCalls;
            Assert.That(session.TryPurchaseHareGenomeNode(profileId, "hare.guarded-burrow", out _), Is.False);
            Assert.That(session.Current, Is.SameAs(before));
            Assert.That(storage.SaveCalls, Is.EqualTo(saves));
            session.UnlockGenome(hare);
            saves = storage.SaveCalls;
            Assert.That(session.TryPurchaseHareGenomeNode("other-profile", "hare.guarded-burrow", out _), Is.False);
            Assert.That(NewSession(new Storage()).TryPurchaseHareGenomeNode(profileId, "hare.guarded-burrow", out _), Is.False);
            Assert.That(storage.SaveCalls, Is.EqualTo(saves));
            var notifications = 0;
            session.SnapshotChanged += snapshot =>
            {
                notifications++;
                Assert.That(session.TryPurchaseHareGenomeNode(profileId, "hare.keen-hearing", out _), Is.False);
            };
            Assert.That(session.TryPurchaseHareGenomeNode(profileId, "hare.guarded-burrow", out var error), Is.True, error);
            Assert.That(notifications, Is.EqualTo(1));
            Assert.That(storage.SaveCalls, Is.EqualTo(saves + 1));
            Assert.That(session.Current.RabbitData, Is.EqualTo(35));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void HareNodePurchaseRejectsOwnedDuplicatesWhetherActiveOrInactive(bool deactivate)
        {
            StartNodes();
            Assert.That(session.TryPurchaseHareGenomeNode(profileId, "hare.guarded-burrow", out var error), Is.True, error);
            if (deactivate)
                Assert.That(session.TrySetGenomeConfiguration(hare, new[] { "hare.guarded-burrow" }, Array.Empty<string>(), out error), Is.True, error);
            var before = session.Current;
            var saves = storage.SaveCalls;
            Assert.That(session.TryPurchaseHareGenomeNode(profileId, "hare.guarded-burrow", out _), Is.False);
            Assert.That(session.Current, Is.SameAs(before));
            Assert.That(storage.SaveCalls, Is.EqualTo(saves));
        }

        [Test]
        public void HareNodePurchaseAllowsExactlyEightCatalogPointsAndRejectsTheFifthWithoutCharging()
        {
            StartNodes();
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            var ids = definition.HareMap.Nodes.Select(node => node.Id).ToArray();
            foreach (var id in ids.Take(4))
                Assert.That(session.TryPurchaseHareGenomeNode(profileId, id, out var error), Is.True, error);
            session.Current.TryGetGenomeProfile(hare, out var genome);
            Assert.That(genome.ActiveNodeIds, Is.EqualTo(ids.Take(4)));
            Assert.That(genome.UnlockedNodeIds, Is.EqualTo(ids.Take(4)));
            Assert.That(genome.ActiveNodeIds.Sum(id => definition.HareMap.GetNode(id).CapacityPoints), Is.EqualTo(8));
            var before = session.Current;
            var saves = storage.SaveCalls;
            Assert.That(session.TryPurchaseHareGenomeNode(profileId, ids[4], out var reason), Is.False);
            StringAssert.Contains("capacity", reason);
            Assert.That(session.Current, Is.SameAs(before));
            Assert.That(storage.SaveCalls, Is.EqualTo(saves));
            Assert.That(session.Current.RabbitData, Is.EqualTo(20));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void HareNodeSaveFailurePublishesNoPurchaseAndRetryChargesOnce(bool persistThenThrow)
        {
            StartNodes(5);
            var before = session.Current;
            var notifications = 0;
            session.SnapshotChanged += snapshot => notifications++;
            storage.FailOnSave = storage.SaveCalls + 1;
            storage.PersistThenThrow = persistThenThrow;
            Assert.That(session.TryPurchaseHareGenomeNode(profileId, "hare.guarded-burrow", out var error), Is.False);
            StringAssert.Contains("saved", error);
            Assert.That(session.Current, Is.SameAs(before));
            Assert.That(notifications, Is.Zero);
            storage.FailOnSave = -1;
            Assert.That(session.TryPurchaseHareGenomeNode(profileId, "hare.guarded-burrow", out error), Is.True, error);
            Assert.That(session.Current.RabbitData, Is.Zero);
            Assert.That(notifications, Is.EqualTo(1));
            Assert.That(session.TryPurchaseHareGenomeNode(profileId, "hare.guarded-burrow", out _), Is.False);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void HareNodeRestartAfterUncertainSaveChargesOnlyOnce(bool persisted)
        {
            StartNodes(5);
            storage.FailOnSave = storage.SaveCalls + 1;
            storage.PersistThenThrow = persisted;
            Assert.That(session.TryPurchaseHareGenomeNode(profileId, "hare.guarded-burrow", out _), Is.False);
            var reboot = NewSession(storage.Restart());
            Assert.That(reboot.TryPurchaseHareGenomeNode(profileId, "hare.guarded-burrow", out var error), Is.EqualTo(!persisted), error);
            Assert.That(reboot.Current.RabbitData, Is.Zero);
            reboot.Current.TryGetGenomeProfile(hare, out var genome);
            Assert.That(genome.IsNodeUnlocked("hare.guarded-burrow"), Is.True);
            Assert.That(genome.IsNodeActive("hare.guarded-burrow"), Is.True);
        }

        [Test]
        public void HareNodePurchasePreservesOtherGenomesResearchHistoryAndPendingSuccess()
        {
            StartNodes();
            var fox = new SpeciesId("fox");
            Assert.That(session.TrySetGenomeConfiguration(fox, new[] { "fox.legacy" }, new[] { "fox.legacy" }, out var error), Is.True, error);
            Assert.That(Settle("node-history", out var receipt, out error), Is.True, error);
            storage.FailOnSave = storage.SaveCalls + 2;
            Assert.That(Settle("node-pending", out _, out _), Is.False);
            storage.FailOnSave = -1;
            var before = session.Current;
            Assert.That(session.TryPurchaseHareGenomeNode(profileId, "hare.guarded-burrow", out error), Is.True, error);
            Assert.That(session.Current.RabbitData, Is.EqualTo(before.RabbitData - 5));
            Assert.That(session.Current.ResearchData, Is.EqualTo(before.ResearchData));
            Assert.That(session.Current.ConversionResearchLevel, Is.EqualTo(before.ConversionResearchLevel));
            Assert.That(session.Current.CompletedRuns, Is.EqualTo(before.CompletedRuns));
            Assert.That(session.Current.SettlementReceipts.Single(), Is.SameAs(receipt));
            Assert.That(session.Current.PendingSettlement, Is.SameAs(before.PendingSettlement));
            Assert.That(session.Current.SpeciesDataWallets[fox], Is.EqualTo(400));
            session.Reload();
            Assert.That(session.Current.TryGetGenomeProfile(fox, out var foxGenome), Is.True);
            Assert.That(foxGenome.ActiveNodeIds, Is.EqualTo(new[] { "fox.legacy" }));
            Assert.That(session.Current.PendingSettlement.JourneyId, Is.EqualTo("node-pending"));
            Assert.That(session.TryRetryPendingFieldDataSettlement(profileId, out _, out error), Is.True, error);
            Assert.That(session.Current.RabbitData, Is.EqualTo(before.RabbitData - 5 + 11));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void HareNodePurchasePreservesAndRejectsIncompatibleLegacyActiveState(bool refundableAllocation)
        {
            StartNodes();
            var json = JObject.Parse(storage.GetString(Helper_ProfileSession.StoreKey));
            var id = refundableAllocation ? "hare.guarded-burrow" : "hare.unknown-legacy";
            json["profiles"][0]["genomeProfiles"] = new JArray(new JObject
            {
                ["speciesId"] = "hare",
                ["unlockedNodeIds"] = refundableAllocation ? new JArray() : new JArray(id),
                ["activeNodeIds"] = new JArray(id),
                ["allocations"] = refundableAllocation ? new JArray(new JObject
                {
                    ["nodeId"] = id, ["rabbitDataSpent"] = 1, ["capacityPoints"] = 2,
                }) : new JArray(),
            });
            storage.SetString(Helper_ProfileSession.StoreKey, json.ToString());
            storage.Save();
            session.Reload();
            var before = session.Current;
            var saves = storage.SaveCalls;
            Assert.That(session.TryPurchaseHareGenomeNode(profileId, "hare.guarded-burrow", out var error), Is.False);
            Assert.That(error, Is.Not.Empty);
            Assert.That(session.Current, Is.SameAs(before));
            Assert.That(storage.SaveCalls, Is.EqualTo(saves));
        }

        [Test]
        public void HareNodeProcessReloadCheckpoint()
        {
            var phase = Environment.GetEnvironmentVariable("FD2_NODE_RELOAD_PHASE");
            var path = Environment.GetEnvironmentVariable("FD2_NODE_RELOAD_PATH");
            if (phase == "read")
            {
                storage.SetString(Helper_ProfileSession.StoreKey, File.ReadAllText(path));
                storage.Save();
                session.Reload();
                profileId = session.Current.ProfileId;
            }
            else
            {
                StartNodes(10);
                Assert.That(session.TryPurchaseHareGenomeNode(profileId, "hare.guarded-burrow", out var error), Is.True, error);
                Assert.That(session.TryPurchaseHareGenomeNode(profileId, "hare.keen-hearing", out error), Is.True, error);
                if (phase == "write") File.WriteAllText(path, storage.Durable[Helper_ProfileSession.StoreKey]);
            }
            var reopened = NewSession(storage.Restart());
            Assert.That(reopened.Current.RabbitData, Is.Zero);
            reopened.Current.TryGetGenomeProfile(hare, out var genome);
            Assert.That(genome.UnlockedNodeIds, Is.EqualTo(new[] { "hare.guarded-burrow", "hare.keen-hearing" }));
            Assert.That(genome.ActiveNodeIds, Is.EqualTo(genome.UnlockedNodeIds));
            var writes = storage.SaveCalls;
            Assert.That(session.TryPurchaseHareGenomeNode(profileId, "hare.keen-hearing", out _), Is.False);
            Assert.That(storage.SaveCalls, Is.EqualTo(writes));
        }

        static void AssertPaid(Helper_ProfileSession target, long speciesData, long researchData, int receiptCount)
        {
            Assert.That(target.Current.RabbitData, Is.EqualTo(speciesData));
            Assert.That(target.Current.ResearchData, Is.EqualTo(researchData));
            Assert.That(target.Current.SettlementReceipts.Count, Is.EqualTo(receiptCount));
        }
    }
}
