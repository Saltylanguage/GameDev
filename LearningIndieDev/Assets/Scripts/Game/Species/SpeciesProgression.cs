using System;
using System.Collections.Generic;

namespace SaltyGame
{
    public sealed class SpeciesProgression
    {
        readonly Dictionary<string, int> purchasedUpgradeLevels =
            new Dictionary<string, int>(StringComparer.Ordinal);
        readonly List<string> orderedUpgradeIds = new List<string>();
        readonly List<SpeciesUpgradeSnapshot> appliedRunUpgrades =
            new List<SpeciesUpgradeSnapshot>();

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
        public IReadOnlyList<string> OrderedUpgradeIds => orderedUpgradeIds.AsReadOnly();
        public IReadOnlyList<SpeciesUpgradeSnapshot> AppliedRunUpgrades => appliedRunUpgrades.AsReadOnly();

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

        public bool CanPurchase(SpeciesUpgrade upgrade)
        {
            if (upgrade == null)
            {
                throw new ArgumentNullException(nameof(upgrade));
            }

            return Currency >= upgrade.Cost
                && GetUpgradeLevel(upgrade.Id) < SpeciesUpgradeCatalog.GetMaxLevel(upgrade.Id);
        }

        public bool TryPurchase(SpeciesUpgrade upgrade)
        {
            if (!CanPurchase(upgrade))
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
            orderedUpgradeIds.Add(upgrade.Id);
            PurchasedUpgradeCount++;
            return true;
        }

        public bool TryApplyRunUpgrade(SpeciesUpgradeSnapshot upgrade)
        {
            if (upgrade == null)
            {
                throw new ArgumentNullException(nameof(upgrade));
            }

            if (upgrade.TargetSpecies != Definition.Id
                || GetUpgradeLevel(upgrade.Id) > 0)
            {
                return false;
            }

            foreach (var prerequisiteId in upgrade.PrerequisiteUpgradeIds)
            {
                if (GetUpgradeLevel(prerequisiteId) == 0)
                {
                    return false;
                }
            }

            foreach (var excludedId in upgrade.ExcludedUpgradeIds)
            {
                if (GetUpgradeLevel(excludedId) > 0)
                {
                    return false;
                }
            }

            var nextRules = upgrade.Apply(CurrentRules);
            CurrentRules = nextRules;
            purchasedUpgradeLevels[upgrade.Id] = 1;
            orderedUpgradeIds.Add(upgrade.Id);
            appliedRunUpgrades.Add(upgrade);
            PurchasedUpgradeCount++;
            return true;
        }

        public void SetRules(SpeciesRules rules)
        {
            CurrentRules = rules ?? throw new ArgumentNullException(nameof(rules));
        }
    }
}
