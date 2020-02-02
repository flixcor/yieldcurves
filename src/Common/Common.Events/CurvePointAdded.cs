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
        public CurvePointAdded(string tenor, Guid instrumentId, short dateLag, bool isMandatory, string priceType)
        {
            Tenor = tenor;
            InstrumentId = instrumentId.ToString();
            DateLag = dateLag;
            IsMandatory = isMandatory;
            PriceType = priceType;
        }

        Guid ICurvePointAdded.InstrumentId => Guid.Parse(InstrumentId);
    }
}
