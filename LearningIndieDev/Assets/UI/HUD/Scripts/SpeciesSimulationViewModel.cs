using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Noesis;
using UnityEngine;

namespace SaltyGame
{
    public sealed class SpeciesSimulationViewModel : MonoBehaviour, INotifyPropertyChanged
    {
        SpeciesSimulationPreview preview;
        SpeciesSimulationBoard board;
        SpeciesPreviewState lastState;
        SimulationRunStatus lastRunStatus;
        int lastTick = -1;
        string stateTitle;
        string runStatusText;
        string runDetailsText;
        string currencyText;
        string settingsMessage;
        string gridWidthText;
        string gridHeightText;
        string baseSeedText;
        string maximumPopulationText;
        string minimumPopulationText;
        string runDurationText;
        string stepIntervalText;
        string plantProbabilityText;
        string herbivoreProbabilityText;
        string carnivoreProbabilityText;
        bool randomizeSeedOnStart;
        bool canEditSettings;
        bool showXamlSettings = true;
        int selectedRuleSpeciesIndex;
        SpeciesRuleEditValues ruleValues = new SpeciesRuleEditValues();
        readonly string[] speciesTabs = { "PLANT", "HERBIVORE", "CARNIVORE" };
        bool canStart;
        bool canPause;
        bool canResume;
        bool canRestart;
        bool canStop;
        bool canPurchaseMovementUpgrade;
        bool canPurchaseAttackUpgrade;
        bool canPurchaseBlockUpgrade;
        bool canPlayNextSimulation;
        Visibility settingsVisibility;
        Visibility runningVisibility;
        Visibility pausedVisibility;
        Visibility rewardsVisibility;
        Visibility resultsVisibility;
        Visibility boardVisibility;

        public event PropertyChangedEventHandler PropertyChanged;

        public DelegateCommand StartCommand { get; private set; }
        public DelegateCommand PauseCommand { get; private set; }
        public DelegateCommand ResumeCommand { get; private set; }
        public DelegateCommand RestartCommand { get; private set; }
        public DelegateCommand StopCommand { get; private set; }
        public DelegateCommand ResetCommand { get; private set; }
        public DelegateCommand PurchaseMovementUpgradeCommand { get; private set; }
        public DelegateCommand PurchaseAttackUpgradeCommand { get; private set; }
        public DelegateCommand PurchaseBlockUpgradeCommand { get; private set; }
        public DelegateCommand ContinueWithoutUpgradeCommand { get; private set; }
        public DelegateCommand PlayNextSimulationCommand { get; private set; }
        public DelegateCommand ApplySettingsCommand { get; private set; }
        public DelegateCommand SaveSettingsCommand { get; private set; }
        public DelegateCommand EditSpeciesRulesCommand { get; private set; }
        public DelegateCommand ApplySpeciesRulesCommand { get; private set; }

