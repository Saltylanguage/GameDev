using System.Collections;
using NUnit.Framework;
using SaltyGame;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace SaltyGame.PlayModeTests
{
    public sealed class IslandSurvivorPlayModeTests
    {
        [UnityTest]
        public IEnumerator IslandSurvivorSceneStartsTheRuntimeAndBuildsTargets()
        {
            yield return SceneManager.LoadSceneAsync("IslandSurvivorPrototype");
            yield return null;

            var root = GameObject.Find("Island Survivor Runtime");
            Assert.That(root, Is.Not.Null);

            var runtime = root.GetComponent<GameRuntime>();
            Assert.That(runtime, Is.Not.Null);
            Assert.That(runtime.State, Is.EqualTo(GameState.Playing));
            Assert.That(runtime.World.IsBuilt, Is.True);
            Assert.That(runtime.World.PlayerTransform, Is.Not.Null);
            Assert.That(runtime.World.Targets.Count, Is.EqualTo(8));
        }

        [UnityTest]
        public IEnumerator IslandSurvivorSceneSupportsTheEatAndRestLoop()
        {
            yield return SceneManager.LoadSceneAsync("IslandSurvivorPrototype");
            yield return null;

            var runtime = GameObject.Find("Island Survivor Runtime").GetComponent<GameRuntime>();
            runtime.Inventory.Add(ResourceId.Wood, CampState.CampfireWoodCost);
            runtime.Inventory.Add(ResourceId.Stone, CampState.CampfireStoneCost);
            var campfire = (CampfireInteractable)runtime.World.Targets[3];
            Assert.That(runtime.Activities.Start(campfire), Is.True);
            yield return null;

            runtime.Inventory.Add(ResourceId.Berries, 2);
            Assert.That(runtime.Activities.Start(campfire), Is.True);
            yield return new WaitForSeconds(1.1f);

            Assert.That(runtime.Inventory.Get(ResourceId.CookedMeal), Is.EqualTo(1));
            Assert.That(campfire.TryEat(runtime.Survival, out _), Is.True);
            Assert.That(runtime.TrySleepAtCamp(), Is.True);
            Assert.That(runtime.Clock.Day, Is.EqualTo(2));
            Assert.That(runtime.Clock.TimeOfDay, Is.EqualTo(TimeOfDay.Morning));
        }

        [UnityTest]
        public IEnumerator IslandSurvivorSceneSupportsPreparingForAndWeatheringTheStorm()
        {
            yield return SceneManager.LoadSceneAsync("IslandSurvivorPrototype");
            yield return null;

            var runtime = GameObject.Find("Island Survivor Runtime").GetComponent<GameRuntime>();
            runtime.Inventory.Add(ResourceId.Wood, CampState.CampfireWoodCost + CampState.ShelterWoodCost);
            runtime.Inventory.Add(ResourceId.Stone, CampState.CampfireStoneCost);
            Assert.That(runtime.Activities.Start(runtime.World.Targets[3]), Is.True);
            yield return null;
            var shelterMarker = runtime.World.transform.Find("Shelter Site/Shelter Marker");
            Assert.That(shelterMarker, Is.Not.Null);
            Assert.That(shelterMarker.gameObject.activeSelf, Is.False);
            Assert.That(runtime.TrySleepAtCamp(), Is.True);

            runtime.Clock.AdvanceActivity();
            runtime.Clock.AdvanceActivity();
            runtime.World.SetShelterMarkerVisible(runtime.Clock.Day == 2 && runtime.Clock.TimeOfDay == TimeOfDay.Night);
            Assert.That(shelterMarker.gameObject.activeSelf, Is.True);

            Assert.That(runtime.Activities.Start(runtime.World.Targets[4]), Is.True);
            yield return null;
            Assert.That(runtime.Camp.ShelterBuilt, Is.True);
            Assert.That(shelterMarker.gameObject.activeSelf, Is.False);
            Assert.That(runtime.TrySleepAtCamp(), Is.True);
            Assert.That(runtime.Storm.IsResolved, Is.True);
        }
    }
}
