using System;
using Common.Core;

namespace Common.Events
{
    public interface ICurvePointAdded : IEvent
    {
        int DateLag { get; }
        Guid InstrumentId { get; }
        bool IsMandatory { get; }
        string PriceType { get; }
        string Tenor { get; }
    }

    internal partial class CurvePointAdded : ICurvePointAdded
    {
        public CurvePointAdded(Guid aggregateId, string tenor, Guid instrumentId, short dateLag, bool isMandatory, string priceType)
        {
            AggregateId = aggregateId.ToString();
            Tenor = tenor;
            InstrumentId = instrumentId.ToString();
            DateLag = dateLag;
            IsMandatory = isMandatory;
            PriceType = priceType;
        }

        Guid ICurvePointAdded.InstrumentId => Guid.Parse(InstrumentId);

        Guid IEvent.AggregateId => Guid.Parse(AggregateId);

        public IEvent WithVersion(int version)
        {
            var clone = (CurvePointAdded)MemberwiseClone();
            clone.Version = version;
            return clone;
        }
    }
}
