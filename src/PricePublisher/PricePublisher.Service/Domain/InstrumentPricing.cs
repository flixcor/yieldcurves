using System;
using Common.Core;
using Common.EventStore.Lib;
using PricePublisher.Domain;
using static Common.Events.Helpers;

namespace PricePublisher.Service.Domain
{
    public class InstrumentPricing : Aggregate<InstrumentPricing>
    {
        public InstrumentPricing Define(Date asOfDate, NonEmptyGuid instrumentId, Price price, PriceType? priceType = null)
        {
            GenerateEvent(InstrumentPricingPublished(asOfDate.NonEmptyString(), instrumentId, price.Currency.ToString().NonEmpty(), price.Value, priceType?.NonEmptyString()));
            return this;
        }

        protected override void When(IEvent @event)
        {
        }
    }
}
