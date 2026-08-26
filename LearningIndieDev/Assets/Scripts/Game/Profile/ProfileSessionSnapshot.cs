namespace SaltyGame
{
    public sealed class ProfileSessionSnapshot
    {
        public static ProfileSessionSnapshot Empty { get; } =
            new ProfileSessionSnapshot(false, string.Empty, string.Empty);

        public bool HasLoadedProfile { get; }
        public string ProfileId { get; }
        public string ProfileName { get; }

        public ProfileSessionSnapshot(bool hasLoadedProfile, string profileId, string profileName)
        {
            HasLoadedProfile = hasLoadedProfile;
            ProfileId = profileId ?? string.Empty;
            ProfileName = profileName ?? string.Empty;
        }
    }
}
