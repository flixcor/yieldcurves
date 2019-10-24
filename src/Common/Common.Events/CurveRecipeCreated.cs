using System;
using Common.Core;

namespace Common.Events
{
    public class CurveRecipeCreated : IEvent
    {
        public CurveRecipeCreated(Guid aggregateId, Guid marketCurveId, string shortName, string description, string lastLiquidTenor, string dayCountConvention, string interpolation,
            string extrapolationShort, string extrapolationLong, string outputSeries, double maximumMaturity, string outputType)
        {
            AggregateId = aggregateId;
            MarketCurveId = marketCurveId;
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

        public Guid MarketCurveId { get; }
        public string ShortName { get; }
        public string Description { get; }
        public string LastLiquidTenor { get; }
        public string DayCountConvention { get; }
        public string Interpolation { get; }
        public string ExtrapolationShort { get; }
        public string ExtrapolationLong { get; }
        public string OutputSeries { get; }
        public double MaximumMaturity { get; }
        public string OutputType { get; }

        public Guid AggregateId { get; }
        public int Version { get; private set; }
		
		public IEvent WithVersion(int version)
		{
			var clone = (CurveRecipeCreated)MemberwiseClone();
			clone.Version = version;
			return clone;
		}
    }
}
