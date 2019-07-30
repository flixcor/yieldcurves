using System;
using CalculationEngine.Service.Domain;

namespace CalculationEngine.Domain
{
    public class PublishedPricing
    {
        public PublishedPricing(Date asOfDate, DateTime asAtDate, Guid instrumentId, Price price, PriceType? priceType = null)
        {
            AsOfDate = asOfDate;
            AsAtDate = asAtDate;
            InstrumentId = instrumentId;
            Price = price;
            PriceType = priceType;
        }

        public Date AsOfDate { get; }
        public DateTime AsAtDate { get; }
        public Guid InstrumentId { get; }
        public Price Price { get; }
        public PriceType? PriceType { get; }
    }
}
