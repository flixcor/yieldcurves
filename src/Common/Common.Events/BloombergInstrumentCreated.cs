using System;
using Common.Core;

namespace Common.Events
{
    public class BloombergInstrumentCreated : Event
    {
        public BloombergInstrumentCreated(Guid id, string ticker, string pricingSource, string yellowKey) : base(id)
        {
            Ticker = ticker ?? throw new ArgumentNullException(nameof(ticker));
            PricingSource = pricingSource ?? throw new ArgumentNullException(nameof(pricingSource));
            YellowKey = yellowKey ?? throw new ArgumentNullException(nameof(yellowKey));
        }

        public string Ticker { get; }
        public string PricingSource { get; }
        public string YellowKey { get; }
    }
}
