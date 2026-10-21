using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;

namespace SaltyGame.EditorTests
{
    public sealed class FieldDataJourneyLedgerTests
    {
        const string DefinitionPath = "Assets/Data/TestData/FieldDataStarterDefinition.asset";

        [Test]
        public void PopulationPayoutAndNodeClearAreIdempotentSeparateAwards()
        {
            var definition = LoadDefinition();
            var ledger = new FieldDataJourneyLedger("attempt-a", definition);
            var counts = Populations(hares: 20, foxes: 3, plants: 50);

            Assert.That(definition.SelectedSpeciesId, Is.EqualTo("hare"));
            Assert.That(counts.TryGetValue(new SpeciesId(definition.SelectedSpeciesId), out var selectedPopulation), Is.True,
                string.Join(",", counts.Keys));
            Assert.That(selectedPopulation, Is.EqualTo(20));
            Assert.That(ledger.Balance, Is.Zero);
            Assert.That(ledger.TrySettleSuccessfulRound(1, 100, counts, EmptyModifiers(),
                out var firstPayout, out var noNodeReward, out var error), Is.True, error);
            Assert.That(noNodeReward, Is.Null);
            Assert.That(firstPayout.Amount, Is.EqualTo(20));
            Assert.That(ledger.Balance, Is.EqualTo(20));
            Assert.That(firstPayout.PopulationCounts[new SpeciesId("fox")], Is.EqualTo(3));

            Assert.That(ledger.TrySettleSuccessfulRound(1, 100, counts, EmptyModifiers(),
                out var retryPayout, out _, out error), Is.True, error);
            Assert.That(retryPayout, Is.SameAs(firstPayout));
            Assert.That(ledger.Balance, Is.EqualTo(20));

            for (var round = 2; round <= 5; round++)
            {
                Assert.That(ledger.TrySettleSuccessfulRound(round, round * 100, counts,
                    EmptyModifiers(), out _, out _, out error), Is.True, error);
            }

            Assert.That(ledger.TrySettlePopulationPayout(6, 600, counts, EmptyModifiers(),
                out var sixthPayout, out error), Is.True, error);
            Assert.That(sixthPayout.Amount, Is.EqualTo(20));
            Assert.That(ledger.Balance, Is.EqualTo(120));
            Assert.That(ledger.TrySettleNodeClearAward(6, 600, counts,
                out var nodeReward, out error), Is.True, error);
            Assert.That(nodeReward.Kind, Is.EqualTo(FieldDataAwardKind.NodeClear));
            Assert.That(nodeReward.Amount, Is.EqualTo(10));
            Assert.That(nodeReward.BalanceBefore, Is.EqualTo(120));
            Assert.That(nodeReward.BalanceAfter, Is.EqualTo(130));
            Assert.That(ledger.Balance, Is.EqualTo(130));

            Assert.That(ledger.TrySettleSuccessfulRound(6, 600, counts, EmptyModifiers(),
                out var repeatedPayout, out var repeatedNodeReward, out error), Is.True, error);
            Assert.That(repeatedPayout, Is.SameAs(sixthPayout));
            Assert.That(repeatedNodeReward, Is.SameAs(nodeReward));
            Assert.That(ledger.Balance, Is.EqualTo(130));
        }

        [Test]
        public void ExtinctPopulationCannotEarnAndFailedJourneyForfeitsOnlyLocalBalance()
        {
            var ledger = new FieldDataJourneyLedger("attempt-fail", LoadDefinition());
            var alive = Populations(hares: 8, foxes: 2, plants: 30);
            Assert.That(ledger.TrySettleSuccessfulRound(1, 100, alive, EmptyModifiers(),
                out _, out _, out var error), Is.True, error);

            var extinct = Populations(hares: 0, foxes: 2, plants: 30);
            Assert.That(ledger.TrySettleSuccessfulRound(2, 200, extinct, EmptyModifiers(),
                out _, out _, out error), Is.False);
            Assert.That(ledger.Balance, Is.EqualTo(8));
            Assert.That(ledger.FailAndForfeit(), Is.EqualTo(8));
            Assert.That(ledger.Balance, Is.Zero);
            Assert.That(ledger.FailAndForfeit(), Is.EqualTo(8));
            Assert.That(ledger.TrySpend(1), Is.False);
        }

