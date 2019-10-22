using System;
using Common.Core;

namespace Common.Events
{
    public class BloombergInstrumentCreated : IEvent
    {
        public BloombergInstrumentCreated(Guid id, string ticker, string pricingSource, string yellowKey, int version = 0)
        {
            Id = id;
            Ticker = ticker ?? throw new ArgumentNullException(nameof(ticker));
            PricingSource = pricingSource ?? throw new ArgumentNullException(nameof(pricingSource));
            YellowKey = yellowKey ?? throw new ArgumentNullException(nameof(yellowKey));
            Version = version;
        }

        public Guid Id { get; }
        public int Version { get; }

        public string Ticker { get; }
        public string PricingSource { get; }
        public string YellowKey { get; }
    }
}
