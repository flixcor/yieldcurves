using System;
using Common.Core;

namespace Common.Events
{
    public class InstrumentPricingPublished : IEvent
    {
        public InstrumentPricingPublished(Guid id, DateTime asOfDate, DateTime asAtDate, Guid instrumentId, string priceCurrency, double priceAmount, string priceType = null, int version = 0)
        {
            Id = id;
            AsOfDate = asOfDate;
            AsAtDate = asAtDate;
            InstrumentId = instrumentId;
            PriceCurrency = priceCurrency;
            PriceAmount = priceAmount;
            PriceType = priceType;
            Version = version;
        }

        public Guid Id { get; }
        public int Version { get; }
        public DateTime AsOfDate { get; }
        public DateTime AsAtDate { get; }
        public Guid InstrumentId { get; }
        public string PriceCurrency { get; }
        public double PriceAmount { get; }
        public string PriceType { get; }

    }
}
