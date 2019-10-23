using System;
using Common.Core;

namespace Common.Events
{
    public class CurveCalculatorCreated : Event
    {
        public CurveCalculatorCreated(Guid id, Guid marketCurveId, DateTime asOfDate) : base(id)
        {
            MarketCurveId = marketCurveId;
            AsOfDate = asOfDate;
        }

        public Guid MarketCurveId { get; }
        public DateTime AsOfDate { get; }
    }
}
