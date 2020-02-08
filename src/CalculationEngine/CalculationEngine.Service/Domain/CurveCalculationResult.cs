using System;
using System.Collections.Generic;
using System.Linq;
using CalculationEngine.Service.Domain;
using Common.Core;
using Common.EventStore.Lib;
using static Common.Events.Helpers;

namespace CalculationEngine.Domain
{
    public class CurveCalculationResult : Aggregate<CurveCalculationResult>
    {
        public CurveCalculationResult() { }

        public CurveCalculationResult WithResult(NonEmptyGuid recipeId, Date asOfDate, Result<IEnumerable<CurvePoint>> result)
        {
            var e = result.IsSuccessful
                ? (IEvent)CurveCalculated(recipeId, asOfDate.NonEmptyString(), result.Content.Select(x => Point(x.Maturity.Value, x.Price.Currency, x.Price.Value)))
                : CurveCalculationFailed(recipeId, asOfDate.NonEmptyString(), result.Messages.ToArray());

            GenerateEvent(e);

            return this;
        }

        protected override void When(IEvent @event)
        {
        }
    }
}
