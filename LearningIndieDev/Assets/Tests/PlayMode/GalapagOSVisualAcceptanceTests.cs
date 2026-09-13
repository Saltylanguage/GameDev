using System;
using System.Collections;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace SaltyGame.PlayModeTests
{
    public sealed class GalapagOSVisualAcceptanceTests
    {
        const string SceneName = "GalapagOSDesktopTest";
        const string DesktopCameraName = "GalapagOS Desktop Test Camera";
        const string SimulationCameraName = "GalapagOS Simulation View";

        [UnityTest]
        public IEnumerator GalapagOSDesktopAndSimulationCaptureGameViewEvidence()
        {
            Screen.SetResolution(
                GetVisualDimension("CELLSIM_VISUAL_WIDTH", 1280),
                GetVisualDimension("CELLSIM_VISUAL_HEIGHT", 720),
                false);
            yield return null;
            yield return SceneManager.LoadSceneAsync(SceneName);
            yield return null;

            var desktopRoot = GameObject.Find(DesktopCameraName);
            Assert.That(desktopRoot, Is.Not.Null);
            var viewModel = desktopRoot.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            Assert.That(viewModel, Is.Not.Null);

            var outputDirectory = TryGetVisualOutputDirectory();
            if (outputDirectory == null)
            {
                Assert.Pass("Set CELLSIM_VISUAL_OUTPUT to capture GalapagOS Game-view evidence.");
            }

            var openCommand = GetProperty(viewModel, "OpenDesktopIconCommand");
            yield return CaptureCamera(outputDirectory, "01-galapagos-desktop-home", desktopRoot.GetComponent<Camera>());

            openCommand.GetType().GetMethod("Execute")?.Invoke(openCommand, new object[] { "Field Notes" });
            var fieldNotesWindows = (System.Collections.IList)GetProperty(viewModel, "OpenDesktopWindows");
            Assert.That(fieldNotesWindows.Count, Is.EqualTo(1));
            Assert.That(GetProperty(fieldNotesWindows[0], "FieldGuideSurfaceVisibility").ToString(), Is.EqualTo("Visible"));
            Assert.That((float)GetProperty(fieldNotesWindows[0], "Width"), Is.InRange(1040f, 1510f));
            Assert.That((float)GetProperty(fieldNotesWindows[0], "Height"), Is.InRange(560f, 820f));
            yield return CaptureCamera(outputDirectory, "02-galapagos-field-notes", desktopRoot.GetComponent<Camera>());

            openCommand.GetType().GetMethod("Execute")?.Invoke(openCommand, new object[] { "Settings" });
            var openWindows = (System.Collections.IList)GetProperty(viewModel, "OpenDesktopWindows");
            Assert.That(openWindows.Count, Is.EqualTo(2));
            Assert.That(GetProperty(openWindows[1], "SettingsSurfaceVisibility").ToString(), Is.EqualTo("Visible"));
            yield return null;
            yield return null;
            yield return null;

            openCommand.GetType().GetMethod("Execute")?.Invoke(openCommand, new object[] { "Simulation" });
            yield return null;
            yield return null;
            yield return null;
            yield return CaptureCamera(
                outputDirectory,
                "03-galapagos-simulation",
                GameObject.Find(SimulationCameraName)?.GetComponent<Camera>());

            var simulationCamera = GameObject.Find(SimulationCameraName)?.GetComponent<Camera>();
            Assert.That(simulationCamera, Is.Not.Null);
            Assert.That(simulationCamera.enabled, Is.True);
            Assert.That(desktopRoot.GetComponent<Camera>().enabled, Is.False);

            var preview = desktopRoot.GetComponent("SaltyGame.SpeciesSimulationPreview");
            Assert.That(preview, Is.Not.Null);
            var simulationViewModel = desktopRoot.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(simulationViewModel, Is.Not.Null);
            Assert.That(GetProperty(preview, "State").ToString(), Is.EqualTo("Running"));
            Assert.That(GetProperty(simulationViewModel, "SettingsVisibility").ToString(), Is.EqualTo("Collapsed"));
            Assert.That(GetProperty(simulationViewModel, "RunningVisibility").ToString(), Is.EqualTo("Visible"));
            Assert.That(((UnityEngine.Object)GetProperty(preview, "SelectedScenario")).name, Is.EqualTo("ForestEdge"));
            Assert.That(GetProperty(GetProperty(preview, "PlayerSpecies"), "Value"), Is.EqualTo("hare"));
            Assert.That(GetProperty(preview, "GridWidth"), Is.EqualTo(42));
            Assert.That(GetProperty(preview, "GridHeight"), Is.EqualTo(20));

            var boardViewModel = desktopRoot.GetComponent("SaltyGame.VM_SimulationBoard");
            Assert.That(boardViewModel, Is.Not.Null);
            var snapshot = GetProperty(boardViewModel, "Snapshot");
            Assert.That(snapshot, Is.Not.Null);
            Assert.That(GetProperty(snapshot, "Width"), Is.EqualTo(42));
            Assert.That(GetProperty(snapshot, "Height"), Is.EqualTo(20));

            Assert.That(GetProperty(simulationViewModel, "HerbivorePopulation"), Is.GreaterThan(0));
            Assert.That(GetProperty(simulationViewModel, "CarnivorePopulation"), Is.GreaterThan(0));
        }

        static object GetProperty(object target, string propertyName)
        {
            var property = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            Assert.That(property, Is.Not.Null, $"Expected property '{propertyName}'.");
            return property.GetValue(target);
        }

        static string TryGetVisualOutputDirectory()
        {
            var configuredPath = Environment.GetEnvironmentVariable("CELLSIM_VISUAL_OUTPUT");
            if (string.IsNullOrWhiteSpace(configuredPath))
            {
                return null;
            }

            var projectRoot = Directory.GetParent(Application.dataPath).FullName;
            var directory = Path.IsPathRooted(configuredPath)
                ? configuredPath
                : Path.Combine(projectRoot, configuredPath);
            Directory.CreateDirectory(directory);
            return directory;
        }

        static IEnumerator CaptureCamera(string directory, string name, Camera camera)
        {
            for (var frame = 0; frame < 10; frame++)
            {
                yield return null;
            }

            Assert.That(camera, Is.Not.Null, $"Visual evidence requires camera '{name}'.");
            var width = GetVisualDimension("CELLSIM_VISUAL_WIDTH", 1280);
            var height = GetVisualDimension("CELLSIM_VISUAL_HEIGHT", 720);
            var path = Path.Combine(directory, name + ".png");
            var previousTarget = camera.targetTexture;
            var previousActive = RenderTexture.active;
            var renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            var image = new Texture2D(width, height, TextureFormat.RGBA32, false);
            try
            {
                renderTexture.Create();
                camera.targetTexture = renderTexture;
                camera.Render();
                RenderTexture.active = renderTexture;
                image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                image.Apply();
                File.WriteAllBytes(path, image.EncodeToPNG());
            }
            finally
            {
                camera.targetTexture = previousTarget;
                RenderTexture.active = previousActive;
                UnityEngine.Object.Destroy(image);
                UnityEngine.Object.Destroy(renderTexture);
            }

            Assert.That(File.Exists(path), Is.True, $"Unity did not write screenshot '{path}'.");
        }

        static int GetVisualDimension(string environmentVariable, int fallback)
        {
            return int.TryParse(Environment.GetEnvironmentVariable(environmentVariable), out var value)
                ? Mathf.Max(1, value)
                : fallback;
        }
    }
}
