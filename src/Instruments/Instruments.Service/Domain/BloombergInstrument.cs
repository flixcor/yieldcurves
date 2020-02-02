using Common.Core;
using Common.EventStore.Lib;
using static Common.Events.Create;

namespace Instruments.Domain
{
    public class BloombergInstrument : Aggregate<BloombergInstrument>
    {
        private BloombergInstrument() { }

        public BloombergInstrument(string ticker, PricingSource pricingSource, YellowKey yellowKey)
        {
            ApplyEvent(BloombergInstrumentCreated(ticker, pricingSource.ToString(), yellowKey.ToString()));
            ApplyEvent(InstrumentCreated(Vendor.Bloomberg.ToString(), $"{ticker} {pricingSource} {yellowKey}", true));
        }

        protected override void Apply(IEvent @event)
        {
        }
    }
}
