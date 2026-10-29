using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SaltyGame
{
    public sealed class Helper_ProfileSession : MonoBehaviour
    {
        public const string StoreKey = "SaltyGame.ProfileSession";
        public const string RecoveryStoreKey = StoreKey + ".Recovery";
        const int StoreVersion = 3;
        const double AutosaveSeconds = 30d;

        [SerializeField] bool trackPlaytime = true;

        [Serializable]
        sealed class GenomeProfileRecord
        {
            public string speciesId;
            public List<string> unlockedNodeIds = new List<string>();
            public List<string> activeNodeIds = new List<string>();
            public List<GenomeAllocationRecord> allocations = new List<GenomeAllocationRecord>();
        }

        [Serializable]
        sealed class GenomeAllocationRecord
        {
            public string nodeId;
            public long rabbitDataSpent;
            public int capacityPoints;
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
            public int completedRuns;
            public List<string> completedRunIds = new List<string>();
            public long simulationTicks;
            public long playTimeMilliseconds;
            public List<string> unlockedSpeciesIds = new List<string>();
            public List<string> unlockedGenomeSpeciesIds = new List<string>();
            public List<string> unlockedFeatureIds = new List<string>();
        }

        [Serializable]
        sealed class ProfileStore
        {
            public int version;
            public List<ProfileRecord> profiles = new List<ProfileRecord>();
            public string lastLoadedProfileId;
        }

        readonly List<ProfileSessionSnapshot> profiles = new List<ProfileSessionSnapshot>();
        long pendingSimulationTicks;
        double pendingPlayTimeMilliseconds;
        double autosaveElapsedSeconds;
        bool applicationFocused = true;
        bool applicationPaused;

        public event Action<ProfileSessionSnapshot> SnapshotChanged;

        public IReadOnlyList<ProfileSessionSnapshot> Profiles => profiles;
        public ProfileSessionSnapshot Current { get; private set; } = ProfileSessionSnapshot.Empty;
        public bool HasProfiles => profiles.Count > 0;
        public string LoadWarning { get; private set; } = string.Empty;

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
            var nextProfile = new SpeciesGenomeProfile(hare, profile?.UnlockedNodeIds, active, allocations);
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

            var nextProfile = new SpeciesGenomeProfile(hare, profile.UnlockedNodeIds);
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
            pendingSimulationTicks = 0;
            pendingPlayTimeMilliseconds = 0;
            autosaveElapsedSeconds = 0;

            if (!PlayerPrefs.HasKey(StoreKey))
            {
                NotifyChanged();
                return;
            }

            var storedJson = PlayerPrefs.GetString(StoreKey);
            ProfileStore store;
            try
            {
                store = JsonUtility.FromJson<ProfileStore>(storedJson);
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
                var seenGenomeSpecies = new HashSet<SpeciesId>();
                if (record.genomeProfiles != null)
                {
                    for (var genomeIndex = 0; genomeIndex < record.genomeProfiles.Count; genomeIndex++)
                    {
                        var genomeRecord = record.genomeProfiles[genomeIndex];
                        if (genomeRecord == null || string.IsNullOrWhiteSpace(genomeRecord.speciesId))
                        {
                            invalidProfileFound = true;
                            continue;
                        }

                        try
                        {
                            var genomeSpeciesId = new SpeciesId(genomeRecord.speciesId);
                            if (!seenGenomeSpecies.Add(genomeSpeciesId))
                            {
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

                            genomeProfiles.Add(new SpeciesGenomeProfile(
                                genomeSpeciesId,
                                genomeRecord.unlockedNodeIds,
                                genomeRecord.activeNodeIds,
                                allocations));
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
                }

                try
                {
                    profiles.Add(new ProfileSessionSnapshot(
                        false,
                        record.id,
                        string.IsNullOrEmpty(record.name) ? "Unnamed Profile" : record.name,
                        genomeProfiles,
                        record.currency,
                        record.simulationTicks,
                        record.playTimeMilliseconds,
                        store.version == 0
                            ? new[] { new SpeciesId("hare") }
                            : ParseSpeciesIds(record.unlockedSpeciesIds),
                        ParseSpeciesIds(record.unlockedGenomeSpeciesIds),
                        record.unlockedFeatureIds,
                        record.rabbitData,
                        record.researchData,
                        record.completedRuns,
                        record.completedRunIds));
                }
                catch (ArgumentException)
                {
                    invalidProfileFound = true;
                }
            }

            if (invalidProfileFound)
            {
                PlayerPrefs.SetString(RecoveryStoreKey, storedJson);
                PlayerPrefs.Save();
                LoadWarning = "Some profile data could not be loaded. The original save was preserved for recovery.";
            }

            if (!string.IsNullOrEmpty(store.lastLoadedProfileId))
            {
                SelectLoadedProfileWithoutSaving(store.lastLoadedProfileId);
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
                if (Current.TryGetGenomeProfile(speciesId, out var existingProfile)
                    && existingProfile.Allocations.Count > 0)
                {
                    validationMessage = "Purchased Genome allocations must be changed through purchase or reset.";
                    return false;
                }

                var nextProfile = Current.WithGenomeProfile(new SpeciesGenomeProfile(
                    speciesId,
                    unlockedNodeIds,
                    activeNodeIds));
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
            var store = new ProfileStore
            {
                version = StoreVersion,
                lastLoadedProfileId = Current.HasLoadedProfile ? Current.ProfileId : string.Empty,
            };

            for (var index = 0; index < profiles.Count; index++)
            {
                var profile = profiles[index];
                store.profiles.Add(new ProfileRecord
                {
                    id = profile.ProfileId,
                    name = profile.ProfileName,
                    currency = profile.Currency,
                    rabbitData = profile.RabbitData,
                    researchData = profile.ResearchData,
                    completedRuns = profile.CompletedRuns,
                    completedRunIds = new List<string>(profile.CompletedRunIds),
                    simulationTicks = profile.SimulationTicks,
                    playTimeMilliseconds = profile.PlayTimeMilliseconds,
                    unlockedSpeciesIds = ToStrings(profile.UnlockedSpeciesIds),
                    unlockedGenomeSpeciesIds = ToStrings(profile.UnlockedGenomeSpeciesIds),
                    unlockedFeatureIds = new List<string>(profile.UnlockedFeatureIds),
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

            PlayerPrefs.SetString(StoreKey, JsonUtility.ToJson(store));
            PlayerPrefs.Save();
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
            Current = nextProfile;
            for (var index = 0; index < profiles.Count; index++)
            {
                if (profiles[index].ProfileId == Current.ProfileId)
                {
                    profiles[index] = Current;
                    break;
                }
            }

            Save();
            NotifyChanged();
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

        void BackUpUnreadableStore(string storedJson)
        {
            PlayerPrefs.SetString(RecoveryStoreKey, storedJson);
            PlayerPrefs.DeleteKey(StoreKey);
            PlayerPrefs.Save();
            LoadWarning = "Profile data could not be loaded. The original save was preserved for recovery.";
        }

        void NotifyChanged()
        {
            SnapshotChanged?.Invoke(Current);
        }
    }
}
