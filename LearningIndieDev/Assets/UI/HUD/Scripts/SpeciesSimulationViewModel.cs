using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Noesis;
using UnityEngine;
using UnityEngine.U2D;

namespace SaltyGame
{
    public sealed class SpeciesSimulationViewModel : MonoBehaviour, INotifyPropertyChanged
    {
        static readonly string[] AnimalSpriteNames =
        {
            "Animals_01_Wolf",
            "Animals_01_Fox",
            "Animals_01_Eagle",
            "Animals_01_Shark",
            "Animals_01_Deer",
            "Animals_01_Rabbit",
            "Animals_01_Cow",
            "Animals_01_Elephant",
        };

        static readonly string[] TerrainSpriteNames = CreateTerrainSpriteNames();

        static string[] CreateTerrainSpriteNames()
        {
            var names = new string[TerrainTileResolver.TerrainVariantCount * 2];
            for (var index = 0; index < TerrainTileResolver.TerrainVariantCount; index++)
            {
                var variantName = TerrainTileResolver.GetVariantName(index);
                names[index] = $"Grass_{variantName}";
                names[index + TerrainTileResolver.TerrainVariantCount] = $"Desert_{variantName}";
            }

            return names;
        }

        SpeciesSimulationPreview preview;
        SpeciesSimulationBoard board;
        SpriteAtlas animalSpriteAtlas;
        SpriteAtlas terrainSpriteAtlas;
        Sprite foxSpeciesSprite;
        Sprite rabbitSpeciesSprite;
        TextureSource animalTextureSource;
        TextureSource terrainTextureSource;
        TextureSource foxTextureSource;
        TextureSource rabbitTextureSource;
        CroppedBitmap[] animalSprites;
        CroppedBitmap[] terrainTiles;
        bool warnedMissingAtlases;
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
        bool developerMode;
        int selectedRuleSpeciesIndex;
        SpeciesRuleEditValues ruleValues = new SpeciesRuleEditValues();
        string[] speciesTabs = Array.Empty<string>();
        SpeciesId[] speciesTabIds = Array.Empty<SpeciesId>();
        bool canStart;
        bool canPause;
        bool canResume;
        bool canRestart;
        bool canStop;
        bool canPurchaseMovementUpgrade;
        bool canPurchaseAttackUpgrade;
        bool canPurchaseBlockUpgrade;
        bool canPlayNextSimulation;
        string[] scenarioOptions = Array.Empty<string>();
        int selectedScenarioIndex;
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
        public bool DeveloperMode
        {
            get => developerMode;
            set
            {
                if (developerMode == value)
                {
                    return;
                }

                developerMode = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DeveloperMode)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DeveloperSettingsVisibility)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PlayerSettingsVisibility)));
            }
        }
        public Visibility DeveloperSettingsVisibility => developerMode ? Visibility.Visible : Visibility.Collapsed;
        public Visibility PlayerSettingsVisibility => developerMode ? Visibility.Collapsed : Visibility.Visible;
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

                selectedRuleSpeciesIndex = Mathf.Clamp(value, 0, Mathf.Max(0, speciesTabs.Length - 1));
                LoadRuleValues();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRuleSpeciesIndex)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRuleSpeciesTitle)));
            }
        }
        public string SelectedRuleSpeciesTitle => speciesTabs.Length == 0
            ? "SPECIES RULES"
            : speciesTabs[selectedRuleSpeciesIndex] + " RULES";
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
        public string MaximumEnergyText { get => ruleValues.MaximumEnergy; set => SetRule(ref ruleValues.MaximumEnergy, value, nameof(MaximumEnergyText)); }
        public string LitterMinimumText { get => ruleValues.LitterMinimum; set => SetRule(ref ruleValues.LitterMinimum, value, nameof(LitterMinimumText)); }
        public string LitterMaximumText { get => ruleValues.LitterMaximum; set => SetRule(ref ruleValues.LitterMaximum, value, nameof(LitterMaximumText)); }
        public string ForageBelowEnergyText { get => ruleValues.ForageBelowEnergy; set => SetRule(ref ruleValues.ForageBelowEnergy, value, nameof(ForageBelowEnergyText)); }
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
        public string[] ScenarioOptions => scenarioOptions;
        public int SelectedScenarioIndex
        {
            get => selectedScenarioIndex;
            set
            {
                if (selectedScenarioIndex == value)
                {
                    return;
                }

                selectedScenarioIndex = Mathf.Max(0, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedScenarioIndex)));
                if (preview == null || !preview.SettingsEditable)
                {
                    return;
                }

                var actualScenarioIndex = selectedScenarioIndex - 1;
                if (actualScenarioIndex != preview.SelectedScenarioIndex)
                {
                    preview.TrySelectScenario(actualScenarioIndex, out _);
                    Refresh(true);
                }
            }
        }
        public Visibility SettingsVisibility => settingsVisibility;
        public Visibility RunningVisibility => runningVisibility;
        public Visibility PausedVisibility => pausedVisibility;
        public Visibility RewardsVisibility => rewardsVisibility;
        public Visibility ResultsVisibility => resultsVisibility;
        public Visibility BoardVisibility => boardVisibility;

        public void Initialize(
            SpeciesSimulationPreview simulationPreview,
            SpriteAtlas animalAtlas = null,
            SpriteAtlas terrainAtlas = null,
            Sprite foxSprite = null,
            Sprite rabbitSprite = null)
        {
            preview = simulationPreview ?? throw new ArgumentNullException(nameof(simulationPreview));
            SetSpriteVisuals(animalAtlas, terrainAtlas, foxSprite, rabbitSprite);
            SyncSpeciesTabs();
            SyncScenarioOptions();
            Refresh(true);
        }

        public void BindToView(NoesisView view)
        {
            if (view == null || view.Content == null)
            {
                return;
            }

            view.Content.DataContext = this;
            board = view.Content.FindName("SimulationBoard") as SpeciesSimulationBoard;
            board?.SetSpriteVisuals(animalSprites, terrainTiles);
            Refresh(true);
        }

        public void SetSpriteVisuals(
            SpriteAtlas animals,
            SpriteAtlas terrain,
            Sprite fox,
            Sprite rabbit)
        {
            if (animalSpriteAtlas == animals
                && terrainSpriteAtlas == terrain
                && foxSpeciesSprite == fox
                && rabbitSpeciesSprite == rabbit
                && (animalSprites != null || terrainTiles != null || warnedMissingAtlases))
            {
                return;
            }

            animalSpriteAtlas = animals;
            terrainSpriteAtlas = terrain;
            foxSpeciesSprite = fox;
            rabbitSpeciesSprite = rabbit;
            animalTextureSource = null;
            terrainTextureSource = null;
            foxTextureSource = null;
            rabbitTextureSource = null;
            animalSprites = null;
            terrainTiles = null;
            warnedMissingAtlases = false;
            PrepareSpriteVisuals();
            board?.SetSpriteVisuals(animalSprites, terrainTiles);
        }

        void PrepareSpriteVisuals()
        {
            if (terrainSpriteAtlas == null
                || (animalSpriteAtlas == null && foxSpeciesSprite == null && rabbitSpeciesSprite == null))
            {
                if (!warnedMissingAtlases)
                {
                    Debug.LogWarning(
                        "SpeciesSimulationViewModel requires Terrain_01 and either animal atlas or fox/rabbit sprites.");
                    warnedMissingAtlases = true;
                }

                return;
            }

            animalSprites = CreateSpeciesSprites();
            terrainTiles = CreateNamedAtlasSprites(terrainSpriteAtlas, TerrainSpriteNames, out terrainTextureSource);
            if (animalSprites == null || terrainTiles == null)
            {
                animalSprites = null;
                terrainTiles = null;
                warnedMissingAtlases = true;
            }
        }

        CroppedBitmap[] CreateSpeciesSprites()
        {
            var sprites = animalSpriteAtlas == null
                ? new CroppedBitmap[8]
                : CreateNamedAtlasSprites(animalSpriteAtlas, AnimalSpriteNames, out animalTextureSource);
            if (sprites == null)
            {
                return null;
            }

            if (foxSpeciesSprite != null)
            {
                sprites[1] = CreateSprite(foxSpeciesSprite, out foxTextureSource);
            }

            if (rabbitSpeciesSprite != null)
            {
                sprites[5] = CreateSprite(rabbitSpeciesSprite, out rabbitTextureSource);
            }

            return sprites;
        }

        static CroppedBitmap[] CreateNamedAtlasSprites(
            SpriteAtlas atlas,
            string[] spriteNames,
            out TextureSource textureSource)
        {
            textureSource = null;
            var packedSprites = new Sprite[atlas.spriteCount];
            atlas.GetSprites(packedSprites);
            if (packedSprites.Length == 0)
            {
                Debug.LogWarning($"SpriteAtlas '{atlas.name}' contains no sprites.");
                return null;
            }

            textureSource = new TextureSource(packedSprites[0].texture);
            var sprites = new CroppedBitmap[spriteNames.Length];
            for (var index = 0; index < spriteNames.Length; index++)
            {
                Sprite matchingSprite = null;
                for (var spriteIndex = 0; spriteIndex < packedSprites.Length; spriteIndex++)
                {
                    if (packedSprites[spriteIndex].name.StartsWith(
                        spriteNames[index],
                        StringComparison.OrdinalIgnoreCase))
                    {
                        matchingSprite = packedSprites[spriteIndex];
                        break;
                    }
                }

                if (matchingSprite == null)
                {
                    Debug.LogWarning(
                        $"SpriteAtlas '{atlas.name}' is missing sprite '{spriteNames[index]}'.");
                    return null;
                }

                sprites[index] = new CroppedBitmap(textureSource, GetSourceRect(matchingSprite));
            }

            return sprites;
        }

        static CroppedBitmap CreateSprite(Sprite sprite, out TextureSource textureSource)
        {
            textureSource = new TextureSource(sprite.texture);
            return new CroppedBitmap(textureSource, GetSourceRect(sprite));
        }

        static CroppedBitmap[] CreateSprites(
            SpriteAtlas atlas,
            int count,
            int columns,
            out TextureSource textureSource)
        {
            textureSource = null;
            var packedSprites = new Sprite[atlas.spriteCount];
            atlas.GetSprites(packedSprites);
            if (packedSprites.Length == 0)
            {
                Debug.LogWarning($"SpriteAtlas '{atlas.name}' contains no sprites.");
                return null;
            }

            Array.Sort(packedSprites, CompareSpritesBySourcePosition);
            textureSource = new TextureSource(packedSprites[0].texture);
            if (packedSprites.Length == 1)
            {
                return CreateSprites(textureSource, GetSourceRect(packedSprites[0]), count, columns);
            }

            if (packedSprites.Length < count)
            {
                Debug.LogWarning(
                    $"SpriteAtlas '{atlas.name}' contains {packedSprites.Length} sprites; expected at least {count}.");
                return null;
            }

            var sprites = new CroppedBitmap[count];
            for (var index = 0; index < count; index++)
            {
                sprites[index] = new CroppedBitmap(textureSource, GetSourceRect(packedSprites[index]));
            }

            return sprites;
        }

        static CroppedBitmap[] CreateSprites(
            TextureSource source,
            Int32Rect sourceRect,
            int count,
            int columns)
        {
            var sprites = new CroppedBitmap[count];
            var tileWidth = sourceRect.Width / columns;
            var tileHeight = sourceRect.Height / ((count + columns - 1) / columns);
            for (var index = 0; index < count; index++)
            {
                sprites[index] = new CroppedBitmap(
                    source,
                    new Int32Rect(
                        sourceRect.X + (index % columns) * tileWidth,
                        sourceRect.Y + (index / columns) * tileHeight,
                        tileWidth,
                        tileHeight));
            }

            return sprites;
        }

        static Int32Rect GetSourceRect(Sprite sprite)
        {
            var rect = sprite.packed && sprite.packingMode == SpritePackingMode.Rectangle
                ? sprite.textureRect
                : sprite.rect;
            var scale = sprite.spriteAtlasTextureScale;
            return new Int32Rect(
                (int)(rect.x * scale),
                sprite.texture.height - (int)((rect.y + rect.height) * scale),
                (int)(rect.width * scale),
                (int)(rect.height * scale));
        }

        static int CompareSpritesBySourcePosition(Sprite left, Sprite right)
        {
            var y = right.rect.y.CompareTo(left.rect.y);
            return y != 0 ? y : left.rect.x.CompareTo(right.rect.x);
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
            BindToView(view);

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

            SyncSpeciesTabs();

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

            board?.SetSpeciesRules(preview.ActiveSpeciesRules);
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
                state == SpeciesPreviewState.Ready
                    ? Visibility.Visible
                    : Visibility.Collapsed,
                nameof(SettingsVisibility));
            Set(ref runningVisibility, state == SpeciesPreviewState.Running ? Visibility.Visible : Visibility.Collapsed, nameof(RunningVisibility));
            Set(ref pausedVisibility, state == SpeciesPreviewState.Paused ? Visibility.Visible : Visibility.Collapsed, nameof(PausedVisibility));
            Set(ref rewardsVisibility, state == SpeciesPreviewState.Rewards ? Visibility.Visible : Visibility.Collapsed, nameof(RewardsVisibility));
            Set(ref resultsVisibility, state == SpeciesPreviewState.Results ? Visibility.Visible : Visibility.Collapsed, nameof(ResultsVisibility));
            Set(ref boardVisibility,
                state == SpeciesPreviewState.Running
                    ? Visibility.Visible
                    : Visibility.Collapsed,
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
            if (preview == null || speciesTabIds.Length == 0)
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

        void LoadRuleValues()
        {
            if (preview == null || speciesTabIds.Length == 0)
            {
                return;
            }

            ruleValues = preview.GetSpeciesRuleEditValues(GetSpeciesId(selectedRuleSpeciesIndex));
            RaiseRulePropertiesChanged();
        }

        SpeciesId GetSpeciesId(int index)
        {
            if (speciesTabIds.Length == 0)
            {
                return SpeciesIds.Herbivore;
            }

            return speciesTabIds[Mathf.Clamp(index, 0, speciesTabIds.Length - 1)];
        }

        void SyncSpeciesTabs()
        {
            if (preview == null || preview.ActiveSpeciesRules == null)
            {
                return;
            }

            var ids = new List<SpeciesId>(preview.ActiveSpeciesRules.Keys);
            ids.Sort((left, right) => string.CompareOrdinal(left.Value, right.Value));
            if (ids.Count == speciesTabIds.Length)
            {
                var unchanged = true;
                for (var index = 0; index < ids.Count; index++)
                {
                    if (ids[index] != speciesTabIds[index])
                    {
                        unchanged = false;
                        break;
                    }
                }

                if (unchanged)
                {
                    return;
                }
            }

            var previousId = speciesTabIds.Length > 0
                && selectedRuleSpeciesIndex < speciesTabIds.Length
                ? speciesTabIds[selectedRuleSpeciesIndex]
                : default;
            speciesTabIds = ids.ToArray();
            speciesTabs = new string[speciesTabIds.Length];
            selectedRuleSpeciesIndex = 0;
            for (var index = 0; index < speciesTabIds.Length; index++)
            {
                speciesTabs[index] = speciesTabIds[index].Value.ToUpperInvariant();
                if (speciesTabIds[index] == previousId)
                {
                    selectedRuleSpeciesIndex = index;
                }
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SpeciesTabs)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRuleSpeciesIndex)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRuleSpeciesTitle)));
            LoadRuleValues();
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
                nameof(MaxReproductionGroupSizeText), nameof(StartingEnergyText), nameof(MaximumEnergyText),
                nameof(LitterMinimumText), nameof(LitterMaximumText), nameof(ForageBelowEnergyText),
                nameof(EnergyValueText),
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
            SyncScenarioOptions();
            Set(ref selectedScenarioIndex, preview.SelectedScenarioIndex + 1, nameof(SelectedScenarioIndex));
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

        void SyncScenarioOptions()
        {
            if (preview == null)
            {
                return;
            }

            var authoredScenarios = preview.ScenarioOptions;
            var names = new string[(authoredScenarios?.Count ?? 0) + 1];
            names[0] = "Legacy Defaults";
            for (var index = 0; index < names.Length - 1; index++)
            {
                var scenario = authoredScenarios[index];
                names[index + 1] = scenario == null ? "(Empty Scenario Slot)" : scenario.name;
            }

            var changed = scenarioOptions.Length != names.Length;
            if (!changed)
            {
                for (var index = 0; index < names.Length; index++)
                {
                    if (!string.Equals(scenarioOptions[index], names[index], StringComparison.Ordinal))
                    {
                        changed = true;
                        break;
                    }
                }
            }

            if (changed)
            {
                scenarioOptions = names;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScenarioOptions)));
            }
        }

        static string GetStateTitle(SpeciesPreviewState state)
        {
            switch (state)
            {
                case SpeciesPreviewState.Ready:
                    return "EXPEDITION SETUP";
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
