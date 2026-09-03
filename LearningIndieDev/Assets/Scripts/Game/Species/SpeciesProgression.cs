using System;
using System.Collections.Generic;

namespace SaltyGame
{
    public sealed class SpeciesProgression
    {
        readonly Dictionary<string, int> purchasedUpgradeLevels =
            new Dictionary<string, int>(StringComparer.Ordinal);

        public SpeciesProgression(SpeciesDefinition definition)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            CurrentRules = definition.Rules;
        }

        public SpeciesDefinition Definition { get; }
        public SpeciesRules CurrentRules { get; private set; }
        public float PreContactAvoidanceChance { get; private set; }
        public int Currency { get; private set; }
        public int PurchasedUpgradeCount { get; private set; }

        public int GetUpgradeLevel(string upgradeId)
        {
            return !string.IsNullOrWhiteSpace(upgradeId)
                && purchasedUpgradeLevels.TryGetValue(upgradeId, out var level)
                ? level
                : 0;
        }

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

            if (SpeciesUpgradeCatalog.IsThreatExposureId(upgrade.Id)
                && GetUpgradeLevel(upgrade.Id) >= SpeciesUpgradeCatalog.ThreatExposureMaxLevel)
            {
                return false;
            }

            if (!TrySpend(upgrade.Cost))
            {
                return false;
            }

            var nextLevel = GetUpgradeLevel(upgrade.Id) + 1;
            if (SpeciesUpgradeCatalog.IsThreatExposureId(upgrade.Id))
            {
                if (SpeciesUpgradeCatalog.IsThreatExposureFleeLevel(nextLevel))
                {
                    CurrentRules = upgrade.Apply(CurrentRules);
                }

                PreContactAvoidanceChance = SpeciesUpgradeCatalog.GetThreatExposureAvoidanceChance(nextLevel);
            }
            else
            {
                CurrentRules = upgrade.Apply(CurrentRules);
            }

            purchasedUpgradeLevels[upgrade.Id] = nextLevel;
            PurchasedUpgradeCount++;
            return true;
        }

        public void SetRules(SpeciesRules rules)
        {
            CurrentRules = rules ?? throw new ArgumentNullException(nameof(rules));
        }
    }
}
