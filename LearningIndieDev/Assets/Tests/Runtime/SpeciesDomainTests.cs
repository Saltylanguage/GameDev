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
        public void GrassIsAResourceButNotAnOccupiedCreatureCell()
        {
            var grass = SpeciesCell.Grass(2f);

            Assert.That(grass.IsOccupied, Is.False);
            Assert.That(grass.IsPlantResource, Is.True);
            Assert.That(grass.TerrainEnergy, Is.EqualTo(2f));
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
            Assert.That(run.PopulationHistory.Count, Is.EqualTo(3));
        }

        [Test]
        public void RunStateCanPauseResumeAndRestart()
        {
            var initialGrid = new Grid<SpeciesCell>(1, 1);
            initialGrid.SetCell(0, 0, new SpeciesCell(SpeciesArchetype.Plant));
            var run = new SimulationRunState(initialGrid, SpeciesArchetype.Plant, seed: 42, durationSeconds: 2f);

            run.Start();
            run.Advance(new Grid<SpeciesCell>(1, 1), 0.5f);
            run.Pause();
            Assert.That(run.Status, Is.EqualTo(SimulationRunStatus.Paused));

            run.Resume();
            run.Advance(new Grid<SpeciesCell>(1, 1), 0.5f);
            run.Restart();

            Assert.That(run.Status, Is.EqualTo(SimulationRunStatus.Ready));
            Assert.That(run.ElapsedSeconds, Is.EqualTo(0f));
            Assert.That(run.Tick, Is.EqualTo(0));
            Assert.That(run.Cells.GetCell(0, 0).Species, Is.EqualTo(SpeciesArchetype.Plant));
            Assert.That(run.PopulationHistory.Count, Is.EqualTo(1));
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
        public void SpeciesSimulationRespectsMaximumPopulation()
        {
            var source = new Grid<SpeciesCell>(2, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesArchetype.Carnivore, energy: 2));
            source.SetCell(1, 0, new SpeciesCell(SpeciesArchetype.Carnivore, energy: 2));

            var next = SpeciesSimulation.Step(source, SpeciesRuleDefaults.Create(), seed: 42, maxPopulation: 1);

            var occupied = next.GetCell(0, 0).IsCreature ? 1 : 0;
            occupied += next.GetCell(1, 0).IsCreature ? 1 : 0;
            Assert.That(occupied, Is.EqualTo(1));
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
                    reproductionChance: 0f,
                    metabolism: 0),
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
                    reproductionChance: 0f,
                    metabolism: 0),
            };

            var next = SpeciesSimulation.Step(source, rules, seed: 42);

            Assert.That(next.GetCell(0, 0).Health, Is.EqualTo(1));
            Assert.That(next.GetCell(1, 0).Health, Is.EqualTo(3));
        }

        [Test]
        public void PlantsCanGrowWithoutNeighborsWhenTheirGrowthChanceSucceeds()
        {
            var source = new Grid<SpeciesCell>(3, 1);
            source.SetCell(1, 0, SpeciesCell.Grass(1f));
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
            source.SetCell(0, 0, new SpeciesCell(SpeciesArchetype.Carnivore, energy: 3));
            source.SetCell(1, 0, new SpeciesCell(
                SpeciesArchetype.Carnivore,
                energy: 3,
                foodReserve: 1));
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
            Assert.That(next.GetCell(1, 0).Energy, Is.EqualTo(1));

            source.SetCell(1, 0, new SpeciesCell(SpeciesArchetype.Carnivore, energy: 0));
            var withoutFood = SpeciesSimulation.Step(source, rules, seed: 42);
            Assert.That(withoutFood.GetCell(2, 0).IsOccupied, Is.False);
        }

        [Test]
        public void PlantMetabolismAddsEnergyWithoutOccupyingTheTile()
        {
            var source = new Grid<SpeciesCell>(1, 1);
            source.SetCell(0, 0, SpeciesCell.Grass(1f));
            var plantRules = new SpeciesRules(
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
                metabolism: -1);
            var rules = new Dictionary<SpeciesArchetype, SpeciesRules>
            {
                [SpeciesArchetype.Plant] = plantRules,
            };

            var next = SpeciesSimulation.Step(source, rules, seed: 42);

            Assert.That(next.GetCell(0, 0).IsOccupied, Is.False);
            Assert.That(next.GetCell(0, 0).TerrainEnergy, Is.EqualTo(2f));
        }

        [Test]
        public void CreaturesCanMoveThroughGrassTiles()
        {
            var source = new Grid<SpeciesCell>(2, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesArchetype.Carnivore, energy: 2));
            source.SetCell(1, 0, SpeciesCell.Grass(2f));
            var right = new GridPattern(new[] { Vector2Int.right });
            var rules = new Dictionary<SpeciesArchetype, SpeciesRules>
            {
                [SpeciesArchetype.Carnivore] = new SpeciesRules(
                    movementSpeed: 1f,
                    movementPattern: right,
                    attackPattern: EmptyPattern,
                    attackAmount: 0,
                    blockPattern: EmptyPattern,
                    blockAmount: 0,
                    dietPattern: EmptyPattern,
                    dietTarget: null,
                    reproductionPattern: EmptyPattern,
                    reproductionNeighborCount: 0,
                    reproductionChance: 0f,
                    metabolism: 0),
            };

            var next = SpeciesSimulation.Step(source, rules, seed: 42);

            Assert.That(next.GetCell(0, 0).IsCreature, Is.False);
            Assert.That(next.GetCell(1, 0).IsCreature, Is.True);
            Assert.That(next.GetCell(1, 0).IsPlantResource, Is.True);
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
                    maxReproductionGroupSize: 3,
                    metabolism: 0),
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
        public void PlantFoodReserveFeedsMultipleHerbivoresBeforeBeingConsumed()
        {
            var source = new Grid<SpeciesCell>(5, 1);
            source.SetCell(1, 0, new SpeciesCell(SpeciesArchetype.Herbivore, energy: 1));
            source.SetCell(2, 0, new SpeciesCell(
                SpeciesArchetype.Plant,
                foodReserve: 3.25f));
            source.SetCell(3, 0, new SpeciesCell(SpeciesArchetype.Herbivore, energy: 1));
            var cardinal = new GridPattern(new[] { Vector2Int.right, Vector2Int.left });
            var plantRules = new SpeciesRules(
                movementSpeed: 0f,
                movementPattern: cardinal,
                attackPattern: EmptyPattern,
                attackAmount: 0,
                blockPattern: EmptyPattern,
                blockAmount: 0,
                dietPattern: cardinal,
                dietTarget: null,
                reproductionPattern: EmptyPattern,
                reproductionNeighborCount: 0);
            var herbivoreRules = new SpeciesRules(
                movementSpeed: 0f,
                movementPattern: cardinal,
                attackPattern: cardinal,
                attackAmount: 1,
                blockPattern: EmptyPattern,
                blockAmount: 0,
                dietPattern: cardinal,
                dietTarget: SpeciesArchetype.Plant,
                reproductionPattern: EmptyPattern,
                reproductionNeighborCount: 0,
                startingEnergy: 2,
                metabolism: 0);
            var rules = new Dictionary<SpeciesArchetype, SpeciesRules>
            {
                [SpeciesArchetype.Plant] = plantRules,
                [SpeciesArchetype.Herbivore] = herbivoreRules,
            };

            var first = SpeciesSimulation.Step(source, rules, seed: 42);
            Assert.That(first.GetCell(2, 0).FoodReserve, Is.EqualTo(1.25f).Within(0.001f));
            Assert.That(first.GetCell(1, 0).FoodReserve, Is.EqualTo(1f));
            Assert.That(first.GetCell(3, 0).FoodReserve, Is.EqualTo(1f));

            var second = SpeciesSimulation.Step(first, rules, seed: 43);
            Assert.That(second.GetCell(2, 0).IsOccupied, Is.False);
            Assert.That(second.GetCell(1, 0).FoodReserve, Is.EqualTo(2f));
            Assert.That(second.GetCell(3, 0).FoodReserve, Is.EqualTo(1.25f).Within(0.001f));
        }

        [Test]
        public void FedHerbivoresCanDropSeedsIntoEmptyTiles()
        {
            var source = new Grid<SpeciesCell>(2, 1);
            source.SetCell(0, 0, new SpeciesCell(
                SpeciesArchetype.Herbivore,
                foodReserve: 1f));
            var right = new GridPattern(new[] { Vector2Int.right });
            var plantRules = new SpeciesRules(
                movementSpeed: 0f,
                movementPattern: right,
                attackPattern: EmptyPattern,
                attackAmount: 0,
                blockPattern: EmptyPattern,
                blockAmount: 0,
                dietPattern: right,
                dietTarget: null,
                reproductionPattern: EmptyPattern,
                reproductionNeighborCount: 0,
                startingFoodReserve: 3.25f);
            var herbivoreRules = new SpeciesRules(
                movementSpeed: 0f,
                movementPattern: right,
                attackPattern: EmptyPattern,
                attackAmount: 0,
                blockPattern: EmptyPattern,
                blockAmount: 0,
                dietPattern: right,
                    dietTarget: SpeciesArchetype.Plant,
                    reproductionPattern: EmptyPattern,
                    reproductionNeighborCount: 0,
                    seedDropChance: 1f,
                    metabolism: 0);
            var rules = new Dictionary<SpeciesArchetype, SpeciesRules>
            {
                [SpeciesArchetype.Plant] = plantRules,
                [SpeciesArchetype.Herbivore] = herbivoreRules,
            };

            var next = SpeciesSimulation.Step(source, rules, seed: 42);

            Assert.That(next.GetCell(1, 0).IsGrass, Is.True);
            Assert.That(next.GetCell(1, 0).TerrainEnergy, Is.EqualTo(3.25f).Within(0.001f));
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

        [Test]
        public void CellularSimDataEditsReturnNewDataWithoutMutatingOriginal()
        {
            var data = new CellularSimData(
                4,
                3,
                new Dictionary<SpeciesArchetype, float>
                {
                    [SpeciesArchetype.Plant] = 0.4f,
                    [SpeciesArchetype.Herbivore] = 0.2f,
                    [SpeciesArchetype.Carnivore] = 0.1f,
                },
                SpeciesRuleDefaults.Create(),
                runDurationSeconds: 10f,
                stepInterval: 0.1f);

            var updated = data
                .WithStartingProbability(SpeciesArchetype.Herbivore, 0.35f)
                .WithoutSpecies(SpeciesArchetype.Carnivore);

            Assert.That(data.StartingProbabilities[SpeciesArchetype.Herbivore], Is.EqualTo(0.2f));
            Assert.That(data.SpeciesRules.ContainsKey(SpeciesArchetype.Carnivore), Is.True);
            Assert.That(updated.StartingProbabilities[SpeciesArchetype.Herbivore], Is.EqualTo(0.35f));
            Assert.That(updated.SpeciesRules.ContainsKey(SpeciesArchetype.Carnivore), Is.False);
        }

        [Test]
        public void RunnerAcceptsCellularSimDataSnapshot()
        {
            var data = new CellularSimData(
                2,
                1,
                new Dictionary<SpeciesArchetype, float>
                {
                    [SpeciesArchetype.Plant] = 0f,
                    [SpeciesArchetype.Herbivore] = 0f,
                    [SpeciesArchetype.Carnivore] = 0f,
                },
                SpeciesRuleDefaults.Create(),
                runDurationSeconds: 1f,
                stepInterval: 0.5f);
            var run = new SimulationRunState(
                new Grid<SpeciesCell>(data.Width, data.Height),
                SpeciesArchetype.Herbivore,
                seed: 10,
                durationSeconds: data.RunDurationSeconds);
            var runner = new SpeciesSimulationRunner(run, data);

            Assert.That(runner.StepSeconds, Is.EqualTo(0.5f));
            Assert.That(runner.AdvanceOneTick(), Is.True);
        }

        [Test]
        public void InitialGridFactoryIsDeterministicForTheSameSeedAndData()
        {
            var data = new CellularSimData(
                8,
                6,
                new Dictionary<SpeciesArchetype, float>
                {
                    [SpeciesArchetype.Plant] = 0.4f,
                    [SpeciesArchetype.Herbivore] = 0.2f,
                    [SpeciesArchetype.Carnivore] = 0.1f,
                },
                SpeciesRuleDefaults.Create(),
                runDurationSeconds: 10f,
                stepInterval: 0.1f);

            var first = SpeciesInitialGridFactory.Create(data, runSeed: 1234);
            var second = SpeciesInitialGridFactory.Create(data.Copy(), runSeed: 1234);

            for (var y = 0; y < data.Height; y++)
            {
                for (var x = 0; x < data.Width; x++)
                {
                    var firstCell = first.GetCell(x, y);
                    var secondCell = second.GetCell(x, y);
                    Assert.That(secondCell.IsOccupied, Is.EqualTo(firstCell.IsOccupied));
                    Assert.That(secondCell.Species, Is.EqualTo(firstCell.Species));
                    Assert.That(secondCell.Terrain, Is.EqualTo(firstCell.Terrain));
                    Assert.That(secondCell.TerrainEnergy, Is.EqualTo(firstCell.TerrainEnergy));
                    Assert.That(secondCell.Energy, Is.EqualTo(firstCell.Energy));
                }
            }
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
