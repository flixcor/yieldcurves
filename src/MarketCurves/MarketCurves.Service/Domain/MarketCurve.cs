using System;
using System.Collections.Generic;
using System.Linq;
using Common.Core;
using Common.Events;
using static Common.Events.Create;

namespace MarketCurves.Domain
{
    public class MarketCurve : Aggregate<MarketCurve>
    {
        static MarketCurve()
        {
            RegisterApplyMethod<IMarketCurveCreated>(Apply);
        }

        private MarketCurve()
        {
        }

        public static Result<MarketCurve> TryCreate(Guid id, Country country, CurveType curveType, FloatingLeg? floatingLeg = null)
        {
            var errors = new List<string>();

            if (id.Equals(Guid.Empty))
            {
                errors.Add($"{nameof(id)} cannot be empty");
            }

            if (errors.Any())
            {
                return Result.Fail<MarketCurve>(errors.ToArray());
            }

            return Result.Ok(new MarketCurve(id, country, curveType, floatingLeg));
        }

        private MarketCurve(Guid id, Country country, CurveType curveType, FloatingLeg? floatingLeg = null)
        {
            var @event = MarketCurveCreated(id, country.ToString(), curveType.ToString(), floatingLeg?.ToString());

            ApplyEvent(@event);
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

            var @event = CurvePointAdded(Id, tenor.ToString(), instrumentId, dateLag.Value, isMandatory, priceType.ToString());
            ApplyEvent(@event);

            return Result.Ok();
        }

        private static void Apply(MarketCurve c, IMarketCurveCreated e)
        {
            c.Id = e.AggregateId;
        }
    }
}
