using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace SaltyGame
{
    public enum SpeciesPreviewState
    {
        Ready,
        Running,
        Paused,
        Rewards,
        Results,
    }

    public sealed class SpeciesSimulationPreview : MonoBehaviour
    {
        public static event Action<SpeciesSimulationPreview, SimulationRunState> RunCompleted;

        enum PatternPreset
        {
            Cardinal,
            Moore,
        }

        enum DietTargetOption
        {
            None,
            Plant,
            Herbivore,
            Carnivore,
        }

        [System.Serializable]
        sealed class SpeciesRuleDraft
        {
            public SpeciesRuleDraft()
            {
            }

            public SpeciesRuleDraft(SpeciesRules rules)
            {
                MovementSpeed = rules.MovementSpeed;
                MovementSpeedText = FormatFloat(rules.MovementSpeed);
                MovementPattern = GetPatternPreset(rules.MovementPattern);
                MovementEnabled = rules.MovementSpeed > 0f;
                AttackAmount = rules.AttackAmount;
                AttackAmountText = rules.AttackAmount.ToString(CultureInfo.InvariantCulture);
                AttackPattern = GetPatternPreset(rules.AttackPattern);
                AttackEnabled = rules.AttackAmount > 0;
                BlockAmount = rules.BlockAmount;
                BlockAmountText = rules.BlockAmount.ToString(CultureInfo.InvariantCulture);
                BlockPattern = GetPatternPreset(rules.BlockPattern);
                DietPattern = GetPatternPreset(rules.DietPattern);
                ReproductionPattern = GetPatternPreset(rules.ReproductionPattern);
                DietTargetSpecies = rules.DietTargetId;
                DietTarget = GetDietTargetOption(rules.DietTargetId);
                ReproductionChance = rules.ReproductionChance;
                ReproductionChanceText = FormatFloat(rules.ReproductionChance);
                ReproductionNeighborCount = rules.ReproductionNeighborCount;
                ReproductionNeighborCountText = rules.ReproductionNeighborCount.ToString(CultureInfo.InvariantCulture);
                ReproductionFoodRequired = rules.ReproductionFoodRequired;
                ReproductionFoodRequiredText = rules.ReproductionFoodRequired.ToString(CultureInfo.InvariantCulture);
                MaxReproductionGroupSize = rules.MaxReproductionGroupSize;
                MaxReproductionGroupSizeText = rules.MaxReproductionGroupSize.ToString(CultureInfo.InvariantCulture);
                StartingEnergy = rules.StartingEnergy;
                StartingEnergyText = rules.StartingEnergy.ToString(CultureInfo.InvariantCulture);
                MaximumEnergy = rules.MaximumEnergy;
                MaximumEnergyText = rules.MaximumEnergy.ToString(CultureInfo.InvariantCulture);
                LitterMinimum = rules.LitterMinimum;
                LitterMinimumText = rules.LitterMinimum.ToString(CultureInfo.InvariantCulture);
                LitterMaximum = rules.LitterMaximum;
                LitterMaximumText = rules.LitterMaximum.ToString(CultureInfo.InvariantCulture);
                ForageBelowEnergy = rules.ForageBelowEnergy;
                ForageBelowEnergyText = rules.ForageBelowEnergy.ToString(CultureInfo.InvariantCulture);
                EnergyValue = rules.EnergyValue;
                EnergyValueText = rules.EnergyValue.ToString(CultureInfo.InvariantCulture);
                Metabolism = rules.Metabolism;
                MetabolismText = rules.Metabolism.ToString(CultureInfo.InvariantCulture);
                VisionRange = rules.Awareness.VisionRange;
                VisionRangeText = rules.Awareness.VisionRange.ToString(CultureInfo.InvariantCulture);
                Intelligence = rules.Awareness.Intelligence;
                IntelligenceText = rules.Awareness.Intelligence.ToString(CultureInfo.InvariantCulture);
                ReproductionEnabled = rules.ReproductionChance > 0f;
                WiltChance = rules.WiltChance;
                WiltChanceText = FormatFloat(rules.WiltChance);
                WiltEnabled = rules.WiltChance > 0f;
                CrowdingEnergyPenalty = rules.CrowdingEnergyPenalty;
                CrowdingEnergyPenaltyText = rules.CrowdingEnergyPenalty.ToString(CultureInfo.InvariantCulture);
                StartingFoodReserve = rules.StartingFoodReserve;
                StartingFoodReserveText = FormatFloat(rules.StartingFoodReserve);
                SeedDropChance = rules.SeedDropChance;
                SeedDropChanceText = FormatFloat(rules.SeedDropChance);
                SeedDropEnabled = rules.SeedDropChance > 0f;
                Role = rules.Role;
            }

            public bool MovementEnabled;
            public float MovementSpeed;
            public string MovementSpeedText;
            public PatternPreset MovementPattern;
            public bool AttackEnabled;
            public int AttackAmount;
            public string AttackAmountText;
            public PatternPreset AttackPattern;
            public int BlockAmount;
            public string BlockAmountText;
            public PatternPreset BlockPattern;
            public PatternPreset DietPattern;
            public PatternPreset ReproductionPattern;
            public DietTargetOption DietTarget;
            public SpeciesId? DietTargetSpecies;
            public bool ReproductionEnabled;
            public float ReproductionChance;
            public string ReproductionChanceText;
            public int ReproductionNeighborCount;
            public string ReproductionNeighborCountText;
            public int ReproductionFoodRequired;
            public string ReproductionFoodRequiredText;
            public int MaxReproductionGroupSize;
            public string MaxReproductionGroupSizeText;
            public int StartingEnergy;
            public string StartingEnergyText;
            public int MaximumEnergy;
            public string MaximumEnergyText;
            public int LitterMinimum;
            public string LitterMinimumText;
            public int LitterMaximum;
            public string LitterMaximumText;
            public int ForageBelowEnergy;
            public string ForageBelowEnergyText;
            public int EnergyValue;
            public string EnergyValueText;
            public int Metabolism;
            public string MetabolismText;
            public int VisionRange;
            public string VisionRangeText;
            public int Intelligence;
            public string IntelligenceText;
            public bool WiltEnabled;
            public float WiltChance;
            public string WiltChanceText;
            public int CrowdingEnergyPenalty;
            public string CrowdingEnergyPenaltyText;
            public float StartingFoodReserve;
            public string StartingFoodReserveText;
            public bool SeedDropEnabled;
            public float SeedDropChance;
            public string SeedDropChanceText;
            public SpeciesRole Role;
        }

        [System.Serializable]
        sealed class SavedSettings
        {
            public int width;
            public int height;
            public int seed;
            public bool randomizeSeedOnStart;
            public float plantProbability;
            public float herbivoreProbability;
            public float carnivoreProbability;
            public float runDurationSeconds;
            public float stepInterval;
            public int maxPopulation;
            public int minPopulation;
            public SpeciesRuleDraft plant;
            public SpeciesRuleDraft herbivore;
            public SpeciesRuleDraft carnivore;
        }

        [Header("Grid")]
        [SerializeField, Min(1)] int width = 32;
        [SerializeField, Min(1)] int height = 32;
        [SerializeField] int seed = 12345;
        [SerializeField] bool randomizeSeedOnStart = true;
        [SerializeField] string playerSpeciesKey = "herbivore";
        [SerializeField, Range(0f, 1f)] float plantProbability = 0.4f;
        [SerializeField, Range(0f, 1f)] float herbivoreProbability = 0.02f;
        [SerializeField, Range(0f, 1f)] float carnivoreProbability = 0.004f;
        [SerializeField, Min(0)] int maxPopulation;
        [SerializeField, Min(0)] int minPopulation;

        [Header("Authored Scenarios")]
        [SerializeField] List<ScenarioDefinitionAsset> scenarioOptions = new List<ScenarioDefinitionAsset>();
        [SerializeField, Min(-1)] int selectedScenarioIndex = -1;

        [Header("Run")]
        [SerializeField, Min(1f)] float runDurationSeconds = 20f;
        [SerializeField, Min(0.01f)] float stepInterval = 0.1f;

        [Header("Bev Experimental Features")]
        [SerializeField] bool bevExperimentalFeaturesEnabled;
        [SerializeField, Min(0)] int foxAttackCooldownTicks;

        readonly SpeciesUpgrade[] rewardOptions =
        {
            SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.FasterMovementId),
            SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.StrongerAttackId),
            SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.StrongerBlockId),
        };

        SpeciesId playerSpecies;
        readonly List<SpeciesId> rosterSpecies = new List<SpeciesId>();
        readonly List<SpeciesId> playableSpecies = new List<SpeciesId>();
        IReadOnlyDictionary<SpeciesId, SpeciesRules> rules;
        SpeciesProgression progression;
        SpeciesSimulationRunner runner;
        SimulationRunResult result;
        SpeciesUpgrade selectedUpgrade;
        SpeciesPreviewState previewState;
        string rewardMessage;
        Dictionary<SpeciesId, SpeciesRuleDraft> ruleDrafts;
        float tickTimer;
        int runNumber;
        bool rewardGranted;
        bool sessionStarted;
        string settingsMessage;

        const string DefaultSettingsKey = "SaltyGame.SpeciesSimulationPreview.DefaultSettings.v3";

        public SimulationRunState Run => runner?.Run;
        public SpeciesProgression Progression => progression;
        public int PurchasedUpgradeCount => progression?.PurchasedUpgradeCount ?? 0;
        public SpeciesPreviewState State => previewState;
        public int GridWidth => width;
        public int GridHeight => height;
        public int BaseSeed => seed;
        public int MaximumPopulation => maxPopulation;
        public int MinimumPopulation => minPopulation;
        public float RunDurationSeconds => runDurationSeconds;
        public float StepInterval => stepInterval;
        public float PlantProbability => plantProbability;
        public float HerbivoreProbability => herbivoreProbability;
        public float CarnivoreProbability => carnivoreProbability;
        public bool RandomizeSeedOnStart => randomizeSeedOnStart;
        public bool BevExperimentalFeaturesEnabled => bevExperimentalFeaturesEnabled;
        public int FoxAttackCooldownTicks => foxAttackCooldownTicks;
        public IReadOnlyDictionary<SpeciesId, SpeciesRules> ActiveSpeciesRules => rules;
        public IReadOnlyList<ScenarioDefinitionAsset> ScenarioOptions => scenarioOptions;
        public IReadOnlyList<SpeciesId> RosterSpecies => rosterSpecies;
        public IReadOnlyList<SpeciesId> PlayableSpecies => playableSpecies;
        public SpeciesId PlayerSpecies => playerSpecies;
        public int SelectedScenarioIndex => selectedScenarioIndex;
        public ScenarioDefinitionAsset SelectedScenario => GetSelectedScenario();
        public string SettingsMessage => settingsMessage ?? string.Empty;
        public bool SettingsEditable => previewState == SpeciesPreviewState.Ready && !sessionStarted;
        public void ConfigureScenarioOptions(IReadOnlyList<ScenarioDefinitionAsset> options, int initialSelection = -1)
        {
            scenarioOptions = options == null
                ? new List<ScenarioDefinitionAsset>()
                : new List<ScenarioDefinitionAsset>(options);
            selectedScenarioIndex = Mathf.Clamp(initialSelection, -1, scenarioOptions.Count - 1);

            if (previewState == SpeciesPreviewState.Ready && !sessionStarted)
            {
                ResetToStart();
            }
        }

        public bool TrySelectScenario(int scenarioIndex, out string validationMessage)
        {
            validationMessage = string.Empty;
            if (!SettingsEditable)
            {
                validationMessage = "Scenarios can only be changed before a session starts.";
                settingsMessage = validationMessage;
                return false;
            }

            if (scenarioIndex < -1 || scenarioIndex >= scenarioOptions.Count)
            {
                validationMessage = "The selected scenario is not available.";
                settingsMessage = validationMessage;
                return false;
            }

            selectedScenarioIndex = scenarioIndex;
            ResetToStart();
            settingsMessage = SelectedScenario == null
                ? "Legacy defaults selected."
                : $"Scenario '{SelectedScenario.name}' selected.";
            return true;
        }

        public bool TrySetPlayerSpecies(string speciesKey, out string validationMessage)
        {
            validationMessage = string.Empty;
            if (!SettingsEditable)
            {
                validationMessage = "The player species can only be changed before a session starts.";
                settingsMessage = validationMessage;
                return false;
            }

            if (string.IsNullOrWhiteSpace(speciesKey))
            {
                validationMessage = "A player species is required.";
                settingsMessage = validationMessage;
                return false;
            }

            var selectedSpecies = new SpeciesId(speciesKey.Trim());
            if (!rules.TryGetValue(selectedSpecies, out var selectedRules) || selectedRules.IsPlant)
            {
                validationMessage = $"The selected player species '{speciesKey}' is not playable in this scenario.";
                settingsMessage = validationMessage;
                return false;
            }

            playerSpecies = selectedSpecies;
            playerSpeciesKey = selectedSpecies.Value;
            progression = new SpeciesProgression(new SpeciesDefinition(playerSpecies, selectedRules));
            PrepareNextRun();
            settingsMessage = $"Player species '{playerSpecies.Value}' selected.";
            validationMessage = settingsMessage;
            return true;
        }

        void Awake()
        {
            playerSpecies = new SpeciesId(string.IsNullOrWhiteSpace(playerSpeciesKey)
                ? SpeciesIds.Herbivore.Value
                : playerSpeciesKey);
            ruleDrafts = CreateRuleDrafts(SpeciesRuleDefaults.Create());
            LoadSavedSettings();
            ApplySelectedScenario();
            ResetToStart();
        }

        void Update()
        {
            if (runner == null || runner.Run.Status != SimulationRunStatus.Running)
            {
                return;
            }

            tickTimer += Time.deltaTime;
            while (tickTimer >= runner.StepSeconds && runner.Run.Status == SimulationRunStatus.Running)
            {
                tickTimer -= runner.StepSeconds;
                runner.AdvanceOneTick();
            }

            if (runner.Run.Status == SimulationRunStatus.Complete && !rewardGranted)
            {
                result = SimulationRunResults.Create(runner.Run);
                RunCompleted?.Invoke(this, runner.Run);
                progression.AddCurrency(result.CurrencyEarned);
                rewardGranted = true;
                previewState = SpeciesPreviewState.Rewards;
            }
        }

        public void StartSimulation()
        {
            if (runner != null && runner.Run.Status == SimulationRunStatus.Ready)
            {
                if (!sessionStarted)
                {
                    if (SelectedScenario == null)
                    {
                        rules = CreateRulesFromDrafts();
                    }

                    progression = new SpeciesProgression(new SpeciesDefinition(
                        playerSpecies,
                        rules[playerSpecies]));
                    runNumber = 0;
                    PrepareNextRun();
                }

                runner.Start();
                sessionStarted = true;
                previewState = SpeciesPreviewState.Running;
            }
        }

        public bool TryApplyGlobalSettings(
            string widthValue,
            string heightValue,
            string seedValue,
            string maximumPopulationValue,
            string minimumPopulationValue,
            string runDurationValue,
            string stepIntervalValue,
            string plantProbabilityValue,
            string herbivoreProbabilityValue,
            string carnivoreProbabilityValue,
            bool randomizeSeed,
            out string validationMessage)
        {
            validationMessage = string.Empty;
            if (previewState != SpeciesPreviewState.Ready || sessionStarted)
            {
                validationMessage = "Settings can only be changed before a session starts.";
                settingsMessage = validationMessage;
                return false;
            }

            if (!TryParseInt(widthValue, "Grid width", out var parsedWidth)
                || !TryParseInt(heightValue, "Grid height", out var parsedHeight)
                || !TryParseInt(seedValue, "Base seed", out var parsedSeed)
                || !TryParseInt(maximumPopulationValue, "Maximum population", out var parsedMaximumPopulation)
                || !TryParseInt(minimumPopulationValue, "Minimum population", out var parsedMinimumPopulation)
                || !TryParseFloat(runDurationValue, "Run duration", out var parsedRunDuration)
                || !TryParseFloat(stepIntervalValue, "Step interval", out var parsedStepInterval)
                || !TryParseFloat(plantProbabilityValue, "Plant probability", out var parsedPlantProbability)
                || !TryParseFloat(herbivoreProbabilityValue, "Herbivore probability", out var parsedHerbivoreProbability)
                || !TryParseFloat(carnivoreProbabilityValue, "Carnivore probability", out var parsedCarnivoreProbability))
            {
                validationMessage = settingsMessage;
                return false;
            }

            width = Mathf.Max(1, parsedWidth);
            height = Mathf.Max(1, parsedHeight);
            seed = parsedSeed;
            maxPopulation = Mathf.Max(0, parsedMaximumPopulation);
            minPopulation = Mathf.Max(0, parsedMinimumPopulation);
            runDurationSeconds = Mathf.Max(1f, parsedRunDuration);
            stepInterval = Mathf.Max(0.01f, parsedStepInterval);
            plantProbability = Mathf.Clamp01(parsedPlantProbability);
            herbivoreProbability = Mathf.Clamp01(parsedHerbivoreProbability);
            carnivoreProbability = Mathf.Clamp01(parsedCarnivoreProbability);
            randomizeSeedOnStart = randomizeSeed;
            settingsMessage = "Global settings applied to the next run.";
            PrepareNextRun();
            validationMessage = settingsMessage;
            return true;
        }

        public bool TryApplyExperimentalFeatures(
            bool enabled,
            string foxAttackCooldownValue,
            out string validationMessage)
        {
            validationMessage = string.Empty;
            if (!SettingsEditable)
            {
                validationMessage = "Experimental features can only be changed before a session starts.";
                settingsMessage = validationMessage;
                return false;
            }

            if (!TryParseInt(foxAttackCooldownValue, "Fox attack cooldown", out var parsedCooldown))
            {
                validationMessage = settingsMessage;
                return false;
            }

            if (parsedCooldown < 0)
            {
                validationMessage = "Fox attack cooldown must be zero or greater.";
                settingsMessage = validationMessage;
                return false;
            }

            bevExperimentalFeaturesEnabled = enabled;
            foxAttackCooldownTicks = parsedCooldown;
            settingsMessage = enabled
                ? $"Bev experimental features enabled: opposed-roll combat, herbivore stat line, fox cooldown {foxAttackCooldownTicks} ticks."
                : "Bev experimental features disabled; legacy combat retained.";
            PrepareNextRun();
            validationMessage = settingsMessage;
            return true;
        }

        public SpeciesRuleEditValues GetSpeciesRuleEditValues(SpeciesId species)
        {
            if (!ruleDrafts.TryGetValue(species, out var draft))
            {
                throw new ArgumentOutOfRangeException(nameof(species), species, "Unknown species.");
            }

            return new SpeciesRuleEditValues
            {
                MovementEnabled = draft.MovementEnabled,
                MovementSpeed = FormatFloat(draft.MovementSpeed),
                MovementPattern = (int)draft.MovementPattern,
                AttackEnabled = draft.AttackEnabled,
                AttackAmount = draft.AttackAmount.ToString(CultureInfo.InvariantCulture),
                AttackPattern = (int)draft.AttackPattern,
                BlockAmount = draft.BlockAmount.ToString(CultureInfo.InvariantCulture),
                BlockPattern = (int)draft.BlockPattern,
                DietTarget = (int)draft.DietTarget,
                DietPattern = (int)draft.DietPattern,
                ReproductionPattern = (int)draft.ReproductionPattern,
                ReproductionEnabled = draft.ReproductionEnabled,
                ReproductionChance = FormatFloat(draft.ReproductionChance),
                ReproductionNeighborCount = draft.ReproductionNeighborCount.ToString(CultureInfo.InvariantCulture),
                ReproductionFoodRequired = draft.ReproductionFoodRequired.ToString(CultureInfo.InvariantCulture),
                MaxReproductionGroupSize = draft.MaxReproductionGroupSize.ToString(CultureInfo.InvariantCulture),
                StartingEnergy = draft.StartingEnergy.ToString(CultureInfo.InvariantCulture),
                MaximumEnergy = draft.MaximumEnergy.ToString(CultureInfo.InvariantCulture),
                LitterMinimum = draft.LitterMinimum.ToString(CultureInfo.InvariantCulture),
                LitterMaximum = draft.LitterMaximum.ToString(CultureInfo.InvariantCulture),
                ForageBelowEnergy = draft.ForageBelowEnergy.ToString(CultureInfo.InvariantCulture),
                EnergyValue = draft.EnergyValue.ToString(CultureInfo.InvariantCulture),
                Metabolism = draft.Metabolism.ToString(CultureInfo.InvariantCulture),
                VisionRange = draft.VisionRange.ToString(CultureInfo.InvariantCulture),
                Intelligence = draft.Intelligence.ToString(CultureInfo.InvariantCulture),
                WiltEnabled = draft.WiltEnabled,
                WiltChance = FormatFloat(draft.WiltChance),
                CrowdingEnergyPenalty = draft.CrowdingEnergyPenalty.ToString(CultureInfo.InvariantCulture),
                StartingFoodReserve = FormatFloat(draft.StartingFoodReserve),
                SeedDropEnabled = draft.SeedDropEnabled,
                SeedDropChance = FormatFloat(draft.SeedDropChance),
            };
        }

        public bool TryApplySpeciesRuleEditValues(
            SpeciesId species,
            SpeciesRuleEditValues values,
            out string validationMessage)
        {
            validationMessage = string.Empty;
            if (previewState != SpeciesPreviewState.Ready || sessionStarted)
            {
                validationMessage = "Species rules can only be changed before a session starts.";
                settingsMessage = validationMessage;
                return false;
            }

            if (values == null || !ruleDrafts.TryGetValue(species, out var draft))
            {
                validationMessage = "The selected species is not available.";
                settingsMessage = validationMessage;
                return false;
            }

            if (!TryParseFloat(values.MovementSpeed, "Movement speed", out var movementSpeed)
                || !TryParseInt(values.AttackAmount, "Attack amount", out var attackAmount)
                || !TryParseInt(values.BlockAmount, "Block amount", out var blockAmount)
                || !TryParseFloat(values.ReproductionChance, "Reproduction chance", out var reproductionChance)
                || !TryParseInt(values.ReproductionNeighborCount, "Nearby mate requirement", out var reproductionNeighborCount)
                || !TryParseInt(values.ReproductionFoodRequired, "Energy transferred to offspring", out var reproductionFoodRequired)
                || !TryParseInt(values.MaxReproductionGroupSize, "Maximum group size", out var maxReproductionGroupSize)
                || !TryParseInt(values.StartingEnergy, "Starting energy", out var startingEnergy)
                || !TryParseInt(values.MaximumEnergy, "Maximum energy", out var maximumEnergy)
                || !TryParseInt(values.LitterMinimum, "Minimum litter size", out var litterMinimum)
                || !TryParseInt(values.LitterMaximum, "Maximum litter size", out var litterMaximum)
                || !TryParseInt(values.ForageBelowEnergy, "Forage energy threshold", out var forageBelowEnergy)
                || !TryParseInt(values.EnergyValue, "Energy value", out var energyValue)
                || !TryParseInt(values.Metabolism, "Metabolism", out var metabolism)
                || !TryParseInt(values.VisionRange, "Vision range", out var visionRange)
                || !TryParseInt(values.Intelligence, "Intelligence", out var intelligence)
                || !TryParseFloat(values.WiltChance, "Wilt chance", out var wiltChance)
                || !TryParseInt(values.CrowdingEnergyPenalty, "Crowding cost", out var crowdingEnergyPenalty)
                || !TryParseFloat(values.StartingFoodReserve, "Starting food reserve", out var startingFoodReserve)
                || !TryParseFloat(values.SeedDropChance, "Seed drop chance", out var seedDropChance))
            {
                validationMessage = settingsMessage;
                return false;
            }

            draft.MovementEnabled = values.MovementEnabled;
            draft.MovementSpeed = Mathf.Max(0f, movementSpeed);
            draft.MovementSpeedText = FormatFloat(draft.MovementSpeed);
            draft.MovementPattern = (PatternPreset)Mathf.Clamp(values.MovementPattern, 0, 1);
            draft.AttackEnabled = values.AttackEnabled;
            draft.AttackAmount = Mathf.Max(0, attackAmount);
            draft.AttackAmountText = draft.AttackAmount.ToString(CultureInfo.InvariantCulture);
            draft.AttackPattern = (PatternPreset)Mathf.Clamp(values.AttackPattern, 0, 1);
            draft.BlockAmount = Mathf.Max(0, blockAmount);
            draft.BlockAmountText = draft.BlockAmount.ToString(CultureInfo.InvariantCulture);
            draft.BlockPattern = (PatternPreset)Mathf.Clamp(values.BlockPattern, 0, 1);
            draft.DietTarget = (DietTargetOption)Mathf.Clamp(values.DietTarget, 0, 3);
            draft.DietTargetSpecies = ResolveDietTargetSpecies(draft.DietTarget);
            draft.DietPattern = (PatternPreset)Mathf.Clamp(values.DietPattern, 0, 1);
            draft.ReproductionPattern = (PatternPreset)Mathf.Clamp(values.ReproductionPattern, 0, 1);
            draft.ReproductionEnabled = values.ReproductionEnabled;
            draft.ReproductionChance = Mathf.Clamp01(reproductionChance);
            draft.ReproductionChanceText = FormatFloat(draft.ReproductionChance);
            draft.ReproductionNeighborCount = Mathf.Max(0, reproductionNeighborCount);
            draft.ReproductionNeighborCountText = draft.ReproductionNeighborCount.ToString(CultureInfo.InvariantCulture);
            draft.ReproductionFoodRequired = Mathf.Max(0, reproductionFoodRequired);
            draft.ReproductionFoodRequiredText = draft.ReproductionFoodRequired.ToString(CultureInfo.InvariantCulture);
            draft.MaxReproductionGroupSize = Mathf.Max(0, maxReproductionGroupSize);
            draft.MaxReproductionGroupSizeText = draft.MaxReproductionGroupSize.ToString(CultureInfo.InvariantCulture);
            draft.StartingEnergy = Mathf.Max(0, startingEnergy);
            draft.StartingEnergyText = draft.StartingEnergy.ToString(CultureInfo.InvariantCulture);
            draft.MaximumEnergy = Mathf.Max(0, maximumEnergy);
            draft.MaximumEnergyText = draft.MaximumEnergy.ToString(CultureInfo.InvariantCulture);
            draft.LitterMinimum = Mathf.Max(1, litterMinimum);
            draft.LitterMinimumText = draft.LitterMinimum.ToString(CultureInfo.InvariantCulture);
            draft.LitterMaximum = Mathf.Max(draft.LitterMinimum, litterMaximum);
            draft.LitterMaximumText = draft.LitterMaximum.ToString(CultureInfo.InvariantCulture);
            draft.ForageBelowEnergy = Mathf.Max(0, forageBelowEnergy);
            draft.ForageBelowEnergyText = draft.ForageBelowEnergy.ToString(CultureInfo.InvariantCulture);
            draft.EnergyValue = Mathf.Max(0, energyValue);
            draft.EnergyValueText = draft.EnergyValue.ToString(CultureInfo.InvariantCulture);
            draft.Metabolism = Mathf.Max(-1000, metabolism);
            draft.MetabolismText = draft.Metabolism.ToString(CultureInfo.InvariantCulture);
            draft.VisionRange = Mathf.Max(0, visionRange);
            draft.VisionRangeText = draft.VisionRange.ToString(CultureInfo.InvariantCulture);
            draft.Intelligence = Mathf.Max(0, intelligence);
            draft.IntelligenceText = draft.Intelligence.ToString(CultureInfo.InvariantCulture);
            draft.WiltEnabled = values.WiltEnabled;
            draft.WiltChance = Mathf.Clamp01(wiltChance);
            draft.WiltChanceText = FormatFloat(draft.WiltChance);
            draft.CrowdingEnergyPenalty = Mathf.Max(0, crowdingEnergyPenalty);
            draft.CrowdingEnergyPenaltyText = draft.CrowdingEnergyPenalty.ToString(CultureInfo.InvariantCulture);
            draft.StartingFoodReserve = Mathf.Max(0f, startingFoodReserve);
            draft.StartingFoodReserveText = FormatFloat(draft.StartingFoodReserve);
            draft.SeedDropEnabled = values.SeedDropEnabled;
            draft.SeedDropChance = Mathf.Clamp01(seedDropChance);
            draft.SeedDropChanceText = FormatFloat(draft.SeedDropChance);

            rules = CreateRulesFromDrafts();
            progression = new SpeciesProgression(new SpeciesDefinition(playerSpecies, rules[playerSpecies]));
            settingsMessage = $"{species.Value} rules applied to the next run.";
            PrepareNextRun();
            validationMessage = settingsMessage;
            return true;
        }

        public void PauseSimulation()
        {
            if (runner != null && runner.Run.Status == SimulationRunStatus.Running)
            {
                runner.Pause();
                previewState = SpeciesPreviewState.Paused;
            }
        }

        public void ResumeSimulation()
        {
            if (runner != null && runner.Run.Status == SimulationRunStatus.Paused)
            {
                runner.Resume();
                previewState = SpeciesPreviewState.Running;
            }
        }

        public void RestartSimulation()
        {
            if (runner == null
                || (runner.Run.Status != SimulationRunStatus.Running
                    && runner.Run.Status != SimulationRunStatus.Paused))
            {
                return;
            }

            runner.Restart();
            runner.Start();
            tickTimer = 0f;
            result = default;
            rewardGranted = false;
            selectedUpgrade = null;
            rewardMessage = string.Empty;
            previewState = SpeciesPreviewState.Running;
        }

        public void StopSimulation()
        {
            if (runner != null
                && (runner.Run.Status == SimulationRunStatus.Running
                    || runner.Run.Status == SimulationRunStatus.Paused))
            {
                ResetToStart();
            }
        }

        public bool CanPurchaseReward(int rewardIndex)
        {
            return previewState == SpeciesPreviewState.Rewards
                && progression != null
                && rewardIndex >= 0
                && rewardIndex < rewardOptions.Length
                && progression.Currency >= rewardOptions[rewardIndex].Cost;
        }

        public bool PurchaseReward(int rewardIndex)
        {
            if (!CanPurchaseReward(rewardIndex))
            {
                return false;
            }

            var upgrade = rewardOptions[rewardIndex];
            if (!progression.TryPurchase(upgrade))
            {
                return false;
            }

            selectedUpgrade = upgrade;
            previewState = SpeciesPreviewState.Results;
            rewardMessage = string.Empty;
            return true;
        }

        public void ContinueWithoutUpgrade()
        {
            if (previewState == SpeciesPreviewState.Rewards)
            {
                previewState = SpeciesPreviewState.Results;
            }
        }

        public void PlayNextSimulation()
        {
            if (previewState == SpeciesPreviewState.Results)
            {
                PrepareNextRun();
            }
        }

        public void ResetToStart()
        {
            if (randomizeSeedOnStart)
            {
                seed = Guid.NewGuid().GetHashCode();
            }

            ApplySelectedScenario();
            rules = CreateRulesFromDrafts();
            if (SelectedScenario != null)
            {
                var authoredData = SelectedScenario.CreateRuntimeData();
                rules = new Dictionary<SpeciesId, SpeciesRules>(authoredData.SpeciesRules);
                if (!rules.ContainsKey(playerSpecies))
                {
                    playerSpecies = FindPlayableSpecies(rules);
                    playerSpeciesKey = playerSpecies.Value;
                }
            }
            SyncRosterSpecies();
            progression = new SpeciesProgression(new SpeciesDefinition(
                playerSpecies,
                rules[playerSpecies]));
            runNumber = 0;
            sessionStarted = false;
            settingsMessage = string.Empty;
            PrepareNextRun();
        }

        public void SaveCurrentSettingsAsDefault()
        {
            var saved = new SavedSettings
            {
                width = width,
                height = height,
                seed = seed,
                randomizeSeedOnStart = randomizeSeedOnStart,
                plantProbability = plantProbability,
                herbivoreProbability = herbivoreProbability,
                carnivoreProbability = carnivoreProbability,
                runDurationSeconds = runDurationSeconds,
                stepInterval = stepInterval,
                maxPopulation = maxPopulation,
                minPopulation = minPopulation,
                plant = GetRuleDraftOrDefault(SpeciesIds.Plant),
                herbivore = GetRuleDraftOrDefault(SpeciesIds.Herbivore),
                carnivore = GetRuleDraftOrDefault(SpeciesIds.Carnivore),
            };

            PlayerPrefs.SetString(DefaultSettingsKey, JsonUtility.ToJson(saved));
            PlayerPrefs.Save();
            settingsMessage = "Current settings saved as the default.";
        }

        void LoadSavedSettings()
        {
            if (!PlayerPrefs.HasKey(DefaultSettingsKey))
            {
                return;
            }

            SavedSettings saved;
            try
            {
                saved = JsonUtility.FromJson<SavedSettings>(PlayerPrefs.GetString(DefaultSettingsKey));
            }
            catch (Exception)
            {
                PlayerPrefs.DeleteKey(DefaultSettingsKey);
                return;
            }
            if (saved == null)
            {
                return;
            }

            width = Mathf.Max(1, saved.width);
            height = Mathf.Max(1, saved.height);
            seed = saved.seed;
            randomizeSeedOnStart = saved.randomizeSeedOnStart;
            plantProbability = Mathf.Clamp01(saved.plantProbability);
            herbivoreProbability = Mathf.Clamp01(saved.herbivoreProbability);
            carnivoreProbability = Mathf.Clamp01(saved.carnivoreProbability);
            runDurationSeconds = Mathf.Max(1f, saved.runDurationSeconds);
            stepInterval = Mathf.Max(0.01f, saved.stepInterval);
            maxPopulation = Mathf.Max(0, saved.maxPopulation);
            minPopulation = Mathf.Max(0, saved.minPopulation);
            // Saved defaults may come from a different scenario. Do not add
            // canonical species that are not present in the active scenario.
            if (saved.plant != null && ruleDrafts.ContainsKey(SpeciesIds.Plant))
            {
                ruleDrafts[SpeciesIds.Plant] = saved.plant;
            }
            if (saved.herbivore != null && ruleDrafts.ContainsKey(SpeciesIds.Herbivore))
            {
                ruleDrafts[SpeciesIds.Herbivore] = saved.herbivore;
            }
            if (saved.carnivore != null && ruleDrafts.ContainsKey(SpeciesIds.Carnivore))
            {
                ruleDrafts[SpeciesIds.Carnivore] = saved.carnivore;
            }
        }

        SpeciesRuleDraft GetRuleDraftOrDefault(SpeciesId species)
        {
            if (ruleDrafts.TryGetValue(species, out var draft))
            {
                return draft;
            }

            return new SpeciesRuleDraft(SpeciesRuleDefaults.Create()[species]);
        }

        void PrepareNextRun()
        {
            var currentRules = new Dictionary<SpeciesId, SpeciesRules>(rules)
            {
                [playerSpecies] = progression?.CurrentRules ?? rules[playerSpecies],
            };
            rules = currentRules;
            var simulationData = CreateSimulationData();

            var run = new SimulationRunState(
                SpeciesInitialGridFactory.Create(simulationData, seed + runNumber),
                playerSpecies,
                seed + runNumber,
                simulationData.RunDurationSeconds);
            var experimentalOptions = bevExperimentalFeaturesEnabled
                ? new SpeciesExperimentalOptions(
                    SpeciesExperimentalOptions.BevExperimentalFeaturesId,
                    foxAttackCooldownTicks)
                : SpeciesExperimentalOptions.None;
            runner = new SpeciesSimulationRunner(
                run,
                simulationData,
                combatResolutionMode: bevExperimentalFeaturesEnabled
                    ? SpeciesCombatResolutionMode.OpposedRoll
                    : SpeciesCombatResolutionMode.LegacyFixedDamage,
                experimentalOptions: experimentalOptions);
            tickTimer = 0f;
            result = default;
            rewardGranted = false;
            selectedUpgrade = null;
            rewardMessage = string.Empty;
            previewState = SpeciesPreviewState.Ready;
            runNumber++;
        }

        Dictionary<SpeciesId, SpeciesRuleDraft> CreateRuleDrafts(
            IReadOnlyDictionary<SpeciesId, SpeciesRules> sourceRules)
        {
            var drafts = new Dictionary<SpeciesId, SpeciesRuleDraft>();
            foreach (var entry in sourceRules)
            {
                var draft = new SpeciesRuleDraft(entry.Value);
                draft.DietTarget = GetDietTargetOption(entry.Value.DietTargetId, sourceRules);
                drafts[entry.Key] = draft;
            }

            return drafts;
        }

        IReadOnlyDictionary<SpeciesId, SpeciesRules> CreateRulesFromDrafts()
        {
            var result = new Dictionary<SpeciesId, SpeciesRules>();
            foreach (var entry in ruleDrafts)
            {
                var draft = entry.Value;
                result[entry.Key] = new SpeciesRules(
                    movementSpeed: draft.MovementEnabled ? draft.MovementSpeed : 0f,
                    movementPattern: GetPattern(draft.MovementPattern),
                    attackPattern: GetPattern(draft.AttackPattern),
                    attackAmount: draft.AttackEnabled ? draft.AttackAmount : 0,
                    blockPattern: GetPattern(draft.BlockPattern),
                    blockAmount: draft.BlockAmount,
                    dietPattern: GetPattern(draft.DietPattern),
                    dietTarget: draft.DietTargetSpecies ?? ResolveDietTargetSpecies(draft.DietTarget),
                    reproductionPattern: GetPattern(draft.ReproductionPattern),
                    reproductionNeighborCount: draft.ReproductionEnabled ? draft.ReproductionNeighborCount : 0,
                    reproductionChance: draft.ReproductionEnabled ? draft.ReproductionChance : 0f,
                    reproductionFoodRequired: draft.ReproductionEnabled ? draft.ReproductionFoodRequired : 0,
                    maxReproductionGroupSize: draft.ReproductionEnabled ? draft.MaxReproductionGroupSize : 0,
                    startingEnergy: draft.StartingEnergy,
                    wiltChance: draft.WiltEnabled ? draft.WiltChance : 0f,
                    crowdingEnergyPenalty: draft.CrowdingEnergyPenalty,
                    startingFoodReserve: draft.StartingFoodReserve,
                    seedDropChance: draft.SeedDropEnabled ? draft.SeedDropChance : 0f,
                    energyValue: draft.EnergyValue,
                    metabolism: draft.Metabolism,
                    awareness: new SpeciesAwarenessRules(draft.VisionRange, draft.Intelligence),
                    role: draft.Role,
                    forageBelowEnergy: draft.ForageBelowEnergy,
                    maximumEnergy: draft.MaximumEnergy,
                    litterMinimum: draft.LitterMinimum,
                    litterMaximum: draft.LitterMaximum);
            }

            return result;
        }

        static GridPattern GetPattern(PatternPreset preset)
        {
            return preset == PatternPreset.Moore
                ? SpeciesRuleDefaults.CreateMoorePattern()
                : SpeciesRuleDefaults.CreateCardinalPattern();
        }

        static PatternPreset GetPatternPreset(GridPattern pattern)
        {
            return pattern.Count >= 8 ? PatternPreset.Moore : PatternPreset.Cardinal;
        }

        static SpeciesId? GetDietTarget(DietTargetOption target)
        {
            switch (target)
            {
                case DietTargetOption.Plant:
                    return SpeciesIds.Plant;
                case DietTargetOption.Herbivore:
                    return SpeciesIds.Herbivore;
                case DietTargetOption.Carnivore:
                    return SpeciesIds.Carnivore;
                default:
                    return null;
            }
        }

        static DietTargetOption GetDietTargetOption(SpeciesId? target)
        {
            if (!target.HasValue)
            {
                return DietTargetOption.None;
            }

            if (target.Value == SpeciesIds.Plant)
            {
                return DietTargetOption.Plant;
            }

            if (target.Value == SpeciesIds.Herbivore)
            {
                return DietTargetOption.Herbivore;
            }

            if (target.Value == SpeciesIds.Carnivore)
            {
                return DietTargetOption.Carnivore;
            }

            return DietTargetOption.None;
        }

        static DietTargetOption GetDietTargetOption(
            SpeciesId? target,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> sourceRules)
        {
            var option = GetDietTargetOption(target);
            if (option != DietTargetOption.None || !target.HasValue || sourceRules == null)
            {
                return option;
            }

            if (sourceRules.TryGetValue(target.Value, out var targetRules))
            {
                switch (targetRules.Role)
                {
                    case SpeciesRole.Plant:
                        return DietTargetOption.Plant;
                    case SpeciesRole.Herbivore:
                        return DietTargetOption.Herbivore;
                    case SpeciesRole.Carnivore:
                        return DietTargetOption.Carnivore;
                }
            }

            return DietTargetOption.None;
        }

        SpeciesId? ResolveDietTargetSpecies(DietTargetOption target)
        {
            var canonical = GetDietTarget(target);
            if (canonical.HasValue && ruleDrafts.ContainsKey(canonical.Value))
            {
                return canonical;
            }

            SpeciesRole? role = null;
            switch (target)
            {
                case DietTargetOption.Plant:
                    role = SpeciesRole.Plant;
                    break;
                case DietTargetOption.Herbivore:
                    role = SpeciesRole.Herbivore;
                    break;
                case DietTargetOption.Carnivore:
                    role = SpeciesRole.Carnivore;
                    break;
            }

            if (role.HasValue)
            {
                foreach (var entry in ruleDrafts)
                {
                    if (entry.Value.Role == role.Value)
                    {
                        return entry.Key;
                    }
                }
            }

            return canonical;
        }

        static string FormatFloat(float value)
        {
            return value.ToString("0.###", CultureInfo.InvariantCulture);
        }

        bool TryParseInt(string text, string label, out int value)
        {
            if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
            {
                return true;
            }

            settingsMessage = $"{label} must be a whole number.";
            return false;
        }

        bool TryParseFloat(string text, string label, out float value)
        {
            if (float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            {
                return true;
            }

            settingsMessage = $"{label} must be a number.";
            return false;
        }

        CellularSimData CreateSimulationData()
        {
            if (SelectedScenario != null)
            {
                var authoredData = SelectedScenario.CreateRuntimeData();
                return authoredData
                    .WithGridSize(width, height)
                    .WithSpeciesRules(playerSpecies, rules[playerSpecies]);
            }

            return new CellularSimData(
                width,
                height,
                new Dictionary<SpeciesId, float>
                {
                    [SpeciesIds.Plant] = plantProbability,
                    [SpeciesIds.Herbivore] = herbivoreProbability,
                    [SpeciesIds.Carnivore] = carnivoreProbability,
                },
                rules,
                runDurationSeconds,
                stepInterval,
                maxPopulation,
                minPopulation);
        }

        void ApplySelectedScenario()
        {
            var authoredData = SelectedScenario?.CreateRuntimeData();
            if (authoredData == null)
            {
                return;
            }

            width = authoredData.Width;
            height = authoredData.Height;
            runDurationSeconds = authoredData.RunDurationSeconds;
            stepInterval = authoredData.StepInterval;
            maxPopulation = authoredData.MaxPopulation;
            minPopulation = authoredData.MinPopulation;
            rules = new Dictionary<SpeciesId, SpeciesRules>(authoredData.SpeciesRules);
            ruleDrafts = CreateRuleDrafts(rules);
            if (!rules.ContainsKey(playerSpecies))
            {
                playerSpecies = FindPlayableSpecies(rules);
                playerSpeciesKey = playerSpecies.Value;
            }
        }

        ScenarioDefinitionAsset GetSelectedScenario()
        {
            return selectedScenarioIndex >= 0
                && scenarioOptions != null
                && selectedScenarioIndex < scenarioOptions.Count
                ? scenarioOptions[selectedScenarioIndex]
                : null;
        }

        void SyncRosterSpecies()
        {
            rosterSpecies.Clear();
            playableSpecies.Clear();

            if (SelectedScenario != null)
            {
                foreach (var entry in SelectedScenario.Species)
                {
                    if (entry?.Definition != null)
                    {
                        rosterSpecies.Add(entry.Definition.Id);
                    }
                }
            }
            else if (rules != null)
            {
                foreach (var entry in rules)
                {
                    rosterSpecies.Add(entry.Key);
                }
            }

            foreach (var species in rosterSpecies)
            {
                if (rules != null
                    && rules.TryGetValue(species, out var speciesRules)
                    && !speciesRules.IsPlant)
                {
                    playableSpecies.Add(species);
                }
            }
        }

        static SpeciesId FindPlayableSpecies(IReadOnlyDictionary<SpeciesId, SpeciesRules> definitions)
        {
            foreach (var entry in definitions)
            {
                if (!entry.Value.IsPlant)
                {
                    return entry.Key;
                }
            }

            foreach (var entry in definitions)
            {
                return entry.Key;
            }

            throw new InvalidOperationException("The selected scenario does not define any species.");
        }

    }
}
