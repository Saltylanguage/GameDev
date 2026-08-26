using System;
using System.Collections.Generic;

namespace SaltyGame
{
    /// <summary>
    /// Immutable presentation projection of one simulation board state.
    /// </summary>
    public sealed class SimulationBoardSnapshot
    {
        readonly SimulationCellSnapshot[] cells;
        readonly IReadOnlyDictionary<SpeciesId, SpeciesRole> speciesRoles;

        SimulationBoardSnapshot(
            int width,
            int height,
            int tick,
            SimulationRunStatus status,
            SpeciesId playerSpecies,
            SimulationCellSnapshot[] cells,
            IReadOnlyDictionary<SpeciesId, SpeciesRole> speciesRoles)
        {
            Width = width;
            Height = height;
            Tick = tick;
            Status = status;
            PlayerSpecies = playerSpecies;
            this.cells = cells;
            this.speciesRoles = speciesRoles;
        }

        public static SimulationBoardSnapshot Create(
            SimulationRunState run,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules,
            SpeciesId playerSpecies)
        {
            if (run == null)
            {
                return null;
            }

            var cells = new SimulationCellSnapshot[run.Cells.Count];
            for (var y = 0; y < run.Cells.Height; y++)
            {
                for (var x = 0; x < run.Cells.Width; x++)
                {
                    var cell = run.Cells.GetCell(x, y);
                    var terrainMask = TerrainTileResolver.ResolveTerrainMask(
                        run.Cells,
                        x,
                        y,
                        cell.TerrainId);
                    cells[x + y * run.Cells.Width] = SimulationCellSnapshot.Create(cell, terrainMask);
                }
            }

            var speciesRoles = new Dictionary<SpeciesId, SpeciesRole>();
            if (rules != null)
            {
                foreach (var entry in rules)
                {
                    speciesRoles[entry.Key] = entry.Value.Role;
                }
            }

            return new SimulationBoardSnapshot(
                run.Cells.Width,
                run.Cells.Height,
                run.Tick,
                run.Status,
                playerSpecies,
                cells,
                speciesRoles);
        }

        public int Width { get; }
        public int Height { get; }
        public int Revision => Tick;
        public int Tick { get; }
        public SimulationRunStatus Status { get; }
        public SpeciesId PlayerSpecies { get; }
        public IReadOnlyList<SimulationCellSnapshot> Cells => cells;
        public IReadOnlyDictionary<SpeciesId, SpeciesRole> SpeciesRoles => speciesRoles;

        public SimulationCellSnapshot GetCell(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                throw new ArgumentOutOfRangeException(nameof(x), $"Cell ({x}, {y}) is outside the {Width} x {Height} board.");
            }

            return cells[x + y * Width];
        }

        public bool TryGetCell(int x, int y, out SimulationCellSnapshot cell)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                cell = default;
                return false;
            }

            cell = cells[x + y * Width];
            return true;
        }
    }

    public readonly struct SimulationCellSnapshot
    {
        SimulationCellSnapshot(SpeciesCell cell, int terrainVariantMask)
        {
            IsOccupied = cell.IsOccupied;
            IsCreature = cell.IsCreature;
            IsTerrainResource = cell.IsTerrainResource;
            IsPlantResource = cell.IsPlantResource;
            IsPassable = cell.IsPassable;
            SpeciesId = cell.SpeciesId;
            ResourceSpeciesId = cell.ResourceSpeciesId;
            TerrainId = cell.TerrainId;
            EntityId = cell.EntityId;
            Health = cell.Health;
            Energy = cell.Energy;
            Age = cell.Age;
            FoodEaten = cell.FoodEaten;
            FoodReserve = cell.FoodReserve;
            IsAlpha = cell.IsAlpha;
            TerrainEnergy = cell.TerrainEnergy;
            MovementCost = cell.MovementCost;
            BehaviorState = cell.BehaviorState;
            BehaviorStateTicks = cell.BehaviorStateTicks;
            AttackCooldownTicksRemaining = cell.AttackCooldownTicksRemaining;
            TerrainVariantMask = terrainVariantMask;
        }

        public static SimulationCellSnapshot Create(SpeciesCell cell, int terrainVariantMask)
        {
            return new SimulationCellSnapshot(cell, terrainVariantMask);
        }

        public bool IsOccupied { get; }
        public bool IsCreature { get; }
        public bool IsTerrainResource { get; }
        public bool IsPlantResource { get; }
        public bool IsPassable { get; }
        public SpeciesId SpeciesId { get; }
        public SpeciesId ResourceSpeciesId { get; }
        public TerrainId TerrainId { get; }
        public long EntityId { get; }
        public int Health { get; }
        public int Energy { get; }
        public int Age { get; }
        public int FoodEaten { get; }
        public float FoodReserve { get; }
        public bool IsAlpha { get; }
        public float TerrainEnergy { get; }
        public float MovementCost { get; }
        public SpeciesBehaviorState BehaviorState { get; }
        public int BehaviorStateTicks { get; }
        public int AttackCooldownTicksRemaining { get; }
        public int TerrainVariantMask { get; }
    }
}
