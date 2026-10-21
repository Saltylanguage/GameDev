using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace SaltyGame
{
    [Flags]
    public enum FieldDataIncomeSource
    {
        None = 0,
        RoundReward = 1,
        NodeReward = 2,
        Shop = 4,
    }

    public enum FieldDataIncomeOperation
    {
        AddPerHare = 0,
        MultiplyPopulationPayout = 1,
    }

    public readonly struct FieldDataRouteNode
    {
        public FieldDataRouteNode(string id, JourneyNodeKind kind, string nextNodeId)
        {
            Id = id?.Trim() ?? string.Empty;
            Kind = kind;
            NextNodeId = nextNodeId?.Trim() ?? string.Empty;
        }

        public string Id { get; }
        public JourneyNodeKind Kind { get; }
        public string NextNodeId { get; }
    }

    public readonly struct FieldDataPopulationControl
    {
        public FieldDataPopulationControl(string id, string speciesId, int amount, int cost)
        {
            Id = id?.Trim() ?? string.Empty;
            SpeciesId = speciesId?.Trim() ?? string.Empty;
            Amount = amount;
            Cost = cost;
        }

        public string Id { get; }
        public string SpeciesId { get; }
        public int Amount { get; }
        public int Cost { get; }
    }

    public readonly struct FieldDataIncomeOffer
    {
        public FieldDataIncomeOffer(
            string id,
            string displayName,
            FieldDataIncomeOperation operation,
            decimal value,
            FieldDataIncomeSource sources,
            int firstShopVisit,
            int shopCost)
        {
            Id = id?.Trim() ?? string.Empty;
            DisplayName = displayName?.Trim() ?? string.Empty;
            Operation = operation;
            Value = value;
            Sources = sources;
            FirstShopVisit = firstShopVisit;
            ShopCost = shopCost;
        }

        public string Id { get; }
        public string DisplayName { get; }
        public FieldDataIncomeOperation Operation { get; }
        public decimal Value { get; }
        public FieldDataIncomeSource Sources { get; }
        public int FirstShopVisit { get; }
        public int ShopCost { get; }
    }

    public readonly struct FieldDataSignedSpeciesOffer
    {
        public FieldDataSignedSpeciesOffer(
            string id,
            string familyId,
            string targetSpeciesId,
            string attributeId,
            float signedAmount,
            int cost,
            int firstShopVisit)
        {
            Id = id?.Trim() ?? string.Empty;
            FamilyId = familyId?.Trim() ?? string.Empty;
            TargetSpeciesId = targetSpeciesId?.Trim() ?? string.Empty;
            AttributeId = attributeId?.Trim() ?? string.Empty;
            SignedAmount = signedAmount;
            Cost = cost;
            FirstShopVisit = firstShopVisit;
        }

        public string Id { get; }
        public string FamilyId { get; }
        public string TargetSpeciesId { get; }
        public string AttributeId { get; }
        public float SignedAmount { get; }
        public int Cost { get; }
        public int FirstShopVisit { get; }
    }

    public readonly struct FieldDataReorderPricing
    {
        public FieldDataReorderPricing(int baseRandomCost, int increaseNumerator, int increaseDenominator, int chosenMultiplier)
        {
            BaseRandomCost = baseRandomCost;
            IncreaseNumerator = increaseNumerator;
            IncreaseDenominator = increaseDenominator;
            ChosenMultiplier = chosenMultiplier;
        }

        public int BaseRandomCost { get; }
        public int IncreaseNumerator { get; }
        public int IncreaseDenominator { get; }
        public int ChosenMultiplier { get; }
    }

    /// <summary>Validated immutable rules resolved once from the authoring asset.</summary>
    public sealed class FieldDataJourneyDefinition
    {
        public const string CurrentVersion = "field-data-journey-v1";
        public const int InitialBalance = 0;
        public const int RoundLengthTicks = 100;
        public const int RoundsPerSimulation = 6;
        public const int SimulationCount = 3;
        public const int NodeClearReward = 10;
        public const int FreeChoicesPerReward = 3;
        public const int RoundChoicesFirstLocalRound = 1;
        public const int RoundChoicesLastLocalRound = 5;
        public const int NodeChoicesAfterSimulationCount = 2;
        public const int ShopOfferCount = 5;
        public const int SpeciesOfferCost = 30;
        public const int SpeciesOfferFamilyCopyLimit = 3;

        readonly ReadOnlyCollection<FieldDataRouteNode> route;
        readonly ReadOnlyCollection<FieldDataPopulationControl> populationControls;
        readonly ReadOnlyCollection<FieldDataIncomeOffer> incomeOffers;
        readonly ReadOnlyCollection<FieldDataSignedSpeciesOffer> signedSpeciesOffers;

        internal FieldDataJourneyDefinition(
            string id,
            string scenarioId,
            string selectedSpeciesId,
            IEnumerable<FieldDataRouteNode> route,
            IEnumerable<FieldDataPopulationControl> populationControls,
            IEnumerable<FieldDataIncomeOffer> incomeOffers,
            IEnumerable<FieldDataSignedSpeciesOffer> signedSpeciesOffers,
            FieldDataReorderPricing reorderPricing)
        {
            Id = id;
            ScenarioId = scenarioId;
            SelectedSpeciesId = selectedSpeciesId;
            this.route = Copy(route);
            this.populationControls = Copy(populationControls);
            this.incomeOffers = Copy(incomeOffers);
            this.signedSpeciesOffers = Copy(signedSpeciesOffers);
            ReorderPricing = reorderPricing;
            Fingerprint = CreateFingerprint();
        }

        public string Id { get; }
        public string ScenarioId { get; }
        public string SelectedSpeciesId { get; }
        public string Version => CurrentVersion;
        public string Fingerprint { get; }
        public int InitialFieldData => InitialBalance;
        public int RoundLength => RoundLengthTicks;
        public int RoundsPerNode => RoundsPerSimulation;
        public int NodeClearFieldData => NodeClearReward;
        public int FreeChoicesPerRewardEvent => FreeChoicesPerReward;
        public int FirstRoundChoice => RoundChoicesFirstLocalRound;
        public int LastRoundChoice => RoundChoicesLastLocalRound;
        public int NodeRewardChoiceCount => NodeChoicesAfterSimulationCount;
        public int OffersPerShop => ShopOfferCount;
        public int SpeciesOfferCopyLimit => SpeciesOfferFamilyCopyLimit;
        public FieldDataReorderPricing ReorderPricing { get; }
        public IReadOnlyList<FieldDataRouteNode> Route => route;
        public IReadOnlyList<FieldDataPopulationControl> PopulationControls => populationControls;
        public IReadOnlyList<FieldDataIncomeOffer> IncomeOffers => incomeOffers;
        public IReadOnlyList<FieldDataSignedSpeciesOffer> SignedSpeciesOffers => signedSpeciesOffers;

        static ReadOnlyCollection<T> Copy<T>(IEnumerable<T> source)
        {
            return new ReadOnlyCollection<T>(new List<T>(source ?? Array.Empty<T>()));
        }

        string CreateFingerprint()
        {
            var canonical = new StringBuilder();
            Append(canonical, Version);
            Append(canonical, Id);
            Append(canonical, ScenarioId);
            Append(canonical, SelectedSpeciesId);
            Append(canonical, InitialBalance.ToString(CultureInfo.InvariantCulture));
            Append(canonical, RoundLengthTicks.ToString(CultureInfo.InvariantCulture));
            Append(canonical, RoundsPerSimulation.ToString(CultureInfo.InvariantCulture));
            Append(canonical, SimulationCount.ToString(CultureInfo.InvariantCulture));
            Append(canonical, NodeClearReward.ToString(CultureInfo.InvariantCulture));
            Append(canonical, FreeChoicesPerReward.ToString(CultureInfo.InvariantCulture));
            Append(canonical, RoundChoicesFirstLocalRound.ToString(CultureInfo.InvariantCulture));
            Append(canonical, RoundChoicesLastLocalRound.ToString(CultureInfo.InvariantCulture));
            Append(canonical, NodeChoicesAfterSimulationCount.ToString(CultureInfo.InvariantCulture));
            Append(canonical, ShopOfferCount.ToString(CultureInfo.InvariantCulture));
            Append(canonical, SpeciesOfferCost.ToString(CultureInfo.InvariantCulture));
            Append(canonical, SpeciesOfferFamilyCopyLimit.ToString(CultureInfo.InvariantCulture));
            Append(canonical, ReorderPricing.BaseRandomCost.ToString(CultureInfo.InvariantCulture));
            Append(canonical, ReorderPricing.IncreaseNumerator.ToString(CultureInfo.InvariantCulture));
            Append(canonical, ReorderPricing.IncreaseDenominator.ToString(CultureInfo.InvariantCulture));
            Append(canonical, ReorderPricing.ChosenMultiplier.ToString(CultureInfo.InvariantCulture));
            foreach (var node in route)
            {
                Append(canonical, node.Id);
                Append(canonical, ((int)node.Kind).ToString(CultureInfo.InvariantCulture));
                Append(canonical, node.NextNodeId);
            }
            foreach (var control in populationControls)
            {
                Append(canonical, control.Id);
                Append(canonical, control.SpeciesId);
                Append(canonical, control.Amount.ToString(CultureInfo.InvariantCulture));
                Append(canonical, control.Cost.ToString(CultureInfo.InvariantCulture));
            }
            foreach (var offer in incomeOffers)
            {
                Append(canonical, offer.Id);
                Append(canonical, offer.DisplayName);
                Append(canonical, ((int)offer.Operation).ToString(CultureInfo.InvariantCulture));
                Append(canonical, offer.Value.ToString(CultureInfo.InvariantCulture));
                Append(canonical, ((int)offer.Sources).ToString(CultureInfo.InvariantCulture));
                Append(canonical, offer.FirstShopVisit.ToString(CultureInfo.InvariantCulture));
                Append(canonical, offer.ShopCost.ToString(CultureInfo.InvariantCulture));
            }
            foreach (var offer in signedSpeciesOffers)
            {
                Append(canonical, offer.Id);
                Append(canonical, offer.FamilyId);
                Append(canonical, offer.TargetSpeciesId);
                Append(canonical, offer.AttributeId);
                Append(canonical, offer.SignedAmount.ToString("R", CultureInfo.InvariantCulture));
                Append(canonical, offer.Cost.ToString(CultureInfo.InvariantCulture));
                Append(canonical, offer.FirstShopVisit.ToString(CultureInfo.InvariantCulture));
            }

            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(canonical.ToString()));
                var result = new StringBuilder(hash.Length * 2);
                foreach (var value in hash)
                {
                    result.Append(value.ToString("x2", CultureInfo.InvariantCulture));
                }
                return result.ToString();
            }
        }

        static void Append(StringBuilder builder, string value)
        {
            builder.Append(value.Length.ToString(CultureInfo.InvariantCulture))
                .Append(':').Append(value).Append('|');
        }
    }
}
