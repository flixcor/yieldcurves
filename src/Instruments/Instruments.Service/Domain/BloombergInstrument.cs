using Common.Core;
using Common.EventStore.Lib;
using static Common.Events.Helpers;

namespace Instruments.Domain
{
    public class BloombergInstrument : Aggregate<BloombergInstrument>
    {
        public BloombergInstrument Define(string ticker, PricingSource pricingSource, YellowKey yellowKey)
        {
            GenerateEvent(BloombergInstrumentCreated(ticker, pricingSource.ToString(), yellowKey.ToString()));
            GenerateEvent(InstrumentCreated(Vendor.Bloomberg.ToString(), $"{ticker} {pricingSource} {yellowKey}", true));
            return this;
        }

        protected override void When(IEvent @event)
        {
        }
    }
}
