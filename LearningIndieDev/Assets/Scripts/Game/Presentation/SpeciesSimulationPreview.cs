using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
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

        readonly SpeciesUpgrade speedUpgrade = new SpeciesUpgrade(
            "faster-movement",
            cost: 5,
            type: SpeciesUpgradeType.MovementSpeed,
            value: 0.5f);

        IReadOnlyDictionary<SpeciesArchetype, SpeciesRules> rules;
        SpeciesProgression progression;
        SpeciesSimulationRunner runner;
        SimulationRunResult result;
        float tickTimer;
        int runNumber;
        bool rewardGranted;

        public SimulationRunState Run => runner?.Run;
        public SpeciesProgression Progression => progression;

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

            GUI.Label(new Rect(padding, padding, Screen.width - padding * 2f, 24f),
                $"Species Run {runNumber}  |  {run.Status}  |  {run.ElapsedSeconds:0.0}/{run.DurationSeconds:0.0}s  |  Currency: {progression.Currency}");

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
            if (run.Status == SimulationRunStatus.Ready
                && GUI.Button(new Rect(padding, Screen.height - 44f, 180f, 28f), "Start Simulation"))
            {
                StartSimulation();
            }

            if (run.Status == SimulationRunStatus.Complete)
            {
                GUI.Label(new Rect(padding, Screen.height - 72f, 360f, 24f),
                    $"Run complete  |  Reward: {result.CurrencyEarned}");
                if (GUI.Button(new Rect(padding, Screen.height - 44f, 180f, 28f), "Buy Speed Upgrade (5)"))
                {
                    progression.TryPurchase(speedUpgrade);
                }

                if (GUI.Button(new Rect(padding + 188f, Screen.height - 44f, 140f, 28f), "Start Next Run"))
                {
                    PrepareNextRun();
                }
            }
        }

        public void StartSimulation()
        {
            if (runner != null && runner.Run.Status == SimulationRunStatus.Ready)
            {
                runner.Start();
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
            runNumber++;
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
