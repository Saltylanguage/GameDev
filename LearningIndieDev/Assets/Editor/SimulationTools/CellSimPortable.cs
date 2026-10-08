using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
#if !CELLSIM_STANDALONE
using UnityEditor;
#endif

namespace SaltyGame.EditorTools
{
    public static partial class CellularSimulationExperimentRunner
    {
        // This is also linked into CellSim.Batch. Production rules and the existing
        // experiment schedules remain the single simulation implementation.
        public static readonly string[] PortableSourceFiles = new[]
        {
            "Assets/Scripts/Game/Grid/Grid.cs",
            "Assets/Scripts/Game/Grid/GridPattern.cs",
            "Assets/Scripts/Game/Grid/GridPatternTemplates.cs",
            "Assets/Scripts/Game/Grid/GridSimulation.cs",
            "Assets/Scripts/Game/Terrain/TerrainDefinition.cs",
            "Assets/Scripts/Game/Terrain/TerrainId.cs",
            "Assets/Scripts/Game/Species/SpeciesArchetype.cs",
            "Assets/Scripts/Game/Species/SpeciesAttributeRegistry.cs",
            "Assets/Scripts/Game/Species/SpeciesAwarenessRules.cs",
            "Assets/Scripts/Game/Species/SpeciesCell.cs",
            "Assets/Scripts/Game/Species/SpeciesDefinition.cs",
            "Assets/Scripts/Game/Species/SpeciesGenomeContract.cs",
            "Assets/Scripts/Game/Species/SpeciesId.cs",
            "Assets/Scripts/Game/Species/SpeciesProgression.cs",
            "Assets/Scripts/Game/Species/SpeciesRole.cs",
            "Assets/Scripts/Game/Species/SpeciesRuleDefaults.cs",
            "Assets/Scripts/Game/Species/SpeciesRules.cs",
            "Assets/Scripts/Game/Species/SpeciesUpgrade.cs",
            "Assets/Scripts/Game/Species/SpeciesUpgradeAsset.cs",
            "Assets/Scripts/Game/Species/SpeciesUpgradeContract.cs",
            "Assets/Scripts/Game/Species/SpeciesUpgradeLoadoutFingerprint.cs",
            "Assets/Scripts/Game/Species/SpeciesUpgradePredictionInputAdapter.cs",
            "Assets/Scripts/Game/Simulation/AlphaOffspringRule.cs",
            "Assets/Scripts/Game/Simulation/CellularSimData.cs",
            "Assets/Scripts/Game/Simulation/CellularSimDataFingerprint.cs",
            "Assets/Scripts/Game/Simulation/SimulationRunResult.cs",
            "Assets/Scripts/Game/Simulation/SpeciesAttackOpportunity.cs",
            "Assets/Scripts/Game/Simulation/SpeciesExperimentalOptions.cs",
            "Assets/Scripts/Game/Simulation/SpeciesInitialGridFactory.cs",
            "Assets/Scripts/Game/Simulation/SpeciesNavigation.cs",
            "Assets/Scripts/Game/Simulation/SpeciesPairedSimulationRunner.cs",
            "Assets/Scripts/Game/Simulation/SpeciesPerception.cs",
            "Assets/Scripts/Game/Simulation/SpeciesSimulation.cs",
            "Assets/Scripts/Game/Simulation/SpeciesSimulationMetrics.cs",
            "Assets/Scripts/Game/Simulation/SpeciesSimulationRunner.cs",
            "Assets/Editor/SimulationTools/CellularSimulationExperimentRunner.cs",
            "Assets/Editor/SimulationTools/SimulationReportSerialization.cs",
            "Assets/Editor/SimulationTools/CellSimPortable.cs"
        };

        public static readonly string[] PortableScientificArguments = new[]
        {
            PlayerSpeciesArgument, UpgradeIdArgument, UpgradeSequenceArgument,
            UpgradeAssetSequenceArgument, UpgradeAssetCatalogPathArgument,
            UpgradeValueOverrideArgument, CombatModeArgument, AttackOpportunityModeArgument,
            ExperimentalFeaturesArgument, FoxAttackCooldownTicksArgument,
            PreContactAvoidanceChanceArgument, CoupledSpeciesResponsesArgument,
            WrapEdgesArgument, GridWidthArgument, GridHeightArgument, StartingPopulationsArgument,
            RunTicksArgument, PhaseLengthTicksArgument, PhaseUpgradeScheduleArgument,
            PhaseUpgradeAssetScheduleArgument, MutationPolicyArgument, "-harePurchasePolicy",
            MutationPolicyEarlyFirstChoiceArgument, RunDurationArgument, StepIntervalArgument, "-speciesStats"
        };

