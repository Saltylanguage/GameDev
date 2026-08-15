using System.Collections.Generic;
using System.Reflection;
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
        public void CreatureEntityIdsAreUniqueAndSurviveStateUpdates()
        {
            var first = new SpeciesCell(SpeciesIds.Herbivore);
            var second = new SpeciesCell(SpeciesIds.Herbivore);

            Assert.That(first.EntityId, Is.GreaterThan(0));
            Assert.That(second.EntityId, Is.Not.EqualTo(first.EntityId));
            Assert.That(first.WithBehaviorState(SpeciesBehaviorState.Hunting).EntityId, Is.EqualTo(first.EntityId));
            Assert.That(SpeciesCell.Empty.EntityId, Is.EqualTo(0));
        }

        [Test]
        public void TerrainDefinitionsRetainSlowerPassableTerrainData()
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
        public void TerrainMovementCostReducesTheChanceToEnterSlowTerrain()
        {
            var sand = new TerrainDefinition(
                new TerrainId("sand"),
                isPassable: true,
                movementCost: 2f,
                providesResource: false,
                presentationColor: Color.yellow);
            var source = new Grid<SpeciesCell>(2, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Herbivore, energy: 10));
            source.SetCell(1, 0, SpeciesCell.FromTerrain(sand));
            var rules = new Dictionary<SpeciesId, SpeciesRules>
            {
                [SpeciesIds.Herbivore] = new SpeciesRules(
                    movementSpeed: 1f,
                    movementPattern: new GridPattern(new[] { Vector2Int.right }),
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
            var moved = 0;

            for (var seed = 0; seed < 32; seed++)
            {
                moved += SpeciesSimulation.Step(source, rules, seed).GetCell(1, 0).IsCreature ? 1 : 0;
            }

            Assert.That(moved, Is.GreaterThan(0));
            Assert.That(moved, Is.LessThan(32));
        }

        [Test]
        public void EntitiesAgeAndDepletedResourceTerrainRegrowsEachTick()
        {
            var grass = new TerrainDefinition(
                TerrainIds.Grass,
                isPassable: true,
                movementCost: 1f,
                providesResource: true,
                presentationColor: Color.green,
                regrowthPerTick: 0.75f);
            var terrain = new Dictionary<TerrainId, TerrainDefinition>
            {
                [TerrainIds.Bare] = TerrainDefaults.Bare,
                [TerrainIds.Grass] = grass,
            };
            var source = new Grid<SpeciesCell>(1, 1);
            source.SetCell(0, 0, SpeciesCell.FromTerrain(grass, 0f, SpeciesIds.Plant).WithEntity(
                SpeciesIds.Herbivore,
                health: 1,
                energy: 5,
                age: 2,
                foodEaten: 0,
                foodReserve: 0f));
            var rules = new Dictionary<SpeciesId, SpeciesRules>
            {
                [SpeciesIds.Herbivore] = CreateRules(metabolism: 0),
            };

            var next = SpeciesSimulation.Step(source, rules, seed: 42, terrainDefinitions: terrain);

            Assert.That(next.GetCell(0, 0).Age, Is.EqualTo(3));
            Assert.That(next.GetCell(0, 0).TerrainEnergy, Is.EqualTo(0.75f).Within(0.001f));
            Assert.That(next.GetCell(0, 0).IsPlantResource, Is.True);
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
        public void CellularSimDataAssetCreatesAnIndependentRuntimeSnapshot()
        {
            var asset = ScriptableObject.CreateInstance<CellularSimDataAsset>();
            var first = asset.CreateRuntimeData();
            var second = asset.CreateRuntimeData();

            Assert.That(first.SpeciesRules.Count, Is.EqualTo(3));
            Assert.That(first.SpeciesRules.ContainsKey(SpeciesIds.Herbivore), Is.True);
            Assert.That(second, Is.Not.SameAs(first));
            Assert.That(second.Fingerprint, Is.EqualTo(first.Fingerprint));

            Object.DestroyImmediate(asset);
        }

        [Test]
        public void ScenariosOwnStartingProbabilityForReusableSpeciesAssets()
        {
            var species = ScriptableObject.CreateInstance<HerbivoreSpeciesDefinitionAsset>();
            var sparseScenario = ScriptableObject.CreateInstance<ScenarioDefinitionAsset>();
            var denseScenario = ScriptableObject.CreateInstance<ScenarioDefinitionAsset>();
            SetPrivateField(species, "id", "reusable-herbivore");
            SetPrivateField(sparseScenario, "species", new[]
            {
                new ScenarioDefinitionAsset.SpeciesEntry(species, 0.1f),
            });
            SetPrivateField(denseScenario, "species", new[]
            {
                new ScenarioDefinitionAsset.SpeciesEntry(species, 0.7f),
            });

            var sparse = sparseScenario.CreateRuntimeData();
            var dense = denseScenario.CreateRuntimeData();

            Assert.That(sparse.TryGetStartingProbability(species.Id, out var sparseProbability), Is.True);
            Assert.That(dense.TryGetStartingProbability(species.Id, out var denseProbability), Is.True);
            Assert.That(sparseProbability, Is.EqualTo(0.1f));
            Assert.That(denseProbability, Is.EqualTo(0.7f));

            Object.DestroyImmediate(sparseScenario);
            Object.DestroyImmediate(denseScenario);
            Object.DestroyImmediate(species);
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
            var rules = CreateRules(role: SpeciesRole.Carnivore, forageBelowEnergy: 3);
            var progression = new SpeciesProgression(new SpeciesDefinition(SpeciesArchetype.Herbivore, rules));
            progression.AddCurrency(5);
            var upgrade = new SpeciesUpgrade("faster", 3, SpeciesUpgradeType.MovementSpeed, 0.5f);

            Assert.That(progression.TryPurchase(upgrade), Is.True);
            Assert.That(progression.Currency, Is.EqualTo(2));
            Assert.That(progression.CurrentRules.MovementSpeed, Is.EqualTo(1.5f));
            Assert.That(progression.CurrentRules.Role, Is.EqualTo(SpeciesRole.Carnivore));
            Assert.That(progression.CurrentRules.ForageBelowEnergy, Is.EqualTo(3));
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

            Assert.That(source.GetCell(0, 0).SpeciesId, Is.EqualTo(SpeciesIds.Herbivore));
            Assert.That(first.GetCell(0, 0).SpeciesId, Is.EqualTo(second.GetCell(0, 0).SpeciesId));
            Assert.That(first.GetCell(1, 0).SpeciesId, Is.EqualTo(second.GetCell(1, 0).SpeciesId));
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

            var metrics = new SpeciesSimulationMetrics();
            var next = SpeciesSimulation.Step(source, rules, seed: 42, metrics: metrics);

            Assert.That(next.GetCell(0, 0).Health, Is.EqualTo(1));
            Assert.That(next.GetCell(1, 0).Health, Is.EqualTo(3));
            Assert.That(metrics.GetActivity(SpeciesIds.Carnivore).DamageDealt, Is.EqualTo(2));
        }

        [Test]
        public void PredatorForagesOnlyAtOrBelowItsEnergyThreshold()
        {
            var source = new Grid<SpeciesCell>(2, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Carnivore, energy: 6));
            source.SetCell(1, 0, new SpeciesCell(SpeciesIds.Herbivore));
            var right = new GridPattern(new[] { Vector2Int.right });
            var rules = new Dictionary<SpeciesId, SpeciesRules>
            {
                [SpeciesIds.Carnivore] = new SpeciesRules(
                    movementSpeed: 0f,
                    movementPattern: EmptyPattern,
                    attackPattern: right,
                    attackAmount: 1,
                    blockPattern: EmptyPattern,
                    blockAmount: 0,
                    dietPattern: right,
                    dietTarget: SpeciesIds.Herbivore,
                    reproductionPattern: EmptyPattern,
                    reproductionNeighborCount: 0,
                    reproductionChance: 0f,
                    metabolism: 0,
                    forageBelowEnergy: 5),
                [SpeciesIds.Herbivore] = CreateRules(),
            };

            var satiated = SpeciesSimulation.Step(source, rules, seed: 42);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Carnivore, energy: 5));
            var hungry = SpeciesSimulation.Step(source, rules, seed: 42);

            Assert.That(satiated.GetCell(1, 0).IsCreature, Is.True);
            Assert.That(hungry.GetCell(1, 0).IsCreature, Is.False);
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
            source.SetCell(2, 0, SpeciesCell.Grass(2f));
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
                startingEnergy: 99);
            var rules = new Dictionary<SpeciesArchetype, SpeciesRules>
            {
                [SpeciesArchetype.Carnivore] = carnivoreRules,
            };

            var metrics = new SpeciesSimulationMetrics();
            var next = SpeciesSimulation.Step(source, rules, seed: 42, metrics: metrics);

            Assert.That(next.GetCell(2, 0).Species, Is.EqualTo(SpeciesArchetype.Carnivore));
            Assert.That(next.GetCell(2, 0).Energy, Is.EqualTo(1));
            Assert.That(next.GetCell(2, 0).IsPlantResource, Is.True);
            Assert.That(next.GetCell(2, 0).TerrainEnergy, Is.EqualTo(2f));
            Assert.That(next.GetCell(1, 0).Energy, Is.EqualTo(1));
            Assert.That(metrics.GetActivity(SpeciesIds.Carnivore).Births, Is.EqualTo(1));

            source.SetCell(1, 0, new SpeciesCell(SpeciesArchetype.Carnivore, energy: 0));
            var withoutFood = SpeciesSimulation.Step(source, rules, seed: 42);
            Assert.That(withoutFood.GetCell(2, 0).IsOccupied, Is.False);
        }

        [Test]
        public void AlphaOffspringRulePromotesNewbornCreaturesAndChangesTheRulesetFingerprint()
        {
            var source = new Grid<SpeciesCell>(3, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Carnivore, energy: 3));
            source.SetCell(1, 0, new SpeciesCell(SpeciesIds.Carnivore, energy: 3));
            var parentId = source.GetCell(0, 0).EntityId;
            var reproductionPattern = new GridPattern(new[] { Vector2Int.right, Vector2Int.left });
            var carnivoreRules = new SpeciesRules(
                movementSpeed: 0f,
                movementPattern: EmptyPattern,
                attackPattern: EmptyPattern,
                attackAmount: 0,
                blockPattern: EmptyPattern,
                blockAmount: 0,
                dietPattern: EmptyPattern,
                dietTarget: null,
                reproductionPattern: reproductionPattern,
                reproductionNeighborCount: 1,
                reproductionChance: 1f,
                reproductionFoodRequired: 0,
                maxReproductionGroupSize: 3,
                startingEnergy: 2,
                metabolism: 0);
            var alphaRule = new AlphaOffspringRule(
                SpeciesIds.Carnivore,
                chance: 1f,
                healthBonus: 2,
                energyBonus: 3);
            var data = new CellularSimData(
                3,
                1,
                new Dictionary<SpeciesId, float>(),
                new Dictionary<SpeciesId, SpeciesRules>
                {
                    [SpeciesIds.Carnivore] = carnivoreRules,
                },
                runDurationSeconds: 1f,
                stepInterval: 0.1f,
                alphaOffspringRules: new Dictionary<SpeciesId, AlphaOffspringRule>
                {
                    [SpeciesIds.Carnivore] = alphaRule,
                });

            var next = SpeciesSimulation.Step(source, data, seed: 42);
            var offspring = next.GetCell(2, 0);

            Assert.That(offspring.IsAlpha, Is.True);
            Assert.That(offspring.Health, Is.EqualTo(3));
            Assert.That(offspring.Energy, Is.EqualTo(5));
            Assert.That(offspring.EntityId, Is.Not.EqualTo(parentId));
            Assert.That(data.WithoutAlphaOffspringRule(SpeciesIds.Carnivore).Fingerprint,
                Is.Not.EqualTo(data.Fingerprint));
        }

        [Test]
        public void VisionFindsFoodWithinRangeAndNavigationRoutesAroundBlockedTerrain()
        {
            var rock = new TerrainDefinition(
                new TerrainId("rock"),
                isPassable: false,
                movementCost: 1f,
                providesResource: false,
                presentationColor: Color.gray);
            var cells = new Grid<SpeciesCell>(3, 2);
            cells.SetCell(0, 0, new SpeciesCell(SpeciesIds.Herbivore));
            cells.SetCell(1, 0, SpeciesCell.FromTerrain(rock));
            cells.SetCell(2, 0, SpeciesCell.Grass(2f));
            var rules = new SpeciesRules(
                movementSpeed: 1f,
                movementPattern: SpeciesRuleDefaults.CreateCardinalPattern(),
                attackPattern: EmptyPattern,
                attackAmount: 0,
                blockPattern: EmptyPattern,
                blockAmount: 0,
                dietPattern: SpeciesRuleDefaults.CreateCardinalPattern(),
                dietTarget: SpeciesIds.Plant,
                reproductionPattern: EmptyPattern,
                reproductionNeighborCount: 0,
                awareness: new SpeciesAwarenessRules(visionRange: 2));

            Assert.That(SpeciesPerception.TryFindFoodTarget(
                cells,
                0,
                0,
                rules,
                new System.Random(5),
                out var target), Is.True);
            Assert.That(target.Location, Is.EqualTo(new Vector2Int(2, 0)));
            Assert.That(SpeciesNavigation.TryFindNextStep(
                cells,
                new Vector2Int(0, 0),
                target.Location,
                rules.MovementPattern,
                rules.DietPattern,
                new System.Random(5),
                out var nextStep), Is.True);
            Assert.That(nextStep, Is.EqualTo(new Vector2Int(0, 1)));
        }

        [Test]
        public void HareFleesVisibleFoxWhileFoxPursuesVisibleHare()
        {
            var hare = new SpeciesId("hare");
            var fox = new SpeciesId("fox");
            var leftRight = new GridPattern(new[] { Vector2Int.left, Vector2Int.right });
            var right = new GridPattern(new[] { Vector2Int.right });
            var rules = new Dictionary<SpeciesId, SpeciesRules>
            {
                [hare] = new SpeciesRules(
                    movementSpeed: 1f,
                    movementPattern: leftRight,
                    attackPattern: EmptyPattern,
                    attackAmount: 0,
                    blockPattern: EmptyPattern,
                    blockAmount: 0,
                    dietPattern: leftRight,
                    dietTarget: SpeciesIds.Plant,
                    reproductionPattern: EmptyPattern,
                    reproductionNeighborCount: 0,
                    reproductionChance: 0f,
                    startingEnergy: 8,
                    metabolism: 0,
                    awareness: new SpeciesAwarenessRules(visionRange: 2, intelligence: 1)),
                [fox] = new SpeciesRules(
                    movementSpeed: 1f,
                    movementPattern: right,
                    attackPattern: EmptyPattern,
                    attackAmount: 0,
                    blockPattern: EmptyPattern,
                    blockAmount: 0,
                    dietPattern: right,
                    dietTarget: hare,
                    reproductionPattern: EmptyPattern,
                    reproductionNeighborCount: 0,
                    reproductionChance: 0f,
                    startingEnergy: 8,
                    metabolism: 0,
                    awareness: new SpeciesAwarenessRules(visionRange: 3, intelligence: 1)),
                [SpeciesIds.Plant] = CreateRules(
                    movementSpeed: 0f,
                    role: SpeciesRole.Plant,
                    metabolism: -1),
            };

            var fleeing = new Grid<SpeciesCell>(3, 1);
            fleeing.SetCell(0, 0, SpeciesCell.Grass(2f));
            fleeing.SetCell(1, 0, new SpeciesCell(hare, energy: 8));
            fleeing.SetCell(2, 0, new SpeciesCell(fox, energy: 8));
            var escaped = SpeciesSimulation.Step(fleeing, rules, seed: 11);

            Assert.That(escaped.GetCell(0, 0).SpeciesId, Is.EqualTo(hare));
            Assert.That(escaped.GetCell(0, 0).IsTerrainResource, Is.True);
            Assert.That(escaped.GetCell(2, 0).SpeciesId, Is.EqualTo(fox));

            var pursuing = new Grid<SpeciesCell>(3, 1);
            pursuing.SetCell(0, 0, new SpeciesCell(fox, energy: 8));
            pursuing.SetCell(2, 0, new SpeciesCell(hare, energy: 8));
            var hunted = SpeciesSimulation.Step(pursuing, rules, seed: 11);

            Assert.That(hunted.GetCell(1, 0).SpeciesId, Is.EqualTo(fox));
            Assert.That(hunted.GetCell(2, 0).SpeciesId, Is.EqualTo(hare));
        }

        [Test]
        public void HareFoxFixtureStartsWithAmpleGrassAndReciprocalDietRules()
        {
            var hare = new SpeciesId("hare");
            var fox = new SpeciesId("fox");
            var movement = SpeciesRuleDefaults.CreateCardinalPattern();
            var grassRules = CreateRules(
                movementSpeed: 0f,
                role: SpeciesRole.Plant,
                metabolism: -1);
            var hareRules = new SpeciesRules(
                movementSpeed: 2.2f,
                movementPattern: movement,
                attackPattern: EmptyPattern,
                attackAmount: 0,
                blockPattern: movement,
                blockAmount: 0,
                dietPattern: SpeciesRuleDefaults.CreateMoorePattern(),
                dietTarget: SpeciesIds.Plant,
                reproductionPattern: SpeciesRuleDefaults.CreateMoorePattern(),
                reproductionNeighborCount: 1,
                reproductionChance: 0.25f,
                reproductionFoodRequired: 1,
                maxReproductionGroupSize: 4,
                startingEnergy: 16,
                metabolism: 1,
                awareness: new SpeciesAwarenessRules(visionRange: 5, intelligence: 1),
                forageBelowEnergy: 16);
            var foxRules = new SpeciesRules(
                movementSpeed: 0.8f,
                movementPattern: SpeciesRuleDefaults.CreateMoorePattern(),
                attackPattern: SpeciesRuleDefaults.CreateMoorePattern(),
                attackAmount: 2,
                blockPattern: movement,
                blockAmount: 0,
                dietPattern: SpeciesRuleDefaults.CreateMoorePattern(),
                dietTarget: hare,
                reproductionPattern: movement,
                reproductionNeighborCount: 1,
                reproductionChance: 0.02f,
                reproductionFoodRequired: 1,
                maxReproductionGroupSize: 3,
                startingEnergy: 32,
                energyValue: 8,
                metabolism: 1,
                awareness: new SpeciesAwarenessRules(visionRange: 6, intelligence: 1),
                forageBelowEnergy: 32);
            var data = new CellularSimData(
                32,
                20,
                new Dictionary<SpeciesId, float>
                {
                    [SpeciesIds.Plant] = 0.65f,
                    [hare] = 0.15f,
                    [fox] = 0.015f,
                },
                new Dictionary<SpeciesId, SpeciesRules>
                {
                    [SpeciesIds.Plant] = grassRules,
                    [hare] = hareRules,
                    [fox] = foxRules,
                },
                runDurationSeconds: 20f,
                stepInterval: 0.1f);

            var grid = SpeciesInitialGridFactory.Create(data, runSeed: 11);
            var grassCount = 0;
            var hareCount = 0;
            var foxCount = 0;
            for (var y = 0; y < grid.Height; y++)
            {
                for (var x = 0; x < grid.Width; x++)
                {
                    var cell = grid.GetCell(x, y);
                    if (cell.IsPlantResource && cell.ResourceSpeciesId == SpeciesIds.Plant)
                    {
                        grassCount++;
                    }
                    else if (cell.IsCreature && cell.SpeciesId == hare)
                    {
                        hareCount++;
                    }
                    else if (cell.IsCreature && cell.SpeciesId == fox)
                    {
                        foxCount++;
                    }
                }
            }

            Assert.That(grassCount, Is.GreaterThan(hareCount + foxCount));
            Assert.That(data.StartingProbabilities[SpeciesIds.Plant], Is.GreaterThan(data.StartingProbabilities[hare]));
            Assert.That(hareRules.DietTargetId, Is.EqualTo(SpeciesIds.Plant));
            Assert.That(foxRules.DietTargetId, Is.EqualTo(hare));
        }

        [Test]
        public void IntelligencePrioritizesAVisibleMateOverVisibleFoodWhenReadyToReproduce()
        {
            var source = new Grid<SpeciesCell>(5, 1);
            source.SetCell(1, 0, SpeciesCell.Grass(2f));
            source.SetCell(2, 0, new SpeciesCell(SpeciesIds.Herbivore, energy: 5));
            source.SetCell(4, 0, new SpeciesCell(SpeciesIds.Herbivore, energy: 5));
            var cardinal = SpeciesRuleDefaults.CreateCardinalPattern();
            var herbivoreRules = new SpeciesRules(
                movementSpeed: 1f,
                movementPattern: cardinal,
                attackPattern: EmptyPattern,
                attackAmount: 0,
                blockPattern: EmptyPattern,
                blockAmount: 0,
                dietPattern: cardinal,
                dietTarget: SpeciesIds.Plant,
                reproductionPattern: new GridPattern(new[] { Vector2Int.right }),
                reproductionNeighborCount: 1,
                reproductionChance: 1f,
                reproductionFoodRequired: 1,
                startingEnergy: 1,
                metabolism: 0,
                awareness: new SpeciesAwarenessRules(visionRange: 3, intelligence: 1));
            var rules = new Dictionary<SpeciesId, SpeciesRules>
            {
                [SpeciesIds.Herbivore] = herbivoreRules,
            };

            var next = SpeciesSimulation.Step(source, rules, seed: 42);

            Assert.That(next.GetCell(3, 0).SpeciesId, Is.EqualTo(SpeciesIds.Herbivore));
            Assert.That(next.GetCell(2, 0).IsCreature, Is.False);
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
            Assert.That(next.GetCell(1, 0).WithoutEntity().SpeciesId, Is.EqualTo(SpeciesIds.Plant));
            var snapshot = SpeciesPopulationSnapshot.Create(next, tick: 1);
            Assert.That(snapshot.GetCount(SpeciesIds.Carnivore), Is.EqualTo(1));
            Assert.That(snapshot.GetCount(SpeciesIds.Plant), Is.EqualTo(1));
        }

        [Test]
        public void CreatureMovementAndResourceDepletionPreserveBothCellLayers()
        {
            var source = new Grid<SpeciesCell>(2, 1);
            source.SetCell(0, 0, SpeciesCell.Grass(2f).WithEntity(
                SpeciesIds.Herbivore,
                health: 1,
                energy: 5,
                age: 0,
                foodEaten: 0,
                foodReserve: 0f));
            var right = new GridPattern(new[] { Vector2Int.right });
            var rules = new Dictionary<SpeciesId, SpeciesRules>
            {
                [SpeciesIds.Herbivore] = new SpeciesRules(
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
            var vacatedGrass = next.GetCell(0, 0);
            var depletedUnderCreature = SpeciesCell.Grass(2f).WithEntity(
                SpeciesIds.Herbivore,
                health: 1,
                energy: 5,
                age: 0,
                foodEaten: 0,
                foodReserve: 0f).WithoutPlantResource();

            Assert.That(vacatedGrass.IsCreature, Is.False);
            Assert.That(vacatedGrass.IsPlantResource, Is.True);
            Assert.That(vacatedGrass.ResourceSpeciesId, Is.EqualTo(SpeciesIds.Plant));
            Assert.That(depletedUnderCreature.IsCreature, Is.True);
            Assert.That(depletedUnderCreature.TerrainId, Is.EqualTo(TerrainIds.Grass));
            Assert.That(depletedUnderCreature.TerrainEnergy, Is.EqualTo(0f));
            Assert.That(depletedUnderCreature.WithTerrainEnergy(1f).IsPlantResource, Is.True);
        }

        [Test]
        public void PopulationLimitCountsCreatureAndResourceLayersSeparately()
        {
            var source = new Grid<SpeciesCell>(1, 1);
            source.SetCell(0, 0, SpeciesCell.Grass(2f).WithEntity(
                SpeciesIds.Herbivore,
                health: 1,
                energy: 5,
                age: 0,
                foodEaten: 0,
                foodReserve: 0f));
            var rules = new Dictionary<SpeciesId, SpeciesRules>
            {
                [SpeciesIds.Herbivore] = CreateRules(metabolism: 0),
            };

            var next = SpeciesSimulation.Step(source, rules, seed: 42, maxPopulation: 1);
            var snapshot = SpeciesPopulationSnapshot.Create(next, tick: 1);

            Assert.That(
                snapshot.GetCount(SpeciesIds.Herbivore) + snapshot.GetCount(SpeciesIds.Plant),
                Is.EqualTo(1));
            Assert.That(next.GetCell(0, 0).TerrainId, Is.EqualTo(TerrainIds.Grass));
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

            var metrics = new SpeciesSimulationMetrics();
            var next = SpeciesSimulation.Step(source, rules, seed: 42, metrics: metrics);

            Assert.That(next.GetCell(0, 0).IsOccupied, Is.False);
            Assert.That(metrics.GetActivity(SpeciesIds.Carnivore).StarvationDeaths, Is.EqualTo(1));
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
        public void FractionalMovementSpeedUsesASeededChanceForTheNextMove()
        {
            var source = new Grid<SpeciesCell>(3, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Herbivore, energy: 10));
            var rules = new Dictionary<SpeciesId, SpeciesRules>
            {
                [SpeciesIds.Herbivore] = new SpeciesRules(
                    movementSpeed: 1.5f,
                    movementPattern: new GridPattern(new[] { Vector2Int.right }),
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
            var oneMoveRuns = 0;
            var twoMoveRuns = 0;

            for (var seed = 0; seed < 32; seed++)
            {
                var metrics = new SpeciesSimulationMetrics();
                SpeciesSimulation.Step(source, rules, seed, metrics: metrics);
                var moves = metrics.GetActivity(SpeciesIds.Herbivore).MovementSteps;
                oneMoveRuns += moves == 1 ? 1 : 0;
                twoMoveRuns += moves == 2 ? 1 : 0;
            }

            Assert.That(oneMoveRuns, Is.GreaterThan(0));
            Assert.That(twoMoveRuns, Is.GreaterThan(0));
            Assert.That(oneMoveRuns + twoMoveRuns, Is.EqualTo(32));
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

            var metrics = new SpeciesSimulationMetrics();
            var next = SpeciesSimulation.Step(source, rules, seed: 42, metrics: metrics);

            Assert.That(next.GetCell(0, 0).IsOccupied, Is.False);
            Assert.That(metrics.GetActivity(SpeciesIds.Plant).WiltDeaths, Is.EqualTo(1));
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
                forageBelowEnergy: 2,
                metabolism: 0);
            var rules = new Dictionary<SpeciesArchetype, SpeciesRules>
            {
                [SpeciesArchetype.Plant] = plantRules,
                [SpeciesArchetype.Herbivore] = herbivoreRules,
            };

            var firstMetrics = new SpeciesSimulationMetrics();
            var first = SpeciesSimulation.Step(source, rules, seed: 42, metrics: firstMetrics);
            Assert.That(first.GetCell(2, 0).FoodReserve, Is.EqualTo(1.25f).Within(0.001f));
            Assert.That(first.GetCell(1, 0).FoodReserve, Is.EqualTo(1f));
            Assert.That(first.GetCell(3, 0).FoodReserve, Is.EqualTo(1f));
            Assert.That(firstMetrics.GetActivity(SpeciesIds.Herbivore).FoodConsumed, Is.EqualTo(2f));

            var secondMetrics = new SpeciesSimulationMetrics();
            var second = SpeciesSimulation.Step(first, rules, seed: 43, metrics: secondMetrics);
            Assert.That(second.GetCell(2, 0).IsOccupied, Is.False);
            Assert.That(second.GetCell(1, 0).FoodReserve, Is.EqualTo(2f));
            Assert.That(second.GetCell(3, 0).FoodReserve, Is.EqualTo(1.25f).Within(0.001f));
            Assert.That(secondMetrics.GetActivity(SpeciesIds.Herbivore).FoodConsumed, Is.EqualTo(1.25f));
            Assert.That(secondMetrics.GetActivity(SpeciesIds.Plant).Deaths, Is.EqualTo(1));
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
            Assert.That(next.GetCell(0, 0).FoodReserve, Is.EqualTo(0f));
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
            Assert.That(first.WithSpeciesRules(
                    SpeciesIds.Herbivore,
                    CreateRules(new SpeciesAwarenessRules(visionRange: 1, intelligence: 1))).Fingerprint,
                Is.Not.EqualTo(first.Fingerprint));
            Assert.That(first.WithSpeciesRules(
                    SpeciesIds.Herbivore,
                    CreateRules(movementSpeed: 1.5f)).Fingerprint,
                Is.Not.EqualTo(first.Fingerprint));
            Assert.That(first.WithSpeciesRules(
                    SpeciesIds.Herbivore,
                    CreateRules(forageBelowEnergy: 1)).Fingerprint,
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
            var rules = SpeciesRuleDefaults.Create();
            var data = new CellularSimData(
                8,
                6,
                new Dictionary<SpeciesArchetype, float>
                {
                    [SpeciesArchetype.Plant] = 0.4f,
                    [SpeciesArchetype.Herbivore] = 0.2f,
                    [SpeciesArchetype.Carnivore] = 0.1f,
                },
                rules,
                runDurationSeconds: 10f,
                stepInterval: 0.1f);
            var reordered = new CellularSimData(
                8,
                6,
                new Dictionary<SpeciesId, float>
                {
                    [SpeciesIds.Carnivore] = 0.1f,
                    [SpeciesIds.Herbivore] = 0.2f,
                    [SpeciesIds.Plant] = 0.4f,
                },
                new Dictionary<SpeciesId, SpeciesRules>
                {
                    [SpeciesIds.Carnivore] = rules[SpeciesIds.Carnivore],
                    [SpeciesIds.Herbivore] = rules[SpeciesIds.Herbivore],
                    [SpeciesIds.Plant] = rules[SpeciesIds.Plant],
                },
                runDurationSeconds: 10f,
                stepInterval: 0.1f);

            var first = SpeciesInitialGridFactory.Create(data, runSeed: 1234);
            var second = SpeciesInitialGridFactory.Create(reordered, runSeed: 1234);

            Assert.That(reordered.Fingerprint, Is.EqualTo(data.Fingerprint));

            for (var y = 0; y < data.Height; y++)
            {
                for (var x = 0; x < data.Width; x++)
                {
                    var firstCell = first.GetCell(x, y);
                    var secondCell = second.GetCell(x, y);
                    Assert.That(secondCell.IsCreature, Is.EqualTo(firstCell.IsCreature));
                    Assert.That(secondCell.SpeciesId, Is.EqualTo(firstCell.SpeciesId));
                    Assert.That(secondCell.TerrainId, Is.EqualTo(firstCell.TerrainId));
                    Assert.That(secondCell.TerrainEnergy, Is.EqualTo(firstCell.TerrainEnergy));
                    Assert.That(secondCell.Energy, Is.EqualTo(firstCell.Energy));
                }
            }
        }

        [Test]
        public void SpeciesRulesKeepTheirAuthoredRoleInTheRulesetFingerprint()
        {
            var plant = new SpeciesRules(
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
                role: SpeciesRole.Plant);
            var creature = CreateRules();

            var plantData = new CellularSimData(
                2,
                2,
                new Dictionary<SpeciesId, float> { [new SpeciesId("fern")] = 0.5f },
                new Dictionary<SpeciesId, SpeciesRules> { [new SpeciesId("fern")] = plant },
                runDurationSeconds: 1f,
                stepInterval: 0.1f);
            var creatureData = new CellularSimData(
                2,
                2,
                new Dictionary<SpeciesId, float> { [new SpeciesId("fern")] = 0.5f },
                new Dictionary<SpeciesId, SpeciesRules> { [new SpeciesId("fern")] = creature },
                runDurationSeconds: 1f,
                stepInterval: 0.1f);

            Assert.That(plant.IsPlant, Is.True);
            Assert.That(creature.IsPlant, Is.False);
            Assert.That(plantData.Fingerprint, Is.Not.EqualTo(creatureData.Fingerprint));
        }

        static SpeciesRules CreateRules(
            SpeciesAwarenessRules awareness = null,
            float movementSpeed = 1f,
            SpeciesRole role = SpeciesRole.Herbivore,
            int forageBelowEnergy = 0,
            int metabolism = 1)
        {
            return new SpeciesRules(
                movementSpeed,
                movementPattern: EmptyPattern,
                attackPattern: EmptyPattern,
                attackAmount: 0,
                blockPattern: EmptyPattern,
                blockAmount: 0,
                dietPattern: EmptyPattern,
                dietTarget: null,
                reproductionPattern: EmptyPattern,
                reproductionNeighborCount: 0,
                metabolism: metabolism,
                awareness: awareness,
                role: role,
                forageBelowEnergy: forageBelowEnergy);
        }

        static void SetPrivateField(object target, string name, object value)
        {
            for (var type = target.GetType(); type != null; type = type.BaseType)
            {
                var field = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
                if (field == null)
                {
                    continue;
                }

                field.SetValue(target, value);
                return;
            }

            Assert.Fail($"Field '{name}' was not found on {target.GetType().Name}.");
        }
    }
}

namespace SaltyGame.Tests
{
    [TestFixture]
    public sealed class SpeciesBehaviorTests
    {
        [Test]
        public void BehaviorSystemChoosesEatingForAdjacentFood()
        {
            var rules = SpeciesRuleDefaults.Create();
            var source = new Grid<SpeciesCell>(3, 1);
            source.SetCell(0, 0, new SpeciesCell(
                SpeciesIds.Herbivore,
                energy: 6,
                age: 0));
            source.SetCell(1, 0, SpeciesCell.Grass(8f));
            var next = source.Copy();
            var metrics = new SpeciesSimulationMetrics();

            SpeciesBehaviorSystem.Update(
                source,
                next,
                rules,
                new System.Random(7),
                metrics);

            Assert.That(next.GetCell(0, 0).BehaviorState, Is.EqualTo(SpeciesBehaviorState.Eating));
            Assert.That(metrics.GetStateTicks(SpeciesIds.Herbivore, SpeciesBehaviorState.Eating), Is.EqualTo(1));
        }

        [Test]
        public void DeadStateIsRecordedBeforeStarvedCreatureIsRemoved()
        {
            var source = new Grid<SpeciesCell>(1, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Herbivore, energy: 1));
            var metrics = new SpeciesSimulationMetrics();

            var next = SpeciesSimulation.Step(
                source,
                SpeciesRuleDefaults.Create(),
                seed: 11,
                metrics: metrics);

            Assert.That(next.GetCell(0, 0).IsCreature, Is.False);
            Assert.That(metrics.GetStateTicks(SpeciesIds.Herbivore, SpeciesBehaviorState.Dead), Is.EqualTo(1));
        }

        [Test]
        public void ExplicitStartingPopulationRemainsStableAcrossSeeds()
        {
            var rules = SpeciesRuleDefaults.Create();
            var probabilities = new Dictionary<SpeciesId, float>
            {
                [SpeciesIds.Plant] = 0f,
                [SpeciesIds.Herbivore] = 0f,
                [SpeciesIds.Carnivore] = 0f,
            };
            var startingPopulations = new Dictionary<SpeciesId, int>
            {
                [SpeciesIds.Herbivore] = 2,
                [SpeciesIds.Carnivore] = 1,
            };
            var data = new CellularSimData(
                8,
                8,
                probabilities,
                rules,
                runDurationSeconds: 1f,
                stepInterval: 0.1f,
                startingPopulations: startingPopulations);

            foreach (var seed in new[] { 1, 2, 3, 4 })
            {
                var grid = SpeciesInitialGridFactory.Create(data, seed);
                var snapshot = SpeciesPopulationSnapshot.Create(grid, 0);
                Assert.That(snapshot.GetCount(SpeciesIds.Herbivore), Is.EqualTo(2));
                Assert.That(snapshot.GetCount(SpeciesIds.Carnivore), Is.EqualTo(1));
            }
        }

        [Test]
        public void SimulationTestHarnessReportsExpectedInitialPopulation()
        {
            var rules = SpeciesRuleDefaults.Create();
            var probabilities = new Dictionary<SpeciesId, float>
            {
                [SpeciesIds.Plant] = 0.5f,
                [SpeciesIds.Herbivore] = 0f,
                [SpeciesIds.Carnivore] = 0f,
            };
            var startingPopulations = new Dictionary<SpeciesId, int>
            {
                [SpeciesIds.Herbivore] = 1,
            };
            var data = new CellularSimData(
                8,
                8,
                probabilities,
                rules,
                runDurationSeconds: 1f,
                stepInterval: 0.1f,
                startingPopulations: startingPopulations);
            var testCase = new SimulationTestCase(
                "explicit starting population",
                data,
                SpeciesIds.Herbivore,
                seedStart: 10,
                seedCount: 2,
                new SimulationTestCriteria(
                    expectedInitialPlayerPopulation: 1,
                    minimumPlayerStateTransitions: 0));

            var report = SimulationTestHarness.Run(testCase);

            Assert.That(report.Runs, Has.Count.EqualTo(2));
            Assert.That(report.Runs[0].InitialPlayerPopulation, Is.EqualTo(1));
            Assert.That(report.Runs[1].InitialPlayerPopulation, Is.EqualTo(1));
            Assert.That(report.Failures, Is.Empty);
        }

        [Test]
        public void BehaviorSystemChoosesHuntingForVisibleFood()
        {
            var rules = SpeciesRuleDefaults.Create();
            var source = new Grid<SpeciesCell>(3, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Herbivore, energy: 6));
            source.SetCell(2, 0, SpeciesCell.Grass(8f));

            var next = source.Copy();
            SpeciesBehaviorSystem.Update(source, next, rules, new System.Random(4));
            Assert.That(next.GetCell(0, 0).BehaviorState, Is.EqualTo(SpeciesBehaviorState.Hunting));
        }

        [Test]
        public void BehaviorSystemChoosesAttackingForAdjacentPrey()
        {
            var rules = SpeciesRuleDefaults.Create();
            var source = new Grid<SpeciesCell>(3, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Carnivore, energy: 6));
            source.SetCell(1, 0, new SpeciesCell(SpeciesIds.Herbivore, energy: 6));
            var next = source.Copy();

            SpeciesBehaviorSystem.Update(source, next, rules, new System.Random(5));

            Assert.That(next.GetCell(0, 0).BehaviorState, Is.EqualTo(SpeciesBehaviorState.Attacking));
        }

        [Test]
        public void BehaviorSystemChoosesFleeingWhenThreatIsVisible()
        {
            var rules = SpeciesRuleDefaults.Create();
            var source = new Grid<SpeciesCell>(3, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Herbivore, energy: 6));
            source.SetCell(1, 0, new SpeciesCell(SpeciesIds.Carnivore, energy: 6));
            var next = source.Copy();

            SpeciesBehaviorSystem.Update(source, next, rules, new System.Random(6));

            Assert.That(next.GetCell(0, 0).BehaviorState, Is.EqualTo(SpeciesBehaviorState.Fleeing));
        }

        [Test]
        public void BehaviorSystemChoosesMatingForAnEnergizedPair()
        {
            var rules = SpeciesRuleDefaults.Create();
            var source = new Grid<SpeciesCell>(3, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Herbivore, energy: 17));
            source.SetCell(1, 0, new SpeciesCell(SpeciesIds.Herbivore, energy: 17));
            var next = source.Copy();

            SpeciesBehaviorSystem.Update(source, next, rules, new System.Random(7));

            Assert.That(next.GetCell(0, 0).BehaviorState, Is.EqualTo(SpeciesBehaviorState.Mating));
        }

        [Test]
        public void SleepingStatePersistsForItsConfiguredDuration()
        {
            var rules = SpeciesRuleDefaults.Create();
            var source = new Grid<SpeciesCell>(1, 1);
            source.SetCell(
                0,
                0,
                new SpeciesCell(SpeciesIds.Herbivore, energy: 17)
                    .WithBehaviorState(SpeciesBehaviorState.Sleeping, ticks: 1));
            var next = source.Copy();

            SpeciesBehaviorSystem.Update(source, next, rules, new System.Random(8));

            Assert.That(next.GetCell(0, 0).BehaviorState, Is.EqualTo(SpeciesBehaviorState.Sleeping));
            Assert.That(next.GetCell(0, 0).BehaviorStateTicks, Is.EqualTo(2));
        }

        [Test]
        public void MovementPreservesBehaviorState()
        {
            var movement = new GridPattern(new[] { Vector2Int.right });
            var rules = new Dictionary<SpeciesId, SpeciesRules>
            {
                [SpeciesIds.Herbivore] = new SpeciesRules(
                    movementSpeed: 1f,
                    movementPattern: movement,
                    attackPattern: movement,
                    attackAmount: 0,
                    blockPattern: movement,
                    blockAmount: 0,
                    dietPattern: movement,
                    dietTarget: null,
                    reproductionPattern: new GridPattern(new Vector2Int[0]),
                    reproductionNeighborCount: 0,
                    reproductionChance: 0f,
                    startingEnergy: 1,
                    role: SpeciesRole.Herbivore),
            };
            var source = new Grid<SpeciesCell>(2, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Herbivore, energy: 1));
            var entityId = source.GetCell(0, 0).EntityId;

            var next = SpeciesSimulation.Step(source, rules, seed: 9);

            Assert.That(next.GetCell(1, 0).IsCreature, Is.True);
            Assert.That(next.GetCell(1, 0).EntityId, Is.EqualTo(entityId));
            Assert.That(next.GetCell(1, 0).BehaviorState, Is.EqualTo(SpeciesBehaviorState.Wandering));
            Assert.That(next.GetCell(1, 0).BehaviorStateTicks, Is.EqualTo(1));
        }
    }
}
