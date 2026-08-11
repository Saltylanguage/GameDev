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
            Assert.That(cell.TerrainId, Is.EqualTo(TerrainIds.Bare));
            Assert.That(cell.IsPassable, Is.True);
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
        public void TerrainDefinitionsSupportAFutureSlowerPassableTerrain()
        {
            var sand = new TerrainDefinition(
                new TerrainId("sand"),
                isPassable: true,
                movementCost: 1.5f,
                providesResource: false,
                presentationColor: Color.yellow);
            var cell = SpeciesCell.FromTerrain(sand);

            Assert.That(cell.TerrainId, Is.EqualTo(sand.Id));
            Assert.That(cell.IsPassable, Is.True);
            Assert.That(cell.MovementCost, Is.EqualTo(1.5f));
            Assert.That(cell.IsPlantResource, Is.False);
        }

        [Test]
        public void CellularSimDataRetainsCustomTerrainDefinitions()
        {
            var definitions = new Dictionary<TerrainId, TerrainDefinition>();
            foreach (var definition in TerrainDefaults.Create())
            {
                definitions.Add(definition.Key, definition.Value);
            }

            var sand = new TerrainDefinition(
                new TerrainId("sand"),
                isPassable: true,
                movementCost: 1.5f,
                providesResource: false,
                presentationColor: Color.yellow);
            definitions.Add(sand.Id, sand);
            var data = new CellularSimData(
                2,
                2,
                new Dictionary<SpeciesId, float>(),
                new Dictionary<SpeciesId, SpeciesRules>(),
                runDurationSeconds: 1f,
                stepInterval: 0.1f,
                terrainDefinitions: definitions);

            Assert.That(data.TerrainDefinitions.ContainsKey(new TerrainId("sand")), Is.True);
            Assert.That(data.TerrainDefinitions[TerrainIds.Grass].ProvidesResource, Is.True);
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
        public void PopulationSnapshotCountsArbitrarySpeciesAndEmptyCells()
        {
            var customSpecies = new SpeciesId("scavenger");
            var cells = new Grid<SpeciesCell>(6, 1);
            cells.SetCell(0, 0, new SpeciesCell(customSpecies));
            cells.SetCell(1, 0, SpeciesCell.Grass(customSpecies, 2f));
            cells.SetCell(2, 0, SpeciesCell.Grass(2f));
            cells.SetCell(3, 0, new SpeciesCell(SpeciesIds.Plant));
            cells.SetCell(4, 0, SpeciesCell.Empty);
            cells.SetCell(5, 0, new SpeciesCell(SpeciesIds.Carnivore));

            var snapshot = SpeciesPopulationSnapshot.Create(cells, tick: 7);

            Assert.That(snapshot.GetCount(customSpecies), Is.EqualTo(2));
            Assert.That(snapshot.GetCount(SpeciesIds.Plant), Is.EqualTo(2));
            Assert.That(snapshot.GetCount(SpeciesIds.Carnivore), Is.EqualTo(1));
            Assert.That(snapshot.GetCount(SpeciesIds.Herbivore), Is.EqualTo(0));
            Assert.That(snapshot.Empty, Is.EqualTo(1));
            Assert.That(snapshot.Counts.ContainsKey(customSpecies), Is.True);
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
        public void MooreMovementTieBreakIsSeededAndNotPatternOrdered()
        {
            var source = new Grid<SpeciesCell>(3, 3);
            source.SetCell(1, 1, new SpeciesCell(SpeciesArchetype.Carnivore, energy: 10));
            var rules = new Dictionary<SpeciesArchetype, SpeciesRules>
            {
                [SpeciesArchetype.Carnivore] = new SpeciesRules(
                    movementSpeed: 1f,
                    movementPattern: SpeciesRuleDefaults.CreateMoorePattern(),
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

            var first = SpeciesSimulation.Step(source, rules, seed: 17);
            var second = SpeciesSimulation.Step(source, rules, seed: 17);
            var destinations = new HashSet<Vector2Int>();
            for (var seed = 0; seed < 32; seed++)
            {
                var next = SpeciesSimulation.Step(source, rules, seed);
                for (var y = 0; y < next.Height; y++)
                {
                    for (var x = 0; x < next.Width; x++)
                    {
                        if (next.GetCell(x, y).IsCreature)
                        {
                            destinations.Add(new Vector2Int(x, y));
                        }
                    }
                }
            }

            for (var y = 0; y < first.Height; y++)
            {
                for (var x = 0; x < first.Width; x++)
                {
                    Assert.That(
                        first.GetCell(x, y).IsCreature,
                        Is.EqualTo(second.GetCell(x, y).IsCreature));
                }
            }

            Assert.That(destinations.Count, Is.GreaterThan(1));
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
        public void CellularSimDataSupportsCustomSpeciesIds()
        {
            var scavenger = new SpeciesId("scavenger");
            var data = new CellularSimData(
                3,
                2,
                new Dictionary<SpeciesId, float>
                {
                    [scavenger] = 1f,
                },
                new Dictionary<SpeciesId, SpeciesRules>
                {
                    [scavenger] = CreateRules(),
                },
                runDurationSeconds: 10f,
                stepInterval: 0.1f);

            var updated = data.WithSpeciesRules(scavenger, CreateRules());
            var grid = SpeciesInitialGridFactory.Create(updated, runSeed: 123);
            var occupied = 0;
            for (var y = 0; y < grid.Height; y++)
            {
                for (var x = 0; x < grid.Width; x++)
                {
                    var cell = grid.GetCell(x, y);
                    if (!cell.IsOccupied)
                    {
                        continue;
                    }

                    occupied++;
                    Assert.That(cell.SpeciesId, Is.EqualTo(scavenger));
                }
            }

            Assert.That(occupied, Is.GreaterThan(0));
            Assert.That(updated.SpeciesRules.ContainsKey(scavenger), Is.True);
            Assert.That(updated.WithoutSpecies(scavenger).SpeciesRules.ContainsKey(scavenger), Is.False);
        }

        [Test]
        public void CellularSimDataFingerprintIsStableAcrossDictionaryOrderAndChangesWithRules()
        {
            var first = new CellularSimData(
                4,
                3,
                new Dictionary<SpeciesId, float>
                {
                    [SpeciesIds.Plant] = 0.4f,
                    [SpeciesIds.Herbivore] = 0.2f,
                },
                new Dictionary<SpeciesId, SpeciesRules>
                {
                    [SpeciesIds.Plant] = CreateRules(),
                    [SpeciesIds.Herbivore] = CreateRules(),
                },
                runDurationSeconds: 10f,
                stepInterval: 0.1f,
                maxPopulation: 20,
                minPopulation: 2);
            var reordered = new CellularSimData(
                4,
                3,
                new Dictionary<SpeciesId, float>
                {
                    [SpeciesIds.Herbivore] = 0.2f,
                    [SpeciesIds.Plant] = 0.4f,
                },
                new Dictionary<SpeciesId, SpeciesRules>
                {
                    [SpeciesIds.Herbivore] = CreateRules(),
                    [SpeciesIds.Plant] = CreateRules(),
                },
                runDurationSeconds: 10f,
                stepInterval: 0.1f,
                maxPopulation: 20,
                minPopulation: 2);

            Assert.That(first.Fingerprint, Is.EqualTo(reordered.Fingerprint));
            Assert.That(first.Fingerprint, Has.Length.EqualTo(64));
            Assert.That(first.WithStartingProbability(SpeciesIds.Plant, 0.5f).Fingerprint,
                Is.Not.EqualTo(first.Fingerprint));
        }

        [Test]
        public void DataBackedRunnerAndResultsCarryRulesetFingerprint()
        {
            var data = new CellularSimData(
                2,
                1,
                new Dictionary<SpeciesId, float>(),
                new Dictionary<SpeciesId, SpeciesRules>(),
                runDurationSeconds: 1f,
                stepInterval: 0.5f);
            var run = new SimulationRunState(
                new Grid<SpeciesCell>(2, 1),
                SpeciesIds.Herbivore,
                seed: 10,
                durationSeconds: data.RunDurationSeconds);
            var runner = new SpeciesSimulationRunner(run, data);

            Assert.That(run.RulesetFingerprint, Is.EqualTo(data.Fingerprint));
            Assert.That(SimulationRunResults.Create(run).RulesetFingerprint,
                Is.EqualTo(data.Fingerprint));
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
