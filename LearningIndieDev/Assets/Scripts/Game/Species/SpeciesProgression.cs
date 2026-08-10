using System;

namespace SaltyGame
{
    public sealed class SpeciesProgression
    {
        public SpeciesProgression(SpeciesDefinition definition)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            CurrentRules = definition.Rules;
        }

        public SpeciesDefinition Definition { get; }
        public SpeciesRules CurrentRules { get; private set; }
        public int Currency { get; private set; }

        public void AddCurrency(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Currency amount cannot be negative.");
            }

            Currency = checked(Currency + amount);
        }

        public bool TrySpend(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Currency amount cannot be negative.");
            }

            if (Currency < amount)
            {
                return false;
            }

            Currency -= amount;
            return true;
        }

        public bool TryPurchase(SpeciesUpgrade upgrade)
        {
            if (upgrade == null)
            {
                throw new ArgumentNullException(nameof(upgrade));
            }

            if (!TrySpend(upgrade.Cost))
            {
                return false;
            }

            CurrentRules = upgrade.Apply(CurrentRules);
            return true;
        }

        public void SetRules(SpeciesRules rules)
        {
            CurrentRules = rules ?? throw new ArgumentNullException(nameof(rules));
        }
    }
}
