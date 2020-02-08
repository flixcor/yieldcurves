using Common.Core;
using Common.EventStore.Lib;
using static Common.Events.Helpers;

namespace Instruments.Domain
{
    public class BloombergInstrument : Aggregate<BloombergInstrument>
    {
        public BloombergInstrument Define(NonEmptyString ticker, PricingSource pricingSource, YellowKey yellowKey)
        {
            GenerateEvent(BloombergInstrumentCreated(ticker, pricingSource.NonEmptyString(), yellowKey.NonEmptyString()));
            GenerateEvent(InstrumentCreated(Vendor.Bloomberg.NonEmptyString(), $"{ticker} {pricingSource} {yellowKey}".NonEmpty(), true));
            return this;
        }

        protected override void When(IEvent @event)
        {
        }
    }
}
