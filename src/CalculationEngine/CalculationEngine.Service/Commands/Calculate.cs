using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Common.Core.Events;

namespace CalculationEngine.Service.ActorModel.Commands
{
    public class Calculate
    {
        public Calculate(DateTime asOfDate, ICollection<CurvePointAdded> curvePoints, ICollection<InstrumentPricingPublished> pricings)
        {
            AsOfDate = asOfDate;
            CurvePoints = curvePoints?.ToImmutableArray() ?? throw new ArgumentNullException(nameof(curvePoints));
            Pricings = pricings?.ToImmutableArray() ?? throw new ArgumentNullException(nameof(pricings));
        }

        public DateTime AsOfDate { get; }
        public ImmutableArray<CurvePointAdded> CurvePoints { get; }
        public ImmutableArray<InstrumentPricingPublished> Pricings { get; }
    }
}
