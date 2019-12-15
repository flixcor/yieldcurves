using System;
using System.Collections.Generic;
using System.Linq;
using CalculationEngine.Service.Domain;
using Common.Core;
using Common.Events;
using static Common.Events.Create;

namespace CalculationEngine.Domain
{
    public class CurveCalculationResult : Aggregate<CurveCalculationResult>
    {
        static CurveCalculationResult()
        {
            RegisterApplyMethod<ICurveCalculated>(Apply);
            RegisterApplyMethod<ICurveCalculationFailed>(Apply);
        }

        private static void Apply(CurveCalculationResult r, ICurveCalculationFailed e)
        {
            r.Id = e.AggregateId;
        }

        private static void Apply(CurveCalculationResult r, ICurveCalculated e)
        {
            r.Id = e.AggregateId;
        }

        public CurveCalculationResult(Guid id, Guid recipeId, Date asOfDate, Result<IEnumerable<CurvePoint>> result)
        {
            var e = result.IsSuccessful
                ? (IEvent)CurveCalculated(id, recipeId, asOfDate.ToString(), DateTime.UtcNow, result.Content.Select(x => Point(x.Maturity.Value, x.Price.Currency, x.Price.Value)))
                : CurveCalculationFailed(id, recipeId, asOfDate.ToString(), DateTime.UtcNow, result.Messages.ToArray());

            ApplyEvent(e);
        }
    }
}
