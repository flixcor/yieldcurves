using System;
using System.Collections.Generic;
using Common.Core;

namespace Common.Events
{
    public class CurveCalculationFailed : IEvent
    {
        public CurveCalculationFailed(Guid id, Guid curveRecipeId, DateTime asOfDate, DateTime asAtDate, string[] messages, int version = 0)
        {
            Id = id;
            CurveRecipeId = curveRecipeId;
            AsOfDate = asOfDate;
            AsAtDate = asAtDate;
            Version = version;
            Messages = messages;
        }

        public Guid Id { get; }
        public int Version { get; }

        public Guid CurveRecipeId { get; }
        public DateTime AsOfDate { get; }
        public DateTime AsAtDate { get; }
        public ICollection<string> Messages { get; }
    }
}
