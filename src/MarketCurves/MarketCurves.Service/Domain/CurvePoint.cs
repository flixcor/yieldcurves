using System;

namespace MarketCurves.Domain
{
    public class CurvePoint
    {
        public CurvePoint(Tenor tenor, Guid instrumentId, DateLag dateLag, bool isMandatory, PriceType? priceType)
        {
            Tenor = tenor;
            InstrumentId = instrumentId;
            DateLag = dateLag ?? throw new ArgumentNullException(nameof(dateLag));
            IsMandatory = isMandatory;
            PriceType = priceType;
        }

        public Tenor Tenor { get; }
        public Guid InstrumentId { get; }
        public DateLag DateLag { get; }
        public bool IsMandatory { get; }
        public PriceType? PriceType { get; }
    }
}