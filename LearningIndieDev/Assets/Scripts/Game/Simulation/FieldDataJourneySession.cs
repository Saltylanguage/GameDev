using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SaltyGame
{
    public enum FieldDataJourneyFlowState
    {
        Ready,
        Running,
        RoundDecision,
        Shop,
        Succeeded,
        Failed,
        Closed,
    }

    /// <summary>Coordinates one retained simulation run through the Field Data route.</summary>
    public sealed class FieldDataJourneySession : IDisposable
    {
        readonly SimulationManager simulationManager;
        readonly SpeciesProgression progression;
        readonly SpeciesSimulationRunner retainedRunner;
        readonly SimulationRunState retainedRun;
        readonly List<FieldDataIncomeModifierInstance> orderedIncomeModifiers =
            new List<FieldDataIncomeModifierInstance>();
        readonly List<FieldDataIncomeAcquisitionRecord> incomeAcquisitionHistory =
            new List<FieldDataIncomeAcquisitionRecord>();
        readonly List<FieldDataIncomeReorderReceipt> incomeReorderHistory =
            new List<FieldDataIncomeReorderReceipt>();
        readonly Dictionary<string, FieldDataIncomeReorderReceipt> reorderRequests =
            new Dictionary<string, FieldDataIncomeReorderReceipt>(StringComparer.Ordinal);
        FieldDataChosenReorderDraft chosenReorderDraft;
        readonly List<FieldDataSpeciesEffectInstance> speciesEffects = new List<FieldDataSpeciesEffectInstance>();
        readonly Dictionary<string, FieldDataSpeciesEffectInstance> speciesEffectPurchases =
            new Dictionary<string, FieldDataSpeciesEffectInstance>(StringComparer.Ordinal);
        readonly List<SpeciesUpgradeSnapshot> ordinaryRewardPool = new List<SpeciesUpgradeSnapshot>();
        readonly Dictionary<string, FieldDataRewardChoiceSet> rewardChoiceSets =
            new Dictionary<string, FieldDataRewardChoiceSet>(StringComparer.Ordinal);
        readonly Dictionary<int, FieldDataShopVisit> shopVisits = new Dictionary<int, FieldDataShopVisit>();
        readonly Dictionary<string, string> shopPurchaseRequests = new Dictionary<string, string>(StringComparer.Ordinal);
        readonly HashSet<string> purchasedShopOffers = new HashSet<string>(StringComparer.Ordinal);
        readonly Dictionary<string, FieldDataPopulationInterventionReceipt> populationInterventions =
            new Dictionary<string, FieldDataPopulationInterventionReceipt>(StringComparer.Ordinal);
        bool disposed;
        bool shopPurchasesStarted;
        int successfulPopulationInterventionCount;

        public event Action<FieldDataJourneySession> StateChanged;
        public event Action<FieldDataJourneySession> Updated;

        public FieldDataJourneySession(
            FieldDataJourneyDefinition definition,
            SimulationManager simulationManager,
            SpeciesProgression progression)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            this.simulationManager = simulationManager ?? throw new ArgumentNullException(nameof(simulationManager));
            this.progression = progression ?? throw new ArgumentNullException(nameof(progression));
            retainedRunner = simulationManager.Runner ?? throw new ArgumentException(
                "A prepared simulation runner is required.", nameof(simulationManager));
            retainedRun = simulationManager.Run ?? throw new ArgumentException(
                "A prepared simulation run is required.", nameof(simulationManager));

            var selectedSpecies = new SpeciesId(definition.SelectedSpeciesId);
            if (retainedRun.PlayerSpeciesId != selectedSpecies || progression.Definition.Id != selectedSpecies)
            {
                throw new ArgumentException(
                    "The prepared run and retained progression must use the selected species from the Field Data definition.",
                    nameof(simulationManager));
            }

            if (!retainedRun.SupportsContinuation
                || retainedRun.Status != SimulationRunStatus.Ready
                || retainedRun.PhaseLengthTicks != definition.RoundLength
                || retainedRun.TargetTicks != definition.RoundLength
                    * FieldDataJourneyDefinition.RoundsPerSimulation
                    * FieldDataJourneyDefinition.SimulationCount)
            {
                throw new ArgumentException(
                    "The prepared run must use the Field Data round length and complete 18-round route.",
                    nameof(simulationManager));
            }

            State = FieldDataJourneyFlowState.Ready;
            Ledger = new FieldDataJourneyLedger(Guid.NewGuid().ToString("N"), definition);
            simulationManager.PhaseBoundaryReached += HandlePhaseBoundaryReached;
            simulationManager.RunCompleted += HandleRunCompleted;
        }

        public FieldDataJourneyDefinition Definition { get; }
        public FieldDataJourneyLedger Ledger { get; }
        public FieldDataJourneyFlowState State { get; private set; }
        public string SettlementError { get; private set; }
        public SimulationRunState Run => retainedRun;
        public SpeciesProgression Progression => progression;
        public int GlobalRound => retainedRun.Tick / Definition.RoundLength;
        public int AvailableFieldData => State != FieldDataJourneyFlowState.Failed
            && State != FieldDataJourneyFlowState.Closed
                ? Ledger.Balance
                : 0;
        public int FieldDataLostOnFailure => Ledger.FieldDataLostOnFailure;
        public bool CanUseShopActions => !disposed && IsBoundToRetainedRun
            && State == FieldDataJourneyFlowState.Shop
            && GetCurrentNodeRewardChoices()?.Resolved == true;
        public bool CanUsePopulationControls
        {
            get
            {
                if (State == FieldDataJourneyFlowState.RoundDecision)
                {
                    var choices = GetCurrentRoundRewardChoices();
                    return choices == null || choices.Resolved;
                }
                return false;
            }
        }
        public IReadOnlyCollection<FieldDataPopulationInterventionReceipt> PopulationInterventions =>
            populationInterventions.Values;
        public IReadOnlyList<FieldDataSpeciesEffectInstance> SpeciesEffects => speciesEffects.AsReadOnly();
        public IReadOnlyCollection<FieldDataRewardChoiceSet> RewardChoiceSets => rewardChoiceSets.Values;
        public IReadOnlyCollection<FieldDataShopVisit> ShopVisits => shopVisits.Values;
        public IReadOnlyList<FieldDataIncomeAcquisitionRecord> IncomeAcquisitionHistory => incomeAcquisitionHistory.AsReadOnly();
        public IReadOnlyList<FieldDataIncomeReorderReceipt> IncomeReorderHistory => incomeReorderHistory.AsReadOnly();
        public IReadOnlyList<FieldDataIncomeModifierInstance> CurrentIncomeExecutionOrder => orderedIncomeModifiers.AsReadOnly();
        public FieldDataChosenReorderDraft ChosenReorderDraft => chosenReorderDraft;
        public int SimulationIndex => GlobalRound == 0
            ? 0
            : (GlobalRound - 1) / FieldDataJourneyDefinition.RoundsPerSimulation;

        public bool Begin()
        {
            return !disposed
                && State == FieldDataJourneyFlowState.Ready
                && IsBoundToRetainedRun
                && simulationManager.Start()
                && SetState(FieldDataJourneyFlowState.Running);
        }

        public bool SetOrdinaryRewardPool(IEnumerable<SpeciesUpgradeSnapshot> snapshots)
        {
            if (disposed || State != FieldDataJourneyFlowState.Ready || ordinaryRewardPool.Count > 0) return false;
            foreach (var snapshot in snapshots ?? Array.Empty<SpeciesUpgradeSnapshot>())
            {
                if (snapshot == null || snapshot.TargetSpecies != retainedRun.PlayerSpeciesId
                    || !snapshot.CanApplyAfterRunStart || snapshot.PopulationToAdd != 0)
                    continue;
                if (!ordinaryRewardPool.Exists(existing => string.Equals(existing.Id, snapshot.Id, StringComparison.Ordinal)))
                    ordinaryRewardPool.Add(snapshot);
            }
            return true;
        }

        public FieldDataRewardChoiceSet GetCurrentRoundRewardChoices()
        {
            if (State != FieldDataJourneyFlowState.RoundDecision || GlobalRound <= 0
                || GlobalRound % FieldDataJourneyDefinition.RoundsPerSimulation == 0)
                return null;
            return GetOrCreateRewardChoiceSet("round-" + GlobalRound, FieldDataIncomeSource.RoundReward);
        }

        public FieldDataRewardChoiceSet GetCurrentNodeRewardChoices()
        {
            if (State != FieldDataJourneyFlowState.Shop || retainedRun.Tick <= 0
                || retainedRun.Tick % (Definition.RoundLength * FieldDataJourneyDefinition.RoundsPerSimulation) != 0)
                return null;
            var node = retainedRun.Tick / (Definition.RoundLength * FieldDataJourneyDefinition.RoundsPerSimulation);
            if (node < 1 || node >= FieldDataJourneyDefinition.SimulationCount) return null;
            return GetOrCreateRewardChoiceSet("node-" + node, FieldDataIncomeSource.NodeReward);
        }

        public FieldDataShopVisit GetCurrentShopVisit()
        {
            if (State != FieldDataJourneyFlowState.Shop || retainedRun.Tick <= 0
                || retainedRun.Tick % (Definition.RoundLength * FieldDataJourneyDefinition.RoundsPerSimulation) != 0)
                return null;
            var visitNumber = retainedRun.Tick / (Definition.RoundLength * FieldDataJourneyDefinition.RoundsPerSimulation);
            if (shopVisits.TryGetValue(visitNumber, out var existing)) return existing;

            var candidates = CreateShopCandidates(visitNumber);
            ShuffleShopCandidates(candidates, CreateShopSeed(visitNumber, "offers"));
            var offers = new List<FieldDataShopOffer>();
            var includedFamilies = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var candidate in candidates)
            {
                if (offers.Count >= Definition.OffersPerShop) break;
                if (candidate.Kind == FieldDataShopOfferKind.SpeciesEffect
                    && includedFamilies.TryGetValue(candidate.FamilyId, out var sign)
                    && sign != Math.Sign(candidate.SignedSpeciesOffer.Value.SignedAmount))
                    continue;
                candidate.Locked = candidate.Kind == FieldDataShopOfferKind.SpeciesEffect
                    && IsSpeciesFamilyCapped(candidate.FamilyId);
                offers.Add(candidate);
                if (candidate.Kind == FieldDataShopOfferKind.SpeciesEffect)
                    includedFamilies[candidate.FamilyId] = Math.Sign(candidate.SignedSpeciesOffer.Value.SignedAmount);
            }
            existing = new FieldDataShopVisit(visitNumber, offers);
            shopVisits.Add(visitNumber, existing);
            return existing;
        }

        public bool TryBuyShopOffer(string requestId, string offerId, out string error)
        {
            error = null;
            requestId = requestId?.Trim();
            if (string.IsNullOrWhiteSpace(requestId) || string.IsNullOrWhiteSpace(offerId))
            {
                error = "A stable request ID and shop offer ID are required.";
                return false;
            }
            if (shopPurchaseRequests.TryGetValue(requestId, out var priorOfferId))
            {
                if (string.Equals(priorOfferId, offerId, StringComparison.Ordinal)) return true;
                error = "A shop-purchase request ID cannot be reused for a different offer.";
                return false;
            }

            var visit = GetCurrentShopVisit();
            if (disposed || visit == null || !CanUseShopActions)
            {
                error = "Resolve or skip the free node reward before using shop actions.";
                return false;
            }
            FieldDataShopOffer selected = null;
            foreach (var offer in visit.Offers)
                if (string.Equals(offer.Id, offerId, StringComparison.Ordinal)) selected = offer;
            var purchaseKey = visit.VisitNumber + ":" + offerId;
            if (selected == null || selected.Locked || selected.Purchased || purchasedShopOffers.Contains(purchaseKey))
            {
                error = "The selected shop offer is unavailable or already purchased.";
                return false;
            }
            if (AvailableFieldData < selected.Cost)
            {
                error = "There is not enough Field Data for this shop offer.";
                return false;
            }

            if (selected.Kind == FieldDataShopOfferKind.SpeciesEffect)
            {
                if (!TryBuySpeciesEffect(requestId, offerId, out _, out error)) return false;
            }
            else
            {
                var income = selected.IncomeOffer.Value;
                var instance = new FieldDataIncomeModifierInstance(
                    "shop-" + visit.VisitNumber + "-" + requestId,
                    income.Id,
                    income.Operation,
                    income.Value);
                if (!Ledger.TrySpend(selected.Cost, out _, out _, out error)) return false;
                if (!TryAddIncomeModifier(instance, out error, "shop-" + visit.VisitNumber))
                    throw new InvalidOperationException("A validated shop income offer could not be appended: " + error);
                Updated?.Invoke(this);
            }
            selected.Purchased = true;
            purchasedShopOffers.Add(purchaseKey);
            shopPurchaseRequests.Add(requestId, offerId);
            shopPurchasesStarted = true;
            Updated?.Invoke(this);
            return true;
        }

        public bool TryRerollCappedShopOffer(
            string offerId,
            out FieldDataShopOffer replacement,
            out string error)
        {
            replacement = null;
            error = null;
            var visit = GetCurrentShopVisit();
            if (disposed || visit == null || !CanUseShopActions || string.IsNullOrWhiteSpace(offerId))
            {
                error = "There is no active shop visit or capped offer to reroll.";
                return false;
            }
            var index = -1;
            for (var i = 0; i < visit.MutableOffers.Count; i++)
                if (string.Equals(visit.MutableOffers[i].Id, offerId, StringComparison.Ordinal)) index = i;
            if (index < 0 || !visit.MutableOffers[index].Locked
                || visit.RerolledOfferIds.Contains(offerId))
            {
                error = "Only a capped locked offer can be rerolled once per visit.";
                return false;
            }

            visit.RerolledOfferIds.Add(offerId);
            var current = visit.MutableOffers[index];
            visit.MutableOffers.RemoveAt(index);
            var eligible = new List<FieldDataShopOffer>();
            foreach (var candidate in CreateShopCandidates(visit.VisitNumber))
            {
                if (candidate.Kind == FieldDataShopOfferKind.SpeciesEffect && IsSpeciesFamilyCapped(candidate.FamilyId))
                    continue;
                if (IsConflictingShopOffer(candidate, visit.MutableOffers)) continue;
                var duplicate = false;
                foreach (var existing in visit.MutableOffers)
                    if (string.Equals(existing.Id, candidate.Id, StringComparison.Ordinal)) duplicate = true;
                if (!duplicate) eligible.Add(candidate);
            }
            if (eligible.Count > 0)
            {
                ShuffleShopCandidates(eligible, CreateShopSeed(visit.VisitNumber,
                    "reroll|" + offerId + "|" + visit.RerollOrdinal));
                replacement = eligible[0];
                visit.MutableOffers.Insert(index, replacement);
            }
            visit.RerollOrdinal++;
            Updated?.Invoke(this);
            return true;
        }

        public bool TryGetCurrentReorderCosts(out int randomCost, out int chosenCost, out string error)
        {
            randomCost = 0;
            chosenCost = 0;
            error = null;
            var visit = GetCurrentShopVisit();
            if (visit == null)
            {
                error = "Reordering is only available during an active shop visit.";
                return false;
            }
            try
            {
                var raw = RawRandomReorderPrice(visit.ReorderPriceStep);
                randomCost = checked((int)decimal.Ceiling(raw));
                chosenCost = checked(randomCost * Definition.ReorderPricing.ChosenMultiplier);
                return true;
            }
            catch (OverflowException)
            {
                error = "The reorder price is outside the supported Field Data range.";
                return false;
            }
        }

        public bool TryBuyRandomIncomeReorder(
            string requestId,
            out FieldDataIncomeReorderReceipt receipt,
            out string error)
        {
            receipt = null;
            error = null;
            requestId = requestId?.Trim();
            if (string.IsNullOrWhiteSpace(requestId))
            {
                error = "A stable reorder request ID is required.";
                return false;
            }
            if (reorderRequests.TryGetValue(requestId, out receipt))
            {
                if (receipt.Kind == FieldDataIncomeReorderKind.Random) return true;
                receipt = null;
                error = "A reorder request ID cannot be reused for a different reorder kind.";
                return false;
            }
            var visit = GetCurrentShopVisit();
            if (disposed || visit == null || !CanUseShopActions || orderedIncomeModifiers.Count < 2)
            {
                error = "A random reorder requires an active shop and at least two income modifiers.";
                return false;
            }
            if (!TryGetCurrentReorderCosts(out var cost, out _, out error)) return false;
            if (AvailableFieldData < cost)
            {
                error = "There is not enough Field Data for a random reorder.";
                return false;
            }

            var previous = CurrentIncomeOrderIds();
            var next = new List<string>(previous);
            ShuffleIncomeOrder(next, CreateShopSeed(visit.VisitNumber, "income-random-reorder|" + visit.ReorderPriceStep));
            if (OrdersEqual(previous, next))
            {
                var first = next[0];
                next.RemoveAt(0);
                next.Insert(next.Count, first);
            }
            if (!Ledger.TrySpend(cost, out _, out _, out error)) return false;
            ApplyIncomeOrder(next);
            receipt = new FieldDataIncomeReorderReceipt(requestId, visit.VisitNumber,
                FieldDataIncomeReorderKind.Random, visit.ReorderPriceStep, cost, previous, next);
            CompleteIncomeReorder(visit, receipt);
            return true;
        }

        public bool TryCreateChosenIncomeReorderDraft(out FieldDataChosenReorderDraft draft, out string error)
        {
            draft = null;
            error = null;
            var visit = GetCurrentShopVisit();
            if (disposed || visit == null || !CanUseShopActions || orderedIncomeModifiers.Count < 2)
            {
                error = "A chosen reorder requires an active shop and at least two income modifiers.";
                return false;
            }
            if (!TryGetCurrentReorderCosts(out _, out var cost, out error)) return false;
            draft = new FieldDataChosenReorderDraft(visit.VisitNumber, visit.ReorderPriceStep,
                cost, CurrentIncomeOrderIds());
            chosenReorderDraft = draft;
            Updated?.Invoke(this);
            return true;
        }

        public bool TryConfirmChosenIncomeReorder(
            string requestId,
            IEnumerable<string> requestedOrder,
            out FieldDataIncomeReorderReceipt receipt,
            out string error)
        {
            receipt = null;
            error = null;
            requestId = requestId?.Trim();
            if (string.IsNullOrWhiteSpace(requestId) || requestedOrder == null)
            {
                error = "A stable reorder request ID and full ordered instance list are required.";
                return false;
            }
            var requested = new List<string>(requestedOrder);
            if (reorderRequests.TryGetValue(requestId, out receipt))
            {
                if (receipt.Kind == FieldDataIncomeReorderKind.Chosen && OrdersEqual(receipt.NextOrder, requested)) return true;
                receipt = null;
                error = "A reorder request ID cannot be reused for a different order or reorder kind.";
                return false;
            }
            var visit = GetCurrentShopVisit();
            if (disposed || visit == null || !CanUseShopActions || chosenReorderDraft == null
                || chosenReorderDraft.ShopVisit != visit.VisitNumber
                || chosenReorderDraft.PriceStep != visit.ReorderPriceStep
                || !OrdersEqual(chosenReorderDraft.CurrentOrder, CurrentIncomeOrderIds()))
            {
                error = "The chosen reorder draft is stale or unavailable.";
                return false;
            }
            if (!IsExactIncomePermutation(requested))
            {
                error = "Chosen reorder must contain every current income instance exactly once.";
                return false;
            }
            var previous = CurrentIncomeOrderIds();
            if (OrdersEqual(previous, requested))
            {
                error = "Chosen reorder must change the execution order.";
                return false;
            }
            if (!TryGetCurrentReorderCosts(out _, out var cost, out error)) return false;
            if (AvailableFieldData < cost)
            {
                error = "There is not enough Field Data for the chosen reorder.";
                return false;
            }
            if (!Ledger.TrySpend(cost, out _, out _, out error)) return false;
            ApplyIncomeOrder(requested);
            receipt = new FieldDataIncomeReorderReceipt(requestId, visit.VisitNumber,
                FieldDataIncomeReorderKind.Chosen, visit.ReorderPriceStep, cost, previous, requested);
            chosenReorderDraft = null;
            CompleteIncomeReorder(visit, receipt);
            return true;
        }

        public bool CancelChosenIncomeReorderDraft()
        {
            if (chosenReorderDraft == null) return false;
            chosenReorderDraft = null;
            Updated?.Invoke(this);
            return true;
        }

        public bool ResolveRewardChoice(string sourceId, string choiceId, out string error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(sourceId) || string.IsNullOrWhiteSpace(choiceId))
            {
                error = "Reward source and choice IDs are required.";
                return false;
            }
            if (!rewardChoiceSets.TryGetValue(sourceId, out var set))
            {
                error = "The reward offer has not been generated for this decision.";
                return false;
            }
            if (set.Resolved)
                return string.Equals(set.SelectedChoiceId, choiceId, StringComparison.Ordinal);
            if ((sourceId.StartsWith("round-", StringComparison.Ordinal)
                    && (State != FieldDataJourneyFlowState.RoundDecision
                        || !string.Equals(sourceId, "round-" + GlobalRound, StringComparison.Ordinal)))
                || (sourceId.StartsWith("node-", StringComparison.Ordinal)
                    && (State != FieldDataJourneyFlowState.Shop
                        || !string.Equals(sourceId, "node-" + (retainedRun.Tick
                            / (Definition.RoundLength * FieldDataJourneyDefinition.RoundsPerSimulation)), StringComparison.Ordinal))))
            {
                error = "This reward decision is no longer active.";
                return false;
            }

            FieldDataRewardChoice selected = null;
            foreach (var choice in set.Choices)
                if (string.Equals(choice.Id, choiceId, StringComparison.Ordinal)) selected = choice;
            if (choiceId != "skip" && selected == null)
            {
                error = "The selected reward is not in this offer.";
                return false;
            }
            if (selected?.IncomeOffer is FieldDataIncomeOffer income)
            {
                var instance = new FieldDataIncomeModifierInstance(
                    "reward-" + sourceId + "-" + income.Id,
                    income.Id,
                    income.Operation,
                    income.Value);
                if (!TryAddIncomeModifier(instance, out error, sourceId)) return false;
            }
            else if (selected?.OrdinaryUpgrade != null
                && !progression.TryApplyFieldDataReward(selected.OrdinaryUpgrade))
            {
                error = "The ordinary species upgrade is no longer eligible.";
                return false;
            }

            if (!set.TryResolve(choiceId))
            {
                error = "The reward choice could not be resolved.";
                return false;
            }
            Updated?.Invoke(this);
            return true;
        }

        public bool Continue()
        {
            var requiredChoice = State == FieldDataJourneyFlowState.RoundDecision
                ? GetCurrentRoundRewardChoices()
                : State == FieldDataJourneyFlowState.Shop
                    ? GetCurrentNodeRewardChoices()
                    : null;
            if (disposed
                || (State != FieldDataJourneyFlowState.RoundDecision && State != FieldDataJourneyFlowState.Shop)
                || !string.IsNullOrEmpty(SettlementError)
                || !IsBoundToRetainedRun
                || retainedRun.Status != SimulationRunStatus.AwaitingDecision
                || retainedRun.Tick % Definition.RoundLength != 0
                || GlobalRound >= FieldDataJourneyDefinition.RoundsPerSimulation
                    * FieldDataJourneyDefinition.SimulationCount
                || (requiredChoice != null && !requiredChoice.Resolved))
            {
                return false;
            }

            var continued = false;
            if (speciesEffects.Count > 0 || progression.AppliedRunUpgrades.Count > retainedRun.UpgradeLoadout.Count)
            {
                if (!FieldDataSpeciesEffects.TryResolve(
                        progression.CurrentRules,
                        progression.PreContactAvoidanceChance,
                        speciesEffects,
                        out var effectiveRules,
                        out var avoidance,
                        out var effectsFingerprint,
                        out var resolveError))
                {
                    SettlementError = resolveError;
                    return false;
                }
                continued = simulationManager.ContinueWithFieldDataSpeciesRules(
                    retainedRun.PlayerSpeciesId, effectiveRules, avoidance, effectsFingerprint,
                    progression.AppliedRunUpgrades);
            }
            else
            {
                continued = simulationManager.ContinueWithoutUpgrade();
            }
            if (!continued)
            {
                return false;
            }

            shopPurchasesStarted = false;
            return SetState(FieldDataJourneyFlowState.Running);
        }

        public bool BeginShopPurchases()
        {
            if (disposed || State != FieldDataJourneyFlowState.Shop || shopPurchasesStarted)
            {
                return false;
            }

            shopPurchasesStarted = true;
            Updated?.Invoke(this);
            return true;
        }

        public bool TryBuyPopulationControl(
            string requestId,
            FieldDataPopulationInterventionKind kind,
            out FieldDataPopulationInterventionReceipt receipt,
            out string error)
        {
            receipt = null;
            error = null;
            requestId = requestId?.Trim();
            if (string.IsNullOrWhiteSpace(requestId))
            {
                error = "A stable action request ID is required.";
                return false;
            }

            if (populationInterventions.TryGetValue(requestId, out receipt))
            {
                if (receipt.Kind == kind)
                {
                    return true;
                }

                receipt = null;
                error = "A population action request ID cannot be reused for a different control.";
                return false;
            }

            if (disposed || !CanUsePopulationControls || !IsBoundToRetainedRun
                || retainedRun.Status != SimulationRunStatus.AwaitingDecision)
            {
                error = !CanUsePopulationControls && State == FieldDataJourneyFlowState.RoundDecision
                    ? "Resolve or skip the free round reward before using population controls."
                    : "Population controls are only available between rounds within a simulation node.";
                return false;
            }

            var controlId = PopulationControlId(kind);
            if (controlId == null || !TryGetPopulationControl(controlId, out var control))
            {
                error = "The selected population control is not part of this Field Data definition.";
                return false;
            }

            var quantity = Math.Abs(control.Amount);
            var species = new SpeciesId(control.SpeciesId);
            var currentSnapshot = retainedRun.PopulationHistory[retainedRun.PopulationHistory.Count - 1];
            if (control.Amount < 0
                && string.Equals(control.SpeciesId, Definition.SelectedSpeciesId, StringComparison.Ordinal)
                && currentSnapshot.GetCount(species) <= quantity)
            {
                error = "The final member of the selected species cannot be removed.";
                return false;
            }

            if (AvailableFieldData < control.Cost)
            {
                error = "There is not enough Field Data for this purchase.";
                return false;
            }

            var seed = CreatePopulationInterventionSeed();
            if (!simulationManager.TryPrepareFieldDataPopulationChange(
                kind, seed, out var prepared, out error))
            {
                return false;
            }

            if (!IsBoundToRetainedRun || retainedRun.Status != SimulationRunStatus.AwaitingDecision
                || !ReferenceEquals(retainedRun.Cells, prepared.ExpectedCells)
                || !simulationManager.CommitFieldDataPopulationChange(prepared))
            {
                error = "The simulation changed before the population control could be committed.";
                return false;
            }

            if (!Ledger.TrySpend(control.Cost, out var balanceBefore, out var balanceAfter, out error))
            {
                // The session serializes these operations; this branch indicates a broken wallet invariant.
                throw new InvalidOperationException("The validated Field Data purchase could not debit its wallet: " + error);
            }

            receipt = new FieldDataPopulationInterventionReceipt(
                requestId,
                kind,
                species,
                quantity,
                control.Cost,
                balanceBefore,
                balanceAfter,
                prepared.Positions);
            populationInterventions.Add(requestId, receipt);
            successfulPopulationInterventionCount++;
            Updated?.Invoke(this);
            return true;
        }

        public bool TryAddIncomeModifier(FieldDataIncomeModifierInstance modifier, out string error, string sourceId = "unknown")
        {
            error = null;
            if (disposed || (State != FieldDataJourneyFlowState.RoundDecision
                    && State != FieldDataJourneyFlowState.Shop)
                || string.IsNullOrWhiteSpace(modifier.InstanceId)
                || orderedIncomeModifiers.Exists(existing =>
                    string.Equals(existing.InstanceId, modifier.InstanceId, StringComparison.Ordinal)))
            {
                error = "Income modifiers can only be acquired with a unique instance ID at a journey decision or shop.";
                return false;
            }

            if (!FieldDataIncome.TryCalculate(0, 0,
                new[] { modifier }, out _, out error))
            {
                return false;
            }

            orderedIncomeModifiers.Add(modifier);
            incomeAcquisitionHistory.Add(new FieldDataIncomeAcquisitionRecord(
                incomeAcquisitionHistory.Count + 1, modifier.InstanceId, modifier.OfferId, sourceId ?? string.Empty));
            return true;
        }

        public bool TryBuySpeciesEffect(
            string requestId,
            string offerId,
            out FieldDataSpeciesEffectInstance effect,
            out string error)
        {
            effect = null;
            error = null;
            requestId = requestId?.Trim();
            if (string.IsNullOrWhiteSpace(requestId) || string.IsNullOrWhiteSpace(offerId))
            {
                error = "A stable request ID and offer ID are required.";
                return false;
            }
            if (speciesEffectPurchases.TryGetValue(requestId, out effect))
            {
                if (string.Equals(effect.OfferId, offerId, StringComparison.Ordinal)) return true;
                effect = null;
                error = "A species-effect request ID cannot be reused for a different offer.";
                return false;
            }
            if (disposed || !CanUseShopActions)
            {
                error = "Resolve or skip the free node reward before buying shop species effects.";
                return false;
            }

            var shopVisit = retainedRun.Tick / (Definition.RoundLength * FieldDataJourneyDefinition.RoundsPerSimulation);
            foreach (var offer in Definition.SignedSpeciesOffers)
            {
                if (!string.Equals(offer.Id, offerId, StringComparison.Ordinal)
                    || offer.FirstShopVisit > shopVisit)
                    continue;

                var familyCount = 0;
                foreach (var owned in speciesEffects)
                    if (string.Equals(owned.FamilyId, offer.FamilyId, StringComparison.Ordinal)) familyCount++;
                if (familyCount >= FieldDataSpeciesEffects.FamilyCopyLimit)
                {
                    error = "This species-effect family has reached its shared three-copy journey cap.";
                    return false;
                }
                if (AvailableFieldData < offer.Cost)
                {
                    error = "There is not enough Field Data for this species effect.";
                    return false;
                }
                if (!Ledger.TrySpend(offer.Cost, out _, out _, out error)) return false;

                effect = new FieldDataSpeciesEffectInstance("effect-" + requestId, offer);
                speciesEffects.Add(effect);
                speciesEffectPurchases.Add(requestId, effect);
                Updated?.Invoke(this);
                return true;
            }

            error = "The selected species-effect offer is unavailable at this shop.";
            return false;
        }

        public bool Abandon()
        {
            if (disposed || IsTerminal || !IsBoundToRetainedRun)
            {
                return false;
            }

            return simulationManager.End() && State == FieldDataJourneyFlowState.Failed;
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            simulationManager.PhaseBoundaryReached -= HandlePhaseBoundaryReached;
            simulationManager.RunCompleted -= HandleRunCompleted;
            SetState(FieldDataJourneyFlowState.Closed);
            disposed = true;
        }

        bool IsTerminal => State == FieldDataJourneyFlowState.Succeeded
            || State == FieldDataJourneyFlowState.Failed
            || State == FieldDataJourneyFlowState.Closed;

        bool IsBoundToRetainedRun => ReferenceEquals(simulationManager.Runner, retainedRunner)
            && ReferenceEquals(simulationManager.Run, retainedRun);

        void HandlePhaseBoundaryReached(SimulationRunState run)
        {
            if (disposed || !IsBoundToRetainedRun || !ReferenceEquals(run, retainedRun)
                || State != FieldDataJourneyFlowState.Running
                || run.Status != SimulationRunStatus.AwaitingDecision
                || run.Tick <= 0
                || run.Tick % Definition.RoundLength != 0)
            {
                return;
            }

            if (run.Tick >= run.TargetTicks)
            {
                return;
            }

            var selectedPopulation = run.PopulationHistory[run.PopulationHistory.Count - 1]
                .GetCount(run.PlayerSpeciesId);
            if (selectedPopulation <= 0)
            {
                Ledger.FailAndForfeit();
                SetState(FieldDataJourneyFlowState.Failed);
                return;
            }

            var nextState = run.Tick % (Definition.RoundLength
                * FieldDataJourneyDefinition.RoundsPerSimulation) == 0
                ? FieldDataJourneyFlowState.Shop
                : FieldDataJourneyFlowState.RoundDecision;
            TrySettleCurrentRound(run);
            SetState(nextState);
        }

        void HandleRunCompleted(SimulationRunState run)
        {
            if (disposed || !IsBoundToRetainedRun || !ReferenceEquals(run, retainedRun) || IsTerminal)
            {
                return;
            }

            var population = run.PopulationHistory.Count == 0
                ? 0
                : run.PopulationHistory[run.PopulationHistory.Count - 1].GetCount(run.PlayerSpeciesId);
            var reachedSuccess = run.Tick == run.TargetTicks && population > 0;
            if (reachedSuccess && !TrySettleCurrentRound(run))
            {
                return;
            }

            if (!reachedSuccess)
            {
                Ledger.FailAndForfeit();
            }
            SetState(reachedSuccess ? FieldDataJourneyFlowState.Succeeded : FieldDataJourneyFlowState.Failed);
        }

        public bool TryRetrySettlementAtCurrentBoundary()
        {
            if (disposed || string.IsNullOrEmpty(SettlementError) || !IsBoundToRetainedRun
                || retainedRun.Tick <= 0 || retainedRun.Tick % Definition.RoundLength != 0
                || (retainedRun.Status != SimulationRunStatus.AwaitingDecision
                    && retainedRun.Status != SimulationRunStatus.Complete))
            {
                return false;
            }

            if (!TrySettleCurrentRound(retainedRun))
            {
                return false;
            }

            if (retainedRun.Tick == retainedRun.TargetTicks)
            {
                var finalPopulation = retainedRun.PopulationHistory[retainedRun.PopulationHistory.Count - 1]
                    .GetCount(retainedRun.PlayerSpeciesId);
                if (finalPopulation <= 0)
                {
                    Ledger.FailAndForfeit();
                    SetState(FieldDataJourneyFlowState.Failed);
                    return false;
                }

                SetState(FieldDataJourneyFlowState.Succeeded);
            }

            return true;
        }

        bool TrySettleCurrentRound(SimulationRunState run)
        {
            if (run == null || run.Tick <= 0 || run.Tick % Definition.RoundLength != 0
                || run.PopulationHistory.Count == 0
                || !ReferenceEquals(run, retainedRun))
            {
                SettlementError = "The run is not at a completed Field Data round boundary.";
                return false;
            }

            var population = run.PopulationHistory[run.PopulationHistory.Count - 1];
            if (!Ledger.TrySettleSuccessfulRound(
                run.Tick / Definition.RoundLength,
                run.Tick,
                population.Counts,
                orderedIncomeModifiers,
                out _,
                out _,
                out var error))
            {
                SettlementError = error;
                return false;
            }

            SettlementError = string.Empty;
            return true;
        }

        bool TryGetPopulationControl(string id, out FieldDataPopulationControl control)
        {
            foreach (var candidate in Definition.PopulationControls)
            {
                if (string.Equals(candidate.Id, id, StringComparison.Ordinal))
                {
                    control = candidate;
                    return true;
                }
            }

            control = default;
            return false;
        }

        List<FieldDataShopOffer> CreateShopCandidates(int visitNumber)
        {
            var candidates = new List<FieldDataShopOffer>();
            foreach (var species in Definition.SignedSpeciesOffers)
                if (species.FirstShopVisit <= visitNumber) candidates.Add(new FieldDataShopOffer(species));
            foreach (var income in Definition.IncomeOffers)
                if (income.FirstShopVisit <= visitNumber && income.ShopCost > 0)
                    candidates.Add(new FieldDataShopOffer(income));
            return candidates;
        }

        bool IsSpeciesFamilyCapped(string familyId)
        {
            var count = 0;
            foreach (var owned in speciesEffects)
                if (string.Equals(owned.FamilyId, familyId, StringComparison.Ordinal)) count++;
            return count >= FieldDataSpeciesEffects.FamilyCopyLimit;
        }

        static bool IsConflictingShopOffer(FieldDataShopOffer candidate, IReadOnlyList<FieldDataShopOffer> existing)
        {
            if (candidate.Kind != FieldDataShopOfferKind.SpeciesEffect) return false;
            var sign = Math.Sign(candidate.SignedSpeciesOffer.Value.SignedAmount);
            foreach (var offer in existing)
            {
                if (offer.Kind == FieldDataShopOfferKind.SpeciesEffect
                    && string.Equals(offer.FamilyId, candidate.FamilyId, StringComparison.Ordinal)
                    && Math.Sign(offer.SignedSpeciesOffer.Value.SignedAmount) != sign)
                    return true;
            }
            return false;
        }

        int CreateShopSeed(int visitNumber, string stream)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(
                    "field-data-shop-v1|" + Definition.Fingerprint + "|" + retainedRun.Seed
                    + "|" + visitNumber + "|" + stream));
                return BitConverter.ToInt32(bytes, 0);
            }
        }

        static void ShuffleShopCandidates(List<FieldDataShopOffer> candidates, int seed)
        {
            uint state = unchecked((uint)seed);
            if (state == 0) state = 0xA341316C;
            for (var i = candidates.Count - 1; i > 0; i--)
            {
                state ^= state << 13;
                state ^= state >> 17;
                state ^= state << 5;
                var index = (int)(state % (uint)(i + 1));
                var value = candidates[i];
                candidates[i] = candidates[index];
                candidates[index] = value;
            }
        }

        decimal RawRandomReorderPrice(int priceStep)
        {
            var pricing = Definition.ReorderPricing;
            decimal price = pricing.BaseRandomCost;
            for (var i = 0; i < priceStep; i++)
                price = checked(price * pricing.IncreaseNumerator / pricing.IncreaseDenominator);
            return price;
        }

        List<string> CurrentIncomeOrderIds()
        {
            var ids = new List<string>(orderedIncomeModifiers.Count);
            foreach (var modifier in orderedIncomeModifiers) ids.Add(modifier.InstanceId);
            return ids;
        }

        bool IsExactIncomePermutation(IReadOnlyList<string> requested)
        {
            if (requested.Count != orderedIncomeModifiers.Count) return false;
            var current = new HashSet<string>(CurrentIncomeOrderIds(), StringComparer.Ordinal);
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var id in requested)
                if (string.IsNullOrWhiteSpace(id) || !current.Contains(id) || !seen.Add(id)) return false;
            return seen.Count == current.Count;
        }

        void ApplyIncomeOrder(IReadOnlyList<string> requested)
        {
            var byId = new Dictionary<string, FieldDataIncomeModifierInstance>(StringComparer.Ordinal);
            foreach (var modifier in orderedIncomeModifiers) byId.Add(modifier.InstanceId, modifier);
            var next = new List<FieldDataIncomeModifierInstance>(requested.Count);
            foreach (var id in requested) next.Add(byId[id]);
            orderedIncomeModifiers.Clear();
            orderedIncomeModifiers.AddRange(next);
        }

        void CompleteIncomeReorder(FieldDataShopVisit visit, FieldDataIncomeReorderReceipt receipt)
        {
            visit.ReorderPriceStep++;
            incomeReorderHistory.Add(receipt);
            reorderRequests.Add(receipt.RequestId, receipt);
            shopPurchasesStarted = true;
            Updated?.Invoke(this);
        }

        static bool OrdersEqual(IReadOnlyList<string> left, IReadOnlyList<string> right)
        {
            if (left == null || right == null || left.Count != right.Count) return false;
            for (var i = 0; i < left.Count; i++)
                if (!string.Equals(left[i], right[i], StringComparison.Ordinal)) return false;
            return true;
        }

        static void ShuffleIncomeOrder(List<string> ids, int seed)
        {
            uint state = unchecked((uint)seed);
            if (state == 0) state = 0xC8013EA4;
            for (var i = ids.Count - 1; i > 0; i--)
            {
                state ^= state << 13;
                state ^= state >> 17;
                state ^= state << 5;
                var index = (int)(state % (uint)(i + 1));
                var value = ids[i];
                ids[i] = ids[index];
                ids[index] = value;
            }
        }

        FieldDataRewardChoiceSet GetOrCreateRewardChoiceSet(string sourceId, FieldDataIncomeSource source)
        {
            if (rewardChoiceSets.TryGetValue(sourceId, out var existing)) return existing;
            var candidates = new List<FieldDataRewardChoice>();
            foreach (var snapshot in ordinaryRewardPool)
            {
                if (IsOrdinaryRewardEligible(snapshot))
                    candidates.Add(new FieldDataRewardChoice("upgrade-" + snapshot.Id, snapshot.DisplayName, snapshot, null));
            }
            foreach (var income in Definition.IncomeOffers)
            {
                if (source == FieldDataIncomeSource.Shop
                    && (income.Sources & FieldDataIncomeSource.Shop) != 0)
                    candidates.Add(new FieldDataRewardChoice("income-" + income.Id, income.DisplayName, null, income));
            }

            ShuffleRewardCandidates(candidates, CreateRewardSeed(sourceId));
            if (candidates.Count > FieldDataJourneyDefinition.FreeChoicesPerReward)
                candidates.RemoveRange(FieldDataJourneyDefinition.FreeChoicesPerReward,
                    candidates.Count - FieldDataJourneyDefinition.FreeChoicesPerReward);
            var created = new FieldDataRewardChoiceSet(sourceId, candidates);
            rewardChoiceSets.Add(sourceId, created);
            return created;
        }

        bool IsOrdinaryRewardEligible(SpeciesUpgradeSnapshot snapshot)
        {
            if (snapshot == null || snapshot.TargetSpecies != retainedRun.PlayerSpeciesId
                || !snapshot.CanApplyAfterRunStart || snapshot.PopulationToAdd != 0)
                return false;
            var currentLevel = progression.GetUpgradeLevel(snapshot.Id);
            if (currentLevel >= SpeciesUpgradeCatalog.GetFieldDataRewardMaxLevel(snapshot.TargetSpecies, snapshot.Id)) return false;
            foreach (var prerequisite in snapshot.PrerequisiteUpgradeIds)
                if (progression.GetUpgradeLevel(prerequisite) == 0) return false;
            foreach (var excluded in snapshot.ExcludedUpgradeIds)
                if (progression.GetUpgradeLevel(excluded) > 0) return false;
            return true;
        }

        int CreateRewardSeed(string sourceId)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(
                    "field-data-reward-choice-v1|" + Definition.Fingerprint + "|"
                    + retainedRun.Seed + "|" + sourceId));
                return BitConverter.ToInt32(bytes, 0);
            }
        }

        static void ShuffleRewardCandidates(List<FieldDataRewardChoice> candidates, int seed)
        {
            uint state = unchecked((uint)seed);
            if (state == 0) state = 0x6D2B79F5;
            for (var i = candidates.Count - 1; i > 0; i--)
            {
                state ^= state << 13;
                state ^= state >> 17;
                state ^= state << 5;
                var index = (int)(state % (uint)(i + 1));
                var value = candidates[i];
                candidates[i] = candidates[index];
                candidates[index] = value;
            }
        }

        int CreatePopulationInterventionSeed()
        {
            unchecked
            {
                var value = retainedRun.Seed;
                value = value * 31 + retainedRun.Tick;
                value = value * 31 + successfulPopulationInterventionCount;
                return value ^ 0x4F1BBCDC;
            }
        }

        static string PopulationControlId(FieldDataPopulationInterventionKind kind)
        {
            switch (kind)
            {
                case FieldDataPopulationInterventionKind.AddHare: return "add-hares";
                case FieldDataPopulationInterventionKind.RemoveHare: return "remove-hares";
                case FieldDataPopulationInterventionKind.AddFox: return "add-foxes";
                case FieldDataPopulationInterventionKind.RemoveFox: return "remove-foxes";
                case FieldDataPopulationInterventionKind.AddPlants: return "add-plants";
                case FieldDataPopulationInterventionKind.RemovePlants: return "remove-plants";
                default: return null;
            }
        }

        public bool TryExitSuccessfulResults(out int discardedFieldData)
        {
            discardedFieldData = 0;
            if (disposed || State != FieldDataJourneyFlowState.Succeeded
                || !Ledger.TryDiscardSuccessfulRemainder(out discardedFieldData))
            {
                return false;
            }

            return SetState(FieldDataJourneyFlowState.Closed);
        }

        public bool TryExitFailedResults()
        {
            if (disposed || State != FieldDataJourneyFlowState.Failed)
            {
                return false;
            }

            return SetState(FieldDataJourneyFlowState.Closed);
        }

        bool SetState(FieldDataJourneyFlowState next)
        {
            if (State == FieldDataJourneyFlowState.Closed || disposed)
            {
                return false;
            }

            State = next;
            StateChanged?.Invoke(this);
            return true;
        }
    }
}
