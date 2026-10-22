using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SaltyGame
{
    public enum FieldDataPopulationInterventionKind
    {
        AddHare,
        RemoveHare,
        AddFox,
        RemoveFox,
        AddPlants,
        RemovePlants,
    }

    public readonly struct FieldDataPopulationPosition
    {
        public FieldDataPopulationPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }
    }

    /// <summary>Immutable record of one accepted journey-local population intervention.</summary>
    public sealed class FieldDataPopulationInterventionReceipt
    {
        readonly ReadOnlyCollection<FieldDataPopulationPosition> affectedPositions;

        internal FieldDataPopulationInterventionReceipt(
            string requestId,
            FieldDataPopulationInterventionKind kind,
            SpeciesId speciesId,
            int quantity,
            int cost,
            int balanceBefore,
            int balanceAfter,
            IEnumerable<FieldDataPopulationPosition> affectedPositions)
        {
            RequestId = requestId;
            Kind = kind;
            SpeciesId = speciesId;
            Quantity = quantity;
            Cost = cost;
            BalanceBefore = balanceBefore;
            BalanceAfter = balanceAfter;
            this.affectedPositions = new ReadOnlyCollection<FieldDataPopulationPosition>(
                new List<FieldDataPopulationPosition>(affectedPositions));
        }

        public string RequestId { get; }
        public FieldDataPopulationInterventionKind Kind { get; }
        public SpeciesId SpeciesId { get; }
        public int Quantity { get; }
        public int Cost { get; }
        public int BalanceBefore { get; }
        public int BalanceAfter { get; }
        public IReadOnlyList<FieldDataPopulationPosition> AffectedPositions => affectedPositions;
    }

    internal sealed class PreparedFieldDataPopulationChange
    {
        public PreparedFieldDataPopulationChange(
            Grid<SpeciesCell> expectedCells,
            Grid<SpeciesCell> resultCells,
            FieldDataPopulationInterventionKind kind,
            SpeciesId speciesId,
            int quantity,
            IReadOnlyList<FieldDataPopulationPosition> positions)
        {
            ExpectedCells = expectedCells;
            ResultCells = resultCells;
            Kind = kind;
            SpeciesId = speciesId;
            Quantity = quantity;
            Positions = positions;
        }

        public Grid<SpeciesCell> ExpectedCells { get; }
        public Grid<SpeciesCell> ResultCells { get; }
        public FieldDataPopulationInterventionKind Kind { get; }
        public SpeciesId SpeciesId { get; }
        public int Quantity { get; }
        public IReadOnlyList<FieldDataPopulationPosition> Positions { get; }
    }
}
