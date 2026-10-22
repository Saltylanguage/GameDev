using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UnityEngine;

namespace SaltyGame.EditorTests
{
    public sealed class FieldDataReplayParityTests
    {
        [Test]
        public void FrozenJourneyActionsMatchUnityAtEveryDecisionAndTerminalState()
        {
            var manifestPath = Environment.GetEnvironmentVariable("CELLSIM_FIELD_DATA_PARITY_MANIFEST");
            if (string.IsNullOrWhiteSpace(manifestPath))
                Assert.Ignore("Set CELLSIM_FIELD_DATA_PARITY_MANIFEST to a retained snapshot/actions/replay manifest.");
            var manifest = JObject.Parse(File.ReadAllText(manifestPath));
            var exporter = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType("SaltyGame.EditorTools.CellularSimulationExperimentRunner"))
                .First(type => type != null);
            var snapshotType = exporter.GetNestedType("PortableSnapshot");
            var snapshotJson = JObject.Parse(File.ReadAllText((string)manifest["snapshot"]));
            var portable = JsonConvert.DeserializeObject(snapshotJson.ToString(), snapshotType);
            var data = (CellularSimData)snapshotType.GetMethod("Restore").Invoke(portable, null);
            var portableDefinition = (FieldDataPortableDefinition)snapshotType.GetField("fieldDataDefinition").GetValue(portable);
            Assert.That(portableDefinition.TryRestore(out var definition, out var error), Is.True, error);
            var sourceHash = (string)exporter.GetMethod("PortableSourceHash").Invoke(null, new object[]
            {
                new Func<string, string>(relative => File.ReadAllText(Path.Combine(Application.dataPath, "..", relative)))
            });
            Assert.That(sourceHash, Is.EqualTo((string)snapshotJson["sourceHash"]), "The frozen export must match current shared sources.");
            var upgrades = ((Array)snapshotType.GetField("fieldDataOrdinaryRewards").GetValue(portable)).Cast<object>()
                .Select(item => (SpeciesUpgradeSnapshot)item.GetType().GetMethod("Restore").Invoke(item, null)).ToArray();
            string Digest(SimulationRunState run) => (string)exporter.GetMethod("PortableStateDigest", BindingFlags.Static | BindingFlags.NonPublic)
                .Invoke(null, new object[] { run.Cells });

