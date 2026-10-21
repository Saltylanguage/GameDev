using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
    [Serializable]
    public sealed class FieldDataRouteNodeData
    {
        [SerializeField] string id;
        [SerializeField] JourneyNodeKind kind;
        [SerializeField] string nextNodeId;

        public string Id => id;
        public JourneyNodeKind Kind => kind;
        public string NextNodeId => nextNodeId;
        internal FieldDataRouteNode ToValue() => new FieldDataRouteNode(id, kind, nextNodeId);
    }

    [Serializable]
    public sealed class FieldDataPopulationControlData
    {
        [SerializeField] string id;
        [SerializeField] string speciesId;
        [SerializeField] int amount;
        [SerializeField] int cost;

        public string Id => id;
        public string SpeciesId => speciesId;
        public int Amount => amount;
        public int Cost => cost;
        internal FieldDataPopulationControl ToValue() => new FieldDataPopulationControl(id, speciesId, amount, cost);
    }

    [Serializable]
    public sealed class FieldDataIncomeOfferData
    {
        [SerializeField] string id;
        [SerializeField] string displayName;
        [SerializeField] FieldDataIncomeOperation operation;
        [SerializeField] string value;
        [SerializeField] FieldDataIncomeSource sources;
        [SerializeField, Min(1)] int firstShopVisit = 1;
        [SerializeField, Min(0)] int shopCost;

        public string Id => id;
        public string DisplayName => displayName;
        public FieldDataIncomeOperation Operation => operation;
        public string Value => value;
        public FieldDataIncomeSource Sources => sources;
        public int FirstShopVisit => firstShopVisit;
        public int ShopCost => shopCost;
    }

    [Serializable]
    public sealed class FieldDataSignedSpeciesOfferData
    {
        [SerializeField] string id;
        [SerializeField] string familyId;
        [SerializeField] string targetSpeciesId;
        [SerializeField] string attributeId;
        [SerializeField] float signedAmount;
        [SerializeField, Min(0)] int cost;
        [SerializeField, Min(1)] int firstShopVisit = 1;

        public string Id => id;
        public string FamilyId => familyId;
        public string TargetSpeciesId => targetSpeciesId;
        public string AttributeId => attributeId;
        public float SignedAmount => signedAmount;
        public int Cost => cost;
        public int FirstShopVisit => firstShopVisit;
        internal FieldDataSignedSpeciesOffer ToValue() => new FieldDataSignedSpeciesOffer(
            id, familyId, targetSpeciesId, attributeId, signedAmount, cost, firstShopVisit);
    }

    [CreateAssetMenu(menuName = "Salty Game/Journey/Field Data Definition", fileName = "FieldDataJourneyDefinition")]
    public sealed class FieldDataJourneyDefinitionAsset : ScriptableObject
    {
        [SerializeField] string definitionId;
        [SerializeField] string scenarioId = "ForestEdge";
        [SerializeField] string selectedSpeciesId = "hare";
        [SerializeField] FieldDataRouteNodeData[] route = Array.Empty<FieldDataRouteNodeData>();
        [SerializeField] FieldDataPopulationControlData[] populationControls = Array.Empty<FieldDataPopulationControlData>();
        [SerializeField] FieldDataIncomeOfferData[] incomeOffers = Array.Empty<FieldDataIncomeOfferData>();
        [SerializeField] FieldDataSignedSpeciesOfferData[] signedSpeciesOffers = Array.Empty<FieldDataSignedSpeciesOfferData>();
        [SerializeField, Min(1)] int reorderBaseRandomCost = 10;
        [SerializeField, Min(1)] int reorderIncreaseNumerator = 11;
        [SerializeField, Min(1)] int reorderIncreaseDenominator = 10;
        [SerializeField, Min(1)] int chosenReorderMultiplier = 10;

        public string DefinitionId => definitionId;
        public string ScenarioId => scenarioId;
        public string SelectedSpeciesId => selectedSpeciesId;
        public bool TryResolve(out FieldDataJourneyDefinition definition, out string error)
        {
            definition = null;
            if (string.IsNullOrWhiteSpace(definitionId) || string.IsNullOrWhiteSpace(scenarioId)
                || string.IsNullOrWhiteSpace(selectedSpeciesId))
            {
                error = "A Field Data definition needs stable definition, scenario, and selected-species IDs.";
                return false;
            }

            var nodes = new List<FieldDataRouteNode>();
            foreach (var node in route ?? Array.Empty<FieldDataRouteNodeData>())
            {
                if (node == null)
                {
                    error = "The Field Data route contains a missing node.";
                    return false;
                }
                nodes.Add(node.ToValue());
            }
            if (!ValidateRoute(nodes, out error)) return false;

            if (!ValidatePopulationControls(populationControls, selectedSpeciesId, out var controls, out error)) return false;
            if (!ValidateIncomeOffers(incomeOffers, out var incomes, out error)) return false;
            if (!ValidateSignedSpeciesOffers(signedSpeciesOffers, out var speciesOffers, out error)) return false;
            if (reorderBaseRandomCost <= 0 || reorderIncreaseNumerator <= reorderIncreaseDenominator
                || reorderIncreaseDenominator <= 0 || chosenReorderMultiplier != 10)
            {
                error = "Reorder pricing must start above zero, increase by a valid ratio, and keep chosen reorder at exactly 10× random.";
                return false;
            }

            definition = new FieldDataJourneyDefinition(
                definitionId.Trim(), scenarioId.Trim(), selectedSpeciesId.Trim(),
                nodes, controls, incomes, speciesOffers,
                new FieldDataReorderPricing(reorderBaseRandomCost, reorderIncreaseNumerator,
                    reorderIncreaseDenominator, chosenReorderMultiplier));
            error = string.Empty;
            return true;
        }

        static bool ValidateRoute(IReadOnlyList<FieldDataRouteNode> nodes, out string error)
        {
            var expected = new[]
            {
                JourneyNodeKind.Simulation, JourneyNodeKind.Shop, JourneyNodeKind.Simulation,
                JourneyNodeKind.Shop, JourneyNodeKind.Simulation,
            };
            if (nodes.Count != expected.Length)
            {
                error = "The starter Field Data route must contain exactly five nodes.";
                return false;
            }
            var ids = new HashSet<string>(StringComparer.Ordinal);
            for (var i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                if (string.IsNullOrWhiteSpace(node.Id) || !ids.Add(node.Id)
                    || node.Kind != expected[i])
                {
                    error = "The starter route needs unique stable node IDs in Simulation → Shop → Simulation → Shop → Simulation order.";
                    return false;
                }
                var expectedNext = i == nodes.Count - 1 ? string.Empty : nodes[i + 1].Id;
                if (!string.Equals(node.NextNodeId, expectedNext, StringComparison.Ordinal))
                {
                    error = $"Route node '{node.Id}' must connect to '{expectedNext}'.";
                    return false;
                }
            }
            error = string.Empty;
            return true;
        }

        static bool ValidatePopulationControls(
            IReadOnlyList<FieldDataPopulationControlData> source,
            string selectedSpeciesId,
            out List<FieldDataPopulationControl> values,
            out string error)
        {
            values = new List<FieldDataPopulationControl>();
            var expected = new Dictionary<string, (string species, int amount)>(StringComparer.Ordinal)
            {
                ["add-hares"] = ("hare", 1), ["remove-hares"] = ("hare", -1),
                ["add-foxes"] = ("fox", 1), ["remove-foxes"] = ("fox", -1),
                ["add-plants"] = ("plant", 5), ["remove-plants"] = ("plant", -5),
            };
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var item in source ?? Array.Empty<FieldDataPopulationControlData>())
            {
                if (item == null || string.IsNullOrWhiteSpace(item.Id) || !seen.Add(item.Id)
                    || !expected.TryGetValue(item.Id, out var rule)
                    || !string.Equals(item.SpeciesId, rule.species, StringComparison.Ordinal)
                    || item.Amount != rule.amount || item.Cost != 10 || item.Cost <= 0)
                {
                    error = "Population controls must use the six unique starter IDs, species, quantities, and 10-FD prices.";
                    return false;
                }
                values.Add(item.ToValue());
            }
            if (seen.Count != expected.Count || !seen.Contains("add-" + selectedSpeciesId + "s")
                || selectedSpeciesId != "hare")
            {
                error = "The starter definition requires all six population controls and the Hare-selected starter species.";
                return false;
            }
            error = string.Empty;
            return true;
        }

        static bool ValidateIncomeOffers(
            IReadOnlyList<FieldDataIncomeOfferData> source,
            out List<FieldDataIncomeOffer> values,
            out string error)
        {
            values = new List<FieldDataIncomeOffer>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var item in source ?? Array.Empty<FieldDataIncomeOfferData>())
            {
                if (item == null || string.IsNullOrWhiteSpace(item.Id) || string.IsNullOrWhiteSpace(item.DisplayName)
                    || !seen.Add(item.Id) || !decimal.TryParse(item.Value, System.Globalization.NumberStyles.Number,
                        System.Globalization.CultureInfo.InvariantCulture, out var amount))
                {
                    error = "Income offers need unique stable IDs, display names, and invariant numeric values.";
                    return false;
                }
                var valid = item.Id == "income-plus-one-per-hare"
                    ? item.Operation == FieldDataIncomeOperation.AddPerHare && amount == 1m
                        && item.Sources == FieldDataIncomeSource.Shop
                        && item.FirstShopVisit == 1 && item.ShopCost == 20
                    : item.Id == "income-multiply-one-point-five"
                        ? item.Operation == FieldDataIncomeOperation.MultiplyPopulationPayout && amount == 1.5m
                            && item.Sources == FieldDataIncomeSource.Shop
                            && item.FirstShopVisit == 1 && item.ShopCost == 30
                    : item.Id == "income-multiply-two"
                        ? item.Operation == FieldDataIncomeOperation.MultiplyPopulationPayout && amount == 2m
                            && item.Sources == FieldDataIncomeSource.Shop && item.FirstShopVisit == 2 && item.ShopCost == 50
                        : false;
                if (!valid)
                {
                    error = $"Income offer '{item.Id}' has an unsupported effect, source, visit, or cost.";
                    return false;
                }
                values.Add(new FieldDataIncomeOffer(item.Id, item.DisplayName, item.Operation, amount,
                    item.Sources, item.FirstShopVisit, item.ShopCost));
            }
            if (seen.Count != 3)
            {
                error = "The starter definition requires all three income offers.";
                return false;
            }
            error = string.Empty;
            return true;
        }

        static bool ValidateSignedSpeciesOffers(
            IReadOnlyList<FieldDataSignedSpeciesOfferData> source,
            out List<FieldDataSignedSpeciesOffer> values,
            out string error)
        {
            values = new List<FieldDataSignedSpeciesOffer>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            var familySigns = new HashSet<string>(StringComparer.Ordinal);
            foreach (var item in source ?? Array.Empty<FieldDataSignedSpeciesOfferData>())
            {
                var expectedAttribute = SupportedShopAttribute(item?.FamilyId);
                var expectedAmount = SupportedShopAmount(item?.FamilyId);
                if (item == null || string.IsNullOrWhiteSpace(item.Id) || string.IsNullOrWhiteSpace(item.FamilyId)
                    || !seen.Add(item.Id) || item.Cost != 30 || item.TargetSpeciesId != "hare"
                    || !SpeciesAttributeRegistry.Contains(item.AttributeId)
                    || string.IsNullOrEmpty(expectedAttribute) || item.AttributeId != expectedAttribute
                    || Math.Abs(item.SignedAmount) != expectedAmount
                    || item.FirstShopVisit != 1
                    || float.IsNaN(item.SignedAmount) || float.IsInfinity(item.SignedAmount) || item.SignedAmount == 0f
                    || !familySigns.Add(item.FamilyId + (item.SignedAmount > 0 ? "+" : "-")))
                {
                    error = $"Signed Hare offer '{item?.Id ?? "<missing>"}' is invalid: family='{item?.FamilyId}', attribute='{item?.AttributeId}', amount={item?.SignedAmount}, expected attribute='{expectedAttribute}', expected absolute amount={expectedAmount}; offers require unique ID/family sign, a supported runtime attribute, a non-zero finite amount, and a 30-FD price.";
                    return false;
                }
                values.Add(item.ToValue());
            }
            error = string.Empty;
            return true;
        }

        static string SupportedShopAttribute(string familyId)
        {
            switch (familyId)
            {
                case "long-stride": return "movement.speed";
                case "sharper-instinct": return SpeciesAttributeIds.PreContactAvoidanceChance;
                case "reinforced-hide": return "combat.block";
                case "warren-comfort": return "crowding.energy-reduction";
                case "deep-digestion": return "digestion.energy-bonus";
                case "abundant-seedfall": return "resource.seed-drop-chance";
                default: return string.Empty;
            }
        }

        static float SupportedShopAmount(string familyId)
        {
            switch (familyId)
            {
                case "long-stride": return 1.5f;
                case "sharper-instinct": return 0.24f;
                case "reinforced-hide": return 6f;
                case "warren-comfort":
                case "deep-digestion": return 0.3f;
                case "abundant-seedfall": return 0.03f;
                default: return 0f;
            }
        }
    }
}
