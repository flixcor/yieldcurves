using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Events
{
    public static class Create
    {
        public static IInstrumentCreated InstrumentCreated(Guid aggregateId, string vendor, string description, bool hasPriceType = false) 
            => new InstrumentCreated(aggregateId, vendor, description, hasPriceType);

        public static IMarketCurveCreated MarketCurveCreated(Guid aggregateId, string country, string curveType, string floatingLeg = null)
            => new MarketCurveCreated(aggregateId, country, curveType, floatingLeg);

        public static IBloombergInstrumentCreated BloombergInstrumentCreated(Guid aggregateId, string ticker, string pricingSource, string yellowKey)
            => new BloombergInstrumentCreated(aggregateId, ticker, pricingSource, yellowKey);

        public static ICurveCalculated CurveCalculated(Guid aggregateId, Guid curveRecipeId, string asOfDate, DateTime asAtDate, IEnumerable<IPoint> points)
            => new CurveCalculated(aggregateId, curveRecipeId, asOfDate, asAtDate, points);

        public static IRegularInstrumentCreated RegularInstrumentCreated(Guid aggregateId, string vendor, string description)
            => new RegularInstrumentCreated(aggregateId, vendor, description);

        public static ICurveCalculationFailed CurveCalculationFailed(Guid aggregateId, Guid curveRecipeId, string asOfDate, DateTime asAtDate, string[] messages)
            => new CurveCalculationFailed(aggregateId, curveRecipeId, asOfDate, asAtDate, messages);

        public static ICurvePointAdded CurvePointAdded(Guid aggregateId, string tenor, Guid instrumentId, short dateLag, bool isMandatory, string priceType)
            => new CurvePointAdded(aggregateId, tenor, instrumentId, dateLag, isMandatory, priceType);

        public static ICurveRecipeCreated CurveRecipeCreated(Guid aggregateId, Guid marketCurveId, string shortName, string description, string lastLiquidTenor, string dayCountConvention, string interpolation,
            string extrapolationShort, string extrapolationLong, string outputSeries, double maximumMaturity, string outputType)
                => new CurveRecipeCreated(aggregateId, marketCurveId, shortName, description, lastLiquidTenor, dayCountConvention, interpolation,
                    extrapolationShort, extrapolationLong, outputSeries, maximumMaturity, outputType);

        public static IInstrumentPricingPublished InstrumentPricingPublished(Guid aggregateId, string asOfDate, DateTime asAtDate, Guid instrumentId, string priceCurrency, double priceAmount, string priceType = null)
            => new InstrumentPricingPublished(aggregateId, asOfDate, asAtDate, instrumentId, priceCurrency, priceAmount, priceType);

        public static IKeyRateShockAdded KeyRateShockAdded(Guid aggregateId, int order, string shockTarget, double shift, double[] maturities)
            => new KeyRateShockAdded(aggregateId, order, shockTarget, shift, maturities);

        public static IParallelShockAdded ParallelShockAdded(Guid aggregateId, int order, string shockTarget, double shift)
            => new ParallelShockAdded(aggregateId, order, shockTarget, shift);

        public static IPoint Point(double maturity, string currency, double value)
            => new Point(maturity, currency, value);
    }
}
