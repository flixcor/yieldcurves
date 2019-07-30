﻿using System;
using Common.Core;
using Newtonsoft.Json;

namespace CalculationEngine.Domain
{
    public class CurveCalculatorCreated : Event
    {
        public Guid MarketCurveId { get; }
        public DateTime AsOfDate { get; }

        [JsonConstructor]
        private CurveCalculatorCreated(Guid id, Guid marketCurveId, DateTime asOfDate, int version) : this(id, marketCurveId, asOfDate)
        {
            Version = version;
        }

        public CurveCalculatorCreated(Guid id, Guid marketCurveId, DateTime asOfDate) : base(id)
        {
            MarketCurveId = marketCurveId;
            AsOfDate = asOfDate;
        }
    }
}
