using System;
using System.Collections.Generic;
using System.Text;
using Common.Core;
using Newtonsoft.Json;

namespace CalculationEngine.Domain
{
    public class CurveCalculationFailed : Event
    {
        [JsonConstructor]
        private CurveCalculationFailed(Guid id, Guid curveRecipeId, DateTime asOfDate, DateTime asAtDate, string[] messages, int version) : this(id, curveRecipeId, asOfDate, asAtDate, messages)
        {
            Version = version;
        }

        public CurveCalculationFailed(Guid id, Guid curveRecipeId, DateTime asOfDate, DateTime asAtDate, params string[] messages) : base(id)
        {
            CurveRecipeId = curveRecipeId;
            AsOfDate = asOfDate;
            AsAtDate = asAtDate;
            Messages = messages;
        }

        public Guid CurveRecipeId { get; }
        public DateTime AsOfDate { get; }
        public DateTime AsAtDate { get; }
        public string[] Messages { get; }
    }
}
