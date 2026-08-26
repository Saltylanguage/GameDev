using System;
using System.ComponentModel;
using Noesis;
using UnityEngine;

namespace SaltyGame
{
    /// <summary>
    /// Projects the current simulation run into the board view's immutable snapshot.
    /// </summary>
    public sealed class VM_SimulationBoard : MonoBehaviour, INotifyPropertyChanged
    {
        SpeciesSimulationPreview preview;
        SpeciesSimulationBoard board;
        SimulationRunState lastRun;
        int lastTick = -1;
        SimulationBoardSnapshot snapshot;
        CroppedBitmap[] animalSprites;
        CroppedBitmap[] grassTerrainTiles;
        CroppedBitmap[] desertTerrainTiles;
        int selectedX = -1;
        int selectedY = -1;

        public event PropertyChangedEventHandler PropertyChanged;

        public SimulationBoardSnapshot Snapshot => snapshot;
        public bool HasSelection => snapshot != null && snapshot.TryGetCell(selectedX, selectedY, out _);
        public int SelectedX => selectedX;
        public int SelectedY => selectedY;
        public SimulationCellSnapshot SelectedCell => HasSelection
            ? snapshot.GetCell(selectedX, selectedY)
            : default;

        public void SetSpriteVisuals(
            CroppedBitmap[] animals,
            CroppedBitmap[] grassTerrain,
            CroppedBitmap[] desertTerrain)
        {
            animalSprites = animals;
            grassTerrainTiles = grassTerrain;
            desertTerrainTiles = desertTerrain;
            board?.SetSpriteVisuals(animalSprites, grassTerrainTiles, desertTerrainTiles);
        }

        public void Initialize(SpeciesSimulationPreview simulationPreview)
        {
            preview = simulationPreview ?? throw new ArgumentNullException(nameof(simulationPreview));
            Refresh(true);
        }

        public void BindToView(NoesisView view)
        {
            if (view == null || view.Content == null)
            {
                return;
            }

            board = view.Content.FindName("SimulationBoard") as SpeciesSimulationBoard;
            board?.SetSpriteVisuals(animalSprites, grassTerrainTiles, desertTerrainTiles);
            board?.SetSnapshot(snapshot);
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

            lastRun = run;
            lastTick = tick;
            snapshot = SimulationBoardSnapshot.Create(run, preview.ActiveSpeciesRules, preview.PlayerSpecies);
            board?.SetSnapshot(snapshot);
            if (!HasSelection)
            {
                ClearSelection();
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Snapshot)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasSelection)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCell)));
        }
    }
}
