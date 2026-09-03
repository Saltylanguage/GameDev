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
            Assert.That(rules.AttackModifier, Is.EqualTo(3));
            Assert.That(rules.DamageAmount, Is.EqualTo(3));
            Assert.That(rules.DietTarget, Is.EqualTo(SpeciesArchetype.Plant));
            Assert.That(rules.AttackPattern.Offsets[0], Is.EqualTo(Vector2Int.right));
        }

        [Test]
        public void ExperimentalAttackAndDamageUpgradesRemainIndependent()
        {
            var rules = CreateRules();
            var attackModifierUpgrade = SpeciesUpgradeCatalog.Create(
                SpeciesUpgradeCatalog.StrongerAttackModifierId);
            var damageUpgrade = SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.StrongerDamageId);

            var attackModifierRules = attackModifierUpgrade.Apply(rules);
            Assert.That(attackModifierRules.AttackAmount, Is.EqualTo(rules.AttackAmount));
            Assert.That(attackModifierRules.AttackModifier, Is.EqualTo(rules.AttackModifier + 1));
            Assert.That(attackModifierRules.DamageAmount, Is.EqualTo(rules.DamageAmount));

            var damageRules = damageUpgrade.Apply(rules);
            Assert.That(damageRules.AttackAmount, Is.EqualTo(rules.AttackAmount));
            Assert.That(damageRules.AttackModifier, Is.EqualTo(rules.AttackModifier));
            Assert.That(damageRules.DamageAmount, Is.EqualTo(rules.DamageAmount + 1));
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
            var rules = CreateRules(
                role: SpeciesRole.Carnivore,
                forageBelowEnergy: 3,
                maximumEnergy: 24,
                litterMinimum: 2,
                litterMaximum: 4);
            var progression = new SpeciesProgression(new SpeciesDefinition(SpeciesArchetype.Herbivore, rules));
            progression.AddCurrency(5);
            var upgrade = new SpeciesUpgrade("faster", 3, SpeciesUpgradeType.MovementSpeed, 0.5f);

            Assert.That(progression.TryPurchase(upgrade), Is.True);
            Assert.That(progression.Currency, Is.EqualTo(2));
            Assert.That(progression.CurrentRules.MovementSpeed, Is.EqualTo(1.5f));
            Assert.That(progression.CurrentRules.Role, Is.EqualTo(SpeciesRole.Carnivore));
            Assert.That(progression.CurrentRules.ForageBelowEnergy, Is.EqualTo(3));
            Assert.That(progression.CurrentRules.MaximumEnergy, Is.EqualTo(24));
            Assert.That(progression.CurrentRules.LitterMinimum, Is.EqualTo(2));
            Assert.That(progression.CurrentRules.LitterMaximum, Is.EqualTo(4));
        }

        [Test]
        public void UpgradeProgressionCountsOnlySuccessfulPurchases()
        {
            var progression = new SpeciesProgression(
                new SpeciesDefinition(SpeciesArchetype.Herbivore, CreateRules()));
            var upgrade = new SpeciesUpgrade("faster", 5, SpeciesUpgradeType.MovementSpeed, 0.5f);
            var otherUpgrade = new SpeciesUpgrade("attack", 5, SpeciesUpgradeType.AttackAmount, 1f);
            progression.AddCurrency(15);

            Assert.That(progression.PurchasedUpgradeCount, Is.Zero);
            Assert.That(progression.GetUpgradeLevel(upgrade.Id), Is.Zero);
            Assert.That(progression.TryPurchase(upgrade), Is.True);
            Assert.That(progression.TryPurchase(otherUpgrade), Is.True);
            Assert.That(progression.TryPurchase(upgrade), Is.True);
            Assert.That(progression.PurchasedUpgradeCount, Is.EqualTo(3));
            Assert.That(progression.GetUpgradeLevel(upgrade.Id), Is.EqualTo(2));
            Assert.That(progression.GetUpgradeLevel(otherUpgrade.Id), Is.EqualTo(1));
            Assert.That(progression.TryPurchase(otherUpgrade), Is.False);
            Assert.That(progression.PurchasedUpgradeCount, Is.EqualTo(3));
            Assert.That(progression.GetUpgradeLevel(upgrade.Id), Is.EqualTo(2));
            Assert.That(progression.GetUpgradeLevel(otherUpgrade.Id), Is.EqualTo(1));
            Assert.That(progression.GetUpgradeLevel("missing"), Is.Zero);
        }

        [Test]
        public void UpgradeCatalogProvidesStableFasterMovementDefinition()
        {
            var upgrade = SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.FasterMovementId);

            Assert.That(upgrade.Id, Is.EqualTo(SpeciesUpgradeCatalog.FasterMovementId));
            Assert.That(upgrade.Type, Is.EqualTo(SpeciesUpgradeType.MovementSpeed));
            Assert.That(upgrade.Value, Is.EqualTo(0.5f));
        }

        [Test]
        public void ExperimentalHerbivoreUpgradesChangeOnlyTheirNamedRule()
        {
            var rules = CreateRules();

            var toughHide = SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.ToughHideId).Apply(rules);
            var digestion = SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.EfficientDigestionId).Apply(rules);
            var crowding = SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.CrowdingToleranceId).Apply(rules);
            var threatResponse = SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.ThreatResponseId).Apply(rules);

            Assert.That(toughHide.BlockAmount, Is.EqualTo(rules.BlockAmount + 2));
            Assert.That(toughHide.DigestionEnergyBonus, Is.EqualTo(rules.DigestionEnergyBonus));
            Assert.That(digestion.DigestionEnergyBonus, Is.EqualTo(rules.DigestionEnergyBonus + 1));
            Assert.That(digestion.Metabolism, Is.EqualTo(rules.Metabolism));
            Assert.That(crowding.CrowdingTolerance, Is.EqualTo(rules.CrowdingTolerance + 1));
            Assert.That(crowding.MaxReproductionGroupSize, Is.EqualTo(rules.MaxReproductionGroupSize));
            Assert.That(threatResponse.FleeMovementSpeedBonus, Is.EqualTo(rules.FleeMovementSpeedBonus + 0.75f));
            Assert.That(threatResponse.MovementSpeed, Is.EqualTo(rules.MovementSpeed));
        }

        [Test]
        public void ThreatResponseProgressionGrantsSpeedAndCumulativeAvoidanceThroughLevelTwelve()
        {
            var rules = CreateRules();
            var progression = new SpeciesProgression(
                new SpeciesDefinition(SpeciesArchetype.Herbivore, rules));
            var upgrade = SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.ThreatResponseId);
            progression.AddCurrency(60);

            Assert.That(progression.PreContactAvoidanceChance, Is.Zero);
            for (var level = 1; level <= SpeciesUpgradeCatalog.ThreatResponseMaxLevel; level++)
            {
                Assert.That(progression.TryPurchase(upgrade), Is.True);

                Assert.That(
                    progression.CurrentRules.FleeMovementSpeedBonus,
                    Is.EqualTo(rules.FleeMovementSpeedBonus + SpeciesUpgradeCatalog.ThreatResponseFleeSpeedBonus).Within(0.0001f));
                Assert.That(
                    progression.PreContactAvoidanceChance,
                    Is.EqualTo(level * SpeciesUpgradeCatalog.ThreatResponseAvoidanceChanceBonus).Within(0.0001f));
            }

            Assert.That(progression.GetUpgradeLevel(upgrade.Id), Is.EqualTo(SpeciesUpgradeCatalog.ThreatResponseMaxLevel));
            Assert.That(progression.Currency, Is.Zero);
            Assert.That(progression.TryPurchase(upgrade), Is.False);
            Assert.That(progression.GetUpgradeLevel(upgrade.Id), Is.EqualTo(SpeciesUpgradeCatalog.ThreatResponseMaxLevel));
            Assert.That(progression.PreContactAvoidanceChance, Is.EqualTo(0.96f).Within(0.0001f));
            Assert.That(progression.Currency, Is.Zero);
        }

        [Test]
        public void ExperimentalHerbivoreOffersKeepTheChosenPathAndCycleTheOtherThree()
        {
            var initial = SpeciesUpgradeCatalog.CreateExperimentalHerbivoreOffer(null, rotation: 0, seed: 42);
            Assert.That(initial, Has.Length.EqualTo(2));
            Assert.That(initial[0].Id, Is.Not.EqualTo(initial[1].Id));

            var alternatives = new HashSet<string>();
            for (var rotation = 0; rotation < 3; rotation++)
            {
                var offer = SpeciesUpgradeCatalog.CreateExperimentalHerbivoreOffer(
                    SpeciesUpgradeCatalog.ToughHideId,
                    rotation,
                    seed: 42);
                Assert.That(offer[0].Id, Is.EqualTo(SpeciesUpgradeCatalog.ToughHideId));
                Assert.That(offer[1].Id, Is.Not.EqualTo(SpeciesUpgradeCatalog.ToughHideId));
                alternatives.Add(offer[1].Id);
            }

            Assert.That(alternatives, Has.Count.EqualTo(3));
        }

        [Test]
        public void UpgradeCatalogProvidesStrongerBlockDiagnosticDefinition()
        {
            var upgrade = SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.StrongerBlockTwoId);

            Assert.That(upgrade.Id, Is.EqualTo(SpeciesUpgradeCatalog.StrongerBlockTwoId));
            Assert.That(upgrade.Type, Is.EqualTo(SpeciesUpgradeType.BlockAmount));
            Assert.That(upgrade.Value, Is.EqualTo(2f));
        }

        [Test]
        public void UpgradeCatalogSupportsBlockSweepValues()
        {
            var upgrade = SpeciesUpgradeCatalog.Create("stronger-block-10");

            Assert.That(upgrade.Type, Is.EqualTo(SpeciesUpgradeType.BlockAmount));
            Assert.That(upgrade.Value, Is.EqualTo(10f));
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
                    metabolism: 0,
                    awareness: new SpeciesAwarenessRules(visionRange: 1)),
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
                    metabolism: 0,
                    awareness: new SpeciesAwarenessRules(visionRange: 1)),
            };

            var metrics = new SpeciesSimulationMetrics();
            var next = SpeciesSimulation.Step(source, rules, seed: 42, metrics: metrics);

            Assert.That(next.GetCell(0, 0).Health, Is.EqualTo(1));
            Assert.That(next.GetCell(1, 0).Health, Is.EqualTo(3));
            Assert.That(metrics.GetActivity(SpeciesIds.Carnivore).DamageDealt, Is.EqualTo(2));
        }

        [Test]
        public void SuccessfulPredationRecordsFoodActionSeparatelyFromBehaviorDecision()
        {
            var source = new Grid<SpeciesCell>(2, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Carnivore, energy: 1));
            source.SetCell(1, 0, new SpeciesCell(SpeciesIds.Herbivore, health: 1));
            var right = new GridPattern(new[] { Vector2Int.right });
            var rules = new Dictionary<SpeciesId, SpeciesRules>
            {
                [SpeciesIds.Carnivore] = new SpeciesRules(
                    movementSpeed: 0f,
                    movementPattern: EmptyPattern,
                    attackPattern: right,
                    attackAmount: 2,
                    blockPattern: EmptyPattern,
                    blockAmount: 0,
                    dietPattern: right,
                    dietTarget: SpeciesIds.Herbivore,
                    reproductionPattern: EmptyPattern,
                    reproductionNeighborCount: 0,
                    reproductionChance: 0f,
                    startingEnergy: 1,
                    forageBelowEnergy: 5,
                    metabolism: 0,
                    awareness: new SpeciesAwarenessRules(visionRange: 1)),
                [SpeciesIds.Herbivore] = new SpeciesRules(
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
                    energyValue: 4,
                    metabolism: 0),
            };

            var metrics = new SpeciesSimulationMetrics();
            var next = SpeciesSimulation.Step(source, rules, seed: 42, metrics: metrics);
            var activity = metrics.GetActivity(SpeciesIds.Carnivore);

            Assert.That(next.GetCell(1, 0).IsOccupied, Is.False);
            Assert.That(next.GetCell(0, 0).FoodReserve, Is.EqualTo(1f));
            Assert.That(activity.CombatKills, Is.EqualTo(1));
            Assert.That(activity.FoodConsumed, Is.EqualTo(1f));
            Assert.That(activity.FoodActionAttempts, Is.EqualTo(1));
            Assert.That(activity.FoodActionSuccesses, Is.EqualTo(1));
            Assert.That(activity.FoodActionFailures, Is.EqualTo(0));
            Assert.That(activity.FoodActionAttempts, Is.EqualTo(
                activity.FoodActionSuccesses + activity.FoodActionFailures));
            Assert.That(metrics.GetStateTicks(SpeciesIds.Carnivore, SpeciesBehaviorState.Attacking), Is.EqualTo(1));
            Assert.That(metrics.GetStateTicks(SpeciesIds.Carnivore, SpeciesBehaviorState.Eating), Is.EqualTo(0));
        }

        [Test]
        public void BlockAmountReducesPredationDamageUntilTheAttackIsNegated()
        {
            var right = new GridPattern(new[] { Vector2Int.right });
            var left = new GridPattern(new[] { Vector2Int.left });
            foreach (var blockAmount in new[] { 0, 1, 2 })
            {
                var source = new Grid<SpeciesCell>(2, 1);
                source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Carnivore, energy: 1));
                source.SetCell(1, 0, new SpeciesCell(SpeciesIds.Herbivore, health: 1));
                var rules = new Dictionary<SpeciesId, SpeciesRules>
                {
                    [SpeciesIds.Carnivore] = new SpeciesRules(
                        movementSpeed: 0f,
                        movementPattern: EmptyPattern,
                        attackPattern: right,
                        attackAmount: 2,
                        blockPattern: EmptyPattern,
                        blockAmount: 0,
                        dietPattern: right,
                        dietTarget: SpeciesIds.Herbivore,
                        reproductionPattern: EmptyPattern,
                        reproductionNeighborCount: 0,
                        reproductionChance: 0f,
                        startingEnergy: 1,
                        forageBelowEnergy: 5,
                        metabolism: 0,
                        awareness: new SpeciesAwarenessRules(visionRange: 1)),
                    [SpeciesIds.Herbivore] = new SpeciesRules(
                        movementSpeed: 0f,
                        movementPattern: EmptyPattern,
                        attackPattern: EmptyPattern,
                        attackAmount: 0,
                        blockPattern: left,
                        blockAmount: blockAmount,
                        dietPattern: EmptyPattern,
                        dietTarget: null,
                        reproductionPattern: EmptyPattern,
                        reproductionNeighborCount: 0,
                        reproductionChance: 0f,
                        metabolism: 0),
                };

                var metrics = new SpeciesSimulationMetrics();
                var next = SpeciesSimulation.Step(source, rules, seed: 42, metrics: metrics);
                var activity = metrics.GetActivity(SpeciesIds.Carnivore);

                Assert.That(activity.DamageDealt, Is.EqualTo(blockAmount < 2 ? 1 : 0), $"blockAmount={blockAmount}");
                Assert.That(activity.CombatKills, Is.EqualTo(blockAmount < 2 ? 1 : 0), $"blockAmount={blockAmount}");
                Assert.That(next.GetCell(1, 0).IsCreature, Is.EqualTo(blockAmount >= 2), $"blockAmount={blockAmount}");
            }
        }

        [Test]
        public void OpposedRollUsesTheHigherTotalAndDefenderWinsTies()
        {
            Assert.That(SpeciesSimulation.DoesOpposedRollHit(15, 2, 14, 0), Is.True);
            Assert.That(SpeciesSimulation.DoesOpposedRollHit(10, 2, 12, 0), Is.False);
            Assert.That(SpeciesSimulation.DoesOpposedRollHit(12, 0, 10, 2), Is.False);
        }

        [Test]
        public void OpposedRollExpectedProbabilityAccountsForDefenderWinningTies()
        {
            Assert.That(SpeciesSimulation.GetOpposedRollHitProbability(0, 0), Is.EqualTo(0.475f));
            Assert.That(SpeciesSimulation.GetOpposedRollHitProbability(2, 0), Is.EqualTo(0.5725f));
            Assert.That(SpeciesSimulation.GetOpposedRollHitProbability(0, 2), Is.EqualTo(0.3825f));
        }

        [Test]
        public void OpposedRollModeRollsWithoutDirectionalBlockAndUsesAuthoredModifiers()
        {
            var source = new Grid<SpeciesCell>(2, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Carnivore, energy: 1));
            source.SetCell(1, 0, new SpeciesCell(SpeciesIds.Herbivore, health: 3));
            var right = new GridPattern(new[] { Vector2Int.right });
            var rules = new Dictionary<SpeciesId, SpeciesRules>
            {
                [SpeciesIds.Carnivore] = new SpeciesRules(
                    movementSpeed: 0f,
                    movementPattern: EmptyPattern,
                    attackPattern: right,
                    attackAmount: 2,
                    blockPattern: EmptyPattern,
                    blockAmount: 0,
                    dietPattern: right,
                    dietTarget: SpeciesIds.Herbivore,
                    reproductionPattern: EmptyPattern,
                    reproductionNeighborCount: 0,
                    reproductionChance: 0f,
                    startingEnergy: 1,
                    forageBelowEnergy: 5,
                    metabolism: 0,
                    awareness: new SpeciesAwarenessRules(visionRange: 1)),
                [SpeciesIds.Herbivore] = new SpeciesRules(
                    movementSpeed: 0f,
                    movementPattern: EmptyPattern,
                    attackPattern: EmptyPattern,
                    attackAmount: 0,
                    blockPattern: EmptyPattern,
                    blockAmount: 3,
                    dietPattern: EmptyPattern,
                    dietTarget: null,
                    reproductionPattern: EmptyPattern,
                    reproductionNeighborCount: 0,
                    reproductionChance: 0f,
                    metabolism: 0),
            };

            var legacyMetrics = new SpeciesSimulationMetrics();
            var legacy = SpeciesSimulation.Step(source, rules, seed: 42, metrics: legacyMetrics);
            Assert.That(legacy.GetCell(1, 0).Health, Is.EqualTo(1));
            Assert.That(legacyMetrics.CombatRollEvents, Is.Empty);

            var opposedMetrics = new SpeciesSimulationMetrics();
            var opposed = SpeciesSimulation.Step(
                source,
                rules,
                seed: 42,
                metrics: opposedMetrics,
                combatResolutionMode: SpeciesCombatResolutionMode.OpposedRoll);
            var replayMetrics = new SpeciesSimulationMetrics();
            var replay = SpeciesSimulation.Step(
                source,
                rules,
                seed: 42,
                metrics: replayMetrics,
                combatResolutionMode: SpeciesCombatResolutionMode.OpposedRoll);

            Assert.That(opposedMetrics.CombatRollEvents.Count, Is.EqualTo(1));
            var opposedActivity = opposedMetrics.GetActivity(SpeciesIds.Carnivore);
            Assert.That(opposedActivity.CombatOpportunities, Is.EqualTo(1));
            Assert.That(opposedActivity.CombatAttempts, Is.EqualTo(1));
            Assert.That(
                opposedActivity.CombatHits + opposedActivity.CombatBlocked,
                Is.EqualTo(opposedActivity.CombatAttempts));
            Assert.That(
                opposedActivity.CombatDamageApplications,
                Is.EqualTo(opposedActivity.CombatNonLethalHits + opposedActivity.CombatLethalHits));
            var roll = opposedMetrics.CombatRollEvents[0];
            var replayRoll = replayMetrics.CombatRollEvents[0];
            Assert.That(roll.AttackRoll, Is.InRange(1, 20));
            Assert.That(roll.BlockRoll, Is.InRange(1, 20));
            Assert.That(roll.AttackModifier, Is.EqualTo(2));
            Assert.That(roll.BlockModifier, Is.EqualTo(3));
            Assert.That(
                roll.ExpectedHitProbability,
                Is.EqualTo(SpeciesSimulation.GetOpposedRollHitProbability(2, 3)));
            Assert.That(roll.Hit, Is.EqualTo(SpeciesSimulation.DoesOpposedRollHit(
                roll.AttackRoll,
                roll.AttackModifier,
                roll.BlockRoll,
                roll.BlockModifier)));
            Assert.That(opposed.GetCell(1, 0).Health, Is.EqualTo(roll.Hit ? 1 : 3));
            Assert.That(replay.GetCell(1, 0).Health, Is.EqualTo(opposed.GetCell(1, 0).Health));
            Assert.That(replayRoll.AttackRoll, Is.EqualTo(roll.AttackRoll));
            Assert.That(replayRoll.BlockRoll, Is.EqualTo(roll.BlockRoll));
            Assert.That(replayRoll.Hit, Is.EqualTo(roll.Hit));
        }

        [Test]
        public void BevExperimentalOpposedRollSeparatesAttackModifierFromDamage()
        {
            var source = new Grid<SpeciesCell>(2, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Carnivore, energy: 1));
            source.SetCell(1, 0, new SpeciesCell(SpeciesIds.Herbivore, health: 3));
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
                    startingEnergy: 1,
                    forageBelowEnergy: 5,
                    metabolism: 0,
                    awareness: new SpeciesAwarenessRules(visionRange: 1),
                    attackModifier: 20,
                    damageAmount: 1),
                [SpeciesIds.Herbivore] = new SpeciesRules(
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
                    metabolism: 0),
            };
            var metrics = new SpeciesSimulationMetrics();

            var next = SpeciesSimulation.Step(
                source,
                rules,
                seed: 42,
                metrics: metrics,
                combatResolutionMode: SpeciesCombatResolutionMode.OpposedRoll,
                experimentalOptions: new SpeciesExperimentalOptions(
                    SpeciesExperimentalOptions.BevExperimentalFeaturesId));

            Assert.That(metrics.CombatRollEvents.Count, Is.EqualTo(1));
            Assert.That(metrics.CombatRollEvents[0].AttackModifier, Is.EqualTo(20));
            Assert.That(next.GetCell(1, 0).Health, Is.EqualTo(2));
        }

        [Test]
        public void BevExperimentalDamageUpgradeChangesDamageOnAGuaranteedHit()
        {
            var source = new Grid<SpeciesCell>(2, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Carnivore, energy: 1));
            source.SetCell(1, 0, new SpeciesCell(SpeciesIds.Herbivore, health: 5));
            var right = new GridPattern(new[] { Vector2Int.right });
            var rules = new Dictionary<SpeciesId, SpeciesRules>
            {
                [SpeciesIds.Carnivore] = new SpeciesRules(
                    movementSpeed: 0f,
                    movementPattern: EmptyPattern,
                    attackPattern: right,
                    attackAmount: 2,
                    blockPattern: EmptyPattern,
                    blockAmount: 0,
                    dietPattern: right,
                    dietTarget: SpeciesIds.Herbivore,
                    reproductionPattern: EmptyPattern,
                    reproductionNeighborCount: 0,
                    reproductionChance: 0f,
                    startingEnergy: 1,
                    forageBelowEnergy: 5,
                    metabolism: 0,
                    awareness: new SpeciesAwarenessRules(visionRange: 1),
                    attackModifier: 20,
                    damageAmount: 2),
                [SpeciesIds.Herbivore] = new SpeciesRules(
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
                    metabolism: 0),
            };
            rules[SpeciesIds.Carnivore] = SpeciesUpgradeCatalog.Create(
                SpeciesUpgradeCatalog.StrongerDamageId).Apply(rules[SpeciesIds.Carnivore]);
            var metrics = new SpeciesSimulationMetrics();

            var next = SpeciesSimulation.Step(
                source,
                rules,
                seed: 42,
                metrics: metrics,
                combatResolutionMode: SpeciesCombatResolutionMode.OpposedRoll,
                experimentalOptions: new SpeciesExperimentalOptions(
                    SpeciesExperimentalOptions.BevExperimentalFeaturesId));

            Assert.That(metrics.CombatRollEvents.Count, Is.EqualTo(1));
            Assert.That(metrics.CombatRollEvents[0].AttackModifier, Is.EqualTo(20));
            Assert.That(next.GetCell(1, 0).Health, Is.EqualTo(2));
        }

        [Test]
        public void BevExperimentalFoxCooldownBlocksOnlyFollowUpAttacks()
        {
            var fox = new SpeciesId("fox");
            var hare = new SpeciesId("hare");
            var source = new Grid<SpeciesCell>(2, 1);
            source.SetCell(0, 0, new SpeciesCell(fox, energy: 1));
            source.SetCell(1, 0, new SpeciesCell(hare, health: 3));
            var right = new GridPattern(new[] { Vector2Int.right });
            var left = new GridPattern(new[] { Vector2Int.left });
            var rules = new Dictionary<SpeciesId, SpeciesRules>
            {
                [fox] = new SpeciesRules(
                    movementSpeed: 0f,
                    movementPattern: EmptyPattern,
                    attackPattern: right,
                    attackAmount: 1,
                    blockPattern: EmptyPattern,
                    blockAmount: 0,
                    dietPattern: right,
                    dietTarget: hare,
                    reproductionPattern: EmptyPattern,
                    reproductionNeighborCount: 0,
                    reproductionChance: 0f,
                    startingEnergy: 1,
                    forageBelowEnergy: 5,
                    metabolism: 0,
                    awareness: new SpeciesAwarenessRules(visionRange: 1)),
                [hare] = new SpeciesRules(
                    movementSpeed: 0f,
                    movementPattern: EmptyPattern,
                    attackPattern: EmptyPattern,
                    attackAmount: 0,
                    blockPattern: left,
                    blockAmount: 0,
                    dietPattern: EmptyPattern,
                    dietTarget: null,
                    reproductionPattern: EmptyPattern,
                    reproductionNeighborCount: 0,
                    reproductionChance: 0f,
                    metabolism: 0),
            };
            var experimental = new SpeciesExperimentalOptions(
                SpeciesExperimentalOptions.BevExperimentalFeaturesId,
                foxAttackCooldownTicks: 2);

            var firstMetrics = new SpeciesSimulationMetrics();
            var first = SpeciesSimulation.Step(
                source,
                rules,
                seed: 42,
                metrics: firstMetrics,
                combatResolutionMode: SpeciesCombatResolutionMode.OpposedRoll,
                experimentalOptions: experimental);
            Assert.That(first.GetCell(0, 0).AttackCooldownTicksRemaining, Is.EqualTo(2));
            Assert.That(firstMetrics.GetActivity(fox).CombatAttempts, Is.EqualTo(1));

            var secondMetrics = new SpeciesSimulationMetrics();
            var second = SpeciesSimulation.Step(
                first,
                rules,
                seed: 43,
                metrics: secondMetrics,
                combatResolutionMode: SpeciesCombatResolutionMode.OpposedRoll,
                experimentalOptions: experimental);
            Assert.That(second.GetCell(0, 0).AttackCooldownTicksRemaining, Is.EqualTo(1));
            Assert.That(secondMetrics.GetActivity(fox).CombatAttempts, Is.EqualTo(0));
            Assert.That(second.GetCell(1, 0).Health, Is.EqualTo(first.GetCell(1, 0).Health));
            Assert.That(secondMetrics.CombatCooldownSuppressionEvents.Count, Is.EqualTo(1));
            Assert.That(secondMetrics.CombatCooldownSuppressionEvents[0].AttackerSpecies, Is.EqualTo(fox));
            Assert.That(secondMetrics.CombatCooldownSuppressionEvents[0].RemainingTicks, Is.EqualTo(1));

            var thirdMetrics = new SpeciesSimulationMetrics();
            var third = SpeciesSimulation.Step(
                second,
                rules,
                seed: 44,
                metrics: thirdMetrics,
                combatResolutionMode: SpeciesCombatResolutionMode.OpposedRoll,
                experimentalOptions: experimental);
            Assert.That(third.GetCell(0, 0).AttackCooldownTicksRemaining, Is.EqualTo(2));
            Assert.That(thirdMetrics.GetActivity(fox).CombatAttempts, Is.EqualTo(1));
        }

        [Test]
        public void FixedRateDiagnosticOpportunityIsDeterministicAndUpgradeIndependent()
        {
            var source = new Grid<SpeciesCell>(2, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Carnivore, energy: 10));
            source.SetCell(1, 0, new SpeciesCell(SpeciesIds.Herbivore, health: 3));
            var right = new GridPattern(new[] { Vector2Int.right });
            var left = new GridPattern(new[] { Vector2Int.left });
            var baselineRules = CreateControlledOpportunityRules(right, left, blockAmount: 0);
            var upgradeRules = CreateControlledOpportunityRules(right, left, blockAmount: 2);

            var baselineMetrics = new SpeciesSimulationMetrics();
            var baseline = SpeciesSimulation.Step(
                source,
                baselineRules,
                seed: 10200,
                metrics: baselineMetrics,
                combatResolutionMode: SpeciesCombatResolutionMode.OpposedRoll,
                attackOpportunityMode: SpeciesAttackOpportunityMode.FixedRateDiagnostic);
            var upgradeMetrics = new SpeciesSimulationMetrics();
            var upgrade = SpeciesSimulation.Step(
                source,
                upgradeRules,
                seed: 10200,
                metrics: upgradeMetrics,
                combatResolutionMode: SpeciesCombatResolutionMode.OpposedRoll,
                attackOpportunityMode: SpeciesAttackOpportunityMode.FixedRateDiagnostic);
            var replayMetrics = new SpeciesSimulationMetrics();
            var replay = SpeciesSimulation.Step(
                source,
                baselineRules,
                seed: 10200,
                metrics: replayMetrics,
                combatResolutionMode: SpeciesCombatResolutionMode.OpposedRoll,
                attackOpportunityMode: SpeciesAttackOpportunityMode.FixedRateDiagnostic);

            Assert.That(baselineMetrics.ControlledOpportunityScheduled, Is.EqualTo(1));
            Assert.That(upgradeMetrics.ControlledOpportunityScheduled, Is.EqualTo(1));
            Assert.That(baselineMetrics.ControlledOpportunityEligible, Is.EqualTo(1));
            Assert.That(upgradeMetrics.ControlledOpportunityEligible, Is.EqualTo(1));
            Assert.That(baselineMetrics.GetActivity(SpeciesIds.Carnivore).CombatAttempts, Is.EqualTo(1));
            Assert.That(upgradeMetrics.GetActivity(SpeciesIds.Carnivore).CombatAttempts, Is.EqualTo(1));
            Assert.That(replayMetrics.ControlledOpportunityScheduled, Is.EqualTo(baselineMetrics.ControlledOpportunityScheduled));
            Assert.That(replay.GetCell(1, 0).Health, Is.EqualTo(baseline.GetCell(1, 0).Health));
            Assert.That(upgrade.GetCell(1, 0).IsCreature, Is.True);
        }

        [Test]
        public void FixedContactSurvivalLabShowsHowBlockChangesAttacksUntilFirstLethalHit()
        {
            const int episodeCount = 1000;
            const int maximumAttempts = 100;
            var fox = SpeciesIds.Carnivore;
            var hare = SpeciesIds.Herbivore;
            var right = new GridPattern(new[] { Vector2Int.right });
            var averageAttemptsByBlock = new List<float>();
            var experimental = new SpeciesExperimentalOptions(
                SpeciesExperimentalOptions.BevExperimentalFeaturesId);

            for (var blockAmount = 0; blockAmount <= 10; blockAmount++)
            {
                var totalAttempts = 0;
                for (var episode = 0; episode < episodeCount; episode++)
                {
                    var source = new Grid<SpeciesCell>(2, 1);
                    source.SetCell(0, 0, new SpeciesCell(fox, energy: 0));
                    source.SetCell(1, 0, new SpeciesCell(hare, health: 1));
                    var rules = new Dictionary<SpeciesId, SpeciesRules>
                    {
                        [fox] = new SpeciesRules(
                            movementSpeed: 0f,
                            movementPattern: EmptyPattern,
                            attackPattern: right,
                            attackAmount: 1,
                            blockPattern: EmptyPattern,
                            blockAmount: 0,
                            dietPattern: right,
                            dietTarget: hare,
                            reproductionPattern: EmptyPattern,
                            reproductionNeighborCount: 0,
                            reproductionChance: 0f,
                            forageBelowEnergy: 0,
                            metabolism: 0),
                        [hare] = new SpeciesRules(
                            movementSpeed: 0f,
                            movementPattern: EmptyPattern,
                            attackPattern: EmptyPattern,
                            attackAmount: 0,
                            blockPattern: EmptyPattern,
                            blockAmount: blockAmount,
                            dietPattern: EmptyPattern,
                            dietTarget: null,
                            reproductionPattern: EmptyPattern,
                            reproductionNeighborCount: 0,
                            reproductionChance: 0f,
                            metabolism: 0),
                    };
                    var attemptsBeforeDeath = 0;
                    for (var attackIndex = 0; attackIndex < maximumAttempts; attackIndex++)
                    {
                        var metrics = new SpeciesSimulationMetrics();
                        var next = SpeciesSimulation.Step(
                            source,
                            rules,
                            seed: (episode * maximumAttempts + attackIndex) * 3,
                            metrics: metrics,
                            combatResolutionMode: SpeciesCombatResolutionMode.OpposedRoll,
                            attackOpportunityMode: SpeciesAttackOpportunityMode.FixedRateDiagnostic,
                            experimentalOptions: experimental);
                        if (metrics.GetActivity(fox).CombatAttempts == 0)
                        {
                            continue;
                        }

                        attemptsBeforeDeath++;
                        if (!next.GetCell(1, 0).IsCreature)
                        {
                            break;
                        }

                        source = next;
                    }

                    totalAttempts += attemptsBeforeDeath;
                }

                var averageAttempts = totalAttempts / (float)episodeCount;
                averageAttemptsByBlock.Add(averageAttempts);
                TestContext.Out.WriteLine(
                    $"[CombatLab] block={blockAmount} averageAttemptsBeforeDeath={averageAttempts:0.000}");
            }

            for (var index = 1; index < averageAttemptsByBlock.Count; index++)
            {
                Assert.That(
                    averageAttemptsByBlock[index],
                    Is.GreaterThanOrEqualTo(averageAttemptsByBlock[index - 1]),
                    $"Block {index} should not reduce survival versus block {index - 1}.");
            }
        }

        [Test]
        public void PairedOpportunityIntersectionUsesStableContactIdentity()
        {
            var a = new SpeciesAttackOpportunity(
                SpeciesIds.Carnivore, 0, 0, SpeciesIds.Herbivore, 1, 0, Vector2Int.right);
            var b = new SpeciesAttackOpportunity(
                SpeciesIds.Carnivore, 1, 0, SpeciesIds.Herbivore, 2, 0, Vector2Int.right);
            var c = new SpeciesAttackOpportunity(
                SpeciesIds.Carnivore, 2, 0, SpeciesIds.Herbivore, 3, 0, Vector2Int.right);
            var d = new SpeciesAttackOpportunity(
                SpeciesIds.Carnivore, 3, 0, SpeciesIds.Herbivore, 4, 0, Vector2Int.right);
            var e = new SpeciesAttackOpportunity(
                SpeciesIds.Carnivore, 4, 0, SpeciesIds.Herbivore, 5, 0, Vector2Int.right);
            var baselineOnly = new List<SpeciesAttackOpportunity>();
            var blockPlusTwoOnly = new List<SpeciesAttackOpportunity>();

            var common = SpeciesAttackOpportunity.Intersect(
                new[] { a, b, c, d },
                new[] { b, c, d, e },
                baselineOnly,
                blockPlusTwoOnly);

            Assert.That(common, Is.EqualTo(new[] { b, c, d }));
            Assert.That(baselineOnly, Is.EqualTo(new[] { a }));
            Assert.That(blockPlusTwoOnly, Is.EqualTo(new[] { e }));
        }

        [Test]
        public void OpportunityStrataClassifyEveryValidityCombination()
        {
            Assert.That(SpeciesOpportunityStrata.Classify(true, true), Is.EqualTo(SpeciesOpportunityStrata.Common));
            Assert.That(SpeciesOpportunityStrata.Classify(true, false), Is.EqualTo(SpeciesOpportunityStrata.BaselineOnly));
            Assert.That(SpeciesOpportunityStrata.Classify(false, true), Is.EqualTo(SpeciesOpportunityStrata.BlockOnly));
        }

        [Test]
        public void PairedLockstepExecutesTheCommonOpportunityInBothArms()
        {
            var source = new Grid<SpeciesCell>(2, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Carnivore, energy: 10));
            source.SetCell(1, 0, new SpeciesCell(SpeciesIds.Herbivore, health: 3));
            var right = new GridPattern(new[] { Vector2Int.right });
            var left = new GridPattern(new[] { Vector2Int.left });
            var baselineMetrics = new SpeciesSimulationMetrics();
            var blockPlusTwoMetrics = new SpeciesSimulationMetrics();
            var opportunityObservations = new List<SpeciesPairedOpportunityObservation>();

            var result = SpeciesSimulation.StepPaired(
                source,
                CreateControlledOpportunityRules(right, left, blockAmount: 0),
                source.Copy(),
                CreateControlledOpportunityRules(right, left, blockAmount: 2),
                seed: 10200,
                baselineMaxPopulation: 0,
                blockPlusTwoMaxPopulation: 0,
                baselineTerrainDefinitions: TerrainDefaults.Create(),
                blockPlusTwoTerrainDefinitions: TerrainDefaults.Create(),
                baselineAlphaOffspringRules: null,
                blockPlusTwoAlphaOffspringRules: null,
                baselineMetrics: baselineMetrics,
                blockPlusTwoMetrics: blockPlusTwoMetrics,
                combatResolutionMode: SpeciesCombatResolutionMode.OpposedRoll,
                out var baselineNext,
                out var blockPlusTwoNext,
                out var pairedOpportunityId,
                opportunityObservations,
                tick: 1);

            Assert.That(result.BaselineValid, Is.EqualTo(1));
            Assert.That(result.BlockPlusTwoValid, Is.EqualTo(1));
            Assert.That(result.CommonValid, Is.EqualTo(1));
            Assert.That(result.BaselineOnly, Is.Zero);
            Assert.That(result.BlockPlusTwoOnly, Is.Zero);
            Assert.That(result.PairedAttemptExecuted, Is.True);
            Assert.That(result.Invalidated, Is.False);
            Assert.That(pairedOpportunityId, Does.Contain("carnivore@0,0->herbivore@1,0"));
            Assert.That(opportunityObservations, Has.Count.EqualTo(1));
            Assert.That(opportunityObservations[0].stratum, Is.EqualTo(SpeciesOpportunityStrata.Common));
            Assert.That(opportunityObservations[0].baseline.present, Is.True);
            Assert.That(opportunityObservations[0].blockPlusTwo.present, Is.True);
            Assert.That(
                baselineMetrics.GetActivity(SpeciesIds.Carnivore).CombatAttempts,
                Is.EqualTo(1));
            Assert.That(
                blockPlusTwoMetrics.GetActivity(SpeciesIds.Carnivore).CombatAttempts,
                Is.EqualTo(1));
            Assert.That(
                baselineMetrics.GetActivity(SpeciesIds.Carnivore).CombatAttempts,
                Is.EqualTo(blockPlusTwoMetrics.GetActivity(SpeciesIds.Carnivore).CombatAttempts));
            Assert.That(baselineNext.GetCell(0, 0).IsCreature, Is.True);
            Assert.That(blockPlusTwoNext.GetCell(0, 0).IsCreature, Is.True);
        }

        [Test]
        public void PairedRunnerAdvancesAndCompletesBothArmsTogether()
        {
            var source = new Grid<SpeciesCell>(2, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Carnivore, energy: 10));
            source.SetCell(1, 0, new SpeciesCell(SpeciesIds.Herbivore, health: 3));
            var right = new GridPattern(new[] { Vector2Int.right });
            var left = new GridPattern(new[] { Vector2Int.left });
            var baselineRules = CreateControlledOpportunityRules(right, left, blockAmount: 0);
            var blockRules = CreateControlledOpportunityRules(right, left, blockAmount: 2);
            var emptyProbabilities = new Dictionary<SpeciesId, float>();
            var baselineData = new CellularSimData(
                2, 1, emptyProbabilities, baselineRules, runDurationSeconds: 2f, stepInterval: 1f);
            var blockData = new CellularSimData(
                2, 1, emptyProbabilities, blockRules, runDurationSeconds: 2f, stepInterval: 1f);
            var baselineRun = new SimulationRunState(source.Copy(), SpeciesIds.Herbivore, 10200, 2f);
            var blockRun = new SimulationRunState(source.Copy(), SpeciesIds.Herbivore, 10200, 2f);
            var runner = new SpeciesPairedSimulationRunner(
                baselineRun,
                baselineData,
                blockRun,
                blockData,
                SpeciesCombatResolutionMode.OpposedRoll);

            Assert.That(runner.AdvanceOneTick(), Is.True);
            Assert.That(baselineRun.Tick, Is.EqualTo(1));
            Assert.That(blockRun.Tick, Is.EqualTo(1));
            Assert.That(baselineRun.Status, Is.EqualTo(SimulationRunStatus.Running));
            Assert.That(blockRun.Status, Is.EqualTo(SimulationRunStatus.Running));

            Assert.That(runner.AdvanceOneTick(), Is.True);
            Assert.That(baselineRun.Tick, Is.EqualTo(2));
            Assert.That(blockRun.Tick, Is.EqualTo(2));
            Assert.That(baselineRun.Status, Is.EqualTo(SimulationRunStatus.Complete));
            Assert.That(blockRun.Status, Is.EqualTo(SimulationRunStatus.Complete));
            Assert.That(runner.OpportunityControl.Scheduled, Is.GreaterThan(0));
            Assert.That(runner.AdvanceOneTick(), Is.False);
        }

        static Dictionary<SpeciesId, SpeciesRules> CreateControlledOpportunityRules(
            GridPattern attackPattern,
            GridPattern blockPattern,
            int blockAmount)
        {
            return new Dictionary<SpeciesId, SpeciesRules>
            {
                [SpeciesIds.Carnivore] = new SpeciesRules(
                    movementSpeed: 0f,
                    movementPattern: EmptyPattern,
                    attackPattern: attackPattern,
                    attackAmount: 2,
                    blockPattern: EmptyPattern,
                    blockAmount: 0,
                    dietPattern: attackPattern,
                    dietTarget: SpeciesIds.Herbivore,
                    reproductionPattern: EmptyPattern,
                    reproductionNeighborCount: 0,
                    reproductionChance: 0f,
                    startingEnergy: 10,
                    forageBelowEnergy: 0,
                    metabolism: 0,
                    awareness: new SpeciesAwarenessRules(visionRange: 1)),
                [SpeciesIds.Herbivore] = new SpeciesRules(
                    movementSpeed: 0f,
                    movementPattern: EmptyPattern,
                    attackPattern: EmptyPattern,
                    attackAmount: 0,
                    blockPattern: blockPattern,
                    blockAmount: blockAmount,
                    dietPattern: EmptyPattern,
                    dietTarget: null,
                    reproductionPattern: EmptyPattern,
                    reproductionNeighborCount: 0,
                    reproductionChance: 0f,
                    metabolism: 0),
            };
        }

        [Test]
        public void BlockedPredationRecordsAFailedFoodActionWithoutFoodConsumed()
        {
            var source = new Grid<SpeciesCell>(2, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Carnivore, energy: 1));
            source.SetCell(1, 0, new SpeciesCell(SpeciesIds.Herbivore, health: 3));
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
                    startingEnergy: 1,
                    forageBelowEnergy: 5,
                    metabolism: 0,
                    awareness: new SpeciesAwarenessRules(visionRange: 1)),
                [SpeciesIds.Herbivore] = new SpeciesRules(
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
                    metabolism: 0),
            };

            var metrics = new SpeciesSimulationMetrics();
            var next = SpeciesSimulation.Step(source, rules, seed: 42, metrics: metrics);
            var activity = metrics.GetActivity(SpeciesIds.Carnivore);

            Assert.That(next.GetCell(1, 0).Health, Is.EqualTo(2));
            Assert.That(activity.FoodConsumed, Is.EqualTo(0f));
            Assert.That(activity.FoodActionAttempts, Is.EqualTo(1));
            Assert.That(activity.FoodActionSuccesses, Is.EqualTo(0));
            Assert.That(activity.FoodActionFailures, Is.EqualTo(1));
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
                    forageBelowEnergy: 5,
                    awareness: new SpeciesAwarenessRules(visionRange: 1)),
                [SpeciesIds.Herbivore] = CreateRules(metabolism: 0),
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
            var reproduction = metrics.GetReproductionActivity(SpeciesIds.Carnivore);
            Assert.That(reproduction.Candidates, Is.EqualTo(2));
            Assert.That(reproduction.SuccessfulAttempts, Is.EqualTo(1));
            Assert.That(reproduction.BlockedNoBirthLocation, Is.EqualTo(1));
            Assert.That(reproduction.IsReconciled, Is.True);

            source.SetCell(1, 0, new SpeciesCell(SpeciesArchetype.Carnivore, energy: 0));
            var withoutFood = SpeciesSimulation.Step(source, rules, seed: 42);
            Assert.That(withoutFood.GetCell(2, 0).IsOccupied, Is.False);
        }

        [Test]
        public void ReproductionFunnelRecordsInsufficientEnergy()
        {
            var source = new Grid<SpeciesCell>(1, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Carnivore, energy: 1));
            var metrics = StepWithReproductionMetrics(
                source,
                CreateReproductionRules(reproductionFoodRequired: 1),
                seed: 42);

            var reproduction = metrics.GetReproductionActivity(SpeciesIds.Carnivore);
            Assert.That(reproduction.Candidates, Is.EqualTo(1));
            Assert.That(reproduction.BlockedEnergy, Is.EqualTo(1));
            Assert.That(reproduction.IsReconciled, Is.True);
        }

        [Test]
        public void ReproductionFunnelRecordsMissingMate()
        {
            var source = new Grid<SpeciesCell>(2, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Carnivore, energy: 3));
            var metrics = StepWithReproductionMetrics(
                source,
                CreateReproductionRules(reproductionNeighborCount: 1),
                seed: 42);

            var reproduction = metrics.GetReproductionActivity(SpeciesIds.Carnivore);
            Assert.That(reproduction.Candidates, Is.EqualTo(1));
            Assert.That(reproduction.BlockedMateRequirement, Is.EqualTo(1));
            Assert.That(reproduction.IsReconciled, Is.True);
        }

        [Test]
        public void ReproductionFunnelRecordsGroupLimit()
        {
            var source = new Grid<SpeciesCell>(3, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Carnivore, energy: 3));
            source.SetCell(1, 0, new SpeciesCell(SpeciesIds.Carnivore, energy: 3));
            var metrics = StepWithReproductionMetrics(
                source,
                CreateReproductionRules(
                    reproductionNeighborCount: 1,
                    maxReproductionGroupSize: 2),
                seed: 42);

            var reproduction = metrics.GetReproductionActivity(SpeciesIds.Carnivore);
            Assert.That(reproduction.Candidates, Is.EqualTo(2));
            Assert.That(reproduction.BlockedGroupLimit, Is.EqualTo(2));
            Assert.That(reproduction.IsReconciled, Is.True);
        }

        [Test]
        public void ReproductionFunnelRecordsFailedChanceRoll()
        {
            var source = new Grid<SpeciesCell>(2, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Carnivore, energy: 3));
            var metrics = StepWithReproductionMetrics(
                source,
                CreateReproductionRules(reproductionChance: 0.000001f),
                seed: 42);

            var reproduction = metrics.GetReproductionActivity(SpeciesIds.Carnivore);
            Assert.That(reproduction.Candidates, Is.EqualTo(1));
            Assert.That(reproduction.FailedChanceRoll, Is.EqualTo(1));
            Assert.That(reproduction.IsReconciled, Is.True);
        }

        [Test]
        public void ReproductionFunnelRecordsUnavailableBirthLocation()
        {
            var source = new Grid<SpeciesCell>(2, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Carnivore, energy: 3));
            source.SetCell(1, 0, new SpeciesCell(SpeciesIds.Carnivore, energy: 3));
            var metrics = StepWithReproductionMetrics(
                source,
                CreateReproductionRules(reproductionNeighborCount: 1),
                seed: 42);

            var reproduction = metrics.GetReproductionActivity(SpeciesIds.Carnivore);
            Assert.That(reproduction.Candidates, Is.EqualTo(2));
            Assert.That(reproduction.BlockedNoBirthLocation, Is.EqualTo(2));
            Assert.That(reproduction.IsReconciled, Is.True);
        }

        [Test]
        public void ReproductionFunnelRecordsOneSuccessfulAttemptForMultipleBirths()
        {
            var source = new Grid<SpeciesCell>(3, 1);
            source.SetCell(1, 0, new SpeciesCell(SpeciesIds.Carnivore, energy: 5));
            var metrics = StepWithReproductionMetrics(
                source,
                CreateReproductionRules(
                    reproductionFoodRequired: 1,
                    litterMinimum: 2,
                    litterMaximum: 2),
                seed: 42);

            var reproduction = metrics.GetReproductionActivity(SpeciesIds.Carnivore);
            Assert.That(reproduction.Candidates, Is.EqualTo(1));
            Assert.That(reproduction.SuccessfulAttempts, Is.EqualTo(1));
            Assert.That(metrics.GetActivity(SpeciesIds.Carnivore).Births, Is.EqualTo(2));
            Assert.That(reproduction.IsReconciled, Is.True);
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
                reproductionFoodRequired: 2,
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
        public void HareFleesApproachingFoxWhileFoxPursuesVisibleHare()
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
                    attackPattern: leftRight,
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
                    forageBelowEnergy: 8,
                    awareness: new SpeciesAwarenessRules(visionRange: 3, intelligence: 1)),
                [SpeciesIds.Plant] = CreateRules(
                    movementSpeed: 0f,
                    role: SpeciesRole.Plant,
                    metabolism: -1),
            };

            var fleeing = new Grid<SpeciesCell>(3, 1);
            fleeing.SetCell(0, 0, SpeciesCell.Grass(2f));
            var fleeingHare = new SpeciesCell(hare, energy: 8);
            var fleeingFox = new SpeciesCell(fox, energy: 8);
            fleeing.SetCell(1, 0, fleeingHare);
            fleeing.SetCell(2, 0, fleeingFox);
            var previousThreatened = new Grid<SpeciesCell>(3, 1);
            previousThreatened.SetCell(0, 0, fleeingHare.WithEntity(
                hare,
                fleeingHare.Health,
                fleeingHare.Energy,
                fleeingHare.Age,
                fleeingHare.FoodEaten,
                fleeingHare.FoodReserve,
                entityId: fleeingHare.EntityId));
            previousThreatened.SetCell(2, 0, fleeingFox.WithEntity(
                fox,
                fleeingFox.Health,
                fleeingFox.Energy,
                fleeingFox.Age,
                fleeingFox.FoodEaten,
                fleeingFox.FoodReserve,
                entityId: fleeingFox.EntityId));
            var escaped = SpeciesSimulation.Step(
                fleeing,
                rules,
                seed: 11,
                previousSource: previousThreatened);

            Assert.That(escaped.GetCell(0, 0).SpeciesId, Is.EqualTo(hare));
            Assert.That(escaped.GetCell(0, 0).IsTerrainResource, Is.True);
            Assert.That(escaped.GetCell(2, 0).SpeciesId, Is.EqualTo(fox));

            var pursuing = new Grid<SpeciesCell>(3, 1);
            pursuing.SetCell(0, 0, new SpeciesCell(fox, energy: 8));
            pursuing.SetCell(
                2,
                0,
                new SpeciesCell(hare, energy: 8)
                    .WithBehaviorState(SpeciesBehaviorState.Sleeping));
            var hunted = SpeciesSimulation.Step(pursuing, rules, seed: 11);

            Assert.That(hunted.GetCell(1, 0).SpeciesId, Is.EqualTo(fox));
            Assert.That(hunted.GetCell(2, 0).SpeciesId, Is.EqualTo(hare));

            var threatResponseUpgrade = SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.ThreatResponseId);
            var threatResponseRules = new Dictionary<SpeciesId, SpeciesRules>(rules)
            {
                [hare] = threatResponseUpgrade.Apply(threatResponseUpgrade.Apply(rules[hare])),
            };
            var fasterThreatened = new Grid<SpeciesCell>(5, 1);
            var fasterFox = new SpeciesCell(fox, energy: 8);
            var fasterHare = new SpeciesCell(hare, energy: 8);
            fasterThreatened.SetCell(1, 0, fasterFox);
            fasterThreatened.SetCell(2, 0, fasterHare);
            var previousFasterThreatened = new Grid<SpeciesCell>(5, 1);
            previousFasterThreatened.SetCell(1, 0, fasterFox.WithEntity(
                fox,
                fasterFox.Health,
                fasterFox.Energy,
                fasterFox.Age,
                fasterFox.FoodEaten,
                fasterFox.FoodReserve,
                entityId: fasterFox.EntityId));
            previousFasterThreatened.SetCell(3, 0, fasterHare.WithEntity(
                hare,
                fasterHare.Health,
                fasterHare.Energy,
                fasterHare.Age,
                fasterHare.FoodEaten,
                fasterHare.FoodReserve,
                entityId: fasterHare.EntityId));

            var escapedTwice = SpeciesSimulation.Step(
                fasterThreatened,
                threatResponseRules,
                seed: 11,
                previousSource: previousFasterThreatened);

            Assert.That(escapedTwice.GetCell(4, 0).SpeciesId, Is.EqualTo(hare));
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
                metabolism: -1,
                startingFoodReserve: 1f);
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
                metabolism: 0,
                awareness: new SpeciesAwarenessRules(visionRange: 1));
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
            Assert.That(firstMetrics.GetActivity(SpeciesIds.Herbivore).FoodActionAttempts, Is.EqualTo(2));
            Assert.That(firstMetrics.GetActivity(SpeciesIds.Herbivore).FoodActionSuccesses, Is.EqualTo(2));
            Assert.That(firstMetrics.GetActivity(SpeciesIds.Herbivore).FoodActionFailures, Is.EqualTo(0));

            var secondMetrics = new SpeciesSimulationMetrics();
            var second = SpeciesSimulation.Step(first, rules, seed: 43, metrics: secondMetrics);
            Assert.That(second.GetCell(2, 0).IsOccupied, Is.False);
            Assert.That(
                second.GetCell(1, 0).FoodReserve + second.GetCell(3, 0).FoodReserve,
                Is.EqualTo(3.25f).Within(0.001f));
            Assert.That(secondMetrics.GetActivity(SpeciesIds.Herbivore).FoodConsumed, Is.EqualTo(1.25f));
            Assert.That(secondMetrics.GetActivity(SpeciesIds.Herbivore).FoodActionAttempts, Is.EqualTo(2));
            Assert.That(secondMetrics.GetActivity(SpeciesIds.Herbivore).FoodActionSuccesses, Is.EqualTo(2));
            Assert.That(secondMetrics.GetActivity(SpeciesIds.Herbivore).FoodActionFailures, Is.EqualTo(0));
            Assert.That(
                secondMetrics.GetActivity(SpeciesIds.Herbivore).FoodActionAttempts,
                Is.EqualTo(
                    secondMetrics.GetActivity(SpeciesIds.Herbivore).FoodActionSuccesses
                    + secondMetrics.GetActivity(SpeciesIds.Herbivore).FoodActionFailures));
            Assert.That(secondMetrics.GetActivity(SpeciesIds.Plant).Deaths, Is.EqualTo(1));
        }

        [Test]
        public void EfficientDigestionAddsEnergyWithoutConsumingMorePlantFood()
        {
            var right = new GridPattern(new[] { Vector2Int.right });
            var source = new Grid<SpeciesCell>(2, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Herbivore, energy: 1));
            source.SetCell(1, 0, new SpeciesCell(SpeciesIds.Plant, foodReserve: 5f));
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
                energyValue: 1,
                metabolism: 0,
                role: SpeciesRole.Plant);
            var herbivoreRules = new SpeciesRules(
                movementSpeed: 1f,
                movementPattern: right,
                attackPattern: EmptyPattern,
                attackAmount: 0,
                blockPattern: EmptyPattern,
                blockAmount: 0,
                dietPattern: right,
                dietTarget: SpeciesIds.Plant,
                reproductionPattern: EmptyPattern,
                reproductionNeighborCount: 0,
                forageBelowEnergy: 2,
                metabolism: 0,
                digestionEnergyBonus: 1);
            var rules = new Dictionary<SpeciesId, SpeciesRules>
            {
                [SpeciesIds.Plant] = plantRules,
                [SpeciesIds.Herbivore] = herbivoreRules,
            };

            var next = SpeciesSimulation.Step(source, rules, seed: 42);

            Assert.That(next.GetCell(0, 0).Energy, Is.EqualTo(3));
            Assert.That(next.GetCell(1, 0).FoodReserve, Is.EqualTo(4f));
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
        public void CellularSimDataCanResizeAnAuthoredGridWithoutChangingRules()
        {
            var data = new CellularSimData(
                4,
                3,
                new Dictionary<SpeciesId, float>
                {
                    [SpeciesIds.Herbivore] = 0.5f,
                },
                new Dictionary<SpeciesId, SpeciesRules>
                {
                    [SpeciesIds.Herbivore] = CreateRules(),
                },
                runDurationSeconds: 10f,
                stepInterval: 0.1f);

            var resized = data.WithGridSize(9, 7);

            Assert.That(resized.Width, Is.EqualTo(9));
            Assert.That(resized.Height, Is.EqualTo(7));
            Assert.That(resized.SpeciesRules[SpeciesIds.Herbivore], Is.SameAs(data.SpeciesRules[SpeciesIds.Herbivore]));
            Assert.That(resized.StartingProbabilities[SpeciesIds.Herbivore], Is.EqualTo(0.5f));
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
            Assert.That(first.WithSpeciesRules(
                    SpeciesIds.Herbivore,
                    CreateRules(attackModifier: 1)).Fingerprint,
                Is.Not.EqualTo(first.Fingerprint));
            Assert.That(first.WithSpeciesRules(
                    SpeciesIds.Herbivore,
                    CreateRules(damageAmount: 1)).Fingerprint,
                Is.Not.EqualTo(first.Fingerprint));
            Assert.That(first.WithSpeciesRules(
                    SpeciesIds.Herbivore,
                    CreateRules(maximumEnergy: 10)).Fingerprint,
                Is.Not.EqualTo(first.Fingerprint));
            Assert.That(first.WithSpeciesRules(
                    SpeciesIds.Herbivore,
                    CreateRules(litterMaximum: 2)).Fingerprint,
                Is.Not.EqualTo(first.Fingerprint));
        }

        [Test]
        public void RunProvenanceFingerprintIncludesExecutionOptionsAndOrderedLoadout()
        {
            const string scenarioFingerprint = "scenario-fingerprint";
            var baseline = CellularSimDataFingerprint.CreateRun(
                scenarioFingerprint,
                SpeciesCombatResolutionMode.LegacyFixedDamage,
                SpeciesAttackOpportunityMode.Natural,
                SpeciesExperimentalOptions.None,
                new[] { "first", "second" });

            Assert.That(CellularSimDataFingerprint.CreateRun(
                scenarioFingerprint,
                SpeciesCombatResolutionMode.LegacyFixedDamage,
                SpeciesAttackOpportunityMode.Natural,
                SpeciesExperimentalOptions.None,
                new[] { "first", "second" }), Is.EqualTo(baseline));
            Assert.That(CellularSimDataFingerprint.CreateRun(
                scenarioFingerprint,
                SpeciesCombatResolutionMode.OpposedRoll,
                SpeciesAttackOpportunityMode.Natural,
                SpeciesExperimentalOptions.None,
                new[] { "first", "second" }), Is.Not.EqualTo(baseline));
            Assert.That(CellularSimDataFingerprint.CreateRun(
                scenarioFingerprint,
                SpeciesCombatResolutionMode.LegacyFixedDamage,
                SpeciesAttackOpportunityMode.FixedRateDiagnostic,
                new SpeciesExperimentalOptions(SpeciesExperimentalOptions.BevExperimentalFeaturesId),
                new[] { "first", "second" }), Is.Not.EqualTo(baseline));
            Assert.That(CellularSimDataFingerprint.CreateRun(
                scenarioFingerprint,
                SpeciesCombatResolutionMode.LegacyFixedDamage,
                SpeciesAttackOpportunityMode.Natural,
                SpeciesExperimentalOptions.None,
                new[] { "second", "first" }), Is.Not.EqualTo(baseline));
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
            int metabolism = 1,
            float startingFoodReserve = 0f,
            int maximumEnergy = 0,
            int litterMinimum = 1,
            int litterMaximum = 1,
            int? attackModifier = null,
            int? damageAmount = null)
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
                startingFoodReserve: startingFoodReserve,
                awareness: awareness,
                role: role,
                forageBelowEnergy: forageBelowEnergy,
                maximumEnergy: maximumEnergy,
                litterMinimum: litterMinimum,
                litterMaximum: litterMaximum,
                attackModifier: attackModifier,
                damageAmount: damageAmount);
        }

        static SpeciesSimulationMetrics StepWithReproductionMetrics(
            Grid<SpeciesCell> source,
            SpeciesRules reproductionRules,
            int seed)
        {
            var metrics = new SpeciesSimulationMetrics();
            SpeciesSimulation.Step(
                source,
                new Dictionary<SpeciesId, SpeciesRules>
                {
                    [SpeciesIds.Carnivore] = reproductionRules,
                },
                seed,
                metrics: metrics);
            return metrics;
        }

        static SpeciesRules CreateReproductionRules(
            int reproductionNeighborCount = 0,
            float reproductionChance = 1f,
            int reproductionFoodRequired = 0,
            int maxReproductionGroupSize = 0,
            int litterMinimum = 1,
            int litterMaximum = 1)
        {
            return new SpeciesRules(
                movementSpeed: 0f,
                movementPattern: EmptyPattern,
                attackPattern: EmptyPattern,
                attackAmount: 0,
                blockPattern: EmptyPattern,
                blockAmount: 0,
                dietPattern: EmptyPattern,
                dietTarget: null,
                reproductionPattern: new GridPattern(new[] { Vector2Int.left, Vector2Int.right }),
                reproductionNeighborCount: reproductionNeighborCount,
                reproductionChance: reproductionChance,
                reproductionFoodRequired: reproductionFoodRequired,
                maxReproductionGroupSize: maxReproductionGroupSize,
                metabolism: 0,
                litterMinimum: litterMinimum,
                litterMaximum: litterMaximum);
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
        static readonly GridPattern EmptyPattern = new GridPattern(new Vector2Int[0]);

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
        public void BehaviorMetricsExposeTheTrackedEntitySnapshot()
        {
            var source = new Grid<SpeciesCell>(2, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Herbivore, energy: 6));
            source.SetCell(1, 0, SpeciesCell.Grass(8f));
            var next = source.Copy();
            var metrics = new SpeciesSimulationMetrics();

            SpeciesBehaviorSystem.Update(
                source,
                next,
                SpeciesRuleDefaults.Create(),
                new System.Random(7),
                metrics);

            Assert.That(metrics.TryGetTrackedBehavior(SpeciesIds.Herbivore, out var tracked), Is.True);
            Assert.That(tracked.EntityId, Is.EqualTo(source.GetCell(0, 0).EntityId));
            Assert.That(tracked.Species, Is.EqualTo(SpeciesIds.Herbivore));
            Assert.That(tracked.X, Is.EqualTo(0));
            Assert.That(tracked.Y, Is.EqualTo(0));
            Assert.That(tracked.State, Is.EqualTo(SpeciesBehaviorState.Eating));
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
        public void DeathTelemetryCapturesStarvationCauseAndEntityContext()
        {
            var source = new Grid<SpeciesCell>(1, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Herbivore, energy: 1, age: 4));
            var metrics = new SpeciesSimulationMetrics();

            SpeciesSimulation.Step(
                source,
                SpeciesRuleDefaults.Create(),
                seed: 11,
                metrics: metrics);

            Assert.That(metrics.DeathEvents, Has.Count.EqualTo(1));
            var death = metrics.DeathEvents[0];
            Assert.That(death.Species, Is.EqualTo(SpeciesIds.Herbivore));
            Assert.That(death.EntityId, Is.EqualTo(source.GetCell(0, 0).EntityId));
            Assert.That(death.Age, Is.EqualTo(5));
            Assert.That(death.X, Is.EqualTo(0));
            Assert.That(death.Y, Is.EqualTo(0));
            Assert.That(death.Cause, Is.EqualTo(SpeciesDeathCause.Starvation));
            Assert.That(death.IsCreature, Is.True);
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
        public void BehaviorSystemChoosesThreatenedWhenApproachingThreatIsUnsafe()
        {
            var rules = SpeciesRuleDefaults.Create();
            var source = new Grid<SpeciesCell>(3, 1);
            var hare = new SpeciesCell(SpeciesIds.Herbivore, energy: 6);
            var carnivore = new SpeciesCell(SpeciesIds.Carnivore, energy: 6);
            source.SetCell(0, 0, hare);
            source.SetCell(1, 0, carnivore);
            var previous = new Grid<SpeciesCell>(3, 1);
            previous.SetCell(0, 0, hare.WithEntity(
                hare.SpeciesId,
                hare.Health,
                hare.Energy,
                hare.Age,
                hare.FoodEaten,
                hare.FoodReserve,
                entityId: hare.EntityId));
            previous.SetCell(2, 0, carnivore.WithEntity(
                carnivore.SpeciesId,
                carnivore.Health,
                carnivore.Energy,
                carnivore.Age,
                carnivore.FoodEaten,
                carnivore.FoodReserve,
                entityId: carnivore.EntityId));
            var next = source.Copy();

            SpeciesBehaviorSystem.Update(
                source,
                next,
                rules,
                new System.Random(6),
                previousSource: previous);

            Assert.That(next.GetCell(0, 0).BehaviorState, Is.EqualTo(SpeciesBehaviorState.Threatened));
        }

        [Test]
        public void BehaviorSystemDoesNotFeelStationaryThreat()
        {
            var rules = SpeciesRuleDefaults.Create();
            var source = new Grid<SpeciesCell>(3, 1);
            var hare = new SpeciesCell(SpeciesIds.Herbivore, energy: 6);
            var carnivore = new SpeciesCell(SpeciesIds.Carnivore, energy: 6);
            source.SetCell(0, 0, hare);
            source.SetCell(1, 0, carnivore);
            var previous = source.Copy();
            var next = source.Copy();

            SpeciesBehaviorSystem.Update(
                source,
                next,
                rules,
                new System.Random(7),
                previousSource: previous);

            Assert.That(next.GetCell(0, 0).BehaviorState, Is.EqualTo(SpeciesBehaviorState.Wandering));
        }

        [Test]
        public void BehaviorSystemDoesNotFeelApproachingThreatOutsideAttackRange()
        {
            var rules = SpeciesRuleDefaults.Create();
            var source = new Grid<SpeciesCell>(4, 1);
            var hare = new SpeciesCell(SpeciesIds.Herbivore, energy: 6);
            var carnivore = new SpeciesCell(SpeciesIds.Carnivore, energy: 6);
            source.SetCell(0, 0, hare);
            source.SetCell(2, 0, carnivore);
            var previous = new Grid<SpeciesCell>(4, 1);
            previous.SetCell(0, 0, hare.WithEntity(
                hare.SpeciesId,
                hare.Health,
                hare.Energy,
                hare.Age,
                hare.FoodEaten,
                hare.FoodReserve,
                entityId: hare.EntityId));
            previous.SetCell(3, 0, carnivore.WithEntity(
                carnivore.SpeciesId,
                carnivore.Health,
                carnivore.Energy,
                carnivore.Age,
                carnivore.FoodEaten,
                carnivore.FoodReserve,
                entityId: carnivore.EntityId));
            var next = source.Copy();

            SpeciesBehaviorSystem.Update(
                source,
                next,
                rules,
                new System.Random(8),
                previousSource: previous);

            Assert.That(next.GetCell(0, 0).BehaviorState, Is.EqualTo(SpeciesBehaviorState.Wandering));
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
                    role: SpeciesRole.Herbivore,
                    metabolism: 0),
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

        [Test]
        public void ExperimentalHerbivoreStatLineCalculatesRatesFromReconciledCounts()
        {
            var statLine = new SpeciesHerbivoreStatLine(
                SpeciesIds.Herbivore,
                startingPopulation: 10,
                predatorActiveHerbivoreSteps: 100,
                encounteredHerbivoreSteps: 20,
                encounters: 20,
                preyed: 2,
                starved: 1,
                mating: 4,
                births: 3,
                crowding: 1,
                finalPopulation: 9);

            Assert.That(statLine.InversePreyedAverage, Is.EqualTo(0.9f).Within(0.0001f));
            Assert.That(statLine.InversePreyedAverageStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.Valid));
            Assert.That(statLine.InverseEncounterAverage, Is.EqualTo(0.8f).Within(0.0001f));
            Assert.That(statLine.InverseEncounterAverageStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.Valid));
            Assert.That(statLine.PredationAverage, Is.EqualTo(0.85f).Within(0.0001f));
            Assert.That(statLine.PredationAverageStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.Valid));
            Assert.That(statLine.InverseStarvedAverage, Is.EqualTo(10f / 11f).Within(0.0001f));
            Assert.That(statLine.InverseStarvedAverageStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.Valid));
            Assert.That(statLine.InverseCrowdingAverage, Is.EqualTo(0.9f).Within(0.0001f));
            Assert.That(statLine.InverseCrowdingAverageStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.Valid));
            Assert.That(statLine.BirthAverage, Is.EqualTo(0.75f).Within(0.0001f));
            Assert.That(statLine.BirthAverageStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.Valid));
            Assert.That(statLine.ReplicationFitnessScore, Is.EqualTo(-0.75f).Within(0.0001f));
            Assert.That(statLine.ReplicationFitnessScoreStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.Valid));
            Assert.That(statLine.ActualPreyScore, Is.EqualTo(-0.0909091f).Within(0.0001f));
            Assert.That(statLine.ActualPreyScoreStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.Valid));
            Assert.That(statLine.ExpectedFinalPopulation, Is.EqualTo(9));
            Assert.That(statLine.PopulationReconciled, Is.True);
        }

        [Test]
        public void ExperimentalHerbivoreStatLineTreatsZeroBirthAverageAsZeroRfsMultiplier()
        {
            var statLine = new SpeciesHerbivoreStatLine(
                SpeciesIds.Herbivore,
                startingPopulation: 10,
                predatorActiveHerbivoreSteps: 1,
                encounteredHerbivoreSteps: 1,
                encounters: 1,
                preyed: 1,
                starved: 0,
                mating: 4,
                births: 0,
                crowding: 0,
                finalPopulation: 9);

            Assert.That(statLine.BirthAverage, Is.EqualTo(0f));
            Assert.That(statLine.BirthAverageStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.Valid));
            Assert.That(statLine.ReplicationFitnessScore, Is.EqualTo(0f));
            Assert.That(statLine.ReplicationFitnessScoreStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.Valid));
        }

        [Test]
        public void ExperimentalHerbivoreStatLineUsesNeutralNADenominatorsForAps()
        {
            var statLine = new SpeciesHerbivoreStatLine(
                SpeciesIds.Herbivore,
                startingPopulation: 1,
                predatorActiveHerbivoreSteps: 1,
                encounteredHerbivoreSteps: 1,
                encounters: 1,
                preyed: 1,
                starved: 0,
                mating: 0,
                births: 0,
                crowding: 0,
                finalPopulation: 0);

            Assert.That(statLine.InversePreyedAverageStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.Valid));
            Assert.That(statLine.InverseEncounterAverageStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.Valid));
            Assert.That(statLine.PredationAverageStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.Valid));
            Assert.That(statLine.InverseStarvedAverageStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.NotApplicable));
            Assert.That(statLine.InverseCrowdingAverageStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.NotApplicable));
            Assert.That(statLine.BirthAverageStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.NotApplicable));
            Assert.That(statLine.ReplicationFitnessScoreStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.NotApplicable));
            Assert.That(statLine.ActualPreyScore, Is.EqualTo(0f));
            Assert.That(statLine.ActualPreyScoreStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.Valid));
        }

        [Test]
        public void ExperimentalHerbivoreStatLineUsesApplicableEncounterAvoidanceInAps()
        {
            var avoidedAllEncounters = new SpeciesHerbivoreStatLine(
                SpeciesIds.Herbivore,
                startingPopulation: 1,
                predatorActiveHerbivoreSteps: 10,
                encounteredHerbivoreSteps: 0,
                encounters: 0,
                preyed: 0,
                starved: 0,
                mating: 0,
                births: 0,
                crowding: 0,
                finalPopulation: 1);

            Assert.That(avoidedAllEncounters.InversePreyedAverageStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.NotApplicable));
            Assert.That(avoidedAllEncounters.InverseEncounterAverage, Is.EqualTo(1f));
            Assert.That(avoidedAllEncounters.PredationAverage, Is.EqualTo(1f));
            Assert.That(avoidedAllEncounters.ActualPreyScore, Is.EqualTo(1f));

            var noPredatorActivity = new SpeciesHerbivoreStatLine(
                SpeciesIds.Herbivore,
                startingPopulation: 1,
                predatorActiveHerbivoreSteps: 0,
                encounteredHerbivoreSteps: 0,
                encounters: 0,
                preyed: 0,
                starved: 0,
                mating: 0,
                births: 0,
                crowding: 0,
                finalPopulation: 1);

            Assert.That(noPredatorActivity.PredationAverageStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.NotApplicable));
            Assert.That(noPredatorActivity.ActualPreyScore, Is.EqualTo(0f));

            var overcountedEncounterSteps = new SpeciesHerbivoreStatLine(
                SpeciesIds.Herbivore,
                startingPopulation: 1,
                predatorActiveHerbivoreSteps: 1,
                encounteredHerbivoreSteps: 2,
                encounters: 2,
                preyed: 0,
                starved: 0,
                mating: 0,
                births: 0,
                crowding: 0,
                finalPopulation: 1);

            Assert.That(overcountedEncounterSteps.InverseEncounterAverageStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.Invalid));
            Assert.That(overcountedEncounterSteps.PredationAverageStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.Invalid));
            Assert.That(overcountedEncounterSteps.ActualPreyScoreStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.Invalid));
        }

        [Test]
        public void ExperimentalHerbivoreStatLineMarksZeroDenominatorWithDeathsInvalid()
        {
            var statLine = new SpeciesHerbivoreStatLine(
                SpeciesIds.Herbivore,
                startingPopulation: 1,
                predatorActiveHerbivoreSteps: 0,
                encounteredHerbivoreSteps: 0,
                encounters: 0,
                preyed: 1,
                starved: 0,
                mating: 0,
                births: 0,
                crowding: 0,
                finalPopulation: 0);

            Assert.That(statLine.InversePreyedAverageStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.Invalid));
            Assert.That(statLine.ActualPreyScoreStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.Invalid));

            var starvationWithoutExposure = new SpeciesHerbivoreStatLine(
                SpeciesIds.Herbivore,
                startingPopulation: 0,
                predatorActiveHerbivoreSteps: 0,
                encounteredHerbivoreSteps: 0,
                encounters: 0,
                preyed: 0,
                starved: 1,
                mating: 0,
                births: 0,
                crowding: 0,
                finalPopulation: 0);

            Assert.That(
                starvationWithoutExposure.InverseStarvedAverageStatus,
                Is.EqualTo(SpeciesHerbivoreMetricStatus.Invalid));
            Assert.That(
                starvationWithoutExposure.ActualPreyScoreStatus,
                Is.EqualTo(SpeciesHerbivoreMetricStatus.Invalid));
        }

        [Test]
        public void ExperimentalHerbivoreStatLineMarksFpoReconciliationFailureInvalid()
        {
            var statLine = new SpeciesHerbivoreStatLine(
                SpeciesIds.Herbivore,
                startingPopulation: 10,
                predatorActiveHerbivoreSteps: 10,
                encounteredHerbivoreSteps: 0,
                encounters: 1,
                preyed: 0,
                starved: 0,
                mating: 1,
                births: 0,
                crowding: 0,
                finalPopulation: 9);

            Assert.That(statLine.PopulationReconciled, Is.False);
            Assert.That(statLine.ActualPreyScoreStatus, Is.EqualTo(SpeciesHerbivoreMetricStatus.Invalid));
        }

        [Test]
        public void ExperimentalHerbivoreStatLineFlagsUncountedDeathsWithoutChangingFPO()
        {
            var statLine = new SpeciesHerbivoreStatLine(
                SpeciesIds.Herbivore,
                startingPopulation: 10,
                predatorActiveHerbivoreSteps: 10,
                encounteredHerbivoreSteps: 1,
                encounters: 1,
                preyed: 1,
                starved: 1,
                mating: 1,
                births: 2,
                crowding: 1,
                finalPopulation: 8);

            Assert.That(statLine.ExpectedFinalPopulation, Is.EqualTo(9));
            Assert.That(statLine.FinalPopulation, Is.EqualTo(8));
            Assert.That(statLine.PopulationReconciled, Is.False);
        }

        [Test]
        public void CrowdingRemovesLowEnergyHaresWhenTheGroupExceedsItsLimit()
        {
            var neighborhood = new GridPattern(new[]
            {
                new Vector2Int(-1, -1),
                new Vector2Int(0, -1),
                new Vector2Int(1, -1),
                new Vector2Int(-1, 0),
                new Vector2Int(1, 0),
                new Vector2Int(-1, 1),
                new Vector2Int(0, 1),
                new Vector2Int(1, 1),
            });
            var rules = new Dictionary<SpeciesId, SpeciesRules>
            {
                [SpeciesIds.Herbivore] = new SpeciesRules(
                    movementSpeed: 0f,
                    movementPattern: EmptyPattern,
                    attackPattern: EmptyPattern,
                    attackAmount: 0,
                    blockPattern: EmptyPattern,
                    blockAmount: 0,
                    dietPattern: EmptyPattern,
                    dietTarget: null,
                    reproductionPattern: neighborhood,
                    reproductionNeighborCount: 0,
                    reproductionChance: 0f,
                    maxReproductionGroupSize: 3,
                    startingEnergy: 3,
                    crowdingEnergyPenalty: 2,
                    metabolism: 1,
                    role: SpeciesRole.Herbivore),
            };
            var source = new Grid<SpeciesCell>(2, 2);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Herbivore, energy: 3));
            source.SetCell(1, 0, new SpeciesCell(SpeciesIds.Herbivore, energy: 3));
            source.SetCell(0, 1, new SpeciesCell(SpeciesIds.Herbivore, energy: 3));
            source.SetCell(1, 1, new SpeciesCell(SpeciesIds.Herbivore, energy: 3));
            var metrics = new SpeciesSimulationMetrics();

            var next = SpeciesSimulation.Step(source, rules, seed: 7, metrics: metrics);

            Assert.That(metrics.GetActivity(SpeciesIds.Herbivore).CrowdingDeaths, Is.EqualTo(1));
            var survivingHares = 0;
            for (var y = 0; y < next.Height; y++)
            {
                for (var x = 0; x < next.Width; x++)
                {
                    if (next.GetCell(x, y).IsCreature)
                    {
                        survivingHares++;
                    }
                }
            }

            Assert.That(survivingHares, Is.EqualTo(3));

            var tolerantRules = new Dictionary<SpeciesId, SpeciesRules>
            {
                [SpeciesIds.Herbivore] = SpeciesUpgradeCatalog.Create(
                    SpeciesUpgradeCatalog.CrowdingToleranceId).Apply(rules[SpeciesIds.Herbivore]),
            };
            var tolerantMetrics = new SpeciesSimulationMetrics();
            var tolerantNext = SpeciesSimulation.Step(source, tolerantRules, seed: 7, metrics: tolerantMetrics);

            Assert.That(tolerantMetrics.GetActivity(SpeciesIds.Herbivore).CrowdingDeaths, Is.Zero);
            var tolerantSurvivors = 0;
            for (var y = 0; y < tolerantNext.Height; y++)
            {
                for (var x = 0; x < tolerantNext.Width; x++)
                {
                    tolerantSurvivors += tolerantNext.GetCell(x, y).IsCreature ? 1 : 0;
                }
            }
            Assert.That(tolerantSurvivors, Is.EqualTo(4));
        }

        [Test]
        public void ExperimentalFeatureRecordsCarnivoreToHerbivoreEncounterOnTargetSpecies()
        {
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
                    role: SpeciesRole.Carnivore,
                    awareness: new SpeciesAwarenessRules(visionRange: 1),
                    forageBelowEnergy: 1),
                [SpeciesIds.Herbivore] = new SpeciesRules(
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
                    metabolism: 0,
                    role: SpeciesRole.Herbivore),
            };
            var source = new Grid<SpeciesCell>(2, 1);
            source.SetCell(0, 0, new SpeciesCell(SpeciesIds.Carnivore, energy: 1));
            source.SetCell(1, 0, new SpeciesCell(SpeciesIds.Herbivore, health: 1));
            var metrics = new SpeciesSimulationMetrics();

            SpeciesSimulation.Step(
                source,
                rules,
                seed: 42,
                metrics: metrics,
                experimentalOptions: new SpeciesExperimentalOptions(
                    SpeciesExperimentalOptions.BevExperimentalFeaturesId));

            Assert.That(metrics.GetHerbivoreEncounters(SpeciesIds.Herbivore), Is.EqualTo(1));
            Assert.That(metrics.GetHerbivorePreyed(SpeciesIds.Herbivore), Is.EqualTo(1));
            Assert.That(metrics.GetPredatorActiveHerbivoreSteps(SpeciesIds.Herbivore), Is.EqualTo(1));
            Assert.That(metrics.GetEncounteredHerbivoreSteps(SpeciesIds.Herbivore), Is.EqualTo(1));
        }
    }
}
