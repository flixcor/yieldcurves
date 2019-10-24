using System;
using Common.Core;

namespace Common.Events
{
    public class CurvePointAdded : IEvent
    {
        public CurvePointAdded(Guid aggregateId, string tenor, Guid instrumentId, short dateLag, bool isMandatory, string priceType, int version = 0)
        {
            AggregateId = aggregateId;
            Version = version;
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
        public int Version { get; }
		
		public IEvent WithVersion(int version)
		{
			throw new NotImplementedException();
		}
    }
}
