using System;

namespace Common.Core.Events
{
    public class CurveCalculationInitialized : Event
    {
        private CurveCalculationInitialized(Guid id, Guid recipeId, DateTime asOfDate, int version) : this(id, recipeId, asOfDate)
        {
            Version = version;
        }

        public CurveCalculationInitialized(Guid id, Guid recipeId, DateTime asOfDate) : base(id)
        {
            RecipeId = recipeId;
            AsOfDate = asOfDate;
        }

        public Guid RecipeId { get; }
        public DateTime AsOfDate { get; }
    }
}
