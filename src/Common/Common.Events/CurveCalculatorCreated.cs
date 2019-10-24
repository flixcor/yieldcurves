﻿using System;
using Common.Core;

namespace Common.Events
{
    public class CurveCalculatorCreated : IEvent
    {
        public CurveCalculatorCreated(Guid aggregateId, Guid marketCurveId, DateTime asOfDate, int version = 0)
        {
            AggregateId = aggregateId;
            Version = version;
            MarketCurveId = marketCurveId;
            AsOfDate = asOfDate;
        }

        public Guid MarketCurveId { get; }
        public DateTime AsOfDate { get; }
        public Guid AggregateId { get; }
        public int Version { get; private set; }
		
		public IEvent WithVersion(int version)
		{
			var clone = (dynamic)MemberwiseClone();
			clone.Version = version;
			return clone;
		}
    }
}
