using System;
using System.Collections.Generic;
using Common.Core.Events;

namespace CalculationEngine.Service.ActorModel.Commands
{
    public class SendMeCalculate
    {
        public SendMeCalculate(IEnumerable<CurvePointAdded> curvePoints)
        {
            CurvePoints = curvePoints ?? throw new ArgumentNullException(nameof(curvePoints));
        }

        public IEnumerable<CurvePointAdded> CurvePoints { get; }
    }
}
