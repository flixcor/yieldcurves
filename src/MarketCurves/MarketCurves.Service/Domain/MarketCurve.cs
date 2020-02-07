using System;
using System.Collections.Generic;
using System.Linq;
using Common.Core;
using Common.EventStore.Lib;
using static Common.Events.Helpers;

namespace MarketCurves.Domain
{
    public class MarketCurve : Aggregate<MarketCurve>
    {
        private MarketCurve()
        {
        }

        public static Result<MarketCurve> TryCreate(Country country, CurveType curveType, FloatingLeg? floatingLeg = null)
        {
            var errors = new List<string>();

            if (errors.Any())
            {
                return Result.Fail<MarketCurve>(errors.ToArray());
            }

            return Result.Ok(new MarketCurve(country, curveType, floatingLeg));
        }

        private MarketCurve(Country country, CurveType curveType, FloatingLeg? floatingLeg = null)
        {
            var @event = MarketCurveCreated(country.ToString(), curveType.ToString(), floatingLeg?.ToString());

            GenerateEvent(@event);
        }

        public Result AddCurvePoint(Tenor tenor, Guid instrumentId, DateLag dateLag, PriceType? priceType, bool isMandatory)
        {
            var errors = new List<string>();

            if (instrumentId.Equals(Guid.Empty))
            {
                errors.Add($"{nameof(instrumentId)} cannot be empty");
            }

            if (errors.Any())
            {
                return Result.Fail(errors.ToArray());
            }

            var @event = CurvePointAdded(tenor.ToString(), instrumentId, dateLag.Value, isMandatory, priceType.ToString());
            GenerateEvent(@event);

            return Result.Ok();
        }

        protected override void When(IEvent @event)
        {
        }
    }
}
