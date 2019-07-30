using System;
using Common.Core;
using Common.Core.Events;

namespace PricePublisher.Domain
{
    public class InstrumentPricing : Aggregate<InstrumentPricing>
    {
        static InstrumentPricing()
        {
            RegisterApplyMethod<InstrumentPricingPublished>(Apply);
        }

        public InstrumentPricing(Guid id, DateTime asOfDate, DateTime asAtDate, Guid instrumentId, Price price, PriceType? priceType = null)
        {
            ApplyEvent(new InstrumentPricingPublished(id, asOfDate, asAtDate, instrumentId, price.Currency.ToString(), price.Value, priceType?.ToString()));
        }

        private static void Apply(InstrumentPricing i, InstrumentPricingPublished e)
        {
            i.Id = e.Id;
        }
    }
}
