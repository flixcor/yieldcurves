using System;
using System.Collections.Generic;
using Common.Core;

namespace Common.Events
{
    public class CurveCalculationFailed : IEvent
    {
        public CurveCalculationFailed(Guid aggregateId, Guid curveRecipeId, DateTime asOfDate, DateTime asAtDate, string[] messages, int version = 0)
        {
            AggregateId = aggregateId;
            Version = version;
            CurveRecipeId = curveRecipeId;
            AsOfDate = asOfDate;
            AsAtDate = asAtDate;
            Messages = messages;
        }


        public Guid CurveRecipeId { get; }
        public DateTime AsOfDate { get; }
        public DateTime AsAtDate { get; }
        public ICollection<string> Messages { get; }
        public Guid AggregateId { get; }
        public int Version { get; }
		
		public IEvent WithVersion(int version)
		{
			throw new NotImplementedException();
		}
    }
}
