using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using SaltyGame;
using UnityEditor;
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

        [Test]
        public void SpatialObserverCountsTravelCrowdingAndFoxProximityWithoutChangingGrid()
        {
            var hare = new SpeciesId("hare");
            var fox = new SpeciesId("fox");
            var grid = new Grid<SpeciesCell>(4, 3, (_, _) => SpeciesCell.Empty);
            for (var y = 0; y < 3; y++)
            {
                for (var x = 0; x < 3; x++)
                {
                    grid.SetCell(x, y, new SpeciesCell(x == 0 && y == 0 ? fox : hare));
                }
            }

            var mover = grid.GetCell(2, 2);
            var rules = AssetDatabase.LoadAssetAtPath<SpeciesDefinitionAsset>(
                "Assets/Data/ProductionData/CellularSimulation/Species/hare.asset").CreateRules();
            var observerType = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType(
                    "SaltyGame.EditorTools.CellularSimulationExperimentRunner+SpatialObserver"))
                .First(type => type != null);
            var observer = Activator.CreateInstance(
                observerType,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                new object[] { grid, hare, fox },
                null);
            observerType.GetMethod("Observe").Invoke(observer, new object[] { grid, rules, 1 });
            var next = grid.Copy();
            next.SetCell(2, 2, SpeciesCell.Empty);
            next.SetCell(3, 2, mover);
            observerType.GetMethod("Observe").Invoke(observer, new object[] { next, rules, 2 });
            var record = observerType.GetMethod("CreateRecord").Invoke(observer, null);
            var spatial = JsonUtility.FromJson<SpatialReport>(JsonUtility.ToJson(record));

            Assert.That(spatial.distinctCells, Is.EqualTo(9));
            Assert.That(spatial.distanceCells, Is.EqualTo(1));
            Assert.That(spatial.hareObservations, Is.EqualTo(16));
            Assert.That(spatial.crowdedObservations, Is.EqualTo(1));
            Assert.That(spatial.foxAdjacentObservations, Is.EqualTo(6));
            Assert.That(spatial.phaseWindows.Select(phase => phase.distinctCells), Is.EqualTo(new[] { 8, 8 }));
            Assert.That(grid.GetCell(2, 2).EntityId, Is.EqualTo(mover.EntityId));
        }

        [Serializable]
        sealed class ActivityReport
        {
            public int reproductionEligibleAttempts;
            public int reproductionBlockedMateRequirement;
            public int reproductionSuccessfulAttempts;
            public bool reproductionReconciled;
        }

        [Serializable]
        sealed class SpatialReport
        {
            public int distinctCells;
            public int distanceCells;
            public int hareObservations;
            public int crowdedObservations;
            public int foxAdjacentObservations;
            public SpatialPhaseReport[] phaseWindows;
        }

        [Serializable]
        sealed class SpatialPhaseReport
        {
            public int distinctCells;
        }
    }
}