        public static string PortableSourceHash(Func<string, string> readSource)
        {
            var builder = new StringBuilder();
            foreach (var path in PortableSourceFiles.OrderBy(path => Path.GetFileName(path), StringComparer.Ordinal))
            {
                var source = readSource(path).Replace("\r\n", "\n");
                builder.Append(Path.GetFileName(path)).Append('\0').Append(source.Length)
                    .Append('\0').Append(source).Append('\0');
            }
            return PortableHash(builder.ToString());
        }

        static string PortableHash(string value)
        {
            using (var sha = SHA256.Create())
            {
                return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(value)))
                    .Replace("-", "").ToLowerInvariant();
            }
        }

        static string PortableStateDigest(Grid<SpeciesCell> cells)
        {
            // Entity labels are process-wide. Compare their live graph by grid
            // position; a target absent from the live grid is represented by -1.
            var positions = new Dictionary<long, int>();
            for (var y = 0; y < cells.Height; y++)
                for (var x = 0; x < cells.Width; x++)
                    if (cells.GetCell(x, y).IsCreature)
                        positions[cells.GetCell(x, y).EntityId] = x + y * cells.Width + 1;
            var fields = typeof(SpeciesCell).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .OrderBy(field => field.Name, StringComparer.Ordinal).ToArray();
            var builder = new StringBuilder();
            builder.Append(cells.Width).Append('|').Append(cells.Height).Append('|').Append(cells.WrapEdges);
            for (var y = 0; y < cells.Height; y++)
                for (var x = 0; x < cells.Width; x++)
                {
                    var cell = cells.GetCell(x, y);
                    foreach (var field in fields)
                    {
                        var value = field.GetValue(cell);
                        if (field.Name == "<EntityId>k__BackingField" || field.Name == "<TrackingTargetEntityId>k__BackingField")
                        {
                            var id = (long)value;
                            value = id == 0 ? 0 : positions.TryGetValue(id, out var position) ? position : -1;
                        }
                        var text = value is float number
                            ? BitConverter.ToInt32(BitConverter.GetBytes(number), 0).ToString(CultureInfo.InvariantCulture)
                            : Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
                        builder.Append(field.Name).Append(':').Append(text.Length).Append(':').Append(text).Append('|');
                    }
                }
            return PortableHash(builder.ToString());
        }

        public static string RunPortableSeed(PortableSnapshot snapshot, string[] arguments, int seed, bool detailed, CellularSimData resolvedData = null)
        {
            ValidatePortableArguments(arguments);
            var options = CommandLineOptions.Parse(arguments.Concat(new[]
            {
                SeedStartArgument, seed.ToString(CultureInfo.InvariantCulture), SeedCountArgument, "1"
            }).ToArray());
            if (options.AttackOpportunityMode == SpeciesAttackOpportunityMode.PairedLockstepDiagnostic)
                throw new ArgumentException("Paired lockstep requires two linked arms; use the existing Editor runner for that mode.");
            options.IncludeFinalStateDigest = true;
            options.CaptureDetailedEvents = detailed;
            var data = ApplyPortableStats(ApplyOverrides(resolvedData ?? snapshot.Restore(), options), arguments);
            if (options.AuthoredUpgradeCatalogPath != snapshot.catalogPath
                && (options.AuthoredUpgradeLoadout != null || options.PhaseAuthoredUpgradeSchedule != null))
                throw new ArgumentException("Authored upgrade catalog differs from the frozen export.");
            var authored = options.AuthoredUpgradeLoadout == null ? null : snapshot.ResolveUpgrades(options.AuthoredUpgradeLoadout);
            var authoredPhases = ResolvePortablePhases(snapshot, options);
            var report = CreateReport(data, options, "portable", GetExperimentalOptions(options), authored, authoredPhases);
            var run = report.runs[0];
            run.populationTrajectory = CreatePopulationTrajectory(run.populationHistory);
            // Use the existing harness to retain complete phase/policy semantics.
            // Digest is requested by the portable option, rather than legacy runs.
            if (!detailed)
            {
                run.populationHistory = new[] { run.populationHistory[0], run.populationHistory[run.populationHistory.Length - 1] };
                run.behaviorTransitions = null;
                run.trackedBehavior = null;
                run.deathEvents = null;
                run.combatRolls = null;
                run.combatCooldownSuppressions = null;
                foreach (var phase in run.phaseResults ?? Array.Empty<SimulationPhaseResultRecord>())
                {
                    phase.behaviorTransitions = null;
                    phase.trackedBehavior = null;
                    phase.deathEvents = null;
                    phase.combatRolls = null;
                    phase.combatCooldownSuppressions = null;
                }
            }
            return JsonUtility.ToJson(run);
        }

        public static void ValidatePortableArguments(string[] arguments)
        {
            if (arguments == null || arguments.Length % 2 != 0)
                throw new ArgumentException("Scientific arguments must be option/value pairs.");
            var seen = new HashSet<string>(StringComparer.Ordinal);
            for (var index = 0; index < arguments.Length; index += 2)
                if (!PortableScientificArguments.Contains(arguments[index]) || !seen.Add(arguments[index])
                    || arguments[index + 1] == null)
                    throw new ArgumentException("Unknown, duplicate or missing scientific argument: " + arguments[index]);
        }

        public static string PortableReportHeader(PortableSnapshot snapshot, string[] arguments, int seedStart, int seedCount)
        {
            ValidatePortableConfiguration(snapshot, arguments);
            var options = CommandLineOptions.Parse(arguments.Concat(new[] { SeedStartArgument, seedStart.ToString(CultureInfo.InvariantCulture),
                SeedCountArgument, seedCount.ToString(CultureInfo.InvariantCulture), ScenarioPathArgument, snapshot.scenarioPath }).ToArray());
            var data = ApplyPortableStats(ApplyOverrides(snapshot.Restore(), options), arguments);
            var authored = options.AuthoredUpgradeLoadout == null ? null : snapshot.ResolveUpgrades(options.AuthoredUpgradeLoadout);
            return JsonUtility.ToJson(CreateReport(data, options, "portable", GetExperimentalOptions(options), authored,
                ResolvePortablePhases(snapshot, options), true));
        }

        public static void ValidatePortableConfiguration(PortableSnapshot snapshot, string[] arguments)
        {
            ValidatePortableArguments(arguments);
            var options = CommandLineOptions.Parse(arguments);
            var data = ApplyPortableStats(ApplyOverrides(snapshot.Restore(), options), arguments);
            GetExperimentalOptions(options);
            if (!data.SpeciesRules.ContainsKey(new SpeciesId(options.PlayerSpeciesId)))
                throw new ArgumentException("Player species is absent from the frozen scenario.");
            if (options.AttackOpportunityMode == SpeciesAttackOpportunityMode.PairedLockstepDiagnostic)
                throw new ArgumentException("Use the Editor runner for paired lockstep diagnostics.");
            if ((options.AuthoredUpgradeLoadout != null || options.PhaseAuthoredUpgradeSchedule != null)
                && options.AuthoredUpgradeCatalogPath != snapshot.catalogPath)
                throw new ArgumentException("Authored upgrade catalog differs from the frozen export.");
            if (options.AuthoredUpgradeLoadout != null)
                ApplySnapshotLoadout(data, options.PlayerSpeciesId, snapshot.ResolveUpgrades(options.AuthoredUpgradeLoadout));
            else
                ApplySnapshotLoadout(data, options.PlayerSpeciesId,
                    CreateLegacyUpgradeSnapshots(options.UpgradeLoadout, options.PlayerSpeciesId, options.UpgradeValueOverride));
            if (options.PhaseUpgradeSchedule != null)
                foreach (var phase in options.PhaseUpgradeSchedule)
                    ApplySnapshotLoadout(data, options.PlayerSpeciesId, CreateLegacyUpgradeSnapshots(phase, options.PlayerSpeciesId));
            foreach (var phase in ResolvePortablePhases(snapshot, options) ?? Array.Empty<SpeciesUpgradeSnapshot[]>())
                ApplySnapshotLoadout(data, options.PlayerSpeciesId, phase);
        }

        static SpeciesUpgradeSnapshot[][] ResolvePortablePhases(PortableSnapshot snapshot, CommandLineOptions options)
        {
            return options.PhaseAuthoredUpgradeSchedule == null ? null : ResolveAuthoredPhaseSchedule(
                options.PhaseAuthoredUpgradeSchedule, options.AuthoredUpgradeCatalogPath, options.PlayerSpeciesId,
                (ids, catalog) => snapshot.ResolveUpgrades(ids.ToArray()));
        }

        // These are data fields for the existing registered stats, not a gameplay
        // expression language. All values are applied together before validation.
        static readonly Dictionary<string, string> PortableStatFields = new Dictionary<string, string>
        {
            { SpeciesAttributeIds.MovementSpeed, nameof(RuleData.movementSpeed) },
            { SpeciesAttributeIds.AttackAmount, nameof(RuleData.attackAmount) },
            { SpeciesAttributeIds.AttackModifier, nameof(RuleData.attackModifier) },
            { SpeciesAttributeIds.DamageAmount, nameof(RuleData.damageAmount) },
            { SpeciesAttributeIds.BlockAmount, nameof(RuleData.blockAmount) },
            { SpeciesAttributeIds.ReproductionNeighborCount, nameof(RuleData.reproductionNeighborCount) },
            { SpeciesAttributeIds.ReproductionChance, nameof(RuleData.reproductionChance) },
            { SpeciesAttributeIds.ReproductionFoodRequired, nameof(RuleData.reproductionFoodRequired) },
            { SpeciesAttributeIds.MaxReproductionGroupSize, nameof(RuleData.maxReproductionGroupSize) },
            { SpeciesAttributeIds.StartingEnergy, nameof(RuleData.startingEnergy) },
            { SpeciesAttributeIds.ForageBelowEnergy, nameof(RuleData.forageBelowEnergy) },
            { SpeciesAttributeIds.WiltChance, nameof(RuleData.wiltChance) },
            { SpeciesAttributeIds.CrowdingMetabolismMultiplier, nameof(RuleData.crowdingMetabolismMultiplier) },
            { SpeciesAttributeIds.StartingFoodReserve, nameof(RuleData.startingFoodReserve) },
            { SpeciesAttributeIds.SeedDropChance, nameof(RuleData.seedDropChance) },
            { SpeciesAttributeIds.EnergyValue, nameof(RuleData.energyValue) },
            { SpeciesAttributeIds.Metabolism, nameof(RuleData.metabolism) },
            { SpeciesAttributeIds.VisionRange, nameof(RuleData.visionRange) },
            { SpeciesAttributeIds.Intelligence, nameof(RuleData.intelligence) },
            { SpeciesAttributeIds.MaximumEnergy, nameof(RuleData.maximumEnergy) },
            { SpeciesAttributeIds.LitterMinimum, nameof(RuleData.litterMinimum) },
            { SpeciesAttributeIds.LitterMaximum, nameof(RuleData.litterMaximum) },
            { SpeciesAttributeIds.DigestionEnergyBonus, nameof(RuleData.digestionEnergyBonus) },
            { SpeciesAttributeIds.CrowdingTolerance, nameof(RuleData.crowdingTolerance) },
            { SpeciesAttributeIds.CrowdingEnergyReduction, nameof(RuleData.crowdingEnergyReduction) },
            { SpeciesAttributeIds.FleeMovementSpeedBonus, nameof(RuleData.fleeMovementSpeedBonus) },
            { SpeciesAttributeIds.TrackingPersistenceSteps, nameof(RuleData.trackingPersistenceSteps) },
        };

        public static CellularSimData ApplyPortableStats(CellularSimData data, string[] arguments)
        {
            var text = CommandLineOptions.GetOptionalValue(arguments, "-speciesStats");
            if (text == null) return data;
            var seen = new HashSet<string>(StringComparer.Ordinal);
            var edited = new Dictionary<SpeciesId, RuleData>();
            var requested = new List<(SpeciesId species, FieldInfo field, object value)>();
            foreach (var entry in text.Split(','))
            {
                var parts = entry.Split(new[] { ':', '=' });
                if (parts.Length != 3 || !seen.Add(parts[0] + ":" + parts[1])
                    || !SpeciesAttributeRegistry.TryGet(parts[1], out var definition)
                    || !PortableStatFields.TryGetValue(parts[1], out var fieldName))
                    throw new ArgumentException("Use unique species:registered-attribute=value entries: " + entry);
                var species = new SpeciesId(parts[0]);
                if (!data.SpeciesRules.TryGetValue(species, out var rules)) throw new ArgumentException("Unknown stat species: " + parts[0]);
                object value;
                if (definition.ValueKind == SpeciesAttributeValueKind.Integer)
                    value = int.Parse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture);
                else
                {
                    var number = float.Parse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture);
                    if (float.IsNaN(number) || float.IsInfinity(number)) throw new ArgumentException("Stat must be finite: " + entry);
                    value = number;
                }
                if (!edited.TryGetValue(species, out var dto)) edited[species] = dto = RuleData.Create(rules);
                var field = typeof(RuleData).GetField(fieldName);
                field.SetValue(dto, value);
                requested.Add((species, field, value));
            }
            foreach (var pair in edited) data = data.WithSpeciesRules(pair.Key, pair.Value.Restore());
            foreach (var item in requested)
                if (!item.value.Equals(item.field.GetValue(RuleData.Create(data.SpeciesRules[item.species]))))
                    throw new ArgumentException("Requested stat was normalized by the rules constructor: " + item.species + ":" + item.field.Name);
            return data;
        }

        [Serializable]
        public sealed class PopulationTrajectory
        {
            public string speciesId;
            public int first, last, min, max, observedTicks, positiveTicks, firstZeroTick;
            public long populationTickIntegral;
        }

        static PopulationTrajectory[] CreatePopulationTrajectory(SimulationPopulationSnapshotRecord[] history)
        {
            var samples = history.GroupBy(sample => sample.tick).Select(group => group.Last()).ToArray();
            return samples[0].species.Select(species =>
            {
                var values = samples.Select(sample => sample.species.First(s => s.speciesId == species.speciesId).population).ToArray();
                var zero = Array.FindIndex(values, value => value == 0);
                return new PopulationTrajectory { speciesId = species.speciesId, first = values[0], last = values[values.Length - 1],
                    min = values.Min(), max = values.Max(), observedTicks = samples.Length - 1,
                    positiveTicks = values.Skip(1).Count(value => value > 0), firstZeroTick = zero < 0 ? -1 : samples[zero].tick,
                    populationTickIntegral = values.Skip(1).Sum(value => (long)value) };
            }).ToArray();
        }

        static void AddPortableSpeciesStatLines(ExperimentRun report, SimulationRunState run,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules)
        {
            var herbivores = rules.Where(pair => pair.Value.Role == SpeciesRole.Herbivore).Select(pair => pair.Key).OrderBy(s => s.Value).ToArray();
            var predators = rules.Where(pair => pair.Value.Role == SpeciesRole.Carnivore).Select(pair => pair.Key).OrderBy(s => s.Value).ToArray();
            report.herbivoreStatLines = herbivores.Select(s => SimulationReportSerialization.CreateHerbivoreStatLine(run, s)).ToArray();
            report.predatorStatLines = predators.Select(s => SimulationReportSerialization.CreatePredatorStatLine(run, s)).ToArray();
            for (var i = 0; i < run.PhaseResults.Count; i++)
            {
                var phase = run.PhaseResults[i];
                report.phaseResults[i].herbivoreStatLines = herbivores.Select(s => SimulationReportSerialization.CreateHerbivoreStatLine(
                    phase.Metrics, s, phase.OpeningPopulation.GetCount(s), phase.ClosingPopulation.GetCount(s))).ToArray();
                report.phaseResults[i].predatorStatLines = predators.Select(s => SimulationReportSerialization.CreatePredatorStatLine(
                    phase.Metrics, s, phase.OpeningPopulation.GetCount(s), phase.ClosingPopulation.GetCount(s))).ToArray();
            }
        }

