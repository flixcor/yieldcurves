using System;
using Common.Core;

namespace Common.Events
{
    public class CurveCalculationInitialized : IEvent
    {
        public CurveCalculationInitialized(Guid id, Guid recipeId, DateTime asOfDate, int version = 0)
        {
            Id = id;
            RecipeId = recipeId;
            AsOfDate = asOfDate;
            Version = version;
        }

        public Guid Id { get; }
        public int Version { get; }
        public Guid RecipeId { get; }
        public DateTime AsOfDate { get; }

    }
}
