using System;
using Common.Core;
using Google.Protobuf.WellKnownTypes;

namespace Common.Events
{
    public interface IInstrumentPricingPublished : IEvent
    {
        DateTime AsAtDate { get; }
        string AsOfDate { get; }
        Guid InstrumentId { get; }
        double PriceAmount { get; }
        string PriceCurrency { get; }
        string PriceType { get; }
    }

    internal partial class InstrumentPricingPublished : IInstrumentPricingPublished
    {
        public InstrumentPricingPublished(Guid aggregateId, string asOfDate, DateTime asAtDate, Guid instrumentId, string priceCurrency, double priceAmount, string priceType = null)
        {
            AggregateId = aggregateId.ToString("N");
            AsOfDate = asOfDate;
            AsAtDate = Timestamp.FromDateTime(asAtDate.ToUniversalTime());
            InstrumentId = instrumentId.ToString();
            PriceCurrency = priceCurrency;
            PriceAmount = priceAmount;

            if (!string.IsNullOrWhiteSpace(priceType))
            {
                PriceType = priceType;
            }
        }

        DateTime IInstrumentPricingPublished.AsAtDate => AsAtDate.ToDateTime();

        Guid IInstrumentPricingPublished.InstrumentId => Guid.Parse(InstrumentId);

        Guid IEvent.AggregateId => Guid.Parse(AggregateId);

        public IEvent WithVersion(int version)
        {
            var clone = (InstrumentPricingPublished)MemberwiseClone();
            clone.Version = version;
            return clone;
        }
    }
}
