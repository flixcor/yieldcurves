using System;
using System.Collections.Generic;
using System.Linq;
using CalculationEngine.Domain;
using Common.Core;
using Common.Core.Events;

namespace CalculationEngine.Service.Domain
{
    public static class CurveCalculation
    {
        public static Result<IEnumerable<CurvePoint>> Calculate(DateTime asOfDate, CurveRecipeCreated recipe, IEnumerable<CurvePointAdded> marketCurve, IEnumerable<InstrumentPricingPublished> pricings)
        {
            var matchingPricesResult = TryGetAllMatchingPrices(asOfDate, marketCurve, pricings);
            var recipeResult = TryMap(recipe);

            return Result.Combine(matchingPricesResult, recipeResult)
                .Promise(()=> recipeResult.Content.ApplyTo(matchingPricesResult.Content));
        }

        public static Result<IEnumerable<CurvePoint>> TryGetAllMatchingPrices(DateTime asOfDate, IEnumerable<CurvePointAdded> marketCurve, IEnumerable<InstrumentPricingPublished> pricings)
        {
            var pricingResult = pricings.Select(TryMap).Convert();
            var pointResult = marketCurve.Select(TryMap).Convert();
            var date = Date.FromDateTime(asOfDate);

            return Result.Combine(pricingResult, pointResult)
                .Promise(() => GetPointsFromPricings(date, pointResult.Content, pricingResult.Content));
        }

        private static Result<IEnumerable<CurvePoint>> GetPointsFromPricings(Date asOfDate, IEnumerable<PointRecipe> recipes, IEnumerable<PublishedPricing> pricings)
        {
            return recipes.Select(x => x.GetPoint(pricings, asOfDate)).Convert();
        }

        private static Result<PointRecipe> TryMap(CurvePointAdded e)
        {
            var priceTypeResult = e.PriceType.TryParseOptionalEnum<PriceType>();
            var tenorResult = e.Tenor.TryParseEnum<Tenor>();

            return Result
                .Combine(priceTypeResult, tenorResult)
                .Promise(() =>
                {
                    var dateLag = new DateLag(e.DateLag);
                    return new PointRecipe(e.InstrumentId, tenorResult.Content, dateLag, priceTypeResult.Content);
                });
        }

        private static Result<PublishedPricing> TryMap(InstrumentPricingPublished e)
        {
            var price = new Price(e.PriceCurrency, e.PriceAmount);
            var asOfDate = Date.FromDateTime(e.AsOfDate);
            var priceTypeResult = e.PriceType.TryParseOptionalEnum<PriceType>();

            return priceTypeResult
                .Promise(() => new PublishedPricing(asOfDate, e.AsAtDate, e.InstrumentId, price, priceTypeResult.Content));
        }

        private static Result<CurveRecipe> TryMap(CurveRecipeCreated e)
        {
            var lastLiquidTenor = e.LastLiquidTenor.TryParseEnum<Tenor>();
            var dcc = e.DayCountConvention.TryParseEnum<DayCountConvention>();
            var inter = e.Interpolation.TryParseEnum<Interpolation>();
            var exShort = e.ExtrapolationShort.TryParseEnum<ExtrapolationShort>();
            var exLong = e.ExtrapolationLong.TryParseEnum<ExtrapolationLong>();
            var outSeries = e.OutputSeries.TryParseEnum<OutputSeries>();
            var outType = e.OutputType.TryParseEnum<OutputType>();

            return Result
                .Combine(lastLiquidTenor, dcc, inter, exShort, exLong, outSeries, outType)
                .Promise(() => new CurveRecipe(e.Id, lastLiquidTenor.Content, dcc.Content, inter.Content, exShort.Content, exLong.Content, new OutputFrequency(outSeries.Content, new Maturity(e.MaximumMaturity)), outType.Content));
        }
    }
}
