using System;
using System.IO;
using UnityEngine;

namespace SaltyGame.PlayModeTests
{
    static class VisualTestConfiguration
    {
        const string RequestPath = "Temp/cellsim_visual_request.json";

        public static string GetValue(string name)
        {
            var environmentValue = Environment.GetEnvironmentVariable(name);
            if (!string.IsNullOrWhiteSpace(environmentValue))
            {
                return environmentValue;
            }

            var projectRoot = Directory.GetParent(Application.dataPath).FullName;
            var path = Path.Combine(projectRoot, RequestPath.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(path))
            {
                return null;
            }

            try
            {
                var request = JsonUtility.FromJson<VisualRequest>(File.ReadAllText(path));
                if (request?.values == null)
                {
                    return null;
                }

                foreach (var entry in request.values)
                {
                    if (entry != null && string.Equals(entry.name, name, StringComparison.Ordinal))
                    {
                        return entry.value;
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Could not read CellSim visual request '{path}': {exception.Message}");
            }

            return null;
        }

        [Serializable]
        sealed class VisualRequest
        {
            public VisualValue[] values;
        }

        [Serializable]
        sealed class VisualValue
        {
            public string name;
            public string value;
        }
    }
}
