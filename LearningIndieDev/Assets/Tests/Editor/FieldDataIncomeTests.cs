using NUnit.Framework;

namespace SaltyGame.EditorTests
{
    public sealed class FieldDataIncomeTests
    {
        [Test]
        public void AppliesModifiersInInstanceOrderAndReturnsEveryStep()
        {
            var modifiers = new[]
            {
                Modifier("hare-bonus-01", "field-data-per-hare", FieldDataIncomeOperation.AddPerHare, 1m),
                Modifier("payout-double-01", "population-payout-double", FieldDataIncomeOperation.MultiplyPopulationPayout, 2m),
            };

            Assert.That(FieldDataIncome.TryCalculate(20, 20, modifiers, out var receipt, out var error), Is.True, error);
            Assert.That(receipt.SelectedPopulation, Is.EqualTo(20));
            Assert.That(receipt.HarePopulation, Is.EqualTo(20));
            Assert.That(receipt.Steps.Count, Is.EqualTo(2));
            Assert.That(receipt.Steps[0].InstanceId, Is.EqualTo("hare-bonus-01"));
            Assert.That(receipt.Steps[0].InputAmount, Is.EqualTo(20m));
            Assert.That(receipt.Steps[0].OutputAmount, Is.EqualTo(40m));
            Assert.That(receipt.Steps[1].InstanceId, Is.EqualTo("payout-double-01"));
            Assert.That(receipt.Steps[1].InputAmount, Is.EqualTo(40m));
            Assert.That(receipt.Steps[1].OutputAmount, Is.EqualTo(80m));
            Assert.That(receipt.UnroundedTotal, Is.EqualTo(80m));
            Assert.That(receipt.Award, Is.EqualTo(80));
        }

        [Test]
        public void ReversingTheSameOperationsChangesTheAward()
        {
            var modifiers = new[]
            {
                Modifier("payout-double-01", "population-payout-double", FieldDataIncomeOperation.MultiplyPopulationPayout, 2m),
                Modifier("hare-bonus-01", "field-data-per-hare", FieldDataIncomeOperation.AddPerHare, 1m),
            };

            Assert.That(FieldDataIncome.TryCalculate(20, 20, modifiers, out var receipt, out var error), Is.True, error);
            Assert.That(receipt.Steps[0].OutputAmount, Is.EqualTo(40m));
            Assert.That(receipt.Steps[1].OutputAmount, Is.EqualTo(60m));
            Assert.That(receipt.Award, Is.EqualTo(60));
        }

        [Test]
        public void KeepsFractionalIntermediatesAndFloorsOnlyTheFinalAward()
        {
            var modifiers = new[]
            {
                Modifier("half-01", "population-payout-half", FieldDataIncomeOperation.MultiplyPopulationPayout, 1.5m),
                Modifier("half-02", "population-payout-half", FieldDataIncomeOperation.MultiplyPopulationPayout, 1.5m),
            };

            Assert.That(FieldDataIncome.TryCalculate(1, 1, modifiers, out var receipt, out var error), Is.True, error);
            Assert.That(receipt.Steps[0].OutputAmount, Is.EqualTo(1.5m));
            Assert.That(receipt.Steps[1].InputAmount, Is.EqualTo(1.5m));
            Assert.That(receipt.UnroundedTotal, Is.EqualTo(2.25m));
            Assert.That(receipt.Award, Is.EqualTo(2));
        }

        [Test]
        public void ZeroPopulationProducesZeroEvenWithActiveModifiers()
        {
            var modifiers = new[]
            {
                Modifier("hare-bonus-01", "field-data-per-hare", FieldDataIncomeOperation.AddPerHare, 1m),
                Modifier("payout-half-01", "population-payout-half", FieldDataIncomeOperation.MultiplyPopulationPayout, 1.5m),
            };

            Assert.That(FieldDataIncome.TryCalculate(0, 0, modifiers, out var receipt, out var error), Is.True, error);
            Assert.That(receipt.Award, Is.Zero);
            Assert.That(receipt.UnroundedTotal, Is.Zero);
        }

