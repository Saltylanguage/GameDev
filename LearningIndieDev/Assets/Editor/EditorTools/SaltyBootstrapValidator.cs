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

                if (AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/Scripts/Prototype/WoodChopPrototype.cs") == null)
                    checks.Add("PASS: Legacy giant prototype is absent");
                else
                    checks.Add("FAIL: Legacy giant prototype still exists");
            }
            finally
            {
                if (openedHere && scene.IsValid() && scene.isLoaded)
                    EditorSceneManager.CloseScene(scene, true);
            }

            var passed = checks.All(check => check.StartsWith("PASS:"));
            foreach (var check in checks)
            {
                if (passed)
                    Debug.Log("[Salty] " + check);
                else
                    Debug.LogError("[Salty] " + check);
            }

            if (!Application.isBatchMode)
            {
                EditorUtility.DisplayDialog(
                    "Salty Bootstrap Validation",
                    string.Join("\n", checks),
                    "Close");
            }
        }
    }
}
