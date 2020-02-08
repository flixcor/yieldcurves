using Common.Core;
using Common.EventStore.Lib;
using MarketCurves.Service.Domain;
using static Common.Core.Result;
using static Common.Events.Helpers;

namespace MarketCurves.Domain
{
    public class MarketCurve : Aggregate<MarketCurve>
    {
        public MarketCurve()
        {

        }

        public MarketCurve Define(Country country, CurveType curveType, FloatingLeg? floatingLeg = null)
        {
            var @event = MarketCurveCreated(country.ToString(), curveType.ToString(), floatingLeg?.ToString());
            GenerateEvent(@event);
            return this;
        }

        public Result<MarketCurve> AddCurvePoint(Tenor tenor, Instrument instrument, DateLag dateLag, PriceType? priceType, bool isMandatory)
        {
            if (instrument.Vendor.HasPriceType() && !priceType.HasValue)
            {
                return Fail<MarketCurve>($"instrument {instrument.Id} needs a price type");
            }

            var @event = CurvePointAdded(tenor.ToString(), instrument.Id, dateLag.Value, isMandatory, priceType?.ToString());
            GenerateEvent(@event);

            return Ok(this);
        }

        protected override void When(IEvent @event)
        {
        }
    }
}