            foreach (var fixture in manifest["fixtures"])
            {
                var expected = JObject.Parse(File.ReadAllText((string)fixture["report"]));
                var schedule = JObject.Parse(File.ReadAllText((string)fixture["actions"]));
                Assert.That((int)expected["schemaVersion"], Is.EqualTo(1));
                Assert.That((string)expected["reportType"], Is.EqualTo("FieldDataJourneyReplay"));
                Assert.That((string)expected["sourceHash"], Is.EqualTo(sourceHash));
                Assert.That((string)expected["definitionFingerprint"], Is.EqualTo(definition.Fingerprint));
                Assert.That((string)expected["scenarioFingerprint"], Is.EqualTo(data.Fingerprint));
                var selected = new SpeciesId(definition.SelectedSpeciesId);
                var seed = (int)expected["seed"];
                var run = new SimulationRunState(SpeciesInitialGridFactory.Create(data, seed), selected, seed,
                    data.RunDurationSeconds, 1800);
                run.ConfigureContinuousPhases(100);
                var manager = new SimulationManager();
                manager.SetRunner(new SpeciesSimulationRunner(run, data));
                var progression = new SpeciesProgression(new SpeciesDefinition(selected, data.SpeciesRules[selected]));
                using (var session = new FieldDataJourneySession(definition, manager, progression))
                {
                    Assert.That(session.SetOrdinaryRewardPool(upgrades.Where(upgrade => upgrade.TargetSpecies == selected)), Is.True);
                    Assert.That(session.Begin(), Is.True);
                    var boundaryIndex = 0;
                    var actionIndex = 0;
                    while (session.State == FieldDataJourneyFlowState.Running)
                    {
                        manager.Advance(data.StepInterval);
                        if (session.State == FieldDataJourneyFlowState.Running) continue;
                        var terminal = session.State == FieldDataJourneyFlowState.Failed
                            || session.State == FieldDataJourneyFlowState.Succeeded;
                        JToken boundary = null;
                        if (!terminal)
                        {
                            boundary = expected["boundaries"][boundaryIndex++];
                            Assert.That(run.Tick, Is.EqualTo((int)boundary["tick"]));
                            Assert.That(Digest(run), Is.EqualTo((string)boundary["boardDigestBeforeActions"]));
                            Assert.That(session.Ledger.Balance, Is.EqualTo((int)boundary["balanceBeforeActions"]));
                            Assert.That(session.CurrentIncomeExecutionOrder.Select(item => item.InstanceId),
                                Is.EqualTo(boundary["incomeOrderBeforeActions"].Values<string>()));
                            var choices = session.GetCurrentRoundRewardChoices() ?? session.GetCurrentNodeRewardChoices();
                            Assert.That(choices.Choices.Select(choice => choice.Id), Is.EqualTo(boundary["pendingRewardChoices"].Values<string>()));
                            if (session.State == FieldDataJourneyFlowState.Shop)
                            {
                                var offers = session.GetCurrentShopVisit().Offers;
                                Assert.That(offers.Select(offer => offer.Id), Is.EqualTo(boundary["shopOffers"].Select(offer => (string)offer["id"])));
                                Assert.That(offers.Select(offer => offer.Cost), Is.EqualTo(boundary["shopOffers"].Select(offer => (int)offer["cost"])));
                                Assert.That(offers.Select(offer => offer.Locked), Is.EqualTo(boundary["shopOffers"].Select(offer => (bool)offer["locked"])));
                            }
                        }
                        foreach (var action in schedule["actions"].Where(action => (int)action["tick"] == run.Tick))
                        {
                            var actual = Execute(session, action);
                            var recorded = expected["scheduledActions"][actionIndex++];
                            foreach (var fact in actual.Properties())
                                Assert.That(JToken.DeepEquals(fact.Value, recorded[fact.Name]), Is.True,
                                    $"Seed {seed}, tick {run.Tick}, {action["type"]}, {fact.Name}: {fact.Value} != {recorded[fact.Name]}");
                        }
                        if (!terminal)
                        {
                            var choices = session.GetCurrentRoundRewardChoices() ?? session.GetCurrentNodeRewardChoices();
                            if (choices != null && !choices.Resolved)
                            {
                                Assert.That(session.ResolveRewardChoice(choices.SourceId, "skip", out error), Is.True, error);
                                Assert.That((string)expected["scheduledActions"][actionIndex++]["type"], Is.EqualTo("auto-skip-choice"));
                            }
                            Assert.That(Digest(run), Is.EqualTo((string)boundary["boardDigestAfterActions"]));
                            Assert.That(session.Ledger.Balance, Is.EqualTo((int)boundary["balanceAfterActions"]));
                            Assert.That(session.CurrentIncomeExecutionOrder.Select(item => item.InstanceId),
                                Is.EqualTo(boundary["incomeOrderAfterActions"].Values<string>()));
                            Assert.That(session.Continue(), Is.True);
                        }
                    }
                    Assert.That(boundaryIndex, Is.EqualTo(expected["boundaries"].Count()));
                    Assert.That(Digest(run), Is.EqualTo((string)expected["finalBoardDigest"]));
                    Assert.That(run.Tick, Is.EqualTo((int)expected["finalTick"]));
                    foreach (var kind in new[] { FieldDataAwardKind.PopulationPayout, FieldDataAwardKind.NodeClear })
                    {
                        var awards = session.Ledger.Awards.Where(award => award.Kind == kind).ToArray();
                        var recorded = expected[kind == FieldDataAwardKind.PopulationPayout ? "populationAwards" : "nodeClearAwards"];
                        Assert.That(awards.Length, Is.EqualTo(recorded.Count()));
                        for (var i = 0; i < awards.Length; i++)
                        {
                            Assert.That(awards[i].Amount, Is.EqualTo((int)recorded[i]["amount"]));
                            Assert.That(awards[i].BalanceBefore, Is.EqualTo((int)recorded[i]["balanceBefore"]));
                            Assert.That(awards[i].BalanceAfter, Is.EqualTo((int)recorded[i]["balanceAfter"]));
                            foreach (var population in awards[i].PopulationCounts)
                                Assert.That(population.Value, Is.EqualTo((int)recorded[i]["populations"][population.Key.Value]));
                            if (awards[i].Income == null) continue;
                            var steps = awards[i].Income.Steps;
                            Assert.That(steps.Count, Is.EqualTo(recorded[i]["incomeSteps"].Count()));
                            for (var step = 0; step < steps.Count; step++)
                            {
                                Assert.That(steps[step].InstanceId, Is.EqualTo((string)recorded[i]["incomeSteps"][step]["instanceId"]));
                                Assert.That(steps[step].InputAmount, Is.EqualTo((decimal)recorded[i]["incomeSteps"][step]["inputAmount"]));
                                Assert.That(steps[step].OutputAmount, Is.EqualTo((decimal)recorded[i]["incomeSteps"][step]["outputAmount"]));
                            }
                        }
                    }
                    Assert.That(session.IncomeAcquisitionHistory.Select(item => item.InstanceId), Is.EqualTo(expected["acquisitions"].Select(item => (string)item["instanceId"])));
                    Assert.That(session.IncomeReorderHistory.Select(item => item.Cost), Is.EqualTo(expected["reorders"].Select(item => (int)item["cost"])));
                    Assert.That(session.CurrentIncomeExecutionOrder.Select(item => item.InstanceId), Is.EqualTo(expected["effectiveIncomeOrder"].Values<string>()));
                    if ((bool)schedule["exitSuccessfulResults"] && session.State == FieldDataJourneyFlowState.Succeeded)
                    {
                        Assert.That(session.TryExitSuccessfulResults(out var discarded), Is.True);
                        Assert.That(discarded, Is.EqualTo((int)expected["successfulExitDiscardedFieldData"]));
                    }
                    Assert.That(session.State.ToString(), Is.EqualTo((string)expected["terminalState"]));
                    Assert.That(session.Ledger.Balance, Is.EqualTo((int)expected["finalBalance"]));
                    Assert.That(session.FieldDataLostOnFailure, Is.EqualTo((int)expected["fieldDataLostOnFailure"]));
                }
            }
        }