        [TestCase(1)]
        [TestCase(6)]
        [TestCase(18)]
        public void ExtinctionAtAnyNodeEdgeCannotEarnThatRoundsPayoutOrNodeReward(int extinctRound)
        {
            var ledger = new FieldDataJourneyLedger("attempt-edge-" + extinctRound, LoadDefinition());
            var alive = Populations(hares: 8, foxes: 2, plants: 30);
            for (var round = 1; round < extinctRound; round++)
            {
                Assert.That(ledger.TrySettleSuccessfulRound(round, round * 100, alive,
                    EmptyModifiers(), out _, out _, out var priorError), Is.True, priorError);
            }

            var extinct = Populations(hares: 0, foxes: 2, plants: 30);
            Assert.That(ledger.TrySettleSuccessfulRound(extinctRound, extinctRound * 100,
                extinct, EmptyModifiers(), out var payout, out var nodeReward, out var error), Is.False);
            Assert.That(payout, Is.Null);
            Assert.That(nodeReward, Is.Null);
            Assert.That(ledger.Awards.Any(award => award.GlobalRound == extinctRound), Is.False);
            var expectedForfeit = ledger.Awards.Sum(award => award.Amount);
            Assert.That(ledger.FailAndForfeit(), Is.EqualTo(expectedForfeit));
            Assert.That(ledger.Balance, Is.Zero);
        }

        [Test]
        public void NewlyAcquiredIncomeModifierChangesOnlyLaterRoundPayouts()
        {
            var ledger = new FieldDataJourneyLedger("attempt-modifier", LoadDefinition());
            var counts = Populations(hares: 10, foxes: 2, plants: 30);
            Assert.That(ledger.TrySettlePopulationPayout(1, 100, counts, EmptyModifiers(),
                out var firstReceipt, out var error), Is.True, error);

            var acquiredAfterRoundOne = new[]
            {
                new FieldDataIncomeModifierInstance(
                    "instance-1", "income-plus-one-per-hare",
                    FieldDataIncomeOperation.AddPerHare, 1m),
            };
            Assert.That(ledger.TrySettlePopulationPayout(2, 200, counts, acquiredAfterRoundOne,
                out var secondReceipt, out error), Is.True, error);

            Assert.That(firstReceipt.Amount, Is.EqualTo(10));
            Assert.That(secondReceipt.Amount, Is.EqualTo(20));
            Assert.That(firstReceipt.BalanceAfter, Is.EqualTo(10));
            Assert.That(secondReceipt.BalanceAfter, Is.EqualTo(30));
        }

        [Test]
        public void SuccessfulRemainderCanOnlyBeDiscardedAfterAllRoundsAndNodeClears()
        {
            var ledger = new FieldDataJourneyLedger("attempt-success", LoadDefinition());
            var counts = Populations(hares: 2, foxes: 1, plants: 10);
            Assert.That(ledger.TryDiscardSuccessfulRemainder(out _), Is.False);

            for (var round = 1; round <= 18; round++)
            {
                Assert.That(ledger.TrySettleSuccessfulRound(round, round * 100, counts,
                    EmptyModifiers(), out _, out _, out var error), Is.True, error);
            }

            var profileProgression = new SpeciesProgression(new SpeciesDefinition(
                new SpeciesId("hare"), SpeciesRuleDefaults.Create()[SpeciesIds.Herbivore]));
            profileProgression.AddCurrency(77);
            var discardedPermanentCurrency = profileProgression.Currency;
            Assert.That(ledger.TryDiscardSuccessfulRemainder(out var discarded), Is.True);
            Assert.That(discarded, Is.EqualTo(66));
            Assert.That(ledger.Balance, Is.Zero);
            Assert.That(ledger.IsClosed, Is.True);
            Assert.That(profileProgression.Currency, Is.EqualTo(discardedPermanentCurrency));
        }

        static FieldDataJourneyDefinition LoadDefinition()
        {
            var asset = AssetDatabase.LoadAssetAtPath<FieldDataJourneyDefinitionAsset>(DefinitionPath);
            Assert.That(asset, Is.Not.Null);
            Assert.That(asset.TryResolve(out var definition, out var error), Is.True, error);
            return definition;
        }

        static IReadOnlyList<FieldDataIncomeModifierInstance> EmptyModifiers()
        {
            return new FieldDataIncomeModifierInstance[0];
        }

        static IReadOnlyDictionary<SpeciesId, int> Populations(int hares, int foxes, int plants)
        {
            return new Dictionary<SpeciesId, int>
            {
                [new SpeciesId("hare")] = hares,
                [new SpeciesId("fox")] = foxes,
                [SpeciesIds.Plant] = plants,
            };
        }
    }
}
