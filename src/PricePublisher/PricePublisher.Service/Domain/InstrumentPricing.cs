using System;
using Common.Core;
using Common.EventStore.Lib;
using PricePublisher.Domain;
using static Common.Events.Helpers;

namespace PricePublisher.Service.Domain
{
    public class InstrumentPricing : Aggregate<InstrumentPricing>
    {
        public InstrumentPricing(Date asOfDate, DateTime asAtDate, Guid instrumentId, Price price, PriceType? priceType = null)
        {
            GenerateEvent(InstrumentPricingPublished(asOfDate.ToString(), asAtDate, instrumentId, price.Currency.ToString(), price.Value, priceType?.ToString()));
        }

        protected override void When(IEvent @event)
        {
        }
    }
}