        static JObject Execute(FieldDataJourneySession session, JToken action)
        {
            var before = session.Ledger.Balance;
            var request = (string)action["requestId"];
            var type = (string)action["type"];
            bool succeeded;
            var result = new JObject { ["tick"] = session.Run.Tick, ["type"] = type, ["balanceBefore"] = before };
            switch (type)
            {
                case "resolve-choice":
                    var choices = session.GetCurrentRoundRewardChoices() ?? session.GetCurrentNodeRewardChoices();
                    succeeded = choices != null && session.ResolveRewardChoice(choices.SourceId, (string)action["choiceId"], out _);
                    break;
                case "population":
                    succeeded = session.TryBuyPopulationControl(request, (FieldDataPopulationInterventionKind)Enum.Parse(typeof(FieldDataPopulationInterventionKind), (string)action["controlKind"]), out var population, out _);
                    if (population != null)
                    {
                        result["receiptId"] = population.RequestId;
                        result["cost"] = population.Cost;
                        result["populationDelta"] = population.Quantity;
                        result["affectedPositions"] = JArray.FromObject(population.AffectedPositions.Select(position => new[] { position.X, position.Y }));
                    }
                    break;
                case "shop-offer":
                    succeeded = session.TryBuyShopOffer(request, (string)action["offerId"], out _);
                    result["receiptId"] = succeeded ? request : string.Empty;
                    break;
                case "reroll":
                    succeeded = session.TryRerollCappedShopOffer((string)action["offerId"], out var replacement, out _);
                    result["receiptId"] = replacement?.Id ?? string.Empty;
                    break;
                case "random-reorder":
                    succeeded = session.TryBuyRandomIncomeReorder(request, out var random, out _);
                    if (random != null)
                    {
                        result["cost"] = random.Cost;
                        result["previousOrder"] = JArray.FromObject(random.PreviousOrder);
                        result["nextOrder"] = JArray.FromObject(random.NextOrder);
                    }
                    break;
                case "chosen-reorder":
                    succeeded = session.TryCreateChosenIncomeReorderDraft(out _, out _)
                        && session.TryConfirmChosenIncomeReorder(request, action["orderIds"].Values<string>().ToArray(), out _, out _);
                    break;
                default: throw new InvalidOperationException("Unsupported parity fixture action: " + type);
            }
            result["succeeded"] = succeeded;
            result["status"] = succeeded ? "succeeded" : "rejected";
            result["balanceAfter"] = session.Ledger.Balance;
            result["chargedFieldData"] = before - session.Ledger.Balance;
            return result;
        }
    }
}
