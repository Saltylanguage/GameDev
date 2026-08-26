using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
    public sealed class Helper_ProfileSession : MonoBehaviour
    {
        public const string StoreKey = "SaltyGame.ProfileSession";

        [Serializable]
        sealed class ProfileRecord
        {
            public string id;
            public string name;
        }

        [Serializable]
        sealed class ProfileStore
        {
            public List<ProfileRecord> profiles = new List<ProfileRecord>();
            public string lastLoadedProfileId;
        }

        readonly List<ProfileSessionSnapshot> profiles = new List<ProfileSessionSnapshot>();

        public event Action<ProfileSessionSnapshot> SnapshotChanged;

        public IReadOnlyList<ProfileSessionSnapshot> Profiles => profiles;
        public ProfileSessionSnapshot Current { get; private set; } = ProfileSessionSnapshot.Empty;
        public bool HasProfiles => profiles.Count > 0;

        void Awake()
        {
            Load();
        }

        public ProfileSessionSnapshot CreateInitialProfile(string displayName)
        {
            if (HasProfiles)
            {
                return Current;
            }

            var normalizedName = string.IsNullOrWhiteSpace(displayName)
                ? "Researcher 01"
                : displayName.Trim();
            var profile = new ProfileSessionSnapshot(
                true,
                Guid.NewGuid().ToString("N"),
                normalizedName);

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

                Current = new ProfileSessionSnapshot(true, profile.ProfileId, profile.ProfileName);
                Save();
                NotifyChanged();
                return true;
            }

            return false;
        }

        public void Reload()
        {
            Load();
        }

        void Load()
        {
            profiles.Clear();
            Current = ProfileSessionSnapshot.Empty;

            if (!PlayerPrefs.HasKey(StoreKey))
            {
                NotifyChanged();
                return;
            }

            ProfileStore store;
            try
            {
                store = JsonUtility.FromJson<ProfileStore>(PlayerPrefs.GetString(StoreKey));
            }
            catch (Exception)
            {
                PlayerPrefs.DeleteKey(StoreKey);
                NotifyChanged();
                return;
            }

            if (store?.profiles == null)
            {
                NotifyChanged();
                return;
            }

            for (var index = 0; index < store.profiles.Count; index++)
            {
                var record = store.profiles[index];
                if (record == null || string.IsNullOrEmpty(record.id))
                {
                    continue;
                }

                profiles.Add(new ProfileSessionSnapshot(
                    false,
                    record.id,
                    string.IsNullOrEmpty(record.name) ? "Unnamed Profile" : record.name));
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

                Current = new ProfileSessionSnapshot(true, profile.ProfileId, profile.ProfileName);
                return;
            }
        }

        void Save()
        {
            var store = new ProfileStore
            {
                lastLoadedProfileId = Current.HasLoadedProfile ? Current.ProfileId : string.Empty,
            };

            for (var index = 0; index < profiles.Count; index++)
            {
                var profile = profiles[index];
                store.profiles.Add(new ProfileRecord
                {
                    id = profile.ProfileId,
                    name = profile.ProfileName,
                });
            }

            PlayerPrefs.SetString(StoreKey, JsonUtility.ToJson(store));
            PlayerPrefs.Save();
        }

        void NotifyChanged()
        {
            SnapshotChanged?.Invoke(Current);
        }
    }
}
