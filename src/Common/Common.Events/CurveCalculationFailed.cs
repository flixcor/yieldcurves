using System;
using System.Collections.Generic;
using Common.Core;

namespace Common.Events
{
    public class CurveCalculationFailed : Event
    {
        public CurveCalculationFailed(Guid id, Guid curveRecipeId, DateTime asOfDate, DateTime asAtDate, string[] messages) : base(id)
        {
            CurveRecipeId = curveRecipeId;
            AsOfDate = asOfDate;
            AsAtDate = asAtDate;
            Messages = messages;
        }


        public Guid CurveRecipeId { get; }
        public DateTime AsOfDate { get; }
        public DateTime AsAtDate { get; }
        public ICollection<string> Messages { get; }
    }
}
