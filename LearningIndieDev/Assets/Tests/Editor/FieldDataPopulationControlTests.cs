using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace SaltyGame.EditorTests
{
    public sealed class FieldDataPopulationControlTests
    {
        static readonly SpeciesId Hare = new SpeciesId("hare");
        static readonly SpeciesId Fox = new SpeciesId("fox");
        static readonly SpeciesId Plant = new SpeciesId("plant");
        const string DefinitionPath = "Assets/Data/TestData/FieldDataStarterDefinition.asset";
        const string ScenarioPath = "Assets/Data/ProductionData/CellularSimulation/Scenarios/ForestEdge.asset";

        [TestCase(0, false, 10)]
        [TestCase(1, true, 15)]
        [TestCase(2, true, 20)]
        [TestCase(3, true, 25)]
        [TestCase(4, true, 30)]
        [TestCase(5, true, 35)]
        [TestCase(5, false, 10)]
        public void FrozenPopulationReservesRankControlsJourneyAllowance(int rank, bool equipped, int expected)
        {
            var definition = FieldDataPermanentProgressionDefinition.CreateStarter();
            var owned = rank == 0 ? new string[0] : definition.HareMap.Nodes.Select(node => node.Id).ToArray();
            var active = equipped ? new[] { "hare.warren-network" } : new string[0];
            var profile = new SpeciesGenomeProfile(Hare, owned, active, populationReservesRank: rank);
            var snapshot = definition.FreezeOwnedLoadout(profile);
            var (_, manager, _, session) = CreateSessionParts(hareCount: 20, plantCount: 4, activeGenome: snapshot);
            using (session)
            {
                Assert.That(session.HarePurchaseAllowance, Is.EqualTo(expected));
                Assert.That(session.Begin(), Is.True);
                manager.Advance(10.1f);
                var restored = session.Run.CreateCheckpoint().Restore();
                Assert.That(restored.ActiveGenomeSnapshot.GetSpeciesGenome(Hare).PopulationReservesRank,
                    Is.EqualTo(equipped ? rank : 0));
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public void RoundOffersRequireResearchAndEmptyResearchCanSkip(bool researched)
        {
            var (_, manager, progression, session) = CreateSessionParts(hareCount: 20, plantCount: 4,
                mutationResearch: id => researched && id == SpeciesUpgradeCatalog.ToughHideId);
            using (session)
            {
                session.SetOrdinaryRewardPool(SpeciesUpgradeCatalog.ResolveFieldDataOrdinaryRewardPool(Hare, null));
                Assert.That(session.Begin(), Is.True);
                manager.Advance(10.1f);
                var choices = session.GetCurrentRoundRewardChoices();
                Assert.That(choices.Choices.Count, Is.EqualTo(researched ? 1 : 0));
                Assert.That(session.ResolveRewardChoice(choices.SourceId, "upgrade-" + SpeciesUpgradeCatalog.SeedDispersalId, out _), Is.False);
                Assert.That(progression.TryApplyFieldDataReward(SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.SeedDispersalId).CreateSnapshot(Hare)), Is.False);
                if (researched)
                    Assert.That(choices.Choices.Single().OrdinaryUpgrade.Id, Is.EqualTo(SpeciesUpgradeCatalog.ToughHideId));
                Assert.That(session.ResolveRewardChoice(choices.SourceId, "skip", out _), Is.True);
                Assert.That(session.CanUsePopulationControls, Is.True);
                Assert.That(session.Continue(), Is.True);
            }
        }

        [TestCase(FieldDataPopulationInterventionKind.AddHare, 1, 0)]
        [TestCase(FieldDataPopulationInterventionKind.RemoveHare, 0, 1)]
        [TestCase(FieldDataPopulationInterventionKind.AddFox, 1, 0)]
        [TestCase(FieldDataPopulationInterventionKind.RemoveFox, 0, 1)]
        [TestCase(FieldDataPopulationInterventionKind.AddPlants, 5, 0)]
        [TestCase(FieldDataPopulationInterventionKind.RemovePlants, 0, 5)]
        public void PaidControlsReconcileExternalPopulationWithoutNaturalEvents(
            FieldDataPopulationInterventionKind kind, int added, int removed)
        {
            var (_, manager, _, session) = CreateSessionParts(
                hareCount: 100, foxCount: 4, plantCount: 20, includeFox: true);
            using (session)
            {
                ReachFirstFundedRoundDecision(manager, session);
                Assert.That(session.TryBuyPopulationControl("accounting", kind, out var receipt, out var error), Is.True, error);
                Assert.That(session.TryBuyPopulationControl("accounting", kind, out _, out error), Is.True, error);
                var run = session.Run;
                var species = receipt.SpeciesId;
                Assert.That(run.GetBoundaryPopulationAdded(species), Is.EqualTo(added));
                Assert.That(run.GetBoundaryPopulationRemoved(species), Is.EqualTo(removed));
                Assert.That(run.Metrics.GetActivity(species).Births, Is.Zero);
                Assert.That(run.Metrics.DeathEvents, Is.Empty);
                var initial = run.PopulationHistory[0].GetCount(species);
                if (species == Hare)
                {
                    var line = run.Metrics.CreateHerbivoreStatLine(species, initial, GetCount(run, species), added, removed);
                    Assert.That(line.PopulationReconciled, Is.True);
                    Assert.That(line.ActualPreyScoreStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.Valid));
                }
                if (species == Fox)
                {
                    var line = run.Metrics.CreatePredatorStatLine(species, initial, GetCount(run, species), added, removed);
                    Assert.That(line.PopulationReconciled, Is.True);
                    Assert.That(line.ActualHuntScoreStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.Valid));
                }
                if (species == Hare || species == Fox)
                {
                    var serializer = System.AppDomain.CurrentDomain.GetAssemblies()
                        .Select(assembly => assembly.GetType("SaltyGame.EditorTools.SimulationReportSerialization"))
                        .First(type => type != null);
                    var method = serializer.GetMethods().Single(candidate =>
                        candidate.Name == (species == Hare ? "CreateHerbivoreStatLine" : "CreatePredatorStatLine")
                        && candidate.GetParameters().Length == 2);
                    var report = method.Invoke(null, new object[] { run, species });
                    Assert.That((int)report.GetType().GetField("ADD").GetValue(report), Is.EqualTo(added));
                    Assert.That((int)report.GetType().GetField("REM").GetValue(report), Is.EqualTo(removed));
                    Assert.That((bool)report.GetType().GetField("fpoReconciled").GetValue(report), Is.True);
                }
            }
        }

        [Test]
        public void IncomeUpgradesAreOneTimeAndPayoutTiersReplaceRatherThanCompound()
        {
            var (_, manager, _, session) = CreateSessionParts(hareCount: 20, plantCount: 4);
            using (session)
            {
                Assert.That(session.Begin(), Is.True);
                manager.Advance(10.1f);
                Assert.That(session.TryAddIncomeModifier(new FieldDataIncomeModifierInstance("add", "add",
                    FieldDataIncomeOperation.AddPerHare, 1m), out _), Is.True);
                Assert.That(session.TryAddIncomeModifier(new FieldDataIncomeModifierInstance("add-again", "add",
                    FieldDataIncomeOperation.AddPerHare, 1m), out _), Is.False);
                Assert.That(session.TryAddIncomeModifier(new FieldDataIncomeModifierInstance("skip", "tier3",
                    FieldDataIncomeOperation.MultiplyPopulationPayout, 2m), out _), Is.False);
                foreach (var tier in new[] { 1.5m, 1.75m, 2m })
                {
                    Assert.That(session.TryAddIncomeModifier(new FieldDataIncomeModifierInstance("tier-" + tier, "tier",
                        FieldDataIncomeOperation.MultiplyPopulationPayout, tier), out var error), Is.True, error);
                    Assert.That(session.CurrentIncomeExecutionOrder.Count, Is.EqualTo(2));
                    Assert.That(FieldDataIncome.TryCalculate(20, 20, session.CurrentIncomeExecutionOrder,
                        out var receipt, out error), Is.True, error);
                    Assert.That(receipt.Award, Is.EqualTo((int)(40m * tier)));
                }
                Assert.That(session.TryAddIncomeModifier(new FieldDataIncomeModifierInstance("repeat", "tier3",
                    FieldDataIncomeOperation.MultiplyPopulationPayout, 2m), out _), Is.False);
            }
        }

        [Test]
        public void HareAllowanceCountsGrossPurchasesAndResetsAtTheNextSimulationNode()
        {
            var (_, manager, _, session) = CreateSessionParts(hareCount: 200, plantCount: 4);
            using (session)
            {
                Assert.That(session.Begin(), Is.True);
                manager.Advance(10.1f);
                ResolveCurrentReward(session);
                for (var i = 0; i < 10; i++)
                    Assert.That(session.TryBuyPopulationControl("hare-" + i,
                        FieldDataPopulationInterventionKind.AddHare, out _, out var error), Is.True, error);
                Assert.That(session.PurchasedHaresThisSimulation, Is.EqualTo(10));
                Assert.That(session.HarePurchasesRemaining, Is.Zero);
                Assert.That(session.TryBuyPopulationControl("hare-0",
                    FieldDataPopulationInterventionKind.AddHare, out _, out _), Is.True);
                Assert.That(session.PurchasedHaresThisSimulation, Is.EqualTo(10));
                Assert.That(session.TryBuyPopulationControl("remove",
                    FieldDataPopulationInterventionKind.RemoveHare, out _, out _), Is.True);
                var balance = session.AvailableFieldData;
                var population = GetCount(session.Run, Hare);
                Assert.That(session.TryBuyPopulationControl("over-limit",
                    FieldDataPopulationInterventionKind.AddHare, out _, out _), Is.False);
                Assert.That(session.AvailableFieldData, Is.EqualTo(balance));
                Assert.That(GetCount(session.Run, Hare), Is.EqualTo(population));
                ReachFirstFundedRoundDecision(manager, session);
                Assert.That(session.PurchasedHaresThisSimulation, Is.Zero);
                Assert.That(session.HarePurchasesRemaining, Is.EqualTo(10));
            }
        }

        [Test]
        public void CompletedHumanRunReconcilesEightAddedAndNineRemovedHares()
        {
            var line = new SpeciesHerbivoreStatLine(Hare, 55, 80864, 38950, 963,
                327, 194, 46285, 610, 0, 143, addedPopulation: 8, removedPopulation: 9);
            Assert.That(line.ExpectedFinalPopulation, Is.EqualTo(143));
            Assert.That(line.PopulationReconciled, Is.True);
            Assert.That(line.ActualPreyScoreStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.Valid));
            var inconsistent = new SpeciesHerbivoreStatLine(Hare, 55, 80864, 38950, 963,
                327, 194, 46285, 610, 0, 144, addedPopulation: 8, removedPopulation: 9);
            Assert.That(inconsistent.PopulationReconciled, Is.False);
            Assert.That(inconsistent.ActualPreyScoreStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.Invalid));
        }

        [Test]
        public void SixControlsApplyConfiguredAmountsAndChargeEachDistinctIntentOnce()
        {
            var (definition, manager, progression, session) = CreateSessionParts(
                hareCount: 100, foxCount: 4, plantCount: 20, includeFox: true);
            using (session)
            {
                Assert.That(session.Begin(), Is.True);
                manager.Advance(10.1f);
                Assert.That(session.State, Is.EqualTo(FieldDataJourneyFlowState.RoundDecision));
                Assert.That(session.Ledger.Balance, Is.EqualTo(100));
                Assert.That(session.CanUsePopulationControls, Is.False);
                Assert.That(session.TryBuyPopulationControl("before-reward", FieldDataPopulationInterventionKind.AddHare,
                    out _, out var blockedError), Is.False);
                Assert.That(blockedError, Does.Contain("Resolve or skip"));
                ResolveCurrentReward(session);
                Assert.That(session.CanUsePopulationControls, Is.True);
                Assert.That(session.AvailableFieldData, Is.EqualTo(100));
                Assert.That(session.TryBuyPopulationControl("first-round", FieldDataPopulationInterventionKind.AddHare,
                    out var firstReceipt, out var firstError), Is.True, firstError);
                Assert.That(firstReceipt.Cost, Is.EqualTo(10));
                Assert.That(session.AvailableFieldData, Is.EqualTo(90));
                ReachFirstFundedRoundDecision(manager, session);
                var settledAward = session.Ledger.Awards.Last();
                var balanceBeforeControls = session.Ledger.Balance;

                var before = GetCounts(session.Run);
                var naturalActivityBefore = new[]
                {
                    session.Run.Metrics.GetActivity(Hare),
                    session.Run.Metrics.GetActivity(Fox),
                    session.Run.Metrics.GetActivity(Plant),
                };
                var naturalBirthEventsBefore = session.Run.Metrics.BirthEvents.Count;
                var naturalDeathEventsBefore = session.Run.Metrics.DeathEvents.Count;
                var actions = new[]
                {
                    FieldDataPopulationInterventionKind.AddHare,
                    FieldDataPopulationInterventionKind.RemoveHare,
                    FieldDataPopulationInterventionKind.AddFox,
                    FieldDataPopulationInterventionKind.RemoveFox,
                    FieldDataPopulationInterventionKind.AddPlants,
                    FieldDataPopulationInterventionKind.RemovePlants,
                };

                for (var index = 0; index < actions.Length; index++)
                {
                    var requestId = "intent-" + index;
                    Assert.That(session.TryBuyPopulationControl(requestId, actions[index],
                        out var receipt, out var error), Is.True, error);
                    Assert.That(receipt.Cost, Is.EqualTo(10));
                    Assert.That(receipt.AffectedPositions.Count, Is.EqualTo(
                        actions[index] == FieldDataPopulationInterventionKind.AddPlants
                            || actions[index] == FieldDataPopulationInterventionKind.RemovePlants ? 5 : 1));
                    Assert.That(session.Ledger.Balance, Is.EqualTo(balanceBeforeControls - (index + 1) * 10));

                    Assert.That(session.TryBuyPopulationControl(requestId, actions[index],
                        out var replay, out error), Is.True, error);
                    Assert.That(replay, Is.SameAs(receipt));
                    Assert.That(session.Ledger.Balance, Is.EqualTo(balanceBeforeControls - (index + 1) * 10));
                }

                var balanceBeforeMismatchedReplay = session.Ledger.Balance;
                Assert.That(session.TryBuyPopulationControl("intent-0",
                    FieldDataPopulationInterventionKind.RemoveHare, out _, out var replayError), Is.False);
                Assert.That(replayError, Does.Contain("cannot be reused"));
                Assert.That(session.Ledger.Balance, Is.EqualTo(balanceBeforeMismatchedReplay));

                Assert.That(GetCount(session.Run, Hare), Is.EqualTo(before[Hare]));
                Assert.That(GetCount(session.Run, Fox), Is.EqualTo(before[Fox]));
                Assert.That(GetCount(session.Run, Plant), Is.EqualTo(before[Plant]));

                Assert.That(session.TryBuyPopulationControl("extra-hare-1",
                    FieldDataPopulationInterventionKind.AddHare, out _, out var distinctError), Is.True, distinctError);
                Assert.That(session.TryBuyPopulationControl("extra-hare-2",
                    FieldDataPopulationInterventionKind.AddHare, out _, out distinctError), Is.True, distinctError);
                Assert.That(GetCount(session.Run, Hare), Is.EqualTo(before[Hare] + 2));
                Assert.That(session.PopulationInterventions.Count, Is.EqualTo(actions.Length + 3));
                Assert.That(settledAward.PopulationCounts[Hare], Is.EqualTo(before[Hare]),
                    "Later population controls must not rewrite an already-settled payout snapshot.");
                Assert.That(new[]
                {
                    session.Run.Metrics.GetActivity(Hare),
                    session.Run.Metrics.GetActivity(Fox),
                    session.Run.Metrics.GetActivity(Plant),
                }, Is.EqualTo(naturalActivityBefore), "Paid interventions are not natural ecology events.");
                Assert.That(session.Run.Metrics.BirthEvents.Count, Is.EqualTo(naturalBirthEventsBefore));
                Assert.That(session.Run.Metrics.DeathEvents.Count, Is.EqualTo(naturalDeathEventsBefore));
                Assert.That(progression.Currency, Is.Zero);
            }
        }

        [Test]
        public void RejectedControlsLeaveWalletBoardAndEntityIdsUntouched()
        {
            var (definition, manager, _, session) = CreateSessionParts(hareCount: 1, plantCount: 4);
            using (session)
            {
                Assert.That(session.Begin(), Is.True);
                manager.Advance(10.1f);
                ResolveCurrentReward(session);
                var beforeCells = session.Run.Cells;
                var hare = FindCreature(beforeCells, Hare);
                var hareId = hare.cell.EntityId;
                var balance = session.Ledger.Balance;

                Assert.That(session.TryBuyPopulationControl("remove-last", 
                    FieldDataPopulationInterventionKind.RemoveHare, out _, out var error), Is.False);
                Assert.That(error, Does.Contain("final member"));
                Assert.That(session.TryBuyPopulationControl("insufficient-funds",
                    FieldDataPopulationInterventionKind.AddHare, out _, out error), Is.False);
                Assert.That(error, Does.Contain("not enough Field Data"));

                Assert.That(session.Run.Cells, Is.SameAs(beforeCells));
                Assert.That(FindCreature(session.Run.Cells, Hare).cell.EntityId, Is.EqualTo(hareId));
                Assert.That(session.Ledger.Balance, Is.EqualTo(balance));
                Assert.That(session.PopulationInterventions, Is.Empty);
            }
        }

        [Test]
        public void RejectedPopulationControlDoesNotChangeTheNextPlacementSequence()
        {
            var (_, leftManager, _, left) = CreateSessionParts(hareCount: 20, plantCount: 4);
            var (_, rightManager, _, right) = CreateSessionParts(hareCount: 20, plantCount: 4);
            using (left)
            using (right)
            {
                ReachFirstFundedRoundDecision(leftManager, left);
                ReachFirstFundedRoundDecision(rightManager, right);

                var leftCells = left.Run.Cells;
                var leftBalance = left.Ledger.Balance;
                Assert.That(left.TryBuyPopulationControl("rejected-before-placement",
                    FieldDataPopulationInterventionKind.RemovePlants, out _, out _), Is.False);
                Assert.That(left.Run.Cells, Is.SameAs(leftCells));
                Assert.That(left.Ledger.Balance, Is.EqualTo(leftBalance));

                Assert.That(left.TryBuyPopulationControl("next-placement",
                    FieldDataPopulationInterventionKind.AddHare, out var leftReceipt, out var leftError),
                    Is.True, leftError);
                Assert.That(right.TryBuyPopulationControl("next-placement",
                    FieldDataPopulationInterventionKind.AddHare, out var rightReceipt, out var rightError),
                    Is.True, rightError);

                Assert.That(leftReceipt.AffectedPositions.Count, Is.EqualTo(1));
                Assert.That(leftReceipt.AffectedPositions[0].X, Is.EqualTo(rightReceipt.AffectedPositions[0].X));
                Assert.That(leftReceipt.AffectedPositions[0].Y, Is.EqualTo(rightReceipt.AffectedPositions[0].Y));
            }
        }

        [Test]
        public void PlantRemovalShortageRejectsAtomicallyWithEnoughFunds()
        {
            var (_, manager, _, session) = CreateSessionParts(hareCount: 20, plantCount: 4);
            using (session)
            {
                ReachFirstFundedRoundDecision(manager, session);
                var beforeCells = session.Run.Cells;
                var beforeBalance = session.Ledger.Balance;
                Assert.That(beforeBalance, Is.GreaterThanOrEqualTo(10));

                Assert.That(session.TryBuyPopulationControl("plant-shortage",
                    FieldDataPopulationInterventionKind.RemovePlants, out _, out var error), Is.False);
                Assert.That(error, Does.Contain("not enough matching"));
                Assert.That(session.Run.Cells, Is.SameAs(beforeCells));
                Assert.That(session.Ledger.Balance, Is.EqualTo(beforeBalance));
                Assert.That(session.PopulationInterventions, Is.Empty);
            }
        }

        [Test]
        public void PlantAdditionRejectsInsufficientBatchSpaceWithoutCharging()
        {
            var (_, manager, _, session) = CreateSessionParts(
                hareCount: 20, plantCount: 20, maxPopulationOverride: 40);
            using (session)
            {
                ReachFirstFundedRoundDecision(manager, session);
                Assert.That(session.Ledger.Balance, Is.GreaterThanOrEqualTo(10));
                var cellsBefore = session.Run.Cells;
                var balanceBefore = session.Ledger.Balance;

                Assert.That(session.TryBuyPopulationControl("add-five-plants-no-space",
                    FieldDataPopulationInterventionKind.AddPlants, out _, out var error), Is.False);
                Assert.That(error, Does.Contain("maximum population"));
                Assert.That(session.Run.Cells, Is.SameAs(cellsBefore));
                Assert.That(session.Ledger.Balance, Is.EqualTo(balanceBefore));
                Assert.That(session.PopulationInterventions, Is.Empty);
            }
        }

        [Test]
        public void PopulationCapacityCountsSharedCreatureAndPlantLayers()
        {
            var (_, manager, _, session) = CreateSessionParts(
                hareCount: 20, plantCount: 0, maxPopulationOverride: 21);
            using (session)
            {
                ReachFirstFundedRoundDecision(manager, session);

                var shared = FindCreature(session.Run.Cells, Hare);
                session.Run.Cells.SetCell(shared.x, shared.y,
                    shared.cell.WithPlantResource(TerrainDefaults.Grass, Plant, 20f));
                var cellsBefore = session.Run.Cells;
                var balanceBefore = session.Ledger.Balance;

                Assert.That(session.TryBuyPopulationControl("shared-layer-capacity",
                    FieldDataPopulationInterventionKind.AddHare, out _, out var error), Is.False);
                Assert.That(error, Does.Contain("maximum population"));
                Assert.That(session.Run.Cells, Is.SameAs(cellsBefore));
                Assert.That(session.Ledger.Balance, Is.EqualTo(balanceBefore));
                Assert.That(session.PopulationInterventions, Is.Empty);
            }
        }

        [Test]
        public void PlantAdditionDoesNotOverwriteExistingPlantResourcesToFillBatch()
        {
            var (_, manager, _, session) = CreateSessionParts(hareCount: 20, plantCount: 4);
            using (session)
            {
                ReachFirstFundedRoundDecision(manager, session);

                // Leave only four cells without a plant resource. The request must
                // reject the full five-Plant batch instead of overwriting existing plants.
                var grid = session.Run.Cells;
                var availablePositions = new List<FieldDataPopulationPosition>();
                for (var y = 0; y < grid.Height; y++)
                {
                    for (var x = 0; x < grid.Width; x++)
                    {
                        var cell = grid.GetCell(x, y);
                        if (!cell.IsPassable)
                            continue;

                        if (!cell.IsPlantResource)
                            grid.SetCell(x, y, cell.WithPlantResource(
                                TerrainDefaults.Grass, Plant, 20f));
                        availablePositions.Add(new FieldDataPopulationPosition(x, y));
                    }
                }

                Assert.That(availablePositions.Count, Is.GreaterThan(5));
                for (var index = 0; index < 4; index++)
                {
                    var position = availablePositions[index];
                    var cell = grid.GetCell(position.X, position.Y);
                    grid.SetCell(position.X, position.Y, cell.WithoutPlantResource());
                }

                var cellsBefore = session.Run.Cells;
                var balanceBefore = session.Ledger.Balance;
                Assert.That(session.TryBuyPopulationControl("four-plant-spaces",
                    FieldDataPopulationInterventionKind.AddPlants, out _, out var error), Is.False);
                Assert.That(error, Does.Contain("not enough valid cells"));
                Assert.That(session.Run.Cells, Is.SameAs(cellsBefore));
                Assert.That(session.Ledger.Balance, Is.EqualTo(balanceBefore));
                Assert.That(session.PopulationInterventions, Is.Empty);
            }
        }

        [Test]
        public void PlantRemovalPreservesCreatureAndAnimalRemovalPreservesPlantResource()
        {
            var (definition, manager, _, session) = CreateSessionParts(hareCount: 20, plantCount: 4);
            var initialHares = FindCreatures(session.Run.Cells, Hare);
            var shared = initialHares[0];
            var plant = Plant;
            session.Run.Cells.SetCell(shared.x, shared.y,
                shared.cell.WithPlantResource(TerrainDefaults.Grass, plant, 20f));
            using (session)
            {
                ReachFirstFundedRoundDecision(manager, session);
                var sharedCell = session.Run.Cells.GetCell(shared.x, shared.y);
                var sharedEntity = sharedCell.EntityId;

                Assert.That(session.TryBuyPopulationControl("remove-five-plants",
                    FieldDataPopulationInterventionKind.RemovePlants, out var plantRemoval, out var error), Is.True, error);
                Assert.That(plantRemoval.AffectedPositions.Count, Is.EqualTo(5));
                var afterPlantRemoval = session.Run.Cells.GetCell(shared.x, shared.y);
                Assert.That(afterPlantRemoval.IsCreature, Is.True);
                Assert.That(afterPlantRemoval.EntityId, Is.EqualTo(sharedEntity));
                Assert.That(afterPlantRemoval.IsPlantResource, Is.False);

                // Put a plant resource beneath every remaining Hare so a random Hare removal
                // always exercises the layer-preserving path.
                for (var y = 0; y < session.Run.Cells.Height; y++)
                {
                    for (var x = 0; x < session.Run.Cells.Width; x++)
                    {
                        var cell = session.Run.Cells.GetCell(x, y);
                        if (cell.IsCreature && cell.SpeciesId == Hare)
                        {
                            session.Run.Cells.SetCell(x, y,
                                cell.WithPlantResource(TerrainDefaults.Grass, plant, 20f));
                        }
                    }
                }

                Assert.That(session.TryBuyPopulationControl("remove-one-hare",
                    FieldDataPopulationInterventionKind.RemoveHare, out var hareRemoval, out error), Is.True, error);
                var removedAt = hareRemoval.AffectedPositions[0];
                var cellAfter = session.Run.Cells.GetCell(removedAt.X, removedAt.Y);
                Assert.That(cellAfter.IsCreature, Is.False);
                Assert.That(cellAfter.IsPlantResource, Is.True);
                Assert.That(cellAfter.TerrainId, Is.EqualTo(TerrainIds.Grass));
            }
        }

        [Test]
        public void PopulationControlsCloseAtNodeSettlementAndStayClosedInShop()
        {
            var (definition, manager, _, session) = CreateSessionParts(
                hareCount: 20, plantCount: 10, foxCount: 4, includeFox: true);
            using (session)
            {
                Assert.That(session.Begin(), Is.True);
                for (var boundary = 1; boundary <= 6; boundary++)
                {
                    manager.Advance(10.1f);
                    if (boundary < 6)
                    {
                        var choices = session.GetCurrentRoundRewardChoices();
                        if (choices != null && !choices.Resolved)
                            session.ResolveRewardChoice(choices.SourceId, "skip", out _);
                        Assert.That(session.Continue(), Is.True);
                    }
                }

                Assert.That(session.State, Is.EqualTo(FieldDataJourneyFlowState.Shop));
                var wallet = session.Ledger.Balance;
                var board = session.Run.Cells;
                Assert.That(session.CanUsePopulationControls, Is.False);
                ResolveCurrentReward(session);
                Assert.That(session.CanUsePopulationControls, Is.False);
                Assert.That(session.TryBuyPopulationControl("round-six-add-fox",
                    FieldDataPopulationInterventionKind.AddFox, out _, out _), Is.False);
                Assert.That(session.Ledger.Balance, Is.EqualTo(wallet));
                Assert.That(session.Run.Cells, Is.SameAs(board));
                Assert.That(session.BeginShopPurchases(), Is.True);
                Assert.That(session.CanUsePopulationControls, Is.False);
                Assert.That(session.TryBuyPopulationControl("too-late",
                    FieldDataPopulationInterventionKind.AddHare, out _, out var error), Is.False);
                Assert.That(error, Does.Contain("between rounds within a simulation node"));
            }
        }

        static (FieldDataJourneyDefinition, SimulationManager, SpeciesProgression, FieldDataJourneySession)
            CreateSessionParts(int hareCount, int plantCount, int foxCount = 0, bool includeFox = false,
                int maxPopulationOverride = 0, System.Func<string, bool> mutationResearch = null,
                GenomeSimulationSnapshot activeGenome = null)
        {
            var asset = AssetDatabase.LoadAssetAtPath<FieldDataJourneyDefinitionAsset>(DefinitionPath);
            Assert.That(asset.TryResolve(out var definition, out var error), Is.True, error);
            var scenario = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(ScenarioPath).CreateRuntimeData();
            var hare = new SpeciesId("hare");
            var fox = new SpeciesId("fox");
            var plant = new SpeciesId("plant");
            var defaultHare = scenario.SpeciesRules[hare];
            var defaultFox = scenario.SpeciesRules[fox];
            var defaultPlant = scenario.SpeciesRules[plant];
            var hareRules = new SpeciesRules(
                movementSpeed: 0f, movementPattern: defaultHare.MovementPattern,
                attackPattern: defaultHare.AttackPattern, attackAmount: 0,
                blockPattern: defaultHare.BlockPattern, blockAmount: 0,
                dietPattern: defaultHare.DietPattern, dietTarget: plant,
                reproductionPattern: defaultHare.ReproductionPattern, reproductionNeighborCount: 1,
                reproductionChance: 0f, startingEnergy: 1000, metabolism: 0,
                role: SpeciesRole.Herbivore, maximumEnergy: 1000, energyLossIntervalTicks: 1);
            var foxRules = new SpeciesRules(
                movementSpeed: 0f, movementPattern: defaultFox.MovementPattern,
                attackPattern: defaultFox.AttackPattern, attackAmount: 0,
                blockPattern: defaultFox.BlockPattern, blockAmount: 0,
                dietPattern: defaultFox.DietPattern, dietTarget: plant,
                reproductionPattern: defaultFox.ReproductionPattern, reproductionNeighborCount: 1,
                reproductionChance: 0f, startingEnergy: 1000, metabolism: 0,
                role: SpeciesRole.Carnivore, maximumEnergy: 1000, energyLossIntervalTicks: 1);
            var plantRules = new SpeciesRules(
                movementSpeed: 0f, movementPattern: defaultPlant.MovementPattern,
                attackPattern: defaultPlant.AttackPattern, attackAmount: 0,
                blockPattern: defaultPlant.BlockPattern, blockAmount: 0,
                dietPattern: defaultPlant.DietPattern, dietTarget: null,
                reproductionPattern: defaultPlant.ReproductionPattern, reproductionNeighborCount: 1,
                reproductionChance: 0f, startingFoodReserve: 20f, metabolism: 0,
                role: SpeciesRole.Plant);

            if (!includeFox)
            {
                scenario = scenario.WithoutSpecies(fox);
            }

            scenario = scenario.WithSpeciesRules(hare, hareRules)
                .WithSpeciesRules(plant, plantRules)
                .WithStartingProbability(hare, 0f)
                .WithStartingProbability(plant, 0f)
                .WithStartingPopulations(new Dictionary<SpeciesId, int>
                {
                    [hare] = hareCount,
                    [plant] = plantCount,
                });

            if (includeFox)
            {
                scenario = scenario.WithSpeciesRules(fox, foxRules)
                    .WithStartingProbability(fox, 0f)
                    .WithStartingPopulations(new Dictionary<SpeciesId, int>
                    {
                        [hare] = hareCount,
                        [fox] = foxCount,
                        [plant] = plantCount,
                    });
            }

            var targetTicks = definition.RoundLength * definition.RoundsPerNode * definition.RouteSimulationCount;
            scenario = scenario.WithRunTicks(targetTicks, 0.1f);
            var run = new SimulationRunState(
                SpeciesInitialGridFactory.Create(scenario, 10100), hare, 10100, scenario.RunDurationSeconds, targetTicks, activeGenome);
            run.ConfigureContinuousPhases(100);
            var manager = new SimulationManager();
            manager.SetRunner(maxPopulationOverride > 0
                ? new SpeciesSimulationRunner(run, scenario.SpeciesRules,
                    scenario.StepInterval, maxPopulation: maxPopulationOverride)
                : new SpeciesSimulationRunner(run, scenario));
            var progression = new SpeciesProgression(new SpeciesDefinition(hare, scenario.SpeciesRules[hare]), mutationResearch);
            var session = new FieldDataJourneySession(definition, manager, progression);
            return (definition, manager, progression, session);
        }

        static void ReachFirstFundedRoundDecision(SimulationManager manager, FieldDataJourneySession session)
        {
            if (session.State == FieldDataJourneyFlowState.Ready)
                Assert.That(session.Begin(), Is.True);
            if (session.State == FieldDataJourneyFlowState.RoundDecision)
            {
                ResolveCurrentReward(session);
                Assert.That(session.Continue(), Is.True);
            }

            for (var round = session.GlobalRound + 1; round <= FieldDataJourneyDefinition.RoundsPerSimulation; round++)
            {
                manager.Advance(10.1f);
                if (round < FieldDataJourneyDefinition.RoundsPerSimulation)
                {
                    Assert.That(session.State, Is.EqualTo(FieldDataJourneyFlowState.RoundDecision));
                    ResolveCurrentReward(session);
                    Assert.That(session.Continue(), Is.True);
                }
            }

            Assert.That(session.State, Is.EqualTo(FieldDataJourneyFlowState.Shop));
            ResolveCurrentReward(session);
            Assert.That(session.AvailableFieldData, Is.GreaterThan(0));
            Assert.That(session.BeginShopPurchases(), Is.True);
            Assert.That(session.Continue(), Is.True);
            manager.Advance(10.1f);
            ResolveCurrentReward(session);
            Assert.That(session.GlobalRound, Is.EqualTo(FieldDataJourneyDefinition.RoundsPerSimulation + 1));
            Assert.That(session.CanUsePopulationControls, Is.True);
        }

        static void ResolveCurrentReward(FieldDataJourneySession session)
        {
            var choices = session.GetCurrentRoundRewardChoices() ?? session.GetCurrentNodeRewardChoices();
            if (choices != null && !choices.Resolved)
                Assert.That(session.ResolveRewardChoice(choices.SourceId, "skip", out var error), Is.True, error);
        }

        static Dictionary<SpeciesId, int> GetCounts(SimulationRunState run)
        {
            var snapshot = run.PopulationHistory[run.PopulationHistory.Count - 1];
            return new Dictionary<SpeciesId, int>
            {
                [Hare] = snapshot.GetCount(Hare),
                [Fox] = snapshot.GetCount(Fox),
                [Plant] = snapshot.GetCount(Plant),
            };
        }

        static int GetCount(SimulationRunState run, SpeciesId species)
        {
            return run.PopulationHistory[run.PopulationHistory.Count - 1].GetCount(species);
        }

        static (int x, int y, SpeciesCell cell) FindCreature(Grid<SpeciesCell> grid, SpeciesId species)
        {
            for (var y = 0; y < grid.Height; y++)
            {
                for (var x = 0; x < grid.Width; x++)
                {
                    var cell = grid.GetCell(x, y);
                    if (cell.IsCreature && cell.SpeciesId == species)
                    {
                        return (x, y, cell);
                    }
                }
            }

            Assert.Fail("Expected a creature of " + species + " on the board.");
            return default;
        }

        static List<(int x, int y, SpeciesCell cell)> FindCreatures(Grid<SpeciesCell> grid, SpeciesId species)
        {
            var results = new List<(int, int, SpeciesCell)>();
            for (var y = 0; y < grid.Height; y++)
            {
                for (var x = 0; x < grid.Width; x++)
                {
                    var cell = grid.GetCell(x, y);
                    if (cell.IsCreature && cell.SpeciesId == species)
                    {
                        results.Add((x, y, cell));
                    }
                }
            }

            return results;
        }
    }
}
