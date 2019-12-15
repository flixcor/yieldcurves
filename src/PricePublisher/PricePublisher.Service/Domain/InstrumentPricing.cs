using System;
using Common.Core;
using Common.Events;
using PricePublisher.Domain;
using static Common.Events.Create;

namespace PricePublisher.Service.Domain
{
    public class InstrumentPricing : Aggregate<InstrumentPricing>
    {
        static InstrumentPricing()
        {
            RegisterApplyMethod<IInstrumentPricingPublished>(Apply);
        }

        public InstrumentPricing(Guid id, Date asOfDate, DateTime asAtDate, Guid instrumentId, Price price, PriceType? priceType = null)
        {
            ApplyEvent(InstrumentPricingPublished(id, asOfDate.ToString(), asAtDate, instrumentId, price.Currency.ToString(), price.Value, priceType?.ToString()));
        }

        private static void Apply(InstrumentPricing i, IInstrumentPricingPublished e)
        {
            i.Id = e.AggregateId;
        }
    }
}
