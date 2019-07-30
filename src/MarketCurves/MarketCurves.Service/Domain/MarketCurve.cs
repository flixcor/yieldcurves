using System;
using Common.Core;
using Common.Core.Events;

namespace MarketCurves.Domain
{
    public class MarketCurve : Aggregate<MarketCurve>
    {
        static MarketCurve()
        {
            RegisterApplyMethod<MarketCurveCreated>(Apply);
        }

        private MarketCurve()
        {
        }

        public MarketCurve(Guid id, Country country, CurveType curveType, FloatingLeg? floatingLeg = null)
        {
            var @event = new MarketCurveCreated(id, country.ToString(), curveType.ToString(), floatingLeg?.ToString());

            ApplyEvent(@event);
        }

        public void AddCurvePoint(Tenor tenor, Guid instrumentId, DateLag dateLag, PriceType? priceType, bool isMandatory)
        {
            var @event = new CurvePointAdded(Id, tenor.ToString(), instrumentId, dateLag.Value, isMandatory, priceType.ToString());
            ApplyEvent(@event);
        }

        private static void Apply(MarketCurve c, MarketCurveCreated e)
        {
            c.Id = e.Id;
        }
    }
}
