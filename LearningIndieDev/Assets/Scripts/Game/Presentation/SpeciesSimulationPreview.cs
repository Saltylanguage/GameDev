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
                DietTarget = GetDietTargetOption(rules.DietTarget);
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
                EnergyValue = rules.EnergyValue;
                EnergyValueText = rules.EnergyValue.ToString(CultureInfo.InvariantCulture);
                Metabolism = rules.Metabolism;
                MetabolismText = rules.Metabolism.ToString(CultureInfo.InvariantCulture);
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
            public int EnergyValue;
            public string EnergyValueText;
            public int Metabolism;
            public string MetabolismText;
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
        [SerializeField, Min(1)] int height = 20;
        [SerializeField] int seed = 12345;
        [SerializeField] bool randomizeSeedOnStart = true;
        [SerializeField] SpeciesArchetype playerSpecies = SpeciesArchetype.Herbivore;
        [SerializeField, Range(0f, 1f)] float plantProbability = 0.4f;
        [SerializeField, Range(0f, 1f)] float herbivoreProbability = 0.16f;
        [SerializeField, Range(0f, 1f)] float carnivoreProbability = 0.04f;
        [SerializeField, Min(0)] int maxPopulation;
        [SerializeField, Min(0)] int minPopulation;

        [Header("Run")]
        [SerializeField, Min(1f)] float runDurationSeconds = 20f;
        [SerializeField, Min(0.01f)] float stepInterval = 0.1f;

        [Header("Colors")]
        [SerializeField] Color emptyColor = new Color(0.35f, 0.2f, 0.1f);
        [SerializeField] Color plantColor = new Color(0.2f, 0.75f, 0.25f);
        [SerializeField] Color herbivoreColor = new Color(0.2f, 0.7f, 1f);
        [SerializeField] Color carnivoreColor = new Color(0.95f, 0.25f, 0.2f);

        readonly SpeciesUpgrade[] rewardOptions =
        {
            new SpeciesUpgrade("faster-movement", 5, SpeciesUpgradeType.MovementSpeed, 0.5f),
            new SpeciesUpgrade("stronger-attack", 5, SpeciesUpgradeType.AttackAmount, 1f),
            new SpeciesUpgrade("stronger-block", 5, SpeciesUpgradeType.BlockAmount, 1f),
        };

        IReadOnlyDictionary<SpeciesArchetype, SpeciesRules> rules;
        SpeciesProgression progression;
        SpeciesSimulationRunner runner;
        SimulationRunResult result;
        SpeciesUpgrade selectedUpgrade;
        SpeciesPreviewState previewState;
        string rewardMessage;
        Dictionary<SpeciesArchetype, SpeciesRuleDraft> ruleDrafts;
        int selectedSettingsSpecies;
        Vector2 settingsScrollPosition;
        float tickTimer;
        int runNumber;
        bool rewardGranted;
        bool sessionStarted;
        string settingsMessage;
        string widthText;
        string heightText;
        string seedText;
        string maxPopulationText;
        string minPopulationText;
        string runDurationText;
        string stepIntervalText;
        string plantProbabilityText;
        string herbivoreProbabilityText;
        string carnivoreProbabilityText;

        const string DefaultSettingsKey = "SaltyGame.SpeciesSimulationPreview.DefaultSettings.v2";

        public SimulationRunState Run => runner?.Run;
        public SpeciesProgression Progression => progression;
        public SpeciesPreviewState State => previewState;

        void Awake()
        {
            ruleDrafts = CreateRuleDrafts(SpeciesRuleDefaults.Create());
            LoadSavedSettings();
            ResetToStart();
        }

        void Update()
        {
            if (runner == null || runner.Run.Status != SimulationRunStatus.Running)
            {
                return;
            }

            tickTimer += Time.deltaTime;
            while (tickTimer >= stepInterval && runner.Run.Status == SimulationRunStatus.Running)
            {
                tickTimer -= stepInterval;
                runner.AdvanceOneTick();
            }

            if (runner.Run.Status == SimulationRunStatus.Complete && !rewardGranted)
            {
                result = SimulationRunResults.Create(runner.Run);
                progression.AddCurrency(result.CurrencyEarned);
                rewardGranted = true;
                previewState = SpeciesPreviewState.Rewards;
            }
        }

        void DrawSettingsPanel(float panelLeft, float panelTop, float panelWidth, float panelHeight, GUIStyle buttonStyle)
        {
            GUI.Label(new Rect(panelLeft + 24f, panelTop + 82f, panelWidth - 48f, 48f),
                "Configure each species. Changes apply when you start the simulation.",
                new GUIStyle(GUI.skin.label)
                {
                    fontSize = 30,
                    alignment = TextAnchor.MiddleCenter,
                });

            var speciesNames = new[] { "GLOBAL", "PLANT", "HERBIVORE", "CARNIVORE" };
            var speciesTabStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 28,
                fixedHeight = 56f,
            };
            selectedSettingsSpecies = GUI.SelectionGrid(
                new Rect(panelLeft + 24f, panelTop + 138f, panelWidth - 48f, 56f),
                selectedSettingsSpecies,
                speciesNames,
                4,
                speciesTabStyle);

            var scrollRect = new Rect(
                panelLeft + 24f,
                panelTop + 208f,
                panelWidth - 48f,
                panelHeight - 278f);
            var contentWidth = panelWidth - 72f;
            settingsScrollPosition = GUI.BeginScrollView(
                scrollRect,
                settingsScrollPosition,
                new Rect(0f, 0f, contentWidth, 760f));

            var labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 30,
                alignment = TextAnchor.MiddleLeft,
            };
            var fieldStyle = new GUIStyle(GUI.skin.textField)
            {
                fontSize = 30,
                alignment = TextAnchor.MiddleLeft,
            };
            var optionStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 24,
                fixedHeight = 44f,
            };
            var columnWidth = contentWidth * 0.5f;
            if (selectedSettingsSpecies == 0)
            {
                DrawGlobalSettingsPanel(contentWidth, labelStyle, fieldStyle, optionStyle);
            }
            else
            {
                var draft = ruleDrafts[(SpeciesArchetype)(selectedSettingsSpecies - 1)];
                DrawSettingsLeftColumn(draft, 12f, columnWidth, labelStyle, fieldStyle, optionStyle);
                DrawSettingsRightColumn(draft, columnWidth + 12f, columnWidth, labelStyle, fieldStyle, optionStyle);
            }
            GUI.EndScrollView();

            var startButtonTop = panelTop + panelHeight - 82f;
            if (selectedSettingsSpecies == 0)
            {
                GUI.Label(new Rect(panelLeft + 24f, panelTop + panelHeight - 190f, panelWidth - 48f, 34f),
                    settingsMessage ?? string.Empty,
                    new GUIStyle(GUI.skin.label)
                    {
                        fontSize = 24,
                        alignment = TextAnchor.MiddleCenter,
                    });
                var saveButtonStyle = new GUIStyle(buttonStyle)
                {
                    fontSize = 28,
                };
                if (GUI.Button(
                    new Rect(panelLeft + 180f, panelTop + panelHeight - 146f, panelWidth - 360f, 56f),
                    "SAVE CURRENT SETTINGS AS DEFAULT",
                    saveButtonStyle))
                {
                    SaveCurrentSettingsAsDefault();
                }

                startButtonTop = panelTop + panelHeight - 82f;
            }

            if (GUI.Button(
                new Rect(panelLeft + 180f, startButtonTop, panelWidth - 360f, 64f),
                "START SIMULATION",
                buttonStyle))
            {
                StartSimulation();
            }
        }

        void DrawGlobalSettingsPanel(
            float contentWidth,
            GUIStyle labelStyle,
            GUIStyle fieldStyle,
            GUIStyle optionStyle)
        {
            var left = 12f;
            var fieldWidth = contentWidth - 24f;
            var y = 8f;
            y = DrawIntField(left, y, fieldWidth, "Grid width", ref widthText, ref width, labelStyle, fieldStyle, minimum: 1);
            y = DrawIntField(left, y, fieldWidth, "Grid height", ref heightText, ref height, labelStyle, fieldStyle, minimum: 1);
            y = DrawIntField(left, y, fieldWidth, "Base seed", ref seedText, ref seed, labelStyle, fieldStyle);
            y = DrawIntField(left, y, fieldWidth, "Maximum population (0 = unlimited)", ref maxPopulationText, ref maxPopulation, labelStyle, fieldStyle);
            y = DrawIntField(left, y, fieldWidth, "Minimum starting population", ref minPopulationText, ref minPopulation, labelStyle, fieldStyle);
            y = DrawFloatField(left, y, fieldWidth, "Run duration (seconds)", ref runDurationText, ref runDurationSeconds, labelStyle, fieldStyle, 1f);
            y = DrawFloatField(left, y, fieldWidth, "Step interval (seconds)", ref stepIntervalText, ref stepInterval, labelStyle, fieldStyle, 0.01f);

            GUI.Label(new Rect(left, y, fieldWidth - 280f, 40f), "Seed mode", labelStyle);
            var randomMode = GUI.SelectionGrid(
                new Rect(left + fieldWidth - 270f, y, 260f, 44f),
                randomizeSeedOnStart ? 1 : 0,
                new[] { "Deterministic", "Random" },
                2,
                optionStyle);
            randomizeSeedOnStart = randomMode == 1;
            y += 48f;

            y = DrawFloatField(left, y, fieldWidth, "Initial plant chance", ref plantProbabilityText, ref plantProbability, labelStyle, fieldStyle, 0f, 1f);
            y = DrawFloatField(left, y, fieldWidth, "Initial herbivore chance", ref herbivoreProbabilityText, ref herbivoreProbability, labelStyle, fieldStyle, 0f, 1f);
            DrawFloatField(left, y, fieldWidth, "Initial carnivore chance", ref carnivoreProbabilityText, ref carnivoreProbability, labelStyle, fieldStyle, 0f, 1f);
        }

        void DrawSettingsLeftColumn(
            SpeciesRuleDraft draft,
            float left,
            float width,
            GUIStyle labelStyle,
            GUIStyle fieldStyle,
            GUIStyle optionStyle)
        {
            var y = 8f;
            draft.MovementEnabled = GUI.Toggle(new Rect(left, y, width, 40f), draft.MovementEnabled, "Movement enabled", labelStyle);
            y += 48f;
            y = DrawFloatField(left, y, width, "Movement speed", ref draft.MovementSpeedText, ref draft.MovementSpeed, labelStyle, fieldStyle);
            y = DrawPatternField(left, y, width, "Movement pattern", ref draft.MovementPattern, labelStyle, optionStyle);
            draft.AttackEnabled = GUI.Toggle(new Rect(left, y, width, 40f), draft.AttackEnabled, "Attack enabled", labelStyle);
            y += 48f;
            y = DrawIntField(left, y, width, "Attack amount", ref draft.AttackAmountText, ref draft.AttackAmount, labelStyle, fieldStyle);
            y = DrawPatternField(left, y, width, "Attack pattern", ref draft.AttackPattern, labelStyle, optionStyle);
            y = DrawIntField(left, y, width, "Block amount", ref draft.BlockAmountText, ref draft.BlockAmount, labelStyle, fieldStyle);
            y = DrawPatternField(left, y, width, "Block pattern", ref draft.BlockPattern, labelStyle, optionStyle);
            y = DrawDietTargetField(left, y, width, draft, labelStyle, optionStyle);
            y = DrawPatternField(left, y, width, "Diet pattern", ref draft.DietPattern, labelStyle, optionStyle);
            DrawPatternField(left, y, width, "Reproduction pattern", ref draft.ReproductionPattern, labelStyle, optionStyle);
        }

        void DrawSettingsRightColumn(
            SpeciesRuleDraft draft,
            float left,
            float width,
            GUIStyle labelStyle,
            GUIStyle fieldStyle,
            GUIStyle optionStyle)
        {
            var y = 8f;
            draft.ReproductionEnabled = GUI.Toggle(new Rect(left, y, width, 40f), draft.ReproductionEnabled, "Reproduction enabled", labelStyle);
            y += 48f;
            y = DrawFloatField(left, y, width, "Reproduction chance", ref draft.ReproductionChanceText, ref draft.ReproductionChance, labelStyle, fieldStyle, 0f, 1f);
            y = DrawIntField(left, y, width, "Nearby mate requirement", ref draft.ReproductionNeighborCountText, ref draft.ReproductionNeighborCount, labelStyle, fieldStyle);
            y = DrawIntField(left, y, width, "Energy required to mate", ref draft.ReproductionFoodRequiredText, ref draft.ReproductionFoodRequired, labelStyle, fieldStyle);
            y = DrawIntField(left, y, width, "Maximum group size", ref draft.MaxReproductionGroupSizeText, ref draft.MaxReproductionGroupSize, labelStyle, fieldStyle);
            y = DrawIntField(left, y, width, "Starting energy", ref draft.StartingEnergyText, ref draft.StartingEnergy, labelStyle, fieldStyle);
            y = DrawIntField(left, y, width, "Energy value", ref draft.EnergyValueText, ref draft.EnergyValue, labelStyle, fieldStyle);
            draft.WiltEnabled = GUI.Toggle(new Rect(left, y, width, 40f), draft.WiltEnabled, "Wilt enabled", labelStyle);
            y += 48f;
            y = DrawFloatField(left, y, width, "Wilt chance", ref draft.WiltChanceText, ref draft.WiltChance, labelStyle, fieldStyle, 0f, 1f);
            y = DrawIntField(left, y, width, "Metabolism (- adds)", ref draft.MetabolismText, ref draft.Metabolism, labelStyle, fieldStyle, minimum: -1000);
            y = DrawIntField(left, y, width, "Crowding cost", ref draft.CrowdingEnergyPenaltyText, ref draft.CrowdingEnergyPenalty, labelStyle, fieldStyle);
            y = DrawFloatField(left, y, width, "Starting food reserve", ref draft.StartingFoodReserveText, ref draft.StartingFoodReserve, labelStyle, fieldStyle);
            draft.SeedDropEnabled = GUI.Toggle(new Rect(left, y, width, 40f), draft.SeedDropEnabled, "Seed drops enabled", labelStyle);
            y += 48f;
            DrawFloatField(left, y, width, "Seed drop chance", ref draft.SeedDropChanceText, ref draft.SeedDropChance, labelStyle, fieldStyle, 0f, 1f);
        }

        static float DrawFloatField(
            float left,
            float top,
            float width,
            string label,
            ref string text,
            ref float value,
            GUIStyle labelStyle,
            GUIStyle fieldStyle,
            float minimum = 0f,
            float maximum = float.MaxValue)
        {
            GUI.Label(new Rect(left, top, width - 190f, 40f), label, labelStyle);
            text = GUI.TextField(new Rect(left + width - 180f, top, 170f, 40f), text, fieldStyle);
            if (float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
            {
                value = Mathf.Clamp(parsed, minimum, maximum);
            }

            return top + 48f;
        }

        static float DrawIntField(
            float left,
            float top,
            float width,
            string label,
            ref string text,
            ref int value,
            GUIStyle labelStyle,
            GUIStyle fieldStyle,
            int minimum = 0)
        {
            GUI.Label(new Rect(left, top, width - 190f, 40f), label, labelStyle);
            text = GUI.TextField(new Rect(left + width - 180f, top, 170f, 40f), text, fieldStyle);
            if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
            {
                value = Mathf.Max(minimum, parsed);
            }

            return top + 48f;
        }

        static float DrawPatternField(
            float left,
            float top,
            float width,
            string label,
            ref PatternPreset value,
            GUIStyle labelStyle,
            GUIStyle optionStyle)
        {
            GUI.Label(new Rect(left, top, width - 280f, 40f), label, labelStyle);
            value = (PatternPreset)GUI.SelectionGrid(
                new Rect(left + width - 270f, top, 260f, 40f),
                (int)value,
                new[] { "Cardinal", "Moore" },
                2,
                optionStyle);
            return top + 48f;
        }

        static float DrawDietTargetField(
            float left,
            float top,
            float width,
            SpeciesRuleDraft draft,
            GUIStyle labelStyle,
            GUIStyle optionStyle)
        {
            GUI.Label(new Rect(left, top, width - 280f, 40f), "Diet target", labelStyle);
            draft.DietTarget = (DietTargetOption)GUI.SelectionGrid(
                new Rect(left + width - 270f, top, 260f, 88f),
                (int)draft.DietTarget,
                new[] { "None", "Plant", "Herbivore", "Carnivore" },
                2,
                optionStyle);
            return top + 96f;
        }

        void OnGUI()
        {
            var run = Run;
            if (run == null)
            {
                return;
            }

            const float padding = 16f;
            const float headerHeight = 52f;
            var cellSize = Mathf.Min(
                (Screen.width - padding * 2f) / run.Cells.Width,
                (Screen.height - padding * 2f - headerHeight) / run.Cells.Height);
            var gridWidth = run.Cells.Width * cellSize;
            var gridHeight = run.Cells.Height * cellSize;
            var gridLeft = (Screen.width - gridWidth) * 0.5f;
            var gridTop = headerHeight + (Screen.height - headerHeight - gridHeight) * 0.5f;

            var headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 24,
                alignment = TextAnchor.MiddleCenter,
            };
            GUI.Label(new Rect(padding, padding, Screen.width - padding * 2f, 32f),
                $"Species Run {runNumber}  |  Seed: {run.Seed}  |  {run.Status}  |  {run.ElapsedSeconds:0.0}/{run.DurationSeconds:0.0}s  |  Currency: {progression.Currency}",
                headerStyle);

            var previousColor = GUI.color;
            for (var y = 0; y < run.Cells.Height; y++)
            {
                for (var x = 0; x < run.Cells.Width; x++)
                {
                    GUI.color = GetCellColor(run.Cells.GetCell(x, y));
                    GUI.DrawTexture(
                        new Rect(gridLeft + x * cellSize, gridTop + y * cellSize, cellSize - 1f, cellSize - 1f),
                        Texture2D.whiteTexture);
                }
            }

            GUI.color = previousColor;
            DrawControlPanel();
        }

        public void StartSimulation()
        {
            if (runner != null && runner.Run.Status == SimulationRunStatus.Ready)
            {
                if (!sessionStarted)
                {
                    rules = CreateRulesFromDrafts();
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

        public void ResetToStart()
        {
            if (randomizeSeedOnStart)
            {
                seed = Guid.NewGuid().GetHashCode();
            }

            rules = CreateRulesFromDrafts();
            progression = new SpeciesProgression(new SpeciesDefinition(
                playerSpecies,
                rules[playerSpecies]));
            runNumber = 0;
            sessionStarted = false;
            selectedSettingsSpecies = 0;
            settingsScrollPosition = Vector2.zero;
            settingsMessage = string.Empty;
            SyncGlobalSettingTextFields();
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
                plant = ruleDrafts[SpeciesArchetype.Plant],
                herbivore = ruleDrafts[SpeciesArchetype.Herbivore],
                carnivore = ruleDrafts[SpeciesArchetype.Carnivore],
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

            var saved = JsonUtility.FromJson<SavedSettings>(PlayerPrefs.GetString(DefaultSettingsKey));
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
            ruleDrafts[SpeciesArchetype.Plant] = saved.plant ?? ruleDrafts[SpeciesArchetype.Plant];
            ruleDrafts[SpeciesArchetype.Herbivore] = saved.herbivore ?? ruleDrafts[SpeciesArchetype.Herbivore];
            ruleDrafts[SpeciesArchetype.Carnivore] = saved.carnivore ?? ruleDrafts[SpeciesArchetype.Carnivore];
        }

        void SyncGlobalSettingTextFields()
        {
            widthText = width.ToString(CultureInfo.InvariantCulture);
            heightText = height.ToString(CultureInfo.InvariantCulture);
            seedText = seed.ToString(CultureInfo.InvariantCulture);
            maxPopulationText = maxPopulation.ToString(CultureInfo.InvariantCulture);
            minPopulationText = minPopulation.ToString(CultureInfo.InvariantCulture);
            runDurationText = FormatFloat(runDurationSeconds);
            stepIntervalText = FormatFloat(stepInterval);
            plantProbabilityText = FormatFloat(plantProbability);
            herbivoreProbabilityText = FormatFloat(herbivoreProbability);
            carnivoreProbabilityText = FormatFloat(carnivoreProbability);
        }

        void PrepareNextRun()
        {
            var currentRules = new Dictionary<SpeciesArchetype, SpeciesRules>(rules)
            {
                [playerSpecies] = progression?.CurrentRules ?? rules[playerSpecies],
            };
            rules = currentRules;

            var run = new SimulationRunState(
                CreateInitialGrid(seed + runNumber),
                playerSpecies,
                seed + runNumber,
                runDurationSeconds);
            runner = new SpeciesSimulationRunner(run, rules, stepInterval, maxPopulation);
            tickTimer = 0f;
            result = default;
            rewardGranted = false;
            selectedUpgrade = null;
            rewardMessage = string.Empty;
            previewState = SpeciesPreviewState.Ready;
            runNumber++;
        }

        void DrawControlPanel()
        {
            var simulationControlsVisible = previewState == SpeciesPreviewState.Running
                || previewState == SpeciesPreviewState.Paused;
            var panelWidth = simulationControlsVisible
                ? Mathf.Min(560f, Screen.width - 32f)
                : Mathf.Min(1200f, Screen.width - 32f);
            var settingsVisible = previewState == SpeciesPreviewState.Ready && !sessionStarted;
            var panelHeight = settingsVisible
                ? Mathf.Min(1100f, Screen.height - 24f)
                : previewState == SpeciesPreviewState.Rewards
                    ? 640f
                    : simulationControlsVisible
                        ? 300f
                        : 380f;
            var panelLeft = simulationControlsVisible
                ? Screen.width - panelWidth - 24f
                : (Screen.width - panelWidth) * 0.5f;
            var panelTop = simulationControlsVisible
                ? Screen.height - panelHeight - 24f
                : (Screen.height - panelHeight) * 0.5f;
            var panelRect = new Rect(panelLeft, panelTop, panelWidth, panelHeight);
            var titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = simulationControlsVisible ? 28 : 44,
                alignment = TextAnchor.MiddleCenter,
            };
            var defaultBodyFontSize = GUI.skin.label.fontSize > 0 ? GUI.skin.label.fontSize : 12;
            var bodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = defaultBodyFontSize * 2,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
            };
            var buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 40,
                fixedHeight = 88f,
            };
            var simulationBodyStyle = new GUIStyle(bodyStyle)
            {
                fontSize = 22,
            };
            var simulationButtonStyle = new GUIStyle(buttonStyle)
            {
                fontSize = 22,
                fixedHeight = 48f,
            };
            var cardButtonStyle = new GUIStyle(buttonStyle)
            {
                fontSize = 28,
                fixedHeight = 52f,
            };

            GUI.Box(panelRect, GUIContent.none);
            GUI.Label(new Rect(panelLeft + 20f, panelTop + 16f, panelWidth - 40f, 64f),
                GetPanelTitle(), titleStyle);

            switch (previewState)
            {
                case SpeciesPreviewState.Ready:
                    if (settingsVisible)
                    {
                        DrawSettingsPanel(panelLeft, panelTop, panelWidth, panelHeight, buttonStyle);
                    }
                    else
                    {
                        GUI.Label(new Rect(panelLeft + 40f, panelTop + 92f, panelWidth - 80f, 64f),
                            "Your species is ready for the next simulation.", bodyStyle);
                        if (GUI.Button(new Rect(panelLeft + 180f, panelTop + 190f, panelWidth - 360f, 88f),
                            "START NEXT SIMULATION", buttonStyle))
                        {
                            StartSimulation();
                        }
                    }

                    break;
                case SpeciesPreviewState.Running:
                    DrawSimulationControls(panelLeft, panelTop, panelWidth, simulationBodyStyle, simulationButtonStyle, paused: false);
                    break;
                case SpeciesPreviewState.Paused:
                    DrawSimulationControls(panelLeft, panelTop, panelWidth, simulationBodyStyle, simulationButtonStyle, paused: true);
                    break;
                case SpeciesPreviewState.Rewards:
                    DrawRewardPanel(panelLeft, panelTop, panelWidth, bodyStyle, cardButtonStyle);
                    break;
                case SpeciesPreviewState.Results:
                    DrawResultsPanel(panelLeft, panelTop, panelWidth, bodyStyle, buttonStyle);
                    break;
            }
        }

        void DrawSimulationControls(
            float panelLeft,
            float panelTop,
            float panelWidth,
            GUIStyle bodyStyle,
            GUIStyle buttonStyle,
            bool paused)
        {
            GUI.Label(new Rect(panelLeft + 16f, panelTop + 58f, panelWidth - 32f, 40f),
                paused ? "The simulation is paused." : "The ecosystem is evolving...", bodyStyle);

            if (GUI.Button(
                new Rect(panelLeft + 24f, panelTop + 106f, panelWidth - 48f, 48f),
                paused ? "RESUME SIMULATION" : "PAUSE SIMULATION",
                buttonStyle))
            {
                if (paused)
                {
                    ResumeSimulation();
                }
                else
                {
                    PauseSimulation();
                }
            }

            if (GUI.Button(
                new Rect(panelLeft + 24f, panelTop + 162f, panelWidth - 48f, 48f),
                "RESTART SIMULATION",
                buttonStyle))
            {
                RestartSimulation();
            }

            if (GUI.Button(
                new Rect(panelLeft + 24f, panelTop + 218f, panelWidth - 48f, 48f),
                "STOP AND EDIT SETTINGS",
                buttonStyle))
            {
                StopSimulation();
            }
        }

        void DrawRewardPanel(float panelLeft, float panelTop, float panelWidth, GUIStyle bodyStyle, GUIStyle buttonStyle)
        {
            GUI.Label(new Rect(panelLeft + 30f, panelTop + 82f, panelWidth - 60f, 42f),
                $"Run reward: +{result.CurrencyEarned} currency  |  Choose one upgrade", bodyStyle);

            var cardWidth = (panelWidth - 56f) / rewardOptions.Length;
            for (var index = 0; index < rewardOptions.Length; index++)
            {
                var upgrade = rewardOptions[index];
                var cardLeft = panelLeft + 14f + index * (cardWidth + 14f);
                var cardTop = panelTop + 148f;
                GUI.Box(new Rect(cardLeft, cardTop, cardWidth, 230f), GUIContent.none);
                GUI.Label(new Rect(cardLeft + 12f, cardTop + 14f, cardWidth - 24f, 56f),
                    GetUpgradeTitle(upgrade), bodyStyle);
                GUI.Label(new Rect(cardLeft + 12f, cardTop + 76f, cardWidth - 24f, 76f),
                    GetUpgradeDescription(upgrade), bodyStyle);

                GUI.enabled = progression.Currency >= upgrade.Cost;
                if (GUI.Button(new Rect(cardLeft + 16f, cardTop + 166f, cardWidth - 32f, 56f),
                    $"PURCHASE ({upgrade.Cost})", buttonStyle))
                {
                    if (progression.TryPurchase(upgrade))
                    {
                        selectedUpgrade = upgrade;
                        previewState = SpeciesPreviewState.Results;
                        rewardMessage = string.Empty;
                    }
                }

                GUI.enabled = true;
            }

            GUI.Label(new Rect(panelLeft + 20f, panelTop + 400f, panelWidth - 40f, 42f),
                string.IsNullOrEmpty(rewardMessage) ? "Select an upgrade to apply it to your species." : rewardMessage,
                bodyStyle);
            if (GUI.Button(new Rect(panelLeft + 250f, panelTop + 462f, panelWidth - 500f, 56f),
                "CONTINUE WITHOUT UPGRADE", buttonStyle))
            {
                previewState = SpeciesPreviewState.Results;
            }

            if (GUI.Button(new Rect(panelLeft + 250f, panelTop + 540f, panelWidth - 500f, 56f),
                "RESET TO SETTINGS", buttonStyle))
            {
                ResetToStart();
            }
        }

        void DrawResultsPanel(float panelLeft, float panelTop, float panelWidth, GUIStyle bodyStyle, GUIStyle buttonStyle)
        {
            var updateText = selectedUpgrade == null
                ? "No upgrade selected this run."
                : $"Applied: {GetUpgradeTitle(selectedUpgrade)}";
            GUI.Label(new Rect(panelLeft + 30f, panelTop + 92f, panelWidth - 60f, 48f), updateText, bodyStyle);
            GUI.Label(new Rect(panelLeft + 30f, panelTop + 154f, panelWidth - 60f, 100f),
                $"Movement: {progression.CurrentRules.MovementSpeed:0.0}    "
                + $"Attack: {progression.CurrentRules.AttackAmount}    "
                + $"Block: {progression.CurrentRules.BlockAmount}\n"
                + $"Currency remaining: {progression.Currency}", bodyStyle);
            if (GUI.Button(new Rect(panelLeft + 180f, panelTop + 270f, panelWidth - 360f, 88f),
                "PLAY NEXT SIMULATION", buttonStyle))
            {
                PrepareNextRun();
            }
        }

        string GetPanelTitle()
        {
            switch (previewState)
            {
                case SpeciesPreviewState.Ready:
                    return sessionStarted ? "SPECIES SIMULATION" : "SPECIES SETTINGS";
                case SpeciesPreviewState.Running:
                    return "SIMULATION IN PROGRESS";
                case SpeciesPreviewState.Paused:
                    return "SIMULATION PAUSED";
                case SpeciesPreviewState.Rewards:
                    return "CHOOSE YOUR REWARD";
                case SpeciesPreviewState.Results:
                    return "SPECIES UPDATE";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        static string GetUpgradeTitle(SpeciesUpgrade upgrade)
        {
            switch (upgrade.Type)
            {
                case SpeciesUpgradeType.MovementSpeed:
                    return "Swift Cells";
                case SpeciesUpgradeType.AttackAmount:
                    return "Sharper Cells";
                case SpeciesUpgradeType.BlockAmount:
                    return "Hardier Cells";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        static string GetUpgradeDescription(SpeciesUpgrade upgrade)
        {
            switch (upgrade.Type)
            {
                case SpeciesUpgradeType.MovementSpeed:
                    return "+0.5 movement speed";
                case SpeciesUpgradeType.AttackAmount:
                    return "+1 attack amount";
                case SpeciesUpgradeType.BlockAmount:
                    return "+1 block amount";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        Dictionary<SpeciesArchetype, SpeciesRuleDraft> CreateRuleDrafts(
            IReadOnlyDictionary<SpeciesArchetype, SpeciesRules> sourceRules)
        {
            return new Dictionary<SpeciesArchetype, SpeciesRuleDraft>
            {
                [SpeciesArchetype.Plant] = new SpeciesRuleDraft(sourceRules[SpeciesArchetype.Plant]),
                [SpeciesArchetype.Herbivore] = new SpeciesRuleDraft(sourceRules[SpeciesArchetype.Herbivore]),
                [SpeciesArchetype.Carnivore] = new SpeciesRuleDraft(sourceRules[SpeciesArchetype.Carnivore]),
            };
        }

        IReadOnlyDictionary<SpeciesArchetype, SpeciesRules> CreateRulesFromDrafts()
        {
            var result = new Dictionary<SpeciesArchetype, SpeciesRules>();
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
                    dietTarget: GetDietTarget(draft.DietTarget),
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
                    metabolism: draft.Metabolism);
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

        static SpeciesArchetype? GetDietTarget(DietTargetOption target)
        {
            switch (target)
            {
                case DietTargetOption.Plant:
                    return SpeciesArchetype.Plant;
                case DietTargetOption.Herbivore:
                    return SpeciesArchetype.Herbivore;
                case DietTargetOption.Carnivore:
                    return SpeciesArchetype.Carnivore;
                default:
                    return null;
            }
        }

        static DietTargetOption GetDietTargetOption(SpeciesArchetype? target)
        {
            if (!target.HasValue)
            {
                return DietTargetOption.None;
            }

            switch (target.Value)
            {
                case SpeciesArchetype.Plant:
                    return DietTargetOption.Plant;
                case SpeciesArchetype.Herbivore:
                    return DietTargetOption.Herbivore;
                case SpeciesArchetype.Carnivore:
                    return DietTargetOption.Carnivore;
                default:
                    throw new ArgumentOutOfRangeException(nameof(target), target, "Unknown diet target.");
            }
        }

        static string FormatFloat(float value)
        {
            return value.ToString("0.###", CultureInfo.InvariantCulture);
        }

        Grid<SpeciesCell> CreateInitialGrid(int runSeed)
        {
            var random = new System.Random(runSeed);
            var grid = new Grid<SpeciesCell>(width, height);
            var populationCount = 0;
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var roll = random.NextDouble();
                    SpeciesArchetype species;
                    if (roll < plantProbability)
                    {
                        species = SpeciesArchetype.Plant;
                    }
                    else if (roll < plantProbability + herbivoreProbability)
                    {
                        species = SpeciesArchetype.Herbivore;
                    }
                    else if (roll < plantProbability + herbivoreProbability + carnivoreProbability)
                    {
                        species = SpeciesArchetype.Carnivore;
                    }
                    else
                    {
                        continue;
                    }

                    if (maxPopulation > 0 && populationCount >= maxPopulation)
                    {
                        continue;
                    }

                    var sameSpeciesNeighbors = CountNearbySpecies(grid, x, y, species);
                    var clumpPenalty = sameSpeciesNeighbors > 2
                        ? 0.9d
                        : sameSpeciesNeighbors > 0 ? 0.65d : 0d;
                    if (random.NextDouble() < clumpPenalty)
                    {
                        continue;
                    }

                    grid.SetCell(x, y, species == SpeciesArchetype.Plant
                        ? SpeciesCell.Grass(rules[species].StartingFoodReserve)
                        : new SpeciesCell(
                            species,
                            energy: rules[species].StartingEnergy,
                            foodReserve: rules[species].StartingFoodReserve));
                    populationCount++;
                }
            }

            var minimumPopulation = Mathf.Min(minPopulation, grid.Count);
            if (maxPopulation > 0)
            {
                minimumPopulation = Mathf.Min(minimumPopulation, maxPopulation);
            }

            var attempts = 0;
            while (populationCount < minimumPopulation && attempts++ < grid.Count * 4)
            {
                var index = random.Next(grid.Count);
                var x = index % width;
                var y = index / width;
                if (grid.GetCell(x, y).IsCreature || grid.GetCell(x, y).IsPlantResource)
                {
                    continue;
                }

                var species = GetInitialSpecies(random.NextDouble());
                grid.SetCell(x, y, species == SpeciesArchetype.Plant
                    ? SpeciesCell.Grass(rules[species].StartingFoodReserve)
                    : new SpeciesCell(
                        species,
                        energy: rules[species].StartingEnergy,
                        foodReserve: rules[species].StartingFoodReserve));
                populationCount++;
            }

            return grid;
        }

        SpeciesArchetype GetInitialSpecies(double roll)
        {
            if (roll < plantProbability)
            {
                return SpeciesArchetype.Plant;
            }

            if (roll < plantProbability + herbivoreProbability)
            {
                return SpeciesArchetype.Herbivore;
            }

            if (roll < plantProbability + herbivoreProbability + carnivoreProbability)
            {
                return SpeciesArchetype.Carnivore;
            }

            return SpeciesArchetype.Plant;
        }

        static int CountNearbySpecies(Grid<SpeciesCell> grid, int x, int y, SpeciesArchetype species)
        {
            var count = 0;
            for (var offsetY = -1; offsetY <= 1; offsetY++)
            {
                for (var offsetX = -1; offsetX <= 1; offsetX++)
                {
                    if (offsetX == 0 && offsetY == 0)
                    {
                        continue;
                    }

                    if (grid.TryGetCell(x + offsetX, y + offsetY, out var neighbor)
                        && ((species == SpeciesArchetype.Plant && neighbor.IsPlantResource)
                            || (species != SpeciesArchetype.Plant
                                && neighbor.IsCreature
                                && neighbor.Species == species)))
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        Color GetCellColor(SpeciesCell cell)
        {
            if (cell.IsPlantResource && !cell.IsCreature)
            {
                return plantColor;
            }

            if (!cell.IsCreature)
            {
                return emptyColor;
            }

            switch (cell.Species)
            {
                case SpeciesArchetype.Plant:
                    return plantColor;
                case SpeciesArchetype.Herbivore:
                    return herbivoreColor;
                case SpeciesArchetype.Carnivore:
                    return carnivoreColor;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
