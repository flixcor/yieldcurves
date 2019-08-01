﻿using System;
using System.Collections.Generic;
using System.Linq;
using Common.Core;

namespace CalculationEngine.Domain
{
    public class CurveCalculationResult : Aggregate<CurveCalculationResult>
    {
        static CurveCalculationResult()
        {
            RegisterApplyMethod<CurveCalculated>(Apply);
            RegisterApplyMethod<CurveCalculationFailed>(Apply);
        }

        private static void Apply(CurveCalculationResult r, CurveCalculationFailed e)
        {
            r.Id = e.Id;
        }

        private static void Apply(CurveCalculationResult r, CurveCalculated e)
        {
            r.Id = e.Id;
        }

        public CurveCalculationResult(Guid id, Guid recipeId, DateTime asOfDate, Result<IEnumerable<CurvePoint>> result)
        {
            var e = result.IsSuccessful
                ? (Event)new CurveCalculated(id, recipeId, asOfDate, DateTime.Now, result.Content.Select(x => new CurveCalculated.Point(x.Maturity.Value, x.Price.Currency, x.Price.Value)))
                : new CurveCalculationFailed(id, recipeId, asOfDate, DateTime.Now, result.Messages);

            ApplyEvent(e);
        }
    }
}