        public string StateTitle => stateTitle;
        public string RunStatusText => runStatusText;
        public string RunDetailsText => runDetailsText;
        public string CurrencyText => currencyText;
        public string SettingsMessage => settingsMessage;
        public string GridWidthText
        {
            get => gridWidthText;
            set => Set(ref gridWidthText, value, nameof(GridWidthText));
        }
        public string GridHeightText
        {
            get => gridHeightText;
            set => Set(ref gridHeightText, value, nameof(GridHeightText));
        }
        public string BaseSeedText
        {
            get => baseSeedText;
            set => Set(ref baseSeedText, value, nameof(BaseSeedText));
        }
        public string MaximumPopulationText
        {
            get => maximumPopulationText;
            set => Set(ref maximumPopulationText, value, nameof(MaximumPopulationText));
        }
        public string MinimumPopulationText
        {
            get => minimumPopulationText;
            set => Set(ref minimumPopulationText, value, nameof(MinimumPopulationText));
        }
        public string RunDurationText
        {
            get => runDurationText;
            set => Set(ref runDurationText, value, nameof(RunDurationText));
        }
        public string StepIntervalText
        {
            get => stepIntervalText;
            set => Set(ref stepIntervalText, value, nameof(StepIntervalText));
        }
        public string PlantProbabilityText
        {
            get => plantProbabilityText;
            set => Set(ref plantProbabilityText, value, nameof(PlantProbabilityText));
        }
        public string HerbivoreProbabilityText
        {
            get => herbivoreProbabilityText;
            set => Set(ref herbivoreProbabilityText, value, nameof(HerbivoreProbabilityText));
        }
        public string CarnivoreProbabilityText
        {
            get => carnivoreProbabilityText;
            set => Set(ref carnivoreProbabilityText, value, nameof(CarnivoreProbabilityText));
        }
        public bool RandomizeSeedOnStart
        {
            get => randomizeSeedOnStart;
            set => Set(ref randomizeSeedOnStart, value, nameof(RandomizeSeedOnStart));
        }
        public bool CanEditSettings => canEditSettings;
        public string[] SpeciesTabs => speciesTabs;
        public int SelectedRuleSpeciesIndex
        {
            get => selectedRuleSpeciesIndex;
            set
            {
                if (selectedRuleSpeciesIndex == value)
                {
                    return;
                }

                selectedRuleSpeciesIndex = Mathf.Clamp(value, 0, speciesTabs.Length - 1);
                LoadRuleValues();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRuleSpeciesIndex)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRuleSpeciesTitle)));
            }
        }
        public string SelectedRuleSpeciesTitle => speciesTabs[selectedRuleSpeciesIndex] + " RULES";
        public bool MovementEnabled { get => ruleValues.MovementEnabled; set => SetRule(ref ruleValues.MovementEnabled, value, nameof(MovementEnabled)); }
        public string MovementSpeedText { get => ruleValues.MovementSpeed; set => SetRule(ref ruleValues.MovementSpeed, value, nameof(MovementSpeedText)); }
        public int MovementPattern { get => ruleValues.MovementPattern; set => SetRule(ref ruleValues.MovementPattern, value, nameof(MovementPattern)); }
        public bool AttackEnabled { get => ruleValues.AttackEnabled; set => SetRule(ref ruleValues.AttackEnabled, value, nameof(AttackEnabled)); }
        public string AttackAmountText { get => ruleValues.AttackAmount; set => SetRule(ref ruleValues.AttackAmount, value, nameof(AttackAmountText)); }
        public int AttackPattern { get => ruleValues.AttackPattern; set => SetRule(ref ruleValues.AttackPattern, value, nameof(AttackPattern)); }
        public string BlockAmountText { get => ruleValues.BlockAmount; set => SetRule(ref ruleValues.BlockAmount, value, nameof(BlockAmountText)); }
        public int BlockPattern { get => ruleValues.BlockPattern; set => SetRule(ref ruleValues.BlockPattern, value, nameof(BlockPattern)); }
        public int DietTarget { get => ruleValues.DietTarget; set => SetRule(ref ruleValues.DietTarget, value, nameof(DietTarget)); }
        public int DietPattern { get => ruleValues.DietPattern; set => SetRule(ref ruleValues.DietPattern, value, nameof(DietPattern)); }
        public int ReproductionPattern { get => ruleValues.ReproductionPattern; set => SetRule(ref ruleValues.ReproductionPattern, value, nameof(ReproductionPattern)); }
        public bool ReproductionEnabled { get => ruleValues.ReproductionEnabled; set => SetRule(ref ruleValues.ReproductionEnabled, value, nameof(ReproductionEnabled)); }
        public string ReproductionChanceText { get => ruleValues.ReproductionChance; set => SetRule(ref ruleValues.ReproductionChance, value, nameof(ReproductionChanceText)); }
        public string ReproductionNeighborCountText { get => ruleValues.ReproductionNeighborCount; set => SetRule(ref ruleValues.ReproductionNeighborCount, value, nameof(ReproductionNeighborCountText)); }
        public string ReproductionFoodRequiredText { get => ruleValues.ReproductionFoodRequired; set => SetRule(ref ruleValues.ReproductionFoodRequired, value, nameof(ReproductionFoodRequiredText)); }
        public string MaxReproductionGroupSizeText { get => ruleValues.MaxReproductionGroupSize; set => SetRule(ref ruleValues.MaxReproductionGroupSize, value, nameof(MaxReproductionGroupSizeText)); }
        public string StartingEnergyText { get => ruleValues.StartingEnergy; set => SetRule(ref ruleValues.StartingEnergy, value, nameof(StartingEnergyText)); }
        public string EnergyValueText { get => ruleValues.EnergyValue; set => SetRule(ref ruleValues.EnergyValue, value, nameof(EnergyValueText)); }
        public string MetabolismText { get => ruleValues.Metabolism; set => SetRule(ref ruleValues.Metabolism, value, nameof(MetabolismText)); }
        public string VisionRangeText { get => ruleValues.VisionRange; set => SetRule(ref ruleValues.VisionRange, value, nameof(VisionRangeText)); }
        public string IntelligenceText { get => ruleValues.Intelligence; set => SetRule(ref ruleValues.Intelligence, value, nameof(IntelligenceText)); }
        public bool WiltEnabled { get => ruleValues.WiltEnabled; set => SetRule(ref ruleValues.WiltEnabled, value, nameof(WiltEnabled)); }
        public string WiltChanceText { get => ruleValues.WiltChance; set => SetRule(ref ruleValues.WiltChance, value, nameof(WiltChanceText)); }
        public string CrowdingEnergyPenaltyText { get => ruleValues.CrowdingEnergyPenalty; set => SetRule(ref ruleValues.CrowdingEnergyPenalty, value, nameof(CrowdingEnergyPenaltyText)); }
        public string StartingFoodReserveText { get => ruleValues.StartingFoodReserve; set => SetRule(ref ruleValues.StartingFoodReserve, value, nameof(StartingFoodReserveText)); }
        public bool SeedDropEnabled { get => ruleValues.SeedDropEnabled; set => SetRule(ref ruleValues.SeedDropEnabled, value, nameof(SeedDropEnabled)); }
        public string SeedDropChanceText { get => ruleValues.SeedDropChance; set => SetRule(ref ruleValues.SeedDropChance, value, nameof(SeedDropChanceText)); }
        public bool CanStart => canStart;
        public bool CanPause => canPause;
        public bool CanResume => canResume;
        public bool CanRestart => canRestart;
        public bool CanStop => canStop;
        public bool CanPurchaseMovementUpgrade => canPurchaseMovementUpgrade;
        public bool CanPurchaseAttackUpgrade => canPurchaseAttackUpgrade;
        public bool CanPurchaseBlockUpgrade => canPurchaseBlockUpgrade;
        public bool CanPlayNextSimulation => canPlayNextSimulation;
        public Visibility SettingsVisibility => settingsVisibility;
        public Visibility RunningVisibility => runningVisibility;
        public Visibility PausedVisibility => pausedVisibility;
        public Visibility RewardsVisibility => rewardsVisibility;
        public Visibility ResultsVisibility => resultsVisibility;
        public Visibility BoardVisibility => boardVisibility;

        public void Initialize(SpeciesSimulationPreview simulationPreview)
        {
            preview = simulationPreview ?? throw new ArgumentNullException(nameof(simulationPreview));
            Refresh(true);
        }

        void Awake()
        {
            StartCommand = new DelegateCommand(() => preview?.StartSimulation());
            PauseCommand = new DelegateCommand(() => preview?.PauseSimulation());
            ResumeCommand = new DelegateCommand(() => preview?.ResumeSimulation());
            RestartCommand = new DelegateCommand(() => preview?.RestartSimulation());
            StopCommand = new DelegateCommand(() => preview?.StopSimulation());
            ResetCommand = new DelegateCommand(() => preview?.ResetToStart());
            PurchaseMovementUpgradeCommand = new DelegateCommand(() => preview?.PurchaseReward(0));
            PurchaseAttackUpgradeCommand = new DelegateCommand(() => preview?.PurchaseReward(1));
            PurchaseBlockUpgradeCommand = new DelegateCommand(() => preview?.PurchaseReward(2));
            ContinueWithoutUpgradeCommand = new DelegateCommand(() => preview?.ContinueWithoutUpgrade());
            PlayNextSimulationCommand = new DelegateCommand(() => preview?.PlayNextSimulation());
            ApplySettingsCommand = new DelegateCommand(ApplySettings);
            SaveSettingsCommand = new DelegateCommand(SaveSettings);
            EditSpeciesRulesCommand = new DelegateCommand(EditSpeciesRules);
            ApplySpeciesRulesCommand = new DelegateCommand(ApplySpeciesRules);
        }

        void Start()
        {
            if (preview == null)
            {
                preview = FindAnyObjectByType<SpeciesSimulationPreview>();
            }

            LoadRuleValues();

            var view = GetComponent<NoesisView>();
            if (view != null && view.Content is FrameworkElement content)
            {
                content.DataContext = this;
                board = content.FindName("SimulationBoard") as SpeciesSimulationBoard;
            }

            Refresh(true);
        }

        void Update()
        {
            Refresh(false);
        }

        void Refresh(bool force)
        {
            if (preview == null)
            {
                return;
            }

            var state = preview.State;
            var run = preview.Run;
            var runStatus = run == null ? SimulationRunStatus.Ready : run.Status;
            var tick = run == null ? -1 : run.Tick;
            if (!force && state == lastState && runStatus == lastRunStatus && tick == lastTick)
            {
                return;
            }

            if (state == SpeciesPreviewState.Ready && lastState != SpeciesPreviewState.Ready)
            {
                showXamlSettings = true;
            }

            lastState = state;
            lastRunStatus = runStatus;
            lastTick = tick;

            if (board == null)
            {
                var view = GetComponent<NoesisView>();
                if (view != null && view.Content is FrameworkElement content)
                {
                    board = content.FindName("SimulationBoard") as SpeciesSimulationBoard;
                }
            }

            board?.SetGrid(run?.Cells);

            Set(ref stateTitle, GetStateTitle(state), nameof(StateTitle));
            Set(ref runStatusText, GetRunStatusText(run), nameof(RunStatusText));
            Set(ref runDetailsText, GetRunDetailsText(run), nameof(RunDetailsText));
            Set(ref currencyText, preview.Progression == null
                ? "Currency: 0"
                : $"Currency: {preview.Progression.Currency}", nameof(CurrencyText));
            if (force || state == SpeciesPreviewState.Ready)
            {
                SyncSettingsFields();
            }
            Set(ref settingsMessage, preview.SettingsMessage, nameof(SettingsMessage));

            Set(ref canStart, runStatus == SimulationRunStatus.Ready, nameof(CanStart));
            Set(ref canEditSettings, preview.SettingsEditable, nameof(CanEditSettings));
            Set(ref canPause, runStatus == SimulationRunStatus.Running, nameof(CanPause));
            Set(ref canResume, runStatus == SimulationRunStatus.Paused, nameof(CanResume));
            Set(ref canRestart, runStatus == SimulationRunStatus.Running || runStatus == SimulationRunStatus.Paused, nameof(CanRestart));
            Set(ref canStop, runStatus == SimulationRunStatus.Running || runStatus == SimulationRunStatus.Paused, nameof(CanStop));
            Set(ref canPurchaseMovementUpgrade, preview.CanPurchaseReward(0), nameof(CanPurchaseMovementUpgrade));
            Set(ref canPurchaseAttackUpgrade, preview.CanPurchaseReward(1), nameof(CanPurchaseAttackUpgrade));
            Set(ref canPurchaseBlockUpgrade, preview.CanPurchaseReward(2), nameof(CanPurchaseBlockUpgrade));
            Set(ref canPlayNextSimulation, state == SpeciesPreviewState.Results, nameof(CanPlayNextSimulation));

            Set(ref settingsVisibility,
                state == SpeciesPreviewState.Ready && showXamlSettings
                    ? Visibility.Visible
                    : Visibility.Collapsed,
                nameof(SettingsVisibility));
            Set(ref runningVisibility, state == SpeciesPreviewState.Running ? Visibility.Visible : Visibility.Collapsed, nameof(RunningVisibility));
            Set(ref pausedVisibility, state == SpeciesPreviewState.Paused ? Visibility.Visible : Visibility.Collapsed, nameof(PausedVisibility));
            Set(ref rewardsVisibility, state == SpeciesPreviewState.Rewards ? Visibility.Visible : Visibility.Collapsed, nameof(RewardsVisibility));
            Set(ref resultsVisibility, state == SpeciesPreviewState.Results ? Visibility.Visible : Visibility.Collapsed, nameof(ResultsVisibility));
            Set(ref boardVisibility,
                run == null || state == SpeciesPreviewState.Ready
                    ? Visibility.Collapsed
                    : Visibility.Visible,
                nameof(BoardVisibility));
        }

        void ApplySettings()
        {
            if (preview == null)
            {
                return;
            }

            preview.TryApplyGlobalSettings(
                GridWidthText,
                GridHeightText,
                BaseSeedText,
                MaximumPopulationText,
                MinimumPopulationText,
                RunDurationText,
                StepIntervalText,
                PlantProbabilityText,
                HerbivoreProbabilityText,
                CarnivoreProbabilityText,
                RandomizeSeedOnStart,
                out _);
            Refresh(true);
        }

        void ApplySpeciesRules()
        {
            if (preview == null)
            {
                return;
            }

            preview.TryApplySpeciesRuleEditValues(
                GetSpeciesId(selectedRuleSpeciesIndex),
                ruleValues,
                out _);
            LoadRuleValues();
            Refresh(true);
        }

        void SaveSettings()
        {
            if (preview == null)
            {
                return;
            }

            preview.SaveCurrentSettingsAsDefault();
            Refresh(true);
        }

        void EditSpeciesRules()
        {
            if (preview == null || !preview.SettingsEditable)
            {
                return;
            }

            preview.OpenLegacySpeciesEditor();
            showXamlSettings = false;
            Refresh(true);
        }

        void LoadRuleValues()
        {
            if (preview == null)
            {
                return;
            }

            ruleValues = preview.GetSpeciesRuleEditValues(GetSpeciesId(selectedRuleSpeciesIndex));
            RaiseRulePropertiesChanged();
        }

        SpeciesId GetSpeciesId(int index)
        {
            switch (index)
            {
                case 0:
                    return SpeciesIds.Plant;
                case 1:
                    return SpeciesIds.Herbivore;
                default:
                    return SpeciesIds.Carnivore;
            }
        }

        void RaiseRulePropertiesChanged()
        {
            var names = new[]
            {
                nameof(MovementEnabled), nameof(MovementSpeedText), nameof(MovementPattern),
                nameof(AttackEnabled), nameof(AttackAmountText), nameof(AttackPattern),
                nameof(BlockAmountText), nameof(BlockPattern), nameof(DietTarget), nameof(DietPattern),
                nameof(ReproductionPattern), nameof(ReproductionEnabled), nameof(ReproductionChanceText),
                nameof(ReproductionNeighborCountText), nameof(ReproductionFoodRequiredText),
                nameof(MaxReproductionGroupSizeText), nameof(StartingEnergyText), nameof(EnergyValueText),
                nameof(MetabolismText), nameof(VisionRangeText), nameof(IntelligenceText),
                nameof(WiltEnabled), nameof(WiltChanceText), nameof(CrowdingEnergyPenaltyText),
                nameof(StartingFoodReserveText), nameof(SeedDropEnabled), nameof(SeedDropChanceText),
            };

            foreach (var name in names)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        void SetRule<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        void SyncSettingsFields()
        {
            GridWidthText = preview.GridWidth.ToString(CultureInfo.InvariantCulture);
            GridHeightText = preview.GridHeight.ToString(CultureInfo.InvariantCulture);
            BaseSeedText = preview.BaseSeed.ToString(CultureInfo.InvariantCulture);
            MaximumPopulationText = preview.MaximumPopulation.ToString(CultureInfo.InvariantCulture);
            MinimumPopulationText = preview.MinimumPopulation.ToString(CultureInfo.InvariantCulture);
            RunDurationText = preview.RunDurationSeconds.ToString("0.###", CultureInfo.InvariantCulture);
            StepIntervalText = preview.StepInterval.ToString("0.###", CultureInfo.InvariantCulture);
            PlantProbabilityText = preview.PlantProbability.ToString("0.###", CultureInfo.InvariantCulture);
            HerbivoreProbabilityText = preview.HerbivoreProbability.ToString("0.###", CultureInfo.InvariantCulture);
            CarnivoreProbabilityText = preview.CarnivoreProbability.ToString("0.###", CultureInfo.InvariantCulture);
            RandomizeSeedOnStart = preview.RandomizeSeedOnStart;
        }

        static string GetStateTitle(SpeciesPreviewState state)
        {
            switch (state)
            {
                case SpeciesPreviewState.Ready:
                    return "SPECIES SETTINGS";
                case SpeciesPreviewState.Running:
                    return "SIMULATION IN PROGRESS";
                case SpeciesPreviewState.Paused:
                    return "SIMULATION PAUSED";
                case SpeciesPreviewState.Rewards:
                    return "CHOOSE YOUR REWARD";
                case SpeciesPreviewState.Results:
                    return "SPECIES UPDATE";
                default:
                    return "CELLULAR SIMULATION";
            }
        }

        static string GetRunStatusText(SimulationRunState run)
        {
            return run == null ? "Ready to configure" : run.Status.ToString();
        }

        static string GetRunDetailsText(SimulationRunState run)
        {
            if (run == null)
            {
                return "Configure your species and start a new run.";
            }

            return string.Format(
                CultureInfo.InvariantCulture,
                "Seed {0}  |  Tick {1}  |  {2:0.0}/{3:0.0}s",
                run.Seed,
                run.Tick,
                run.ElapsedSeconds,
                run.DurationSeconds);
        }

        void Set<T>(ref T field, T value, string propertyName)
        {
            if (Equals(field, value))
            {
                return;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
