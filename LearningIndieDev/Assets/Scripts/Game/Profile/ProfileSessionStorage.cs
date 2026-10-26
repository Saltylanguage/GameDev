using UnityEngine;

namespace SaltyGame
{
    /// <summary>Profile persistence boundary, including failure injection for transaction tests.</summary>
    public interface IProfileSessionStorage
    {
        bool HasKey(string key);
        string GetString(string key);
        void SetString(string key, string value);
        void DeleteKey(string key);
        void Save();
    }

    sealed class PlayerPrefsProfileSessionStorage : IProfileSessionStorage
    {
        public bool HasKey(string key) => PlayerPrefs.HasKey(key);
        public string GetString(string key) => PlayerPrefs.GetString(key);
        public void SetString(string key, string value) => PlayerPrefs.SetString(key, value);
        public void DeleteKey(string key) => PlayerPrefs.DeleteKey(key);
        public void Save() => PlayerPrefs.Save();
    }

    /// <summary>Unpaid success inputs retained until the deposit and receipt are saved together.</summary>
    public sealed class PendingFieldDataSettlement
    {
        internal PendingFieldDataSettlement(string journeyId, SpeciesId speciesId, long leftoverFieldData, int researchLevel)
        {
            JourneyId = journeyId;
            SelectedSpeciesId = speciesId;
            LeftoverFieldData = leftoverFieldData;
            ConversionResearchLevel = researchLevel;
        }

        public string JourneyId { get; }
        public SpeciesId SelectedSpeciesId { get; }
        public long LeftoverFieldData { get; }
        public int ConversionResearchLevel { get; }
    }
}
