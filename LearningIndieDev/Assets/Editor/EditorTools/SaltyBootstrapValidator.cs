using System.Collections.Generic;
using System.Linq;
using SaltyGame;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SaltyGame.EditorTools
{
    public static class SaltyBootstrapValidator
    {
        const string BootstrapPath = "Assets/Scenes/Boostrap.unity";
        const string LegacyPrototypePath = "Assets/Scripts/Prototype/WoodChopPrototype.cs";
        const string SurvivalStatePath = "Assets/Scripts/Game/Survival/SurvivalState.cs";
        const string CampfirePath = "Assets/Scripts/Game/Camp/CampfireInteractable.cs";

        [MenuItem("Salty/Validate Bootstrap Scene")]
        public static void ValidateBootstrapScene()
        {
            var checks = new List<string>();
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(BootstrapPath);
            if (sceneAsset == null)
                checks.Add($"FAIL: Missing scene at {BootstrapPath}");
            else
                checks.Add($"PASS: Found {BootstrapPath}");

            var buildScene = EditorBuildSettings.scenes.FirstOrDefault(scene => scene.path == BootstrapPath);
            if (buildScene != null && buildScene.path == BootstrapPath && buildScene.enabled)
                checks.Add("PASS: Bootstrap is enabled in Build Settings");
            else
                checks.Add("FAIL: Bootstrap is missing or disabled in Build Settings");

            if (sceneAsset != null)
            {
                var scene = SceneManager.GetSceneByPath(BootstrapPath);
                var openedHere = false;
                try
                {
                    if (!scene.IsValid() || !scene.isLoaded)
                    {
                        scene = EditorSceneManager.OpenScene(BootstrapPath, OpenSceneMode.Additive);
                        openedHere = true;
                    }

                    var runtimes = scene.GetRootGameObjects()
                        .Select(root => root.GetComponent<GameRuntime>())
                        .Where(runtime => runtime != null)
                        .ToArray();

                    if (runtimes.Length == 1 && runtimes[0].enabled && runtimes[0].gameObject.activeInHierarchy)
                        checks.Add("PASS: Exactly one active GameRuntime composition root exists");
                    else
                        checks.Add($"FAIL: Expected one active GameRuntime, found {runtimes.Length}");

            if (AssetDatabase.LoadAssetAtPath<MonoScript>(LegacyPrototypePath) == null)
                checks.Add("PASS: Legacy giant prototype is absent");
            else
                checks.Add("FAIL: Legacy giant prototype still exists");

            if (AssetDatabase.LoadAssetAtPath<MonoScript>(SurvivalStatePath) != null)
                checks.Add("PASS: Survival runtime dependency exists");
            else
                checks.Add("FAIL: Missing SurvivalState runtime dependency");

            if (AssetDatabase.LoadAssetAtPath<MonoScript>(CampfirePath) != null)
                checks.Add("PASS: Camp runtime dependency exists");
            else
                checks.Add("FAIL: Missing CampfireInteractable runtime dependency");
                }
                finally
                {
                    if (openedHere && scene.IsValid() && scene.isLoaded)
                        EditorSceneManager.CloseScene(scene, true);
                }
            }
            else
            {
                checks.Add("FAIL: Composition root could not be inspected because the scene is missing");
                checks.Add("FAIL: Legacy prototype check skipped because the scene is missing");
            }

            var passed = checks.All(check => check.StartsWith("PASS:"));
            foreach (var check in checks)
            {
                if (check.StartsWith("PASS:"))
                    Debug.Log("[Salty] " + check);
                else
                    Debug.LogError("[Salty] " + check);
            }

            if (!Application.isBatchMode)
            {
                EditorUtility.DisplayDialog(
                    $"Salty Bootstrap Validation - {(passed ? "PASS" : "FAIL")}",
                    string.Join("\n", checks),
                    "Close");
            }
        }
    }
}
