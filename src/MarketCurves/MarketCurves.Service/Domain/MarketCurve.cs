using Common.Core;
using Common.EventStore.Lib;
using MarketCurves.Service.Domain;
using static Common.Events.Helpers;

namespace MarketCurves.Domain
{
    public class MarketCurve : Aggregate
    {
        public MarketCurve Define(Country country, CurveType curveType, FloatingLeg? floatingLeg = null)
        {
            var @event = MarketCurveCreated(country.NonEmptyString(), curveType.NonEmptyString(), floatingLeg?.NonEmptyString());
            GenerateEvent(@event);
            return this;
        }

        public Either<Error, MarketCurve> AddCurvePoint(Tenor tenor, Instrument instrument, DateLag dateLag, PriceType? priceType, bool isMandatory)
        {
            if (instrument.Vendor.HasPriceType() && !priceType.HasValue)
            {
                return new Error($"instrument {instrument.Id} needs a price type");
            }

            var @event = CurvePointAdded(tenor.NonEmptyString(), instrument.Id, dateLag.Value, isMandatory, priceType?.NonEmptyString());
            GenerateEvent(@event);

            return this;
        }

        protected override void When(IEvent @event)
        {
        }
    }
}
