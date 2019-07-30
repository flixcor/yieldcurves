using Common.Core;
using Common.Core.Events;
using System;

namespace Instruments.Domain
{
    public class BloombergInstrument : Aggregate<BloombergInstrument>
    {
        static BloombergInstrument()
        {
            RegisterApplyMethod<BloombergInstrumentCreated>(Apply);
        }

        private BloombergInstrument()
        {
        }

        public BloombergInstrument(Guid id, string ticker, PricingSource pricingSource, YellowKey yellowKey)
        {
            if (string.IsNullOrWhiteSpace(ticker))
            {
                throw new ArgumentNullException(nameof(ticker));
            }

            ApplyEvent(new BloombergInstrumentCreated(id, ticker, pricingSource.ToString(), yellowKey.ToString()));
        }

        private static void Apply(BloombergInstrument i, BloombergInstrumentCreated e)
        {
            i.Id = e.Id;
        }
    }
}
