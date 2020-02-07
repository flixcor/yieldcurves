using Common.Core;
using Common.EventStore.Lib;
using static Common.Events.Helpers;

namespace Instruments.Domain
{
    public class BloombergInstrument : Aggregate<BloombergInstrument>
    {
        private BloombergInstrument() { }

        public BloombergInstrument(string ticker, PricingSource pricingSource, YellowKey yellowKey)
        {
            GenerateEvent(BloombergInstrumentCreated(ticker, pricingSource.ToString(), yellowKey.ToString()));
            GenerateEvent(InstrumentCreated(Vendor.Bloomberg.ToString(), $"{ticker} {pricingSource} {yellowKey}", true));
        }

        protected override void When(IEvent @event)
        {
        }
    }
}
