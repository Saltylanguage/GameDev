using NUnit.Framework;
using SaltyGame;
using UnityEngine;

namespace SaltyGame.Tests
{
    [TestFixture]
    public sealed class GameRuntimeTests
    {
        GameObject runtimeRoot;

        [TearDown]
        public void TearDown()
        {
            if (runtimeRoot != null)
                Object.DestroyImmediate(runtimeRoot);
        }

        [Test]
        public void GameRuntimeBuildsTheFirstVerticalSliceAndResetsTargets()
        {
            runtimeRoot = new GameObject("Game Runtime Test");
            var runtime = runtimeRoot.AddComponent<GameRuntime>();
            runtime.Initialize();

            Assert.That(runtime.State, Is.EqualTo(GameState.Playing));
            Assert.That(runtime.World.IsBuilt, Is.True);
            Assert.That(runtime.World.transform.parent, Is.EqualTo(runtimeRoot.transform));
            Assert.That(runtime.World.PlayerTransform, Is.Not.Null);
            Assert.That(runtime.World.Targets.Count, Is.EqualTo(7));
            Assert.That(Camera.main.clearFlags, Is.EqualTo(CameraClearFlags.SolidColor));

            var worldChildCount = runtime.World.transform.childCount;
            runtime.World.Build(null);
            Assert.That(runtime.World.transform.childCount, Is.EqualTo(worldChildCount));
            Assert.That(runtime.Inventory.Get(ResourceId.Wood), Is.EqualTo(0));
            Assert.That(runtime.Activities.IsActive, Is.False);

            var target = runtime.World.Targets[0];
            target.ApplyActivityResult(new ActivityResult(true, ResourceId.Wood, 3));
            Assert.That(target.CanInteract, Is.False);
            runtime.World.ResetTargetsForNewDay();
            Assert.That(target.CanInteract, Is.True);
        }

        [Test]
        public void CampfireObjectiveConsumesResourcesAndPersists()
        {
            runtimeRoot = new GameObject("Game Runtime Test");
            var runtime = runtimeRoot.AddComponent<GameRuntime>();
            runtime.Initialize();

            runtime.Inventory.Add(ResourceId.Wood, CampState.CampfireWoodCost);
            runtime.Inventory.Add(ResourceId.Stone, CampState.CampfireStoneCost);
            var campfire = runtime.World.Targets[3];

            Assert.That(campfire.DisplayName, Is.EqualTo("Campfire Site"));
            Assert.That(campfire.CanInteract, Is.True);
            Assert.That(runtime.Activities.Start(campfire), Is.True);
            Assert.That(runtime.Activities.Tick(0.5f), Is.True);
            Assert.That(runtime.Camp.CampfireBuilt, Is.True);
            Assert.That(runtime.Inventory.Get(ResourceId.Wood), Is.EqualTo(0));
            Assert.That(runtime.Inventory.Get(ResourceId.Stone), Is.EqualTo(0));
            Assert.That(campfire.CanInteract, Is.True);
        }

        [Test]
        public void CampfireCooksBerriesAndEatingUsesTheBetterMealValue()
        {
            runtimeRoot = new GameObject("Game Runtime Test");
            var runtime = runtimeRoot.AddComponent<GameRuntime>();
            runtime.Initialize();

            runtime.Inventory.Add(ResourceId.Wood, CampState.CampfireWoodCost);
            runtime.Inventory.Add(ResourceId.Stone, CampState.CampfireStoneCost);
            var campfire = (CampfireInteractable)runtime.World.Targets[3];
            runtime.Activities.Start(campfire);
            runtime.Activities.Tick(0f);

            runtime.Inventory.Add(ResourceId.Berries, 2);
            Assert.That(runtime.Activities.Start(campfire), Is.True);
            Assert.That(runtime.Activities.Tick(1f), Is.True);
            Assert.That(runtime.Inventory.Get(ResourceId.Berries), Is.EqualTo(0));
            Assert.That(runtime.Inventory.Get(ResourceId.CookedMeal), Is.EqualTo(1));

            var hungerBeforeEating = runtime.Survival.Hunger;
            Assert.That(campfire.TryEat(runtime.Survival, out _), Is.True);
            Assert.That(runtime.Survival.Hunger, Is.LessThan(hungerBeforeEating));
        }

        [Test]
        public void SleepingAtTheBuiltCampfireStartsTheNextMorningAndResetsTargets()
        {
            runtimeRoot = new GameObject("Game Runtime Test");
            var runtime = runtimeRoot.AddComponent<GameRuntime>();
            runtime.Initialize();

            runtime.Inventory.Add(ResourceId.Wood, CampState.CampfireWoodCost);
            runtime.Inventory.Add(ResourceId.Stone, CampState.CampfireStoneCost);
            var campfire = runtime.World.Targets[3];
            runtime.Activities.Start(campfire);
            runtime.Activities.Tick(0f);
            runtime.World.Targets[0].ApplyActivityResult(new ActivityResult(true, ResourceId.Wood, 1));

            Assert.That(runtime.World.Targets[0].CanInteract, Is.False);
            Assert.That(runtime.TrySleepAtCamp(), Is.True);
            Assert.That(runtime.Clock.Day, Is.EqualTo(2));
            Assert.That(runtime.Clock.TimeOfDay, Is.EqualTo(TimeOfDay.Morning));
            Assert.That(runtime.World.Targets[0].CanInteract, Is.True);
        }

