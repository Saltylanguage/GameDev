using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SaltyGame
{
    public sealed class FieldDataRewardChoice
    {
        public FieldDataRewardChoice(string id, string displayName, SpeciesUpgradeSnapshot ordinaryUpgrade, FieldDataIncomeOffer? incomeOffer)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException("Reward choice ID and display name are required.");
            if ((ordinaryUpgrade != null) == incomeOffer.HasValue)
                throw new ArgumentException("A reward choice must contain exactly one ordinary upgrade or income offer.");
            Id = id.Trim();
            DisplayName = displayName.Trim();
            OrdinaryUpgrade = ordinaryUpgrade;
            IncomeOffer = incomeOffer;
        }

        public string Id { get; }
        public string DisplayName { get; }
        public SpeciesUpgradeSnapshot OrdinaryUpgrade { get; }
        public FieldDataIncomeOffer? IncomeOffer { get; }
    }

    public sealed class FieldDataRewardChoiceSet
    {
        readonly ReadOnlyCollection<FieldDataRewardChoice> choices;

        internal FieldDataRewardChoiceSet(string sourceId, IEnumerable<FieldDataRewardChoice> choices)
        {
            SourceId = sourceId;
            this.choices = new ReadOnlyCollection<FieldDataRewardChoice>(new List<FieldDataRewardChoice>(choices));
        }

        public string SourceId { get; }
        public IReadOnlyList<FieldDataRewardChoice> Choices => choices;
        public bool Resolved { get; private set; }
        public string SelectedChoiceId { get; private set; }

        internal bool TryResolve(string choiceId)
        {
            if (Resolved) return string.Equals(SelectedChoiceId, choiceId, StringComparison.Ordinal);
            var found = false;
            foreach (var choice in choices)
                if (string.Equals(choice.Id, choiceId, StringComparison.Ordinal)) found = true;
            if (choiceId != "skip" && !found)
                return false;
            SelectedChoiceId = choiceId;
            Resolved = true;
            return true;
        }
    }
}
