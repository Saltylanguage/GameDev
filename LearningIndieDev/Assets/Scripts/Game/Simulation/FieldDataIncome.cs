using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SaltyGame
{
    /// <summary>A single acquired income upgrade in its current payout order.</summary>
    public readonly struct FieldDataIncomeModifierInstance
    {
        public FieldDataIncomeModifierInstance(
            string instanceId,
            string offerId,
            FieldDataIncomeOperation operation,
            decimal value)
        {
            InstanceId = instanceId?.Trim() ?? string.Empty;
            OfferId = offerId?.Trim() ?? string.Empty;
            Operation = operation;
            Value = value;
        }

        public string InstanceId { get; }
        public string OfferId { get; }
        public FieldDataIncomeOperation Operation { get; }
        public decimal Value { get; }
    }

    /// <summary>One immutable arithmetic step retained for payout explanation.</summary>
    public readonly struct FieldDataIncomeBreakdownStep
    {
        internal FieldDataIncomeBreakdownStep(
            string instanceId,
            string offerId,
            FieldDataIncomeOperation operation,
            decimal inputAmount,
            int harePopulation,
            decimal modifierValue,
            decimal outputAmount)
        {
            InstanceId = instanceId;
            OfferId = offerId;
            Operation = operation;
            InputAmount = inputAmount;
            HarePopulation = harePopulation;
            ModifierValue = modifierValue;
            OutputAmount = outputAmount;
        }

        public string InstanceId { get; }
        public string OfferId { get; }
        public FieldDataIncomeOperation Operation { get; }
        public decimal InputAmount { get; }
        public int HarePopulation { get; }
        public decimal ModifierValue { get; }
        public decimal OutputAmount { get; }
    }

    /// <summary>Frozen result of a population payout calculation; excludes node-clear rewards.</summary>
    public sealed class FieldDataIncomeReceipt
    {
        readonly ReadOnlyCollection<FieldDataIncomeBreakdownStep> steps;

        internal FieldDataIncomeReceipt(
            int selectedPopulation,
            int harePopulation,
            IEnumerable<FieldDataIncomeBreakdownStep> steps,
            decimal unroundedTotal,
            int award)
        {
            SelectedPopulation = selectedPopulation;
            HarePopulation = harePopulation;
            this.steps = new ReadOnlyCollection<FieldDataIncomeBreakdownStep>(
                new List<FieldDataIncomeBreakdownStep>(steps));
            UnroundedTotal = unroundedTotal;
            Award = award;
        }

        public int SelectedPopulation { get; }
        public int HarePopulation { get; }
        public IReadOnlyList<FieldDataIncomeBreakdownStep> Steps => steps;
        public decimal UnroundedTotal { get; }
        public int Award { get; }
    }

    /// <summary>Calculates the population portion of a Field Data payout without mutating a wallet.</summary>
    public static class FieldDataIncome
    {
        public static bool TryCalculate(
            int selectedPopulation,
            int harePopulation,
            IReadOnlyList<FieldDataIncomeModifierInstance> orderedModifiers,
            out FieldDataIncomeReceipt receipt,
            out string error)
        {
            receipt = null;
            error = null;

            if (selectedPopulation < 0 || harePopulation < 0)
            {
                error = "Population counts cannot be negative.";
                return false;
            }

            if (orderedModifiers == null)
            {
                error = "The ordered income modifier list is required.";
                return false;
            }

            var seenInstances = new HashSet<string>(StringComparer.Ordinal);
            var steps = new List<FieldDataIncomeBreakdownStep>(orderedModifiers.Count);
            var amount = (decimal)selectedPopulation;

            try
            {
                foreach (var modifier in orderedModifiers)
                {
                    if (string.IsNullOrWhiteSpace(modifier.InstanceId)
                        || string.IsNullOrWhiteSpace(modifier.OfferId)
                        || !seenInstances.Add(modifier.InstanceId))
                    {
                        error = "Income modifier instance IDs and offer IDs must be present and instance IDs unique.";
                        return false;
                    }

                    var input = amount;
                    switch (modifier.Operation)
                    {
                        case FieldDataIncomeOperation.AddPerHare:
                            if (modifier.Value != 1m)
                            {
                                error = "Only the approved +1 Field Data per Hare operation is supported.";
                                return false;
                            }

                            amount += modifier.Value * harePopulation;
                            break;

                        case FieldDataIncomeOperation.MultiplyPopulationPayout:
                            if (modifier.Value != 1.5m && modifier.Value != 2m)
                            {
                                error = "Only the approved ×1.5 and ×2 population-payout operations are supported.";
                                return false;
                            }

                            amount *= modifier.Value;
                            break;

                        default:
                            error = "The income modifier uses an unsupported operation.";
                            return false;
                    }

                    steps.Add(new FieldDataIncomeBreakdownStep(
                        modifier.InstanceId,
                        modifier.OfferId,
                        modifier.Operation,
                        input,
                        harePopulation,
                        modifier.Value,
                        amount));
                }

                var floored = decimal.Floor(amount);
                if (floored > int.MaxValue)
                {
                    error = "The Field Data award exceeds the supported integer wallet range.";
                    return false;
                }

                receipt = new FieldDataIncomeReceipt(
                    selectedPopulation,
                    harePopulation,
                    steps,
                    amount,
                    (int)floored);
                return true;
            }
            catch (OverflowException)
            {
                error = "The Field Data calculation exceeded the supported numeric range.";
                return false;
            }
        }
    }
}
