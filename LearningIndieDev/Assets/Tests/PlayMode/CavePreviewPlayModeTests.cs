using System;
using System.Collections;
using System.Globalization;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace SaltyGame.PlayModeTests
{
    public sealed class CavePreviewPlayModeTests
    {
        [UnityTest]
        public IEnumerator CellularAutomataPrototypeCreatesAndAnimatesTheSpeciesPreview()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var runtime = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>();

            Assert.That(runtime, Is.Not.Null);
            Assert.That(runtime.SpeciesPreview, Is.Not.Null);
            runtime.SpeciesPreview.LegacyUiEnabled = false;
            runtime.SpeciesPreview.NoesisUiEnabled = true;
            Assert.That(runtime.SpeciesPreview.Run, Is.Not.Null);
            Assert.That(runtime.SpeciesPreview.Run.Status, Is.EqualTo(SimulationRunStatus.Ready));

            var replay = TryGetReplayConfiguration();
            if (replay != null)
            {
                var scenarioName = Path.GetFileNameWithoutExtension(
                    replay.ScenarioPath.Replace('/', Path.DirectorySeparatorChar));
                var scenarioIndex = -1;
                for (var index = 0; index < runtime.SpeciesPreview.ScenarioOptions.Count; index++)
                {
                    var option = runtime.SpeciesPreview.ScenarioOptions[index];
                    if (option != null && string.Equals(option.name, scenarioName, StringComparison.OrdinalIgnoreCase))
                    {
                        scenarioIndex = index;
                        break;
                    }
                }

                Assert.That(scenarioIndex, Is.GreaterThanOrEqualTo(0),
                    $"Visual replay scenario '{replay.ScenarioPath}' is not assigned to the prototype scene.");
                Assert.That(runtime.SpeciesPreview.TrySelectScenario(scenarioIndex, out var scenarioMessage),
                    Is.True, scenarioMessage);
                Assert.That(runtime.SpeciesPreview.TrySetPlayerSpecies(replay.PlayerSpeciesId, out var speciesMessage),
                    Is.True, speciesMessage);
            }

            var gridWidth = replay?.GridWidth ?? 64;
            var gridHeight = replay?.GridHeight ?? 64;
            var seed = replay?.Seed ?? runtime.SpeciesPreview.BaseSeed;
            var settingsApplied = runtime.SpeciesPreview.TryApplyGlobalSettings(
                gridWidth.ToString(CultureInfo.InvariantCulture),
                gridHeight.ToString(CultureInfo.InvariantCulture),
                seed.ToString(CultureInfo.InvariantCulture),
                runtime.SpeciesPreview.MaximumPopulation.ToString(CultureInfo.InvariantCulture),
                runtime.SpeciesPreview.MinimumPopulation.ToString(CultureInfo.InvariantCulture),
                runtime.SpeciesPreview.RunDurationSeconds.ToString(CultureInfo.InvariantCulture),
                runtime.SpeciesPreview.StepInterval.ToString(CultureInfo.InvariantCulture),
                runtime.SpeciesPreview.PlantProbability.ToString(CultureInfo.InvariantCulture),
                runtime.SpeciesPreview.HerbivoreProbability.ToString(CultureInfo.InvariantCulture),
                runtime.SpeciesPreview.CarnivoreProbability.ToString(CultureInfo.InvariantCulture),
                replay == null && runtime.SpeciesPreview.RandomizeSeedOnStart,
                out var settingsMessage);

            Assert.That(settingsApplied, Is.True, settingsMessage);
            Assert.That(runtime.SpeciesPreview.GridWidth, Is.EqualTo(gridWidth));
            Assert.That(runtime.SpeciesPreview.GridHeight, Is.EqualTo(gridHeight));

            if (TryGetVisualOutputDirectory(out var visualOutputDirectory))
            {
                yield return CaptureScreenshot(visualOutputDirectory, "01-settings");
            }

            runtime.SpeciesPreview.StartSimulation();

            yield return new WaitForSeconds(0.35f);

            Assert.That(runtime.SpeciesPreview.Run.Tick, Is.GreaterThanOrEqualTo(1));

            if (!TryGetVisualOutputDirectory(out visualOutputDirectory))
            {
                yield break;
            }

            var lateRunSeconds = Mathf.Max(0f,
                runtime.SpeciesPreview.RunDurationSeconds - runtime.SpeciesPreview.StepInterval);
            while (runtime.SpeciesPreview.Run.Status == SimulationRunStatus.Running &&
                   runtime.SpeciesPreview.Run.ElapsedSeconds < lateRunSeconds)
            {
                yield return null;
            }

            Assert.That(runtime.SpeciesPreview.Run.Status, Is.EqualTo(SimulationRunStatus.Running));
            Assert.That(runtime.SpeciesPreview.Run.ElapsedSeconds, Is.GreaterThanOrEqualTo(lateRunSeconds));
            yield return CaptureScreenshot(visualOutputDirectory, "02-running");

            var timeout = Time.realtimeSinceStartup + runtime.SpeciesPreview.RunDurationSeconds + 5f;
            while (runtime.SpeciesPreview.Run.Status != SimulationRunStatus.Complete &&
                   Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(runtime.SpeciesPreview.Run.Status, Is.EqualTo(SimulationRunStatus.Complete));
            yield return CaptureScreenshot(visualOutputDirectory, "03-rewards");

            runtime.SpeciesPreview.ContinueWithoutUpgrade();
            yield return null;
            yield return CaptureScreenshot(visualOutputDirectory, "04-results");
        }

        static ReplayConfiguration TryGetReplayConfiguration()
        {
            var scenarioPath = Environment.GetEnvironmentVariable("CELLSIM_REPLAY_SCENARIO");
            var playerSpeciesId = Environment.GetEnvironmentVariable("CELLSIM_REPLAY_PLAYER_SPECIES_ID");
            var seedValue = Environment.GetEnvironmentVariable("CELLSIM_REPLAY_SEED");
            var widthValue = Environment.GetEnvironmentVariable("CELLSIM_REPLAY_GRID_WIDTH");
            var heightValue = Environment.GetEnvironmentVariable("CELLSIM_REPLAY_GRID_HEIGHT");
            if (string.IsNullOrWhiteSpace(scenarioPath)
                || string.IsNullOrWhiteSpace(playerSpeciesId)
                || !int.TryParse(seedValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var seed)
                || !int.TryParse(widthValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var width)
                || !int.TryParse(heightValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var height))
            {
                return null;
            }

            return new ReplayConfiguration(scenarioPath, playerSpeciesId, seed, width, height);
        }

        sealed class ReplayConfiguration
        {
            public ReplayConfiguration(string scenarioPath, string playerSpeciesId, int seed, int gridWidth, int gridHeight)
            {
                ScenarioPath = scenarioPath;
                PlayerSpeciesId = playerSpeciesId;
                Seed = seed;
                GridWidth = gridWidth;
                GridHeight = gridHeight;
            }

            public string ScenarioPath { get; }
            public string PlayerSpeciesId { get; }
            public int Seed { get; }
            public int GridWidth { get; }
            public int GridHeight { get; }
        }

        static bool TryGetVisualOutputDirectory(out string directory)
        {
            var configuredPath = Environment.GetEnvironmentVariable("CELLSIM_VISUAL_OUTPUT");
            if (string.IsNullOrWhiteSpace(configuredPath))
            {
                directory = null;
                return false;
            }

            var projectRoot = Directory.GetParent(Application.dataPath).FullName;
            directory = Path.IsPathRooted(configuredPath)
                ? configuredPath
                : Path.Combine(projectRoot, configuredPath);
            Directory.CreateDirectory(directory);
            return true;
        }

        static IEnumerator CaptureScreenshot(string directory, string name)
        {
            var path = Path.Combine(directory, name + ".png");
            yield return null;
            yield return null;
            var camera = UnityEngine.Object.FindAnyObjectByType<Camera>();
            Assert.That(camera, Is.Not.Null, "Visual evidence requires a scene camera.");

            var previousTarget = camera.targetTexture;
            var previousActive = RenderTexture.active;
            var renderTexture = new RenderTexture(1280, 720, 24, RenderTextureFormat.ARGB32);
            var image = new Texture2D(1280, 720, TextureFormat.RGBA32, false);
            try
            {
                renderTexture.Create();
                camera.targetTexture = renderTexture;
                camera.Render();
                RenderTexture.active = renderTexture;
                image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
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
    }
}
