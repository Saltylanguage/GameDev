using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SaltyGame
{
    public enum FieldDataIncomeReorderKind
    {
        Random,
        Chosen,
    }

    public readonly struct FieldDataIncomeAcquisitionRecord
    {
        public FieldDataIncomeAcquisitionRecord(int sequence, string instanceId, string offerId, string sourceId)
        {
            Sequence = sequence;
            InstanceId = instanceId;
            OfferId = offerId;
            SourceId = sourceId;
        }
        public int Sequence { get; }
        public string InstanceId { get; }
        public string OfferId { get; }
        public string SourceId { get; }
    }

    public sealed class FieldDataIncomeReorderReceipt
    {
        readonly ReadOnlyCollection<string> previousOrder;
        readonly ReadOnlyCollection<string> nextOrder;

        internal FieldDataIncomeReorderReceipt(
            string requestId,
            int shopVisit,
            FieldDataIncomeReorderKind kind,
            int priceStep,
            int cost,
            IEnumerable<string> previousOrder,
            IEnumerable<string> nextOrder)
        {
            RequestId = requestId;
            ShopVisit = shopVisit;
            Kind = kind;
            PriceStep = priceStep;
            Cost = cost;
            this.previousOrder = new ReadOnlyCollection<string>(new List<string>(previousOrder));
            this.nextOrder = new ReadOnlyCollection<string>(new List<string>(nextOrder));
        }

        public string RequestId { get; }
        public int ShopVisit { get; }
        public FieldDataIncomeReorderKind Kind { get; }
        public int PriceStep { get; }
        public int Cost { get; }
        public IReadOnlyList<string> PreviousOrder => previousOrder;
        public IReadOnlyList<string> NextOrder => nextOrder;
    }

    public sealed class FieldDataChosenReorderDraft
    {
        readonly ReadOnlyCollection<string> currentOrder;
        internal FieldDataChosenReorderDraft(int shopVisit, int priceStep, int cost, IEnumerable<string> currentOrder)
        {
            ShopVisit = shopVisit;
            PriceStep = priceStep;
            Cost = cost;
            this.currentOrder = new ReadOnlyCollection<string>(new List<string>(currentOrder));
        }
        public int ShopVisit { get; }
        public int PriceStep { get; }
        public int Cost { get; }
        public IReadOnlyList<string> CurrentOrder => currentOrder;
    }
}
