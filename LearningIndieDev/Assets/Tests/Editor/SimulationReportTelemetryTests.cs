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
