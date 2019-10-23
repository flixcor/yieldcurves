using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CalculationEngine.Domain;
using Common.Core;
using Common.Events;

namespace CalculationEngine.Service.Domain
{
    public static class CurveCalculation
    {
        public static Result<ImmutableArray<CurvePoint>> Calculate(DateTime asOfDate, CurveRecipeCreated recipe, ICollection<CurvePointAdded> marketCurve, ICollection<InstrumentPricingPublished> pricings)
        {
            var matchingPricesResult = TryGetAllMatchingPrices(asOfDate, marketCurve, pricings);
            var recipeResult = TryMap(recipe);

            return Result.Combine(matchingPricesResult, recipeResult, (p, r) => r.ApplyTo(p));
        }

        public static Result<ImmutableArray<CurvePoint>> TryGetAllMatchingPrices(DateTime asOfDate, ICollection<CurvePointAdded> marketCurve, ICollection<InstrumentPricingPublished> pricings)
        {
            var pricingResult = pricings.Select(TryMap).Convert();
            var pointResult = marketCurve.Select(TryMap).Convert();
            var date = Date.FromDateTime(asOfDate);

            return Result.Combine(pricingResult, pointResult, (pr, p) => GetPointsFromPricings(date, p, pr));
        }

        private static Result<ImmutableArray<CurvePoint>> GetPointsFromPricings(Date asOfDate, ICollection<PointRecipe> recipes, ICollection<PublishedPricing> pricings) => 
            recipes
                .Select(x => x.GetPoint(pricings, asOfDate))
                .Convert();

        private static Result<PointRecipe> TryMap(CurvePointAdded e)
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
                .Promise(() => new CurveRecipe(e.AggregateId, lastLiquidTenor.Content, dcc.Content, inter.Content, exShort.Content, exLong.Content, new OutputFrequency(outSeries.Content, new Maturity(e.MaximumMaturity)), outType.Content));
        }
    }
}
