using System;
using Common.Core;

namespace Common.Events
{
    public class BloombergInstrumentCreated : IEvent
    {
        public BloombergInstrumentCreated(Guid aggregateId, string ticker, string pricingSource, string yellowKey, int version = 0)
        {
            AggregateId = aggregateId;
            Version = version;
            Ticker = ticker ?? throw new ArgumentNullException(nameof(ticker));
            PricingSource = pricingSource ?? throw new ArgumentNullException(nameof(pricingSource));
            YellowKey = yellowKey ?? throw new ArgumentNullException(nameof(yellowKey));
        }

        public string Ticker { get; }
        public string PricingSource { get; }
        public string YellowKey { get; }

        public Guid AggregateId { get; }
        public int Version { get; }
		
		public IEvent WithVersion(int version)
		{
			throw new NotImplementedException();
		}
    }
}
