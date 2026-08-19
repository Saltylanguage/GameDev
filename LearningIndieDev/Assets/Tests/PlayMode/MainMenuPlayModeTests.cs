using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace SaltyGame.PlayModeTests
{
    public sealed class MainMenuPlayModeTests
    {
        const string MainMenuScene = "MainMenu";
        const string ViewModelType = "SaltyGame.MainMenuViewModel";

        [UnityTest]
        public IEnumerator MainMenuRouteLoadsAndNavigatesThroughResearchPreview()
        {
            yield return SceneManager.LoadSceneAsync(MainMenuScene);
            yield return null;
            yield return null;

            var viewModel = FindViewModel();
            Assert.That(viewModel, Is.Not.Null, "MainMenu.unity must contain MainMenuViewModel.");
            Assert.That(GetVisibility(viewModel, "MainMenuVisibility"), Is.EqualTo("Visible"));

            ExecuteCommand(viewModel, "EnterLabCommand");
            Assert.That(GetVisibility(viewModel, "MainMenuVisibility"), Is.EqualTo("Collapsed"));
            Assert.That(GetVisibility(viewModel, "LabVisibility"), Is.EqualTo("Visible"));

            ExecuteCommand(viewModel, "OpenResearchCommand");
            Assert.That(GetVisibility(viewModel, "LabVisibility"), Is.EqualTo("Collapsed"));
            Assert.That(GetVisibility(viewModel, "ResearchVisibility"), Is.EqualTo("Visible"));
            Assert.That(GetString(viewModel, "SelectedProjectState"), Does.Contain("AVAILABLE"));

            ExecuteCommand(viewModel, "SelectLockedProjectCommand");
            Assert.That(GetString(viewModel, "SelectedProjectState"), Does.Contain("LOCKED"));
            Assert.That(GetString(viewModel, "SelectedProjectPrerequisite"), Does.Contain("Forage Route Mapping"));

            ExecuteCommand(viewModel, "BackCommand");
            Assert.That(GetVisibility(viewModel, "LabVisibility"), Is.EqualTo("Visible"));
            ExecuteCommand(viewModel, "BackCommand");
            Assert.That(GetVisibility(viewModel, "MainMenuVisibility"), Is.EqualTo("Visible"));
        }

        static Component FindViewModel()
        {
            var camera = GameObject.Find("Main Camera");
            return camera == null ? null : camera.GetComponent(ViewModelType);
        }

        static void ExecuteCommand(Component viewModel, string commandProperty)
        {
            var command = viewModel.GetType().GetProperty(commandProperty)?.GetValue(viewModel);
            Assert.That(command, Is.Not.Null, $"Expected command property '{commandProperty}'.");
            command.GetType().GetMethod("Execute")?.Invoke(command, new object[] { null });
        }

        static string GetVisibility(Component viewModel, string propertyName)
        {
            var value = viewModel.GetType().GetProperty(propertyName)?.GetValue(viewModel);
            Assert.That(value, Is.Not.Null, $"Expected visibility property '{propertyName}'.");
            return value.ToString();
        }

        static string GetString(Component viewModel, string propertyName)
        {
            var value = viewModel.GetType().GetProperty(propertyName)?.GetValue(viewModel) as string;
            Assert.That(value, Is.Not.Null, $"Expected string property '{propertyName}'.");
            return value;
        }
    }
}
