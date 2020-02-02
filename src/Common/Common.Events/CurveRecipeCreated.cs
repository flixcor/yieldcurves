using System;
using Common.Core;

namespace Common.Events
{
    public interface ICurveRecipeCreated : IEvent
    {
        string ShortName { get; }
        string DayCountConvention { get; }
        string Description { get; }
        string ExtrapolationLong { get; }
        string ExtrapolationShort { get; }
        string Interpolation { get; }
        string LastLiquidTenor { get; }
        Guid MarketCurveId { get; }
        double MaximumMaturity { get; }
        string OutputSeries { get; }
        string OutputType { get; }
    }

    internal partial class CurveRecipeCreated : ICurveRecipeCreated
    {
        public CurveRecipeCreated(Guid marketCurveId, string shortName, string description, string lastLiquidTenor, string dayCountConvention, string interpolation,
            string extrapolationShort, string extrapolationLong, string outputSeries, double maximumMaturity, string outputType)
        {
            MarketCurveId = marketCurveId.ToString();
            ShortName = shortName;
            Description = description;
            LastLiquidTenor = lastLiquidTenor;
            DayCountConvention = dayCountConvention;
            Interpolation = interpolation;
            ExtrapolationShort = extrapolationShort;
            ExtrapolationLong = extrapolationLong;
            OutputSeries = outputSeries;
            MaximumMaturity = maximumMaturity;
            OutputType = outputType;
        }

        Guid ICurveRecipeCreated.MarketCurveId => Guid.Parse(MarketCurveId);
    }
}
