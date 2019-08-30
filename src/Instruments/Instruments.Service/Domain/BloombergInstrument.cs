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

        public static Result<BloombergInstrument> TryCreate(Guid id, string ticker, PricingSource pricingSource, YellowKey yellowKey)
        {
            if (string.IsNullOrWhiteSpace(ticker))
            {
                return Result.Fail<BloombergInstrument>($"{nameof(ticker)} cannot be empty");
            }

            var instrument = new BloombergInstrument(id, ticker, pricingSource, yellowKey);
            return Result.Ok(instrument);
        }

        private BloombergInstrument(Guid id, string ticker, PricingSource pricingSource, YellowKey yellowKey)
        {
            ApplyEvent(new BloombergInstrumentCreated(id, ticker, pricingSource.ToString(), yellowKey.ToString()));
            ApplyEvent(new InstrumentCreated(id, Vendor.Bloomberg.ToString(), $"{ticker} {pricingSource} {yellowKey}", true));
        }

        private static void Apply(BloombergInstrument i, BloombergInstrumentCreated e)
        {
            i.Id = e.Id;
        }
    }
}
