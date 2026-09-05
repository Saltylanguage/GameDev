using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace SaltyGame
{
    public enum SpeciesPreviewState
    {
        Ready,
        Running,
        Paused,
        PhaseDecision,
        Rewards,
        Results,
    }

    public sealed class SpeciesSimulationPreview : MonoBehaviour
    {
        public const int ContinuousExpeditionPhaseCount = 10;

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
            public int runTicks;
            public bool continuousPhasesEnabled;
            public int phaseLengthTicks;
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

        [Header("Authored Run Upgrades")]
        [SerializeField] List<SpeciesUpgradeAsset> authoredUpgradeCatalog = new List<SpeciesUpgradeAsset>();

        [Header("Run")]
        [SerializeField, Min(1f)] float runDurationSeconds = 20f;
        [SerializeField, Min(0.01f)] float stepInterval = 0.1f;
        [SerializeField, Min(0)]
        [Tooltip("Exact ticks per run. Zero keeps the legacy duration field; the default 20 seconds at a 0.1 second step is 200 ticks.")]
        int runTicks;
        [SerializeField]
        [Tooltip("Developer test path: freeze the same run at each configured phase boundary instead of creating a new run.")]
        bool continuousPhasesEnabled = true;
        [SerializeField, Min(1)]
        [Tooltip("Ticks between decision boundaries when continuous phases are enabled.")]
        int phaseLengthTicks = 100;

        [Header("Bev Experimental Features")]
        [SerializeField] bool bevExperimentalFeaturesEnabled;
        [SerializeField, Min(0)] int foxAttackCooldownTicks;

        static readonly SpeciesUpgrade[] LegacyRewardOptions =
        {
            SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.FasterMovementId),
            SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.StrongerAttackId),
            SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.StrongerBlockId),
        };

        SpeciesUpgrade[] rewardOptions = LegacyRewardOptions;
        SpeciesUpgradeSnapshot[] authoredRewardOptions = Array.Empty<SpeciesUpgradeSnapshot>();
        bool usingAuthoredRewardOptions;

        SpeciesId playerSpecies;
        readonly List<SpeciesId> rosterSpecies = new List<SpeciesId>();
        readonly List<SpeciesId> playableSpecies = new List<SpeciesId>();
        IReadOnlyDictionary<SpeciesId, SpeciesRules> rules;
        SpeciesProgression progression;
        [SerializeField] Helper_Simulation simulationHelper;
        SimulationManager simulationManager;
        SimulationRunResult result;
        SpeciesUpgrade selectedUpgrade;
        SpeciesUpgradeSnapshot selectedUpgradeSnapshot;
        SpeciesPreviewState previewState;
        string rewardMessage;
        Dictionary<SpeciesId, SpeciesRuleDraft> ruleDrafts;
        int runNumber;
        bool rewardGranted;
        bool sessionStarted;
        string settingsMessage;
        string lastExperimentalUpgradeId;
        int experimentalOfferRotation;
        bool phaseDecisionCommitted;
        int lastSettledPhaseIndex = -1;
        bool selectedUpgradeAppliedToCurrentRun;
        string phaseRewardMessage;

        const string DefaultSettingsKey = "SaltyGame.SpeciesSimulationPreview.DefaultSettings.v3";

        public SimulationRunState Run => simulationHelper?.Run ?? simulationManager?.Run;
        public SpeciesProgression Progression => progression;
        public int PurchasedUpgradeCount => progression?.PurchasedUpgradeCount ?? 0;
        public int GetUpgradeLevel(string upgradeId) => progression?.GetUpgradeLevel(upgradeId) ?? 0;
        public int RewardOptionCount => usingAuthoredRewardOptions
            ? authoredRewardOptions.Length
            : rewardOptions.Length;
        public string GetRewardOptionId(int rewardIndex)
        {
            if (usingAuthoredRewardOptions)
            {
                return rewardIndex >= 0 && rewardIndex < authoredRewardOptions.Length
                    ? authoredRewardOptions[rewardIndex].Id
                    : string.Empty;
            }

            return rewardIndex >= 0 && rewardIndex < rewardOptions.Length
                ? rewardOptions[rewardIndex].Id
                : string.Empty;
        }

        public string GetRewardOptionDisplayName(int rewardIndex)
        {
            if (usingAuthoredRewardOptions)
            {
                if (rewardIndex < 0 || rewardIndex >= authoredRewardOptions.Length)
                {
                    return string.Empty;
                }

                return FormatAuthoredRewardOption(authoredRewardOptions[rewardIndex]);
            }

            if (rewardIndex < 0 || rewardIndex >= rewardOptions.Length)
            {
                return string.Empty;
            }

            var legacyUpgrade = rewardOptions[rewardIndex];
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}\nCost: {1} data — {2}",
                SpeciesUpgradeCatalog.GetDisplayName(legacyUpgrade.Id),
                legacyUpgrade.Cost,
                GetLegacyRewardStatus(legacyUpgrade));
        }

        public string GetSelectedUpgradeSummary()
        {
            if (selectedUpgradeSnapshot != null)
            {
                return FormatSnapshotSummary(selectedUpgradeSnapshot, selectedUpgradeAppliedToCurrentRun);
            }

            return selectedUpgrade == null
                ? "No upgrade selected."
                : $"{SpeciesUpgradeCatalog.GetDisplayName(selectedUpgrade.Id)} — legacy upgrade applied to the next run.";
        }
        public SpeciesPreviewState State => previewState;
        public int GridWidth => width;
        public int GridHeight => height;
        public int BaseSeed => seed;
        public int MaximumPopulation => maxPopulation;
        public int MinimumPopulation => minPopulation;
        public float RunDurationSeconds => runTicks > 0
            ? (float)(runTicks * (double)stepInterval)
            : runDurationSeconds;
        public float StepInterval => stepInterval;
        public int RunTicks => runTicks > 0
            ? runTicks
            : CellularSimData.CalculateRunTicks(runDurationSeconds, stepInterval);
        public bool ContinuousPhasesEnabled => continuousPhasesEnabled;
        public int PhaseLengthTicks => phaseLengthTicks;
        public int ContinuousPhaseCount => ContinuousExpeditionPhaseCount;
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
        public string PhaseRewardMessage => phaseRewardMessage ?? string.Empty;
        public bool SettingsEditable => previewState == SpeciesPreviewState.Ready && !sessionStarted;

        public bool TryApplyLaunchRequest(SimulationLaunchRequest launch, out string validationMessage)
        {
            validationMessage = string.Empty;
            if (launch == null)
            {
                validationMessage = "A simulation launch request is required.";
                return false;
            }

            randomizeSeedOnStart = false;
            seed = launch.Seed;

            var scenarioIndex = -1;
            for (var index = 0; index < scenarioOptions.Count; index++)
            {
                var scenario = scenarioOptions[index];
                if (scenario != null
                    && string.Equals(scenario.name, launch.ScenarioId, StringComparison.OrdinalIgnoreCase))
                {
                    scenarioIndex = index;
                    break;
                }
            }

            if (scenarioIndex < 0)
            {
                validationMessage = $"Scenario '{launch.ScenarioId}' is not available in the Simulation scene.";
                settingsMessage = validationMessage;
                return false;
            }

            if (!TrySelectScenario(scenarioIndex, out validationMessage))
            {
                return false;
            }

            if (!TrySetPlayerSpecies(launch.PlayerSpeciesId, out validationMessage))
            {
                return false;
            }

            if (!TryApplyLaunchUpgrades(launch.OrderedUpgradeSnapshots, out validationMessage))
            {
                return false;
            }

            settingsMessage = $"Launch accepted: {launch.ScenarioId} / {launch.PlayerSpeciesId} / seed {launch.Seed}.";
            return true;
        }

        bool TryApplyLaunchUpgrades(
            IReadOnlyList<SpeciesUpgradeSnapshot> upgrades,
            out string validationMessage)
        {
            validationMessage = string.Empty;
            if (upgrades == null || upgrades.Count == 0)
            {
                return true;
            }

            if (progression == null)
            {
                validationMessage = "The player progression is not ready for upgrade snapshots.";
                settingsMessage = validationMessage;
                return false;
            }

            // Validate the complete ordered loadout before mutating progression.
            // This keeps a malformed request from applying only its prefix.
            var plannedUpgradeIds = new HashSet<string>(progression.OrderedUpgradeIds, StringComparer.Ordinal);
            var plannedRules = progression.CurrentRules;
            foreach (var upgrade in upgrades)
            {
                if (upgrade == null)
                {
                    validationMessage = "Launch upgrade snapshots cannot contain null entries.";
                    settingsMessage = validationMessage;
                    return false;
                }

                if (upgrade.TargetSpecies != playerSpecies
                    || plannedUpgradeIds.Contains(upgrade.Id))
                {
                    validationMessage =
                        $"Upgrade '{upgrade.Id}' cannot be applied to species '{playerSpecies.Value}' or is duplicated.";
                    settingsMessage = validationMessage;
                    return false;
                }

                foreach (var prerequisiteId in upgrade.PrerequisiteUpgradeIds)
                {
                    if (!plannedUpgradeIds.Contains(prerequisiteId))
                    {
                        validationMessage =
                            $"Upgrade '{upgrade.Id}' requires '{prerequisiteId}' before it can be applied.";
                        settingsMessage = validationMessage;
                        return false;
                    }
                }

                foreach (var excludedId in upgrade.ExcludedUpgradeIds)
                {
                    if (plannedUpgradeIds.Contains(excludedId))
                    {
                        validationMessage =
                            $"Upgrade '{upgrade.Id}' conflicts with already selected upgrade '{excludedId}'.";
                        settingsMessage = validationMessage;
                        return false;
                    }
                }

                try
                {
                    plannedRules = upgrade.Apply(plannedRules);
                }
                catch (Exception exception) when (
                    exception is ArgumentException
                    || exception is InvalidOperationException
                    || exception is OverflowException)
                {
                    validationMessage = $"Upgrade '{upgrade.Id}' is invalid: {exception.Message}";
                    settingsMessage = validationMessage;
                    return false;
                }

                plannedUpgradeIds.Add(upgrade.Id);
            }

            foreach (var upgrade in upgrades)
            {
                if (!progression.TryApplyRunUpgrade(upgrade))
                {
                    validationMessage = $"Upgrade '{upgrade.Id}' cannot be applied to species '{playerSpecies.Value}'.";
                    settingsMessage = validationMessage;
                    return false;
                }
            }

            var updatedRules = new Dictionary<SpeciesId, SpeciesRules>(rules)
            {
                [playerSpecies] = progression.CurrentRules,
            };
            rules = updatedRules;
            PrepareNextRun();
            return true;
        }

        public void BindSimulationHelper(Helper_Simulation helper)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }

            if (ReferenceEquals(simulationHelper, helper))
            {
                return;
            }

            var existingRunner = simulationManager?.Runner;

            if (simulationHelper != null)
            {
                simulationHelper.RunCompleted -= HandleRunCompleted;
                simulationHelper.PhaseBoundaryReached -= HandlePhaseBoundaryReached;
            }

            if (simulationManager != null)
            {
                simulationManager.RunCompleted -= HandleRunCompleted;
                simulationManager.PhaseBoundaryReached -= HandlePhaseBoundaryReached;
                simulationManager = null;
            }

            simulationHelper = helper;
            simulationHelper.RunCompleted += HandleRunCompleted;
            simulationHelper.PhaseBoundaryReached += HandlePhaseBoundaryReached;
            if (existingRunner != null)
            {
                simulationHelper.SetRunner(existingRunner);
            }
        }

        public void ConfigureScenarioOptions(IReadOnlyList<ScenarioDefinitionAsset> options, int initialSelection = -1)
        {
            scenarioOptions = options == null
                ? new List<ScenarioDefinitionAsset>()
                : new List<ScenarioDefinitionAsset>(options);
            selectedScenarioIndex = Mathf.Clamp(initialSelection, -1, scenarioOptions.Count - 1);

            // CellularAutomataPrototypeRuntime may configure options from its
            // Awake before this component's Awake has initialized ruleDrafts.
            // Defer the reset until our own initialization in that case.
            if (ruleDrafts == null)
            {
                return;
            }

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
            if (simulationHelper != null)
            {
                simulationHelper.RunCompleted += HandleRunCompleted;
                simulationHelper.PhaseBoundaryReached += HandlePhaseBoundaryReached;
            }
            else
            {
                simulationManager = new SimulationManager();
                simulationManager.RunCompleted += HandleRunCompleted;
                simulationManager.PhaseBoundaryReached += HandlePhaseBoundaryReached;
            }

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
            if (simulationHelper != null)
            {
                simulationHelper.Advance(Time.deltaTime);
            }
            else
            {
                simulationManager?.Advance(Time.deltaTime);
            }
        }

        void OnDestroy()
        {
            if (simulationManager != null)
            {
                simulationManager.RunCompleted -= HandleRunCompleted;
                simulationManager.PhaseBoundaryReached -= HandlePhaseBoundaryReached;
            }

            if (simulationHelper != null)
            {
                simulationHelper.RunCompleted -= HandleRunCompleted;
                simulationHelper.PhaseBoundaryReached -= HandlePhaseBoundaryReached;
            }
        }

        void HandlePhaseBoundaryReached(SimulationRunState run)
        {
            if (continuousPhasesEnabled
                && run != null
                && run.Status == SimulationRunStatus.AwaitingDecision)
            {
                if (lastSettledPhaseIndex == run.PhaseIndex)
                {
                    return;
                }

                var phaseResult = SimulationRunResults.Create(run);
                progression?.AddCurrency(phaseResult.CurrencyEarned);
                lastSettledPhaseIndex = run.PhaseIndex;
                phaseDecisionCommitted = false;
                phaseRewardMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    "Phase {0} complete: {1} data earned from current survivors.",
                    run.PhaseIndex,
                    phaseResult.CurrencyEarned);
                PrepareRewardOptions();
                previewState = SpeciesPreviewState.PhaseDecision;
            }
        }

        void HandleRunCompleted(SimulationRunState run)
        {
            if (rewardGranted)
            {
                return;
            }

            result = SimulationRunResults.Create(run);
            RunCompleted?.Invoke(this, run);
            progression.AddCurrency(result.CurrencyEarned);
            rewardGranted = true;

            // Continuous runs only offer upgrades at phase boundaries. A
            // terminal result must not fall back into the legacy reward flow.
            if (continuousPhasesEnabled && run?.SupportsContinuation == true)
            {
                rewardMessage = string.Empty;
                previewState = SpeciesPreviewState.Results;
                return;
            }

            PrepareRewardOptions();
            previewState = SpeciesPreviewState.Rewards;
        }

        public void StartSimulation()
        {
            if (Run != null && Run.Status == SimulationRunStatus.Ready)
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

                if (simulationHelper != null)
                {
                    simulationHelper.StartRun();
                }
                else
                {
                    simulationManager.Start();
                }
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
            return TryApplyGlobalSettingsCore(
                widthValue,
                heightValue,
                seedValue,
                maximumPopulationValue,
                minimumPopulationValue,
                runDurationValue,
                stepIntervalValue,
                plantProbabilityValue,
                herbivoreProbabilityValue,
                carnivoreProbabilityValue,
                randomizeSeed,
                runWindowIsTicks: false,
                out validationMessage);
        }

        public bool TryApplyGlobalSettingsForTicks(
            string widthValue,
            string heightValue,
            string seedValue,
            string maximumPopulationValue,
            string minimumPopulationValue,
            string runTicksValue,
            string stepIntervalValue,
            string plantProbabilityValue,
            string herbivoreProbabilityValue,
            string carnivoreProbabilityValue,
            bool randomizeSeed,
            out string validationMessage)
        {
            return TryApplyGlobalSettingsCore(
                widthValue,
                heightValue,
                seedValue,
                maximumPopulationValue,
                minimumPopulationValue,
                runTicksValue,
                stepIntervalValue,
                plantProbabilityValue,
                herbivoreProbabilityValue,
                carnivoreProbabilityValue,
                randomizeSeed,
                runWindowIsTicks: true,
                out validationMessage);
        }

        bool TryApplyGlobalSettingsCore(
            string widthValue,
            string heightValue,
            string seedValue,
            string maximumPopulationValue,
            string minimumPopulationValue,
            string runWindowValue,
            string stepIntervalValue,
            string plantProbabilityValue,
            string herbivoreProbabilityValue,
            string carnivoreProbabilityValue,
            bool randomizeSeed,
            bool runWindowIsTicks,
            out string validationMessage)
        {
            validationMessage = string.Empty;
            if (previewState != SpeciesPreviewState.Ready || sessionStarted)
            {
                validationMessage = "Settings can only be changed before a session starts.";
                settingsMessage = validationMessage;
                return false;
            }

            var parsedWidth = 0;
            var parsedHeight = 0;
            var parsedSeed = 0;
            var parsedMaximumPopulation = 0;
            var parsedMinimumPopulation = 0;
            var parsedRunTicks = 0;
            var parsedRunDuration = 0f;
            var parsedStepInterval = 0f;
            var parsedPlantProbability = 0f;
            var parsedHerbivoreProbability = 0f;
            var parsedCarnivoreProbability = 0f;
            if (!TryParseInt(widthValue, "Grid width", out parsedWidth)
                || !TryParseInt(heightValue, "Grid height", out parsedHeight)
                || !TryParseInt(seedValue, "Base seed", out parsedSeed)
                || !TryParseInt(maximumPopulationValue, "Maximum population", out parsedMaximumPopulation)
                || !TryParseInt(minimumPopulationValue, "Minimum population", out parsedMinimumPopulation)
                || (runWindowIsTicks
                    ? !TryParseInt(runWindowValue, "Run ticks", out parsedRunTicks)
                    : !TryParseFloat(runWindowValue, "Run duration", out parsedRunDuration))
                || !TryParseFloat(stepIntervalValue, "Step interval", out parsedStepInterval)
                || !TryParseFloat(plantProbabilityValue, "Plant probability", out parsedPlantProbability)
                || !TryParseFloat(herbivoreProbabilityValue, "Herbivore probability", out parsedHerbivoreProbability)
                || !TryParseFloat(carnivoreProbabilityValue, "Carnivore probability", out parsedCarnivoreProbability))
            {
                validationMessage = settingsMessage;
                return false;
            }

            width = Mathf.Max(1, parsedWidth);
            height = Mathf.Max(1, parsedHeight);
            seed = parsedSeed;
            maxPopulation = Mathf.Max(0, parsedMaximumPopulation);
            minPopulation = Mathf.Max(0, parsedMinimumPopulation);
            stepInterval = Mathf.Max(0.01f, parsedStepInterval);
            if (runWindowIsTicks)
            {
                runTicks = Mathf.Max(1, parsedRunTicks);
                runDurationSeconds = (float)(runTicks * (double)stepInterval);
                if (float.IsNaN(runDurationSeconds) || float.IsInfinity(runDurationSeconds))
                {
                    validationMessage = "Run ticks and step interval produce an invalid run duration.";
                    settingsMessage = validationMessage;
                    return false;
                }
            }
            else
            {
                runTicks = 0;
                runDurationSeconds = Mathf.Max(1f, parsedRunDuration);
            }

            plantProbability = Mathf.Clamp01(parsedPlantProbability);
            herbivoreProbability = Mathf.Clamp01(parsedHerbivoreProbability);
            carnivoreProbability = Mathf.Clamp01(parsedCarnivoreProbability);
            randomizeSeedOnStart = randomizeSeed;
            settingsMessage = "Global settings applied to the next run.";
            PrepareNextRun();
            validationMessage = settingsMessage;
            return true;
        }

        public bool TryApplyContinuousPhases(
            bool enabled,
            string phaseLengthValue,
            out string validationMessage)
        {
            validationMessage = string.Empty;
            if (!SettingsEditable)
            {
                validationMessage = "Continuous phases can only be changed before a session starts.";
                settingsMessage = validationMessage;
                return false;
            }

            if (!enabled)
            {
                continuousPhasesEnabled = false;
                settingsMessage = "Continuous phases disabled; runs use the normal terminal flow.";
                validationMessage = settingsMessage;
                return true;
            }

            if (!TryParseInt(phaseLengthValue, "Phase length", out var parsedPhaseLength))
            {
                validationMessage = settingsMessage;
                return false;
            }

            if (parsedPhaseLength <= 0)
            {
                validationMessage = "Phase length must be greater than zero.";
                settingsMessage = validationMessage;
                return false;
            }

            if (parsedPhaseLength > int.MaxValue / ContinuousExpeditionPhaseCount)
            {
                validationMessage = "Phase length is too large for a ten-phase expedition.";
                settingsMessage = validationMessage;
                return false;
            }

            continuousPhasesEnabled = true;
            phaseLengthTicks = parsedPhaseLength;
            settingsMessage = $"Continuous phases enabled: {ContinuousExpeditionPhaseCount} phases, decision every {phaseLengthTicks} ticks.";
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
            lastExperimentalUpgradeId = null;
            experimentalOfferRotation = 0;
            rewardOptions = LegacyRewardOptions;
            settingsMessage = enabled
                ? $"Bev experimental features enabled: opposed-roll combat, herbivore stat line, two-of-four herbivore upgrades, fox cooldown {foxAttackCooldownTicks} ticks."
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
            if ((simulationHelper != null && simulationHelper.PauseRun())
                || (simulationHelper == null && simulationManager != null && simulationManager.Pause()))
            {
                previewState = SpeciesPreviewState.Paused;
            }
        }

        public void ResumeSimulation()
        {
            if ((simulationHelper != null && simulationHelper.ResumeRun())
                || (simulationHelper == null && simulationManager != null && simulationManager.Resume()))
            {
                previewState = SpeciesPreviewState.Running;
            }
        }

        public void RestartSimulation()
        {
            var restarted = simulationHelper != null
                ? simulationHelper.RestartRun()
                : simulationManager != null && simulationManager.Restart();
            if (!restarted)
            {
                return;
            }

            result = default;
            rewardGranted = false;
            selectedUpgrade = null;
            selectedUpgradeSnapshot = null;
            rewardMessage = string.Empty;
            phaseRewardMessage = string.Empty;
            phaseDecisionCommitted = false;
            lastSettledPhaseIndex = -1;
            selectedUpgradeAppliedToCurrentRun = false;
            previewState = SpeciesPreviewState.Running;
        }

        public void StopSimulation()
        {
            if (Run != null
                && (Run.Status == SimulationRunStatus.Running
                    || Run.Status == SimulationRunStatus.Paused
                    || Run.Status == SimulationRunStatus.AwaitingDecision))
            {
                if (simulationHelper != null)
                {
                    simulationHelper.StopRun();
                }
                else
                {
                    simulationManager?.Stop();
                }

                ResetToStart();
            }
        }

        public bool CanPurchaseReward(int rewardIndex)
        {
            if (usingAuthoredRewardOptions)
            {
                return rewardIndex >= 0
                    && rewardIndex < authoredRewardOptions.Length
                    && CanPurchaseAuthoredReward(authoredRewardOptions[rewardIndex]);
            }

            if (previewState == SpeciesPreviewState.PhaseDecision)
            {
                return rewardIndex >= 0
                    && rewardIndex < rewardOptions.Length
                    && progression != null
                    && !phaseDecisionCommitted
                    && Run?.Status == SimulationRunStatus.AwaitingDecision
                    && CanPurchaseLegacyBoundaryReward(rewardOptions[rewardIndex]);
            }

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

            if (previewState == SpeciesPreviewState.PhaseDecision)
            {
                return PurchaseBoundaryReward(rewardIndex);
            }

            if (usingAuthoredRewardOptions)
            {
                var authoredUpgrade = authoredRewardOptions[rewardIndex];
                if (!progression.TrySpend(authoredUpgrade.Cost))
                {
                    return false;
                }

                if (!progression.TryApplyRunUpgrade(authoredUpgrade))
                {
                    progression.AddCurrency(authoredUpgrade.Cost);
                    return false;
                }

                selectedUpgrade = null;
                selectedUpgradeSnapshot = authoredUpgrade;
                selectedUpgradeAppliedToCurrentRun = false;
                previewState = SpeciesPreviewState.Results;
                rewardMessage = string.Empty;
                return true;
            }

            var upgrade = rewardOptions[rewardIndex];
            if (!progression.TryPurchase(upgrade))
            {
                return false;
            }

            selectedUpgrade = upgrade;
            selectedUpgradeSnapshot = null;
            selectedUpgradeAppliedToCurrentRun = false;
            if (bevExperimentalFeaturesEnabled)
            {
                lastExperimentalUpgradeId = upgrade.Id;
            }
            previewState = SpeciesPreviewState.Results;
            rewardMessage = string.Empty;
            return true;
        }

        bool PurchaseBoundaryReward(int rewardIndex)
        {
            if (Run == null
                || Run.Status != SimulationRunStatus.AwaitingDecision
                || phaseDecisionCommitted
                || GetBoundaryUpgrade(rewardIndex) == null)
            {
                return false;
            }

            var authoredUpgrade = GetBoundaryUpgrade(rewardIndex);
            if (authoredUpgrade == null
                || !authoredUpgrade.CanApplyAfterRunStart
                || !progression.TrySpend(authoredUpgrade.Cost))
            {
                return false;
            }

            if (!progression.TryApplyRunUpgrade(authoredUpgrade))
            {
                progression.AddCurrency(authoredUpgrade.Cost);
                return false;
            }

            var nextRules = new Dictionary<SpeciesId, SpeciesRules>(rules)
            {
                [playerSpecies] = progression.CurrentRules,
            };
            var continued = simulationHelper != null
                ? simulationHelper.ContinueWithBoundaryState(
                    nextRules,
                    CreateExperimentalOptions(),
                    progression.AppliedRunUpgrades)
                : simulationManager != null
                    && simulationManager.ContinueWithBoundaryState(
                        nextRules,
                        CreateExperimentalOptions(),
                        progression.AppliedRunUpgrades);
            if (!continued)
            {
                // The status check above makes this an unreachable path in the
                // single-threaded preview, but do not present a successful
                // purchase when the retained run could not resume.
                return false;
            }

            rules = nextRules;
            selectedUpgrade = null;
            selectedUpgradeSnapshot = authoredUpgrade;
            selectedUpgradeAppliedToCurrentRun = true;
            phaseDecisionCommitted = true;
            previewState = SpeciesPreviewState.Running;
            rewardMessage = string.Empty;
            return true;
        }

        bool CanPurchaseLegacyBoundaryReward(SpeciesUpgrade upgrade)
        {
            if (upgrade == null || progression.GetUpgradeLevel(upgrade.Id) > 0)
            {
                return false;
            }

            var snapshot = upgrade.CreateSnapshot(playerSpecies);
            return snapshot.CanApplyAfterRunStart
                && progression.Currency >= snapshot.Cost;
        }

        string GetLegacyRewardStatus(SpeciesUpgrade upgrade)
        {
            if (progression == null)
            {
                return "UNAVAILABLE";
            }

            if (previewState == SpeciesPreviewState.PhaseDecision
                && progression.GetUpgradeLevel(upgrade.Id) > 0)
            {
                return "OWNED";
            }

            return progression.Currency < upgrade.Cost
                ? $"NEED {upgrade.Cost - progression.Currency} more data"
                : "AVAILABLE";
        }

        SpeciesUpgradeSnapshot GetBoundaryUpgrade(int rewardIndex)
        {
            return usingAuthoredRewardOptions
                ? rewardIndex >= 0 && rewardIndex < authoredRewardOptions.Length
                    ? authoredRewardOptions[rewardIndex]
                    : null
                : rewardIndex >= 0 && rewardIndex < rewardOptions.Length
                    ? rewardOptions[rewardIndex].CreateSnapshot(playerSpecies)
                    : null;
        }

        public void ContinueWithoutUpgrade()
        {
            if (previewState == SpeciesPreviewState.PhaseDecision)
            {
                if (phaseDecisionCommitted)
                {
                    return;
                }

                var continued = simulationHelper != null
                    ? simulationHelper.ContinueWithoutUpgrade()
                    : simulationManager != null && simulationManager.ContinueWithoutUpgrade();
                if (continued)
                {
                    phaseDecisionCommitted = true;
                    previewState = SpeciesPreviewState.Running;
                }
            }
            else if (previewState == SpeciesPreviewState.Rewards)
            {
                previewState = SpeciesPreviewState.Results;
            }
        }

        public void EndSimulation()
        {
            if (Run == null
                || (Run.Status != SimulationRunStatus.Running
                    && Run.Status != SimulationRunStatus.AwaitingDecision))
            {
                return;
            }

            var ended = simulationHelper != null
                ? simulationHelper.EndRun()
                : simulationManager != null && simulationManager.End();
            if (ended
                && previewState != SpeciesPreviewState.Rewards
                && previewState != SpeciesPreviewState.Results)
            {
                previewState = continuousPhasesEnabled && Run?.SupportsContinuation == true
                    ? SpeciesPreviewState.Results
                    : SpeciesPreviewState.Rewards;
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
            lastExperimentalUpgradeId = null;
            experimentalOfferRotation = 0;
            rewardOptions = LegacyRewardOptions;
            authoredRewardOptions = Array.Empty<SpeciesUpgradeSnapshot>();
            usingAuthoredRewardOptions = false;
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
                runTicks = runTicks,
                continuousPhasesEnabled = continuousPhasesEnabled,
                phaseLengthTicks = phaseLengthTicks,
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
            runTicks = Mathf.Max(0, saved.runTicks);
            continuousPhasesEnabled = saved.continuousPhasesEnabled;
            phaseLengthTicks = Mathf.Max(1, saved.phaseLengthTicks);
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
            var continuousRun = continuousPhasesEnabled
                && phaseLengthTicks > 0
                && phaseLengthTicks <= int.MaxValue / ContinuousExpeditionPhaseCount;
            if (continuousPhasesEnabled && !continuousRun)
            {
                continuousPhasesEnabled = false;
                settingsMessage = "Continuous phases disabled because the phase length is invalid for a ten-phase expedition.";
            }

            var targetTicks = continuousRun
                ? phaseLengthTicks * ContinuousExpeditionPhaseCount
                : simulationData.RunTicks;
            var durationSeconds = continuousRun
                ? (float)(targetTicks * (double)simulationData.StepInterval)
                : simulationData.RunDurationSeconds;

            var run = new SimulationRunState(
                SpeciesInitialGridFactory.Create(simulationData, seed + runNumber),
                playerSpecies,
                seed + runNumber,
                durationSeconds,
                targetTicks);
            if (continuousRun)
            {
                run.ConfigureContinuousPhases(phaseLengthTicks);
            }
            var nextRunner = new SpeciesSimulationRunner(
                run,
                simulationData,
                combatResolutionMode: bevExperimentalFeaturesEnabled
                    ? SpeciesCombatResolutionMode.OpposedRoll
                    : SpeciesCombatResolutionMode.LegacyFixedDamage,
                experimentalOptions: CreateExperimentalOptions(),
                upgradeLoadout: progression?.AppliedRunUpgrades);
            if (simulationHelper != null)
            {
                simulationHelper.SetRunner(nextRunner);
            }
            else
            {
                simulationManager.SetRunner(nextRunner);
            }
            result = default;
            rewardGranted = false;
            selectedUpgrade = null;
            selectedUpgradeSnapshot = null;
            selectedUpgradeAppliedToCurrentRun = false;
            authoredRewardOptions = Array.Empty<SpeciesUpgradeSnapshot>();
            usingAuthoredRewardOptions = false;
            rewardMessage = string.Empty;
            phaseRewardMessage = string.Empty;
            phaseDecisionCommitted = false;
            lastSettledPhaseIndex = -1;
            previewState = SpeciesPreviewState.Ready;
            runNumber++;
        }

        void PrepareRewardOptions()
        {
            authoredRewardOptions = Array.Empty<SpeciesUpgradeSnapshot>();
            usingAuthoredRewardOptions = false;
            if (!bevExperimentalFeaturesEnabled)
            {
                var authoredOptions = new List<SpeciesUpgradeSnapshot>();
                foreach (var asset in authoredUpgradeCatalog ?? new List<SpeciesUpgradeAsset>())
                {
                    if (asset == null
                        || !asset.TryCreateSnapshot(out var snapshot, out _)
                        || snapshot.TargetSpecies != playerSpecies)
                    {
                        continue;
                    }

                    authoredOptions.Add(snapshot);
                }

                if (authoredOptions.Count > 0)
                {
                    authoredRewardOptions = authoredOptions.ToArray();
                    usingAuthoredRewardOptions = true;
                    return;
                }
            }

            if (!bevExperimentalFeaturesEnabled
                || !rules.TryGetValue(playerSpecies, out var playerRules)
                || playerRules.Role != SpeciesRole.Herbivore)
            {
                rewardOptions = LegacyRewardOptions;
                return;
            }

            rewardOptions = SpeciesUpgradeCatalog.CreateExperimentalHerbivoreOffer(
                lastExperimentalUpgradeId,
                experimentalOfferRotation,
                Run?.Seed ?? seed);
            experimentalOfferRotation++;
        }

        SpeciesExperimentalOptions CreateExperimentalOptions()
        {
            return bevExperimentalFeaturesEnabled
                ? new SpeciesExperimentalOptions(
                    SpeciesExperimentalOptions.BevExperimentalFeaturesId,
                    foxAttackCooldownTicks,
                    progression?.PreContactAvoidanceChance ?? 0f)
                : SpeciesExperimentalOptions.None;
        }

        bool CanPurchaseAuthoredReward(SpeciesUpgradeSnapshot upgrade)
        {
            if ((previewState != SpeciesPreviewState.Rewards
                    && previewState != SpeciesPreviewState.PhaseDecision)
                || progression == null
                || upgrade == null
                || upgrade.TargetSpecies != playerSpecies
                || progression.GetUpgradeLevel(upgrade.Id) > 0
                || progression.Currency < upgrade.Cost)
            {
                return false;
            }

            if (previewState == SpeciesPreviewState.PhaseDecision
                && (!continuousPhasesEnabled || phaseDecisionCommitted || !upgrade.CanApplyAfterRunStart))
            {
                return false;
            }

            foreach (var prerequisiteId in upgrade.PrerequisiteUpgradeIds)
            {
                if (progression.GetUpgradeLevel(prerequisiteId) == 0)
                {
                    return false;
                }
            }

            foreach (var excludedId in upgrade.ExcludedUpgradeIds)
            {
                if (progression.GetUpgradeLevel(excludedId) > 0)
                {
                    return false;
                }
            }

            return true;
        }

        string FormatAuthoredRewardOption(SpeciesUpgradeSnapshot upgrade)
        {
            var modifiers = string.Join(
                ", ",
                upgrade.Modifiers.Select(
                    FormatModifierForDisplay));
            var status = GetAuthoredRewardStatus(upgrade);
            return $"{upgrade.DisplayName}\n{modifiers}\nCost: {upgrade.Cost} data — {status}";
        }

        string GetAuthoredRewardStatus(SpeciesUpgradeSnapshot upgrade)
        {
            if (progression == null)
            {
                return "UNAVAILABLE";
            }

            if (progression.GetUpgradeLevel(upgrade.Id) > 0)
            {
                return "OWNED";
            }

            if (previewState == SpeciesPreviewState.PhaseDecision
                && !upgrade.CanApplyAfterRunStart)
            {
                return "LAUNCH ONLY";
            }

            var missingPrerequisites = upgrade.PrerequisiteUpgradeIds
                .Where(id => progression.GetUpgradeLevel(id) == 0)
                .ToArray();
            if (missingPrerequisites.Length > 0)
            {
                return $"LOCKED — requires {string.Join(", ", missingPrerequisites)}";
            }

            var blockedBy = upgrade.ExcludedUpgradeIds
                .FirstOrDefault(id => progression.GetUpgradeLevel(id) > 0);
            if (!string.IsNullOrWhiteSpace(blockedBy))
            {
                return $"LOCKED — conflicts with {blockedBy}";
            }

            if (progression.Currency < upgrade.Cost)
            {
                return $"NEED {upgrade.Cost - progression.Currency} more data";
            }

            return "AVAILABLE";
        }

        static string FormatSnapshotSummary(SpeciesUpgradeSnapshot upgrade, bool appliedToCurrentRun = false)
        {
            var modifiers = string.Join(
                ", ",
                upgrade.Modifiers.Select(
                    FormatModifierForDisplay));
            var timing = appliedToCurrentRun
                ? "Applied to this run and carried into the next run."
                : "Applied to the next run.";
            return $"{upgrade.DisplayName} — {upgrade.Description} Effects: {modifiers}. {timing}";
        }

        static string FormatModifierForDisplay(SpeciesUpgradeModifier modifier)
        {
            var label = SpeciesAttributeRegistry.TryGet(modifier.AttributeId, out var definition)
                ? definition.DisplayName
                : modifier.AttributeId;
            return $"{label} {modifier.SignedValue:+0.###;-0.###;0}";
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
                    .WithRunTicks(RunTicks, stepInterval)
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
                runTicks > 0
                    ? (float)(runTicks * (double)stepInterval)
                    : runDurationSeconds,
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
            runTicks = authoredData.RunTicks;
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
