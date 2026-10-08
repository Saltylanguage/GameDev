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
        JourneyDecision,
        JourneyNodeReward,
        Rewards,
        Results,
    }

    public sealed class SpeciesSimulationPreview : MonoBehaviour
    {
        public const int ContinuousExpeditionPhaseCount = 6;
        public const int JourneyPrototypeCycleCount = 2;
        public const int HareCost = SpeciesProgression.HarePurchaseCost;

        public static event Action<SpeciesSimulationPreview, SimulationRunState> RunCompleted;

        readonly struct JourneyConditionOption
        {
            public JourneyConditionOption(JourneyNodeAsset node, SpeciesUpgradeSnapshot condition)
            {
                Condition = condition;
                DisplayName = node.DisplayName;
                Description = node.Description;
                DataReward = node.DataReward;
            }

            public SpeciesUpgradeSnapshot Condition { get; }
            public string DisplayName { get; }
            public string Description { get; }
            public int DataReward { get; }
        }

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
                ForageThresholdFraction = rules.ForageThresholdFraction;
                MatingEnergyThresholdFraction = rules.MatingEnergyThresholdFraction;
                MatingEnergyCostFraction = rules.MatingEnergyCostFraction;
                DistributeMatingEnergyToOffspring = rules.DistributeMatingEnergyToOffspring;
                ForagesUntilFull = rules.ForagesUntilFull;
                EnergyLossIntervalTicks = rules.EnergyLossIntervalTicks;
                ForageBelowEnergyText = rules.ForageBelowEnergy.ToString(CultureInfo.InvariantCulture);
                EnergyValue = rules.EnergyValue;
                EnergyValueText = rules.EnergyValue.ToString(CultureInfo.InvariantCulture);
                Metabolism = rules.Metabolism;
                MetabolismText = rules.Metabolism.ToString(CultureInfo.InvariantCulture);
                VisionRange = rules.Awareness.VisionRange;
                VisionRangeText = rules.Awareness.VisionRange.ToString(CultureInfo.InvariantCulture);
                Intelligence = rules.Awareness.Intelligence;
                IntelligenceText = rules.Awareness.Intelligence.ToString(CultureInfo.InvariantCulture);
                TrackingPersistenceSteps = rules.TrackingPersistenceSteps;
                BehaviorStateRules = rules.BehaviorStateRules.ToArray();
                ReproductionEnabled = rules.ReproductionChance > 0f;
                WiltChance = rules.WiltChance;
                WiltChanceText = FormatFloat(rules.WiltChance);
                WiltEnabled = rules.WiltChance > 0f;
                CrowdingEnergyReduction = rules.CrowdingEnergyReduction;
                CrowdingMetabolismMultiplier = rules.CrowdingMetabolismMultiplier;
                CrowdingMetabolismMultiplierText = rules.CrowdingMetabolismMultiplier.ToString(CultureInfo.InvariantCulture);
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
            public float ForageThresholdFraction;
            public float MatingEnergyThresholdFraction;
            public float MatingEnergyCostFraction;
            public bool DistributeMatingEnergyToOffspring;
            public bool ForagesUntilFull;
            public int EnergyLossIntervalTicks = 1;
            public int EnergyValue;
            public string EnergyValueText;
            public int Metabolism;
            public string MetabolismText;
            public int VisionRange;
            public string VisionRangeText;
            public int Intelligence;
            public string IntelligenceText;
            public int TrackingPersistenceSteps;
            public SpeciesBehaviorStateRule[] BehaviorStateRules;
            public bool WiltEnabled;
            public float WiltChance;
            public string WiltChanceText;
            public float CrowdingEnergyReduction;
            public int CrowdingMetabolismMultiplier = 2;
            public string CrowdingMetabolismMultiplierText = "2";
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
            public int plantStartingPopulation;
            public int herbivoreStartingPopulation;
            public int carnivoreStartingPopulation;
            public bool startingPopulationOverrideEnabled;
            public float runDurationSeconds;
            public float stepInterval;
            public int runTicks;
            public bool continuousPhasesEnabled;
            public int phaseLengthTicks;
            public int maxPopulation;
            public int minPopulation;
            public bool coupledSpeciesResponsesEnabled;
            public SpeciesRuleDraft plant;
            public SpeciesRuleDraft herbivore;
            public SpeciesRuleDraft carnivore;
        }

        [Header("Grid")]
        [SerializeField, Tooltip("Connect left/right and top/bottom edges for both species. Change before starting a session.")] bool wrapEdges;
        [SerializeField, Min(1)] int width = 32;
        [SerializeField, Min(1)] int height = 20;
        [SerializeField] int seed = 12345;
        [SerializeField] bool randomizeSeedOnStart = true;
        [SerializeField] string playerSpeciesKey = "herbivore";
        [SerializeField, Range(0f, 1f)] float plantProbability = 0.4f;
        [SerializeField, Range(0f, 1f)] float herbivoreProbability = 0.02f;
        [SerializeField, Range(0f, 1f)] float carnivoreProbability = 0.004f;
        [SerializeField, Min(0)] int plantStartingPopulation;
        [SerializeField, Min(0)] int herbivoreStartingPopulation;
        [SerializeField, Min(0)] int carnivoreStartingPopulation;
        bool startingPopulationOverrideEnabled;
        [SerializeField, Min(0)] int maxPopulation;
        [SerializeField, Min(0)] int minPopulation;

        [Header("Authored Scenarios")]
        [SerializeField] List<ScenarioDefinitionAsset> scenarioOptions = new List<ScenarioDefinitionAsset>();
        [SerializeField, Min(-1)] int selectedScenarioIndex = 0;

        [Header("Authored Run Upgrades")]
        [SerializeField] List<SpeciesUpgradeAsset> authoredUpgradeCatalog = new List<SpeciesUpgradeAsset>();

        [Header("Journey")]
        [SerializeField] JourneyMapAsset journeyMap;
        [SerializeField] bool journeyPrototypeEnabled = true;

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
        [SerializeField] bool bevExperimentalFeaturesEnabled = true;
        [SerializeField, Min(0)] int foxAttackCooldownTicks;
        [SerializeField] bool coupledSpeciesResponsesEnabled;

        static readonly SpeciesUpgrade[] LegacyRewardOptions =
        {
            SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.FasterMovementId),
            SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.StrongerAttackId),
            SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.StrongerBlockId),
        };

        SpeciesUpgrade[] rewardOptions = LegacyRewardOptions;
        SpeciesUpgradeSnapshot[] authoredRewardOptions = Array.Empty<SpeciesUpgradeSnapshot>();
        bool usingAuthoredRewardOptions;
        bool usingExperimentalHerbivoreMutations;

        SpeciesId playerSpecies;
        readonly List<SpeciesId> rosterSpecies = new List<SpeciesId>();
        readonly List<SpeciesId> playableSpecies = new List<SpeciesId>();
        IReadOnlyDictionary<SpeciesId, SpeciesRules> rules;
        SpeciesProgression progression;
        SpeciesProgression coupledResponseProgression;
        readonly List<SpeciesUpgradeSnapshot> appliedRunUpgrades = new List<SpeciesUpgradeSnapshot>();
        readonly List<SpeciesUpgradeSnapshot> journeyStartingUpgrades = new List<SpeciesUpgradeSnapshot>();
        readonly Dictionary<string, JourneyConditionOption> journeyConditionOptions =
            new Dictionary<string, JourneyConditionOption>(StringComparer.Ordinal);
        readonly HashSet<string> journeyVisitedNodeIds = new HashSet<string>(StringComparer.Ordinal);
        SpeciesUpgrade[] journeyRewardOptions = Array.Empty<SpeciesUpgrade>();
        JourneyNodeAsset journeyCurrentNode;
        JourneyNodeAsset journeyPendingNode;
        SpeciesUpgradeSnapshot activeJourneyCondition;
        JourneyNodeAsset chosenJourneyNode;
        string chosenJourneyNodeDisplayName;
        bool journeyActive;
        GenomeSimulationSnapshot activeGenomeSnapshot = GenomeSimulationSnapshot.Empty;
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
        bool endingByPlayer;
        bool lastExpeditionFailed;
        bool lastRunEndedEarly;
        string settingsMessage;
        string lastExperimentalUpgradeId;
        int experimentalOfferRotation;
        bool phaseDecisionCommitted;
        int lastSettledPhaseIndex = -1;
        bool selectedUpgradeAppliedToCurrentRun;
        string phaseRewardMessage;
        bool savedSettingsLoaded;

        const string DefaultSettingsKey = "SaltyGame.SpeciesSimulationPreview.DefaultSettings.v5";
        const string PreviousDefaultSettingsKey = "SaltyGame.SpeciesSimulationPreview.DefaultSettings.v4";
        const string LegacyDefaultSettingsKey = "SaltyGame.SpeciesSimulationPreview.DefaultSettings.v3";

        public SimulationRunState Run => simulationHelper?.Run ?? simulationManager?.Run;
        public SpeciesProgression Progression => progression;
        public bool LastExpeditionFailed => lastExpeditionFailed;
        public bool LastRunEndedEarly => lastRunEndedEarly;
        public int PurchasedUpgradeCount => progression?.PurchasedUpgradeCount ?? 0;
        public int GetUpgradeLevel(string upgradeId) => progression?.GetUpgradeLevel(upgradeId) ?? 0;
        public int RewardOptionCount => usingAuthoredRewardOptions
            ? authoredRewardOptions.Length
            : previewState == SpeciesPreviewState.PhaseDecision
                ? rewardOptions.Length
                : rewardOptions.Length > 0
                    && rewardOptions[rewardOptions.Length - 1].Type == SpeciesUpgradeType.PopulationReinforcement
                    ? rewardOptions.Length - 1
                    : rewardOptions.Length;
        public string GetRewardOptionId(int rewardIndex)
        {
            if (usingAuthoredRewardOptions)
            {
                return rewardIndex >= 0 && rewardIndex < authoredRewardOptions.Length
                    ? authoredRewardOptions[rewardIndex].Id
                    : string.Empty;
            }

            return rewardIndex >= 0 && rewardIndex < RewardOptionCount
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

            if (rewardIndex < 0 || rewardIndex >= RewardOptionCount)
            {
                return string.Empty;
            }

            var legacyUpgrade = rewardOptions[rewardIndex];
            if (usingExperimentalHerbivoreMutations && previewState == SpeciesPreviewState.PhaseDecision)
            {
                var mutationDisplay = FormatHerbivoreMutationChoice(
                    legacyUpgrade.Id,
                    (progression?.GetUpgradeLevel(legacyUpgrade.Id) ?? 0) + 1);
                if (coupledSpeciesResponsesEnabled
                    && SpeciesUpgradeCatalog.TryGetCoupledResponse(
                        playerSpecies,
                        legacyUpgrade.Id,
                        out var coupledResponderSpecies,
                        out var coupledResponseUpgradeId))
                {
                    return string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}\n{1} responds: {2}",
                        mutationDisplay,
                        CultureInfo.InvariantCulture.TextInfo.ToTitleCase(coupledResponderSpecies.Value),
                        SpeciesUpgradeCatalog.GetDisplayName(coupledResponseUpgradeId));
                }

                return mutationDisplay;
            }

            var legacySnapshot = legacyUpgrade.CreateSnapshot(playerSpecies);
            var effectSummary = legacySnapshot.PopulationToAdd > 0
                ? $"+{legacySnapshot.PopulationToAdd} {playerSpecies.Value.ToUpperInvariant()}"
                : legacySnapshot.PreContactAvoidanceChanceBonus > 0f
                    ? $"+{legacySnapshot.PreContactAvoidanceChanceBonus:P0} attack avoidance"
                : string.Join(", ", legacySnapshot.Modifiers.Select(FormatModifierForDisplay));
            var display = string.Format(
                CultureInfo.InvariantCulture,
                "{0}\n{1}\n{2}\n{3}",
                SpeciesUpgradeCatalog.GetDisplayName(legacyUpgrade.Id),
                effectSummary,
                previewState == SpeciesPreviewState.PhaseDecision
                    ? "FREE"
                    : $"COST {legacyUpgrade.Cost} DATA",
                GetLegacyRewardStatus(legacyUpgrade));
            if (!coupledSpeciesResponsesEnabled
                || !SpeciesUpgradeCatalog.TryGetCoupledResponse(
                    playerSpecies,
                    legacyUpgrade.Id,
                    out var responseSpecies,
                    out var responseUpgradeId))
            {
                return display;
            }

            var response = SpeciesUpgradeCatalog.Create(responseUpgradeId).CreateSnapshot(responseSpecies);
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}\n{1} responds: {2}\n{3}",
                display,
                CultureInfo.InvariantCulture.TextInfo.ToTitleCase(responseSpecies.Value),
                SpeciesUpgradeCatalog.GetDisplayName(responseUpgradeId),
                string.Join(", ", response.Modifiers.Select(FormatModifierForDisplay)));
        }

        public string GetSelectedUpgradeSummary()
        {
            if (selectedUpgradeSnapshot != null)
            {
                if (usingExperimentalHerbivoreMutations
                    && selectedUpgradeAppliedToCurrentRun
                    && SpeciesUpgradeCatalog.IsExperimentalHerbivoreMutationId(selectedUpgradeSnapshot.Id))
                {
                    return FormatHerbivoreMutationChoice(
                        selectedUpgradeSnapshot.Id,
                        progression.GetUpgradeLevel(selectedUpgradeSnapshot.Id));
                }

                return FormatSnapshotSummary(selectedUpgradeSnapshot, selectedUpgradeAppliedToCurrentRun);
            }

            return selectedUpgrade == null
                ? "No upgrade selected."
                : $"{SpeciesUpgradeCatalog.GetDisplayName(selectedUpgrade.Id)} — legacy upgrade applied to the next run.";
        }
        public SpeciesPreviewState State => previewState;
        public bool WrapEdges => wrapEdges;
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
        public bool JourneyActive => journeyActive;
        public JourneyMapAsset JourneyMap => journeyMap;
        public JourneyNodeAsset JourneyCurrentNode => journeyCurrentNode;
        public JourneyNodeAsset JourneyPendingNode => journeyPendingNode;
        public int JourneyRewardOptionCount => journeyPendingNode?.Kind == JourneyNodeKind.Reward
            ? journeyRewardOptions.Length
            : journeyPendingNode?.Kind == JourneyNodeKind.Condition ? 1 : 0;
        public JourneyNodeAsset ChosenJourneyNode => chosenJourneyNode;
        public string ChosenJourneyNodeDisplayName => chosenJourneyNodeDisplayName;
        public SpeciesUpgradeSnapshot ActiveJourneyCondition => activeJourneyCondition;
        public int JourneyCycleIndex => !journeyActive || Run == null
            ? 1
            : Mathf.Clamp((Run.PhaseIndex - 1) / ContinuousExpeditionPhaseCount + 1,
                1, JourneyPrototypeCycleCount);
        public float PlantProbability => plantProbability;
        public float HerbivoreProbability => herbivoreProbability;
        public float CarnivoreProbability => carnivoreProbability;
        public int PlantStartingPopulation => plantStartingPopulation;
        public int HerbivoreStartingPopulation => herbivoreStartingPopulation;
        public int CarnivoreStartingPopulation => carnivoreStartingPopulation;
        public bool RandomizeSeedOnStart => randomizeSeedOnStart;
        public bool BevExperimentalFeaturesEnabled => bevExperimentalFeaturesEnabled;
        public int FoxAttackCooldownTicks => foxAttackCooldownTicks;
        public bool CoupledSpeciesResponsesEnabled => coupledSpeciesResponsesEnabled;
        public IReadOnlyDictionary<SpeciesId, SpeciesRules> ActiveSpeciesRules => rules;
        public IReadOnlyList<ScenarioDefinitionAsset> ScenarioOptions => scenarioOptions;
        public IReadOnlyList<SpeciesId> RosterSpecies => rosterSpecies;
        public IReadOnlyList<SpeciesId> PlayableSpecies => playableSpecies;
        public SpeciesId PlayerSpecies => playerSpecies;
        public int SelectedScenarioIndex => selectedScenarioIndex;
        public ScenarioDefinitionAsset SelectedScenario => GetSelectedScenario();
        public string SettingsMessage => settingsMessage ?? string.Empty;
        public string PhaseRewardMessage => phaseRewardMessage ?? string.Empty;
        public GenomeSimulationSnapshot ActiveGenomeSnapshot => activeGenomeSnapshot;
        public bool SettingsEditable => previewState == SpeciesPreviewState.Ready && !sessionStarted;

        public bool TrySetJourneyPrototypeEnabled(bool enabled)
        {
            if (!SettingsEditable)
            {
                return false;
            }

            journeyPrototypeEnabled = enabled;
            PrepareNextRun();
            return true;
        }

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

            if (!TrySetPlayerSpecies(
                launch.PlayerSpeciesId,
                launch.ActiveGenomeSnapshot,
                out validationMessage))
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

                appliedRunUpgrades.Add(upgrade);
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

        public void ConfigureScenarioOptions(IReadOnlyList<ScenarioDefinitionAsset> options, int initialSelection = 0)
        {
            var wasUnconfigured = scenarioOptions.Count == 0;
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
                ApplySelectedScenario();
                UseJourneyCadenceForMappedScenario();
                if (wasUnconfigured && savedSettingsLoaded)
                {
                    LoadSavedSettings();
                    UseJourneyCadenceForMappedScenario();
                }
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
            ApplySelectedScenario();
            UseJourneyCadenceForMappedScenario();
            ResetToStart();
            settingsMessage = SelectedScenario == null
                ? "Legacy defaults selected."
                : $"Scenario '{SelectedScenario.name}' selected.";
            return true;
        }

        public bool TrySetPlayerSpecies(string speciesKey, out string validationMessage)
        {
            return TrySetPlayerSpecies(speciesKey, activeGenomeSnapshot, out validationMessage);
        }

        bool TrySetPlayerSpecies(
            string speciesKey,
            GenomeSimulationSnapshot requestedGenomeSnapshot,
            out string validationMessage)
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
            UseJourneyCadenceForMappedScenario();
            ResetExpeditionProgression(selectedRules);
            activeGenomeSnapshot = (requestedGenomeSnapshot ?? GenomeSimulationSnapshot.Empty)
                .IncludeSpecies(rules.Keys);
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
            bevExperimentalFeaturesEnabled = true;
            ApplySelectedScenario();
            LoadSavedSettings();
            UseJourneyCadenceForMappedScenario();
            savedSettingsLoaded = true;
            ResetToStart();
        }

        void UseJourneyCadenceForMappedScenario()
        {
            if (journeyPrototypeEnabled
                && journeyMap != null
                && journeyMap.AppliesTo(SelectedScenario, playerSpecies))
            {
                continuousPhasesEnabled = true;
            }
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
                phaseRewardMessage = FormatPhaseDecisionSummary(run, phaseResult.CurrencyEarned);
                if (journeyActive && run.PhaseIndex == ContinuousExpeditionPhaseCount)
                {
                    previewState = SpeciesPreviewState.JourneyDecision;
                    return;
                }

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

            var isExpedition = continuousPhasesEnabled && run?.SupportsContinuation == true;
            var playerPopulation = run != null && run.PopulationHistory.Count > 0
                ? run.PopulationHistory[run.PopulationHistory.Count - 1].GetCount(run.PlayerSpeciesId)
                : 0;
            lastExpeditionFailed = isExpedition && playerPopulation == 0;
            lastRunEndedEarly = run != null
                && !lastExpeditionFailed
                && run.Tick < run.TargetTicks;
            var forfeitsRewards = lastExpeditionFailed
                || lastRunEndedEarly
                || endingByPlayer;
            result = SimulationRunResults.Create(run, rewardsEligible: !forfeitsRewards);
            RunCompleted?.Invoke(this, run);
            if (forfeitsRewards)
            {
                if (progression != null && progression.Currency > 0)
                {
                    progression.TrySpend(progression.Currency);
                }
            }
            else
            {
                progression?.AddCurrency(result.CurrencyEarned);
            }
            rewardGranted = true;

            // Continuous runs only offer upgrades at phase boundaries. A
            // terminal result must not fall back into the legacy reward flow.
            if ((continuousPhasesEnabled && run?.SupportsContinuation == true)
                || forfeitsRewards)
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
                if (!sessionStarted
                    && !(journeyActive && journeyCurrentNode?.Kind == JourneyNodeKind.Simulation
                        && journeyCurrentNode.Row == 1))
                {
                    if (SelectedScenario == null)
                    {
                        rules = CreateRulesFromDrafts();
                    }

                    var startingUpgrades = progression?.AppliedRunUpgrades.ToArray()
                        ?? Array.Empty<SpeciesUpgradeSnapshot>();
                    var startingRules = startingUpgrades.Length > 0
                        ? progression.Definition.Rules
                        : rules[playerSpecies];
                    ResetExpeditionProgression(startingRules);
                    journeyStartingUpgrades.Clear();
                    foreach (var upgrade in startingUpgrades)
                    {
                        if (!progression.TryApplyRunUpgrade(upgrade))
                        {
                            throw new InvalidOperationException(
                                $"Starting upgrade '{upgrade.Id}' could not be restored for the journey.");
                        }

                        journeyStartingUpgrades.Add(upgrade);
                        appliedRunUpgrades.Add(upgrade);
                    }

                    rules = new Dictionary<SpeciesId, SpeciesRules>(rules)
                    {
                        [playerSpecies] = progression.CurrentRules,
                    };
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

        public bool TryApplyGlobalSettingsForTicksWithStartingPopulations(
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
            string plantStartingPopulationValue,
            string herbivoreStartingPopulationValue,
            string carnivoreStartingPopulationValue,
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
                out validationMessage,
                plantStartingPopulationValue,
                herbivoreStartingPopulationValue,
                carnivoreStartingPopulationValue);
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
            out string validationMessage,
            string plantStartingPopulationValue = null,
            string herbivoreStartingPopulationValue = null,
            string carnivoreStartingPopulationValue = null)
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
            var parsedPlantStartingPopulation = 0;
            var parsedHerbivoreStartingPopulation = 0;
            var parsedCarnivoreStartingPopulation = 0;
            var hasStartingPopulationValues = plantStartingPopulationValue != null
                || herbivoreStartingPopulationValue != null
                || carnivoreStartingPopulationValue != null;
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
                || !TryParseFloat(carnivoreProbabilityValue, "Carnivore probability", out parsedCarnivoreProbability)
                || (hasStartingPopulationValues
                    && (plantStartingPopulationValue == null
                        || herbivoreStartingPopulationValue == null
                        || carnivoreStartingPopulationValue == null
                        || !TryParseInt(plantStartingPopulationValue, "Plant starting population", out parsedPlantStartingPopulation)
                        || !TryParseInt(herbivoreStartingPopulationValue, "Herbivore starting population", out parsedHerbivoreStartingPopulation)
                        || !TryParseInt(carnivoreStartingPopulationValue, "Carnivore starting population", out parsedCarnivoreStartingPopulation))))
            {
                validationMessage = settingsMessage;
                return false;
            }

            if (hasStartingPopulationValues
                && (parsedPlantStartingPopulation < 0
                    || parsedHerbivoreStartingPopulation < 0
                    || parsedCarnivoreStartingPopulation < 0))
            {
                validationMessage = "Starting populations cannot be negative.";
                settingsMessage = validationMessage;
                return false;
            }

            var nextWidth = Mathf.Max(1, parsedWidth);
            var nextHeight = Mathf.Max(1, parsedHeight);
            var nextMaximumPopulation = Mathf.Max(0, parsedMaximumPopulation);
            var nextStepInterval = Mathf.Max(0.01f, parsedStepInterval);
            var nextRunTicks = runWindowIsTicks ? Mathf.Max(1, parsedRunTicks) : 0;
            var nextRunDuration = runWindowIsTicks
                ? (float)(nextRunTicks * (double)nextStepInterval)
                : Mathf.Max(1f, parsedRunDuration);
            if (runWindowIsTicks)
            {
                if (float.IsNaN(nextRunDuration) || float.IsInfinity(nextRunDuration))
                {
                    validationMessage = "Run ticks and step interval produce an invalid run duration.";
                    settingsMessage = validationMessage;
                    return false;
                }
            }

            var totalStartingPopulation = hasStartingPopulationValues
                ? (long)parsedPlantStartingPopulation
                    + parsedHerbivoreStartingPopulation
                    + parsedCarnivoreStartingPopulation
                : startingPopulationOverrideEnabled
                    ? (long)plantStartingPopulation
                        + herbivoreStartingPopulation
                        + carnivoreStartingPopulation
                    : 0L;
            if (SelectedScenario != null
                && ((hasStartingPopulationValues && totalStartingPopulation == 0)
                    || (!hasStartingPopulationValues && !startingPopulationOverrideEnabled)))
            {
                foreach (var population in SelectedScenario.CreateRuntimeData().StartingPopulations.Values)
                {
                    totalStartingPopulation += population;
                }
            }

            if (totalStartingPopulation > (long)nextWidth * nextHeight)
            {
                validationMessage = "Starting populations cannot exceed the grid capacity.";
                settingsMessage = validationMessage;
                return false;
            }

            if (nextMaximumPopulation > 0 && totalStartingPopulation > nextMaximumPopulation)
            {
                validationMessage = $"Starting populations total {totalStartingPopulation} cannot exceed maximum population {nextMaximumPopulation}.";
                settingsMessage = validationMessage;
                return false;
            }

            width = nextWidth;
            height = nextHeight;
            seed = parsedSeed;
            maxPopulation = nextMaximumPopulation;
            minPopulation = Mathf.Max(0, parsedMinimumPopulation);
            stepInterval = nextStepInterval;
            runTicks = nextRunTicks;
            runDurationSeconds = nextRunDuration;
            plantProbability = Mathf.Clamp01(parsedPlantProbability);
            herbivoreProbability = Mathf.Clamp01(parsedHerbivoreProbability);
            carnivoreProbability = Mathf.Clamp01(parsedCarnivoreProbability);
            if (hasStartingPopulationValues)
            {
                var configuredStartingPopulation = (long)parsedPlantStartingPopulation
                    + parsedHerbivoreStartingPopulation
                    + parsedCarnivoreStartingPopulation;

                plantStartingPopulation = parsedPlantStartingPopulation;
                herbivoreStartingPopulation = parsedHerbivoreStartingPopulation;
                carnivoreStartingPopulation = parsedCarnivoreStartingPopulation;
                startingPopulationOverrideEnabled = configuredStartingPopulation > 0;
                if (!startingPopulationOverrideEnabled && SelectedScenario != null)
                {
                    var authoredData = SelectedScenario.CreateRuntimeData();
                    plantStartingPopulation = GetStartingPopulation(authoredData, SpeciesIds.Plant, SpeciesRole.Plant);
                    herbivoreStartingPopulation = GetStartingPopulation(authoredData, SpeciesIds.Herbivore, SpeciesRole.Herbivore);
                    carnivoreStartingPopulation = GetStartingPopulation(authoredData, SpeciesIds.Carnivore, SpeciesRole.Carnivore);
                }
            }
            randomizeSeedOnStart = randomizeSeed;
            settingsMessage = hasStartingPopulationValues
                ? startingPopulationOverrideEnabled
                    ? $"Settings applied to the next run. Starting populations: plant {plantStartingPopulation}, herbivore {herbivoreStartingPopulation}, carnivore {carnivoreStartingPopulation}."
                    : "Settings applied to the next run. Starting populations use the selected scenario defaults."
                : "Global settings applied to the next run.";
            PrepareNextRun();
            validationMessage = settingsMessage;
            return true;
        }

        public bool TryApplyWrapEdges(bool enabled, out string validationMessage)
        {
            validationMessage = "Wrapping can only be changed before a session starts.";
            if (!SettingsEditable) return false;
            wrapEdges = enabled;
            ResetToStart();
            validationMessage = enabled ? "Grid edges connected." : "Grid edges bounded.";
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

            var phaseLimit = ContinuousExpeditionPhaseCount
                * (journeyPrototypeEnabled
                    && journeyMap != null && journeyMap.AppliesTo(SelectedScenario, playerSpecies)
                    ? JourneyPrototypeCycleCount : 1);
            if (parsedPhaseLength > int.MaxValue / phaseLimit)
            {
                validationMessage = "Phase length is too large for this expedition.";
                settingsMessage = validationMessage;
                return false;
            }

            continuousPhasesEnabled = true;
            phaseLengthTicks = parsedPhaseLength;
            settingsMessage = $"Continuous phases enabled: {phaseLimit} phases, decision every {phaseLengthTicks} ticks.";
            validationMessage = settingsMessage;
            return true;
        }

        public bool TryApplyExperimentalFeatures(
            bool enabled,
            string foxAttackCooldownValue,
            out string validationMessage)
        {
            return TryApplyExperimentalFeatures(
                enabled,
                false,
                foxAttackCooldownValue,
                out validationMessage);
        }

        public bool TryApplyExperimentalFeatures(
            bool enabled,
            bool coupledResponsesEnabled,
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

            // The Bev player path is now the default and has no UI opt-out.
            bevExperimentalFeaturesEnabled = true;
            foxAttackCooldownTicks = parsedCooldown;
            coupledSpeciesResponsesEnabled = enabled && coupledResponsesEnabled;
            lastExperimentalUpgradeId = null;
            experimentalOfferRotation = 0;
            rewardOptions = LegacyRewardOptions;
            settingsMessage = $"Bev features enabled: opposed-roll combat, species stat lines, shared Hare skills, fox cooldown {foxAttackCooldownTicks} ticks, coupled responses {(coupledSpeciesResponsesEnabled ? "on" : "off")}.";
            // StartSimulation prepares the pending run after all setup fields are applied.
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
                CrowdingMetabolismMultiplier = draft.CrowdingMetabolismMultiplier.ToString(CultureInfo.InvariantCulture),
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
                || !TryParseInt(values.CrowdingMetabolismMultiplier, "Crowding metabolism multiplier", out var crowdingMetabolismMultiplier)
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
            draft.CrowdingMetabolismMultiplier = Mathf.Max(1, crowdingMetabolismMultiplier);
            draft.CrowdingMetabolismMultiplierText = draft.CrowdingMetabolismMultiplier.ToString(CultureInfo.InvariantCulture);
            draft.StartingFoodReserve = Mathf.Max(0f, startingFoodReserve);
            draft.StartingFoodReserveText = FormatFloat(draft.StartingFoodReserve);
            draft.SeedDropEnabled = values.SeedDropEnabled;
            draft.SeedDropChance = Mathf.Clamp01(seedDropChance);
            draft.SeedDropChanceText = FormatFloat(draft.SeedDropChance);

            rules = CreateRulesFromDrafts();
            ResetExpeditionProgression(rules[playerSpecies]);
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

        public bool AdvanceOneTickWhilePaused()
        {
            return simulationHelper != null
                ? simulationHelper.AdvanceOneTickWhilePaused()
                : simulationManager != null && simulationManager.AdvanceOneTickWhilePaused();
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
                && rewardOptions[rewardIndex].Type != SpeciesUpgradeType.PopulationReinforcement
                && progression.CanPurchase(rewardOptions[rewardIndex]);
        }

        public bool CanBuyHare => previewState == SpeciesPreviewState.PhaseDecision
            && playerSpecies.Value == "hare"
            && !phaseDecisionCommitted
            && Run?.Status == SimulationRunStatus.AwaitingDecision
            && progression?.Currency >= HareCost
            && CanAddBoundaryPopulation(
                SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.PopulationReinforcementId)
                    .CreateSnapshot(playerSpecies));

        public bool BuyHare()
        {
            if (!CanBuyHare
                || !(simulationHelper != null
                    ? simulationHelper.TryAddBoundaryPopulation(playerSpecies, 1)
                    : simulationManager != null
                        && simulationManager.TryAddBoundaryPopulation(playerSpecies, 1)))
            {
                return false;
            }

            if (!progression.TrySpend(HareCost))
            {
                throw new InvalidOperationException("Validated Hare purchase could not be charged.");
            }

            return true;
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
            if (authoredUpgrade == null || !authoredUpgrade.CanApplyAfterRunStart)
            {
                return false;
            }

            SpeciesProgression plannedResponseProgression = null;
            SpeciesUpgrade responseUpgrade = null;
            SpeciesId responseSpecies = default;
            if (!usingAuthoredRewardOptions
                && coupledSpeciesResponsesEnabled
                && SpeciesUpgradeCatalog.TryGetCoupledResponse(
                    playerSpecies,
                    rewardOptions[rewardIndex].Id,
                    out responseSpecies,
                    out var responseUpgradeId))
            {
                if (!rules.TryGetValue(responseSpecies, out var responseRules))
                {
                    return false;
                }

                plannedResponseProgression = coupledResponseProgression
                    ?? new SpeciesProgression(new SpeciesDefinition(responseSpecies, responseRules));
                responseUpgrade = SpeciesUpgradeCatalog.Create(responseUpgradeId);
                if (!plannedResponseProgression.CanApplyFreeUpgrade(responseUpgrade))
                {
                    return false;
                }
            }

            SynchronizePlayerUpgradeSnapshots();

            if (usingAuthoredRewardOptions)
            {
                if (!progression.TryApplyRunUpgrade(authoredUpgrade))
                {
                    return false;
                }
            }
            else
            {
                var upgrade = rewardOptions[rewardIndex];
                if (!progression.TryApplyFreeUpgrade(CreateFreeBoundaryMutation(upgrade)))
                {
                    return false;
                }
                if (bevExperimentalFeaturesEnabled)
                {
                    lastExperimentalUpgradeId = upgrade.Id;
                }
            }

            if (!usingAuthoredRewardOptions)
            {
                appliedRunUpgrades.Add(progression.AppliedRunUpgrades[
                    progression.AppliedRunUpgrades.Count - 1]);
            }

            if (responseUpgrade != null)
            {
                coupledResponseProgression = plannedResponseProgression;
                if (!coupledResponseProgression.TryApplyFreeUpgrade(responseUpgrade))
                {
                    throw new InvalidOperationException("Validated coupled response could not be applied.");
                }

                appliedRunUpgrades.Add(coupledResponseProgression.AppliedRunUpgrades[
                    coupledResponseProgression.AppliedRunUpgrades.Count - 1]);
            }

            var nextRules = new Dictionary<SpeciesId, SpeciesRules>(rules)
            {
                [playerSpecies] = progression.CurrentRules,
            };
            if (coupledResponseProgression != null)
            {
                nextRules[coupledResponseProgression.Definition.Id] = coupledResponseProgression.CurrentRules;
            }
            var continued = simulationHelper != null
                ? simulationHelper.ContinueWithBoundaryState(
                    nextRules,
                    CreateExperimentalOptions(),
                    GetAppliedRunUpgrades(),
                    authoredUpgrade)
                : simulationManager != null
                    && simulationManager.ContinueWithBoundaryState(
                        nextRules,
                        CreateExperimentalOptions(),
                        GetAppliedRunUpgrades(),
                        authoredUpgrade);
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
            phaseRewardMessage = responseUpgrade == null
                ? string.Empty
                : $"{SpeciesUpgradeCatalog.GetDisplayName(rewardOptions[rewardIndex].Id)} triggered {SpeciesUpgradeCatalog.GetDisplayName(responseUpgrade.Id)} for {responseSpecies.Value}.";
            return true;
        }

        bool CanPurchaseLegacyBoundaryReward(SpeciesUpgrade upgrade)
        {
            if (upgrade == null)
            {
                return false;
            }

            var mutation = CreateFreeBoundaryMutation(upgrade);
            var canApply = progression.CanApplyFreeUpgrade(mutation);
            if (!canApply)
            {
                return false;
            }

            var snapshot = mutation.CreateSnapshot(playerSpecies);
            if (!snapshot.CanApplyAfterRunStart || !CanAddBoundaryPopulation(snapshot))
            {
                return false;
            }

            if (!coupledSpeciesResponsesEnabled
                || !SpeciesUpgradeCatalog.TryGetCoupledResponse(
                    playerSpecies,
                    upgrade.Id,
                    out var responseSpecies,
                    out var responseUpgradeId))
            {
                return snapshot.CanApplyAfterRunStart;
            }

            if (!rules.TryGetValue(responseSpecies, out var responseRules))
            {
                return false;
            }

            var responseProgression = coupledResponseProgression
                ?? new SpeciesProgression(new SpeciesDefinition(responseSpecies, responseRules));
            return responseProgression.CanApplyFreeUpgrade(
                SpeciesUpgradeCatalog.Create(responseUpgradeId));
        }

        string GetLegacyRewardStatus(SpeciesUpgrade upgrade)
        {
            if (progression == null)
            {
                return "UNAVAILABLE";
            }

            if (progression.GetUpgradeLevel(upgrade.Id) >= SpeciesUpgradeCatalog.GetMaxLevel(upgrade.Id))
            {
                return "MAX LEVEL";
            }

            if (upgrade.Type == SpeciesUpgradeType.PopulationReinforcement
                && previewState != SpeciesPreviewState.PhaseDecision)
            {
                return "AVAILABLE AT PHASE BREAK";
            }

            if (upgrade.Type == SpeciesUpgradeType.PopulationReinforcement
                && !CanAddBoundaryPopulation(upgrade.CreateSnapshot(playerSpecies)))
            {
                return "NO ROOM FOR REINFORCEMENTS";
            }

            return previewState != SpeciesPreviewState.PhaseDecision
                && progression.Currency < upgrade.Cost
                ? $"NEED {upgrade.Cost - progression.Currency} more data"
                : "AVAILABLE";
        }

        SpeciesUpgradeSnapshot GetBoundaryUpgrade(int rewardIndex)
        {
            if (usingAuthoredRewardOptions)
            {
                return rewardIndex >= 0 && rewardIndex < authoredRewardOptions.Length
                    ? authoredRewardOptions[rewardIndex]
                    : null;
            }

            if (rewardIndex < 0 || rewardIndex >= rewardOptions.Length)
            {
                return null;
            }

            var upgrade = CreateFreeBoundaryMutation(rewardOptions[rewardIndex]);
            return upgrade.CreateSnapshot(playerSpecies);
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

        public bool HasVisitedJourneyNode(JourneyNodeAsset node)
        {
            return node != null && journeyVisitedNodeIds.Contains(node.NodeId);
        }

        public bool CanChooseJourneyNode(string nodeId)
        {
            var node = journeyMap?.GetNode(nodeId);
            var run = Run;
            if (!journeyActive || node == null || !node.PlayableInPrototype
                || journeyPendingNode != null || run == null)
            {
                return false;
            }

            if (journeyCurrentNode == null)
            {
                return node == journeyMap.StartNode
                    && node.Kind == JourneyNodeKind.Reward
                    && previewState == SpeciesPreviewState.Ready
                    && run.Status == SimulationRunStatus.Ready;
            }

            if (!journeyMap.CanChooseAfter(journeyCurrentNode, node))
            {
                return false;
            }

            if (node.Kind == JourneyNodeKind.Simulation && node.Row == 1)
            {
                return previewState == SpeciesPreviewState.Ready
                    && run.Status == SimulationRunStatus.Ready;
            }

            if (node.Kind == JourneyNodeKind.Condition && node.Row == 2)
            {
                return previewState == SpeciesPreviewState.JourneyDecision
                    && run.Status == SimulationRunStatus.AwaitingDecision
                    && run.PhaseIndex == ContinuousExpeditionPhaseCount
                    && !phaseDecisionCommitted
                    && journeyConditionOptions.ContainsKey(nodeId);
            }

            return node.Kind == JourneyNodeKind.Simulation
                && node.Row == 3
                && previewState == SpeciesPreviewState.JourneyDecision
                && run.Status == SimulationRunStatus.AwaitingDecision
                && run.PhaseIndex == ContinuousExpeditionPhaseCount
                && !phaseDecisionCommitted
                && activeJourneyCondition != null;
        }

        public bool ChooseJourneyNode(string nodeId)
        {
            if (!CanChooseJourneyNode(nodeId))
            {
                return false;
            }

            var node = journeyMap.GetNode(nodeId);
            if (node.Kind == JourneyNodeKind.Simulation)
            {
                if (node.Row == 1)
                {
                    var previous = journeyCurrentNode;
                    journeyCurrentNode = node;
                    StartSimulation();
                    if (previewState != SpeciesPreviewState.Running)
                    {
                        journeyCurrentNode = previous;
                        return false;
                    }
                }
                else
                {
                    var nextRules = new Dictionary<SpeciesId, SpeciesRules>(rules)
                    {
                        [playerSpecies] = progression.CurrentRules,
                    };
                    var continued = simulationHelper != null
                        ? simulationHelper.ContinueWithBoundaryState(
                            nextRules, CreateExperimentalOptions(),
                            GetAppliedRunUpgrades(), activeJourneyCondition)
                        : simulationManager != null
                            && simulationManager.ContinueWithBoundaryState(
                                nextRules, CreateExperimentalOptions(),
                                GetAppliedRunUpgrades(), activeJourneyCondition);
                    if (!continued)
                    {
                        return false;
                    }

                    rules = nextRules;
                    phaseDecisionCommitted = true;
                    previewState = SpeciesPreviewState.Running;
                    journeyCurrentNode = node;
                }

                journeyVisitedNodeIds.Add(node.NodeId);
                return true;
            }

            if (node.Kind == JourneyNodeKind.Reward)
            {
                var offers = new List<SpeciesUpgrade>();
                foreach (var mutationId in node.RewardMutationIds)
                {
                    if (!SpeciesUpgradeCatalog.IsExperimentalHerbivoreMutationId(mutationId))
                    {
                        Debug.LogError($"Journey reward '{node.NodeId}' has unknown Mutation '{mutationId}'.");
                        return false;
                    }

                    var upgrade = SpeciesUpgradeCatalog.Create(mutationId);
                    if (progression.CanApplyFreeUpgrade(CreateFreeBoundaryMutation(upgrade)))
                    {
                        offers.Add(upgrade);
                    }
                }

                if (offers.Count == 0)
                {
                    return false;
                }

                journeyRewardOptions = offers.ToArray();
            }
            else if (node.Kind != JourneyNodeKind.Condition)
            {
                return false;
            }

            journeyPendingNode = node;
            previewState = SpeciesPreviewState.JourneyNodeReward;
            return true;
        }

        public string GetJourneyRewardOptionDisplayName(int index)
        {
            if (journeyPendingNode?.Kind == JourneyNodeKind.Reward
                && index >= 0 && index < journeyRewardOptions.Length)
            {
                var upgrade = journeyRewardOptions[index];
                return $"{SpeciesUpgradeCatalog.GetDisplayName(upgrade.Id)}  |  Lv {progression.GetUpgradeLevel(upgrade.Id) + 1}\n{GetHerbivoreMutationDescription(upgrade.Id)}";
            }

            if (journeyPendingNode?.Kind == JourneyNodeKind.Condition
                && index == 0
                && journeyConditionOptions.TryGetValue(journeyPendingNode.NodeId, out var option))
            {
                return option.DataReward > 0
                    ? $"ACCEPT {option.DisplayName}\n+{option.DataReward} FIELD DATA"
                    : $"ACCEPT {option.DisplayName}\nNO IMMEDIATE FIELD DATA";
            }

            return string.Empty;
        }

        public bool ClaimJourneyNodeReward(int index)
        {
            var node = journeyPendingNode;
            if (previewState != SpeciesPreviewState.JourneyNodeReward || node == null
                || index < 0 || index >= JourneyRewardOptionCount)
            {
                return false;
            }

            if (node.Kind == JourneyNodeKind.Reward)
            {
                var upgrade = journeyRewardOptions[index];
                if (!progression.TryApplyFreeUpgrade(CreateFreeBoundaryMutation(upgrade)))
                {
                    return false;
                }

                lastExperimentalUpgradeId = upgrade.Id;
                rules = new Dictionary<SpeciesId, SpeciesRules>(rules)
                {
                    [playerSpecies] = progression.CurrentRules,
                };
                SynchronizePlayerUpgradeSnapshots();
                PrepareNextRun();
                journeyCurrentNode = journeyMap.StartNode;
                journeyVisitedNodeIds.Add(node.NodeId);
                journeyPendingNode = null;
                journeyRewardOptions = Array.Empty<SpeciesUpgrade>();
                phaseRewardMessage = $"First discovery: {SpeciesUpgradeCatalog.GetDisplayName(upgrade.Id)}.";
                return true;
            }

            if (!journeyConditionOptions.TryGetValue(node.NodeId, out var option)
                || option.Condition == null
                || !rules.TryGetValue(option.Condition.TargetSpecies, out var conditionRules))
            {
                return false;
            }

            SpeciesRules affectedRules;
            try
            {
                affectedRules = option.Condition.Apply(conditionRules);
            }
            catch (Exception exception) when (
                exception is ArgumentException
                || exception is InvalidOperationException
                || exception is OverflowException)
            {
                Debug.LogError($"Journey node '{node.NodeId}' is invalid: {exception.Message}");
                return false;
            }

            rules = new Dictionary<SpeciesId, SpeciesRules>(rules)
            {
                [option.Condition.TargetSpecies] = affectedRules,
            };
            progression.AddCurrency(option.DataReward);
            activeJourneyCondition = option.Condition;
            appliedRunUpgrades.Add(option.Condition);
            chosenJourneyNode = node;
            chosenJourneyNodeDisplayName = option.DisplayName;
            journeyCurrentNode = node;
            journeyVisitedNodeIds.Add(node.NodeId);
            journeyPendingNode = null;
            journeyRewardOptions = Array.Empty<SpeciesUpgrade>();
            previewState = SpeciesPreviewState.JourneyDecision;
            phaseRewardMessage = $"{option.DisplayName}: {option.Description}";
            return true;
        }

        public void CancelJourneyNodeReward()
        {
            if (previewState != SpeciesPreviewState.JourneyNodeReward)
            {
                return;
            }

            previewState = journeyCurrentNode == null
                ? SpeciesPreviewState.Ready
                : SpeciesPreviewState.JourneyDecision;
            journeyPendingNode = null;
            journeyRewardOptions = Array.Empty<SpeciesUpgrade>();
        }

        public void EndSimulation()
        {
            if (Run == null
                || (Run.Status != SimulationRunStatus.Running
                    && Run.Status != SimulationRunStatus.Paused
                    && Run.Status != SimulationRunStatus.AwaitingDecision))
            {
                return;
            }

            endingByPlayer = true;
            bool ended;
            try
            {
                ended = simulationHelper != null
                    ? simulationHelper.EndRun()
                    : simulationManager != null && simulationManager.End();
            }
            finally
            {
                endingByPlayer = false;
            }

            if (ended
                && previewState != SpeciesPreviewState.Rewards
                && previewState != SpeciesPreviewState.Results)
            {
                previewState = continuousPhasesEnabled && Run?.SupportsContinuation == true
                    ? SpeciesPreviewState.Results
                    : SpeciesPreviewState.Rewards;
            }
        }

        public void PlayNextSimulation(bool startImmediately = true)
        {
            if (previewState == SpeciesPreviewState.Results)
            {
                // Mutations reset for each expedition; earned field data carries forward.
                var carriedCurrency = progression?.Currency ?? 0;
                rules = CreateRulesFromDrafts().ToDictionary(entry => entry.Key, entry => entry.Value);
                ResetExpeditionProgression(rules[playerSpecies]);
                progression.AddCurrency(carriedCurrency);
                journeyStartingUpgrades.Clear();
                PrepareNextRun();
                if (startImmediately)
                {
                    StartSimulation();
                }
            }
        }

        public void ResetToStart()
        {
            if (randomizeSeedOnStart)
            {
                seed = Guid.NewGuid().GetHashCode();
            }

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
            ResetExpeditionProgression(rules[playerSpecies]);
            journeyStartingUpgrades.Clear();
            runNumber = 0;
            lastExperimentalUpgradeId = null;
            experimentalOfferRotation = 0;
            rewardOptions = LegacyRewardOptions;
            authoredRewardOptions = Array.Empty<SpeciesUpgradeSnapshot>();
            usingAuthoredRewardOptions = false;
            sessionStarted = false;
            activeGenomeSnapshot = GenomeSimulationSnapshot.Empty;
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
                plantStartingPopulation = plantStartingPopulation,
                herbivoreStartingPopulation = herbivoreStartingPopulation,
                carnivoreStartingPopulation = carnivoreStartingPopulation,
                startingPopulationOverrideEnabled = startingPopulationOverrideEnabled,
                runDurationSeconds = runDurationSeconds,
                stepInterval = stepInterval,
                runTicks = runTicks,
                continuousPhasesEnabled = continuousPhasesEnabled,
                phaseLengthTicks = phaseLengthTicks,
                maxPopulation = maxPopulation,
                minPopulation = minPopulation,
                coupledSpeciesResponsesEnabled = coupledSpeciesResponsesEnabled,
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
            var settingsKey = PlayerPrefs.HasKey(DefaultSettingsKey)
                ? DefaultSettingsKey
                : PlayerPrefs.HasKey(PreviousDefaultSettingsKey)
                    ? PreviousDefaultSettingsKey
                    : LegacyDefaultSettingsKey;
            if (!PlayerPrefs.HasKey(settingsKey))
            {
                return;
            }

            SavedSettings saved;
            try
            {
                saved = JsonUtility.FromJson<SavedSettings>(PlayerPrefs.GetString(settingsKey));
            }
            catch (Exception)
            {
                PlayerPrefs.DeleteKey(settingsKey);
                return;
            }
            if (saved == null)
            {
                return;
            }

            if (settingsKey != DefaultSettingsKey && SelectedScenario != null)
            {
                var authoredData = SelectedScenario.CreateRuntimeData();
                saved.width = authoredData.Width;
                saved.height = authoredData.Height;
                PlayerPrefs.SetString(DefaultSettingsKey, JsonUtility.ToJson(saved));
                PlayerPrefs.Save();
            }

            if (SelectedScenario != null && !saved.startingPopulationOverrideEnabled)
            {
                var authoredData = SelectedScenario.CreateRuntimeData();
                var authoredPlant = GetStartingPopulation(authoredData, SpeciesIds.Plant, SpeciesRole.Plant);
                var authoredHerbivore = GetStartingPopulation(authoredData, SpeciesIds.Herbivore, SpeciesRole.Herbivore);
                var authoredCarnivore = GetStartingPopulation(authoredData, SpeciesIds.Carnivore, SpeciesRole.Carnivore);
                saved.plantStartingPopulation = authoredPlant;
                saved.herbivoreStartingPopulation = authoredHerbivore;
                saved.carnivoreStartingPopulation = authoredCarnivore;
            }

            width = Mathf.Max(1, saved.width);
            height = Mathf.Max(1, saved.height);
            seed = saved.seed;
            randomizeSeedOnStart = saved.randomizeSeedOnStart;
            plantProbability = Mathf.Clamp01(saved.plantProbability);
            herbivoreProbability = Mathf.Clamp01(saved.herbivoreProbability);
            carnivoreProbability = Mathf.Clamp01(saved.carnivoreProbability);
            plantStartingPopulation = Mathf.Max(0, saved.plantStartingPopulation);
            herbivoreStartingPopulation = Mathf.Max(0, saved.herbivoreStartingPopulation);
            carnivoreStartingPopulation = Mathf.Max(0, saved.carnivoreStartingPopulation);
            startingPopulationOverrideEnabled = saved.startingPopulationOverrideEnabled;
            runDurationSeconds = Mathf.Max(1f, saved.runDurationSeconds);
            stepInterval = Mathf.Max(0.01f, saved.stepInterval);
            runTicks = Mathf.Max(0, saved.runTicks);
            continuousPhasesEnabled = saved.continuousPhasesEnabled;
            phaseLengthTicks = Mathf.Max(1, saved.phaseLengthTicks);
            maxPopulation = Mathf.Max(0, saved.maxPopulation);
            minPopulation = Mathf.Max(0, saved.minPopulation);
            coupledSpeciesResponsesEnabled = saved.coupledSpeciesResponsesEnabled;
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

        void ResetExpeditionProgression(SpeciesRules playerRules)
        {
            progression = new SpeciesProgression(new SpeciesDefinition(playerSpecies, playerRules));
            coupledResponseProgression = null;
            appliedRunUpgrades.Clear();
        }

        IReadOnlyList<SpeciesUpgradeSnapshot> GetAppliedRunUpgrades()
        {
            SynchronizePlayerUpgradeSnapshots();
            return appliedRunUpgrades;
        }

        void SynchronizePlayerUpgradeSnapshots()
        {
            if (progression == null)
            {
                return;
            }

            foreach (var snapshot in progression.AppliedRunUpgrades)
            {
                if (!appliedRunUpgrades.Contains(snapshot))
                {
                    appliedRunUpgrades.Add(snapshot);
                }
            }
        }

        void PrepareNextRun()
        {
            var currentRules = new Dictionary<SpeciesId, SpeciesRules>(rules)
            {
                [playerSpecies] = progression?.CurrentRules ?? rules[playerSpecies],
            };
            if (coupledResponseProgression != null)
            {
                currentRules[coupledResponseProgression.Definition.Id] = coupledResponseProgression.CurrentRules;
            }
            rules = currentRules;
            var simulationData = CreateSimulationData();
            journeyActive = journeyPrototypeEnabled
                && continuousPhasesEnabled
                && journeyMap != null
                && journeyMap.AppliesTo(SelectedScenario, playerSpecies);
            SnapshotJourneyConditions();
            var phaseCount = ContinuousExpeditionPhaseCount
                * (journeyActive ? JourneyPrototypeCycleCount : 1);
            var continuousRun = continuousPhasesEnabled
                && phaseLengthTicks > 0
                && phaseLengthTicks <= int.MaxValue / phaseCount;
            if (continuousPhasesEnabled && !continuousRun)
            {
                continuousPhasesEnabled = false;
                settingsMessage = "Continuous phases disabled because the phase length is invalid for this expedition.";
                journeyActive = false;
            }

            var targetTicks = continuousRun
                ? phaseLengthTicks * phaseCount
                : simulationData.RunTicks;
            var durationSeconds = continuousRun
                ? (float)(targetTicks * (double)simulationData.StepInterval)
                : simulationData.RunDurationSeconds;

            var run = new SimulationRunState(
                SpeciesInitialGridFactory.Create(simulationData, seed + runNumber),
                playerSpecies,
                seed + runNumber,
                durationSeconds,
                targetTicks,
                activeGenomeSnapshot);
            if (continuousRun)
            {
                run.ConfigureContinuousPhases(phaseLengthTicks);
            }
            var nextRunner = new SpeciesSimulationRunner(
                run,
                simulationData,
                combatResolutionMode: SpeciesCombatResolutionMode.OpposedRoll,
                experimentalOptions: CreateExperimentalOptions(),
                upgradeLoadout: GetAppliedRunUpgrades());
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
            lastExpeditionFailed = false;
            lastRunEndedEarly = false;
            selectedUpgrade = null;
            selectedUpgradeSnapshot = null;
            selectedUpgradeAppliedToCurrentRun = false;
            authoredRewardOptions = Array.Empty<SpeciesUpgradeSnapshot>();
            usingAuthoredRewardOptions = false;
            rewardMessage = string.Empty;
            phaseRewardMessage = string.Empty;
            phaseDecisionCommitted = false;
            lastSettledPhaseIndex = -1;
            journeyVisitedNodeIds.Clear();
            journeyCurrentNode = null;
            journeyPendingNode = null;
            journeyRewardOptions = Array.Empty<SpeciesUpgrade>();
            activeJourneyCondition = null;
            chosenJourneyNode = null;
            chosenJourneyNodeDisplayName = null;
            previewState = SpeciesPreviewState.Ready;
            runNumber++;
        }

        void SnapshotJourneyConditions()
        {
            journeyConditionOptions.Clear();
            if (!journeyActive)
            {
                return;
            }

            var start = journeyMap.StartNode;
            if (start == null || start.Kind != JourneyNodeKind.Reward
                || start.RewardMutationIds.Count == 0 || start.NextNodes.Count != 1)
            {
                throw new InvalidOperationException("The Forest Edge journey needs an opening reward and simulation.");
            }

            var firstSimulation = start.NextNodes[0];
            if (firstSimulation == null || firstSimulation.Kind != JourneyNodeKind.Simulation
                || firstSimulation.Row != 1 || firstSimulation.NextNodes.Count != 2)
            {
                throw new InvalidOperationException("The Forest Edge journey needs two branches after its first simulation.");
            }

            foreach (var mutationId in start.RewardMutationIds)
            {
                if (!SpeciesUpgradeCatalog.IsExperimentalHerbivoreMutationId(mutationId))
                {
                    throw new InvalidOperationException($"Unknown starting Mutation '{mutationId}'.");
                }
            }

            foreach (var node in firstSimulation.NextNodes)
            {
                if (node == null || node.Kind != JourneyNodeKind.Condition || node.Row != 2
                    || node.NextNodes.Count == 0)
                {
                    throw new InvalidOperationException("The Forest Edge journey has an invalid condition node.");
                }

                foreach (var next in node.NextNodes)
                {
                    if (next == null || next.Kind != JourneyNodeKind.Simulation || next.Row != 3)
                    {
                        throw new InvalidOperationException("Each habitat condition needs a connected simulation.");
                    }
                }

                if (!node.TryCreateCondition(out var condition, out var error)
                    || condition.TargetSpecies == playerSpecies
                    || !rules.ContainsKey(condition.TargetSpecies))
                {
                    throw new InvalidOperationException(
                        $"Journey condition '{node.name}' is invalid: {error}");
                }

                journeyConditionOptions.Add(node.NodeId, new JourneyConditionOption(node, condition));
            }
        }

        void PrepareRewardOptions()
        {
            authoredRewardOptions = Array.Empty<SpeciesUpgradeSnapshot>();
            usingAuthoredRewardOptions = false;
            usingExperimentalHerbivoreMutations = false;
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
                || !rules.TryGetValue(playerSpecies, out var playerRules))
            {
                rewardOptions = LegacyRewardOptions;
                return;
            }

            var isPhaseDecision = continuousPhasesEnabled
                && Run?.Status == SimulationRunStatus.AwaitingDecision;
            Func<SpeciesUpgrade, bool> canOffer = upgrade => progression != null
                && (isPhaseDecision
                    ? CanPurchaseLegacyBoundaryReward(upgrade)
                    : progression.CanApplyFreeUpgrade(upgrade));
            rewardOptions = playerRules.Role == SpeciesRole.Herbivore
                ? isPhaseDecision
                    ? SpeciesUpgradeCatalog.CreateExperimentalHerbivoreMutationOffer(
                        lastExperimentalUpgradeId,
                        experimentalOfferRotation,
                        Run?.Seed ?? seed,
                        canOffer)
                    : SpeciesUpgradeCatalog.CreateExperimentalHerbivoreOffer(
                        lastExperimentalUpgradeId,
                        experimentalOfferRotation,
                        Run?.Seed ?? seed,
                        canOffer)
                : playerRules.Role == SpeciesRole.Carnivore
                    ? SpeciesUpgradeCatalog.CreateExperimentalPredatorOffer(
                        lastExperimentalUpgradeId,
                        experimentalOfferRotation,
                        Run?.Seed ?? seed,
                        canOffer)
                    : LegacyRewardOptions;
            usingExperimentalHerbivoreMutations = isPhaseDecision
                && playerRules.Role == SpeciesRole.Herbivore;
            experimentalOfferRotation++;
        }

        static SpeciesUpgrade CreateFreeBoundaryMutation(SpeciesUpgrade upgrade)
        {
            return new SpeciesUpgrade(upgrade.Id, 0, upgrade.Type, upgrade.Value);
        }

        static string FormatHerbivoreMutationChoice(string upgradeId, int level)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}  |  LV {1}\n{2}\nFREE",
                SpeciesUpgradeCatalog.GetDisplayName(upgradeId),
                level,
                GetHerbivoreMutationDescription(upgradeId));
        }

        static string GetHerbivoreMutationDescription(string upgradeId)
        {
            switch (upgradeId)
            {
                case SpeciesUpgradeCatalog.FasterMovementId:
                    return "Move faster to escape predators and reach fresh food.";
                case SpeciesUpgradeCatalog.ToughHideId:
                    return "Block more incoming attacks.";
                case SpeciesUpgradeCatalog.EfficientDigestionId:
                    return "Gain more energy from food.";
                case SpeciesUpgradeCatalog.SeedDispersalId:
                    return "Plant food; spend 1 stored food on success.";
                case SpeciesUpgradeCatalog.CrowdingToleranceId:
                    return "Cut extra crowding cost by 10%.";
                case SpeciesUpgradeCatalog.ReproductiveDriveId:
                    return "Make successful reproduction more likely.";
                case SpeciesUpgradeCatalog.ThreatExposureId:
                    return "Evade predator attacks more often.";
                default:
                    throw new ArgumentOutOfRangeException(nameof(upgradeId), upgradeId, "Unknown Hare Mutation.");
            }
        }

        string FormatPhaseDecisionSummary(SimulationRunState run, int currencyEarned)
        {
            var summary = string.Format(
                CultureInfo.InvariantCulture,
                "Phase {0} complete: {1} data earned from current survivors.",
                run.PhaseIndex,
                currencyEarned);
            if (run.PhaseResults.Count == 0)
            {
                return summary;
            }

            var completedPhase = run.PhaseResults[run.PhaseResults.Count - 1];
            var speciesName = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(playerSpecies.Value);
            summary += string.Format(
                CultureInfo.InvariantCulture,
                " {0} population at phase end: {1}.",
                speciesName,
                completedPhase.ClosingPopulation.GetCount(playerSpecies));
            if (completedPhase.EffectiveUpgradeLoadout.Any(
                    upgrade => upgrade.Id == SpeciesUpgradeCatalog.ToughHideId))
            {
                var blockedIncomingAttacks = completedPhase.Metrics.CombatRollEvents.Count(
                    combat => combat.TargetSpecies == playerSpecies && !combat.Hit);
                summary += string.Format(
                    CultureInfo.InvariantCulture,
                    " Incoming attacks blocked this phase: {0}.",
                    blockedIncomingAttacks);
            }

            return summary;
        }

        SpeciesExperimentalOptions CreateExperimentalOptions()
        {
            return bevExperimentalFeaturesEnabled
                ? new SpeciesExperimentalOptions(
                    SpeciesExperimentalOptions.BevExperimentalFeaturesId,
                    foxAttackCooldownTicks,
                    progression?.PreContactAvoidanceChance ?? 0f,
                    coupledSpeciesResponsesEnabled)
                : SpeciesExperimentalOptions.None;
        }

        bool CanPurchaseAuthoredReward(SpeciesUpgradeSnapshot upgrade)
        {
            if ((previewState != SpeciesPreviewState.Rewards
                    && previewState != SpeciesPreviewState.PhaseDecision)
                || progression == null
                || upgrade == null
                || upgrade.TargetSpecies != playerSpecies
                || (progression.GetUpgradeLevel(upgrade.Id) > 0
                    && (!SpeciesUpgradeCatalog.IsRepeatableRunUpgradeId(upgrade.Id)
                        || upgrade.PopulationToAdd == 0))
                || (previewState != SpeciesPreviewState.PhaseDecision
                    && progression.Currency < upgrade.Cost))
            {
                return false;
            }

            if (upgrade.PopulationToAdd > 0 && previewState != SpeciesPreviewState.PhaseDecision)
            {
                return false;
            }

            if (previewState == SpeciesPreviewState.PhaseDecision
                && (!continuousPhasesEnabled
                    || phaseDecisionCommitted
                    || !upgrade.CanApplyAfterRunStart
                    || !CanAddBoundaryPopulation(upgrade)))
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
            var cost = previewState == SpeciesPreviewState.PhaseDecision
                ? "FREE"
                : $"COST {upgrade.Cost} DATA";
            return $"{upgrade.DisplayName}\n{modifiers}\n{cost}\n{status}";
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

            if (previewState != SpeciesPreviewState.PhaseDecision
                && progression.Currency < upgrade.Cost)
            {
                return $"NEED {upgrade.Cost - progression.Currency} more data";
            }

            return "AVAILABLE";
        }

        static string FormatSnapshotSummary(SpeciesUpgradeSnapshot upgrade, bool appliedToCurrentRun = false)
        {
            var effects = upgrade.PopulationToAdd > 0
                ? $"Adds {upgrade.PopulationToAdd} {upgrade.TargetSpecies.Value} to the next phase at a deterministic open cell."
                : string.Join(", ", upgrade.Modifiers.Select(FormatModifierForDisplay));
            var timing = appliedToCurrentRun
                ? "Added to the next phase and retained if this run restarts."
                : "Applied to the next run.";
            return $"{upgrade.DisplayName} — {upgrade.Description} Effects: {effects}. {timing}";
        }

        bool CanAddBoundaryPopulation(SpeciesUpgradeSnapshot upgrade)
        {
            if (upgrade == null || upgrade.PopulationToAdd == 0)
            {
                return true;
            }

            if (Run == null
                || !rules.TryGetValue(upgrade.TargetSpecies, out _)
                || !SpeciesSimulation.CanAddBoundaryPopulation(Run.Cells, upgrade.PopulationToAdd, maxPopulation))
            {
                return false;
            }

            return SpeciesSimulation.CanAddBoundaryPopulation(
                Run.CopyInitialCells(),
                upgrade.PopulationToAdd,
                maxPopulation);
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
                    crowdingMetabolismMultiplier: draft.CrowdingMetabolismMultiplier,
                    startingFoodReserve: draft.StartingFoodReserve,
                    seedDropChance: draft.SeedDropEnabled ? draft.SeedDropChance : 0f,
                    energyValue: draft.EnergyValue,
                    metabolism: draft.Metabolism,
                    awareness: new SpeciesAwarenessRules(draft.VisionRange, draft.Intelligence),
                    role: draft.Role,
                    forageBelowEnergy: draft.ForageBelowEnergy,
                    maximumEnergy: draft.MaximumEnergy,
                    litterMinimum: draft.LitterMinimum,
                    litterMaximum: draft.LitterMaximum,
                    trackingPersistenceSteps: draft.TrackingPersistenceSteps,
                    behaviorStateRules: draft.BehaviorStateRules,
                    foragesUntilFull: draft.ForagesUntilFull,
                    energyLossIntervalTicks: Math.Max(1, draft.EnergyLossIntervalTicks),
                    forageThresholdFraction: draft.ForageThresholdFraction,
                    matingEnergyThresholdFraction: draft.MatingEnergyThresholdFraction,
                    matingEnergyCostFraction: draft.MatingEnergyCostFraction,
                    distributeMatingEnergyToOffspring: draft.DistributeMatingEnergyToOffspring,
                    crowdingEnergyReduction: draft.CrowdingEnergyReduction);
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
                var scenarioData = SelectedScenario.CreateRuntimeData();
                var authoredData = (startingPopulationOverrideEnabled
                        ? scenarioData.WithGridSizeAndStartingPopulations(
                            width, height, CreateStartingPopulations())
                        : scenarioData.WithGridSize(width, height))
                    .WithWrapEdges(wrapEdges)
                    .WithRunTicks(RunTicks, stepInterval)
                    .WithSpeciesRules(playerSpecies, rules[playerSpecies]);
                return authoredData;
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
                minPopulation,
                startingPopulations: startingPopulationOverrideEnabled
                    ? CreateStartingPopulations()
                    : null,
                wrapEdges: wrapEdges);
        }

        Dictionary<SpeciesId, int> CreateStartingPopulations()
        {
            var populations = new Dictionary<SpeciesId, int>();
            AddStartingPopulation(populations, SpeciesIds.Plant, SpeciesRole.Plant, plantStartingPopulation);
            AddStartingPopulation(populations, SpeciesIds.Herbivore, SpeciesRole.Herbivore, herbivoreStartingPopulation);
            AddStartingPopulation(populations, SpeciesIds.Carnivore, SpeciesRole.Carnivore, carnivoreStartingPopulation);
            return populations;
        }

        void AddStartingPopulation(
            Dictionary<SpeciesId, int> populations,
            SpeciesId canonicalSpecies,
            SpeciesRole role,
            int population)
        {
            var species = ResolveSpeciesId(rules, canonicalSpecies, role);
            if (species.HasValue && population > 0)
            {
                populations[species.Value] = population;
            }
        }

        static SpeciesId? ResolveSpeciesId(
            IReadOnlyDictionary<SpeciesId, SpeciesRules> definitions,
            SpeciesId canonicalSpecies,
            SpeciesRole role)
        {
            if (definitions.ContainsKey(canonicalSpecies))
            {
                return canonicalSpecies;
            }

            foreach (var entry in definitions)
            {
                if (entry.Value.Role == role)
                {
                    return entry.Key;
                }
            }

            return null;
        }

        void ApplySelectedScenario()
        {
            var authoredData = SelectedScenario?.CreateRuntimeData();
            if (authoredData == null)
            {
                rules = new Dictionary<SpeciesId, SpeciesRules>(SpeciesRuleDefaults.Create());
                ruleDrafts = CreateRuleDrafts(rules);
                if (!rules.ContainsKey(playerSpecies) || rules[playerSpecies].IsPlant)
                {
                    playerSpecies = SpeciesIds.Herbivore;
                    playerSpeciesKey = playerSpecies.Value;
                }

                plantStartingPopulation = 0;
                herbivoreStartingPopulation = 0;
                carnivoreStartingPopulation = 0;
                startingPopulationOverrideEnabled = false;

                return;
            }

            width = authoredData.Width;
            height = authoredData.Height;
            runDurationSeconds = authoredData.RunDurationSeconds;
            stepInterval = authoredData.StepInterval;
            runTicks = authoredData.RunTicks;
            maxPopulation = authoredData.MaxPopulation;
            minPopulation = authoredData.MinPopulation;
            plantStartingPopulation = GetStartingPopulation(authoredData, SpeciesIds.Plant, SpeciesRole.Plant);
            herbivoreStartingPopulation = GetStartingPopulation(authoredData, SpeciesIds.Herbivore, SpeciesRole.Herbivore);
            carnivoreStartingPopulation = GetStartingPopulation(authoredData, SpeciesIds.Carnivore, SpeciesRole.Carnivore);
            startingPopulationOverrideEnabled = false;
            rules = new Dictionary<SpeciesId, SpeciesRules>(authoredData.SpeciesRules);
            ruleDrafts = CreateRuleDrafts(rules);
            if (!rules.ContainsKey(playerSpecies))
            {
                playerSpecies = FindPlayableSpecies(rules);
                playerSpeciesKey = playerSpecies.Value;
            }
        }

        static int GetStartingPopulation(CellularSimData data, SpeciesId canonicalSpecies, SpeciesRole role)
        {
            var species = ResolveSpeciesId(data.SpeciesRules, canonicalSpecies, role);
            return species.HasValue && data.StartingPopulations.TryGetValue(species.Value, out var population)
                ? population
                : 0;
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
