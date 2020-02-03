using System;
using System.Collections.Generic;
using Common.Core;
using NodaTime;

namespace Common.Events
{
    public static class Create
    {
        public static IInstrumentCreated InstrumentCreated(string vendor, string description, bool hasPriceType = false)
            => new InstrumentCreated(vendor, description, hasPriceType);

        public static IMarketCurveCreated MarketCurveCreated(string country, string curveType, string? floatingLeg = null)
            => new MarketCurveCreated(country, curveType, floatingLeg);

        public static IBloombergInstrumentCreated BloombergInstrumentCreated(string ticker, string pricingSource, string yellowKey)
            => new BloombergInstrumentCreated(ticker, pricingSource, yellowKey);

        public static ICurveCalculated CurveCalculated(Guid curveRecipeId, string asOfDate, IEnumerable<IPoint> points)
            => new CurveCalculated(curveRecipeId, asOfDate, points);

        public static IRegularInstrumentCreated RegularInstrumentCreated(string vendor, string description)
            => new RegularInstrumentCreated(vendor, description);

        public static ICurveCalculationFailed CurveCalculationFailed(Guid curveRecipeId, string asOfDate, string[] messages)
            => new CurveCalculationFailed(curveRecipeId, asOfDate, messages);

        public static ICurvePointAdded CurvePointAdded(string tenor, Guid instrumentId, short dateLag, bool isMandatory, string priceType)
            => new CurvePointAdded(tenor, instrumentId, dateLag, isMandatory, priceType);

        public static ICurveRecipeCreated CurveRecipeCreated(Guid marketCurveId, string shortName, string description,
                                                             string lastLiquidTenor, string dayCountConvention,
                                                             string interpolation, string extrapolationShort,
                                                             string extrapolationLong, string outputSeries,
                                                             double maximumMaturity, string outputType)
                => new CurveRecipeCreated(marketCurveId, shortName, description, lastLiquidTenor, dayCountConvention,
                                          interpolation, extrapolationShort, extrapolationLong, outputSeries,
                                          maximumMaturity, outputType);

        public static IInstrumentPricingPublished InstrumentPricingPublished(string asOfDate, DateTime asAtDate,
                                                                             Guid instrumentId, string priceCurrency,
                                                                             double priceAmount, string? priceType = null)
            => new InstrumentPricingPublished(asOfDate, asAtDate, instrumentId, priceCurrency, priceAmount, priceType);

        public static IKeyRateShockAdded KeyRateShockAdded(int order, string shockTarget, double shift, double[] maturities)
            => new KeyRateShockAdded(order, shockTarget, shift, maturities);

        public static IParallelShockAdded ParallelShockAdded(int order, string shockTarget, double shift)
            => new ParallelShockAdded(order, shockTarget, shift);

        public static IPoint Point(double maturity, string currency, double value)
            => new Point(maturity, currency, value);

        public static IEventWrapperMetadata Metadata(long id, Guid aggregateId, int version, Instant timestamp) 
            => new Metadata(id, timestamp, aggregateId, version);
    }
}
