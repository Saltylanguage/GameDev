using NUnit.Framework;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace SaltyGame.EditorTests
{
    public sealed class FieldDataJourneySessionTests
    {
        const string DefinitionPath = "Assets/Data/TestData/FieldDataStarterDefinition.asset";
        const string ScenarioPath = "Assets/Data/ProductionData/CellularSimulation/Scenarios/ForestEdge.asset";

        [TestCase(SpeciesUpgradeCatalog.ToughHideId)]
        [TestCase(SpeciesUpgradeCatalog.EfficientDigestionId)]
        [TestCase(SpeciesUpgradeCatalog.SeedDispersalId)]
        [TestCase(SpeciesUpgradeCatalog.CrowdingToleranceId)]
        [TestCase(SpeciesUpgradeCatalog.ReproductiveDriveId)]
        [TestCase(SpeciesUpgradeCatalog.KeenSensesId)]
        public void FieldDataHareRewardsRespectTheirExistingLevelCaps(string upgradeId)
        {
            var (definition, manager, progression) = CreateSessionParts();
            var upgrade = SpeciesUpgradeCatalog.Create(upgradeId).CreateSnapshot(new SpeciesId("hare"));
            var cap = SpeciesUpgradeCatalog.GetMaxLevel(upgradeId);
            for (var level = 1; level <= cap; level++)
                Assert.That(progression.TryApplyFieldDataReward(upgrade), Is.True, "Level " + level);
            var cappedRules = progression.CurrentRules;
            Assert.That(progression.TryApplyFieldDataReward(upgrade), Is.False);
            Assert.That(progression.CurrentRules, Is.SameAs(cappedRules));
            Assert.That(progression.GetUpgradeLevel(upgradeId), Is.EqualTo(cap));
            using (var session = new FieldDataJourneySession(definition, manager, progression))
            {
                Assert.That(session.SetOrdinaryRewardPool(new[] { upgrade }), Is.True);
                Assert.That(session.Begin(), Is.True);
                manager.Advance(10.1f);
                var choices = session.GetCurrentRoundRewardChoices();
                Assert.That(choices.Choices, Is.Empty, "Capped skills must not be offered again.");
                var balance = session.Ledger.Balance;
                Assert.That(session.ResolveRewardChoice(choices.SourceId, "upgrade-" + upgradeId, out _), Is.False);
                Assert.That(session.Ledger.Balance, Is.EqualTo(balance));
                Assert.That(session.ResolveRewardChoice(choices.SourceId, "skip", out var error), Is.True, error);
                Assert.That(session.Continue(), Is.True, "An empty pool must remain skippable.");
            }
        }

        [Test]
        public void ChoosingEveryFreeRewardKeepsHareSkillsAvailableAfterBothShops()
        {
            var (definition, manager, progression) = CreateSessionParts();
            using (var session = new FieldDataJourneySession(definition, manager, progression))
            {
                var pool = SpeciesUpgradeCatalog.ResolveFieldDataOrdinaryRewardPool(new SpeciesId("hare"), null);
                Assert.That(session.SetOrdinaryRewardPool(pool), Is.True);
                Assert.That(session.Begin(), Is.True);
                for (var round = 1; round <= 17; round++)
                {
                    manager.Advance(10.1f);
                    Assert.That(session.GlobalRound, Is.EqualTo(round));
                    var choices = session.GetCurrentRoundRewardChoices() ?? session.GetCurrentNodeRewardChoices();
                    Assert.That(choices, Is.Not.Null);
                    Assert.That(choices.Choices, Is.Not.Empty,
                        $"Round {round} must still offer Hare skills after choosing every earlier free reward.");
                    var beforeReward = session.Ledger.Balance;
                    Assert.That(session.ResolveRewardChoice(choices.SourceId, choices.Choices[0].Id, out var error), Is.True, error);
                    Assert.That(session.Ledger.Balance, Is.EqualTo(beforeReward), "Free mutations must not charge Field Data.");
                    if (session.State == FieldDataJourneyFlowState.Shop)
                    {
                        var offer = session.GetCurrentShopVisit().Offers.First(item => !item.Locked);
                        Assert.That(session.TryBuyShopOffer("shop-" + round, offer.Id, out error), Is.True, error);
                    }
                    Assert.That(session.Continue(), Is.True);
                }
                manager.Advance(10.1f);
                Assert.That(session.State, Is.EqualTo(FieldDataJourneyFlowState.Succeeded));
                Assert.That(progression.AppliedRunUpgrades.Count, Is.EqualTo(17));
            }
        }

        [Test]
        public void RoutePausesAtBothShopsAndCompletesOnTheSameRun()
        {
            var activeGenome = new GenomeSimulationSnapshot(new[]
            {
                new SpeciesGenomeSnapshot(new SpeciesId("hare"), new[] { "hare.guard" }),
            });
            var (definition, manager, progression) = CreateSessionParts(activeGenomeSnapshot: activeGenome);
            progression.AddCurrency(77);
            var run = manager.Run;
            var session = new FieldDataJourneySession(definition, manager, progression);
            var observedShop = false;
            var observedRoundDecision = false;
            session.StateChanged += changed =>
            {
                observedShop |= changed.State == FieldDataJourneyFlowState.Shop;
                observedRoundDecision |= changed.State == FieldDataJourneyFlowState.RoundDecision;
            };
            using (session)
            {
                Assert.That(session.Begin(), Is.True);
                Assert.That(session.Run, Is.SameAs(run));
                Assert.That(session.Progression, Is.SameAs(progression));

                for (var boundary = 1; boundary <= 17; boundary++)
                {
                    // Leave a small timing margin: 100 repeated float subtractions of 0.1f
                    // can otherwise settle at 99 ticks when the input is exactly 10 seconds.
                    manager.Advance(10.1f);
                    Assert.That(run.Tick, Is.EqualTo(boundary * 100));
                    Assert.That(session.State, Is.EqualTo(boundary % 6 == 0
                        ? FieldDataJourneyFlowState.Shop
                        : FieldDataJourneyFlowState.RoundDecision));
                    Assert.That(session.AvailableFieldData,
                        Is.EqualTo(boundary < 6 ? 0 : session.Ledger.Balance),
                        "Field Data becomes available only after the first six-round simulation.");
                    if (boundary == 6 || boundary == 12)
                    {
                        var pausedBoard = run.Cells;
                        var pausedHistory = run.PopulationHistory.Count;
                        var pausedGenome = run.ActiveGenomeSnapshot;
                        manager.Advance(10f);
                        Assert.That(run.Cells, Is.SameAs(pausedBoard));
                        Assert.That(run.PopulationHistory.Count, Is.EqualTo(pausedHistory));
                        Assert.That(run.ActiveGenomeSnapshot, Is.SameAs(pausedGenome));
                        Assert.That(pausedGenome.Fingerprint, Is.EqualTo(activeGenome.Fingerprint));
                    }
                    manager.Advance(10f);
                    Assert.That(run.Tick, Is.EqualTo(boundary * 100),
                        "A round decision or shop must not advance simulation time.");
                    Assert.That(ContinueSession(session), Is.True);
                    Assert.That(session.State, Is.EqualTo(FieldDataJourneyFlowState.Running));
                    Assert.That(session.Run, Is.SameAs(run));
                    Assert.That(manager.Run, Is.SameAs(run));
                    Assert.That(run.Seed, Is.EqualTo(10100));
                    Assert.That(run.PopulationHistory.Count, Is.EqualTo(run.Tick + 1));
                }

                manager.Advance(10.1f);
                Assert.That(run.Tick, Is.EqualTo(1800));
                Assert.That(session.State, Is.EqualTo(FieldDataJourneyFlowState.Succeeded));
                Assert.That(observedShop, Is.True);
                Assert.That(observedRoundDecision, Is.True);
                Assert.That(session.Continue(), Is.False);
                Assert.That(session.Begin(), Is.False);
                var unbankedBalance = session.Ledger.Balance;
                Assert.That(unbankedBalance, Is.GreaterThan(0));
                Assert.That(session.TryExitSuccessfulResults(out var discarded), Is.True);
                Assert.That(discarded, Is.EqualTo(unbankedBalance));
                Assert.That(session.Ledger.Balance, Is.Zero);
                Assert.That(progression.Currency, Is.EqualTo(77));
            }

            Assert.That(session.State, Is.EqualTo(FieldDataJourneyFlowState.Closed));
        }

        [Test]
        public void AbandonmentFailsAndDisposeUnsubscribesTheSession()
        {
            var (definition, manager, progression) = CreateSessionParts();
            progression.AddCurrency(77);
            var session = new FieldDataJourneySession(definition, manager, progression);
            Assert.That(session.Begin(), Is.True);
            manager.Advance(10.1f);
            Assert.That(session.Ledger.Balance, Is.EqualTo(20));
            Assert.That(session.Abandon(), Is.True);
            Assert.That(session.State, Is.EqualTo(FieldDataJourneyFlowState.Failed));
            Assert.That(session.FieldDataLostOnFailure, Is.EqualTo(20));
            Assert.That(session.Ledger.Balance, Is.Zero);
            Assert.That(progression.Currency, Is.EqualTo(77));
            Assert.That(session.TryExitFailedResults(), Is.True);
            Assert.That(session.State, Is.EqualTo(FieldDataJourneyFlowState.Closed));
            session.Dispose();
            Assert.That(session.State, Is.EqualTo(FieldDataJourneyFlowState.Closed));
            Assert.That(session.Continue(), Is.False);
            Assert.That(session.Abandon(), Is.False);
        }

        [TestCase(1)]
        [TestCase(6)]
        [TestCase(18)]
        public void ExtinctionAtRoundBoundariesFailsBeforeThatRoundsAward(int extinctRound)
        {
            var (definition, manager, progression) = CreateSessionParts(
                stableHare: false, hareCount: 1, plantCount: 0, extinctionAfterSteps: extinctRound);
            var session = new FieldDataJourneySession(definition, manager, progression);
            using (session)
            {
                Assert.That(session.Begin(), Is.True);
                for (var round = 1; round <= extinctRound; round++)
                {
                    manager.Advance(10.1f);
                    Assert.That(session.Run.Tick, Is.EqualTo(round * 100));
                    if (round < extinctRound)
                        Assert.That(ContinueSession(session), Is.True);
                }

                Assert.That(session.State, Is.EqualTo(FieldDataJourneyFlowState.Failed));
                Assert.That(session.Ledger.Awards.Count(award => award.Kind == FieldDataAwardKind.PopulationPayout),
                    Is.EqualTo(extinctRound - 1));
                Assert.That(session.Ledger.Awards.Any(award => award.GlobalRound == extinctRound), Is.False);
                Assert.That(session.Ledger.Awards.Count(award => award.Kind == FieldDataAwardKind.NodeClear),
                    Is.EqualTo((extinctRound - 1) / FieldDataJourneyDefinition.RoundsPerSimulation));
            }
        }

        [Test]
        public void ConstructorRejectsRunsWithTheWrongRouteLength()
        {
            var asset = AssetDatabase.LoadAssetAtPath<FieldDataJourneyDefinitionAsset>(DefinitionPath);
            Assert.That(asset.TryResolve(out var definition, out var error), Is.True, error);
            var manager = new SimulationManager();
            var scenario = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(ScenarioPath).CreateRuntimeData();
            var hareSpecies = new SpeciesId("hare");
            var shortRun = new SimulationRunState(
                SpeciesInitialGridFactory.Create(scenario, 10100), hareSpecies, 10100,
                600 * scenario.StepInterval, 600);
            shortRun.ConfigureContinuousPhases(100);
            manager.SetRunner(new SpeciesSimulationRunner(shortRun, scenario));
            var progression = new SpeciesProgression(new SpeciesDefinition(
                hareSpecies, scenario.SpeciesRules[hareSpecies]));

            Assert.Throws<System.ArgumentException>(() =>
                new FieldDataJourneySession(definition, manager, progression));
        }

        [Test]
        public void ReplacedRunnerCannotStartTheCapturedJourney()
        {
            var (definition, manager, progression) = CreateSessionParts();
            var session = new FieldDataJourneySession(definition, manager, progression);
            var data = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(ScenarioPath)
                .CreateRuntimeData().WithRunTicks(1800, 0.1f);
            manager.SetRunner(new SpeciesSimulationRunner(manager.Run, data));

            Assert.That(session.Begin(), Is.False);
            Assert.That(session.State, Is.EqualTo(FieldDataJourneyFlowState.Ready));
            session.Dispose();
        }

        [Test]
        public void SignedSpeciesEffectsAreChargedOnceCappedAcrossSignsAndAppliedAtNextSimulation()
        {
            var (definition, manager, progression) = CreateSessionParts();
            var session = new FieldDataJourneySession(definition, manager, progression);
            using (session)
            {
                Assert.That(session.Begin(), Is.True);
                for (var round = 1; round <= 6; round++)
                {
                    manager.Advance(10.1f);
                    if (round < 6) Assert.That(ContinueSession(session), Is.True);
                }
                Assert.That(session.State, Is.EqualTo(FieldDataJourneyFlowState.Shop));
                var species = new SpeciesId("hare");
                Assert.That(manager.TryGetCurrentRules(species, out var before), Is.True);
                Assert.That(before.MovementSpeed, Is.Zero);

                var nodeChoice = session.GetCurrentNodeRewardChoices();
                Assert.That(session.ResolveRewardChoice(nodeChoice.SourceId, "skip", out _), Is.True);

                var ordinary = SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.FasterMovementId);
                for (var level = 0; level < SpeciesUpgradeCatalog.FasterMovementMaxLevel; level++)
                    Assert.That(progression.TryApplyFreeUpgrade(ordinary), Is.True);
                Assert.That(progression.TryApplyFreeUpgrade(ordinary), Is.False);
                var ordinaryMovement = progression.CurrentRules.MovementSpeed;

                var grid = session.Run.Cells;
                var creatureIndex = Enumerable.Range(0, grid.Width * grid.Height)
                    .First(index => grid.GetCell(index % grid.Width, index / grid.Width).IsCreature);
                var creatureX = creatureIndex % grid.Width;
                var creatureY = creatureIndex / grid.Width;
                var creature = grid.GetCell(creatureX, creatureY);
                grid.SetCell(creatureX, creatureY, creature.WithEntity(
                    species, creature.Health, 17, 23, 2, 3f)
                    .WithAttackCooldown(4).WithReproductionCooldown(7)
                    .WithBehaviorState(SpeciesBehaviorState.Sleeping, 3));
                var cellsBeforeContinuation = grid.Copy();

                Assert.That(session.TryBuySpeciesEffect("effect-1", "hare-long-stride-plus", out var first, out var error), Is.True, error);
                Assert.That(session.TryBuySpeciesEffect("effect-1", "hare-long-stride-plus", out var retried, out error), Is.True, error);
                Assert.That(retried, Is.SameAs(first));
                Assert.That(session.TryBuySpeciesEffect("effect-2", "hare-long-stride-plus", out _, out error), Is.True, error);
                Assert.That(session.TryBuySpeciesEffect("effect-3", "hare-long-stride-minus", out _, out error), Is.True, error);
                Assert.That(session.TryBuySpeciesEffect("effect-4", "hare-long-stride-minus", out _, out error), Is.False);
                Assert.That(session.SpeciesEffects.Count, Is.EqualTo(3));
                Assert.That(progression.GetUpgradeLevel(ordinary.Id),
                    Is.EqualTo(SpeciesUpgradeCatalog.FasterMovementMaxLevel),
                    "The shop family cap is separate from ordinary upgrade levels.");
                Assert.That(manager.TryGetCurrentRules(species, out var unchanged), Is.True);
                Assert.That(unchanged.MovementSpeed, Is.Zero, "Shop purchases begin at the next simulation, not during the paused shop.");
                Assert.That(ContinueSession(session), Is.True);
                Assert.That(manager.TryGetCurrentRules(species, out var applied), Is.True);
                Assert.That(applied.MovementSpeed, Is.EqualTo(ordinaryMovement + 1.5f),
                    "Two positive copies and one negative copy resolve on top of ordinary progression.");
                Assert.That(progression.CurrentRules.MovementSpeed, Is.EqualTo(ordinaryMovement),
                    "The shop layer must not mutate ordinary progression storage.");
                for (var y = 0; y < grid.Height; y++)
                    for (var x = 0; x < grid.Width; x++)
                        Assert.That(session.Run.Cells.GetCell(x, y), Is.EqualTo(cellsBeforeContinuation.GetCell(x, y)),
                            "Applying shop rules must preserve energy, cooldowns, identities and all other cell state.");

                for (var round = 7; round <= 12; round++)
                {
                    manager.Advance(10.1f);
                    if (round < 12) Assert.That(ContinueSession(session), Is.True);
                }
                Assert.That(session.State, Is.EqualTo(FieldDataJourneyFlowState.Shop));
                Assert.That(ContinueSession(session), Is.True);
                Assert.That(manager.TryGetCurrentRules(species, out var retained), Is.True);
                Assert.That(retained.MovementSpeed, Is.EqualTo(ordinaryMovement + 1.5f),
                    "Later nodes retain the overlay without applying its amount again.");
            }

            var (nextDefinition, nextManager, nextProgression) = CreateSessionParts();
            using (var next = new FieldDataJourneySession(nextDefinition, nextManager, nextProgression))
                Assert.That(next.SpeciesEffects, Is.Empty, "A fresh journey cannot inherit shop effects.");
        }

        [Test]
        public void RoundRewardsAreSeededFreeSpeciesChoicesWithoutIncomeModifiers()
        {
            var (definition, manager, progression) = CreateSessionParts();
            var hare = new SpeciesId("hare");
            var ordinary = SpeciesUpgradeCatalog.ResolveFieldDataOrdinaryRewardPool(hare, null);
            var session = new FieldDataJourneySession(definition, manager, progression);
            Assert.That(session.SetOrdinaryRewardPool(ordinary), Is.True);
            using (session)
            {
                Assert.That(session.Begin(), Is.True);
                manager.Advance(10.1f);
                var choices = session.GetCurrentRoundRewardChoices();
                Assert.That(choices, Is.Not.Null);
                Assert.That(choices.Choices.Count, Is.EqualTo(3));
                Assert.That(choices.Choices.All(choice => choice.OrdinaryUpgrade != null
                    && !choice.IncomeOffer.HasValue), Is.True);
                var selected = choices.Choices[0];
                Assert.That(session.Ledger.Balance, Is.EqualTo(20));
                Assert.That(session.ResolveRewardChoice(choices.SourceId, selected.Id, out var error), Is.True, error);
                Assert.That(session.Ledger.Balance, Is.EqualTo(20), "Free reward choices must not charge Field Data.");
                Assert.That(session.ResolveRewardChoice(choices.SourceId, selected.Id, out error), Is.True, error);
                Assert.That(progression.GetUpgradeLevel(selected.OrdinaryUpgrade.Id), Is.EqualTo(1));
                Assert.That(session.CurrentIncomeExecutionOrder, Is.Empty);
                Assert.That(session.ResolveRewardChoice(choices.SourceId, "skip", out _), Is.False);

                Assert.That(ContinueSession(session), Is.True);
                manager.Advance(10.1f);
                var roundTwo = session.Ledger.Awards.Single(award => award.GlobalRound == 2);
                Assert.That(roundTwo.Amount, Is.EqualTo(roundTwo.PopulationCounts[hare]));
                Assert.That(session.GetCurrentRoundRewardChoices().SourceId, Is.EqualTo("round-2"));
            }

            var (sameDefinition, sameManager, sameProgression) = CreateSessionParts();
            var sameSeedSession = new FieldDataJourneySession(sameDefinition, sameManager, sameProgression);
            Assert.That(sameSeedSession.SetOrdinaryRewardPool(ordinary), Is.True);
            using (sameSeedSession)
            {
                Assert.That(sameSeedSession.Begin(), Is.True);
                sameManager.Advance(10.1f);
                var repeatedChoices = sameSeedSession.GetCurrentRoundRewardChoices();
                Assert.That(repeatedChoices.Choices.Select(choice => choice.Id),
                    Is.EqualTo(session.RewardChoiceSets.First(set => set.SourceId == "round-1")
                        .Choices.Select(choice => choice.Id)));
                var ordinaryChoice = repeatedChoices.Choices.First(choice => choice.OrdinaryUpgrade != null);
                var balanceBeforeChoice = sameSeedSession.Ledger.Balance;
                Assert.That(sameSeedSession.ResolveRewardChoice(repeatedChoices.SourceId,
                    ordinaryChoice.Id, out var error), Is.True, error);
                Assert.That(sameSeedSession.Ledger.Balance, Is.EqualTo(balanceBeforeChoice));
                Assert.That(sameProgression.GetUpgradeLevel(ordinaryChoice.OrdinaryUpgrade.Id), Is.EqualTo(1));
                Assert.That(ContinueSession(sameSeedSession), Is.True);
                sameManager.Advance(10.1f);
                var laterChoices = sameSeedSession.GetCurrentRoundRewardChoices();
                Assert.That(laterChoices.Choices.Count, Is.EqualTo(3));
                Assert.That(laterChoices.Choices.All(choice => !choice.IncomeOffer.HasValue), Is.True);
            }
        }

        [Test]
        public void NoRoundOrNodeRewardGrantsIncomeAcrossTheEntireRoute()
        {
            var (definition, manager, progression) = CreateSessionParts();
            using (var session = new FieldDataJourneySession(definition, manager, progression))
            {
                Assert.That(session.Begin(), Is.True);
                for (var round = 1; round <= 18; round++)
                {
                    manager.Advance(10.1f);
                    if (round == 18)
                    {
                        Assert.That(session.State, Is.EqualTo(FieldDataJourneyFlowState.Succeeded));
                        Assert.That(session.GetCurrentRoundRewardChoices(), Is.Null);
                        Assert.That(session.GetCurrentNodeRewardChoices(), Is.Null);
                        break;
                    }

                    var roundChoices = session.GetCurrentRoundRewardChoices();
                    var nodeChoices = session.GetCurrentNodeRewardChoices();
                    var choices = roundChoices ?? nodeChoices;
                    Assert.That(choices, Is.Not.Null);
                    Assert.That(choices.Choices, Is.Empty,
                        "An empty species pool cannot be filled with income modifiers.");
                    Assert.That(session.GetCurrentRoundRewardChoices() ?? session.GetCurrentNodeRewardChoices(),
                        Is.SameAs(choices));
                    var balanceBeforeChoice = session.Ledger.Balance;
                    Assert.That(session.ResolveRewardChoice(choices.SourceId, "skip", out var error), Is.True, error);
                    Assert.That(session.ResolveRewardChoice(choices.SourceId, "skip", out error), Is.True, error);
                    Assert.That(session.CurrentIncomeExecutionOrder, Is.Empty);
                    Assert.That(session.Ledger.Balance, Is.EqualTo(balanceBeforeChoice));
                    Assert.That(ContinueSession(session), Is.True);
                }

                Assert.That(session.CurrentIncomeExecutionOrder, Is.Empty);
                Assert.That(session.Ledger.Awards.First().Amount, Is.EqualTo(20),
                    "Species rewards must not rewrite a settled population payout.");
            }
        }

        [Test]
        public void ShopActionsWaitForTheNodeRewardChoice()
        {
            var (definition, manager, progression) = CreateSessionParts(hareCount: 100);
            using (var session = new FieldDataJourneySession(definition, manager, progression))
            {
                Assert.That(session.Begin(), Is.True);
                for (var round = 1; round <= 6; round++)
                {
                    manager.Advance(10.1f);
                    if (round == 1)
                    {
                        Assert.That(session.TryAddIncomeModifier(new FieldDataIncomeModifierInstance(
                            "shop-gate-a", "income-plus-one-per-hare", FieldDataIncomeOperation.AddPerHare, 1m),
                            out _), Is.True);
                        Assert.That(session.TryAddIncomeModifier(new FieldDataIncomeModifierInstance(
                            "shop-gate-b", "income-plus-one-per-hare", FieldDataIncomeOperation.AddPerHare, 1m),
                            out _), Is.True);
                    }
                    if (round < 6) Assert.That(ContinueSession(session), Is.True);
                }
                var choices = session.GetCurrentNodeRewardChoices();
                Assert.That(choices.Resolved, Is.False);
                var offer = session.GetCurrentShopVisit().Offers.First(item => !item.Locked);
                var balanceBefore = session.Ledger.Balance;
                var orderBefore = session.CurrentIncomeExecutionOrder.Select(item => item.InstanceId).ToArray();
                Assert.That(session.TryBuyShopOffer("before-node-choice", offer.Id, out _), Is.False);
                Assert.That(session.TryBuyRandomIncomeReorder("before-node-choice-reorder", out _, out _), Is.False);
                Assert.That(session.TryCreateChosenIncomeReorderDraft(out _, out _), Is.False);
                Assert.That(session.Ledger.Balance, Is.EqualTo(balanceBefore));
                Assert.That(session.CurrentIncomeExecutionOrder.Select(item => item.InstanceId), Is.EqualTo(orderBefore));
                Assert.That(session.GetCurrentShopVisit().ReorderPriceStep, Is.Zero);
                Assert.That(session.ResolveRewardChoice(choices.SourceId, "skip", out var error), Is.True, error);
                Assert.That(session.TryBuyShopOffer("after-node-choice", offer.Id, out error), Is.True, error);
            }
        }

        [Test]
        public void ShopOffersAreDeterministicDistinctNonConflictingAndPurchasedOnce()
        {
            var (definition, manager, progression) = CreateSessionParts();
            var session = new FieldDataJourneySession(definition, manager, progression);
            using (session)
            {
                Assert.That(session.Begin(), Is.True);
                for (var round = 1; round <= 6; round++)
                {
                    manager.Advance(10.1f);
                    if (round < 6) Assert.That(ContinueSession(session), Is.True);
                }
                Assert.That(session.State, Is.EqualTo(FieldDataJourneyFlowState.Shop));
                var offers = session.GetCurrentShopVisit();
                Assert.That(offers, Is.Not.Null);
                var nodeChoice = session.GetCurrentNodeRewardChoices();
                Assert.That(session.ResolveRewardChoice(nodeChoice.SourceId, "skip", out _), Is.True);
                Assert.That(offers.Offers.Count, Is.EqualTo(5));
                Assert.That(session.GetCurrentShopVisit(), Is.SameAs(offers), "Re-entry must retain the original offer set.");
                Assert.That(offers.Offers.All(offer => offer.Id != "income-multiply-two"), Is.True,
                    "The ×2 income modifier is not eligible at the first shop.");
                var speciesOffers = offers.Offers.Where(offer => offer.Kind == FieldDataShopOfferKind.SpeciesEffect).ToArray();
                Assert.That(speciesOffers.GroupBy(offer => offer.FamilyId)
                    .All(group => group.Select(offer => System.Math.Sign(offer.SignedSpeciesOffer.Value.SignedAmount)).Distinct().Count() == 1), Is.True,
                    "A shop visit cannot offer both signs of one effect family.");

                var selected = offers.Offers.First(offer => !offer.Locked);
                var balanceBefore = session.Ledger.Balance;
                Assert.That(session.TryBuyShopOffer("shop-buy-1", selected.Id, out var error), Is.True, error);
                Assert.That(session.Ledger.Balance, Is.EqualTo(balanceBefore - selected.Cost));
                Assert.That(session.TryBuyShopOffer("shop-buy-1", selected.Id, out error), Is.True, error,
                    "A retry of the accepted request must not charge twice.");
                Assert.That(session.TryBuyShopOffer("shop-buy-2", selected.Id, out _), Is.False,
                    "A distinct purchase request cannot buy the same offer again.");
                Assert.That(selected.Purchased, Is.True);
                Assert.That(session.CanUsePopulationControls, Is.False,
                    "Once the player starts a shop purchase, round population controls close.");
                var second = offers.Offers.First(offer => !offer.Purchased && !offer.Locked);
                var balanceBeforeSecond = session.Ledger.Balance;
                Assert.That(session.TryBuyShopOffer("shop-buy-second", second.Id, out error), Is.True, error);
                Assert.That(session.Ledger.Balance, Is.EqualTo(balanceBeforeSecond - second.Cost));
                var other = offers.Offers.First(offer => !offer.Purchased && !offer.Locked);
                Assert.That(session.TryBuyShopOffer("shop-buy-1", other.Id, out _), Is.False,
                    "A request identity cannot be reused for another offer.");
                Assert.That(session.Ledger.TrySpend(session.Ledger.Balance), Is.True);
                Assert.That(session.TryBuyShopOffer("unaffordable", other.Id, out var fundsError), Is.False);
                Assert.That(fundsError, Does.Contain("not enough Field Data"));
                Assert.That(other.Purchased, Is.False);
                Assert.That(session.Ledger.Balance, Is.Zero);
                Assert.That(ContinueSession(session), Is.True);
                Assert.That(session.TryBuyShopOffer("stale-shop", other.Id, out _), Is.False);
            }
        }

        [Test]
        public void SameSpeciesOfferCanStackWhenOfferedAtSeparateShopVisits()
        {
            var (definition, manager, progression) = CreateSessionParts(hareCount: 100);
            var (referenceDefinition, referenceManager, referenceProgression) = CreateSessionParts(hareCount: 100);
            using (var session = new FieldDataJourneySession(definition, manager, progression))
            using (var reference = new FieldDataJourneySession(referenceDefinition, referenceManager, referenceProgression))
            {
                ReachFirstShop(session, manager);
                ReachFirstShop(reference, referenceManager);
                var firstVisit = session.GetCurrentShopVisit();
                Assert.That(ContinueSession(reference), Is.True);
                for (var round = 7; round <= 12; round++)
                {
                    referenceManager.Advance(10.1f);
                    if (round < 12) Assert.That(ContinueSession(reference), Is.True);
                }
                var laterOfferIds = reference.GetCurrentShopVisit().Offers.Select(offer => offer.Id).ToArray();
                var repeated = firstVisit.Offers.First(offer => offer.Kind == FieldDataShopOfferKind.SpeciesEffect
                    && laterOfferIds.Contains(offer.Id));
                Assert.That(session.TryBuyShopOffer("first-visit", repeated.Id, out var error), Is.True, error);
                Assert.That(ContinueSession(session), Is.True);
                for (var round = 7; round <= 12; round++)
                {
                    manager.Advance(10.1f);
                    if (round < 12) Assert.That(ContinueSession(session), Is.True);
                }
                var secondVisit = session.GetCurrentShopVisit();
                Assert.That(secondVisit.Offers.Select(offer => offer.Id), Is.EqualTo(laterOfferIds));
                var secondNodeChoice = session.GetCurrentNodeRewardChoices();
                Assert.That(session.ResolveRewardChoice(secondNodeChoice.SourceId, "skip", out _), Is.True);
                var balanceBefore = session.Ledger.Balance;
                Assert.That(session.TryBuyShopOffer("second-visit", repeated.Id, out error), Is.True, error);
                Assert.That(session.Ledger.Balance, Is.EqualTo(balanceBefore - repeated.Cost));
                Assert.That(session.SpeciesEffects.Count(effect => effect.OfferId == repeated.Id), Is.EqualTo(2));
                Assert.That(firstVisit.Offers.Single(offer => offer.Id == repeated.Id).Purchased, Is.True);
                Assert.That(secondVisit.Offers.Single(offer => offer.Id == repeated.Id).Purchased, Is.True);
            }
        }

        [Test]
        public void CappedShopOfferRerollsOnceFreeWithoutConflictingWithPurchasedOffers()
        {
            var (definition, manager, progression) = CreateSessionParts(hareCount: 100);
            var (referenceDefinition, referenceManager, referenceProgression) = CreateSessionParts(hareCount: 100);
            using (var session = new FieldDataJourneySession(definition, manager, progression))
            using (var reference = new FieldDataJourneySession(referenceDefinition, referenceManager, referenceProgression))
            {
                ReachFirstShop(session, manager);
                ReachFirstShop(reference, referenceManager);
                var referenceOffers = reference.GetCurrentShopVisit().Offers;
                var familyOffer = referenceOffers.First(offer => offer.Kind == FieldDataShopOfferKind.SpeciesEffect);
                for (var copy = 0; copy < 3; copy++)
                    Assert.That(session.TryBuySpeciesEffect("fixture-copy-" + copy, familyOffer.Id,
                        out _, out var capError), Is.True, capError);

                var visit = session.GetCurrentShopVisit();
                Assert.That(visit.Offers.Select(offer => offer.Id), Is.EqualTo(referenceOffers.Select(offer => offer.Id)),
                    "Ownership and wallet changes must not consume the offer-draw stream.");
                var capped = visit.Offers.Single(offer => offer.Id == familyOffer.Id);
                Assert.That(capped.Locked, Is.True);
                var balanceBeforeReject = session.Ledger.Balance;
                Assert.That(session.TryBuyShopOffer("capped-purchase", capped.Id, out _), Is.False);
                Assert.That(session.Ledger.Balance, Is.EqualTo(balanceBeforeReject));
                var purchased = visit.Offers.First(offer => !offer.Locked
                    && offer.Kind == FieldDataShopOfferKind.SpeciesEffect);
                Assert.That(session.TryBuyShopOffer("other-family", purchased.Id, out var error), Is.True, error);
                var balanceBeforeReroll = session.Ledger.Balance;

                Assert.That(session.TryRerollCappedShopOffer(capped.Id, out var replacement, out error), Is.True, error);
                Assert.That(replacement, Is.Not.Null);
                Assert.That(replacement.Locked, Is.False);
                Assert.That(replacement.Id, Is.Not.EqualTo("income-multiply-two"));
                Assert.That(session.Ledger.Balance, Is.EqualTo(balanceBeforeReroll));
                Assert.That(visit.Offers.Select(offer => offer.Id).Distinct().Count(), Is.EqualTo(visit.Offers.Count));
                Assert.That(visit.Offers.Where(offer => offer.Kind == FieldDataShopOfferKind.SpeciesEffect)
                    .GroupBy(offer => offer.FamilyId)
                    .All(group => group.Select(offer => System.Math.Sign(offer.SignedSpeciesOffer.Value.SignedAmount))
                        .Distinct().Count() == 1), Is.True);
                Assert.That(purchased.Purchased, Is.True);
                Assert.That(visit.Offers, Does.Contain(purchased));
                var idsAfterReroll = visit.Offers.Select(offer => offer.Id).ToArray();
                Assert.That(session.TryRerollCappedShopOffer(capped.Id, out _, out _), Is.False);
                Assert.That(session.GetCurrentShopVisit(), Is.SameAs(visit));
                Assert.That(visit.Offers.Select(offer => offer.Id), Is.EqualTo(idsAfterReroll));
                Assert.That(session.Ledger.Balance, Is.EqualTo(balanceBeforeReroll));
            }
        }

        [Test]
        public void ShopShowsFewerOffersWhenAllSpeciesFamiliesAreCappedAndRerollsRunOut()
        {
            var (definition, manager, progression) = CreateSessionParts(hareCount: 100);
            using (var session = new FieldDataJourneySession(definition, manager, progression))
            {
                ReachFirstShop(session, manager);
                foreach (var offer in definition.SignedSpeciesOffers.Where(offer => offer.SignedAmount > 0f))
                    for (var copy = 0; copy < 3; copy++)
                        Assert.That(session.TryBuySpeciesEffect(offer.Id + "-" + copy, offer.Id,
                            out _, out var capError), Is.True, capError);

                var visit = session.GetCurrentShopVisit();
                var lockedOffers = visit.Offers.Where(offer => offer.Locked).ToArray();
                Assert.That(lockedOffers.Length, Is.GreaterThanOrEqualTo(3));
                var balanceBeforeRerolls = session.Ledger.Balance;
                foreach (var offer in lockedOffers)
                {
                    Assert.That(session.TryRerollCappedShopOffer(offer.Id, out _, out var error), Is.True, error);
                    Assert.That(session.TryRerollCappedShopOffer(offer.Id, out _, out _), Is.False);
                }
                Assert.That(visit.Offers.Count, Is.EqualTo(2));
                Assert.That(visit.Offers.All(offer => offer.Kind == FieldDataShopOfferKind.IncomeModifier), Is.True);
                Assert.That(visit.Offers.Select(offer => offer.Id).Distinct().Count(), Is.EqualTo(2));
                Assert.That(session.Ledger.Balance, Is.EqualTo(balanceBeforeRerolls));
            }
        }

        [Test]
        public void IncomeReordersRequireTwoDistinctInstancesEvenWhenEffectsMatch()
        {
            var (definition, manager, progression) = CreateSessionParts(hareCount: 100);
            using (var session = new FieldDataJourneySession(definition, manager, progression))
            {
                ReachFirstShop(session, manager);
                Assert.That(session.TryAddIncomeModifier(new FieldDataIncomeModifierInstance(
                    "copy-a", "income-plus-one-per-hare", FieldDataIncomeOperation.AddPerHare, 1m),
                    out var error, "fixture-shop"), Is.True, error);
                var balanceBefore = session.Ledger.Balance;
                Assert.That(session.TryBuyRandomIncomeReorder("too-few", out _, out _), Is.False);
                Assert.That(session.TryCreateChosenIncomeReorderDraft(out _, out _), Is.False);
                Assert.That(session.Ledger.Balance, Is.EqualTo(balanceBefore));
                Assert.That(session.GetCurrentShopVisit().ReorderPriceStep, Is.Zero);

                Assert.That(session.TryAddIncomeModifier(new FieldDataIncomeModifierInstance(
                    "copy-b", "income-plus-one-per-hare", FieldDataIncomeOperation.AddPerHare, 1m),
                    out error, "fixture-shop"), Is.True, error);
                Assert.That(session.TryBuyRandomIncomeReorder("two-matching-effects", out var receipt, out error),
                    Is.True, error);
                Assert.That(receipt.PreviousOrder, Is.EqualTo(new[] { "copy-a", "copy-b" }));
                Assert.That(receipt.NextOrder, Is.EqualTo(new[] { "copy-b", "copy-a" }));
                Assert.That(session.IncomeAcquisitionHistory.Select(item => item.InstanceId),
                    Is.EqualTo(new[] { "copy-a", "copy-b" }));
            }
        }

        [Test]
        public void IncomeReordersUseCompoundingPricesRetainHistoryAndResetAtNextShop()
        {
            var (definition, manager, progression) = CreateSessionParts(hareCount: 100);
            var session = new FieldDataJourneySession(definition, manager, progression);
            using (session)
            {
                Assert.That(session.Begin(), Is.True);
                manager.Advance(10.1f);
                Assert.That(session.TryAddIncomeModifier(
                    new FieldDataIncomeModifierInstance("acq-a", "income-plus-one-per-hare",
                        FieldDataIncomeOperation.AddPerHare, 1m), out var error, "round-1"), Is.True, error);
                Assert.That(session.TryAddIncomeModifier(
                    new FieldDataIncomeModifierInstance("acq-b", "income-multiply-one-point-five",
                        FieldDataIncomeOperation.MultiplyPopulationPayout, 1.5m), out error, "round-1"), Is.True, error);
                for (var round = 1; round <= 6; round++)
                {
                    if (round > 1) manager.Advance(10.1f);
                    if (round < 6) Assert.That(ContinueSession(session), Is.True);
                }
                Assert.That(session.State, Is.EqualTo(FieldDataJourneyFlowState.Shop));
                Assert.That(session.IncomeAcquisitionHistory.Select(item => item.InstanceId),
                    Is.EqualTo(new[] { "acq-a", "acq-b" }));

                var nodeChoice = session.GetCurrentNodeRewardChoices();
                Assert.That(session.ResolveRewardChoice(nodeChoice.SourceId, "skip", out _), Is.True);

                Assert.That(session.TryGetCurrentReorderCosts(out var randomCost, out var chosenCost, out error), Is.True, error);
                Assert.That(randomCost, Is.EqualTo(10));
                Assert.That(chosenCost, Is.EqualTo(100));
                var original = session.CurrentIncomeExecutionOrder.Select(item => item.InstanceId).ToArray();
                Assert.That(session.TryBuyRandomIncomeReorder("reorder-random-0", out var first, out error), Is.True, error);
                Assert.That(first.Cost, Is.EqualTo(10));
                Assert.That(first.NextOrder, Is.Not.EqualTo(original));
                var afterRandomBalance = session.Ledger.Balance;
                Assert.That(session.TryBuyRandomIncomeReorder("reorder-random-0", out var retry, out error), Is.True, error);
                Assert.That(retry, Is.SameAs(first));
                Assert.That(session.Ledger.Balance, Is.EqualTo(afterRandomBalance));

                Assert.That(session.TryGetCurrentReorderCosts(out randomCost, out chosenCost, out error), Is.True, error);
                Assert.That(randomCost, Is.EqualTo(11));
                Assert.That(chosenCost, Is.EqualTo(110));
                Assert.That(session.TryCreateChosenIncomeReorderDraft(out var draft, out error), Is.True, error);
                var beforeDraftConfirmation = session.CurrentIncomeExecutionOrder.Select(item => item.InstanceId).ToArray();
                Assert.That(draft.CurrentOrder, Is.EqualTo(beforeDraftConfirmation));
                var chosenOrder = draft.CurrentOrder.Reverse().ToArray();
                Assert.That(session.TryConfirmChosenIncomeReorder("reorder-chosen-1", chosenOrder, out var chosen, out error), Is.True, error);
                Assert.That(chosen.Cost, Is.EqualTo(110));
                Assert.That(chosen.NextOrder, Is.EqualTo(chosenOrder));
                var balanceAfterChosen = session.Ledger.Balance;
                Assert.That(session.TryConfirmChosenIncomeReorder("reorder-chosen-1", chosenOrder,
                    out var chosenRetry, out error), Is.True, error);
                Assert.That(chosenRetry, Is.SameAs(chosen));
                Assert.That(session.Ledger.Balance, Is.EqualTo(balanceAfterChosen));

                Assert.That(session.TryGetCurrentReorderCosts(out randomCost, out chosenCost, out error), Is.True, error);
                Assert.That(randomCost, Is.EqualTo(13));
                Assert.That(chosenCost, Is.EqualTo(130));
                var stepTwoOrder = session.CurrentIncomeExecutionOrder.Select(item => item.InstanceId).ToArray();
                Assert.That(session.TryCreateChosenIncomeReorderDraft(out draft, out error), Is.True, error);
                Assert.That(session.TryConfirmChosenIncomeReorder("reorder-invalid", new[] { "missing" }, out _, out _), Is.False);
                Assert.That(session.TryGetCurrentReorderCosts(out randomCost, out _, out error), Is.True, error);
                Assert.That(randomCost, Is.EqualTo(13), "Rejected reorder attempts must not advance pricing.");
                Assert.That(session.CancelChosenIncomeReorderDraft(), Is.True);
                Assert.That(session.CurrentIncomeExecutionOrder.Select(item => item.InstanceId), Is.EqualTo(stepTwoOrder));
                Assert.That(session.TryCreateChosenIncomeReorderDraft(out draft, out error), Is.True, error);
                Assert.That(session.TryAddIncomeModifier(new FieldDataIncomeModifierInstance(
                    "acq-c", "income-plus-one-per-hare", FieldDataIncomeOperation.AddPerHare, 1m),
                    out error, "shop-1"), Is.True, error);
                Assert.That(session.CurrentIncomeExecutionOrder.Select(item => item.InstanceId),
                    Is.EqualTo(stepTwoOrder.Concat(new[] { "acq-c" })));
                var balanceBeforeStale = session.Ledger.Balance;
                Assert.That(session.TryConfirmChosenIncomeReorder("stale-acquisition", draft.CurrentOrder.Reverse(),
                    out _, out var staleError), Is.False);
                Assert.That(staleError, Does.Contain("stale"));
                Assert.That(session.Ledger.Balance, Is.EqualTo(balanceBeforeStale));
                Assert.That(session.CancelChosenIncomeReorderDraft(), Is.True);
                Assert.That(session.TryBuyRandomIncomeReorder("reorder-random-2", out var third, out error), Is.True, error);
                Assert.That(third.Cost, Is.EqualTo(13));

                Assert.That(session.TryGetCurrentReorderCosts(out randomCost, out chosenCost, out error), Is.True, error);
                Assert.That(randomCost, Is.EqualTo(14));
                Assert.That(chosenCost, Is.EqualTo(140));
                Assert.That(session.TryCreateChosenIncomeReorderDraft(out draft, out error), Is.True, error);
                var fourthOrder = session.CurrentIncomeExecutionOrder.Select(item => item.InstanceId).Reverse().ToArray();
                Assert.That(session.TryConfirmChosenIncomeReorder("reorder-chosen-3", fourthOrder, out var fourth, out error), Is.True, error);
                Assert.That(fourth.Cost, Is.EqualTo(140));
                Assert.That(session.IncomeReorderHistory.Select(receipt => receipt.PriceStep), Is.EqualTo(new[] { 0, 1, 2, 3 }));
                Assert.That(session.IncomeAcquisitionHistory.Select(item => item.InstanceId),
                    Is.EqualTo(new[] { "acq-a", "acq-b", "acq-c" }));
                Assert.That(session.Ledger.Awards.First().Amount, Is.EqualTo(100));
                Assert.That(session.Ledger.Awards.First().Income.Steps, Is.Empty);

                Assert.That(ContinueSession(session), Is.True);
                for (var round = 7; round <= 12; round++)
                {
                    manager.Advance(10.1f);
                    if (round < 12) Assert.That(ContinueSession(session), Is.True);
                }
                Assert.That(session.State, Is.EqualTo(FieldDataJourneyFlowState.Shop));
                Assert.That(session.TryGetCurrentReorderCosts(out randomCost, out chosenCost, out error), Is.True, error);
                Assert.That(randomCost, Is.EqualTo(10));
                Assert.That(chosenCost, Is.EqualTo(100));
                var secondNodeChoice = session.GetCurrentNodeRewardChoices();
                Assert.That(session.ResolveRewardChoice(secondNodeChoice.SourceId, "skip", out _), Is.True);
                Assert.That(session.Ledger.TrySpend(session.Ledger.Balance - 9), Is.True);
                var orderBeforeRejection = session.CurrentIncomeExecutionOrder.Select(item => item.InstanceId).ToArray();
                Assert.That(session.TryBuyRandomIncomeReorder("unaffordable-random", out _, out _), Is.False);
                Assert.That(session.TryCreateChosenIncomeReorderDraft(out draft, out error), Is.True, error);
                Assert.That(session.TryConfirmChosenIncomeReorder("unaffordable-chosen", draft.CurrentOrder.Reverse(),
                    out _, out var fundsError), Is.False);
                Assert.That(fundsError, Does.Contain("not enough Field Data"));
                Assert.That(session.Ledger.Balance, Is.EqualTo(9));
                Assert.That(session.GetCurrentShopVisit().ReorderPriceStep, Is.Zero);
                Assert.That(session.CurrentIncomeExecutionOrder.Select(item => item.InstanceId),
                    Is.EqualTo(orderBeforeRejection));
                Assert.That(session.CancelChosenIncomeReorderDraft(), Is.True);
            }
        }

        [Test]
        public void PreSettledRoundPayoutCanCompleteItsMissingNodeRewardExactlyOnce()
        {
            var (definition, manager, progression) = CreateSessionParts();
            FieldDataJourneySession session = null;
            manager.PhaseBoundaryReached += run =>
            {
                if (run.Tick != 600)
                {
                    return;
                }

                var population = run.PopulationHistory[run.PopulationHistory.Count - 1];
                Assert.That(session.Ledger.TrySettlePopulationPayout(6, 600,
                    population.Counts, new FieldDataIncomeModifierInstance[0],
                    out _, out var error), Is.True, error);
            };
            session = new FieldDataJourneySession(definition, manager, progression);
            using (session)
            {
                Assert.That(session.Begin(), Is.True);
                for (var round = 1; round <= 6; round++)
                {
                    manager.Advance(10.1f);
                    if (round < 6)
                    {
                        Assert.That(ContinueSession(session), Is.True);
                    }
                }

                Assert.That(session.State, Is.EqualTo(FieldDataJourneyFlowState.Shop));
                var sixthRoundAwards = new List<FieldDataAwardReceipt>();
                foreach (var award in session.Ledger.Awards)
                {
                    if (award.GlobalRound == 6)
                    {
                        sixthRoundAwards.Add(award);
                    }
                }
                Assert.That(sixthRoundAwards.Count, Is.EqualTo(2));
                Assert.That(sixthRoundAwards.Count(award =>
                    award.Kind == FieldDataAwardKind.PopulationPayout), Is.EqualTo(1));
                Assert.That(sixthRoundAwards.Count(award =>
                    award.Kind == FieldDataAwardKind.NodeClear), Is.EqualTo(1));
                Assert.That(session.Ledger.Balance,
                    Is.EqualTo(6 * 20 + session.Definition.NodeClearFieldData));
            }
        }

        static void ReachFirstShop(FieldDataJourneySession session, SimulationManager manager)
        {
            Assert.That(session.Begin(), Is.True);
            for (var round = 1; round <= 6; round++)
            {
                manager.Advance(10.1f);
                if (round < 6) Assert.That(ContinueSession(session), Is.True);
            }
            Assert.That(session.State, Is.EqualTo(FieldDataJourneyFlowState.Shop));
            var choices = session.GetCurrentNodeRewardChoices();
            Assert.That(session.ResolveRewardChoice(choices.SourceId, "skip", out var error), Is.True, error);
        }

        static bool ContinueSession(FieldDataJourneySession session)
        {
            var roundChoices = session.GetCurrentRoundRewardChoices();
            if (roundChoices != null && !roundChoices.Resolved)
                session.ResolveRewardChoice(roundChoices.SourceId, "skip", out _);
            var nodeChoices = session.GetCurrentNodeRewardChoices();
            if (nodeChoices != null && !nodeChoices.Resolved)
                session.ResolveRewardChoice(nodeChoices.SourceId, "skip", out _);
            return session.Continue();
        }

        static (FieldDataJourneyDefinition, SimulationManager, SpeciesProgression) CreateSessionParts(
            bool stableHare = true,
            int hareCount = 20,
            int plantCount = 500,
            GenomeSimulationSnapshot activeGenomeSnapshot = null,
            int extinctionAfterSteps = 0)
        {
            var definitionAsset = AssetDatabase.LoadAssetAtPath<FieldDataJourneyDefinitionAsset>(DefinitionPath);
            Assert.That(definitionAsset.TryResolve(out var definition, out var error), Is.True, error);
            var hareSpecies = new SpeciesId("hare");
            var plantSpecies = new SpeciesId("plant");
            var defaultHare = SpeciesRuleDefaults.Create()[SpeciesIds.Herbivore];
            var testHare = new SpeciesRules(
                movementSpeed: 0f,
                movementPattern: defaultHare.MovementPattern,
                attackPattern: defaultHare.AttackPattern,
                attackAmount: 0,
                blockPattern: defaultHare.BlockPattern,
                blockAmount: 0,
                dietPattern: defaultHare.DietPattern,
                dietTarget: plantSpecies,
                reproductionPattern: defaultHare.ReproductionPattern,
                reproductionNeighborCount: 1,
                reproductionChance: 0f,
                startingEnergy: stableHare ? 1000 : System.Math.Max(1, extinctionAfterSteps),
                metabolism: stableHare ? 0 : 1,
                role: SpeciesRole.Herbivore,
                maximumEnergy: stableHare ? 1000 : System.Math.Max(1, extinctionAfterSteps),
                energyLossIntervalTicks: stableHare ? 1 : 100);
            var data = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(ScenarioPath)
                .CreateRuntimeData()
                .WithoutSpecies(new SpeciesId("fox"))
                .WithSpeciesRules(hareSpecies, testHare)
                .WithStartingProbability(hareSpecies, 0f)
                .WithStartingProbability(plantSpecies, 0f)
                .WithStartingPopulations(new Dictionary<SpeciesId, int>
                {
                    [plantSpecies] = plantCount,
                    [hareSpecies] = hareCount,
                })
                .WithRunTicks(1800, 0.1f);
            var run = new SimulationRunState(
                SpeciesInitialGridFactory.Create(data, 10100), hareSpecies, 10100,
                180f, 1800, activeGenomeSnapshot);
            run.ConfigureContinuousPhases(100);
            var manager = new SimulationManager();
            manager.SetRunner(new SpeciesSimulationRunner(run, data));
            var progression = new SpeciesProgression(new SpeciesDefinition(
                hareSpecies, data.SpeciesRules[hareSpecies]));
            return (definition, manager, progression);
        }
    }
}
