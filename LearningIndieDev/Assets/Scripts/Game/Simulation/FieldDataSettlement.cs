using System;
using System.Collections.Generic;

namespace SaltyGame
{
    /// <summary>A calculated success deposit; creating it does not pay or persist anything.</summary>
    public sealed class FieldDataSettlementReceipt
    {
        internal FieldDataSettlementReceipt(string journeyId, SpeciesId selectedSpeciesId,
            long leftoverFieldData, int researchLevel, long multiplierNumerator,
            long multiplierDenominator, long baseSpeciesData, long speciesDataDeposit,
            long speciesDataBefore, long speciesDataAfter, long researchDataBefore, long researchDataAfter)
        {
            JourneyId = journeyId;
            SelectedSpeciesId = selectedSpeciesId;
            LeftoverFieldData = leftoverFieldData;
            ConversionResearchLevel = researchLevel;
            MultiplierNumerator = multiplierNumerator;
            MultiplierDenominator = multiplierDenominator;
            BaseSpeciesData = baseSpeciesData;
            SpeciesDataDeposit = speciesDataDeposit;
            SpeciesDataBefore = speciesDataBefore;
            SpeciesDataAfter = speciesDataAfter;
            ResearchDataBefore = researchDataBefore;
            ResearchDataAfter = researchDataAfter;
        }

        public string DefinitionVersion => FieldDataPermanentProgressionDefinition.Version;
        public string JourneyId { get; }
        public SpeciesId SelectedSpeciesId { get; }
        public long LeftoverFieldData { get; }
        public int FieldDataPerSpeciesData => FieldDataPermanentProgressionDefinition.FieldDataPerSpeciesData;
        public int ConversionResearchLevel { get; }
        public long MultiplierNumerator { get; }
        public long MultiplierDenominator { get; }
        public long BaseSpeciesData { get; }
        public long SpeciesDataDeposit { get; }
        public long ResearchDataBonus => FieldDataPermanentProgressionDefinition.SuccessResearchData;
        public long SpeciesDataBefore { get; }
        public long SpeciesDataAfter { get; }
        public long ResearchDataBefore { get; }
        public long ResearchDataAfter { get; }
    }

    /// <summary>Uses the final unspent FD balance and frozen research level; has no wallet side effects.</summary>
    public static class FieldDataSettlement
    {
        public static bool TryCalculate(FieldDataPermanentProgressionDefinition definition,
            string journeyId, FieldDataJourneyFlowState state, SpeciesId selectedSpeciesId,
            long leftoverFieldData, int researchLevel, IReadOnlyDictionary<SpeciesId, long> speciesDataWallets,
            long researchData, out FieldDataSettlementReceipt receipt, out string error)
        {
            receipt = null;
            error = null;
            if (definition == null || speciesDataWallets == null || string.IsNullOrWhiteSpace(journeyId))
            {
                error = "A progression definition, journey identity and species wallets are required.";
                return false;
            }

            if (state != FieldDataJourneyFlowState.Succeeded)
            {
                error = "Only a successful Field Data journey can produce a settlement receipt.";
                return false;
            }

            if (!selectedSpeciesId.IsValid || !definition.Catalog.TryGetSpeciesMap(selectedSpeciesId, out _))
            {
                error = "The selected species is not supported by this progression definition.";
                return false;
            }

            if (leftoverFieldData < 0 || researchData < 0
                || researchLevel < 0 || researchLevel > definition.MaximumResearchLevel)
            {
                error = "Balances must be nonnegative and the conversion research level must be within its cap.";
                return false;
            }

            foreach (var wallet in speciesDataWallets)
            {
                if (!wallet.Key.IsValid || wallet.Value < 0)
                {
                    error = "Species wallets require valid species identities and nonnegative balances.";
                    return false;
                }
            }

            try
            {
                checked
                {
                    long numerator = 1;
                    long denominator = 1;
                    for (var level = 0; level < researchLevel; level++)
                    {
                        numerator *= FieldDataPermanentProgressionDefinition.ResearchMultiplierNumerator;
                        denominator *= FieldDataPermanentProgressionDefinition.ResearchMultiplierDenominator;
                    }

                    var baseData = leftoverFieldData / FieldDataPermanentProgressionDefinition.FieldDataPerSpeciesData;
                    // Split the product so representable deposits do not overflow before division.
                    var deposit = (baseData / denominator) * numerator
                        + (baseData % denominator) * numerator / denominator;
                    speciesDataWallets.TryGetValue(selectedSpeciesId, out var speciesBefore);
                    var speciesAfter = speciesBefore + deposit;
                    var researchAfter = researchData + FieldDataPermanentProgressionDefinition.SuccessResearchData;
                    receipt = new FieldDataSettlementReceipt(journeyId, selectedSpeciesId,
                        leftoverFieldData, researchLevel, numerator, denominator, baseData, deposit,
                        speciesBefore, speciesAfter, researchData, researchAfter);
                }

                return true;
            }
            catch (OverflowException)
            {
                error = "The settlement exceeds the supported permanent wallet range.";
                return false;
            }
        }
    }
}
