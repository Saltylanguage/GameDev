using System.Collections.Generic;
using NUnit.Framework;

namespace SaltyGame.Tests
{
    [TestFixture]
    [Category("Simulation")]
    public sealed class FieldDataSettlementTests
    {
        readonly SpeciesId hare = new SpeciesId("hare");
        readonly SpeciesId fox = new SpeciesId("fox");
        FieldDataPermanentProgressionDefinition definition;
        Dictionary<SpeciesId, long> wallets;

        [SetUp]
        public void SetUp()
        {
            definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            wallets = new Dictionary<SpeciesId, long> { [fox] = 400, [hare] = 7 };
        }

        [TestCase(0L, 0, 0L)]
        [TestCase(9L, 0, 0L)]
        [TestCase(10L, 0, 1L)]
        [TestCase(19L, 0, 1L)]
        [TestCase(19L, 1, 1L)]
        [TestCase(99L, 3, 11L)]
        [TestCase(100L, 0, 10L)]
        [TestCase(100L, 1, 11L)]
        [TestCase(100L, 2, 12L)]
        [TestCase(100L, 3, 13L)]
        [TestCase(199L, 3, 25L)]
        [TestCase(long.MaxValue, 3, 1227630818105370658L)]
        public void UsesTwoFloorsAndIncludesCompleteBeforeAndAfterBalances(long leftover, int level, long deposit)
        {
            Assert.That(Calculate(leftover, level, out var receipt, out var error), Is.True, error);
            Assert.That(receipt.JourneyId, Is.EqualTo("journey-01"));
            Assert.That(receipt.SelectedSpeciesId, Is.EqualTo(hare));
            Assert.That(receipt.LeftoverFieldData, Is.EqualTo(leftover));
            Assert.That(receipt.FieldDataPerSpeciesData, Is.EqualTo(10));
            Assert.That(receipt.ConversionResearchLevel, Is.EqualTo(level));
            Assert.That(receipt.BaseSpeciesData, Is.EqualTo(leftover / 10));
            Assert.That(receipt.SpeciesDataDeposit, Is.EqualTo(deposit));
            Assert.That(receipt.ResearchDataBonus, Is.EqualTo(10));
            Assert.That(receipt.SpeciesDataBefore, Is.EqualTo(7));
            Assert.That(receipt.SpeciesDataAfter, Is.EqualTo(7 + deposit));
            Assert.That(receipt.ResearchDataBefore, Is.EqualTo(30));
            Assert.That(receipt.ResearchDataAfter, Is.EqualTo(40));
            Assert.That(receipt.DefinitionVersion, Is.EqualTo("field-data-permanent-v1"));
        }

        [TestCase(0, 1L, 1L)]
        [TestCase(1, 11L, 10L)]
        [TestCase(2, 121L, 100L)]
        [TestCase(3, 1331L, 1000L)]
        public void FreezesTheExactResearchRatio(int level, long numerator, long denominator)
        {
            Assert.That(Calculate(100, level, out var receipt, out var error), Is.True, error);
            Assert.That(receipt.MultiplierNumerator, Is.EqualTo(numerator));
            Assert.That(receipt.MultiplierDenominator, Is.EqualTo(denominator));
        }

        [Test]
        public void TargetsTheSelectedWalletAndDoesNotMutateOrRetainInputWallets()
        {
            Assert.That(Calculate(100, 1, out var first, out var error), Is.True, error);
            Assert.That(Calculate(100, 1, out var repeated, out error), Is.True, error);
            Assert.That(first.SpeciesDataBefore, Is.EqualTo(7));
            Assert.That(first.SpeciesDataAfter, Is.EqualTo(18));
            Assert.That(repeated.SpeciesDataAfter, Is.EqualTo(first.SpeciesDataAfter));
            Assert.That(wallets[hare], Is.EqualTo(7));
            Assert.That(wallets[fox], Is.EqualTo(400));
            wallets[hare] = 999;
            Assert.That(first.SpeciesDataBefore, Is.EqualTo(7));
            Assert.That(first.SpeciesDataAfter, Is.EqualTo(18));
        }

        [Test]
        public void MissingSelectedWalletStartsAtZeroWithoutCreatingAnEntry()
        {
            wallets.Remove(hare);
            Assert.That(Calculate(10, 0, out var receipt, out var error), Is.True, error);
            Assert.That(receipt.SpeciesDataBefore, Is.Zero);
            Assert.That(receipt.SpeciesDataAfter, Is.EqualTo(1));
            Assert.That(wallets.ContainsKey(hare), Is.False);
            Assert.That(wallets[fox], Is.EqualTo(400));
        }

