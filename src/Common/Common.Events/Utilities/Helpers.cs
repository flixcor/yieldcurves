using System;
using System.Collections.Generic;
using Common.Core;
using NodaTime;

namespace Common.Events
{
    public static class Helpers
    {
        public static IInstrumentCreated InstrumentCreated(NonEmptyString vendor, NonEmptyString description, bool hasPriceType = false)
            => new InstrumentCreated(vendor, description, hasPriceType);

        public static IMarketCurveCreated MarketCurveCreated(NonEmptyString country, NonEmptyString curveType, NonEmptyString? floatingLeg = null)
            => new MarketCurveCreated(country, curveType, floatingLeg);

        public static IBloombergInstrumentCreated BloombergInstrumentCreated(NonEmptyString ticker, NonEmptyString pricingSource, NonEmptyString yellowKey)
            => new BloombergInstrumentCreated(ticker, pricingSource, yellowKey);

        public static ICurveCalculated CurveCalculated(NonEmptyGuid curveRecipeId, NonEmptyString asOfDate, IEnumerable<IPoint> points)
            => new CurveCalculated(curveRecipeId, asOfDate, points);

        public static IRegularInstrumentCreated RegularInstrumentCreated(NonEmptyString vendor, NonEmptyString description)
            => new RegularInstrumentCreated(vendor, description);

        public static ICurveCalculationFailed CurveCalculationFailed(NonEmptyGuid curveRecipeId, NonEmptyString asOfDate, string[] messages)
            => new CurveCalculationFailed(curveRecipeId, asOfDate, messages);

        public static ICurvePointAdded CurvePointAdded(NonEmptyString tenor, NonEmptyGuid instrumentId, short dateLag, bool isMandatory, NonEmptyString? priceType)
            => new CurvePointAdded(tenor, instrumentId, dateLag, isMandatory, priceType);

        public static ICurveRecipeCreated CurveRecipeCreated(NonEmptyGuid marketCurveId, NonEmptyString shortName, NonEmptyString description,
                                                             NonEmptyString lastLiquidTenor, NonEmptyString dayCountConvention,
                                                             NonEmptyString interpolation, NonEmptyString extrapolationShort,
                                                             NonEmptyString extrapolationLong, NonEmptyString outputSeries,
                                                             double maximumMaturity, NonEmptyString outputType)
                => new CurveRecipeCreated(marketCurveId, shortName, description, lastLiquidTenor, dayCountConvention,
                                          interpolation, extrapolationShort, extrapolationLong, outputSeries,
                                          maximumMaturity, outputType);

        public static IInstrumentPricingPublished InstrumentPricingPublished(NonEmptyString asOfDate,
                                                                             NonEmptyGuid instrumentId, NonEmptyString priceCurrency,
                                                                             double priceAmount, NonEmptyString? priceType = null)
            => new InstrumentPricingPublished(asOfDate, instrumentId, priceCurrency, priceAmount, priceType);

        public static IKeyRateShockAdded KeyRateShockAdded(int order, string shockTarget, double shift, double[] maturities)
            => new KeyRateShockAdded(order, shockTarget, shift, maturities);

        public static IParallelShockAdded ParallelShockAdded(int order, string shockTarget, double shift)
            => new ParallelShockAdded(order, shockTarget, shift);

        public static IPoint Point(double maturity, string currency, double value)
            => new Point(maturity, currency, value);

        public static IMetadata CreateMetadata(IDictionary<string,string> values) 
            => new Metadata(values);

        public static IEventWrapper Wrap(Guid aggregateId, Instant timestamp, int version, IEvent content, long id = 0) 
            => new EventWrapper(aggregateId, timestamp, version, content, id);

        public static IEventWrapper DeserializeEventWrapper(byte[] byteArray) => EventWrapper.Parser.ParseFrom(byteArray);

        public static IMetadata DeserializeMetadata(byte[] byteArray) => Metadata.Parser.ParseFrom(byteArray);
    }
}
