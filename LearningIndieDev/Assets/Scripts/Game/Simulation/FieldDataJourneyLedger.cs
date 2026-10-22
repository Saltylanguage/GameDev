using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SaltyGame
{
    public enum FieldDataAwardKind
    {
        PopulationPayout,
        NodeClear,
    }

    /// <summary>Immutable record of one successful Field Data award transaction.</summary>
    public sealed class FieldDataAwardReceipt
    {
        readonly ReadOnlyDictionary<SpeciesId, int> populationCounts;

        internal FieldDataAwardReceipt(
            string identity,
            FieldDataAwardKind kind,
            int globalRound,
            int tick,
            IReadOnlyDictionary<SpeciesId, int> populationCounts,
            FieldDataIncomeReceipt income,
            int amount,
            int balanceBefore,
            int balanceAfter)
        {
            Identity = identity;
            Kind = kind;
            GlobalRound = globalRound;
            Tick = tick;
            this.populationCounts = new ReadOnlyDictionary<SpeciesId, int>(
                new Dictionary<SpeciesId, int>(populationCounts));
            Income = income;
            Amount = amount;
            BalanceBefore = balanceBefore;
            BalanceAfter = balanceAfter;
        }

        public string Identity { get; }
        public FieldDataAwardKind Kind { get; }
        public int GlobalRound { get; }
        public int Tick { get; }
        public IReadOnlyDictionary<SpeciesId, int> PopulationCounts => populationCounts;
        public FieldDataIncomeReceipt Income { get; }
        public int Amount { get; }
        public int BalanceBefore { get; }
        public int BalanceAfter { get; }
    }

    /// <summary>
    /// Owns the journey-only Field Data balance and idempotent award receipts.
    /// It has no connection to desktop currency or permanent unlocks.
    /// </summary>
    public sealed class FieldDataJourneyLedger
    {
        readonly string journeyId;
        readonly FieldDataJourneyDefinition definition;
        readonly Dictionary<string, FieldDataAwardReceipt> awards =
            new Dictionary<string, FieldDataAwardReceipt>(StringComparer.Ordinal);

        public FieldDataJourneyLedger(string journeyId, FieldDataJourneyDefinition definition)
        {
            this.journeyId = journeyId?.Trim();
            this.definition = definition ?? throw new ArgumentNullException(nameof(definition));
            if (string.IsNullOrWhiteSpace(this.journeyId))
            {
                throw new ArgumentException("A stable journey identity is required.", nameof(journeyId));
            }

            Balance = definition.InitialFieldData;
        }

        public int Balance { get; private set; }
        public int FieldDataLostOnFailure { get; private set; }
        public bool IsClosed { get; private set; }
        public IReadOnlyCollection<FieldDataAwardReceipt> Awards => awards.Values;

        internal bool TrySpend(int amount, out int balanceBefore, out int balanceAfter, out string error)
        {
            balanceBefore = Balance;
            balanceAfter = Balance;
            error = null;
            if (IsClosed || amount <= 0)
            {
                error = "A positive Field Data purchase cannot be made from a closed journey.";
                return false;
            }

            if (Balance < amount)
            {
                error = "There is not enough Field Data for this purchase.";
                return false;
            }

            balanceAfter = Balance - amount;
            Balance = balanceAfter;
            return true;
        }

        public bool TrySettleSuccessfulRound(
            int globalRound,
            int tick,
            IReadOnlyDictionary<SpeciesId, int> populationCounts,
            IReadOnlyList<FieldDataIncomeModifierInstance> orderedModifiers,
            out FieldDataAwardReceipt populationAward,
            out FieldDataAwardReceipt nodeClearAward,
            out string error)
        {
            populationAward = null;
            nodeClearAward = null;
            if (!TrySettlePopulationPayout(globalRound, tick, populationCounts, orderedModifiers,
                out populationAward, out error))
            {
                return false;
            }

            if (globalRound % FieldDataJourneyDefinition.RoundsPerSimulation != 0)
            {
                error = null;
                return true;
            }

            return TrySettleNodeClearAward(globalRound, tick, populationAward.PopulationCounts,
                out nodeClearAward, out error);
        }

        public bool TrySettlePopulationPayout(
            int globalRound,
            int tick,
            IReadOnlyDictionary<SpeciesId, int> populationCounts,
            IReadOnlyList<FieldDataIncomeModifierInstance> orderedModifiers,
            out FieldDataAwardReceipt populationAward,
            out string error)
        {
            populationAward = null;
            if (!TryValidateRound(globalRound, tick, populationCounts, out var selectedPopulation, out error))
            {
                return false;
            }

            var populationIdentity = AwardIdentity(globalRound, FieldDataAwardKind.PopulationPayout);
            if (awards.TryGetValue(populationIdentity, out populationAward))
            {
                return true;
            }

            if (!FieldDataIncome.TryCalculate(
                selectedPopulation,
                GetHareCount(populationCounts),
                orderedModifiers,
                out var income,
                out error))
            {
                return false;
            }

            var nextBalance = CheckedBalance(Balance, income.Award, out error);
            if (nextBalance < 0)
            {
                return false;
            }

            populationAward = new FieldDataAwardReceipt(
                populationIdentity, FieldDataAwardKind.PopulationPayout, globalRound, tick,
                populationCounts, income, income.Award, Balance, nextBalance);
            awards.Add(populationIdentity, populationAward);
            Balance = nextBalance;
            return true;
        }

        public bool TrySettleNodeClearAward(
            int globalRound,
            int tick,
            IReadOnlyDictionary<SpeciesId, int> populationCounts,
            out FieldDataAwardReceipt nodeClearAward,
            out string error)
        {
            nodeClearAward = null;
            error = null;
            if (IsClosed)
            {
                error = "A closed journey cannot settle awards.";
                return false;
            }

            if (globalRound % FieldDataJourneyDefinition.RoundsPerSimulation != 0
                || !TryValidateRound(globalRound, tick, populationCounts, out _, out error))
            {
                if (string.IsNullOrEmpty(error))
                {
                    error = "A node-clear award must identify a completed simulation node.";
                }
                return false;
            }

            var populationIdentity = AwardIdentity(globalRound, FieldDataAwardKind.PopulationPayout);
            if (!awards.TryGetValue(populationIdentity, out var populationAward))
            {
                error = "The population payout must settle before the simulation node-clear reward.";
                return false;
            }

            var nodeIdentity = AwardIdentity(globalRound, FieldDataAwardKind.NodeClear);
            if (awards.TryGetValue(nodeIdentity, out nodeClearAward))
            {
                error = null;
                return true;
            }

            var nextBalance = CheckedBalance(Balance, definition.NodeClearFieldData, out error);
            if (nextBalance < 0)
            {
                return false;
            }

            nodeClearAward = new FieldDataAwardReceipt(
                nodeIdentity, FieldDataAwardKind.NodeClear, globalRound, tick,
                populationAward.PopulationCounts, null, definition.NodeClearFieldData, Balance, nextBalance);
            awards.Add(nodeIdentity, nodeClearAward);
            Balance = nextBalance;
            return true;
        }

        bool TryValidateRound(
            int globalRound,
            int tick,
            IReadOnlyDictionary<SpeciesId, int> populationCounts,
            out int selectedPopulation,
            out string error)
        {
            selectedPopulation = 0;
            error = null;
            if (IsClosed)
            {
                error = "A closed journey cannot settle awards.";
                return false;
            }

            var totalRounds = FieldDataJourneyDefinition.RoundsPerSimulation
                * FieldDataJourneyDefinition.SimulationCount;
            if (globalRound < 1 || globalRound > totalRounds
                || tick != checked(globalRound * definition.RoundLength))
            {
                error = "An award must identify one of the journey's exact completed round boundaries.";
                return false;
            }

            if (populationCounts == null
                || !populationCounts.TryGetValue(new SpeciesId(definition.SelectedSpeciesId), out selectedPopulation)
                || selectedPopulation <= 0)
            {
                error = "The selected species must still be alive before an award can settle.";
                return false;
            }

            return true;
        }

        public bool TrySpend(int amount)
        {
            if (IsClosed || amount < 0 || Balance < amount)
            {
                return false;
            }

            Balance -= amount;
            return true;
        }

        public int FailAndForfeit()
        {
            if (IsClosed)
            {
                return FieldDataLostOnFailure;
            }

            FieldDataLostOnFailure = Balance;
            Balance = 0;
            IsClosed = true;
            return FieldDataLostOnFailure;
        }

        public bool TryDiscardSuccessfulRemainder(out int discarded)
        {
            discarded = 0;
            var totalRounds = FieldDataJourneyDefinition.RoundsPerSimulation
                * FieldDataJourneyDefinition.SimulationCount;
            for (var round = 1; round <= totalRounds; round++)
            {
                if (!awards.ContainsKey(AwardIdentity(round, FieldDataAwardKind.PopulationPayout)))
                {
                    return false;
                }
            }

            for (var node = 1; node <= FieldDataJourneyDefinition.SimulationCount; node++)
            {
                var clearRound = node * FieldDataJourneyDefinition.RoundsPerSimulation;
                if (!awards.ContainsKey(AwardIdentity(clearRound, FieldDataAwardKind.NodeClear)))
                {
                    return false;
                }
            }

            if (IsClosed)
            {
                return true;
            }

            discarded = Balance;
            Balance = 0;
            IsClosed = true;
            return true;
        }

        string AwardIdentity(int round, FieldDataAwardKind kind)
        {
            return string.Concat(journeyId, "/round/",
                round.ToString(System.Globalization.CultureInfo.InvariantCulture), "/", kind);
        }

        static int GetHareCount(IReadOnlyDictionary<SpeciesId, int> counts)
        {
            return counts.TryGetValue(new SpeciesId("hare"), out var hareCount) ? hareCount : 0;
        }

        int CheckedBalance(int balance, int amount, out string error)
        {
            error = null;
            try
            {
                return checked(balance + amount);
            }
            catch (OverflowException)
            {
                error = "The Field Data wallet exceeded its supported integer range; no award was applied.";
                return -1;
            }
        }
    }
}
