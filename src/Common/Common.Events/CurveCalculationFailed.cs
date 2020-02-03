using System;
using System.Collections.Generic;
using Common.Core;

namespace Common.Events
{
    public interface ICurveCalculationFailed : IEvent
    {
        string AsOfDate { get; }
        Guid CurveRecipeId { get; }
        IEnumerable<string> Messages { get; }
    }

    internal partial class CurveCalculationFailed : ICurveCalculationFailed
    {
        public CurveCalculationFailed(Guid curveRecipeId, string asOfDate, string[] messages)
        {
            CurveRecipeId = curveRecipeId;
            AsOfDate = asOfDate;
            Messages.Add(messages);
        }

        Guid ICurveCalculationFailed.CurveRecipeId => CurveRecipeId;
        IEnumerable<string> ICurveCalculationFailed.Messages => Messages;
    }
}
