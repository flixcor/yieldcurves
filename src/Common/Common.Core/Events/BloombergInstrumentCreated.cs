using Newtonsoft.Json;
using System;

namespace Common.Core.Events
{
    public class BloombergInstrumentCreated : Event
    {
        public BloombergInstrumentCreated(Guid id, string ticker, string pricingSource, string yellowKey) : base(id)
        {
            Ticker = ticker ?? throw new ArgumentNullException(nameof(ticker));
            PricingSource = pricingSource ?? throw new ArgumentNullException(nameof(pricingSource));
            YellowKey = yellowKey ?? throw new ArgumentNullException(nameof(yellowKey));
        }

        [JsonConstructor]
        protected BloombergInstrumentCreated(Guid id, string ticker, string pricingSource, string yellowKey, int version) : this(id, ticker, pricingSource, yellowKey)
        {
            Version = version;
        }

        public string Ticker { get; }
        public string PricingSource { get; }
        public string YellowKey { get; }
    }
}