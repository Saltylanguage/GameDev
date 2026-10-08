using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using SaltyGame;
using UnityEngine;

namespace SaltyGame.EditorTests
{
    public sealed class SimulationReportTelemetryTests
    {
        [Test]
        public void SummaryTelemetryPreservesStateAndPopulationAccounting()
        {
            var rules = SpeciesRuleDefaults.Create();
            var data = new CellularSimData(8, 8, new System.Collections.Generic.Dictionary<SpeciesId, float>
            {
                [SpeciesIds.Plant] = 0.3f,
                [SpeciesIds.Herbivore] = 0.1f,
                [SpeciesIds.Carnivore] = 0.03f,
            }, rules, 4f, 0.1f);
            var compactRun = new SimulationRunState(SpeciesInitialGridFactory.Create(data, 42), SpeciesIds.Herbivore, 42, 4f);
            var detailedRun = new SimulationRunState(SpeciesInitialGridFactory.Create(data, 42), SpeciesIds.Herbivore, 42, 4f);
            compactRun.Metrics.CaptureDetailedEvents = false;
            var compact = new SpeciesSimulationRunner(compactRun, data);
            var detailed = new SpeciesSimulationRunner(detailedRun, data);
            while (detailed.AdvanceOneTick())
            {
                Assert.That(compact.AdvanceOneTick(), Is.True);
                foreach (var species in rules.Keys)
                {
                    Assert.That(compactRun.Metrics.GetActivity(species), Is.EqualTo(detailedRun.Metrics.GetActivity(species)));
                    Assert.That(compactRun.Metrics.GetReproductionActivity(species), Is.EqualTo(detailedRun.Metrics.GetReproductionActivity(species)));
                }
                for (var y = 0; y < data.Height; y++)
                {
                    for (var x = 0; x < data.Width; x++)
                    {
                        var left = compactRun.Cells.GetCell(x, y);
                        var right = detailedRun.Cells.GetCell(x, y);
                        Assert.That(left.SpeciesId, Is.EqualTo(right.SpeciesId));
                        Assert.That(left.Health, Is.EqualTo(right.Health));
                        Assert.That(left.Energy, Is.EqualTo(right.Energy));
                        Assert.That(left.BehaviorState, Is.EqualTo(right.BehaviorState));
                    }
                }
            }
            Assert.That(compact.AdvanceOneTick(), Is.False);
            Assert.That(compactRun.Metrics.CombatRollEvents, Is.Empty);
            foreach (var species in rules.Keys)
            {
                var start = compactRun.PopulationHistory[0].GetCount(species);
                var end = compactRun.PopulationHistory[compactRun.PopulationHistory.Count - 1].GetCount(species);
                if (rules[species].Role == SpeciesRole.Herbivore)
                {
                    Assert.That(compactRun.Metrics.CreateHerbivoreStatLine(species, start, end).FinalPopulation,
                        Is.EqualTo(detailedRun.Metrics.CreateHerbivoreStatLine(species, start, end).FinalPopulation));
                }
            }
        }

        [Test]
        public void ActivityReportSerializesEligibleReproductionAttempts()
        {
            var metrics = new SpeciesSimulationMetrics();
            var recordOutcome = typeof(SpeciesSimulationMetrics).GetMethod(
                "RecordReproductionOutcome",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(recordOutcome, Is.Not.Null);
            recordOutcome.Invoke(metrics, new object[]
            {
                SpeciesIds.Carnivore,
                SpeciesReproductionOutcome.FailedChanceRoll,
            });
            recordOutcome.Invoke(metrics, new object[]
            {
                SpeciesIds.Carnivore,
                SpeciesReproductionOutcome.SuccessfulAttempt,
            });
            recordOutcome.Invoke(metrics, new object[]
            {
                SpeciesIds.Carnivore,
                SpeciesReproductionOutcome.BlockedMateRequirement,
            });

            var serializer = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType(
                    "SaltyGame.EditorTools.SimulationReportSerialization"))
                .FirstOrDefault(type => type != null);
            Assert.That(serializer, Is.Not.Null);
            var createActivity = serializer.GetMethod("CreateActivity", BindingFlags.Public | BindingFlags.Static);
            var records = (Array)createActivity.Invoke(null, new object[]
            {
                metrics,
                new[] { SpeciesIds.Carnivore },
            });

            var report = JsonUtility.FromJson<ActivityReport>(JsonUtility.ToJson(records.GetValue(0)));

            Assert.That(report.reproductionEligibleAttempts, Is.EqualTo(2));
            Assert.That(report.reproductionBlockedMateRequirement, Is.EqualTo(1));
            Assert.That(report.reproductionSuccessfulAttempts, Is.EqualTo(1));
            Assert.That(report.reproductionReconciled, Is.True);
        }

        [Serializable]
        sealed class ActivityReport
        {
            public int reproductionEligibleAttempts;
            public int reproductionBlockedMateRequirement;
            public int reproductionSuccessfulAttempts;
            public bool reproductionReconciled;
        }
    }
}
