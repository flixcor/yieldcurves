using System;
using Common.Core;

namespace Common.Events
{
    public interface IBloombergInstrumentCreated : IEvent
    {
        string PricingSource { get; }
        string Ticker { get; }
        string YellowKey { get; }
    }

    internal partial class BloombergInstrumentCreated : IBloombergInstrumentCreated
    {
        public BloombergInstrumentCreated(Guid aggregateId, string ticker, string pricingSource, string yellowKey)
        {
            AggregateId = aggregateId.ToString();
            Ticker = ticker ?? throw new ArgumentNullException(nameof(ticker));
            PricingSource = pricingSource ?? throw new ArgumentNullException(nameof(pricingSource));
            YellowKey = yellowKey ?? throw new ArgumentNullException(nameof(yellowKey));
        }

        Guid IEvent.AggregateId => Guid.Parse(AggregateId);

        public IEvent WithVersion(int version)
        {
            var clone = (BloombergInstrumentCreated)MemberwiseClone();
            clone.Version = version;
            return clone;
        }
    }
}
