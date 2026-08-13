using System;
using System.Collections.Generic;
using SaltyGame;
using UnityEditor;
using UnityEngine;

namespace SaltyGame.EditorTools
{
    /// <summary>Creates the first reusable species library and three experiment scenarios.</summary>
    public static class CreateSpeciesScenarioAssets
    {
        const string Root = "Assets/Data/CellularSimulation";
        const string SpeciesRoot = Root + "/Species";
        const string ScenarioRoot = Root + "/Scenarios";

        [MenuItem("Salty Game/Simulation/Create Species Scenarios")]
        public static void Create()
        {
            EnsureFolder("Assets/Data");
            EnsureFolder(Root);
            EnsureFolder(SpeciesRoot);
            EnsureFolder(ScenarioRoot);

            var cardinal = new[]
            {
                Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left,
            };
            var moore = new[]
            {
                new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1),
                new Vector2Int(-1, 0), new Vector2Int(1, 0),
                new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1),
            };

            var plants = new[]
            {
                CreatePlant("fern", 20f, 0.01f),
                CreatePlant("reed", 18f, 0.01f),
            };
            var herbivores = new[]
            {
                CreateHerbivore("hare", "fern", 2.2f, 16, 0.25f),
                CreateHerbivore("deer", "fern", 1.4f, 20, 0.2f),
                CreateHerbivore("snail", "reed", 0.55f, 10, 0.3f),
                CreateHerbivore("beetle", "reed", 1.8f, 12, 0.25f),
            };
            var carnivores = new[]
            {
                CreateCarnivore("fox", "hare", 0.8f, 32, 0.02f),
                CreateCarnivore("wolf", "deer", 0.8f, 36, 0.02f),
                CreateCarnivore("owl", "snail", 0.9f, 28, 0.02f),
                CreateCarnivore("stoat", "beetle", 1.0f, 28, 0.02f),
            };

            CreateScenario("ForestEdge", new[]
            {
                Entry(plants[0], 0.42f), Entry(herbivores[0], 0.10f), Entry(carnivores[0], 0.004f),
            });
            CreateScenario("Wetland", new[]
            {
                Entry(plants[1], 0.34f), Entry(herbivores[2], 0.09f),
                Entry(herbivores[3], 0.09f), Entry(carnivores[2], 0.004f),
                Entry(carnivores[3], 0.004f),
            });
            CreateScenario("OpenRange", new[]
            {
                Entry(plants[0], 0.42f), Entry(plants[1], 0.34f),
                Entry(herbivores[1], 0.08f), Entry(herbivores[3], 0.09f),
                Entry(carnivores[1], 0.003f),
            });

