using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
    public enum SpeciesPreviewState
    {
        Ready,
        Running,
        Rewards,
        Results,
    }

    public sealed class SpeciesSimulationPreview : MonoBehaviour
    {
        [Header("Grid")]
        [SerializeField, Min(1)] int width = 32;
        [SerializeField, Min(1)] int height = 20;
        [SerializeField] int seed = 12345;
        [SerializeField] SpeciesArchetype playerSpecies = SpeciesArchetype.Herbivore;
        [SerializeField, Range(0f, 1f)] float plantProbability = 0.25f;
        [SerializeField, Range(0f, 1f)] float herbivoreProbability = 0.1f;
        [SerializeField, Range(0f, 1f)] float carnivoreProbability = 0.04f;

        [Header("Run")]
        [SerializeField, Min(1f)] float runDurationSeconds = 20f;
        [SerializeField, Min(0.01f)] float stepInterval = 0.1f;

        [Header("Colors")]
        [SerializeField] Color emptyColor = new Color(0.03f, 0.04f, 0.07f);
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
        float tickTimer;
        int runNumber;
        bool rewardGranted;

        public SimulationRunState Run => runner?.Run;
        public SpeciesProgression Progression => progression;
        public SpeciesPreviewState State => previewState;

        void Awake()
        {
            rules = SpeciesRuleDefaults.Create();
            progression = new SpeciesProgression(new SpeciesDefinition(
                playerSpecies,
                rules[playerSpecies]));
            PrepareNextRun();
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
                $"Species Run {runNumber}  |  {run.Status}  |  {run.ElapsedSeconds:0.0}/{run.DurationSeconds:0.0}s  |  Currency: {progression.Currency}",
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
                runner.Start();
                previewState = SpeciesPreviewState.Running;
            }
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
            runner = new SpeciesSimulationRunner(run, rules, stepInterval);
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
            var panelWidth = Mathf.Min(960f, Screen.width - 48f);
            var panelHeight = previewState == SpeciesPreviewState.Rewards ? 560f : 380f;
            var panelLeft = (Screen.width - panelWidth) * 0.5f;
            var panelTop = (Screen.height - panelHeight) * 0.5f;
            var panelRect = new Rect(panelLeft, panelTop, panelWidth, panelHeight);
            var titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 44,
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
                    GUI.Label(new Rect(panelLeft + 40f, panelTop + 92f, panelWidth - 80f, 64f),
                        "Your species is ready. Start the simulation to begin the run.", bodyStyle);
                    if (GUI.Button(new Rect(panelLeft + 180f, panelTop + 190f, panelWidth - 360f, 88f),
                        "START SIMULATION", buttonStyle))
                    {
                        StartSimulation();
                    }

                    break;
                case SpeciesPreviewState.Running:
                    GUI.Label(new Rect(panelLeft + 40f, panelTop + 102f, panelWidth - 80f, 64f),
                        "The ecosystem is evolving...", bodyStyle);
                    break;
                case SpeciesPreviewState.Rewards:
                    DrawRewardPanel(panelLeft, panelTop, panelWidth, bodyStyle, cardButtonStyle);
                    break;
                case SpeciesPreviewState.Results:
                    DrawResultsPanel(panelLeft, panelTop, panelWidth, bodyStyle, buttonStyle);
                    break;
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
                    return "SPECIES SIMULATION";
                case SpeciesPreviewState.Running:
                    return "SIMULATION IN PROGRESS";
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

        Grid<SpeciesCell> CreateInitialGrid(int runSeed)
        {
            var random = new System.Random(runSeed);
            return new Grid<SpeciesCell>(width, height, (_, _) =>
            {
                var roll = random.NextDouble();
                if (roll < plantProbability)
                {
                    return new SpeciesCell(SpeciesArchetype.Plant);
                }

                if (roll < plantProbability + herbivoreProbability)
                {
                    return new SpeciesCell(SpeciesArchetype.Herbivore);
                }

                if (roll < plantProbability + herbivoreProbability + carnivoreProbability)
                {
                    return new SpeciesCell(SpeciesArchetype.Carnivore);
                }

                return SpeciesCell.Empty;
            });
        }

        Color GetCellColor(SpeciesCell cell)
        {
            if (!cell.IsOccupied)
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
