using System;
using System.Collections.Generic;

namespace SaltyGame
{
    public sealed class ProfileSessionSnapshot
    {
        public static ProfileSessionSnapshot Empty { get; } =
            new ProfileSessionSnapshot(false, string.Empty, string.Empty, null);

        readonly IReadOnlyList<SpeciesGenomeProfile> genomeProfiles;

        public bool HasLoadedProfile { get; }
        public string ProfileId { get; }
        public string ProfileName { get; }
        public IReadOnlyList<SpeciesGenomeProfile> GenomeProfiles => genomeProfiles;

        public ProfileSessionSnapshot(
            bool hasLoadedProfile,
            string profileId,
            string profileName,
            IEnumerable<SpeciesGenomeProfile> genomeProfiles = null)
        {
            HasLoadedProfile = hasLoadedProfile;
            ProfileId = profileId ?? string.Empty;
            ProfileName = profileName ?? string.Empty;

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
                    nextProfiles);
            }

            nextProfiles.Add(nextProfile);
            return new ProfileSessionSnapshot(
                HasLoadedProfile,
                ProfileId,
                ProfileName,
                nextProfiles);
        }
    }
}
