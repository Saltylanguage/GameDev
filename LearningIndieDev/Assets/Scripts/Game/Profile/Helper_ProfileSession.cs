using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace SaltyGame
{
    public sealed class Helper_ProfileSession : MonoBehaviour
    {
        public const string StoreKey = "SaltyGame.ProfileSession";
        public const string RecoveryStoreKey = StoreKey + ".Recovery";
        const int StoreVersion = 7;
        const double AutosaveSeconds = 30d;

        [SerializeField] bool trackPlaytime = true;

        [Serializable]
        sealed class GenomeProfileRecord
        {
            public string speciesId;
            public List<string> unlockedNodeIds = new List<string>();
            public List<string> activeNodeIds = new List<string>();
            public List<GenomeAllocationRecord> allocations = new List<GenomeAllocationRecord>();
            public long? resetGeneration;
        }

        [Serializable]
        sealed class GenomeAllocationRecord
        {
            public string nodeId;
            public long rabbitDataSpent;
            public int capacityPoints;
        }

        [Serializable]
        sealed class SpeciesDataWalletRecord
        {
            public string speciesId;
            public long balance;
        }

        [Serializable]
        sealed class SettlementRecord
        {
            public string journeyId;
            public string speciesId;
            public long leftoverFieldData;
            public int researchLevel;
            public string definitionVersion;
            public int fieldDataPerSpeciesData;
            public long multiplierNumerator;
            public long multiplierDenominator;
            public long baseSpeciesData;
            public long speciesDataDeposit;
            public long researchDataBonus;
            public long speciesDataBefore;
            public long speciesDataAfter;
            public long researchDataBefore;
            public long researchDataAfter;
        }

        [Serializable]
        sealed class PendingSettlementRecord
        {
            public string journeyId;
            public string speciesId;
            public long leftoverFieldData;
            public int researchLevel;
        }

        [Serializable]
        sealed class ProfileRecord
        {
            public string id;
            public string name;
            public List<GenomeProfileRecord> genomeProfiles = new List<GenomeProfileRecord>();
            public long currency;
            public long rabbitData;
            public long researchData;
            public int? conversionResearchLevel;
            // Null distinguishes incoming Rabbit-v3 saves from local species-wallet-v3 saves.
            public List<SpeciesDataWalletRecord> speciesDataWallets;
            public int completedRuns;
            public List<string> completedRunIds = new List<string>();
            public long simulationTicks;
            public long playTimeMilliseconds;
            public List<string> unlockedSpeciesIds = new List<string>();
            public List<string> unlockedGenomeSpeciesIds = new List<string>();
            public List<string> unlockedFeatureIds = new List<string>();
            public List<SettlementRecord> settlementReceipts;
            public PendingSettlementRecord pendingSettlement;
        }

        [Serializable]
        sealed class ProfileStore
        {
            public int version;
            public List<ProfileRecord> profiles = new List<ProfileRecord>();
            public string lastLoadedProfileId;
            public string migrationSourceJson;
        }

        readonly List<ProfileSessionSnapshot> profiles = new List<ProfileSessionSnapshot>();
        long pendingSimulationTicks;
        double pendingPlayTimeMilliseconds;
        double autosaveElapsedSeconds;
        bool applicationFocused = true;
        bool applicationPaused;
        string migrationSourceJson = string.Empty;
        IProfileSessionStorage storage = new PlayerPrefsProfileSessionStorage();
        bool committingProgression;

        public event Action<ProfileSessionSnapshot> SnapshotChanged;

        public IReadOnlyList<ProfileSessionSnapshot> Profiles => profiles;
        public ProfileSessionSnapshot Current { get; private set; } = ProfileSessionSnapshot.Empty;
        public bool HasProfiles => profiles.Count > 0;
        public string LoadWarning { get; private set; } = string.Empty;

        public void InitializeStorage(IProfileSessionStorage profileStorage)
        {
            if (profiles.Count != 0 || Current.HasLoadedProfile)
                throw new InvalidOperationException("Storage must be configured before loading profiles.");
            storage = profileStorage ?? throw new ArgumentNullException(nameof(profileStorage));
            Load();
        }

        void Awake()
        {
            Load();
        }

        public ProfileSessionSnapshot CreateInitialProfile(string displayName)
        {
            return CreateProfile(displayName);
        }

        public ProfileSessionSnapshot CreateProfile(string displayName)
        {
            Flush();
            UnloadCurrentProfile();

            var normalizedName = string.IsNullOrWhiteSpace(displayName)
                ? "Researcher 01"
                : displayName.Trim();
            var profile = new ProfileSessionSnapshot(
                true,
                Guid.NewGuid().ToString("N"),
                normalizedName,
                unlockedSpeciesIds: new[] { new SpeciesId("hare") });

            profiles.Add(profile);
            Current = profile;
            Save();
            NotifyChanged();
            return profile;
        }

        public bool SelectProfile(string profileId)
        {
            if (string.IsNullOrEmpty(profileId))
            {
                return false;
            }

            for (var index = 0; index < profiles.Count; index++)
            {
                var profile = profiles[index];
                if (profile.ProfileId != profileId)
                {
                    continue;
                }

                Flush();
                UnloadCurrentProfile();
                Current = profiles[index].WithLoaded(true);
                profiles[index] = Current;
                Save();
                NotifyChanged();
                return true;
            }

            return false;
        }

        public void Reload()
        {
            Flush();
            if (Current.PendingSettlement != null)
            {
                try { Save(); }
                catch (Exception exception)
                {
                    LoadWarning = "Pending success is retained in memory: " + exception.Message;
                    return;
                }
            }
            Load();
        }

        void Update()
        {
            var elapsedSeconds = Math.Max(0d, Time.unscaledDeltaTime);
            if (trackPlaytime && Current.HasLoadedProfile && applicationFocused && !applicationPaused)
            {
                pendingPlayTimeMilliseconds += elapsedSeconds * 1000d;
            }

            autosaveElapsedSeconds += elapsedSeconds;
            if (autosaveElapsedSeconds >= AutosaveSeconds)
            {
                Flush();
            }
        }

        void OnApplicationFocus(bool focused)
        {
            applicationFocused = focused;
            if (!focused)
            {
                Flush();
            }
        }

        void OnApplicationPause(bool paused)
        {
            applicationPaused = paused;
            if (paused)
            {
                Flush();
            }
        }

        void OnDestroy()
        {
            Flush();
        }

        public void SetPlaytimeTracking(bool enabled)
        {
            if (!enabled)
            {
                Flush();
            }

            trackPlaytime = enabled;
        }

        public void RecordSimulationTicks(long ticks)
        {
            if (ticks < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(ticks));
            }

            if (Current.HasLoadedProfile)
            {
                pendingSimulationTicks = checked(pendingSimulationTicks + ticks);
            }
        }

        public void RecordPlayedMilliseconds(long milliseconds)
        {
            if (milliseconds < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(milliseconds));
            }

            if (Current.HasLoadedProfile)
            {
                pendingPlayTimeMilliseconds += milliseconds;
            }
        }

        public void Flush()
        {
            if (!Current.HasLoadedProfile)
            {
                pendingSimulationTicks = 0;
                pendingPlayTimeMilliseconds = 0;
                autosaveElapsedSeconds = 0;
                return;
            }

            var playTimeMilliseconds = (long)pendingPlayTimeMilliseconds;
            if (pendingSimulationTicks == 0 && playTimeMilliseconds == 0)
            {
                autosaveElapsedSeconds = 0;
                return;
            }

            var next = Current.WithTime(
                checked(Current.SimulationTicks + pendingSimulationTicks),
                checked(Current.PlayTimeMilliseconds + playTimeMilliseconds));
            pendingSimulationTicks = 0;
            pendingPlayTimeMilliseconds -= playTimeMilliseconds;
            autosaveElapsedSeconds = 0;
            CommitCurrent(next);
        }

        public void AddCurrency(long amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }

            RequireLoadedProfile();
            Flush();
            CommitCurrent(Current.WithCurrency(checked(Current.Currency + amount)));
        }

        public bool TrySpendCurrency(long amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }

            RequireLoadedProfile();
            Flush();
            if (Current.Currency < amount)
            {
                return false;
            }

            CommitCurrent(Current.WithCurrency(Current.Currency - amount));
            return true;
        }

        public void UnlockSpecies(SpeciesId speciesId)
        {
            RequireLoadedProfile();
            Flush();
            CommitCurrent(Current.WithUnlockedSpecies(speciesId));
        }

        public void UnlockGenome(SpeciesId speciesId)
        {
            RequireLoadedProfile();
            Flush();
            CommitCurrent(Current.WithUnlockedGenome(speciesId));
        }

        public void UnlockFeature(string featureId)
        {
            RequireLoadedProfile();
            Flush();
            CommitCurrent(Current.WithUnlockedFeature(featureId));
        }

        public void AddRabbitData(long amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
            RequireLoadedProfile();
            Flush();
            CommitCurrent(Current.WithGenomeEconomy(
                checked(Current.RabbitData + amount), Current.ResearchData, Current.CompletedRuns));
        }

        public void AddResearchData(long amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
            RequireLoadedProfile();
            Flush();
            CommitCurrent(Current.WithGenomeEconomy(
                Current.RabbitData, checked(Current.ResearchData + amount), Current.CompletedRuns));
        }

        // The journey/result flow supplies a stable completion id on settlement.
        public bool RecordCompletedRun(string completionId)
        {
            return RecordRunCompletion(completionId, 0, 0);
        }

        public bool RecordSuccessfulJourney(string completionId)
        {
            return RecordRunCompletion(
                completionId,
                GenomeContract.RabbitDataPerSuccessfulJourney,
                GenomeContract.ResearchDataPerSuccessfulJourney);
        }

        public bool TrySettleFieldDataJourney(string profileId, string journeyId,
            FieldDataJourneyFlowState state, SpeciesId selectedSpeciesId, long leftoverFieldData,
            out FieldDataSettlementReceipt receipt, out string error)
        {
            return TrySettleFieldDataJourney(profileId, journeyId, state, selectedSpeciesId,
                leftoverFieldData, Current.ConversionResearchLevel, out receipt, out error);
        }

        public bool TryPurchaseConversionResearch(string profileId, int expectedLevel, out string error)
        {
            error = null;
            if (committingProgression || !Current.HasLoadedProfile || Current.ProfileId != profileId
                || Current.ConversionResearchLevel != expectedLevel)
            {
                error = "Research purchase requires the same loaded profile and unchanged research level.";
                return false;
            }
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            if (expectedLevel >= definition.MaximumResearchLevel)
            {
                error = "Conversion research is already at its maximum level.";
                return false;
            }
            var price = definition.ConversionResearchPrices[expectedLevel];
            if (Current.ResearchData < price)
            {
                error = "Not enough Research Data for the next conversion level.";
                return false;
            }
            committingProgression = true;
            try
            {
                return TryCommitProfile(Current.WithConversionResearch(Current.ResearchData - price,
                    expectedLevel + 1), out error);
            }
            finally { committingProgression = false; }
        }

        public bool TrySettleFieldDataJourney(string profileId, string journeyId,
            FieldDataJourneyFlowState state, SpeciesId selectedSpeciesId, long leftoverFieldData,
            int researchLevel, out FieldDataSettlementReceipt receipt, out string error)
        {
            receipt = null;
            error = null;
            if (committingProgression || !Current.HasLoadedProfile || Current.ProfileId != profileId
                || string.IsNullOrWhiteSpace(journeyId))
            {
                error = "Settlement requires the journey's loaded profile, identity and no operation in progress.";
                return false;
            }

            if (Current.TryGetSettlement(journeyId, out receipt)) return true;
            if (state != FieldDataJourneyFlowState.Succeeded || Current.CompletedRunIds.Contains(journeyId))
            {
                error = "The journey must be successful and not previously paid by the legacy completion flow.";
                return false;
            }

            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            var pending = Current.PendingSettlement;
            if (pending != null && pending.JourneyId != journeyId)
            {
                error = "Resolve the profile's pending settlement before starting another.";
                return false;
            }

            committingProgression = true;
            try
            {
                if (pending == null)
                {
                    if (researchLevel != Current.ConversionResearchLevel)
                    {
                        error = "A new settlement must use the profile's current conversion research level.";
                        return false;
                    }
                    if (!FieldDataSettlement.TryCalculate(definition, journeyId, state, selectedSpeciesId,
                        leftoverFieldData, researchLevel, Current.SpeciesDataWallets, Current.ResearchData,
                        out _, out error)) return false;
                    pending = new PendingFieldDataSettlement(journeyId, selectedSpeciesId, leftoverFieldData, researchLevel);
                }
                var unpaid = Current.WithPendingSettlement(pending);
                if (!TryCommitSettlement(unpaid, out error))
                {
                    // Keep the frozen success in memory even if its first durable write failed.
                    ApplyCurrent(unpaid);
                    NotifyChanged();
                    return false;
                }

                if (Current.ProfileId != profileId || Current.PendingSettlement?.JourneyId != journeyId)
                {
                    error = "The loaded profile changed; retry on the journey's original profile.";
                    return false;
                }
                if (!FieldDataSettlement.TryCalculate(definition, pending.JourneyId, state,
                    pending.SelectedSpeciesId, pending.LeftoverFieldData, pending.ConversionResearchLevel,
                    Current.SpeciesDataWallets, Current.ResearchData, out var calculated, out error)) return false;
                var paid = Current.WithSettlement(calculated);
                if (!TryCommitSettlement(paid, out error)) return false;
                receipt = calculated;
                return true;
            }
            catch (OverflowException)
            {
                error = "The settlement exceeds the supported profile range.";
                return false;
            }
            finally
            {
                committingProgression = false;
            }
        }

        public bool TryRetryPendingFieldDataSettlement(string profileId,
            out FieldDataSettlementReceipt receipt, out string error)
        {
            var pending = Current.PendingSettlement;
            if (pending == null)
            {
                receipt = null;
                error = "There is no pending Field Data settlement.";
                return false;
            }
            return TrySettleFieldDataJourney(profileId, pending.JourneyId, FieldDataJourneyFlowState.Succeeded,
                pending.SelectedSpeciesId, pending.LeftoverFieldData, pending.ConversionResearchLevel, out receipt, out error);
        }

        bool TryCommitSettlement(ProfileSessionSnapshot next, out string error)
        {
            if (TryCommitProfile(next, out error)) return true;
            error = "Settlement is pending: " + error;
            return false;
        }

        bool TryCommitProfile(ProfileSessionSnapshot next, out string error)
        {
            try
            {
                WriteStore(SerializeStore(next));
            }
            catch (Exception exception)
            {
                error = "The profile could not be saved: " + exception.Message;
                return false;
            }
            ApplyCurrent(next);
            NotifyChanged();
            error = null;
            return true;
        }

        bool RecordRunCompletion(string completionId, long rabbitDataReward, long researchDataReward)
        {
            if (string.IsNullOrWhiteSpace(completionId))
                throw new ArgumentException("A completed run id is required.", nameof(completionId));
            RequireLoadedProfile();
            var normalizedId = completionId.Trim();
            if (Current.CompletedRunIds.Contains(normalizedId))
                return false;

            Flush();
            var completedRunIds = new List<string>(Current.CompletedRunIds) { normalizedId };
            CommitCurrent(Current.WithGenomeEconomy(
                checked(Current.RabbitData + rabbitDataReward),
                checked(Current.ResearchData + researchDataReward),
                checked(Current.CompletedRuns + 1),
                nextCompletedRunIds: completedRunIds));
            return true;
        }

        public bool TryPurchaseHareGenomeLicense(string profileId, out string error)
        {
            error = null;
            if (committingProgression || !Current.HasLoadedProfile || Current.ProfileId != profileId)
            {
                error = "A Hare Genome license requires the same loaded profile and no operation in progress.";
                return false;
            }
            var hare = new SpeciesId("hare");
            if (Current.UnlockedGenomeSpeciesIds.Contains(hare))
            {
                error = "The Hare Genome path license is already owned.";
                return false;
            }
            Current.SpeciesDataWallets.TryGetValue(hare, out var balance);
            if (balance < FieldDataPermanentProgressionDefinition.HareLicensePrice)
            {
                error = "Not enough Hare Data for the Hare Genome path license.";
                return false;
            }
            committingProgression = true;
            try
            {
                return TryCommitProfile(Current.WithGenomeEconomy(
                    balance - FieldDataPermanentProgressionDefinition.HareLicensePrice,
                    Current.ResearchData, Current.CompletedRuns, unlockRabbitTree: true), out error);
            }
            finally { committingProgression = false; }
        }

        public bool TryPurchaseHareGenomeNode(string profileId, string nodeId, out string error)
        {
            error = null;
            if (committingProgression || !Current.HasLoadedProfile || Current.ProfileId != profileId)
            {
                error = "A Hare Genome purchase requires the same loaded profile and no operation in progress.";
                return false;
            }
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            var hare = definition.HareMap.SpeciesId;
            if (!Current.UnlockedGenomeSpeciesIds.Contains(hare))
            {
                error = "The Hare Genome path license is required.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(nodeId) || !definition.HareMap.TryGetNode(nodeId, out var node))
            {
                error = "The node must belong to the FD2 Hare Genome catalog.";
                return false;
            }
            Current.TryGetGenomeProfile(hare, out var profile);
            if (profile != null && (profile.IsNodeUnlocked(node.Id) || profile.IsNodeActive(node.Id)))
            {
                error = "The Hare Genome node is already owned or allocated.";
                return false;
            }
            Current.SpeciesDataWallets.TryGetValue(hare, out var balance);
            if (balance < node.RabbitDataPrice)
            {
                error = "Not enough Hare Data for this Genome node.";
                return false;
            }
            var unlocked = profile == null ? new List<string>() : new List<string>(profile.UnlockedNodeIds);
            var active = profile == null ? new List<string>() : new List<string>(profile.ActiveNodeIds);
            unlocked.Add(node.Id);
            active.Add(node.Id);
            SpeciesGenomeProfile nextProfile;
            try
            {
                nextProfile = new SpeciesGenomeProfile(hare, unlocked, active, profile?.Allocations,
                    profile?.ResetGeneration ?? 0);
                // FD2 uses owned prerequisites and catalog capacity, not the POC's active-parent/one-point fallback.
                definition.FreezeOwnedLoadout(nextProfile);
            }
            catch (Exception exception) when (exception is ArgumentException || exception is InvalidOperationException)
            {
                error = exception.Message;
                return false;
            }
            committingProgression = true;
            try
            {
                return TryCommitProfile(Current.WithGenomeEconomy(balance - node.RabbitDataPrice,
                    Current.ResearchData, Current.CompletedRuns, nextProfile), out error);
            }
            finally { committingProgression = false; }
        }

        public bool TryResetGenomeLoadout(string profileId, SpeciesId speciesId,
            long expectedResetGeneration, out string error)
        {
            error = null;
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            if (committingProgression || !Current.HasLoadedProfile || Current.ProfileId != profileId
                || speciesId != definition.HareMap.SpeciesId || !Current.UnlockedGenomeSpeciesIds.Contains(speciesId)
                || !Current.TryGetGenomeProfile(speciesId, out var profile))
            {
                error = "Reset requires the same loaded profile, supported species license and no operation in progress.";
                return false;
            }
            if (profile.ResetGeneration != expectedResetGeneration || profile.ResetGeneration == long.MaxValue)
            {
                error = "The Genome reset request is stale or exceeds the supported reset range.";
                return false;
            }
            if (profile.ActiveNodeIds.Count == 0 || profile.Allocations.Count != 0)
            {
                error = "Reset requires a nonempty permanent Genome loadout without refundable POC allocations.";
                return false;
            }
            if (Current.ResearchData < FieldDataPermanentProgressionDefinition.ResetResearchDataFee)
            {
                error = "Not enough Research Data for the Genome reset.";
                return false;
            }
            var nextProfile = new SpeciesGenomeProfile(speciesId, profile.UnlockedNodeIds,
                resetGeneration: profile.ResetGeneration + 1);
            committingProgression = true;
            try
            {
                return TryCommitProfile(Current.WithGenomeEconomy(Current.RabbitData,
                    Current.ResearchData - FieldDataPermanentProgressionDefinition.ResetResearchDataFee,
                    Current.CompletedRuns, nextProfile), out error);
            }
            finally { committingProgression = false; }
        }

        public bool TryArrangeGenomeLoadout(string profileId, SpeciesId speciesId,
            long expectedResetGeneration, IEnumerable<string> activeNodeIds, out string error)
        {
            error = null;
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            if (committingProgression || !Current.HasLoadedProfile || Current.ProfileId != profileId
                || speciesId != definition.HareMap.SpeciesId || !Current.UnlockedGenomeSpeciesIds.Contains(speciesId)
                || !Current.TryGetGenomeProfile(speciesId, out var profile)
                || profile.ResetGeneration != expectedResetGeneration || profile.Allocations.Count != 0)
            {
                error = "Arrangement requires the same loaded profile, supported license and current reset request.";
                return false;
            }
            SpeciesGenomeProfile nextProfile;
            try
            {
                if (activeNodeIds == null) throw new ArgumentException("Choose the owned Genome nodes to activate.");
                nextProfile = new SpeciesGenomeProfile(speciesId, profile.UnlockedNodeIds,
                    activeNodeIds, resetGeneration: profile.ResetGeneration);
                definition.FreezeOwnedLoadout(nextProfile);
            }
            catch (Exception exception) when (exception is ArgumentException || exception is InvalidOperationException)
            {
                error = exception.Message;
                return false;
            }
            if (profile.ActiveNodeIds.SequenceEqual(nextProfile.ActiveNodeIds)) return true;
            if (profile.ActiveNodeIds.Count != 0)
            {
                error = "Reset the active Genome loadout before applying a replacement selection.";
                return false;
            }
            committingProgression = true;
            try { return TryCommitProfile(Current.WithGenomeProfile(nextProfile), out error); }
            finally { committingProgression = false; }
        }

        public bool TryPurchaseRabbitGenomeLicense(long rabbitDataPrice, out string reason)
        {
            reason = string.Empty;
            if (rabbitDataPrice <= 0) throw new ArgumentOutOfRangeException(nameof(rabbitDataPrice));
            if (!Current.HasLoadedProfile || !Current.GeneLabUnlocked)
            {
                reason = "The Gene Lab requires three completed runs on a loaded profile.";
                return false;
            }

            var hare = new SpeciesId("hare");
            if (Current.UnlockedGenomeSpeciesIds.Contains(hare))
            {
                reason = "Rabbit Genome access is already owned.";
                return false;
            }

            if (Current.RabbitData < rabbitDataPrice)
            {
                reason = "Not enough Rabbit Data.";
                return false;
            }

            Flush();
            CommitCurrent(Current.WithGenomeEconomy(Current.RabbitData - rabbitDataPrice,
                Current.ResearchData, Current.CompletedRuns, unlockRabbitTree: true));
            return true;
        }

        public bool TryPurchaseRabbitGenomeNode(
            SpeciesGenomeMapSnapshot map,
            string nodeId,
            long rabbitDataPrice,
            int capacityPoints,
            out string reason)
        {
            reason = string.Empty;
            if (rabbitDataPrice <= 0) throw new ArgumentOutOfRangeException(nameof(rabbitDataPrice));
            if (capacityPoints < 1 || capacityPoints > GenomeContract.ActiveCapacity)
                throw new ArgumentOutOfRangeException(nameof(capacityPoints));
            if (map == null) throw new ArgumentNullException(nameof(map));
            var hare = new SpeciesId("hare");
            if (map.SpeciesId != hare || string.IsNullOrWhiteSpace(nodeId)
                || !map.TryGetNode(nodeId, out var node))
            {
                reason = "The node must belong to the Rabbit Genome catalog.";
                return false;
            }

            if (!node.IsPurchasable || node.RabbitDataPrice != rabbitDataPrice
                || node.CapacityPoints != capacityPoints)
            {
                reason = "The Rabbit Genome node price and capacity must match its authored offer.";
                return false;
            }

            if (!Current.HasLoadedProfile || !Current.UnlockedGenomeSpeciesIds.Contains(hare))
            {
                reason = "Rabbit Genome access is required.";
                return false;
            }

            Current.TryGetGenomeProfile(hare, out var profile);
            if (profile != null && profile.IsNodeActive(node.Id))
            {
                reason = "The Genome node is already active.";
                return false;
            }

            foreach (var prerequisite in node.PrerequisiteNodeIds)
            {
                if (profile == null || !profile.IsNodeActive(prerequisite))
                {
                    reason = $"The prerequisite Genome node '{prerequisite}' must be active.";
                    return false;
                }
            }

            if ((profile?.CapacityUsed ?? 0) + capacityPoints > GenomeContract.ActiveCapacity)
            {
                reason = "The active Genome has no room for this node.";
                return false;
            }

            if (Current.RabbitData < rabbitDataPrice)
            {
                reason = "Not enough Rabbit Data.";
                return false;
            }

            var active = profile == null ? new List<string>() : new List<string>(profile.ActiveNodeIds);
            var allocations = profile == null
                ? new List<GenomeNodeAllocation>()
                : new List<GenomeNodeAllocation>(profile.Allocations);
            active.Add(node.Id);
            allocations.Add(new GenomeNodeAllocation(node.Id, rabbitDataPrice, capacityPoints));
            var nextProfile = new SpeciesGenomeProfile(hare, profile?.UnlockedNodeIds, active, allocations,
                profile?.ResetGeneration ?? 0);
            Flush();
            CommitCurrent(Current.WithGenomeEconomy(Current.RabbitData - rabbitDataPrice,
                Current.ResearchData, Current.CompletedRuns, nextProfile));
            return true;
        }

        public bool TryUpgradeRabbitGenomeNode(SpeciesGenomeMapSnapshot map, string nodeId, out string reason)
        {
            reason = string.Empty;
            if (map == null) throw new ArgumentNullException(nameof(map));
            var hare = new SpeciesId("hare");
            if (map.SpeciesId != hare || string.IsNullOrWhiteSpace(nodeId)
                || !map.TryGetNode(nodeId, out var node) || !node.IsPurchasable)
            {
                reason = "The node must belong to the Rabbit Genome catalog.";
                return false;
            }
            if (!Current.HasLoadedProfile || !Current.UnlockedGenomeSpeciesIds.Contains(hare)
                || !Current.TryGetGenomeProfile(hare, out var profile)
                || !profile.TryGetAllocation(node.Id, out var allocation))
            {
                reason = "Activate the Genome node before upgrading it.";
                return false;
            }
            if (allocation.CapacityPoints >= node.MaximumCapacityPoints)
            {
                reason = "The Genome node is at maximum rank.";
                return false;
            }
            if (profile.CapacityUsed >= GenomeContract.ActiveCapacity)
            {
                reason = "The active Genome has no room for this node.";
                return false;
            }
            if (Current.RabbitData < node.RabbitDataPrice)
            {
                reason = $"Not enough Rabbit Data. Each rank costs {node.RabbitDataPrice}.";
                return false;
            }

            var allocations = new List<GenomeNodeAllocation>(profile.Allocations);
            for (var index = 0; index < allocations.Count; index++)
            {
                if (allocations[index].NodeId == node.Id)
                {
                    allocations[index] = new GenomeNodeAllocation(node.Id,
                        checked(allocation.RabbitDataSpent + node.RabbitDataPrice),
                        allocation.CapacityPoints + 1);
                    break;
                }
            }
            var nextProfile = new SpeciesGenomeProfile(hare, profile.UnlockedNodeIds,
                profile.ActiveNodeIds, allocations);
            Flush();
            CommitCurrent(Current.WithGenomeEconomy(Current.RabbitData - node.RabbitDataPrice, Current.ResearchData,
                Current.CompletedRuns, nextProfile));
            return true;
        }

        public bool TryResetRabbitGenome(long researchDataFee, out string reason)
        {
            reason = string.Empty;
            if (researchDataFee <= 0) throw new ArgumentOutOfRangeException(nameof(researchDataFee));
            var hare = new SpeciesId("hare");
            if (!Current.HasLoadedProfile || !Current.UnlockedGenomeSpeciesIds.Contains(hare)
                || !Current.TryGetGenomeProfile(hare, out var profile)
                || profile.ActiveNodeIds.Count == 0)
            {
                reason = "There is no active Rabbit Genome allocation to reset.";
                return false;
            }

            if (Current.ResearchData < researchDataFee)
            {
                reason = "Not enough Research Data.";
                return false;
            }

            long refund = 0;
            long rabbitData;
            try
            {
                foreach (var allocation in profile.Allocations)
                {
                    refund = checked(refund + allocation.RabbitDataSpent);
                }

                rabbitData = checked(Current.RabbitData + refund);
            }
            catch (OverflowException)
            {
                reason = "The Rabbit Data refund exceeds the wallet limit.";
                return false;
            }

            var nextProfile = new SpeciesGenomeProfile(hare, profile.UnlockedNodeIds,
                resetGeneration: profile.ResetGeneration);
            Flush();
            CommitCurrent(Current.WithGenomeEconomy(rabbitData,
                Current.ResearchData - researchDataFee, Current.CompletedRuns, nextProfile));
            return true;
        }

        void Load()
        {
            profiles.Clear();
            Current = ProfileSessionSnapshot.Empty;
            LoadWarning = string.Empty;
            migrationSourceJson = string.Empty;
            pendingSimulationTicks = 0;
            pendingPlayTimeMilliseconds = 0;
            autosaveElapsedSeconds = 0;

            if (!storage.HasKey(StoreKey))
            {
                NotifyChanged();
                return;
            }

            var storedJson = storage.GetString(StoreKey);
            ProfileStore store;
            try
            {
                // JsonUtility fills missing lists, hiding the two incompatible v3 wallet shapes.
                store = JsonConvert.DeserializeObject<ProfileStore>(storedJson);
            }
            catch (Exception)
            {
                BackUpUnreadableStore(storedJson);
                NotifyChanged();
                return;
            }

            if (store?.profiles == null || store.version < 0 || store.version > StoreVersion)
            {
                BackUpUnreadableStore(storedJson);
                NotifyChanged();
                return;
            }

            migrationSourceJson = !string.IsNullOrEmpty(store.migrationSourceJson)
                ? store.migrationSourceJson : store.version < StoreVersion ? storedJson : string.Empty;

            var invalidProfileFound = false;
            var seenProfileIds = new HashSet<string>(StringComparer.Ordinal);
            for (var index = 0; index < store.profiles.Count; index++)
            {
                var record = store.profiles[index];
                if (record == null
                    || string.IsNullOrWhiteSpace(record.id)
                    || !seenProfileIds.Add(record.id))
                {
                    invalidProfileFound = true;
                    continue;
                }

                var genomeProfiles = new List<SpeciesGenomeProfile>();
                var invalidGenomeFound = false;
                var seenGenomeSpecies = new HashSet<SpeciesId>();
                if (record.genomeProfiles != null)
                {
                    for (var genomeIndex = 0; genomeIndex < record.genomeProfiles.Count; genomeIndex++)
                    {
                        var genomeRecord = record.genomeProfiles[genomeIndex];
                        if (genomeRecord == null || string.IsNullOrWhiteSpace(genomeRecord.speciesId))
                        {
                            invalidGenomeFound = true;
                            invalidProfileFound = true;
                            continue;
                        }

                        try
                        {
                            var genomeSpeciesId = new SpeciesId(genomeRecord.speciesId);
                            if (!seenGenomeSpecies.Add(genomeSpeciesId))
                            {
                                invalidGenomeFound = true;
                                invalidProfileFound = true;
                                continue;
                            }

                            var allocations = new List<GenomeNodeAllocation>();
                            if (genomeRecord.allocations != null)
                            {
                                foreach (var allocation in genomeRecord.allocations)
                                {
                                    if (allocation == null) throw new ArgumentException("Null Genome allocation.");
                                    allocations.Add(new GenomeNodeAllocation(allocation.nodeId,
                                        allocation.rabbitDataSpent, allocation.capacityPoints));
                                }
                            }

                            if (store.version >= 7 && (!genomeRecord.resetGeneration.HasValue
                                || genomeRecord.resetGeneration.Value < 0))
                            {
                                throw new ArgumentException("Genome reset generation is missing or invalid.");
                            }
                            genomeProfiles.Add(new SpeciesGenomeProfile(
                                genomeSpeciesId,
                                genomeRecord.unlockedNodeIds,
                                genomeRecord.activeNodeIds,
                                allocations, store.version < 7 ? 0 : genomeRecord.resetGeneration.Value));
                        }
                        catch (ArgumentException)
                        {
                            invalidGenomeFound = true;
                            invalidProfileFound = true;
                        }
                        catch (OverflowException)
                        {
                            invalidGenomeFound = true;
                            invalidProfileFound = true;
                        }
                    }
                }

                if (store.version >= 7 && invalidGenomeFound)
                {
                    // Dropping a Genome would also drop its reset duplicate protection.
                    BackUpUnreadableStore(storedJson);
                    profiles.Clear();
                    Current = ProfileSessionSnapshot.Empty;
                    NotifyChanged();
                    return;
                }

                try
                {
                    ResolveWallets(record, store.version, out var researchData, out var wallets);
                    ReadSettlementState(record, store.version, out var receipts, out var pending);
                    var researchLevel = ReadConversionResearchLevel(record, store.version);
                    profiles.Add(new ProfileSessionSnapshot(
                        false,
                        record.id,
                        string.IsNullOrEmpty(record.name) ? "Unnamed Profile" : record.name,
                        genomeProfiles,
                        researchData,
                        record.simulationTicks,
                        record.playTimeMilliseconds,
                        store.version == 0
                            ? new[] { new SpeciesId("hare") }
                            : ParseSpeciesIds(record.unlockedSpeciesIds),
                        ParseSpeciesIds(record.unlockedGenomeSpeciesIds),
                        record.unlockedFeatureIds,
                        0,
                        researchData,
                        record.completedRuns,
                        record.completedRunIds,
                        wallets, receipts, pending, researchLevel));
                }
                catch (FormatException)
                {
                    // Losing a receipt could pay an old journey again, so reject the whole store.
                    BackUpUnreadableStore(storedJson);
                    profiles.Clear();
                    Current = ProfileSessionSnapshot.Empty;
                    NotifyChanged();
                    return;
                }
                catch (ArgumentException)
                {
                    invalidProfileFound = true;
                }
                catch (OverflowException)
                {
                    invalidProfileFound = true;
                }
            }

            if (invalidProfileFound)
            {
                PreserveRecovery(storedJson);
                LoadWarning = "Some profile data could not be loaded. The original save was preserved for recovery.";
            }

            if (!string.IsNullOrEmpty(store.lastLoadedProfileId))
            {
                SelectLoadedProfileWithoutSaving(store.lastLoadedProfileId);
            }

            if (store.version < StoreVersion && !invalidProfileFound)
            {
                PreserveRecovery(storedJson);
                Save();
            }

            NotifyChanged();
        }

        void SelectLoadedProfileWithoutSaving(string profileId)
        {
            for (var index = 0; index < profiles.Count; index++)
            {
                var profile = profiles[index];
                if (profile.ProfileId != profileId)
                {
                    continue;
                }

                Current = profile.WithLoaded(true);
                profiles[index] = Current;
                return;
            }
        }

        public bool TrySetGenomeConfiguration(
            SpeciesId speciesId,
            IEnumerable<string> unlockedNodeIds,
            IEnumerable<string> activeNodeIds,
            out string validationMessage)
        {
            validationMessage = string.Empty;
            if (!Current.HasLoadedProfile)
            {
                validationMessage = "A loaded profile is required before changing Genome configuration.";
                return false;
            }

            try
            {
                Flush();
                Current.TryGetGenomeProfile(speciesId, out var existingProfile);
                if (existingProfile != null && existingProfile.Allocations.Count > 0)
                {
                    validationMessage = "Purchased Genome allocations must be changed through purchase or reset.";
                    return false;
                }

                var nextProfile = Current.WithGenomeProfile(new SpeciesGenomeProfile(
                    speciesId,
                    unlockedNodeIds,
                    activeNodeIds, resetGeneration: existingProfile?.ResetGeneration ?? 0));
                CommitCurrent(nextProfile);
                return true;
            }
            catch (ArgumentException exception)
            {
                validationMessage = exception.Message;
                return false;
            }
        }

        void Save()
        {
            WriteStore(SerializeStore());
        }

        string SerializeStore(ProfileSessionSnapshot replacement = null)
        {
            var store = new ProfileStore
            {
                version = StoreVersion,
                lastLoadedProfileId = Current.HasLoadedProfile ? Current.ProfileId : string.Empty,
                migrationSourceJson = migrationSourceJson,
            };

            for (var index = 0; index < profiles.Count; index++)
            {
                var profile = replacement != null && profiles[index].ProfileId == replacement.ProfileId
                    ? replacement : profiles[index];
                store.profiles.Add(new ProfileRecord
                {
                    id = profile.ProfileId,
                    name = profile.ProfileName,
                    researchData = profile.ResearchData,
                    conversionResearchLevel = profile.ConversionResearchLevel,
                    speciesDataWallets = ToRecords(profile.SpeciesDataWallets),
                    completedRuns = profile.CompletedRuns,
                    completedRunIds = new List<string>(profile.CompletedRunIds),
                    simulationTicks = profile.SimulationTicks,
                    playTimeMilliseconds = profile.PlayTimeMilliseconds,
                    unlockedSpeciesIds = ToStrings(profile.UnlockedSpeciesIds),
                    unlockedGenomeSpeciesIds = ToStrings(profile.UnlockedGenomeSpeciesIds),
                    unlockedFeatureIds = new List<string>(profile.UnlockedFeatureIds),
                    settlementReceipts = profile.SettlementReceipts.Select(ToRecord).ToList(),
                    pendingSettlement = profile.PendingSettlement == null ? null : new PendingSettlementRecord
                    {
                        journeyId = profile.PendingSettlement.JourneyId,
                        speciesId = profile.PendingSettlement.SelectedSpeciesId.Value,
                        leftoverFieldData = profile.PendingSettlement.LeftoverFieldData,
                        researchLevel = profile.PendingSettlement.ConversionResearchLevel,
                    },
                });
                var record = store.profiles[store.profiles.Count - 1];
                for (var genomeIndex = 0; genomeIndex < profile.GenomeProfiles.Count; genomeIndex++)
                {
                    var genomeProfile = profile.GenomeProfiles[genomeIndex];
                    record.genomeProfiles.Add(new GenomeProfileRecord
                    {
                        speciesId = genomeProfile.SpeciesId.Value,
                        unlockedNodeIds = new List<string>(genomeProfile.UnlockedNodeIds),
                        activeNodeIds = new List<string>(genomeProfile.ActiveNodeIds),
                        resetGeneration = genomeProfile.ResetGeneration,
                    });
                    var genomeRecord = record.genomeProfiles[record.genomeProfiles.Count - 1];
                    foreach (var allocation in genomeProfile.Allocations)
                    {
                        genomeRecord.allocations.Add(new GenomeAllocationRecord
                        {
                            nodeId = allocation.NodeId,
                            rabbitDataSpent = allocation.RabbitDataSpent,
                            capacityPoints = allocation.CapacityPoints,
                        });
                    }
                }
            }

            return JsonConvert.SerializeObject(store);
        }

        void WriteStore(string json)
        {
            var existed = storage.HasKey(StoreKey);
            var previous = storage.GetString(StoreKey);
            try
            {
                storage.SetString(StoreKey, json);
                storage.Save();
            }
            catch
            {
                // Do not leave an unpaid transaction's replacement in the writable cache.
                if (existed) storage.SetString(StoreKey, previous);
                else storage.DeleteKey(StoreKey);
                throw;
            }
        }

        void RequireLoadedProfile()
        {
            if (!Current.HasLoadedProfile)
            {
                throw new InvalidOperationException("A loaded profile is required.");
            }
        }

        void UnloadCurrentProfile()
        {
            if (!Current.HasLoadedProfile)
            {
                return;
            }

            for (var index = 0; index < profiles.Count; index++)
            {
                if (profiles[index].ProfileId == Current.ProfileId)
                {
                    profiles[index] = Current.WithLoaded(false);
                    break;
                }
            }
        }

        void CommitCurrent(ProfileSessionSnapshot nextProfile)
        {
            ApplyCurrent(nextProfile);
            Save();
            NotifyChanged();
        }

        void ApplyCurrent(ProfileSessionSnapshot nextProfile)
        {
            Current = nextProfile;
            for (var index = 0; index < profiles.Count; index++)
            {
                if (profiles[index].ProfileId == Current.ProfileId)
                {
                    profiles[index] = Current;
                    break;
                }
            }

        }

        static List<string> ToStrings(IReadOnlyList<SpeciesId> ids)
        {
            var values = new List<string>(ids.Count);
            for (var index = 0; index < ids.Count; index++)
            {
                values.Add(ids[index].Value);
            }

            return values;
        }

        static List<SpeciesId> ParseSpeciesIds(List<string> values)
        {
            var ids = new List<SpeciesId>();
            if (values != null)
            {
                for (var index = 0; index < values.Count; index++)
                {
                    ids.Add(new SpeciesId(values[index]));
                }
            }

            return ids;
        }

        static void ResolveWallets(ProfileRecord record, int version,
            out long researchData, out List<KeyValuePair<SpeciesId, long>> wallets)
        {
            if (record.currency < 0 || record.rabbitData < 0 || record.researchData < 0)
            {
                throw new ArgumentException("Profile balances cannot be negative.");
            }

            wallets = new List<KeyValuePair<SpeciesId, long>>();
            if (version < 3)
            {
                if (record.rabbitData != 0 || record.researchData != 0 || record.speciesDataWallets != null)
                {
                    throw new ArgumentException("Legacy profile contains ambiguous wallet fields.");
                }

                researchData = record.currency;
                return;
            }

            if (version == 3 && record.speciesDataWallets == null)
            {
                // Incoming v3 stored these as distinct balances; retain both under FD2's RD meaning.
                researchData = checked(record.currency + record.researchData);
                if (record.rabbitData != 0)
                {
                    wallets.Add(new KeyValuePair<SpeciesId, long>(new SpeciesId("hare"), record.rabbitData));
                }

                return;
            }

            if (record.speciesDataWallets == null || record.currency != 0 || record.rabbitData != 0
                || (version == 3 && (record.completedRuns != 0
                    || record.completedRunIds?.Count > 0
                    || record.genomeProfiles?.Any(genome => genome?.allocations?.Count > 0) == true)))
            {
                throw new ArgumentException("Profile mixes incompatible species-wallet and Rabbit-wallet formats.");
            }

            researchData = record.researchData;
            foreach (var wallet in record.speciesDataWallets)
            {
                if (wallet == null || string.IsNullOrWhiteSpace(wallet.speciesId))
                {
                    throw new ArgumentException("Species wallets require a valid species ID.");
                }

                wallets.Add(new KeyValuePair<SpeciesId, long>(new SpeciesId(wallet.speciesId), wallet.balance));
            }
        }

        static List<SpeciesDataWalletRecord> ToRecords(IReadOnlyDictionary<SpeciesId, long> wallets)
        {
            var records = new List<SpeciesDataWalletRecord>();
            foreach (var wallet in wallets)
            {
                records.Add(new SpeciesDataWalletRecord { speciesId = wallet.Key.Value, balance = wallet.Value });
            }

            return records;
        }

        static SettlementRecord ToRecord(FieldDataSettlementReceipt receipt)
        {
            return new SettlementRecord
            {
                journeyId = receipt.JourneyId,
                speciesId = receipt.SelectedSpeciesId.Value,
                leftoverFieldData = receipt.LeftoverFieldData,
                researchLevel = receipt.ConversionResearchLevel,
                definitionVersion = receipt.DefinitionVersion,
                fieldDataPerSpeciesData = receipt.FieldDataPerSpeciesData,
                multiplierNumerator = receipt.MultiplierNumerator,
                multiplierDenominator = receipt.MultiplierDenominator,
                baseSpeciesData = receipt.BaseSpeciesData,
                speciesDataDeposit = receipt.SpeciesDataDeposit,
                researchDataBonus = receipt.ResearchDataBonus,
                speciesDataBefore = receipt.SpeciesDataBefore,
                speciesDataAfter = receipt.SpeciesDataAfter,
                researchDataBefore = receipt.ResearchDataBefore,
                researchDataAfter = receipt.ResearchDataAfter,
            };
        }

        static int ReadConversionResearchLevel(ProfileRecord record, int version)
        {
            if (version < 6)
            {
                if (record.conversionResearchLevel.GetValueOrDefault() != 0)
                    throw new FormatException("Legacy profile contains unrecognized conversion research.");
                return 0;
            }
            if (!record.conversionResearchLevel.HasValue || record.conversionResearchLevel < 0
                || record.conversionResearchLevel > FieldDataPermanentProgressionDefinition.CreateStarter().MaximumResearchLevel)
                throw new FormatException("Missing or invalid conversion research level.");
            return record.conversionResearchLevel.Value;
        }

        static void ReadSettlementState(ProfileRecord record, int version,
            out List<FieldDataSettlementReceipt> receipts, out PendingFieldDataSettlement pending)
        {
            receipts = new List<FieldDataSettlementReceipt>();
            pending = null;
            try
            {
                if (version < 5)
                {
                    if (record.settlementReceipts?.Count > 0 || record.pendingSettlement != null)
                        throw new FormatException("Legacy profile contains an unrecognized settlement journal.");
                    return;
                }
                if (record.settlementReceipts == null)
                    throw new FormatException("The settlement journal is missing.");
                var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
                var ids = new HashSet<string>(StringComparer.Ordinal);
                foreach (var saved in record.settlementReceipts)
                {
                    if (saved == null || !ids.Add(saved.journeyId)
                        || record.completedRunIds?.Contains(saved.journeyId) != true)
                        throw new FormatException("Invalid or duplicate completed settlement identity.");
                    var species = new SpeciesId(saved.speciesId);
                    if (!FieldDataSettlement.TryCalculate(definition, saved.journeyId, FieldDataJourneyFlowState.Succeeded,
                        species, saved.leftoverFieldData, saved.researchLevel,
                        new Dictionary<SpeciesId, long> { [species] = saved.speciesDataBefore },
                        saved.researchDataBefore, out var receipt, out _)
                        || JsonUtility.ToJson(saved) != JsonUtility.ToJson(ToRecord(receipt)))
                        throw new FormatException("Invalid settlement arithmetic or definition version.");
                    receipts.Add(receipt);
                }
                var savedPending = record.pendingSettlement;
                if (savedPending != null)
                {
                    var species = new SpeciesId(savedPending.speciesId);
                    if (record.completedRunIds?.Contains(savedPending.journeyId) == true
                        || !FieldDataSettlement.TryCalculate(definition, savedPending.journeyId,
                            FieldDataJourneyFlowState.Succeeded, species, savedPending.leftoverFieldData,
                            savedPending.researchLevel, new Dictionary<SpeciesId, long>(), 0, out _, out _))
                        throw new FormatException("Invalid pending settlement.");
                    pending = new PendingFieldDataSettlement(savedPending.journeyId, species,
                        savedPending.leftoverFieldData, savedPending.researchLevel);
                }
            }
            catch (ArgumentException exception)
            {
                throw new FormatException("Invalid settlement journal.", exception);
            }
            catch (OverflowException exception)
            {
                throw new FormatException("Invalid settlement journal.", exception);
            }
        }

        void PreserveRecovery(string storedJson)
        {
            if (!storage.HasKey(RecoveryStoreKey))
            {
                storage.SetString(RecoveryStoreKey, storedJson);
                storage.Save();
            }
        }

        void BackUpUnreadableStore(string storedJson)
        {
            migrationSourceJson = storedJson;
            var canRemoveStore = !storage.HasKey(RecoveryStoreKey)
                || storage.GetString(RecoveryStoreKey) == storedJson;
            PreserveRecovery(storedJson);
            if (canRemoveStore)
            {
                storage.DeleteKey(StoreKey);
            }

            storage.Save();
            LoadWarning = "Profile data could not be loaded. The original save was preserved for recovery.";
        }

        void NotifyChanged()
        {
            SnapshotChanged?.Invoke(Current);
        }
    }
}
