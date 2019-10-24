using System;
using Common.Core;

namespace Common.Events
{
    public class CurveCalculationInitialized : IEvent
    {
        public CurveCalculationInitialized(Guid aggregateId, Guid recipeId, DateTime asOfDate)
        {
            AggregateId = aggregateId;
            RecipeId = recipeId;
            AsOfDate = asOfDate;
        }

        public Guid RecipeId { get; }
        public DateTime AsOfDate { get; }

        public Guid AggregateId { get; }
        public int Version { get; private set; }
		
		public IEvent WithVersion(int version)
		{
			var clone = (CurveCalculationInitialized)MemberwiseClone();
			clone.Version = version;
			return clone;
		}
    }
}