        [TestCase(FieldDataJourneyFlowState.Ready)]
        [TestCase(FieldDataJourneyFlowState.Running)]
        [TestCase(FieldDataJourneyFlowState.RoundDecision)]
        [TestCase(FieldDataJourneyFlowState.Shop)]
        [TestCase(FieldDataJourneyFlowState.Failed)]
        [TestCase(FieldDataJourneyFlowState.Closed)]
        [TestCase((FieldDataJourneyFlowState)999)]
        public void DoesNotProduceReceiptsForFailureOrOtherNonSuccessStates(FieldDataJourneyFlowState state)
        {
            AssertRejected(state: state);
        }

        [TestCase(-1L, 0, 0L)]
        [TestCase(0L, -1, 0L)]
        [TestCase(0L, 4, 0L)]
        [TestCase(0L, int.MaxValue, 0L)]
        [TestCase(0L, 0, -1L)]
        [TestCase(0L, 0, long.MaxValue)]
        public void RejectsInvalidBalancesLevelsAndResearchOverflow(long leftover, int level, long researchData)
        {
            AssertRejected(leftover: leftover, level: level, researchData: researchData);
        }

        [Test]
        public void ChecksBothWalletAdditionsBeforeReturningAnyReceipt()
        {
            wallets[hare] = long.MaxValue;
            AssertRejected(leftover: 10);
            Assert.That(wallets[hare], Is.EqualTo(long.MaxValue));
            Assert.That(Calculate(0, 0, out var receipt, out var error), Is.True, error);
            Assert.That(receipt.SpeciesDataAfter, Is.EqualTo(long.MaxValue));
            wallets[hare] = long.MaxValue - 1;
            Assert.That(FieldDataSettlement.TryCalculate(definition, "journey-01", FieldDataJourneyFlowState.Succeeded,
                hare, 10, 0, wallets, long.MaxValue - 10, out receipt, out error), Is.True, error);
            Assert.That(receipt.SpeciesDataAfter, Is.EqualTo(long.MaxValue));
            Assert.That(receipt.ResearchDataAfter, Is.EqualTo(long.MaxValue));
            AssertRejected(leftover: 10, researchData: long.MaxValue - 9);
            Assert.That(wallets[hare], Is.EqualTo(long.MaxValue - 1));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void RequiresAJourneyIdentity(string journeyId)
        {
            Assert.That(FieldDataSettlement.TryCalculate(definition, journeyId, FieldDataJourneyFlowState.Succeeded,
                hare, 10, 0, wallets, 0, out var receipt, out var error), Is.False);
            Assert.That(receipt, Is.Null);
            Assert.That(error, Is.Not.Empty);
        }

        [Test]
        public void RejectsMissingDefinitionsWalletsAndUnknownOrInvalidSpecies()
        {
            Assert.That(FieldDataSettlement.TryCalculate(null, "id", FieldDataJourneyFlowState.Succeeded,
                hare, 0, 0, wallets, 0, out var receipt, out _), Is.False);
            Assert.That(receipt, Is.Null);
            Assert.That(FieldDataSettlement.TryCalculate(definition, "id", FieldDataJourneyFlowState.Succeeded,
                hare, 0, 0, null, 0, out receipt, out _), Is.False);
            Assert.That(receipt, Is.Null);
            foreach (var species in new[] { default(SpeciesId), fox, new SpeciesId("Hare") })
            {
                Assert.That(FieldDataSettlement.TryCalculate(definition, "id", FieldDataJourneyFlowState.Succeeded,
                    species, 0, 0, wallets, 0, out receipt, out var error), Is.False, species.ToString());
                Assert.That(receipt, Is.Null);
                Assert.That(error, Is.Not.Empty);
            }
        }

        [Test]
        public void RejectsInvalidWalletEntriesEvenWhenTheyAreNotTheDepositDestination()
        {
            wallets[fox] = -1;
            AssertRejected();
            wallets.Remove(fox);
            wallets[default] = 0;
            AssertRejected();
        }

        bool Calculate(long leftover, int level, out FieldDataSettlementReceipt receipt, out string error)
        {
            return FieldDataSettlement.TryCalculate(definition, "journey-01", FieldDataJourneyFlowState.Succeeded,
                hare, leftover, level, wallets, 30, out receipt, out error);
        }

        void AssertRejected(long leftover = 100, int level = 0, long researchData = 30,
            FieldDataJourneyFlowState state = FieldDataJourneyFlowState.Succeeded)
        {
            Assert.That(FieldDataSettlement.TryCalculate(definition, "journey-01", state, hare,
                leftover, level, wallets, researchData, out var receipt, out var error), Is.False);
            Assert.That(receipt, Is.Null);
            Assert.That(error, Is.Not.Empty);
        }
    }
}
