using System;
using Common.Core;

namespace Common.Events
{
    public class CurveCalculatorCreated : IEvent
    {
        public CurveCalculatorCreated(Guid id, Guid marketCurveId, DateTime asOfDate, int version = 0)
        {
            Id = id;
            MarketCurveId = marketCurveId;
            AsOfDate = asOfDate;
            Version = version;
        }

        public Guid Id { get; }
        public int Version { get; }

        public Guid MarketCurveId { get; }
        public DateTime AsOfDate { get; }
    }
}
