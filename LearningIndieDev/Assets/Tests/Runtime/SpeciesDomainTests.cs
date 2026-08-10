using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace SaltyGame.Tests
{
    [TestFixture]
    public sealed class SpeciesDomainTests
    {
        static readonly GridPattern EmptyPattern = new GridPattern(new Vector2Int[0]);

        [Test]
        public void EmptySpeciesCellHasNoOccupant()
        {
            var cell = SpeciesCell.Empty;

            Assert.That(cell.IsOccupied, Is.False);
        }

        [Test]
        public void SpeciesRulesPreserveBehaviorValuesAndPatterns()
        {
            var attackPattern = new GridPattern(new[] { Vector2Int.right });
            var rules = new SpeciesRules(
                movementSpeed: 2f,
                movementPattern: attackPattern,
                attackPattern: attackPattern,
                attackAmount: 3,
                blockPattern: EmptyPattern,
                blockAmount: 1,
                dietPattern: attackPattern,
                dietTarget: SpeciesArchetype.Plant,
                reproductionPattern: attackPattern,
                reproductionNeighborCount: 1);

            Assert.That(rules.MovementSpeed, Is.EqualTo(2f));
            Assert.That(rules.AttackAmount, Is.EqualTo(3));
            Assert.That(rules.DietTarget, Is.EqualTo(SpeciesArchetype.Plant));
            Assert.That(rules.AttackPattern.Offsets[0], Is.EqualTo(Vector2Int.right));
        }

        [Test]
        public void ProgressionTracksCurrencyAndCanReplaceRules()
        {
            var rules = CreateRules();
            var progression = new SpeciesProgression(new SpeciesDefinition(SpeciesArchetype.Herbivore, rules));

            progression.AddCurrency(10);

            Assert.That(progression.TrySpend(7), Is.True);
            Assert.That(progression.Currency, Is.EqualTo(3));
            Assert.That(progression.TrySpend(4), Is.False);

            var upgradedRules = CreateRules();
            progression.SetRules(upgradedRules);
            Assert.That(progression.CurrentRules, Is.SameAs(upgradedRules));
        }

        [Test]
        public void UpgradeConsumesCurrencyAndChangesTheNextRulesSnapshot()
        {
            var rules = CreateRules();
            var progression = new SpeciesProgression(new SpeciesDefinition(SpeciesArchetype.Herbivore, rules));
            progression.AddCurrency(5);
            var upgrade = new SpeciesUpgrade("faster", 3, SpeciesUpgradeType.MovementSpeed, 0.5f);

            Assert.That(progression.TryPurchase(upgrade), Is.True);
            Assert.That(progression.Currency, Is.EqualTo(2));
            Assert.That(progression.CurrentRules.MovementSpeed, Is.EqualTo(1.5f));
        }

        [Test]
        public void RunStateAdvancesUntilItsDuration()
        {
            var initialGrid = new Grid<SpeciesCell>(2, 2);
            var run = new SimulationRunState(initialGrid, SpeciesArchetype.Herbivore, seed: 42, durationSeconds: 2f);

            run.Start();
            run.Advance(new Grid<SpeciesCell>(2, 2), 0.5f);
            run.Advance(new Grid<SpeciesCell>(2, 2), 2f);

            Assert.That(run.Status, Is.EqualTo(SimulationRunStatus.Complete));
            Assert.That(run.ElapsedSeconds, Is.EqualTo(2f));
            Assert.That(run.Tick, Is.EqualTo(2));
        }

        [Test]
        public void SpeciesSimulationIsDeterministicAndDoesNotMutateTheSource()
        {
            var source = new Grid<SpeciesCell>(3, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesArchetype.Herbivore));
            source.SetCell(1, 0, new SpeciesCell(SpeciesArchetype.Plant));
            var rules = SpeciesRuleDefaults.Create();

            var first = SpeciesSimulation.Step(source, rules, seed: 123);
            var second = SpeciesSimulation.Step(source, rules, seed: 123);

            Assert.That(source.GetCell(0, 0).Species, Is.EqualTo(SpeciesArchetype.Herbivore));
            Assert.That(first.GetCell(0, 0).Species, Is.EqualTo(second.GetCell(0, 0).Species));
            Assert.That(first.GetCell(1, 0).Species, Is.EqualTo(second.GetCell(1, 0).Species));
        }

        [Test]
        public void CarnivoresAttackHerbivoresButHerbivoresDoNotAttackCarnivores()
        {
            var source = new Grid<SpeciesCell>(2, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesArchetype.Herbivore, health: 3));
            source.SetCell(1, 0, new SpeciesCell(SpeciesArchetype.Carnivore, health: 3));
            var rightPattern = new GridPattern(new[] { Vector2Int.right });
            var leftPattern = new GridPattern(new[] { Vector2Int.left });
            var rules = new Dictionary<SpeciesArchetype, SpeciesRules>
            {
                [SpeciesArchetype.Herbivore] = new SpeciesRules(
                    movementSpeed: 0f,
                    movementPattern: EmptyPattern,
                    attackPattern: rightPattern,
                    attackAmount: 1,
                    blockPattern: EmptyPattern,
                    blockAmount: 0,
                    dietPattern: rightPattern,
                    dietTarget: SpeciesArchetype.Plant,
                    reproductionPattern: EmptyPattern,
                    reproductionNeighborCount: 0,
                    reproductionChance: 0f),
                [SpeciesArchetype.Carnivore] = new SpeciesRules(
                    movementSpeed: 0f,
                    movementPattern: EmptyPattern,
                    attackPattern: leftPattern,
                    attackAmount: 2,
                    blockPattern: EmptyPattern,
                    blockAmount: 0,
                    dietPattern: leftPattern,
                    dietTarget: SpeciesArchetype.Herbivore,
                    reproductionPattern: EmptyPattern,
                    reproductionNeighborCount: 0,
                    reproductionChance: 0f),
            };

            var next = SpeciesSimulation.Step(source, rules, seed: 42);

            Assert.That(next.GetCell(0, 0).Health, Is.EqualTo(1));
            Assert.That(next.GetCell(1, 0).Health, Is.EqualTo(3));
        }

        [Test]
        public void PlantsCanGrowWithoutNeighborsWhenTheirGrowthChanceSucceeds()
        {
            var source = new Grid<SpeciesCell>(3, 1);
            source.SetCell(1, 0, new SpeciesCell(SpeciesArchetype.Plant));
            var plantRules = new SpeciesRules(
                movementSpeed: 0f,
                movementPattern: EmptyPattern,
                attackPattern: EmptyPattern,
                attackAmount: 0,
                blockPattern: EmptyPattern,
                blockAmount: 0,
                dietPattern: EmptyPattern,
                dietTarget: null,
                reproductionPattern: new GridPattern(new[] { Vector2Int.right }),
                reproductionNeighborCount: 0,
                reproductionChance: 1f);

            var rules = new Dictionary<SpeciesArchetype, SpeciesRules>
            {
                [SpeciesArchetype.Plant] = plantRules,
            };
            var next = SpeciesSimulation.Step(source, rules, seed: 42);

            Assert.That(next.GetCell(2, 0).Species, Is.EqualTo(SpeciesArchetype.Plant));
        }

        [Test]
        public void CarnivoreNeedsFoodAndANeighborToReproduce()
        {
            var source = new Grid<SpeciesCell>(3, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesArchetype.Carnivore, energy: 2));
            source.SetCell(1, 0, new SpeciesCell(
                SpeciesArchetype.Carnivore,
                energy: 2,
                foodEaten: 1));
            var reproductionPattern = new GridPattern(new[] { Vector2Int.right, Vector2Int.left });
            var carnivoreRules = new SpeciesRules(
                movementSpeed: 0f,
                movementPattern: EmptyPattern,
                attackPattern: EmptyPattern,
                attackAmount: 0,
                blockPattern: EmptyPattern,
                blockAmount: 0,
                dietPattern: EmptyPattern,
                dietTarget: SpeciesArchetype.Herbivore,
                reproductionPattern: reproductionPattern,
                reproductionNeighborCount: 1,
                reproductionChance: 1f,
                reproductionFoodRequired: 1,
                maxReproductionGroupSize: 3,
                startingEnergy: 2);
            var rules = new Dictionary<SpeciesArchetype, SpeciesRules>
            {
                [SpeciesArchetype.Carnivore] = carnivoreRules,
            };

            var next = SpeciesSimulation.Step(source, rules, seed: 42);

            Assert.That(next.GetCell(2, 0).Species, Is.EqualTo(SpeciesArchetype.Carnivore));

            source.SetCell(1, 0, new SpeciesCell(SpeciesArchetype.Carnivore, energy: 2));
            var withoutFood = SpeciesSimulation.Step(source, rules, seed: 42);
            Assert.That(withoutFood.GetCell(2, 0).IsOccupied, Is.False);
        }

        [Test]
        public void SpeciesWithDietTargetStarvesWhenEnergyRunsOut()
        {
            var source = new Grid<SpeciesCell>(1, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesArchetype.Carnivore, energy: 1));
            var rules = new Dictionary<SpeciesArchetype, SpeciesRules>
            {
                [SpeciesArchetype.Carnivore] = new SpeciesRules(
                    movementSpeed: 0f,
                    movementPattern: EmptyPattern,
                    attackPattern: EmptyPattern,
                    attackAmount: 0,
                    blockPattern: EmptyPattern,
                    blockAmount: 0,
                    dietPattern: EmptyPattern,
                    dietTarget: SpeciesArchetype.Herbivore,
                    reproductionPattern: EmptyPattern,
                    reproductionNeighborCount: 0,
                    startingEnergy: 1),
            };

            var next = SpeciesSimulation.Step(source, rules, seed: 42);

            Assert.That(next.GetCell(0, 0).IsOccupied, Is.False);
        }

        [Test]
        public void HerbivoresStarveWhenPlantFoodIsUnavailable()
        {
            var source = new Grid<SpeciesCell>(1, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesArchetype.Herbivore, energy: 1));

            var next = SpeciesSimulation.Step(source, SpeciesRuleDefaults.Create(), seed: 42);

            Assert.That(next.GetCell(0, 0).IsOccupied, Is.False);
        }

        [Test]
        public void HerbivoreMovesTowardAnAvailableMateInsteadOfWanderingPastIt()
        {
            var source = new Grid<SpeciesCell>(3, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesArchetype.Herbivore));
            source.SetCell(2, 0, new SpeciesCell(SpeciesArchetype.Herbivore));
            var cardinal = new GridPattern(new[] { Vector2Int.right, Vector2Int.left });
            var rules = new Dictionary<SpeciesArchetype, SpeciesRules>
            {
                [SpeciesArchetype.Herbivore] = new SpeciesRules(
                    movementSpeed: 1f,
                    movementPattern: cardinal,
                    attackPattern: EmptyPattern,
                    attackAmount: 0,
                    blockPattern: EmptyPattern,
                    blockAmount: 0,
                    dietPattern: cardinal,
                    dietTarget: SpeciesArchetype.Plant,
                    reproductionPattern: cardinal,
                    reproductionNeighborCount: 1,
                    reproductionChance: 0f,
                    maxReproductionGroupSize: 3),
            };

            var next = SpeciesSimulation.Step(source, rules, seed: 42);

            Assert.That(next.GetCell(0, 0).IsOccupied, Is.False);
            Assert.That(next.GetCell(1, 0).Species, Is.EqualTo(SpeciesArchetype.Herbivore));
            Assert.That(next.GetCell(2, 0).Species, Is.EqualTo(SpeciesArchetype.Herbivore));
        }

        [Test]
        public void PlantsCanWiltAndCreateOpenTiles()
        {
            var source = new Grid<SpeciesCell>(1, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesArchetype.Plant));
            var rules = new Dictionary<SpeciesArchetype, SpeciesRules>
            {
                [SpeciesArchetype.Plant] = new SpeciesRules(
                    movementSpeed: 0f,
                    movementPattern: EmptyPattern,
                    attackPattern: EmptyPattern,
                    attackAmount: 0,
                    blockPattern: EmptyPattern,
                    blockAmount: 0,
                    dietPattern: EmptyPattern,
                    dietTarget: null,
                    reproductionPattern: EmptyPattern,
                    reproductionNeighborCount: 0,
                    reproductionChance: 0f,
                    wiltChance: 1f),
            };

            var next = SpeciesSimulation.Step(source, rules, seed: 42);

            Assert.That(next.GetCell(0, 0).IsOccupied, Is.False);
        }

        [Test]
        public void RunnerAdvancesTheRunAndProducesAResult()
        {
            var grid = new Grid<SpeciesCell>(2, 1);
            grid.SetCell(0, 0, new SpeciesCell(SpeciesArchetype.Herbivore));
            var run = new SimulationRunState(grid, SpeciesArchetype.Herbivore, seed: 10, durationSeconds: 1f);
            var runner = new SpeciesSimulationRunner(run, SpeciesRuleDefaults.Create(), stepSeconds: 0.5f);

            Assert.That(runner.AdvanceOneTick(), Is.True);
            Assert.That(runner.AdvanceOneTick(), Is.True);
            Assert.That(runner.AdvanceOneTick(), Is.False);

            var result = SimulationRunResults.Create(run);
            Assert.That(run.Status, Is.EqualTo(SimulationRunStatus.Complete));
            Assert.That(result.Ticks, Is.EqualTo(2));
            Assert.That(result.CurrencyEarned, Is.EqualTo(result.PlayerPopulation));
        }

        static SpeciesRules CreateRules()
        {
            return new SpeciesRules(
                movementSpeed: 1f,
                movementPattern: EmptyPattern,
                attackPattern: EmptyPattern,
                attackAmount: 0,
                blockPattern: EmptyPattern,
                blockAmount: 0,
                dietPattern: EmptyPattern,
                dietTarget: null,
                reproductionPattern: EmptyPattern,
                reproductionNeighborCount: 0);
        }
    }
}
