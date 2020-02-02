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
        public BloombergInstrumentCreated(string ticker, string pricingSource, string yellowKey)
        {
            Ticker = ticker;
            PricingSource = pricingSource;
            YellowKey = yellowKey;
        }
    }
}
