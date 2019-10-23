using System;
using Common.Core;

namespace Common.Events
{
    public class InstrumentPricingPublished : Event
    {
        public InstrumentPricingPublished(Guid id, DateTime asOfDate, DateTime asAtDate, Guid instrumentId, string priceCurrency, double priceAmount, string priceType = null) : base(id)
        {
            AsOfDate = asOfDate;
            AsAtDate = asAtDate;
            InstrumentId = instrumentId;
            PriceCurrency = priceCurrency;
            PriceAmount = priceAmount;
            PriceType = priceType;
        }

        public DateTime AsOfDate { get; }
        public DateTime AsAtDate { get; }
        public Guid InstrumentId { get; }
        public string PriceCurrency { get; }
        public double PriceAmount { get; }
        public string PriceType { get; }

    }
}
