using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace SaltyGame
{
    /// <summary>
    /// Projects the current simulation run into the board view's immutable snapshot.
    /// </summary>
    public sealed class VM_SimulationBoard : MonoBehaviour, INotifyPropertyChanged
    {
        static readonly SpeciesId FoxSpeciesId = new SpeciesId("fox");
        const int MaxBirthBoardCues = 8;

        SpeciesSimulationPreview preview;
        SimulationRunState lastRun;
        int lastTick = -1;
        SimulationBoardSnapshot snapshot;
        int selectedX = -1;
        int selectedY = -1;
        readonly List<HuntFootprint> huntFootprints = new List<HuntFootprint>(4);
        long trackedHuntingFoxId;
        int trackedHuntingFoxX;
        int trackedHuntingFoxY;
        SpeciesBirthEvent[] recentBirths = Array.Empty<SpeciesBirthEvent>();

        public event PropertyChangedEventHandler PropertyChanged;

        public SimulationBoardSnapshot Snapshot => snapshot;
        public int FoxHuntCueX { get; private set; }
        public int FoxHuntCueY { get; private set; }
        public int FoxHuntCueTick { get; private set; } = -1;
        public int MatingCueX { get; private set; }
        public int MatingCueY { get; private set; }
        public int MatingCueMateX { get; private set; }
        public int MatingCueMateY { get; private set; }
        public int MatingCueOffspringX { get; private set; }
        public int MatingCueOffspringY { get; private set; }
        public int MatingCueTick { get; private set; } = -1;
        public IReadOnlyList<SpeciesBirthEvent> RecentBirths => recentBirths;
        public IReadOnlyList<HuntFootprint> HuntFootprints => huntFootprints;
        public bool HasSelection => snapshot != null && snapshot.TryGetCell(selectedX, selectedY, out _);
        public int SelectedX => selectedX;
        public int SelectedY => selectedY;
        public SimulationCellSnapshot SelectedCell => HasSelection
            ? snapshot.GetCell(selectedX, selectedY)
            : default;
        public string InspectionText
        {
            get
            {
                if (!HasSelection) return "Click a cell, or Tab to the board and use arrow keys to inspect it.";
                var cell = SelectedCell;
                var location = $"CELL {selectedX + 1}, {selectedY + 1}";
                if (cell.IsCreature)
                    return $"{location}  {cell.SpeciesId.Value.ToUpperInvariant()}\nHealth {cell.Health}  Energy {cell.Energy}\n{cell.BehaviorState}";
                if (cell.IsMeatResource)
                    return $"{location}  CARCASS\nFood {cell.FoodReserve.ToString("0.#", CultureInfo.InvariantCulture)}";
                if (cell.IsPlantResource || cell.IsTerrainResource)
                    return $"{location}  FOOD\nAvailable {(cell.IsTerrainResource ? cell.TerrainEnergy : cell.FoodReserve).ToString("0.#", CultureInfo.InvariantCulture)}";
                return $"{location}  {(cell.TerrainId.IsValid ? cell.TerrainId.Value.ToUpperInvariant() : "GROUND")}\nNo creature or food here.";
            }
        }

        public void Initialize(SpeciesSimulationPreview simulationPreview)
        {
            preview = simulationPreview ?? throw new ArgumentNullException(nameof(simulationPreview));
            Refresh(true);
        }

        public bool SelectCell(int x, int y)
        {
            if (snapshot == null || !snapshot.TryGetCell(x, y, out _))
            {
                return false;
            }

            if (selectedX == x && selectedY == y)
            {
                return true;
            }

            selectedX = x;
            selectedY = y;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedX)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedY)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasSelection)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCell)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InspectionText)));
            return true;
        }

        public void ClearSelection()
        {
            if (selectedX < 0 && selectedY < 0)
            {
                return;
            }

            selectedX = -1;
            selectedY = -1;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedX)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedY)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasSelection)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCell)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InspectionText)));
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

            var run = preview.Run;
            var tick = run?.Tick ?? -1;
            if (!force && ReferenceEquals(lastRun, run) && lastTick == tick)
            {
                return;
            }

            if (!ReferenceEquals(lastRun, run) || tick < lastTick)
            {
                ClearSelection();
                FoxHuntCueTick = -1;
                MatingCueTick = -1;
                recentBirths = Array.Empty<SpeciesBirthEvent>();
                huntFootprints.Clear();
                trackedHuntingFoxId = 0;
            }
            else if (run != null && tick > lastTick)
            {
                var births = run.Metrics.BirthEvents;
                for (var index = births.Count - 1; index >= 0; index--)
                {
                    var birth = births[index];
                    if (birth.Tick <= lastTick)
                    {
                        break;
                    }

                    var first = index;
                    while (first > 0 && births[first - 1].Tick == birth.Tick)
                    {
                        first--;
                    }

                    recentBirths = SelectBirthCues(births, first, index, preview.PlayerSpecies);
                    var featured = recentBirths[0];
                    MatingCueX = featured.ParentX;
                    MatingCueY = featured.ParentY;
                    MatingCueMateX = featured.MateX;
                    MatingCueMateY = featured.MateY;
                    MatingCueOffspringX = featured.ChildX;
                    MatingCueOffspringY = featured.ChildY;
                    MatingCueTick = featured.Tick;
                    break;
                }

                UpdateHuntFootprints(run, tick);

                var transitions = run.Metrics.BehaviorTransitions;
                for (var index = transitions.Count - 1; index >= 0; index--)
                {
                    var transition = transitions[index];
                    if (transition.Tick <= lastTick)
                    {
                        break;
                    }

                    if (transition.Species != FoxSpeciesId
                        || transition.CurrentState != SpeciesBehaviorState.Hunting)
                    {
                        continue;
                    }

                    if (IsCurrentHuntingFox(run.Cells, transition))
                    {
                        FoxHuntCueX = transition.X;
                        FoxHuntCueY = transition.Y;
                        FoxHuntCueTick = transition.Tick;
                        break;
                    }
                }
            }

            lastRun = run;
            lastTick = tick;
            snapshot = SimulationBoardSnapshot.Create(run, preview.ActiveSpeciesRules, preview.PlayerSpecies);
            if (!HasSelection)
            {
                ClearSelection();
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Snapshot)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasSelection)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCell)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InspectionText)));
        }

        static SpeciesBirthEvent[] SelectBirthCues(
            IReadOnlyList<SpeciesBirthEvent> births, int first, int last, SpeciesId playerSpecies)
        {
            var cues = new SpeciesBirthEvent[Math.Min(MaxBirthBoardCues, last - first + 1)];
            var count = 0;
            for (var index = last; index >= first && count < cues.Length; index--)
            {
                if (births[index].Species == playerSpecies)
                {
                    cues[count++] = births[index];
                }
            }
            for (var index = last; index >= first && count < cues.Length; index--)
            {
                if (births[index].Species != playerSpecies)
                {
                    cues[count++] = births[index];
                }
            }
            return cues;
        }

        static bool IsCurrentHuntingFox(Grid<SpeciesCell> cells, SpeciesBehaviorTransition transition)
        {
            if (!cells.IsInBounds(transition.X, transition.Y)
                || !cells.TryGetCell(transition.X, transition.Y, out var cell))
            {
                return false;
            }
            return cell.IsCreature
                && cell.SpeciesId == FoxSpeciesId
                && cell.EntityId == transition.EntityId
                && cell.BehaviorState == SpeciesBehaviorState.Hunting;
        }

        void UpdateHuntFootprints(SimulationRunState run, int tick)
        {
            for (var index = huntFootprints.Count - 1; index >= 0; index--)
            {
                if (tick - huntFootprints[index].Tick >= 9)
                {
                    huntFootprints.RemoveAt(index);
                }
            }

            long candidateId = 0;
            var candidateX = 0;
            var candidateY = 0;
            for (var y = 0; y < run.Cells.Height; y++)
            {
                for (var x = 0; x < run.Cells.Width; x++)
                {
                    if (!run.Cells.TryGetCell(x, y, out var cell)
                        || !cell.IsCreature || cell.SpeciesId != FoxSpeciesId
                        || cell.BehaviorState != SpeciesBehaviorState.Hunting)
                    {
                        continue;
                    }

                    if (cell.EntityId == trackedHuntingFoxId)
                    {
                        candidateId = cell.EntityId;
                        candidateX = x;
                        candidateY = y;
                        break;
                    }

                    if (candidateId == 0)
                    {
                        candidateId = cell.EntityId;
                        candidateX = x;
                        candidateY = y;
                    }
                }

                if (candidateId == trackedHuntingFoxId && candidateId != 0)
                {
                    break;
                }
            }

            if (candidateId != 0 && candidateId != trackedHuntingFoxId)
            {
                huntFootprints.Clear();
            }
            else if (candidateId != 0
                && (candidateX != trackedHuntingFoxX || candidateY != trackedHuntingFoxY)
                && Math.Abs(candidateX - trackedHuntingFoxX) <= 1
                && Math.Abs(candidateY - trackedHuntingFoxY) <= 1)
            {
                huntFootprints.Add(new HuntFootprint(
                    trackedHuntingFoxX,
                    trackedHuntingFoxY,
                    candidateX - trackedHuntingFoxX,
                    candidateY - trackedHuntingFoxY,
                    tick));
                if (huntFootprints.Count > 4)
                {
                    huntFootprints.RemoveAt(0);
                }
            }

            if (candidateId != 0)
            {
                trackedHuntingFoxId = candidateId;
                trackedHuntingFoxX = candidateX;
                trackedHuntingFoxY = candidateY;
            }
            else if (huntFootprints.Count == 0)
            {
                trackedHuntingFoxId = 0;
            }
        }
    }

    public readonly struct HuntFootprint
    {
        public HuntFootprint(int x, int y, int directionX, int directionY, int tick)
        {
            X = x;
            Y = y;
            DirectionX = directionX;
            DirectionY = directionY;
            Tick = tick;
        }

        public int X { get; }
        public int Y { get; }
        public int DirectionX { get; }
        public int DirectionY { get; }
        public int Tick { get; }
    }
}
