using System;
using Common.Core;

namespace Common.Events
{
    public class CurveCalculationInitialized : IEvent
    {
        public CurveCalculationInitialized(Guid aggregateId, Guid recipeId, DateTime asOfDate, int version = 0)
        {
            AggregateId = aggregateId;
            Version = version;
            RecipeId = recipeId;
            AsOfDate = asOfDate;
        }

        public Guid RecipeId { get; }
        public DateTime AsOfDate { get; }

        public Guid AggregateId { get; }
        public int Version { get; private set; }
		
		public IEvent WithVersion(int version)
		{
			var clone = (dynamic)MemberwiseClone();
			clone.Version = version;
			return clone;
		}
    }
}
