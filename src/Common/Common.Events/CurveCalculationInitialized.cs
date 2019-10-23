using System;
using Common.Core;

namespace Common.Events
{
    public class CurveCalculationInitialized : Event
    {
        public CurveCalculationInitialized(Guid id, Guid recipeId, DateTime asOfDate) : base(id)
        {
            RecipeId = recipeId;
            AsOfDate = asOfDate;
        }

        public Guid RecipeId { get; }
        public DateTime AsOfDate { get; }

    }
}
