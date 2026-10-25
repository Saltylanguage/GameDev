using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SaltyGame
{
    public sealed class ProfileSessionSnapshot
    {
        public static ProfileSessionSnapshot Empty { get; } =
            new ProfileSessionSnapshot(false, string.Empty, string.Empty, null);

        readonly IReadOnlyList<SpeciesGenomeProfile> genomeProfiles;
        readonly IReadOnlyList<SpeciesId> unlockedSpeciesIds;
        readonly IReadOnlyList<SpeciesId> unlockedGenomeSpeciesIds;
        readonly IReadOnlyList<string> unlockedFeatureIds;
        readonly IReadOnlyList<string> completedRunIds;
        readonly IReadOnlyDictionary<SpeciesId, long> speciesDataWallets;

        public bool HasLoadedProfile { get; }
        public string ProfileId { get; }
        public string ProfileName { get; }
        public IReadOnlyList<SpeciesGenomeProfile> GenomeProfiles => genomeProfiles;
        public IReadOnlyList<SpeciesId> UnlockedSpeciesIds => unlockedSpeciesIds;
        public IReadOnlyList<SpeciesId> UnlockedGenomeSpeciesIds => unlockedGenomeSpeciesIds;
        public IReadOnlyList<string> UnlockedFeatureIds => unlockedFeatureIds;
        public IReadOnlyList<string> CompletedRunIds => completedRunIds;
        public long Currency => ResearchData;
        public long RabbitData => speciesDataWallets.TryGetValue(new SpeciesId("hare"), out var amount) ? amount : 0;
        public IReadOnlyDictionary<SpeciesId, long> SpeciesDataWallets => speciesDataWallets;
        public long ResearchData { get; }
        public int CompletedRuns { get; }
        public bool GeneLabUnlocked => CompletedRuns >= 3;
        public long SimulationTicks { get; }
        public long PlayTimeMilliseconds { get; }
        public double HoursPlayed => PlayTimeMilliseconds / 3600000d;

        public ProfileSessionSnapshot(
            bool hasLoadedProfile,
            string profileId,
            string profileName,
            IEnumerable<SpeciesGenomeProfile> genomeProfiles = null,
            long currency = 0,
            long simulationTicks = 0,
            long playTimeMilliseconds = 0,
            IEnumerable<SpeciesId> unlockedSpeciesIds = null,
            IEnumerable<SpeciesId> unlockedGenomeSpeciesIds = null,
            IEnumerable<string> unlockedFeatureIds = null,
            long rabbitData = 0,
            long? researchData = null,
            int completedRuns = 0,
            IEnumerable<string> completedRunIds = null,
            IEnumerable<KeyValuePair<SpeciesId, long>> speciesDataWallets = null)
        {
            if (currency < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(currency));
            }

            if (rabbitData < 0 || researchData.GetValueOrDefault() < 0 || completedRuns < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rabbitData), "Genome balances and completed runs cannot be negative.");
            }

            if (simulationTicks < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(simulationTicks));
            }

            if (playTimeMilliseconds < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(playTimeMilliseconds));
            }

            HasLoadedProfile = hasLoadedProfile;
            ProfileId = profileId ?? string.Empty;
            ProfileName = profileName ?? string.Empty;
            ResearchData = researchData ?? currency;
            this.speciesDataWallets = CopySpeciesDataWallets(speciesDataWallets, rabbitData);
            CompletedRuns = completedRuns;
            SimulationTicks = simulationTicks;
            PlayTimeMilliseconds = playTimeMilliseconds;
            this.unlockedSpeciesIds = CopySpeciesIds(unlockedSpeciesIds, nameof(unlockedSpeciesIds));
            this.unlockedGenomeSpeciesIds = CopySpeciesIds(unlockedGenomeSpeciesIds, nameof(unlockedGenomeSpeciesIds));
            this.unlockedFeatureIds = CopyFeatureIds(unlockedFeatureIds);
            this.completedRunIds = CopyFeatureIds(completedRunIds, "Completed run");
            if (this.completedRunIds.Count > completedRuns)
            {
                throw new ArgumentException("Completed run ids cannot exceed the recorded run count.", nameof(completedRunIds));
            }

            var copiedGenomeProfiles = new List<SpeciesGenomeProfile>();
            var seenSpecies = new HashSet<SpeciesId>();
            if (genomeProfiles != null)
            {
                foreach (var genomeProfile in genomeProfiles)
                {
                    if (genomeProfile == null)
                    {
                        throw new ArgumentException(
                            "Genome profiles cannot contain null entries.",
                            nameof(genomeProfiles));
                    }

                    if (!seenSpecies.Add(genomeProfile.SpeciesId))
                    {
                        throw new ArgumentException(
                            $"Genome profile for species '{genomeProfile.SpeciesId}' appears more than once.",
                            nameof(genomeProfiles));
                    }

                    copiedGenomeProfiles.Add(genomeProfile);
                }
            }

            this.genomeProfiles = copiedGenomeProfiles.AsReadOnly();
        }

        public bool TryGetGenomeProfile(SpeciesId speciesId, out SpeciesGenomeProfile profile)
        {
            for (var index = 0; index < genomeProfiles.Count; index++)
            {
                if (genomeProfiles[index].SpeciesId == speciesId)
                {
                    profile = genomeProfiles[index];
                    return true;
                }
            }

            profile = null;
            return false;
        }

        public GenomeSimulationSnapshot CreateGenomeSnapshot(GenomeCatalogSnapshot catalog = null)
        {
            var snapshot = GenomeSimulationSnapshot.Create(genomeProfiles);
            return catalog == null ? snapshot : snapshot.Resolve(catalog);
        }

        public ProfileSessionSnapshot WithGenomeProfile(SpeciesGenomeProfile nextProfile)
        {
            if (nextProfile == null)
            {
                throw new ArgumentNullException(nameof(nextProfile));
            }

            var nextProfiles = new List<SpeciesGenomeProfile>(genomeProfiles);
            for (var index = 0; index < nextProfiles.Count; index++)
            {
                if (nextProfiles[index].SpeciesId != nextProfile.SpeciesId)
                {
                    continue;
                }

                nextProfiles[index] = nextProfile;
                return new ProfileSessionSnapshot(
                    HasLoadedProfile,
                    ProfileId,
                    ProfileName,
                    nextProfiles,
                    Currency,
                    SimulationTicks,
                    PlayTimeMilliseconds,
                    unlockedSpeciesIds,
                    unlockedGenomeSpeciesIds,
                    unlockedFeatureIds, RabbitData, ResearchData, CompletedRuns, completedRunIds, speciesDataWallets);
            }

            nextProfiles.Add(nextProfile);
            return new ProfileSessionSnapshot(
                HasLoadedProfile,
                ProfileId,
                ProfileName,
                nextProfiles,
                Currency,
                SimulationTicks,
                PlayTimeMilliseconds,
                unlockedSpeciesIds,
                unlockedGenomeSpeciesIds,
                unlockedFeatureIds, RabbitData, ResearchData, CompletedRuns, completedRunIds, speciesDataWallets);
        }

        public ProfileSessionSnapshot WithLoaded(bool isLoaded)
        {
            return new ProfileSessionSnapshot(isLoaded, ProfileId, ProfileName, genomeProfiles,
                Currency, SimulationTicks, PlayTimeMilliseconds, unlockedSpeciesIds, unlockedGenomeSpeciesIds,
                unlockedFeatureIds, RabbitData, ResearchData, CompletedRuns, completedRunIds, speciesDataWallets);
        }

        public ProfileSessionSnapshot WithCurrency(long amount)
        {
            return new ProfileSessionSnapshot(HasLoadedProfile, ProfileId, ProfileName, genomeProfiles,
                amount, SimulationTicks, PlayTimeMilliseconds, unlockedSpeciesIds, unlockedGenomeSpeciesIds,
                unlockedFeatureIds, RabbitData, amount, CompletedRuns, completedRunIds, speciesDataWallets);
        }

        public ProfileSessionSnapshot WithTime(long simulationTicks, long playTimeMilliseconds)
        {
            return new ProfileSessionSnapshot(HasLoadedProfile, ProfileId, ProfileName, genomeProfiles,
                Currency, simulationTicks, playTimeMilliseconds, unlockedSpeciesIds, unlockedGenomeSpeciesIds,
                unlockedFeatureIds, RabbitData, ResearchData, CompletedRuns, completedRunIds, speciesDataWallets);
        }

        public ProfileSessionSnapshot WithUnlockedSpecies(SpeciesId speciesId)
        {
            if (!speciesId.IsValid)
            {
                throw new ArgumentException("A species id is required.", nameof(speciesId));
            }

            var nextIds = new List<SpeciesId>(unlockedSpeciesIds);
            if (!nextIds.Contains(speciesId))
            {
                nextIds.Add(speciesId);
            }

            return new ProfileSessionSnapshot(HasLoadedProfile, ProfileId, ProfileName, genomeProfiles,
                Currency, SimulationTicks, PlayTimeMilliseconds, nextIds, unlockedGenomeSpeciesIds,
                unlockedFeatureIds, RabbitData, ResearchData, CompletedRuns, completedRunIds, speciesDataWallets);
        }

        public ProfileSessionSnapshot WithUnlockedGenome(SpeciesId speciesId)
        {
            if (!speciesId.IsValid)
            {
                throw new ArgumentException("A species id is required.", nameof(speciesId));
            }

            var nextIds = new List<SpeciesId>(unlockedGenomeSpeciesIds);
            if (!nextIds.Contains(speciesId))
            {
                nextIds.Add(speciesId);
            }

            return new ProfileSessionSnapshot(HasLoadedProfile, ProfileId, ProfileName, genomeProfiles,
                Currency, SimulationTicks, PlayTimeMilliseconds, unlockedSpeciesIds, nextIds,
                unlockedFeatureIds, RabbitData, ResearchData, CompletedRuns, completedRunIds, speciesDataWallets);
        }

        public ProfileSessionSnapshot WithUnlockedFeature(string featureId)
        {
            if (string.IsNullOrWhiteSpace(featureId))
            {
                throw new ArgumentException("A feature id is required.", nameof(featureId));
            }

            var nextIds = new List<string>(unlockedFeatureIds);
            if (!nextIds.Contains(featureId))
            {
                nextIds.Add(featureId);
            }

            return new ProfileSessionSnapshot(HasLoadedProfile, ProfileId, ProfileName, genomeProfiles,
                Currency, SimulationTicks, PlayTimeMilliseconds, unlockedSpeciesIds,
                unlockedGenomeSpeciesIds, nextIds, RabbitData, ResearchData, CompletedRuns, completedRunIds, speciesDataWallets);
        }

        public ProfileSessionSnapshot WithGenomeEconomy(
            long rabbitData,
            long researchData,
            int completedRuns,
            SpeciesGenomeProfile genomeProfile = null,
            bool unlockRabbitTree = false,
            IEnumerable<string> nextCompletedRunIds = null)
        {
            var next = genomeProfile == null ? this : WithGenomeProfile(genomeProfile);
            if (unlockRabbitTree)
            {
                next = next.WithUnlockedGenome(new SpeciesId("hare"));
            }

            var wallets = new Dictionary<SpeciesId, long>(next.SpeciesDataWallets);
            var hare = new SpeciesId("hare");
            if (rabbitData != 0 || wallets.ContainsKey(hare))
            {
                wallets[hare] = rabbitData;
            }

            return new ProfileSessionSnapshot(next.HasLoadedProfile, next.ProfileId, next.ProfileName,
                next.GenomeProfiles, next.Currency, next.SimulationTicks, next.PlayTimeMilliseconds,
                next.UnlockedSpeciesIds, next.UnlockedGenomeSpeciesIds, next.UnlockedFeatureIds,
                rabbitData, researchData, completedRuns, nextCompletedRunIds ?? completedRunIds, wallets);
        }

        static IReadOnlyDictionary<SpeciesId, long> CopySpeciesDataWallets(
            IEnumerable<KeyValuePair<SpeciesId, long>> wallets, long rabbitData)
        {
            var copied = new Dictionary<SpeciesId, long>();
            if (wallets != null)
            {
                foreach (var wallet in wallets)
                {
                    if (!wallet.Key.IsValid || wallet.Value < 0 || copied.ContainsKey(wallet.Key))
                    {
                        throw new ArgumentException("Species wallets need unique valid IDs and nonnegative balances.", nameof(wallets));
                    }

                    copied.Add(wallet.Key, wallet.Value);
                }
            }

            var hare = new SpeciesId("hare");
            if (rabbitData != 0)
            {
                if (copied.TryGetValue(hare, out var amount) && amount != rabbitData)
                {
                    throw new ArgumentException("Rabbit compatibility balance conflicts with the Hare wallet.", nameof(rabbitData));
                }

                copied[hare] = rabbitData;
            }

            return new ReadOnlyDictionary<SpeciesId, long>(copied);
        }

        static IReadOnlyList<SpeciesId> CopySpeciesIds(IEnumerable<SpeciesId> ids, string parameterName)
        {
            var copied = new List<SpeciesId>();
            var seen = new HashSet<SpeciesId>();
            if (ids != null)
            {
                foreach (var id in ids)
                {
                    if (!id.IsValid || !seen.Add(id))
                    {
                        throw new ArgumentException("Species ids must be valid and unique.", parameterName);
                    }

                    copied.Add(id);
                }
            }

            return copied.AsReadOnly();
        }

        static IReadOnlyList<string> CopyFeatureIds(IEnumerable<string> ids, string label = "Feature")
        {
            var copied = new List<string>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            if (ids != null)
            {
                foreach (var id in ids)
                {
                    if (string.IsNullOrWhiteSpace(id) || !seen.Add(id))
                    {
                        throw new ArgumentException($"{label} ids must be non-empty and unique.", nameof(ids));
                    }

                    copied.Add(id);
                }
            }

            return copied.AsReadOnly();
        }
    }
}
