using System;
using Common.Core;

namespace Common.Events
{
    public class InstrumentPricingPublished : IEvent
    {
        public InstrumentPricingPublished(Guid aggregateId, DateTime asOfDate, DateTime asAtDate, Guid instrumentId, string priceCurrency, double priceAmount, string priceType = null)
        {
            AggregateId = aggregateId;
            AsOfDate = asOfDate;
            AsAtDate = asAtDate;
            InstrumentId = instrumentId;
            PriceCurrency = priceCurrency;
            PriceAmount = priceAmount;
            PriceType = priceType;
        }

        public DateTime AsOfDate { get; }
        public DateTime AsAtDate { get; }
        public Guid InstrumentId { get; }
        public string PriceCurrency { get; }
        public double PriceAmount { get; }
        public string PriceType { get; }

        public Guid AggregateId { get; }
        public int Version { get; private set; }
		
		public IEvent WithVersion(int version)
		{
			var clone = (InstrumentPricingPublished)MemberwiseClone();
			clone.Version = version;
			return clone;
		}
    }
}
