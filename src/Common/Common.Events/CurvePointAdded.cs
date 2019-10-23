using System;
using Common.Core;

namespace Common.Events
{
    public class CurvePointAdded : Event
    {
        public CurvePointAdded(Guid id, string tenor, Guid instrumentId, short dateLag, bool isMandatory, string priceType) : base(id)
        {
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

    }
}
