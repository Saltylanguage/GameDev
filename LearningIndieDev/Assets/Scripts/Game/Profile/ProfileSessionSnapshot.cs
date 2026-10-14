using System;
using System.Collections.Generic;

namespace SaltyGame
{
    public sealed class ProfileSessionSnapshot
    {
        public static ProfileSessionSnapshot Empty { get; } =
            new ProfileSessionSnapshot(false, string.Empty, string.Empty, null);

        readonly IReadOnlyList<SpeciesGenomeProfile> genomeProfiles;
        readonly IReadOnlyList<SpeciesId> unlockedSpeciesIds;
        readonly IReadOnlyList<SpeciesId> unlockedGenomeSpeciesIds;

        public bool HasLoadedProfile { get; }
        public string ProfileId { get; }
        public string ProfileName { get; }
        public IReadOnlyList<SpeciesGenomeProfile> GenomeProfiles => genomeProfiles;
        public IReadOnlyList<SpeciesId> UnlockedSpeciesIds => unlockedSpeciesIds;
        public IReadOnlyList<SpeciesId> UnlockedGenomeSpeciesIds => unlockedGenomeSpeciesIds;
        public long Currency { get; }
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
            IEnumerable<SpeciesId> unlockedGenomeSpeciesIds = null)
        {
            if (currency < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(currency));
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
            Currency = currency;
            SimulationTicks = simulationTicks;
            PlayTimeMilliseconds = playTimeMilliseconds;
            this.unlockedSpeciesIds = CopySpeciesIds(unlockedSpeciesIds, nameof(unlockedSpeciesIds));
            this.unlockedGenomeSpeciesIds = CopySpeciesIds(unlockedGenomeSpeciesIds, nameof(unlockedGenomeSpeciesIds));

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

        public GenomeSimulationSnapshot CreateGenomeSnapshot()
        {
            return GenomeSimulationSnapshot.Create(genomeProfiles);
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
                    unlockedGenomeSpeciesIds);
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
                unlockedGenomeSpeciesIds);
        }

        public ProfileSessionSnapshot WithLoaded(bool isLoaded)
        {
            return new ProfileSessionSnapshot(isLoaded, ProfileId, ProfileName, genomeProfiles,
                Currency, SimulationTicks, PlayTimeMilliseconds, unlockedSpeciesIds, unlockedGenomeSpeciesIds);
        }

        public ProfileSessionSnapshot WithCurrency(long amount)
        {
            return new ProfileSessionSnapshot(HasLoadedProfile, ProfileId, ProfileName, genomeProfiles,
                amount, SimulationTicks, PlayTimeMilliseconds, unlockedSpeciesIds, unlockedGenomeSpeciesIds);
        }

        public ProfileSessionSnapshot WithTime(long simulationTicks, long playTimeMilliseconds)
        {
            return new ProfileSessionSnapshot(HasLoadedProfile, ProfileId, ProfileName, genomeProfiles,
                Currency, simulationTicks, playTimeMilliseconds, unlockedSpeciesIds, unlockedGenomeSpeciesIds);
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
                Currency, SimulationTicks, PlayTimeMilliseconds, nextIds, unlockedGenomeSpeciesIds);
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
                Currency, SimulationTicks, PlayTimeMilliseconds, unlockedSpeciesIds, nextIds);
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
    }
}
