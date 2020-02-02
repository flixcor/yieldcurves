using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Common.Core;
using Common.Events;

namespace CalculationEngine.Service.ActorModel.Commands
{
    public class SendMeCalculate
    {
        public SendMeCalculate(ICollection<IEventWrapper<ICurvePointAdded>> curvePoints)
        {
        CurvePoints = curvePoints?.ToImmutableArray() ?? throw new ArgumentNullException(nameof(curvePoints));
        }

        public ImmutableArray<IEventWrapper<ICurvePointAdded>> CurvePoints { get; }
    }
}
