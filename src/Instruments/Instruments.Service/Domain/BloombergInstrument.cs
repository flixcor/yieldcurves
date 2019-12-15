using Common.Core;
using Common.Events;
using System;
using static Common.Events.Create;

namespace Instruments.Domain
{
    public class BloombergInstrument : Aggregate<BloombergInstrument>
    {
        static BloombergInstrument()
        {
            RegisterApplyMethod<IBloombergInstrumentCreated>(Apply);
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
            ApplyEvent(BloombergInstrumentCreated(id, ticker, pricingSource.ToString(), yellowKey.ToString()));
            ApplyEvent(InstrumentCreated(id, Vendor.Bloomberg.ToString(), $"{ticker} {pricingSource} {yellowKey}", true));
        }

        private static void Apply(BloombergInstrument i, IBloombergInstrumentCreated e)
        {
            i.Id = e.AggregateId;
        }
    }
}
