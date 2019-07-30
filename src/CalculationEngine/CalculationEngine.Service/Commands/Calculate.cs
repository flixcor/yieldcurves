using System;
using System.Collections.Generic;
using Common.Core.Events;

namespace CalculationEngine.Service.ActorModel.Commands
{
    public class Calculate
    {
        public Calculate(DateTime asOfDate, IEnumerable<CurvePointAdded> curvePoints, IEnumerable<InstrumentPricingPublished> pricings)
        {
            AsOfDate = asOfDate;
            CurvePoints = curvePoints ?? throw new ArgumentNullException(nameof(curvePoints));
            Pricings = pricings ?? throw new ArgumentNullException(nameof(pricings));
        }

        public DateTime AsOfDate { get; }
        public IEnumerable<CurvePointAdded> CurvePoints { get; }
        public IEnumerable<InstrumentPricingPublished> Pricings { get; }
    }
}
