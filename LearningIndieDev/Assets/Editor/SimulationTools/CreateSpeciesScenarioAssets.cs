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

        [MenuItem("Salty Game/Journey/Add Population Encounters")]
        public static void CreatePopulationEncounters()
        {
            const string folder = "Assets/Data/ProductionData/CellularSimulation/Species/Journey";
            var map = AssetDatabase.LoadAssetAtPath<JourneyMapAsset>(folder + "/ForestEdgeJourney.asset");
            if (map == null) throw new InvalidOperationException("Forest Edge journey is missing.");
            var refresh = PopulationEncounter(folder, "RestStop", "rest-stop", "REST STOP",
                "Choose one refresh before the next cycle: Hares within your population pool, or Plants. You may continue without a refresh.",
                JourneyNodeKind.Refresh, 2, map.GetNode("seedfall").NextNodes[0]);
            var gamble = PopulationEncounter(folder, "SeedCache", "seed-cache", "SEED CACHE",
                "An uncertain cache could renew the food supply or spoil existing Plants. The odds are shown before you choose. You may leave it alone.",
                JourneyNodeKind.Event, 3, map.GetNode("fox-tracks").NextNodes[0]);
            var nodes = new List<JourneyNodeAsset>(map.Nodes);
            if (!nodes.Contains(refresh)) nodes.Add(refresh);
            if (!nodes.Contains(gamble)) nodes.Add(gamble);
            foreach (var node in nodes)
            {
                if (node.Row != 1) continue;
                var serializedNode = new SerializedObject(node);
                var next = serializedNode.FindProperty("nextNodes");
                var links = new List<JourneyNodeAsset>(node.NextNodes);
                if (!links.Contains(refresh)) links.Add(refresh);
                if (!links.Contains(gamble)) links.Add(gamble);
                next.arraySize = links.Count;
                for (var i = 0; i < links.Count; i++) next.GetArrayElementAtIndex(i).objectReferenceValue = links[i];
                serializedNode.ApplyModifiedPropertiesWithoutUndo();
            }
            var serialized = new SerializedObject(map);
            var entries = serialized.FindProperty("nodes");
            entries.arraySize = nodes.Count;
            for (var i = 0; i < nodes.Count; i++) entries.GetArrayElementAtIndex(i).objectReferenceValue = nodes[i];
            var centers = new Dictionary<JourneyNodeAsset, Vector2>();
            foreach (var placement in map.NodePlacements) centers.Add(placement.Node, placement.NormalizedCenter);
            centers[map.GetNode("seedfall")] = new Vector2(0.19787234f, 0.15f);
            centers[refresh] = new Vector2(0.19787234f, 0.35f);
            centers[gamble] = new Vector2(0.19787234f, 0.55f);
            centers[map.GetNode("fox-tracks")] = new Vector2(0.19787234f, 0.76f);
            var positions = serialized.FindProperty("nodePlacements");
            positions.arraySize = nodes.Count;
            for (var i = 0; i < nodes.Count; i++)
            {
                positions.GetArrayElementAtIndex(i).FindPropertyRelative("node").objectReferenceValue = nodes[i];
                positions.GetArrayElementAtIndex(i).FindPropertyRelative("normalizedCenter").vector2Value = centers[nodes[i]];
            }
            serialized.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssets();
            Debug.Log("[Salty] Refresh and seed-cache playtest encounters added to Forest Edge.");
        }

        static JourneyNodeAsset PopulationEncounter(string folder, string file, string id, string title,
            string description, JourneyNodeKind kind, int column, JourneyNodeAsset next)
        {
            var path = folder + "/" + file + ".asset";
            var asset = AssetDatabase.LoadAssetAtPath<JourneyNodeAsset>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<JourneyNodeAsset>();
                AssetDatabase.CreateAsset(asset, path);
            }
            var serialized = new SerializedObject(asset);
            serialized.FindProperty("nodeId").stringValue = id;
            serialized.FindProperty("displayName").stringValue = title;
            serialized.FindProperty("description").stringValue = description;
            serialized.FindProperty("kind").enumValueIndex = (int)kind;
            serialized.FindProperty("row").intValue = 2;
            serialized.FindProperty("column").intValue = column;
            serialized.FindProperty("playableInPrototype").boolValue = true;
            var links = serialized.FindProperty("nextNodes");
            links.arraySize = 1;
            links.GetArrayElementAtIndex(0).objectReferenceValue = next;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return asset;
        }

        [MenuItem("Salty Game/Genome/Add Population Playtest Nodes")]
        public static void CreatePopulationGenomeNodes()
        {
            const string folder = "Assets/Data/TestData/Genome/RabbitSelectionPOC";
            var map = AssetDatabase.LoadAssetAtPath<SpeciesGenomeMapAsset>(folder + "/RabbitSelectionPOC.asset");
            if (map == null) throw new InvalidOperationException("Rabbit selection POC map is missing.");
            var nodes = new List<GenomeUpgradeAsset>(map.Upgrades);
            AddPopulationNode(folder, nodes, "StartingPopulation", "poc.hare.starting-population", "Founding Hares",
                "+5 Hares at the start of a new journey. A larger colony consumes more food. Does not refill each round.",
                SpeciesAttributeIds.StartingPopulation, 5);
            AddPopulationNode(folder, nodes, "PopulationPool", "poc.hare.population-pool", "Population Reserves",
                "+10 population pool capacity. Refreshes and ordinary population grants can restore Hares up to this limit. Births may exceed it.",
                SpeciesAttributeIds.PopulationPoolCapacity, 10);
            var serialized = new SerializedObject(map);
            var upgrades = serialized.FindProperty("upgrades");
            upgrades.arraySize = nodes.Count;
            for (var i = 0; i < nodes.Count; i++) upgrades.GetArrayElementAtIndex(i).objectReferenceValue = nodes[i];
            serialized.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssets();
            Debug.Log("[Salty] Rabbit population playtest nodes are available in the POC catalog.");
        }

        static void AddPopulationNode(string folder, List<GenomeUpgradeAsset> nodes, string fileName,
            string id, string name, string description, string attribute, int value)
        {
            var path = folder + "/" + fileName + ".asset";
            var asset = AssetDatabase.LoadAssetAtPath<GenomeUpgradeAsset>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<GenomeUpgradeAsset>();
                var serialized = new SerializedObject(asset);
                serialized.FindProperty("upgradeId").stringValue = id;
                serialized.FindProperty("displayName").stringValue = name;
                serialized.FindProperty("description").stringValue = description;
                serialized.FindProperty("targetSpeciesId").stringValue = "hare";
                serialized.FindProperty("branchName").stringValue = "Warren";
                serialized.FindProperty("rabbitDataPrice").longValue = 1;
                serialized.FindProperty("capacityPoints").intValue = 2;
                var modifiers = serialized.FindProperty("modifiers");
                modifiers.arraySize = 1;
                modifiers.GetArrayElementAtIndex(0).FindPropertyRelative("attributeId").stringValue = attribute;
                modifiers.GetArrayElementAtIndex(0).FindPropertyRelative("signedValue").floatValue = value;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                AssetDatabase.CreateAsset(asset, path);
            }
            if (!nodes.Contains(asset)) nodes.Add(asset);
        }

        [MenuItem("Salty Game/Simulation/Create Species Scenarios")]
        public static void Create()
        {
            EnsureFolder("Assets/Data");
            EnsureFolder(Root);
            EnsureFolder(SpeciesRoot);
            EnsureFolder(ScenarioRoot);

            var plants = new[]
            {
                CreatePlant("fern", 20f, 0.01f),
                CreatePlant("reed", 18f, 0.01f),
            };
            var herbivores = new[]
            {
                CreateHerbivore("hare", "plant", 2.2f, 6, 0.04f, 16, 96, 24, 2, 3, 2, 0f),
                CreateHerbivore("deer", "fern", 1.4f, 20, 0.2f),
                CreateHerbivore("snail", "reed", 0.55f, 10, 0.3f),
                CreateHerbivore("beetle", "reed", 1.8f, 12, 0.25f),
            };
            var carnivores = new[]
            {
                CreateCarnivore("fox", "hare", 1.0f, 120, 0.03f, 80, 240, 120, 6),
                CreateCarnivore("wolf", "deer", 0.8f, 36, 0.02f),
                CreateCarnivore("owl", "snail", 0.9f, 28, 0.02f),
                CreateCarnivore("stoat", "beetle", 1.0f, 28, 0.02f),
            };

            var grass = CreateLegacyPlant();
            CreateScenario("ForestEdge", new[]
            {
                Entry(grass, 0.4f), Entry(herbivores[0], 0f, 22), Entry(carnivores[0], 0f, 4),
            });
            CreateScenario("Wetland", new[]
            {
                Entry(plants[1], 0.34f), Entry(herbivores[2], 0.02f),
                Entry(herbivores[3], 0.02f), Entry(carnivores[2], 0.004f),
                Entry(carnivores[3], 0.004f),
            });
            CreateScenario("OpenRange", new[]
            {
                Entry(plants[0], 0.42f), Entry(plants[1], 0.34f),
                Entry(herbivores[1], 0.02f), Entry(herbivores[3], 0.02f),
                Entry(carnivores[1], 0.004f),
            });

            // Validation-only scenario: its fingerprint is compared to the legacy defaults.
            var legacyHerbivore = CreateLegacyHerbivore();
            var legacyCarnivore = CreateLegacyCarnivore();
            CreateScenario("BaselineParity", new[]
            {
                Entry(grass, 0.4f), Entry(legacyHerbivore, 0.02f),
                Entry(legacyCarnivore, 0.004f),
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
                Array.Empty<Vector2Int>(), Array.Empty<Vector2Int>(), 0.1f, 0, 0, 0, wilt, 1,
                reserve, 0f, 1, -1, 0);
            return asset;
        }

        static PlantSpeciesDefinitionAsset CreateLegacyPlant()
        {
            var asset = GetOrCreate<PlantSpeciesDefinitionAsset>("plant.asset");
            SetCommon(asset, "plant", 0f, Cardinal(), Cardinal(), 0,
                Cardinal(), Cardinal(), 0.03f, 0, 1, 0, 0.003f, 1,
                10f, 0f, 2, 0, 0);
            Set(asset, "dietPattern", Cardinal());
            Set(asset, "startingEnergy", 0);
            Set(asset, "intelligence", 0);
            return asset;
        }

        static HerbivoreSpeciesDefinitionAsset CreateLegacyHerbivore()
        {
            var asset = GetOrCreate<HerbivoreSpeciesDefinitionAsset>("herbivore.asset");
            SetCommon(asset, "herbivore", 1.5f, Cardinal(), Cardinal(), 1,
                Cardinal(), Moore(), 0.04f, 1, 16, 3, 0f, 2,
                0f, 0.05f, 8, 1, 5);
            Set(asset, "dietPattern", Moore());
            Set(asset, "dietTargetId", SpeciesIds.Plant.Value);
            Set(asset, "startingEnergy", 6);
            Set(asset, "forageBelowEnergy", 6);
            Set(asset, "maximumEnergy", 24);
            Set(asset, "litterMinimum", 1);
            Set(asset, "litterMaximum", 2);
            return asset;
        }

        static CarnivoreSpeciesDefinitionAsset CreateLegacyCarnivore()
        {
            var asset = GetOrCreate<CarnivoreSpeciesDefinitionAsset>("carnivore.asset");
            SetCommon(asset, "carnivore", 1.5f, Moore(), Moore(), 2,
                Cardinal(), Cardinal(), 0.03f, 1, 16, 3, 0f, 2,
                0f, 0f, 12, 1, 4);
            Set(asset, "dietPattern", Moore());
            Set(asset, "dietTargetId", SpeciesIds.Herbivore.Value);
            Set(asset, "startingEnergy", 48);
            Set(asset, "forageBelowEnergy", 16);
            Set(asset, "maximumEnergy", 96);
            return asset;
        }

        static HerbivoreSpeciesDefinitionAsset CreateHerbivore(
            string id,
            string dietTarget,
            float speed,
            int energy,
            float reproductionChance,
            int reproductionFoodRequired = 8,
            int energyValue = 8,
            int maximumEnergy = 18,
            int litterMaximum = 3,
            int maxReproductionGroupSize = 4,
            int crowdingMetabolismMultiplier = 2,
            float seedDropChance = 0.05f)
        {
            var asset = GetOrCreate<HerbivoreSpeciesDefinitionAsset>($"{id}.asset");
            SetCommon(asset, id, speed, Cardinal(), Array.Empty<Vector2Int>(), 0,
                Cardinal(), Moore(), reproductionChance, 1, 1, maxReproductionGroupSize, 0f, crowdingMetabolismMultiplier,
                0f, seedDropChance, 4, 1, 5);
            Set(asset, "dietTargetId", dietTarget);
            Set(asset, "attackAmount", 1);
            Set(asset, "startingEnergy", energy);
            Set(asset, "forageBelowEnergy", energy);
            Set(asset, "reproductionFoodRequired", reproductionFoodRequired);
            Set(asset, "energyValue", energyValue);
            Set(asset, "maximumEnergy", maximumEnergy);
            Set(asset, "litterMinimum", 1);
            Set(asset, "litterMaximum", litterMaximum);
            Set(asset, "visionRange", 5);
            Set(asset, "intelligence", 1);
            return asset;
        }

        static CarnivoreSpeciesDefinitionAsset CreateCarnivore(
            string id,
            string dietTarget,
            float speed,
            int energy,
            float reproductionChance,
            int reproductionFoodRequired = 16,
            int maximumEnergy = 96,
            int forageBelowEnergy = 16,
            int visionRange = 5)
        {
            var asset = GetOrCreate<CarnivoreSpeciesDefinitionAsset>($"{id}.asset");
            SetCommon(asset, id, speed, Moore(), Moore(), 2,
                Cardinal(), Cardinal(), reproductionChance, 1, 1, 3, 0f, 2,
                0f, 0f, 8, 1, 4);
            Set(asset, "dietTargetId", dietTarget);
            Set(asset, "startingEnergy", energy);
            Set(asset, "forageBelowEnergy", forageBelowEnergy);
            Set(asset, "reproductionFoodRequired", reproductionFoodRequired);
            Set(asset, "energyValue", 12);
            Set(asset, "maximumEnergy", maximumEnergy);
            Set(asset, "litterMinimum", 1);
            Set(asset, "litterMaximum", 1);
            Set(asset, "visionRange", visionRange);
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
            Set(asset, "crowdingMetabolismMultiplier", crowding);
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
            Set(asset, "width", id == "ForestEdge" ? 36 : 32);
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
            float startingProbability,
            int startingPopulation = 0)
        {
            return new ScenarioDefinitionAsset.SpeciesEntry(
                definition,
                startingProbability,
                startingPopulation);
        }

        private static Vector2Int[] Cardinal() =>
            new List<Vector2Int>(SpeciesRuleDefaults.CreateCardinalPattern().Offsets).ToArray();

        private static Vector2Int[] Moore() =>
            new List<Vector2Int>(SpeciesRuleDefaults.CreateMoorePattern().Offsets).ToArray();

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
                        entry.FindPropertyRelative("startingPopulation").intValue = definitions[i].StartingPopulation;
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
