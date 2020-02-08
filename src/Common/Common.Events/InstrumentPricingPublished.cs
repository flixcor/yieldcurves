using System;
using Common.Core;
using Google.Protobuf.WellKnownTypes;

namespace Common.Events
{
    public interface IInstrumentPricingPublished : IEvent
    {
        string AsOfDate { get; }
        Guid InstrumentId { get; }
        double PriceAmount { get; }
        string PriceCurrency { get; }
        string PriceType { get; }
    }

    internal partial class InstrumentPricingPublished : IInstrumentPricingPublished
    {
        public InstrumentPricingPublished(string asOfDate, Guid instrumentId, string priceCurrency, double priceAmount, string? priceType = null)
        {
            AsOfDate = asOfDate;
            InstrumentId = instrumentId;
            PriceCurrency = priceCurrency;
            PriceAmount = priceAmount;

            if (!string.IsNullOrWhiteSpace(priceType))
            {
                PriceType = priceType;
            }
        }

        Guid IInstrumentPricingPublished.InstrumentId => InstrumentId;
    }
}
