using System;
using System.Globalization;
using System.Linq;

namespace SaltyGame
{
    /// <summary>Versioned field-only export of a validated Field Data journey definition.</summary>
    [Serializable]
    public sealed class FieldDataPortableDefinition
    {
        public int schemaVersion = 1;
        public string version;
        public string id;
        public string scenarioId;
        public string selectedSpeciesId;
        public string fingerprint;
        public RouteNode[] route;
        public PopulationControl[] populationControls;
        public IncomeOffer[] incomeOffers;
        public SignedSpeciesOffer[] signedSpeciesOffers;
        public int reorderBaseRandomCost;
        public int reorderIncreaseNumerator;
        public int reorderIncreaseDenominator;
        public int chosenReorderMultiplier;

        public static FieldDataPortableDefinition Create(FieldDataJourneyDefinition definition)
        {
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            return new FieldDataPortableDefinition
            {
                version = definition.Version,
                id = definition.Id,
                scenarioId = definition.ScenarioId,
                selectedSpeciesId = definition.SelectedSpeciesId,
                fingerprint = definition.Fingerprint,
                route = definition.Route.Select(value => new RouteNode
                { id = value.Id, kind = (int)value.Kind, nextNodeId = value.NextNodeId }).ToArray(),
                populationControls = definition.PopulationControls.Select(value => new PopulationControl
                { id = value.Id, speciesId = value.SpeciesId, amount = value.Amount, cost = value.Cost }).ToArray(),
                incomeOffers = definition.IncomeOffers.Select(value => new IncomeOffer
                {
                    id = value.Id,
                    displayName = value.DisplayName,
                    operation = (int)value.Operation,
                    value = value.Value.ToString(CultureInfo.InvariantCulture),
                    sources = (int)value.Sources,
                    firstShopVisit = value.FirstShopVisit,
                    shopCost = value.ShopCost,
                }).ToArray(),
                signedSpeciesOffers = definition.SignedSpeciesOffers.Select(value => new SignedSpeciesOffer
                {
                    id = value.Id,
                    familyId = value.FamilyId,
                    targetSpeciesId = value.TargetSpeciesId,
                    attributeId = value.AttributeId,
                    signedAmount = value.SignedAmount,
                    cost = value.Cost,
                    firstShopVisit = value.FirstShopVisit,
                }).ToArray(),
                reorderBaseRandomCost = definition.ReorderPricing.BaseRandomCost,
                reorderIncreaseNumerator = definition.ReorderPricing.IncreaseNumerator,
                reorderIncreaseDenominator = definition.ReorderPricing.IncreaseDenominator,
                chosenReorderMultiplier = definition.ReorderPricing.ChosenMultiplier,
            };
        }

        public bool TryRestore(out FieldDataJourneyDefinition definition, out string error)
        {
            definition = null;
            if (schemaVersion != 1 || version != FieldDataJourneyDefinition.CurrentVersion
                || string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(scenarioId)
                || string.IsNullOrWhiteSpace(selectedSpeciesId) || route == null
                || populationControls == null || incomeOffers == null || signedSpeciesOffers == null
                || string.IsNullOrWhiteSpace(fingerprint))
            {
                error = "The Field Data journey snapshot is incomplete or unsupported.";
                return false;
            }

            try
            {
                var restored = new FieldDataJourneyDefinition(
                    id,
                    scenarioId,
                    selectedSpeciesId,
                    route.Select(value => new FieldDataRouteNode(value.id, (JourneyNodeKind)value.kind, value.nextNodeId)),
                    populationControls.Select(value => new FieldDataPopulationControl(value.id, value.speciesId, value.amount, value.cost)),
                    incomeOffers.Select(value => new FieldDataIncomeOffer(
                        value.id,
                        value.displayName,
                        (FieldDataIncomeOperation)value.operation,
                        decimal.Parse(value.value, NumberStyles.Number, CultureInfo.InvariantCulture),
                        (FieldDataIncomeSource)value.sources,
                        value.firstShopVisit,
                        value.shopCost)),
                    signedSpeciesOffers.Select(value => new FieldDataSignedSpeciesOffer(
                        value.id, value.familyId, value.targetSpeciesId, value.attributeId,
                        value.signedAmount, value.cost, value.firstShopVisit)),
                    new FieldDataReorderPricing(reorderBaseRandomCost, reorderIncreaseNumerator,
                        reorderIncreaseDenominator, chosenReorderMultiplier));
                if (!string.Equals(restored.Fingerprint, fingerprint, StringComparison.Ordinal))
                {
                    error = "The Field Data journey snapshot fingerprint does not match its contents.";
                    return false;
                }
                definition = restored;
                error = string.Empty;
                return true;
            }
            catch (Exception exception) when (exception is ArgumentException || exception is FormatException || exception is OverflowException)
            {
                error = "The Field Data journey snapshot could not be restored: " + exception.Message;
                return false;
            }
        }

        [Serializable] public sealed class RouteNode { public string id; public int kind; public string nextNodeId; }
        [Serializable] public sealed class PopulationControl { public string id; public string speciesId; public int amount; public int cost; }
        [Serializable] public sealed class IncomeOffer
        { public string id; public string displayName; public int operation; public string value; public int sources; public int firstShopVisit; public int shopCost; }
        [Serializable] public sealed class SignedSpeciesOffer
        { public string id; public string familyId; public string targetSpeciesId; public string attributeId; public float signedAmount; public int cost; public int firstShopVisit; }
    }
}