        [Test]
        public void RejectsInvalidPopulationOperationsAndDuplicateInstances()
        {
            AssertInvalid(-1, 0, new FieldDataIncomeModifierInstance[0], "Population");
            AssertInvalid(1, 1, new[] { Modifier("bad", "field-data-per-hare", FieldDataIncomeOperation.AddPerHare, 2m) }, "+1");
            AssertInvalid(1, 1, new[] { Modifier("bad", "population-payout-half", FieldDataIncomeOperation.MultiplyPopulationPayout, 1.25m) }, "×1.5 and ×2");
            AssertInvalid(1, 1, new[] { Modifier("same", "population-payout-half", FieldDataIncomeOperation.MultiplyPopulationPayout, 1.5m), Modifier("same", "population-payout-double", FieldDataIncomeOperation.MultiplyPopulationPayout, 2m) }, "unique");
            AssertInvalid(1, 1, new[] { Modifier("unknown", "unknown", (FieldDataIncomeOperation)999, 1m) }, "unsupported operation");
        }

        [Test]
        public void RejectsAwardOutsideExistingIntegerWalletRangeWithoutReceipt()
        {
            var modifiers = new[]
            {
                Modifier("hare-bonus-01", "field-data-per-hare", FieldDataIncomeOperation.AddPerHare, 1m),
            };

            Assert.That(FieldDataIncome.TryCalculate(int.MaxValue, int.MaxValue, modifiers, out var receipt, out var error), Is.False);
            Assert.That(receipt, Is.Null);
            Assert.That(error, Does.Contain("integer wallet range"));
        }

        [Test]
        public void ReportsDecimalOverflowInsteadOfThrowingOrWrapping()
        {
            var modifiers = new FieldDataIncomeModifierInstance[100];
            for (var i = 0; i < modifiers.Length; i++)
            {
                modifiers[i] = Modifier(
                    $"payout-double-{i:D3}",
                    "population-payout-double",
                    FieldDataIncomeOperation.MultiplyPopulationPayout,
                    2m);
            }

            Assert.That(FieldDataIncome.TryCalculate(1, 1, modifiers, out var receipt, out var error), Is.False);
            Assert.That(receipt, Is.Null);
            Assert.That(error, Does.Contain("numeric range"));
        }

        [Test]
        public void EarlierReceiptRemainsFrozenWhenLaterCalculationUsesAnotherOrder()
        {
            var firstOrder = new[]
            {
                Modifier("hare-bonus-01", "field-data-per-hare", FieldDataIncomeOperation.AddPerHare, 1m),
                Modifier("payout-double-01", "population-payout-double", FieldDataIncomeOperation.MultiplyPopulationPayout, 2m),
            };
            var secondOrder = new[]
            {
                Modifier("payout-double-01", "population-payout-double", FieldDataIncomeOperation.MultiplyPopulationPayout, 2m),
                Modifier("hare-bonus-01", "field-data-per-hare", FieldDataIncomeOperation.AddPerHare, 1m),
            };

            Assert.That(FieldDataIncome.TryCalculate(20, 20, firstOrder, out var firstReceipt, out var error), Is.True, error);
            Assert.That(FieldDataIncome.TryCalculate(20, 20, secondOrder, out var secondReceipt, out error), Is.True, error);
            Assert.That(firstReceipt.Award, Is.EqualTo(80));
            Assert.That(firstReceipt.Steps[0].InstanceId, Is.EqualTo("hare-bonus-01"));
            Assert.That(secondReceipt.Award, Is.EqualTo(60));
        }

        static FieldDataIncomeModifierInstance Modifier(
            string instanceId,
            string offerId,
            FieldDataIncomeOperation operation,
            decimal value)
        {
            return new FieldDataIncomeModifierInstance(instanceId, offerId, operation, value);
        }

        static void AssertInvalid(
            int selectedPopulation,
            int harePopulation,
            FieldDataIncomeModifierInstance[] modifiers,
            string expectedError)
        {
            Assert.That(FieldDataIncome.TryCalculate(selectedPopulation, harePopulation, modifiers, out var receipt, out var error), Is.False);
            Assert.That(receipt, Is.Null);
            Assert.That(error, Does.Contain(expectedError));
        }
    }
}
