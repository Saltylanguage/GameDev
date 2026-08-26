using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace SaltyGame.PlayModeTests
{
    public sealed class LabPlayModeTests
    {
        const string LabScene = "Lab";
        const string ViewModelType = "SaltyGame.VM_Lab";

        [UnityTest]
        public IEnumerator LabSceneHasOneComposedRoot()
        {
            yield return LoadLab();

            var root = FindRoot();
            Assert.That(root, Is.Not.Null);
            Assert.That(root.GetComponent("NoesisView"), Is.Not.Null);
            Assert.That(root.GetComponent(ViewModelType), Is.Not.Null);
            Assert.That(root.GetComponent("SaltyGame.LabNoesisHost"), Is.Not.Null);
            Assert.That(root.GetComponent("SaltyGame.VM_Overview"), Is.Not.Null);
            Assert.That(root.GetComponent("SaltyGame.VM_Research"), Is.Not.Null);
            Assert.That(root.GetComponent("SaltyGame.VM_SpeciesArchive"), Is.Not.Null);
            Assert.That(root.GetComponent("SaltyGame.VM_ExpeditionSetup"), Is.Not.Null);
            Assert.That(root.GetComponent("SaltyGame.VM_Settings"), Is.Not.Null);
        }

        [UnityTest]
        public IEnumerator LabFeatureNavigationUpdatesTheUiState()
        {
            yield return LoadLab();

            var viewModel = FindViewModel();
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(GetVisibility(viewModel, "OverviewVisibility"), Is.EqualTo("Visible"));

            var features = (IEnumerable)GetProperty(viewModel, "Features");
            var openedResearch = false;
            foreach (var feature in features)
            {
                if ((string)GetProperty(feature, "FeatureId") != "Research")
                {
                    continue;
                }

                Execute(GetProperty(feature, "OpenCommand"));
                openedResearch = true;
                break;
            }

            Assert.That(openedResearch, Is.True);
            Assert.That(GetVisibility(viewModel, "ResearchVisibility"), Is.EqualTo("Visible"));
            Assert.That(GetVisibility(viewModel, "OverviewVisibility"), Is.EqualTo("Collapsed"));

            Execute(GetProperty(viewModel, "BackToOverviewCommand"));
            Assert.That(GetVisibility(viewModel, "OverviewVisibility"), Is.EqualTo("Visible"));
        }

        [UnityTest]
        public IEnumerator ExpeditionLaunchStaysDisabledWithoutProfile()
        {
            PlayerPrefs.DeleteKey(Helper_ProfileSession.StoreKey);
            PlayerPrefs.Save();
            yield return LoadLab();

            var viewModel = FindViewModel();
            var launch = GetProperty(viewModel, "LaunchExpeditionCommand");
            Assert.That(CanExecute(launch), Is.False);
            Assert.That((bool)GetProperty(viewModel, "CanLaunchExpedition"), Is.False);
        }

        [UnityTest]
        public IEnumerator ExpeditionLaunchLoadsSimulationWithImmutableRequest()
        {
            PlayerPrefs.DeleteKey(Helper_ProfileSession.StoreKey);
            PlayerPrefs.Save();
            yield return LoadLab();

            var root = FindRoot();
            var profile = root.GetComponent<Helper_ProfileSession>();
            profile.CreateInitialProfile("T6 Test Profile");

            var viewModel = FindViewModel();
            var features = (IEnumerable)GetProperty(viewModel, "Features");
            foreach (var feature in features)
            {
                if ((string)GetProperty(feature, "FeatureId") == "ExpeditionSetup")
                {
                    Execute(GetProperty(feature, "OpenCommand"));
                    break;
                }
            }

            var launch = GetProperty(viewModel, "LaunchExpeditionCommand");
            Assert.That(CanExecute(launch), Is.True);
            Execute(launch);
            yield return null;
            yield return null;

            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo("CellularAutomataPrototype"));
            Component preview = null;
            var previewObjects = Object.FindObjectsByType<Component>(FindObjectsSortMode.None);
            foreach (var candidate in previewObjects)
            {
                if (candidate.GetType().FullName == "SaltyGame.SpeciesSimulationPreview")
                {
                    preview = candidate;
                    break;
                }
            }
            Assert.That(preview, Is.Not.Null);
            Assert.That(GetProperty(preview, "SelectedScenario"), Is.Not.Null);
            Assert.That(((UnityEngine.Object)GetProperty(preview, "SelectedScenario")).name, Is.EqualTo("ForestEdge"));
            Assert.That(GetProperty(GetProperty(preview, "PlayerSpecies"), "Value"), Is.EqualTo("hare"));
            Assert.That((int)GetProperty(preview, "BaseSeed"), Is.EqualTo(10100));
            Assert.That((bool)GetProperty(preview, "RandomizeSeedOnStart"), Is.False);

            PlayerPrefs.DeleteKey(Helper_ProfileSession.StoreKey);
            PlayerPrefs.Save();
        }

        static IEnumerator LoadLab()
        {
            yield return SceneManager.LoadSceneAsync(LabScene);
            yield return null;
            yield return null;
        }

        static GameObject FindRoot()
        {
            var roots = SceneManager.GetActiveScene().GetRootGameObjects();
            return roots.Length == 0 ? null : roots[0];
        }

        static Component FindViewModel()
        {
            var root = FindRoot();
            return root == null ? null : root.GetComponent(ViewModelType);
        }

        static object GetProperty(object target, string propertyName)
        {
            var property = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            Assert.That(property, Is.Not.Null, $"Expected property '{propertyName}'.");
            var value = property.GetValue(target);
            Assert.That(value, Is.Not.Null, $"Expected non-null property '{propertyName}'.");
            return value;
        }

        static string GetVisibility(Component viewModel, string propertyName)
        {
            return GetProperty(viewModel, propertyName).ToString();
        }

        static bool CanExecute(object command)
        {
            return (bool)command.GetType().GetMethod("CanExecute")?.Invoke(command, new object[] { null });
        }

        static void Execute(object command)
        {
            Assert.That(CanExecute(command), Is.True);
            command.GetType().GetMethod("Execute")?.Invoke(command, new object[] { null });
        }
    }
}
