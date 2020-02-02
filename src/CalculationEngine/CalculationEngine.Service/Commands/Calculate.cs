using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using CalculationEngine.Service.Domain;
using Common.Core;
using Common.Events;

namespace CalculationEngine.Service.ActorModel.Commands
{
    public class Calculate
    {
        public Calculate(Date asOfDate, ICollection<IEventWrapper<ICurvePointAdded>> curvePoints, ICollection<IEventWrapper<IInstrumentPricingPublished>> pricings)
        {
            AsOfDate = asOfDate;
            CurvePoints = curvePoints?.ToImmutableArray() ?? throw new ArgumentNullException(nameof(curvePoints));
            Pricings = pricings?.ToImmutableArray() ?? throw new ArgumentNullException(nameof(pricings));
        }

        public Date AsOfDate { get; }
        public ImmutableArray<IEventWrapper<ICurvePointAdded>> CurvePoints { get; }
        public ImmutableArray<IEventWrapper<IInstrumentPricingPublished>> Pricings { get; }
    }
}
