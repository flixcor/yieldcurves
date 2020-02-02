using System;
using System.Collections.Generic;
using Common.Core;

namespace Common.Events
{
    public interface ICurveCalculationFailed : IEvent
    {
        string AsOfDate { get; }
        string CurveRecipeId { get; }
        IEnumerable<string> Messages { get; }
    }

    internal partial class CurveCalculationFailed : ICurveCalculationFailed
    {
        public CurveCalculationFailed(Guid curveRecipeId, string asOfDate, string[] messages)
        {
            CurveRecipeId = curveRecipeId.ToString();
            AsOfDate = asOfDate;
            Messages.Add(messages);
        }

        IEnumerable<string> ICurveCalculationFailed.Messages => Messages;
    }
}
