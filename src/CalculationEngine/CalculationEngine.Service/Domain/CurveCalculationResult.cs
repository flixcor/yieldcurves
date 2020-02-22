using System.Collections.Generic;
using System.Linq;
using CalculationEngine.Service.Domain;
using Common.Core;
using Common.EventStore.Lib;
using static Common.Events.Helpers;

namespace CalculationEngine.Domain
{
    public class CurveCalculationResult : Aggregate
    {
        public CurveCalculationResult() { }

        public CurveCalculationResult WithResult(NonEmptyGuid recipeId, Date asOfDate, Result<IEnumerable<CurvePoint>> result)
        {
            var e = result.ToEither()
                .MapLeft(e => (IEvent)CurveCalculationFailed(recipeId, asOfDate.NonEmptyString(), e.Messages))
                .Reduce(r => CurveCalculated(recipeId, asOfDate.NonEmptyString(), r.Select(x => Point(x.Maturity.Value, x.Price.Currency, x.Price.Value))));

            GenerateEvent(e);

            return this;
        }

        protected override void When(IEvent @event)
        {
        }
    }
}
