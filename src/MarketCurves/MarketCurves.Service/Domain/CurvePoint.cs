using System;
using Common.Core;

namespace MarketCurves.Domain
{
    public class CurvePoint
    {
        public CurvePoint(Tenor tenor, NonEmptyGuid instrumentId, DateLag dateLag, bool isMandatory, PriceType? priceType)
        {
            Tenor = tenor;
            InstrumentId = instrumentId;
            DateLag = dateLag;
            IsMandatory = isMandatory;
            PriceType = priceType;
        }

        public Tenor Tenor { get; }
        public NonEmptyGuid InstrumentId { get; }
        public DateLag DateLag { get; }
        public bool IsMandatory { get; }
        public PriceType? PriceType { get; }
    }
}
