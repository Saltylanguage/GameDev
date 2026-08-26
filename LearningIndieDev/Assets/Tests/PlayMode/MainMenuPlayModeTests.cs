using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace SaltyGame.PlayModeTests
{
    public sealed class MainMenuPlayModeTests
    {
        const string MainMenuScene = "MainMenu";
        const string LabScene = "Lab";
        const string ViewModelType = "SaltyGame.VM_MainMenu";

        [UnityTest]
        public IEnumerator ContinueIsDisabledWithoutAProfile()
        {
            ClearProfiles();
            yield return LoadMainMenu();

            var viewModel = FindViewModel();
            Assert.That(viewModel, Is.Not.Null, "MainMenu.unity must contain VM_MainMenu.");
            Assert.That(GetBool(viewModel, "ContinueEnabled"), Is.False);

            var continueCommand = GetCommand(viewModel, "ContinueCommand");
            Assert.That(CanExecute(continueCommand), Is.False);
            Execute(continueCommand);
            yield return null;
            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(MainMenuScene));
        }

        [UnityTest]
        public IEnumerator ProfileCreationEnablesContinueAndLoadsLab()
        {
            ClearProfiles();
            yield return LoadMainMenu();

            var viewModel = FindViewModel();
            SetProperty(viewModel, "ProfileNameInput", "Darwin Station");
            ExecuteCommand(viewModel, "OpenProfileSelectionCommand");
            ExecuteCommand(viewModel, "CreateInitialProfileCommand");

            Assert.That(GetBool(viewModel, "ContinueEnabled"), Is.True);
            Assert.That(GetString(viewModel, "CurrentProfileName"), Is.EqualTo("Darwin Station"));

            ExecuteCommand(viewModel, "ContinueCommand");
            yield return null;
            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(LabScene));
            ClearProfiles();
        }

        [UnityTest]
        public IEnumerator LastLoadedProfileSurvivesMainMenuReload()
        {
            ClearProfiles();
            yield return LoadMainMenu();

            var firstViewModel = FindViewModel();
            SetProperty(firstViewModel, "ProfileNameInput", "Returning Researcher");
            ExecuteCommand(firstViewModel, "OpenProfileSelectionCommand");
            ExecuteCommand(firstViewModel, "CreateInitialProfileCommand");

            yield return LoadMainMenu();
            var reloadedViewModel = FindViewModel();
            Assert.That(GetString(reloadedViewModel, "CurrentProfileName"), Is.EqualTo("Returning Researcher"));
            Assert.That(GetBool(reloadedViewModel, "ContinueEnabled"), Is.True);
            ClearProfiles();
        }

        [UnityTest]
        public IEnumerator QuitConfirmationIsAVisualState()
        {
            ClearProfiles();
            yield return LoadMainMenu();

            var viewModel = FindViewModel();
            ExecuteCommand(viewModel, "RequestQuitCommand");
            Assert.That(GetVisibility(viewModel, "QuitConfirmationVisibility"), Is.EqualTo("Visible"));

            ExecuteCommand(viewModel, "CancelQuitCommand");
            Assert.That(GetVisibility(viewModel, "QuitConfirmationVisibility"), Is.EqualTo("Collapsed"));
            ClearProfiles();
        }

        static IEnumerator LoadMainMenu()
        {
            yield return SceneManager.LoadSceneAsync(MainMenuScene);
            yield return null;
            yield return null;
        }

        static void ClearProfiles()
        {
            PlayerPrefs.DeleteKey(Helper_ProfileSession.StoreKey);
            PlayerPrefs.Save();
        }

        static Component FindViewModel()
        {
            var camera = GameObject.Find("Main Camera");
            return camera == null ? null : camera.GetComponent(ViewModelType);
        }

        static void ExecuteCommand(Component viewModel, string commandProperty)
        {
            var command = GetCommand(viewModel, commandProperty);
            Assert.That(CanExecute(command), Is.True, $"Expected command '{commandProperty}' to be executable.");
            Execute(command);
        }

        static object GetCommand(Component viewModel, string propertyName)
        {
            var value = viewModel.GetType().GetProperty(propertyName)?.GetValue(viewModel);
            Assert.That(value, Is.Not.Null, $"Expected command property '{propertyName}'.");
            return value;
        }

        static bool CanExecute(object command)
        {
            return (bool)command.GetType().GetMethod("CanExecute")?.Invoke(command, new object[] { null });
        }

        static void Execute(object command)
        {
            command.GetType().GetMethod("Execute")?.Invoke(command, new object[] { null });
        }

        static bool GetBool(Component viewModel, string propertyName)
        {
            var value = viewModel.GetType().GetProperty(propertyName)?.GetValue(viewModel);
            Assert.That(value, Is.Not.Null, $"Expected boolean property '{propertyName}'.");
            return (bool)value;
        }

        static string GetString(Component viewModel, string propertyName)
        {
            var value = viewModel.GetType().GetProperty(propertyName)?.GetValue(viewModel) as string;
            Assert.That(value, Is.Not.Null, $"Expected string property '{propertyName}'.");
            return value;
        }

        static string GetVisibility(Component viewModel, string propertyName)
        {
            var value = viewModel.GetType().GetProperty(propertyName)?.GetValue(viewModel);
            Assert.That(value, Is.Not.Null, $"Expected visibility property '{propertyName}'.");
            return value.ToString();
        }

        static void SetProperty(Component viewModel, string propertyName, object value)
        {
            viewModel.GetType().GetProperty(propertyName)?.SetValue(viewModel, value);
        }
    }
}