            // Validation-only scenario: its fingerprint is compared to the legacy defaults.
            var legacyPlant = CreateLegacyPlant();
            var legacyHerbivore = CreateLegacyHerbivore();
            var legacyCarnivore = CreateLegacyCarnivore();
            CreateScenario("BaselineParity", new[]
            {
                Entry(legacyPlant, 0.4f), Entry(legacyHerbivore, 0.16f),
                Entry(legacyCarnivore, 0.04f),
            });

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Salty] Created 10 species assets and three experiment scenarios under Assets/Data/CellularSimulation.");
        }

        [MenuItem("Salty Game/Simulation/Validate Baseline Parity")]
        public static void ValidateParity()
        {
            var legacyAsset = ScriptableObject.CreateInstance<CellularSimDataAsset>();
            try
            {
                var scenario = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionAsset>(
                    $"{ScenarioRoot}/BaselineParity.asset");
                if (scenario == null)
                {
                    throw new InvalidOperationException("BaselineParity.asset is missing. Run Create Species Scenarios first.");
                }

                var before = legacyAsset.CreateRuntimeData();
                var after = scenario.CreateRuntimeData();
                if (!string.Equals(before.Fingerprint, after.Fingerprint, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        $"Ruleset fingerprints differ. Legacy={before.Fingerprint}, authored={after.Fingerprint}.");
                }

                for (var seed = 10100; seed < 10105; seed++)
                {
                    var beforeRun = Run(before, seed);
                    var afterRun = Run(after, seed);
                    AssertSameGrid(beforeRun.Cells, afterRun.Cells, seed);
                }

                Debug.Log("[Salty] Baseline parity passed: matching fingerprints and final grids for seeds 10100-10104.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(legacyAsset);
            }
        }

        static SimulationRunState Run(CellularSimData data, int seed)
        {
            var run = new SimulationRunState(
                SpeciesInitialGridFactory.Create(data, seed),
                SpeciesIds.Herbivore,
                seed,
                data.RunDurationSeconds);
            var runner = new SpeciesSimulationRunner(run, data);
            while (runner.AdvanceOneTick())
            {
            }

            return run;
        }

        static void AssertSameGrid(Grid<SpeciesCell> before, Grid<SpeciesCell> after, int seed)
        {
            if (before.Width != after.Width || before.Height != after.Height)
            {
                throw new InvalidOperationException($"Grid dimensions differ for seed {seed}.");
            }

            for (var y = 0; y < before.Height; y++)
            {
                for (var x = 0; x < before.Width; x++)
                {
                    var left = before.GetCell(x, y);
                    var right = after.GetCell(x, y);
                    if (left.IsOccupied != right.IsOccupied
                        || left.SpeciesId != right.SpeciesId
                        || left.Health != right.Health
                        || left.Energy != right.Energy
                        || Math.Abs(left.FoodReserve - right.FoodReserve) > 0.0001f
                        || left.TerrainId != right.TerrainId
                        || Math.Abs(left.TerrainEnergy - right.TerrainEnergy) > 0.0001f)
                    {
                        throw new InvalidOperationException($"Grid mismatch at ({x},{y}) for seed {seed}.");
                    }
                }
            }
        }

        static PlantSpeciesDefinitionAsset CreatePlant(string id, float reserve, float wilt)
        {
            var asset = GetOrCreate<PlantSpeciesDefinitionAsset>($"{id}.asset");
            SetCommon(asset, id, 0f, Array.Empty<Vector2Int>(), Array.Empty<Vector2Int>(), 0,
                Array.Empty<Vector2Int>(), Array.Empty<Vector2Int>(), 0.1f, 0, 0, 0, wilt, 0,
                reserve, 0f, 1, -1, 0);
            return asset;
        }

        static PlantSpeciesDefinitionAsset CreateLegacyPlant()
        {
            var asset = GetOrCreate<PlantSpeciesDefinitionAsset>("plant.asset");
            SetCommon(asset, "plant", 0f, Cardinal(), Cardinal(), 0,
                Cardinal(), Cardinal(), 0.1f, 0, 0, 0, 0.003f, 0,
                3.25f, 0f, 1, -1, 0);
            Set(asset, "dietPattern", Cardinal());
            Set(asset, "startingEnergy", 0);
            Set(asset, "intelligence", 0);
            return asset;
        }

        static HerbivoreSpeciesDefinitionAsset CreateLegacyHerbivore()
        {
            var asset = GetOrCreate<HerbivoreSpeciesDefinitionAsset>("herbivore.asset");
            SetCommon(asset, "herbivore", 1.5f, Cardinal(), Cardinal(), 1,
                Cardinal(), Moore(), 0.5f, 1, 1, 4, 0f, 1,
                0f, 0.05f, 4, 1, 5);
            Set(asset, "dietPattern", Moore());
            Set(asset, "dietTargetId", SpeciesIds.Plant.Value);
            Set(asset, "startingEnergy", 12);
            Set(asset, "forageBelowEnergy", 12);
            return asset;
        }

        static CarnivoreSpeciesDefinitionAsset CreateLegacyCarnivore()
        {
            var asset = GetOrCreate<CarnivoreSpeciesDefinitionAsset>("carnivore.asset");
            SetCommon(asset, "carnivore", 1.5f, Moore(), Moore(), 2,
                Cardinal(), Cardinal(), 0.4f, 1, 1, 3, 0f, 1,
                0f, 0f, 8, 1, 4);
            Set(asset, "dietPattern", Moore());
            Set(asset, "dietTargetId", SpeciesIds.Herbivore.Value);
            Set(asset, "startingEnergy", 18);
            Set(asset, "forageBelowEnergy", 18);
            return asset;
        }

        static HerbivoreSpeciesDefinitionAsset CreateHerbivore(
            string id,
            string dietTarget,
            float speed,
            int energy,
            float reproductionChance)
        {
            var asset = GetOrCreate<HerbivoreSpeciesDefinitionAsset>($"{id}.asset");
            SetCommon(asset, id, speed, Cardinal(), Array.Empty<Vector2Int>(), 0,
                Cardinal(), Moore(), reproductionChance, 1, 1, 4, 0f, 1,
                0f, 0.05f, 4, 1, 5);
            Set(asset, "dietTargetId", dietTarget);
            Set(asset, "attackAmount", 1);
            Set(asset, "startingEnergy", energy);
            Set(asset, "forageBelowEnergy", energy);
            Set(asset, "visionRange", 5);
            Set(asset, "intelligence", 1);
            return asset;
        }

        static CarnivoreSpeciesDefinitionAsset CreateCarnivore(
            string id,
            string dietTarget,
            float speed,
            int energy,
            float reproductionChance)
        {
            var asset = GetOrCreate<CarnivoreSpeciesDefinitionAsset>($"{id}.asset");
            SetCommon(asset, id, speed, Moore(), Moore(), 2,
                Cardinal(), Cardinal(), reproductionChance, 1, 1, 3, 0f, 1,
                0f, 0f, 8, 1, 4);
            Set(asset, "dietTargetId", dietTarget);
            Set(asset, "startingEnergy", energy);
            Set(asset, "forageBelowEnergy", energy);
            Set(asset, "visionRange", 5);
            Set(asset, "intelligence", 1);
            return asset;
        }

        static void SetCommon(
            SpeciesDefinitionAsset asset,
            string id,
            float speed,
            Vector2Int[] movement,
            Vector2Int[] attack,
            int attackAmount,
            Vector2Int[] block,
            Vector2Int[] reproduction,
            float reproductionChance,
            int neighborCount,
            int foodRequired,
            int groupSize,
            float wilt,
            int crowding,
            float reserve,
            float seedDrop,
            int energyValue,
            int metabolism,
            int vision)
        {
            Set(asset, "id", id);
            Set(asset, "movementSpeed", speed);
            Set(asset, "movementPattern", movement);
            Set(asset, "attackPattern", attack);
            Set(asset, "attackAmount", attackAmount);
            Set(asset, "blockPattern", block);
            Set(asset, "blockAmount", 0);
            Set(asset, "dietPattern", Moore());
            Set(asset, "reproductionPattern", reproduction);
            Set(asset, "reproductionNeighborCount", neighborCount);
            Set(asset, "reproductionChance", reproductionChance);
            Set(asset, "reproductionFoodRequired", foodRequired);
            Set(asset, "maxReproductionGroupSize", groupSize);
            Set(asset, "startingEnergy", 0);
            Set(asset, "forageBelowEnergy", 0);
            Set(asset, "wiltChance", wilt);
            Set(asset, "crowdingEnergyPenalty", crowding);
            Set(asset, "startingFoodReserve", reserve);
            Set(asset, "seedDropChance", seedDrop);
            Set(asset, "energyValue", energyValue);
            Set(asset, "metabolism", metabolism);
            Set(asset, "visionRange", vision);
            Set(asset, "intelligence", 1);
            Set(asset, "alphaChance", 0f);
            Set(asset, "alphaHealthBonus", 0);
            Set(asset, "alphaEnergyBonus", 0);
        }

        static ScenarioDefinitionAsset CreateScenario(
            string id,
            ScenarioDefinitionAsset.SpeciesEntry[] definitions)
        {
            var asset = GetOrCreate<ScenarioDefinitionAsset>($"{id}.asset", ScenarioRoot);
            Set(asset, "width", 32);
            Set(asset, "height", 20);
            Set(asset, "runDurationSeconds", 20f);
            Set(asset, "stepInterval", 0.1f);
            Set(asset, "maxPopulation", 0);
            Set(asset, "minPopulation", 0);
            Set(asset, "species", definitions);
            return asset;
        }

        static ScenarioDefinitionAsset.SpeciesEntry Entry(
            SpeciesDefinitionAsset definition,
            float startingProbability)
        {
            return new ScenarioDefinitionAsset.SpeciesEntry(definition, startingProbability);
        }

        static Vector2Int[] Cardinal() => new[]
        {
            Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left,
        };

        static Vector2Int[] Moore() => new[]
        {
            new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1),
            new Vector2Int(-1, 0), new Vector2Int(1, 0),
            new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1),
        };

        static T GetOrCreate<T>(string fileName, string directory = SpeciesRoot) where T : ScriptableObject
        {
            var path = $"{directory}/{fileName}";
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
            {
                return asset;
            }

            if (AssetDatabase.LoadMainAssetAtPath(path) != null)
            {
                AssetDatabase.DeleteAsset(path);
            }

            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        static void Set(UnityEngine.Object asset, string property, object value)
        {
            var serialized = new SerializedObject(asset);
            var field = serialized.FindProperty(property);
            if (field == null)
            {
                throw new InvalidOperationException($"Serialized field '{property}' was not found on {asset.name}.");
            }

            switch (value)
            {
                case string text: field.stringValue = text; break;
                case int number: field.intValue = number; break;
                case float number: field.floatValue = number; break;
                case Vector2Int[] vectors:
                    field.arraySize = vectors.Length;
                    for (var i = 0; i < vectors.Length; i++)
                    {
                        field.GetArrayElementAtIndex(i).vector2IntValue = vectors[i];
                    }
                    break;
                case ScenarioDefinitionAsset.SpeciesEntry[] definitions:
                    field.arraySize = definitions.Length;
                    for (var i = 0; i < definitions.Length; i++)
                    {
                        var entry = field.GetArrayElementAtIndex(i);
                        entry.FindPropertyRelative("definition").objectReferenceValue = definitions[i].Definition;
                        entry.FindPropertyRelative("startingProbability").floatValue = definitions[i].StartingProbability;
                    }
                    break;
                default: throw new InvalidOperationException($"Unsupported serialized value type {value?.GetType()}.");
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
        }

        static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            var parent = path.Substring(0, path.LastIndexOf('/'));
            var name = path.Substring(path.LastIndexOf('/') + 1);
            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
