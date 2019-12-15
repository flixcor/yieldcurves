using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using CalculationEngine.Service.Domain;
using Common.Events;

namespace CalculationEngine.Service.ActorModel.Commands
{
    public class Calculate
    {
        public Calculate(Date asOfDate, ICollection<ICurvePointAdded> curvePoints, ICollection<IInstrumentPricingPublished> pricings)
        {
            AsOfDate = asOfDate;
            CurvePoints = curvePoints?.ToImmutableArray() ?? throw new ArgumentNullException(nameof(curvePoints));
            Pricings = pricings?.ToImmutableArray() ?? throw new ArgumentNullException(nameof(pricings));
        }

        public Date AsOfDate { get; }
        public ImmutableArray<ICurvePointAdded> CurvePoints { get; }
        public ImmutableArray<IInstrumentPricingPublished> Pricings { get; }
    }
}
