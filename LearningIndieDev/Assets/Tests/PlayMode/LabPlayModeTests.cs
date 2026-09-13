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

        [UnityTest]
        public IEnumerator SimulationWindowCloseIsDisabledDuringRunAndReturnsToLabAfterResults()
        {
            PlayerPrefs.DeleteKey(Helper_ProfileSession.StoreKey);
            PlayerPrefs.Save();
            yield return LoadLab();

            var root = FindRoot();
            var profile = root.GetComponent<Helper_ProfileSession>();
            profile.CreateInitialProfile("Close Policy Test Profile");

            var viewModel = FindViewModel();
            foreach (var feature in (IEnumerable)GetProperty(viewModel, "Features"))
            {
                if ((string)GetProperty(feature, "FeatureId") == "ExpeditionSetup")
                {
                    Execute(GetProperty(feature, "OpenCommand"));
                    break;
                }
            }

            Execute(GetProperty(viewModel, "LaunchExpeditionCommand"));
            yield return null;
            yield return null;

            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo("CellularAutomataPrototype"));
            var runtime = Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>();
            Assert.That(runtime, Is.Not.Null);
            var preview = runtime.SpeciesPreview;
            Assert.That(preview.TryApplyContinuousPhases(true, "2", out var phaseMessage), Is.True, phaseMessage);
            Assert.That(preview.TryApplyGlobalSettingsForTicks(
                "8",
                "8",
                preview.BaseSeed.ToString(),
                preview.MaximumPopulation.ToString(),
                preview.MinimumPopulation.ToString(),
                "4",
                "0.01",
                preview.PlantProbability.ToString(),
                preview.HerbivoreProbability.ToString(),
                preview.CarnivoreProbability.ToString(),
                randomizeSeed: false,
                out var settingsMessage), Is.True, settingsMessage);

            var simulationViewModel = GameObject.Find("Prototype Camera")
                ?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(simulationViewModel, Is.Not.Null);
            var closeCommand = GetProperty(simulationViewModel, "CloseWindowCommand");
            preview.StartSimulation();
            yield return null;

            var timeout = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.Results
                   && Time.realtimeSinceStartup < timeout)
            {
                Assert.That(GetProperty(simulationViewModel, "CanCloseWindow"), Is.EqualTo(false));
                Assert.That(CanExecute(closeCommand), Is.False);
                if (preview.State == SpeciesPreviewState.PhaseDecision)
                {
                    preview.ContinueWithoutUpgrade();
                }

                yield return null;
            }

            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
            yield return null;
            Assert.That(GetProperty(simulationViewModel, "CanCloseWindow"), Is.EqualTo(true));
            Assert.That(CanExecute(closeCommand), Is.True);

            Execute(closeCommand);
            timeout = Time.realtimeSinceStartup + 5f;
            while (SceneManager.GetActiveScene().name != "Lab"
                   && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo("Lab"));
            PlayerPrefs.DeleteKey(Helper_ProfileSession.StoreKey);
            PlayerPrefs.Save();
        }

        [UnityTest]
        public IEnumerator GalapagOSSettingsAppUsesDedicatedSettingsSurface()
        {
            yield return SceneManager.LoadSceneAsync("GalapagOSDesktopTest");
            yield return null;
            yield return null;

            var viewModel = GameObject.Find("GalapagOS Desktop Test Camera")
                ?.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(GetProperty(viewModel, "SettingsSurfaceVisibility").ToString(), Is.EqualTo("Collapsed"));

            var openCommand = GetProperty(viewModel, "OpenDesktopIconCommand");
            openCommand.GetType().GetMethod("Execute")?.Invoke(openCommand, new object[] { "Settings" });

            Assert.That(GetProperty(viewModel, "ActiveDesktopAppTitle"), Is.EqualTo("Settings"));
            Assert.That(GetProperty(viewModel, "SettingsSurfaceVisibility").ToString(), Is.EqualTo("Visible"));
            Assert.That(GetProperty(viewModel, "GenericDesktopAppVisibility").ToString(), Is.EqualTo("Collapsed"));

            openCommand.GetType().GetMethod("Execute")?.Invoke(openCommand, new object[] { "Field Notes" });

            Assert.That(GetProperty(viewModel, "SettingsSurfaceVisibility").ToString(), Is.EqualTo("Collapsed"));
            Assert.That(GetProperty(viewModel, "GenericDesktopAppVisibility").ToString(), Is.EqualTo("Visible"));
        }

        [UnityTest]
        public IEnumerator GalapagOSDesktopAppsRemainOpenTogether()
        {
            yield return SceneManager.LoadSceneAsync("GalapagOSDesktopTest");
            yield return null;
            yield return null;

            var viewModel = GameObject.Find("GalapagOS Desktop Test Camera")
                ?.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            Assert.That(viewModel, Is.Not.Null);

            var openCommand = GetProperty(viewModel, "OpenDesktopIconCommand");
            var openWindows = (IList)GetProperty(viewModel, "OpenDesktopWindows");
            Assert.That(openWindows.Count, Is.EqualTo(0));

            openCommand.GetType().GetMethod("Execute")?.Invoke(openCommand, new object[] { "Field Notes" });
            openCommand.GetType().GetMethod("Execute")?.Invoke(openCommand, new object[] { "Gene Lab" });

            Assert.That(openWindows.Count, Is.EqualTo(2));
            Assert.That(GetProperty(openWindows[0], "Title"), Is.EqualTo("Field Notes"));
            Assert.That(GetProperty(openWindows[1], "Title"), Is.EqualTo("Gene Lab"));
            Assert.That(GetProperty(viewModel, "ActiveDesktopAppTitle"), Is.EqualTo("Gene Lab"));

            var fieldNotesWindow = openWindows[0];
            Assert.That(GetProperty(fieldNotesWindow, "FieldGuideSurfaceVisibility").ToString(), Is.EqualTo("Visible"));
            Assert.That((float)GetProperty(fieldNotesWindow, "Width"), Is.InRange(1040f, 1510f));
            Assert.That((float)GetProperty(fieldNotesWindow, "Height"), Is.InRange(560f, 820f));

            var geneLabWindow = openWindows[1];
            Assert.That((float)GetProperty(geneLabWindow, "Width"), Is.InRange(960f, 1450f));
            Assert.That((float)GetProperty(geneLabWindow, "Height"), Is.InRange(560f, 820f));
            Assert.That(GetProperty(geneLabWindow, "GuardedBurrowStateText"), Is.EqualTo("UNLOCKED · ACTIVE"));
            Assert.That(GetProperty(geneLabWindow, "ActiveGenomeCapacityText"), Is.EqualTo("6 / 8"));

            var toggleGenomeCommand = GetProperty(geneLabWindow, "ToggleGuardedBurrowCommand");
            toggleGenomeCommand.GetType().GetMethod("Execute")?.Invoke(toggleGenomeCommand, new object[] { null });

            Assert.That(GetProperty(geneLabWindow, "GuardedBurrowStateText"), Is.EqualTo("UNLOCKED · INACTIVE"));
            Assert.That(GetProperty(geneLabWindow, "ActiveGenomeCapacityText"), Is.EqualTo("5 / 8"));

            var preservedWindow = openWindows[0];
            Assert.That(preservedWindow, Is.Not.Null);
            var closeCommand = GetProperty(preservedWindow, "CloseCommand");
            closeCommand.GetType().GetMethod("Execute")?.Invoke(closeCommand, new object[] { null });

            Assert.That(openWindows.Count, Is.EqualTo(1));
            openCommand.GetType().GetMethod("Execute")?.Invoke(openCommand, new object[] { "Gene Lab" });
            Assert.That(openWindows.Count, Is.EqualTo(1));
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
