using System;
using System.Linq;

namespace SaltyGame
{
    /// <summary>Profile-owned results and retry state; wallet arithmetic remains in the settlement API.</summary>
    public sealed class FieldDataPermanentResults
    {
        readonly Helper_ProfileSession profile;
        readonly string profileId;
        int researchLevel;
        ProfileSessionSnapshot failureProfile;

        public FieldDataPermanentResults(Helper_ProfileSession profile)
        {
            this.profile = profile ?? throw new ArgumentNullException(nameof(profile));
            if (!profile.Current.HasLoadedProfile) throw new ArgumentException("Results require a loaded profile.");
            profileId = profile.Current.ProfileId;
            var pending = profile.Current.PendingSettlement;
            if (pending != null)
            {
                SetSuccess(pending.JourneyId, pending.SelectedSpeciesId, pending.LeftoverFieldData,
                    pending.ConversionResearchLevel);
                Error = "An unpaid successful journey was recovered. Retry to save its settlement.";
            }
        }

        public bool IsPresented { get; private set; }
        public bool IsSuccess { get; private set; }
        public bool IsPending => IsPresented && IsSuccess && Receipt == null;
        public bool CanLeave => !IsPending;
        public string JourneyId { get; private set; }
        public SpeciesId SelectedSpecies { get; private set; }
        public long FieldDataAmount { get; private set; }
        public FieldDataSettlementReceipt Receipt { get; private set; }
        public string Error { get; private set; } = string.Empty;
        public string Title => IsSuccess ? IsPending ? "Journey complete - settlement pending" : "Journey complete - settlement saved"
            : "Journey failed";
        public string Summary => IsSuccess ? IsPending
            ? "No permanent deposit is confirmed. Retry saving before leaving these results."
            : "Permanent data saved. Return to the Desktop when ready."
            : $"{FieldDataAmount} Field Data forfeited. No permanent data awarded. Permanent balances and Genome unlocks retained.";
        string SpeciesLabel => SelectedSpecies.IsValid
            ? char.ToUpperInvariant(SelectedSpecies.Value[0]) + SelectedSpecies.Value.Substring(1) : "Species";
        public string Details
        {
            get
            {
                if (IsPending) return $"Unspent Field Data: {FieldDataAmount}\n{Error}";
                if (Receipt != null)
                    return $"Unspent Field Data: {Receipt.LeftoverFieldData}\n"
                        + $"Base rate: {Receipt.FieldDataPerSpeciesData} FD = 1 {SpeciesLabel} Data\n"
                        + $"After base conversion: {Receipt.BaseSpeciesData} {SpeciesLabel} Data (rounded down)\n"
                        + $"Conversion research: level {Receipt.ConversionResearchLevel} (x{Receipt.MultiplierNumerator}/{Receipt.MultiplierDenominator})\n"
                        + $"{SpeciesLabel} Data: +{Receipt.SpeciesDataDeposit} ({Receipt.SpeciesDataBefore} -> {Receipt.SpeciesDataAfter})\n"
                        + $"Research Data: +{Receipt.ResearchDataBonus} ({Receipt.ResearchDataBefore} -> {Receipt.ResearchDataAfter})";
                if (failureProfile == null) return "Permanent balances and unlocks are unchanged.";
                failureProfile.SpeciesDataWallets.TryGetValue(SelectedSpecies, out var balance);
                return $"{SpeciesLabel} Data retained: {balance}\nResearch Data retained: {failureProfile.ResearchData}";
            }
        }

        void SetSuccess(string journeyId, SpeciesId species, long leftover, int level)
        {
            IsPresented = true;
            IsSuccess = true;
            JourneyId = journeyId;
            SelectedSpecies = species;
            FieldDataAmount = leftover;
            researchLevel = level;
        }

        public bool ShowSuccess(string journeyId, SpeciesId species, long leftover)
        {
            if (!IsPresented) SetSuccess(journeyId, species, leftover,
                profile.Profiles.FirstOrDefault(value => value.ProfileId == profileId)?.ConversionResearchLevel ?? 0);
            if (!IsSuccess || JourneyId != journeyId) return false;
            return Retry();
        }

        public bool Retry()
        {
            if (!IsPresented || !IsSuccess) return false;
            if (Receipt != null) return true;
            if (!profile.TrySettleFieldDataJourney(profileId, JourneyId, FieldDataJourneyFlowState.Succeeded,
                SelectedSpecies, FieldDataAmount, researchLevel, out var receipt, out var error))
            {
                Error = error;
                return false;
            }
            Receipt = receipt;
            SelectedSpecies = receipt.SelectedSpeciesId;
            FieldDataAmount = receipt.LeftoverFieldData;
            Error = string.Empty;
            return true;
        }

        public void ShowFailure(string journeyId, SpeciesId species, long lost)
        {
            if (IsPresented) return;
            IsPresented = true;
            IsSuccess = false;
            JourneyId = journeyId;
            SelectedSpecies = species;
            FieldDataAmount = lost;
            failureProfile = profile.Profiles.FirstOrDefault(value => value.ProfileId == profileId);
        }

        public bool ClearForNextJourney()
        {
            if (!CanLeave) return false;
            IsPresented = false;
            Receipt = null;
            failureProfile = null;
            Error = string.Empty;
            return true;
        }
    }
}
