using System.Collections.Generic;
using System.Linq;
using CalculationEngine.Domain;
using Common.Core;
using Common.Events;

namespace CalculationEngine.Service.Domain
{
    public static class CurveCalculation
    {
        public static Either<Error, IEnumerable<CurvePoint>> Calculate(Date asOfDate, IEventWrapper<ICurveRecipeCreated> recipe, IEnumerable<ICurvePointAdded> marketCurve, IEnumerable<IEventWrapper<IInstrumentPricingPublished>> pricings)
        {
            var matchingPricesResult = TryGetAllMatchingPrices(asOfDate, marketCurve, pricings);
            var recipeResult = TryMap(recipe);

            return matchingPricesResult.MapRight(recipeResult, (p, r) => r.ApplyTo(p));
        }

        public static Either<Error, ICollection<CurvePoint>> TryGetAllMatchingPrices(Date asOfDate, IEnumerable<ICurvePointAdded> marketCurve, IEnumerable<IEventWrapper<IInstrumentPricingPublished>> pricings)
        {
            var pricingResult = pricings.Select(TryMap).Flatten();
            var pointResult = marketCurve.Select(TryMap).Flatten();

            return pricingResult
                .MapRight(pointResult, (pr, p) => GetPointsFromPricings(asOfDate, p, pr))
                .Flatten();
        }

        private static Either<Error, ICollection<CurvePoint>> GetPointsFromPricings(Date asOfDate, IEnumerable<PointRecipe> recipes, IEnumerable<PublishedPricing> pricings) =>
            recipes
                .Select(x => x.GetPoint(pricings, asOfDate))
                .Flatten();

        private static Either<Error, PointRecipe> TryMap(ICurvePointAdded e)
        {
            var priceTypeResult = e.PriceType.TryParseOptionalEnum<PriceType>();
            var tenorResult = e.Tenor.TryParseEnum<Tenor>();

            return priceTypeResult.MapRight(tenorResult, (p, t) =>
                {
                    var dateLag = new DateLag(e.DateLag);
                    return new PointRecipe(e.InstrumentId, t, dateLag, p);
                });
        }

        private static Either<Error, PublishedPricing> TryMap(IEventWrapper<IInstrumentPricingPublished> wrapper)
        {
            var e = wrapper.Content;

            var price = new Price(e.PriceCurrency, e.PriceAmount);
            var asOfDate = Date.FromString(e.AsOfDate);
            var priceTypeResult = e.PriceType.TryParseOptionalEnum<PriceType>();

            return priceTypeResult
                .MapRight(priceType => new PublishedPricing(asOfDate, wrapper.Timestamp, e.InstrumentId, price, priceType));
        }

        private static Either<Error, CurveRecipe> TryMap(IEventWrapper<ICurveRecipeCreated> wrapper)
        {
            var e = wrapper.Content;

            var lastLiquidTenorR = e.LastLiquidTenor.TryParseEnum<Tenor>();
            var dccR = e.DayCountConvention.TryParseEnum<DayCountConvention>();
            var interR = e.Interpolation.TryParseEnum<Interpolation>();
            var exShortR = e.ExtrapolationShort.TryParseEnum<ExtrapolationShort>();
            var exLongR = e.ExtrapolationLong.TryParseEnum<ExtrapolationLong>();
            var outSeriesR = e.OutputSeries.TryParseEnum<OutputSeries>();
            var outTypeR = e.OutputType.TryParseEnum<OutputType>();

            return lastLiquidTenorR.MapRight(dccR, interR, exShortR, exLongR, outSeriesR, outTypeR, 
                (lastLiquidTenor, dcc, inter, exShort, exLong, outSeries, outType) 
                    => new CurveRecipe(wrapper.AggregateId, lastLiquidTenor, dcc, inter, exShort, exLong, new OutputFrequency(outSeries, new Maturity(e.MaximumMaturity)), outType));
        }
    }
}
