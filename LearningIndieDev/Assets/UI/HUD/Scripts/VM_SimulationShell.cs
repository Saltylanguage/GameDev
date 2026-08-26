using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using Noesis;
using UnityEngine;
using UnityEngine.U2D;

namespace SaltyGame
{
    public sealed class VM_SimulationShell : MonoBehaviour, INotifyPropertyChanged
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
            var names = new string[TerrainTileResolver.AllValidMasks.Count * 2];
            for (var index = 0; index < TerrainTileResolver.AllValidMasks.Count; index++)
            {
                var mask = TerrainTileResolver.AllValidMasks[index];
                names[index] = TerrainTileResolver.GetTerrainSpriteName(TerrainVisualFamily.Grass, mask);
                names[index + TerrainTileResolver.AllValidMasks.Count] =
                    TerrainTileResolver.GetTerrainSpriteName(TerrainVisualFamily.Desert, mask);
            }

            return names;
        }

        SpeciesSimulationPreview preview;
        SpriteAtlas animalSpriteAtlas;
        SpriteAtlas terrainSpriteAtlas;
        Sprite foxSpeciesSprite;
        Sprite rabbitSpeciesSprite;
        TextureSource animalTextureSource;
        TextureSource terrainTextureSource;
        TextureSource foxTextureSource;
        TextureSource rabbitTextureSource;
        CroppedBitmap[] animalSprites;
        CroppedBitmap[] grassTerrainTiles;
        CroppedBitmap[] desertTerrainTiles;
        bool warnedMissingAtlases;
        SpeciesPreviewState lastState;
        SimulationRunStatus lastRunStatus;
        int lastTick = -1;
        string stateTitle;
        string runStatusText;
        string runDetailsText;
        string experimentalHerbivoreStatLineSummary;
        string experimentalUpgradeCountText;
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
        string foxAttackCooldownTicksText;
        bool randomizeSeedOnStart;
        bool bevExperimentalFeaturesEnabled;
        bool canEditSettings;
        bool developerMode;
        int selectedRuleSpeciesIndex;
        SpeciesRuleEditValues ruleValues = new SpeciesRuleEditValues();
        string[] speciesTabs = Array.Empty<string>();
        SpeciesId[] speciesTabIds = Array.Empty<SpeciesId>();
        string[] playerSpeciesOptions = Array.Empty<string>();
        SpeciesId[] playerSpeciesIds = Array.Empty<SpeciesId>();
        int selectedPlayerSpeciesIndex;
        bool canStart;
        bool canPause;
        bool canResume;
        bool canRestart;
        bool canStop;
        bool canPurchaseRewardOption1;
        bool canPurchaseRewardOption2;
        bool canPurchaseRewardOption3;
        bool canPlayNextSimulation;
        string rewardOption1Text;
        string rewardOption2Text;
        string rewardOption3Text;
        string[] scenarioOptions = Array.Empty<string>();
        int selectedScenarioIndex;
        string scenarioText;
        string playerSpeciesText;
        string rosterText;
        Helper_SceneTransition sceneTransition;
        Helper_ProfileSession profileSession;
        readonly StringBuilder rosterTextBuilder = new StringBuilder();
        Visibility settingsVisibility;
        Visibility runningVisibility;
        Visibility pausedVisibility;
        Visibility rewardsVisibility;
        Visibility resultsVisibility;
        Visibility experimentalHerbivoreStatLineSummaryVisibility;
        Visibility experimentalUpgradeCountVisibility;
        Visibility rewardOption3Visibility;
        Visibility boardVisibility;

        public event PropertyChangedEventHandler PropertyChanged;

        public DelegateCommand StartCommand { get; private set; }
        public DelegateCommand PauseCommand { get; private set; }
        public DelegateCommand ResumeCommand { get; private set; }
        public DelegateCommand RestartCommand { get; private set; }
        public DelegateCommand StopCommand { get; private set; }
        public DelegateCommand ResetCommand { get; private set; }
        public DelegateCommand PurchaseRewardOption1Command { get; private set; }
        public DelegateCommand PurchaseRewardOption2Command { get; private set; }
        public DelegateCommand PurchaseRewardOption3Command { get; private set; }
        public DelegateCommand ContinueWithoutUpgradeCommand { get; private set; }
        public DelegateCommand PlayNextSimulationCommand { get; private set; }
        public DelegateCommand ApplySettingsCommand { get; private set; }
        public DelegateCommand SaveSettingsCommand { get; private set; }
        public DelegateCommand ApplySpeciesRulesCommand { get; private set; }
        public DelegateCommand ReturnToLabCommand { get; private set; }

        public string StateTitle => stateTitle;
        public string RunStatusText => runStatusText;
        public string RunDetailsText => runDetailsText;
        public string ExperimentalHerbivoreStatLineSummary => experimentalHerbivoreStatLineSummary;
        public string ExperimentalUpgradeCountText => experimentalUpgradeCountText;
        public string CurrencyText => currencyText;
        public string ScenarioText => scenarioText;
        public string PlayerSpeciesText => playerSpeciesText;
        public string RosterText => rosterText;
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
        public bool BevExperimentalFeaturesEnabled
        {
            get => bevExperimentalFeaturesEnabled;
            set => Set(ref bevExperimentalFeaturesEnabled, value, nameof(BevExperimentalFeaturesEnabled));
        }
        public string FoxAttackCooldownTicksText
        {
            get => foxAttackCooldownTicksText;
            set => Set(ref foxAttackCooldownTicksText, value, nameof(FoxAttackCooldownTicksText));
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
        public bool CanPurchaseRewardOption1 => canPurchaseRewardOption1;
        public bool CanPurchaseRewardOption2 => canPurchaseRewardOption2;
        public bool CanPurchaseRewardOption3 => canPurchaseRewardOption3;
        public string RewardOption1Text => rewardOption1Text;
        public string RewardOption2Text => rewardOption2Text;
        public string RewardOption3Text => rewardOption3Text;
        public bool CanPlayNextSimulation => canPlayNextSimulation;
        public bool CanReturnToLab => resultsVisibility == Visibility.Visible
            && sceneTransition != null
            && profileSession?.Current?.HasLoadedProfile == true;
        public string[] ScenarioOptions => scenarioOptions;
        public string[] PlayerSpeciesOptions => playerSpeciesOptions;
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
        public int SelectedPlayerSpeciesIndex
        {
            get => selectedPlayerSpeciesIndex;
            set
            {
                if (selectedPlayerSpeciesIndex == value)
                {
                    return;
                }

                selectedPlayerSpeciesIndex = Mathf.Clamp(value, 0, Mathf.Max(0, playerSpeciesIds.Length - 1));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedPlayerSpeciesIndex)));
                if (preview == null || !preview.SettingsEditable || playerSpeciesIds.Length == 0)
                {
                    return;
                }

                preview.TrySetPlayerSpecies(playerSpeciesIds[selectedPlayerSpeciesIndex].Value, out _);
                Refresh(true);
            }
        }
        public Visibility SettingsVisibility => settingsVisibility;
        public Visibility RunningVisibility => runningVisibility;
        public Visibility PausedVisibility => pausedVisibility;
        public Visibility RewardsVisibility => rewardsVisibility;
        public Visibility ResultsVisibility => resultsVisibility;
        public Visibility ExperimentalHerbivoreStatLineSummaryVisibility => experimentalHerbivoreStatLineSummaryVisibility;
        public Visibility ExperimentalUpgradeCountVisibility => experimentalUpgradeCountVisibility;
        public Visibility RewardOption3Visibility => rewardOption3Visibility;
        public Visibility BoardVisibility => boardVisibility;
        internal CroppedBitmap[] AnimalSprites => animalSprites;
        internal CroppedBitmap[] GrassTerrainTiles => grassTerrainTiles;
        internal CroppedBitmap[] DesertTerrainTiles => desertTerrainTiles;

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
            Refresh(true);
        }

        public void BindSceneTransition(Helper_SceneTransition transition, Helper_ProfileSession profile)
        {
            sceneTransition = transition;
            profileSession = profile;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanReturnToLab)));
            ReturnToLabCommand?.RaiseCanExecuteChanged();
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
                && (animalSprites != null || grassTerrainTiles != null || warnedMissingAtlases))
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
            grassTerrainTiles = null;
            desertTerrainTiles = null;
            warnedMissingAtlases = false;
            PrepareSpriteVisuals();
        }

        void PrepareSpriteVisuals()
        {
            if (terrainSpriteAtlas == null
                || (animalSpriteAtlas == null && foxSpeciesSprite == null && rabbitSpeciesSprite == null))
            {
                if (!warnedMissingAtlases)
                {
                    Debug.LogWarning(
                        "VM_SimulationShell requires Terrain_01 and either animal atlas or fox/rabbit sprites.");
                    warnedMissingAtlases = true;
                }

                return;
            }

            animalSprites = CreateSpeciesSprites();
            var allTerrainTiles = CreateNamedAtlasSprites(terrainSpriteAtlas, TerrainSpriteNames, out terrainTextureSource);
            if (allTerrainTiles != null)
            {
                grassTerrainTiles = SliceTerrainTiles(allTerrainTiles, 0);
                desertTerrainTiles = SliceTerrainTiles(allTerrainTiles, TerrainTileResolver.AllValidMasks.Count);
            }

            if (animalSprites == null || grassTerrainTiles == null || desertTerrainTiles == null)
            {
                animalSprites = null;
                grassTerrainTiles = null;
                desertTerrainTiles = null;
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

        static CroppedBitmap[] SliceTerrainTiles(CroppedBitmap[] allTiles, int offset)
        {
            var sprites = new CroppedBitmap[256];
            for (var index = 0; index < TerrainTileResolver.AllValidMasks.Count; index++)
            {
                sprites[TerrainTileResolver.AllValidMasks[index]] = allTiles[offset + index];
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

            if (!TryCreateTextureSource(FindAtlasTexture(packedSprites), out textureSource))
            {
                return null;
            }
            var sprites = new CroppedBitmap[spriteNames.Length];
            for (var index = 0; index < spriteNames.Length; index++)
            {
                Sprite matchingSprite = null;
                for (var spriteIndex = 0; spriteIndex < packedSprites.Length; spriteIndex++)
                {
                    if (packedSprites[spriteIndex] != null
                        && packedSprites[spriteIndex].name.StartsWith(
                        spriteNames[index],
                        StringComparison.OrdinalIgnoreCase))
                    {
                        matchingSprite = packedSprites[spriteIndex];
                        break;
                    }
                }

                if (matchingSprite == null || matchingSprite.texture == null)
                {
                    Debug.LogWarning(
                        $"SpriteAtlas '{atlas.name}' is missing sprite '{spriteNames[index]}'.");
                    return null;
                }

                sprites[index] = new CroppedBitmap(textureSource, GetSourceRect(matchingSprite));
            }

            return sprites;
        }

        static Texture2D FindAtlasTexture(Sprite[] packedSprites)
        {
            for (var index = 0; index < packedSprites.Length; index++)
            {
                if (packedSprites[index] != null && packedSprites[index].texture != null)
                {
                    return packedSprites[index].texture;
                }
            }

            return null;
        }

        static CroppedBitmap[] CreateTerrainAtlasSprites(
            SpriteAtlas atlas,
            out TextureSource textureSource)
        {
            textureSource = null;
            var packedSprites = new Sprite[atlas.spriteCount];
            atlas.GetSprites(packedSprites);
            if (packedSprites.Length == 0)
            {
                Debug.LogWarning($"SpriteAtlas '{atlas.name}' contains no terrain sprites.");
                return null;
            }

            var sprites = new CroppedBitmap[TerrainSpriteNames.Length];
            for (var index = 0; index < TerrainSpriteNames.Length; index++)
            {
                Sprite matchingSprite = null;
                for (var spriteIndex = 0; spriteIndex < packedSprites.Length; spriteIndex++)
                {
                    if (packedSprites[spriteIndex] != null
                        && packedSprites[spriteIndex].name.StartsWith(
                        TerrainSpriteNames[index],
                        StringComparison.OrdinalIgnoreCase))
                    {
                        matchingSprite = packedSprites[spriteIndex];
                        break;
                    }
                }

                if (matchingSprite == null)
                {
                    Debug.LogWarning(
                        $"SpriteAtlas '{atlas.name}' is missing terrain sprite '{TerrainSpriteNames[index]}'.");
                    return null;
                }

                if (textureSource == null
                    && !TryCreateTextureSource(matchingSprite.texture, out textureSource))
                {
                    return null;
                }
                sprites[index] = new CroppedBitmap(textureSource, GetSourceRect(matchingSprite));
            }

            return sprites;
        }

        static CroppedBitmap CreateSprite(Sprite sprite, out TextureSource textureSource)
        {
            if (!TryCreateTextureSource(sprite?.texture, out textureSource))
            {
                return null;
            }

            return new CroppedBitmap(textureSource, GetSourceRect(sprite));
        }

        static bool TryCreateTextureSource(Texture2D texture, out TextureSource textureSource)
        {
            textureSource = null;
            if (texture == null || texture.GetNativeTexturePtr() == IntPtr.Zero)
            {
                return false;
            }

            textureSource = new TextureSource(texture);
            return true;
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
            if (!TryCreateTextureSource(FindAtlasTexture(packedSprites), out textureSource))
            {
                return null;
            }
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
            PurchaseRewardOption1Command = new DelegateCommand(() => preview?.PurchaseReward(0));
            PurchaseRewardOption2Command = new DelegateCommand(() => preview?.PurchaseReward(1));
            PurchaseRewardOption3Command = new DelegateCommand(() => preview?.PurchaseReward(2));
            ContinueWithoutUpgradeCommand = new DelegateCommand(() => preview?.ContinueWithoutUpgrade());
            PlayNextSimulationCommand = new DelegateCommand(() => preview?.PlayNextSimulation());
            ApplySettingsCommand = new DelegateCommand(ApplySettings);
            SaveSettingsCommand = new DelegateCommand(SaveSettings);
            ApplySpeciesRulesCommand = new DelegateCommand(ApplySpeciesRules);
            ReturnToLabCommand = new DelegateCommand(ReturnToLab, () => CanReturnToLab);
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
            var isHerbivorePlayer = preview.ActiveSpeciesRules != null
                && preview.ActiveSpeciesRules.TryGetValue(preview.PlayerSpecies, out var playerRules)
                && playerRules.Role == SpeciesRole.Herbivore;
            var showExperimentalHerbivoreStatLine =
                (state == SpeciesPreviewState.Rewards || state == SpeciesPreviewState.Results)
                && preview.BevExperimentalFeaturesEnabled
                && isHerbivorePlayer;
            var showExperimentalUpgradeCount =
                (state == SpeciesPreviewState.Rewards || state == SpeciesPreviewState.Results)
                && preview.BevExperimentalFeaturesEnabled
                && isHerbivorePlayer;
            if (!force && state == lastState && runStatus == lastRunStatus && tick == lastTick)
            {
                return;
            }

            SyncSpeciesTabs();

            lastState = state;
            lastRunStatus = runStatus;
            lastTick = tick;

            Set(ref stateTitle, GetStateTitle(state), nameof(StateTitle));
            Set(ref runStatusText, GetRunStatusText(run), nameof(RunStatusText));
            Set(ref runDetailsText, GetRunDetailsText(run), nameof(RunDetailsText));
            Set(
                ref experimentalHerbivoreStatLineSummary,
                showExperimentalHerbivoreStatLine
                    ? GetExperimentalHerbivoreStatLineSummary(run, preview.PlayerSpecies)
                    : string.Empty,
                nameof(ExperimentalHerbivoreStatLineSummary));
            Set(
                ref experimentalUpgradeCountText,
                showExperimentalUpgradeCount
                    ? string.Format(
                        CultureInfo.InvariantCulture,
                        "BEV EXPERIMENTAL UPGRADES: {0} | TOUGH HIDE Lv {1} | DIGESTION Lv {2} | CROWDING Lv {3} | ESCAPE Lv {4}",
                        preview.PurchasedUpgradeCount,
                        preview.GetUpgradeLevel(SpeciesUpgradeCatalog.ToughHideId),
                        preview.GetUpgradeLevel(SpeciesUpgradeCatalog.EfficientDigestionId),
                        preview.GetUpgradeLevel(SpeciesUpgradeCatalog.CrowdingToleranceId),
                        preview.GetUpgradeLevel(SpeciesUpgradeCatalog.EscapeArtistId))
                    : string.Empty,
                nameof(ExperimentalUpgradeCountText));
            Set(ref currencyText, preview.Progression == null
                ? "Currency: 0"
                : $"Currency: {preview.Progression.Currency}", nameof(CurrencyText));
            SyncScenarioPresentation(run);
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
            Set(ref rewardOption1Text, preview.GetRewardOptionDisplayName(0), nameof(RewardOption1Text));
            Set(ref rewardOption2Text, preview.GetRewardOptionDisplayName(1), nameof(RewardOption2Text));
            Set(ref rewardOption3Text, preview.GetRewardOptionDisplayName(2), nameof(RewardOption3Text));
            Set(ref canPurchaseRewardOption1, preview.CanPurchaseReward(0), nameof(CanPurchaseRewardOption1));
            Set(ref canPurchaseRewardOption2, preview.CanPurchaseReward(1), nameof(CanPurchaseRewardOption2));
            Set(ref canPurchaseRewardOption3, preview.CanPurchaseReward(2), nameof(CanPurchaseRewardOption3));
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanReturnToLab)));
            ReturnToLabCommand?.RaiseCanExecuteChanged();
            Set(
                ref experimentalHerbivoreStatLineSummaryVisibility,
                showExperimentalHerbivoreStatLine
                    ? Visibility.Visible
                    : Visibility.Collapsed,
                nameof(ExperimentalHerbivoreStatLineSummaryVisibility));
            Set(
                ref experimentalUpgradeCountVisibility,
                showExperimentalUpgradeCount
                    ? Visibility.Visible
                    : Visibility.Collapsed,
                nameof(ExperimentalUpgradeCountVisibility));
            Set(
                ref rewardOption3Visibility,
                preview.RewardOptionCount > 2 ? Visibility.Visible : Visibility.Collapsed,
                nameof(RewardOption3Visibility));
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

            if (!preview.TryApplyGlobalSettings(
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
                out _))
            {
                Refresh(true);
                return;
            }

            preview.TryApplyExperimentalFeatures(
                BevExperimentalFeaturesEnabled,
                FoxAttackCooldownTicksText,
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

        void ReturnToLab()
        {
            if (CanReturnToLab)
            {
                sceneTransition.LoadLab(profileSession.Current);
            }
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
            SyncPlayerSpeciesOptions();
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
            BevExperimentalFeaturesEnabled = preview.BevExperimentalFeaturesEnabled;
            FoxAttackCooldownTicksText = preview.FoxAttackCooldownTicks.ToString(CultureInfo.InvariantCulture);
            RandomizeSeedOnStart = preview.RandomizeSeedOnStart;
        }

        void SyncPlayerSpeciesOptions()
        {
            var availableSpecies = preview.PlayableSpecies;
            playerSpeciesIds = new SpeciesId[availableSpecies.Count];
            playerSpeciesOptions = new string[availableSpecies.Count];
            var selectedIndex = 0;
            for (var index = 0; index < availableSpecies.Count; index++)
            {
                var species = availableSpecies[index];
                playerSpeciesIds[index] = species;
                playerSpeciesOptions[index] = FormatSpeciesName(species);
                if (species == preview.PlayerSpecies)
                {
                    selectedIndex = index;
                }
            }

            Set(ref selectedPlayerSpeciesIndex, selectedIndex, nameof(SelectedPlayerSpeciesIndex));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PlayerSpeciesOptions)));
        }

        void SyncScenarioPresentation(SimulationRunState run)
        {
            Set(ref scenarioText,
                preview.SelectedScenario == null
                    ? "Scenario: Legacy Defaults"
                    : $"Scenario: {preview.SelectedScenario.name}",
                nameof(ScenarioText));
            Set(ref playerSpeciesText,
                $"Player species: {FormatSpeciesName(preview.PlayerSpecies)}",
                nameof(PlayerSpeciesText));
            Set(ref rosterText, BuildRosterText(run), nameof(RosterText));
        }

        string BuildRosterText(SimulationRunState run)
        {
            rosterTextBuilder.Clear();
            rosterTextBuilder.Append("Roster: ");
            var roster = preview.RosterSpecies;
            var population = run != null && run.PopulationHistory.Count > 0
                ? run.PopulationHistory[run.PopulationHistory.Count - 1]
                : default;

            for (var index = 0; index < roster.Count; index++)
            {
                if (index > 0)
                {
                    rosterTextBuilder.Append("  •  ");
                }

                var species = roster[index];
                rosterTextBuilder.Append(FormatSpeciesName(species));
                if (run != null)
                {
                    rosterTextBuilder.Append(' ');
                    rosterTextBuilder.Append(population.GetCount(species));
                }

                if (species == preview.PlayerSpecies)
                {
                    rosterTextBuilder.Append(" (YOU)");
                }
            }

            return rosterTextBuilder.ToString();
        }

        static string FormatSpeciesName(SpeciesId species)
        {
            return species.IsValid
                ? species.Value.Replace('-', ' ').ToUpperInvariant()
                : "(UNASSIGNED)";
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

        static string GetExperimentalHerbivoreStatLineSummary(SimulationRunState run, SpeciesId species)
        {
            if (run == null || run.PopulationHistory.Count == 0)
            {
                return string.Empty;
            }

            var statLine = run.Metrics.CreateHerbivoreStatLine(
                species,
                run.PopulationHistory[0].GetCount(species),
                run.PopulationHistory[run.PopulationHistory.Count - 1].GetCount(species));
            var summary = new StringBuilder();
            AppendMetric(summary, "SPO", statLine.StartingPopulation);
            AppendMetric(summary, "HPS", statLine.PredatorActiveHerbivoreSteps);
            AppendMetric(summary, "EHS", statLine.EncounteredHerbivoreSteps);
            AppendMetric(summary, "ECN", statLine.Encounters);
            summary.Append('\n');
            AppendMetric(summary, "PREY", statLine.Preyed);
            AppendMetric(summary, "STRV", statLine.Starved);
            AppendMetric(summary, "MAT", statLine.Mating);
            AppendMetric(summary, "BIR", statLine.Births);
            summary.Append('\n');
            AppendMetric(summary, "CRWD", statLine.Crowding);
            AppendMetric(summary, "FPO", statLine.FinalPopulation);
            AppendMetric(summary, "pAVI", statLine.InversePreyedAverage, statLine.InversePreyedAverageStatus);
            AppendMetric(summary, "eAVI", statLine.InverseEncounterAverage, statLine.InverseEncounterAverageStatus);
            summary.Append('\n');
            AppendMetric(summary, "predAVG", statLine.PredationAverage, statLine.PredationAverageStatus);
            AppendMetric(summary, "sAVI", statLine.InverseStarvedAverage, statLine.InverseStarvedAverageStatus);
            AppendMetric(summary, "cAVI", statLine.InverseCrowdingAverage, statLine.InverseCrowdingAverageStatus);
            AppendMetric(summary, "bAVG", statLine.BirthAverage, statLine.BirthAverageStatus);
            summary.Append('\n');
            AppendMetric(summary, "RFS", statLine.ReplicationFitnessScore, statLine.ReplicationFitnessScoreStatus);
            AppendMetric(summary, "APS", statLine.ActualPreyScore, statLine.ActualPreyScoreStatus);
            summary.Append('\n')
                .Append("Expected FPO: ")
                .Append(statLine.ExpectedFinalPopulation)
                .Append("  |  Reconciled: ")
                .Append(statLine.PopulationReconciled);
            return summary.ToString();
        }

        static void AppendMetric(StringBuilder summary, string name, int value)
        {
            AppendSeparator(summary);
            summary.Append(name).Append(": ").Append(value);
        }

        static void AppendMetric(
            StringBuilder summary,
            string name,
            float value,
            SpeciesHerbivoreMetricStatus status)
        {
            AppendSeparator(summary);
            summary.Append(name).Append(": ");
            switch (status)
            {
                case SpeciesHerbivoreMetricStatus.NotApplicable:
                    summary.Append("N/A");
                    break;
                case SpeciesHerbivoreMetricStatus.Invalid:
                    summary.Append("INVALID");
                    break;
                default:
                    summary.Append(value.ToString("0.##", CultureInfo.InvariantCulture));
                    break;
            }
        }

        static void AppendSeparator(StringBuilder summary)
        {
            if (summary.Length > 0 && summary[summary.Length - 1] != '\n')
            {
                summary.Append("  |  ");
            }
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
