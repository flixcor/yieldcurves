using System.Collections.Generic;
using System.Linq;
using CalculationEngine.Domain;
using Common.Core;
using Common.Events;

namespace CalculationEngine.Service.Domain
{
    public static class CurveCalculation
    {
        public static Result<IEnumerable<CurvePoint>> Calculate(Date asOfDate, IEventWrapper<ICurveRecipeCreated> recipe, IEnumerable<ICurvePointAdded> marketCurve, IEnumerable<IEventWrapper<IInstrumentPricingPublished>> pricings)
        {
            var matchingPricesResult = TryGetAllMatchingPrices(asOfDate, marketCurve, pricings);
            var recipeResult = TryMap(recipe);

            return Result.Combine(matchingPricesResult, recipeResult, (p, r) => r.ApplyTo(p));
        }

        public static Result<IEnumerable<CurvePoint>> TryGetAllMatchingPrices(Date asOfDate, IEnumerable<ICurvePointAdded> marketCurve, IEnumerable<IEventWrapper<IInstrumentPricingPublished>> pricings)
        {
            var pricingResult = pricings.Select(TryMap).Convert();
            var pointResult = marketCurve.Select(TryMap).Convert();

            return Result.Combine(pricingResult, pointResult, (pr, p) => GetPointsFromPricings(asOfDate, p, pr));
        }

        private static Result<IEnumerable<CurvePoint>> GetPointsFromPricings(Date asOfDate, IEnumerable<PointRecipe> recipes, IEnumerable<PublishedPricing> pricings) =>
            recipes
                .Select(x => x.GetPoint(pricings, asOfDate))
                .Convert();

        private static Result<PointRecipe> TryMap(ICurvePointAdded e)
        {
            var priceTypeResult = e.PriceType.TryParseOptionalEnum<PriceType>();
            var tenorResult = e.Tenor.TryParseEnum<Tenor>();

            return Result
                .Combine(priceTypeResult, tenorResult, (p, t) =>
                {
                    var dateLag = new DateLag(e.DateLag);
                    return new PointRecipe(e.InstrumentId, t, dateLag, p);
                });
        }

        private static Result<PublishedPricing> TryMap(IEventWrapper<IInstrumentPricingPublished> wrapper)
        {
            var e = wrapper.Content;

            var price = new Price(e.PriceCurrency, e.PriceAmount);
            var asOfDate = Date.FromString(e.AsOfDate);
            var priceTypeResult = e.PriceType.TryParseOptionalEnum<PriceType>();

            return priceTypeResult
                .Promise(priceType => new PublishedPricing(asOfDate, wrapper.Timestamp, e.InstrumentId, price, priceType));
        }

        private static Result<CurveRecipe> TryMap(IEventWrapper<ICurveRecipeCreated> wrapper)
        {
            var e = wrapper.Content;

            var lastLiquidTenorR = e.LastLiquidTenor.TryParseEnum<Tenor>();
            var dccR = e.DayCountConvention.TryParseEnum<DayCountConvention>();
            var interR = e.Interpolation.TryParseEnum<Interpolation>();
            var exShortR = e.ExtrapolationShort.TryParseEnum<ExtrapolationShort>();
            var exLongR = e.ExtrapolationLong.TryParseEnum<ExtrapolationLong>();
            var outSeriesR = e.OutputSeries.TryParseEnum<OutputSeries>();
            var outTypeR = e.OutputType.TryParseEnum<OutputType>();

            return Result.Combine(lastLiquidTenorR, dccR, interR, exShortR, exLongR, outSeriesR, outTypeR, 
                (lastLiquidTenor, dcc, inter, exShort, exLong, outSeries, outType) 
                    => new CurveRecipe(wrapper.AggregateId, lastLiquidTenor, dcc, inter, exShort, exLong, new OutputFrequency(outSeries, new Maturity(e.MaximumMaturity)), outType));
        }
    }
}
