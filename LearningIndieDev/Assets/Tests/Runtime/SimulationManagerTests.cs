using System.Collections.Generic;
using NUnit.Framework;

namespace SaltyGame.Tests
{
    public sealed class SimulationManagerTests
    {
        [Test]
        public void AdvanceUsesFixedStepAndRaisesCompletionOnce()
        {
            var manager = CreateManager();
            var completionCount = 0;
            manager.RunCompleted += _ => completionCount++;

            Assert.That(manager.Start(), Is.True);
            manager.Advance(0.09f);
            Assert.That(manager.Run.Tick, Is.EqualTo(0));

            manager.Advance(0.02f);
            Assert.That(manager.Run.Tick, Is.EqualTo(1));

            manager.Advance(0.1f);
            Assert.That(manager.Run.Status, Is.EqualTo(SimulationRunStatus.Complete));
            Assert.That(completionCount, Is.EqualTo(1));

            manager.Advance(1f);
            Assert.That(completionCount, Is.EqualTo(1));
        }

        [Test]
        public void PauseStopsAdvancementUntilResume()
        {
            var manager = CreateManager(durationSeconds: 1f);

            Assert.That(manager.Start(), Is.True);
            manager.Advance(0.1f);
            Assert.That(manager.Run.Tick, Is.EqualTo(1));

            Assert.That(manager.Pause(), Is.True);
            manager.Advance(1f);
            Assert.That(manager.Run.Tick, Is.EqualTo(1));
            Assert.That(manager.Run.Status, Is.EqualTo(SimulationRunStatus.Paused));

            Assert.That(manager.Resume(), Is.True);
            manager.Advance(0.1f);
            Assert.That(manager.Run.Tick, Is.EqualTo(2));
        }

        static SimulationManager CreateManager(float durationSeconds = 0.2f)
        {
            var data = new CellularSimData(
                width: 1,
                height: 1,
                startingProbabilities: new Dictionary<SpeciesId, float>(),
                speciesRules: new Dictionary<SpeciesId, SpeciesRules>(),
                runDurationSeconds: durationSeconds,
                stepInterval: 0.1f);
            var run = new SimulationRunState(
                new Grid<SpeciesCell>(1, 1, (_, __) => SpeciesCell.Empty),
                SpeciesIds.Herbivore,
                seed: 42,
                durationSeconds: durationSeconds);
            var manager = new SimulationManager();
            manager.SetRunner(new SpeciesSimulationRunner(run, data));
            return manager;
        }
    }
}