#if !CELLSIM_STANDALONE
        public static void ExportPortableFromCommandLine()
        {
            try
            {
                var arguments = Environment.GetCommandLineArgs();
                var options = CommandLineOptions.Parse(arguments);
                if (string.IsNullOrWhiteSpace(options.ScenarioPath))
                    throw new ArgumentException("An explicit -scenarioPath is required for portable exports.");
                var path = CommandLineOptions.GetOptionalValue(arguments, "-portableOutput");
                path = GetRequiredOutputPath(path);
                var scientific = new List<string>();
                foreach (var name in PortableScientificArguments)
                {
                    var value = CommandLineOptions.GetOptionalValue(arguments, name);
                    if (value != null) { scientific.Add(name); scientific.Add(value); }
                }
                var data = LoadSimulationData(options.ScenarioPath, out var temporary);
                if (temporary != null) throw new InvalidOperationException("Portable export requires an authored asset.");
                var snapshot = PortableSnapshot.Create(data);
                snapshot.scenarioPath = options.ScenarioPath;
                snapshot.arguments = scientific.ToArray();
                snapshot.catalogPath = string.IsNullOrWhiteSpace(options.AuthoredUpgradeCatalogPath)
                    ? SpeciesUpgradePredictionInputAdapter.ProductionCatalogPath : options.AuthoredUpgradeCatalogPath;
                if (!AssetDatabase.IsValidFolder(snapshot.catalogPath))
                    throw new ArgumentException("Upgrade catalog folder is absent: " + snapshot.catalogPath);
                snapshot.upgrades = AssetDatabase.FindAssets("t:SpeciesUpgradeAsset", new[] { snapshot.catalogPath })
                    .Select(AssetDatabase.GUIDToAssetPath).OrderBy(value => value, StringComparer.Ordinal)
                    .Select(assetPath => AssetDatabase.LoadAssetAtPath<SpeciesUpgradeAsset>(assetPath))
                    .Select(asset =>
                    {
                        if (!asset.TryCreateSnapshot(out var upgrade, out var message)) throw new InvalidOperationException(message);
                        return UpgradeData.Create(upgrade);
                    }).ToArray();
                var project = Directory.GetParent(Application.dataPath).FullName;
                snapshot.sourceHash = PortableSourceHash(relative => File.ReadAllText(Path.Combine(project, relative)));
                snapshot.sourceCommit = CommandLineOptions.GetOptionalValue(arguments, "-portableSourceCommit") ?? "unspecified";
                snapshot.Restore();
                ValidatePortableConfiguration(snapshot, snapshot.arguments);
                if (options.SeedCount > 1000) throw new ArgumentException("Export reference is limited to 1000 seeds.");
                if (File.Exists(path) || File.Exists(Path.ChangeExtension(path, ".reference.jsonl")))
                    throw new IOException("Snapshot or reference already exists; choose a new output path.");
                // Never overwrite frozen inputs or reference evidence.
                using (var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
                using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
                    writer.Write(JsonUtility.ToJson(snapshot, true));
                using (var stream = new FileStream(Path.ChangeExtension(path, ".reference.jsonl"), FileMode.CreateNew, FileAccess.Write))
                using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
                    for (var index = 0; index < options.SeedCount; index++)
                        writer.WriteLine(RunPortableSeed(snapshot, snapshot.arguments, checked(options.SeedStart + index), false, data));
                Debug.Log("[Salty] Exported portable CellSim inputs and reference: " + path);
                if (Application.isBatchMode) EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                if (Application.isBatchMode) EditorApplication.Exit(1);
                throw;
            }
        }
#endif

        [Serializable]
        public sealed class PortableSnapshot
        {
            public int schemaVersion = 1;
            public string sourceHash, sourceCommit, scenarioPath, fingerprint, catalogPath;
            public string[] arguments;
            public int width, height, maxPopulation, minPopulation;
            public bool wrapEdges;
            public float runDurationSeconds, stepInterval;
            public SpeciesData[] species;
            public TerrainData[] terrain;
            public AlphaData[] alpha;
            public UpgradeData[] upgrades = Array.Empty<UpgradeData>();

            public static PortableSnapshot Create(CellularSimData data)
            {
                return new PortableSnapshot
                {
                    fingerprint = data.Fingerprint, width = data.Width, height = data.Height,
                    wrapEdges = data.WrapEdges, maxPopulation = data.MaxPopulation, minPopulation = data.MinPopulation,
                    runDurationSeconds = data.RunDurationSeconds, stepInterval = data.StepInterval,
                    species = data.SpeciesRules.OrderBy(entry => entry.Key.Value, StringComparer.Ordinal).Select(entry => new SpeciesData
                    {
                        id = entry.Key.Value, rules = RuleData.Create(entry.Value),
                        hasProbability = data.StartingProbabilities.ContainsKey(entry.Key),
                        probability = data.StartingProbabilities.TryGetValue(entry.Key, out var probability) ? probability : 0f,
                        hasPopulation = data.StartingPopulations.ContainsKey(entry.Key),
                        population = data.StartingPopulations.TryGetValue(entry.Key, out var population) ? population : 0
                    }).ToArray(),
                    terrain = data.TerrainDefinitions.OrderBy(entry => entry.Key.Value, StringComparer.Ordinal).Select(entry => new TerrainData
                    {
                        id = entry.Key.Value, isPassable = entry.Value.IsPassable, movementCost = entry.Value.MovementCost,
                        providesResource = entry.Value.ProvidesResource, color = entry.Value.PresentationColor,
                        regrowthPerTick = entry.Value.RegrowthPerTick, growthIntervalSeconds = entry.Value.GrowthIntervalSeconds
                    }).ToArray(),
                    alpha = data.AlphaOffspringRules.Select(entry => new AlphaData
                    {
                        species = entry.Key.Value, chance = entry.Value.Chance,
                        healthBonus = entry.Value.HealthBonus, energyBonus = entry.Value.EnergyBonus
                    }).ToArray()
                };
            }

            public CellularSimData Restore()
            {
                if (schemaVersion != 1 || species == null || terrain == null || alpha == null)
                    throw new ArgumentException("Unsupported or incomplete portable snapshot.");
                var data = new CellularSimData(width, height,
                    species.Where(entry => entry.hasProbability).ToDictionary(entry => new SpeciesId(entry.id), entry => entry.probability),
                    species.ToDictionary(entry => new SpeciesId(entry.id), entry => entry.rules.Restore()),
                    runDurationSeconds, stepInterval, maxPopulation, minPopulation,
                    terrain.ToDictionary(entry => new TerrainId(entry.id), entry => new TerrainDefinition(new TerrainId(entry.id),
                        entry.isPassable, entry.movementCost, entry.providesResource, entry.color, entry.regrowthPerTick, entry.growthIntervalSeconds)),
                    alpha.ToDictionary(entry => new SpeciesId(entry.species), entry => new AlphaOffspringRule(new SpeciesId(entry.species),
                        entry.chance, entry.healthBonus, entry.energyBonus)),
                    species.Where(entry => entry.hasPopulation).ToDictionary(entry => new SpeciesId(entry.id), entry => entry.population), wrapEdges);
                if (data.Fingerprint != fingerprint) throw new InvalidOperationException("Snapshot round-trip fingerprint mismatch.");
                return data;
            }

            public SpeciesUpgradeSnapshot[] ResolveUpgrades(string[] ids)
            {
                var catalog = upgrades.ToDictionary(upgrade => upgrade.id, StringComparer.Ordinal);
                if (ids.Distinct(StringComparer.Ordinal).Count() != ids.Length) throw new ArgumentException("Duplicate authored upgrades.");
                return ids.Select(id => catalog.TryGetValue(id, out var upgrade) ? upgrade.Restore()
                    : throw new ArgumentException("Upgrade is absent from the frozen catalog: " + id)).ToArray();
            }
        }

        [Serializable] public sealed class SpeciesData
        {
            public string id;
            public float probability;
            public int population;
            public bool hasProbability, hasPopulation;
            public RuleData rules;
        }
        [Serializable] public sealed class TerrainData
        {
            public string id;
            public bool isPassable, providesResource;
            public float movementCost, regrowthPerTick, growthIntervalSeconds;
            public Color color;
        }
        [Serializable] public sealed class AlphaData
        { public string species; public float chance; public int healthBonus, energyBonus; }
        [Serializable] public sealed class StateRuleData
        { public SpeciesBehaviorState state; public int minimumDurationTicks; public bool stopsMovement; }
        [Serializable] public sealed class ModifierData
        { public string id; public float value; }
        [Serializable] public sealed class UpgradeData
        {
            public string id, displayName, description, targetSpecies;
            public int cost, populationToAdd;
            public float avoidanceBonus;
            public string[] prerequisites, exclusions;
            public ModifierData[] modifiers;
            public static UpgradeData Create(SpeciesUpgradeSnapshot value) => new UpgradeData
            {
                id = value.Id, displayName = value.DisplayName, description = value.Description,
                targetSpecies = value.TargetSpecies.Value, cost = value.Cost, populationToAdd = value.PopulationToAdd,
                avoidanceBonus = value.PreContactAvoidanceChanceBonus,
                prerequisites = value.PrerequisiteUpgradeIds.ToArray(), exclusions = value.ExcludedUpgradeIds.ToArray(),
                modifiers = value.Modifiers.Select(modifier => new ModifierData { id = modifier.AttributeId, value = modifier.SignedValue }).ToArray()
            };
            public SpeciesUpgradeSnapshot Restore() => new SpeciesUpgradeSnapshot(id, displayName, description,
                new SpeciesId(targetSpecies), cost, modifiers.Select(modifier => new SpeciesUpgradeModifier(modifier.id, modifier.value)),
                prerequisites, exclusions, populationToAdd, avoidanceBonus);
        }

                [Serializable]
        public sealed class RuleData
        {
            public float movementSpeed;
            public Vector2Int[] movementPattern;
            public Vector2Int[] attackPattern;
            public int attackAmount;
            public Vector2Int[] blockPattern;
            public int blockAmount;
            public Vector2Int[] dietPattern;
            public string dietTarget;
            public Vector2Int[] reproductionPattern;
            public int reproductionNeighborCount;
            public float reproductionChance;
            public int reproductionFoodRequired;
            public int maxReproductionGroupSize;
            public int startingEnergy;
            public float wiltChance;
            public int crowdingMetabolismMultiplier;
            public float startingFoodReserve;
            public float seedDropChance;
            public int energyValue;
            public int metabolism;
            public int visionRange;
            public int intelligence;
            public SpeciesRole role;
            public int forageBelowEnergy;
            public int maximumEnergy;
            public int litterMinimum;
            public int litterMaximum;
            public int attackModifier;
            public int damageAmount;
            public float digestionEnergyBonus;
            public int crowdingTolerance;
            public float fleeMovementSpeedBonus;
            public int trackingPersistenceSteps;
            public StateRuleData[] behaviorStateRules;
            public bool foragesUntilFull;
            public int energyLossIntervalTicks;
            public float forageThresholdFraction;
            public float matingEnergyThresholdFraction;
            public float matingEnergyCostFraction;
            public bool distributeMatingEnergyToOffspring;
            public float crowdingEnergyReduction;
            public static RuleData Create(SpeciesRules value)
            {
                return new RuleData
                {
                    movementSpeed = value.MovementSpeed,
                    movementPattern = value.MovementPattern.Offsets.ToArray(),
                    attackPattern = value.AttackPattern.Offsets.ToArray(),
                    attackAmount = value.AttackAmount,
                    blockPattern = value.BlockPattern.Offsets.ToArray(),
                    blockAmount = value.BlockAmount,
                    dietPattern = value.DietPattern.Offsets.ToArray(),
                    dietTarget = value.DietTargetId.HasValue ? value.DietTargetId.Value.Value : null,
                    reproductionPattern = value.ReproductionPattern.Offsets.ToArray(),
                    reproductionNeighborCount = value.ReproductionNeighborCount,
                    reproductionChance = value.ReproductionChance,
                    reproductionFoodRequired = value.ReproductionFoodRequired,
                    maxReproductionGroupSize = value.MaxReproductionGroupSize,
                    startingEnergy = value.StartingEnergy,
                    wiltChance = value.WiltChance,
                    crowdingMetabolismMultiplier = value.CrowdingMetabolismMultiplier,
                    startingFoodReserve = value.StartingFoodReserve,
                    seedDropChance = value.SeedDropChance,
                    energyValue = value.EnergyValue,
                    metabolism = value.Metabolism,
                    visionRange = value.Awareness.VisionRange,
                    intelligence = value.Awareness.Intelligence,
                    role = value.Role,
                    forageBelowEnergy = value.ForageBelowEnergy,
                    maximumEnergy = value.MaximumEnergy,
                    litterMinimum = value.LitterMinimum,
                    litterMaximum = value.LitterMaximum,
                    attackModifier = value.AttackModifier,
                    damageAmount = value.DamageAmount,
                    digestionEnergyBonus = value.DigestionEnergyBonus,
                    crowdingTolerance = value.CrowdingTolerance,
                    fleeMovementSpeedBonus = value.FleeMovementSpeedBonus,
                    trackingPersistenceSteps = value.TrackingPersistenceSteps,
                    behaviorStateRules = value.BehaviorStateRules.Select(rule => new StateRuleData { state = rule.State, minimumDurationTicks = rule.MinimumDurationTicks, stopsMovement = rule.StopsMovement }).ToArray(),
                    foragesUntilFull = value.ForagesUntilFull,
                    energyLossIntervalTicks = value.EnergyLossIntervalTicks,
                    forageThresholdFraction = value.ForageThresholdFraction,
                    matingEnergyThresholdFraction = value.MatingEnergyThresholdFraction,
                    matingEnergyCostFraction = value.MatingEnergyCostFraction,
                    distributeMatingEnergyToOffspring = value.DistributeMatingEnergyToOffspring,
                    crowdingEnergyReduction = value.CrowdingEnergyReduction,
                };
            }
            public SpeciesRules Restore() => new SpeciesRules(
                movementSpeed: movementSpeed,
                movementPattern: new GridPattern(movementPattern),
                attackPattern: new GridPattern(attackPattern),
                attackAmount: attackAmount,
                blockPattern: new GridPattern(blockPattern),
                blockAmount: blockAmount,
                dietPattern: new GridPattern(dietPattern),
                dietTarget: string.IsNullOrEmpty(dietTarget) ? (SpeciesId?)null : new SpeciesId(dietTarget),
                reproductionPattern: new GridPattern(reproductionPattern),
                reproductionNeighborCount: reproductionNeighborCount,
                reproductionChance: reproductionChance,
                reproductionFoodRequired: reproductionFoodRequired,
                maxReproductionGroupSize: maxReproductionGroupSize,
                startingEnergy: startingEnergy,
                wiltChance: wiltChance,
                crowdingMetabolismMultiplier: crowdingMetabolismMultiplier,
                startingFoodReserve: startingFoodReserve,
                seedDropChance: seedDropChance,
                energyValue: energyValue,
                metabolism: metabolism,
                awareness: new SpeciesAwarenessRules(visionRange, intelligence),
                role: role,
                forageBelowEnergy: forageBelowEnergy,
                maximumEnergy: maximumEnergy,
                litterMinimum: litterMinimum,
                litterMaximum: litterMaximum,
                attackModifier: attackModifier,
                damageAmount: damageAmount,
                digestionEnergyBonus: digestionEnergyBonus,
                crowdingTolerance: crowdingTolerance,
                fleeMovementSpeedBonus: fleeMovementSpeedBonus,
                trackingPersistenceSteps: trackingPersistenceSteps,
                behaviorStateRules: behaviorStateRules.Select(rule => new SpeciesBehaviorStateRule(rule.state, rule.minimumDurationTicks, rule.stopsMovement)).ToArray(),
                foragesUntilFull: foragesUntilFull,
                energyLossIntervalTicks: energyLossIntervalTicks,
                forageThresholdFraction: forageThresholdFraction,
                matingEnergyThresholdFraction: matingEnergyThresholdFraction,
                matingEnergyCostFraction: matingEnergyCostFraction,
                distributeMatingEnergyToOffspring: distributeMatingEnergyToOffspring,
                crowdingEnergyReduction: crowdingEnergyReduction);
        }

    }
}
