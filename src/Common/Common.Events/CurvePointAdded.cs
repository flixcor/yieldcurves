using System;
using Common.Core;

namespace Common.Events
{
    public class CurvePointAdded : IEvent
    {
        public CurvePointAdded(Guid aggregateId, string tenor, Guid instrumentId, short dateLag, bool isMandatory, string priceType)
        {
            AggregateId = aggregateId;
            Tenor = tenor;
            InstrumentId = instrumentId;
            DateLag = dateLag;
            IsMandatory = isMandatory;
            PriceType = priceType;
        }

        public string Tenor { get; }
        public Guid InstrumentId { get; }
        public short DateLag { get; }
        public bool IsMandatory { get; }
        public string PriceType { get; }

        public Guid AggregateId { get; }
        public int Version { get; private set; }
		
		public IEvent WithVersion(int version)
		{
			var clone = (CurvePointAdded)MemberwiseClone();
			clone.Version = version;
			return clone;
		}
    }
}