        [Test]
        public void ShelterBuiltBeforeDayTwoStormPreventsExposureAndResolvesTheScenario()
        {
            runtimeRoot = new GameObject("Game Runtime Test");
            var runtime = runtimeRoot.AddComponent<GameRuntime>();
            runtime.Initialize();

            runtime.Inventory.Add(ResourceId.Wood, CampState.CampfireWoodCost + CampState.ShelterWoodCost);
            runtime.Inventory.Add(ResourceId.Stone, CampState.CampfireStoneCost);
            runtime.Activities.Start(runtime.World.Targets[3]);
            runtime.Activities.Tick(0f);
            Assert.That(runtime.TrySleepAtCamp(), Is.True);

            var shelter = runtime.World.Targets[4];
            Assert.That(runtime.Activities.Start(shelter), Is.True);
            runtime.Activities.Tick(0f);
            var hungerBeforeStorm = runtime.Survival.Hunger;
            var energyBeforeStorm = runtime.Survival.Energy;

            Assert.That(runtime.Camp.ShelterBuilt, Is.True);
            Assert.That(runtime.TrySleepAtCamp(), Is.True);
            Assert.That(runtime.Storm.IsResolved, Is.True);
            Assert.That(runtime.Survival.Hunger, Is.LessThanOrEqualTo(hungerBeforeStorm + 12));
            Assert.That(runtime.Survival.Energy, Is.GreaterThanOrEqualTo(energyBeforeStorm));
        }

        [Test]
        public void OpenCampStormHasARecoverableSurvivalCost()
        {
            runtimeRoot = new GameObject("Game Runtime Test");
            var runtime = runtimeRoot.AddComponent<GameRuntime>();
            runtime.Initialize();

            runtime.Inventory.Add(ResourceId.Wood, CampState.CampfireWoodCost);
            runtime.Inventory.Add(ResourceId.Stone, CampState.CampfireStoneCost);
            runtime.Activities.Start(runtime.World.Targets[3]);
            runtime.Activities.Tick(0f);
            runtime.TrySleepAtCamp();
            var hungerBeforeStorm = runtime.Survival.Hunger;
            var energyBeforeStorm = runtime.Survival.Energy;

            Assert.That(runtime.TrySleepAtCamp(), Is.True);
            Assert.That(runtime.Storm.IsResolved, Is.True);
            Assert.That(runtime.Survival.Hunger, Is.EqualTo(hungerBeforeStorm + 32));
            Assert.That(runtime.Survival.Energy, Is.EqualTo(energyBeforeStorm - 25));
        }

        [Test]
        public void CrudeAxeCostsResourcesAndImprovesTreeChopping()
        {
            runtimeRoot = new GameObject("Game Runtime Test");
            var runtime = runtimeRoot.AddComponent<GameRuntime>();
            runtime.Initialize();

            runtime.Inventory.Add(ResourceId.Wood, CampState.CampfireWoodCost + CampState.CrudeAxeWoodCost);
            runtime.Inventory.Add(ResourceId.Stone, CampState.CampfireStoneCost + CampState.CrudeAxeStoneCost);
            var campfire = (CampfireInteractable)runtime.World.Targets[3];
            Assert.That(runtime.Activities.Start(campfire), Is.True);
            runtime.Activities.Tick(0f);

            Assert.That(campfire.TryCraftCrudeAxe(), Is.True);
            Assert.That(runtime.Camp.CrudeAxeCrafted, Is.True);
            Assert.That(runtime.Inventory.Get(ResourceId.Wood), Is.EqualTo(0));
            Assert.That(runtime.Inventory.Get(ResourceId.Stone), Is.EqualTo(0));

            var tree = (TreeInteractable)runtime.World.Targets[0];
            var axeChop = (WoodChoppingActivity)tree.CreateActivity();
            axeChop.Submit(0.5f);
            axeChop.Submit(0.5f);
            Assert.That(axeChop.IsComplete, Is.True);
            Assert.That(axeChop.Result.Amount, Is.EqualTo(8));
        }

        [Test]
        public void JungleEdgeUsesTheAxeAndSurvivorHelpsOncePerDay()
        {
            runtimeRoot = new GameObject("Game Runtime Test");
            var runtime = runtimeRoot.AddComponent<GameRuntime>();
            runtime.Initialize();

            var jungle = (JungleEdgeInteractable)runtime.World.Targets[5];
            var blockedByHands = (WoodChoppingActivity)jungle.CreateActivity();
            blockedByHands.Submit(0.5f);
            Assert.That(blockedByHands.IsComplete, Is.False);

            runtime.Inventory.Add(ResourceId.Wood, CampState.CampfireWoodCost + CampState.CrudeAxeWoodCost);
            runtime.Inventory.Add(ResourceId.Stone, CampState.CampfireStoneCost + CampState.CrudeAxeStoneCost);
            var campfire = (CampfireInteractable)runtime.World.Targets[3];
            runtime.Activities.Start(campfire);
            runtime.Activities.Tick(0f);
            Assert.That(campfire.TryCraftCrudeAxe(), Is.True);

            var clearedWithAxe = (WoodChoppingActivity)jungle.CreateActivity();
            clearedWithAxe.Submit(0.5f);
            clearedWithAxe.Submit(0.5f);
            Assert.That(clearedWithAxe.IsComplete, Is.True);
            Assert.That(clearedWithAxe.Result.Amount, Is.EqualTo(8));

            var survivor = (SurvivorInteractable)runtime.World.Targets[6];
            var berriesBefore = runtime.Inventory.Get(ResourceId.Berries);
            var timeBefore = runtime.Clock.TimeOfDay;
            Assert.That(survivor.TrySendScavenging(out _), Is.True);
            Assert.That(runtime.Inventory.Get(ResourceId.Berries), Is.EqualTo(berriesBefore + 1));
            Assert.That(survivor.CanInteract, Is.False);
            Assert.That(runtime.Clock.TimeOfDay, Is.EqualTo(timeBefore));
            survivor.ResetForNewDay();
            Assert.That(survivor.CanInteract, Is.True);
        }
    }
}
