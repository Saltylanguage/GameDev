using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SaltyGame
{
    public enum FieldDataShopOfferKind
    {
        SpeciesEffect,
        IncomeModifier,
    }

    public sealed class FieldDataShopOffer
    {
        internal FieldDataShopOffer(FieldDataSignedSpeciesOffer offer)
        {
            Kind = FieldDataShopOfferKind.SpeciesEffect;
            Id = offer.Id;
            DisplayName = FormatSpeciesName(offer);
            Cost = offer.Cost;
            FamilyId = offer.FamilyId;
            SignedSpeciesOffer = offer;
        }

        internal FieldDataShopOffer(FieldDataIncomeOffer offer)
        {
            Kind = FieldDataShopOfferKind.IncomeModifier;
            Id = offer.Id;
            DisplayName = offer.DisplayName;
            Cost = offer.ShopCost;
            IncomeOffer = offer;
        }

        public FieldDataShopOfferKind Kind { get; }
        public string Id { get; }
        public string DisplayName { get; }
        public int Cost { get; }
        public string FamilyId { get; }
        public FieldDataSignedSpeciesOffer? SignedSpeciesOffer { get; }
        public FieldDataIncomeOffer? IncomeOffer { get; }
        public bool Locked { get; internal set; }
        public bool Purchased { get; internal set; }

        static string FormatSpeciesName(FieldDataSignedSpeciesOffer offer)
        {
            string name;
            switch (offer.FamilyId)
            {
                case "long-stride": name = "Long Stride"; break;
                case "sharper-instinct": name = "Sharper Instinct"; break;
                case "reinforced-hide": name = "Reinforced Hide"; break;
                case "warren-comfort": name = "Warren Comfort"; break;
                case "deep-digestion": name = "Deep Digestion"; break;
                case "abundant-seedfall": name = "Abundant Seedfall"; break;
                default: name = offer.FamilyId; break;
            }
            return (offer.SignedAmount > 0f ? "+ " : "− ") + name;
        }
    }

    public sealed class FieldDataShopVisit
    {
        readonly List<FieldDataShopOffer> offers;
        readonly HashSet<string> rerolledOfferIds = new HashSet<string>(StringComparer.Ordinal);

        internal FieldDataShopVisit(int visitNumber, IEnumerable<FieldDataShopOffer> offers)
        {
            VisitNumber = visitNumber;
            this.offers = new List<FieldDataShopOffer>(offers);
        }

        public int VisitNumber { get; }
        public IReadOnlyList<FieldDataShopOffer> Offers => new ReadOnlyCollection<FieldDataShopOffer>(offers);
        internal List<FieldDataShopOffer> MutableOffers => offers;
        internal HashSet<string> RerolledOfferIds => rerolledOfferIds;
        internal int RerollOrdinal { get; set; }
        public int ReorderPriceStep { get; internal set; }
    }
}
