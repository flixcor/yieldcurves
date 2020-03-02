using System;
using Common.Core;
using CurveRecipes.Domain;

namespace CurveRecipes.Service.Features.CreateCurveRecipe
{
    public class OutputFrequency
    {
        public OutputSeries OutputSeries { get; set; }
        public double MaximumMaturity { get; set; }
    }

    public class Command : ICommand
    {
        public Guid Id { get; set; }
        public Guid MarketCurveId { get; set; }
        public string? ShortName { get; set; }
        public string? Description { get; set; }
        public Tenor LastLiquidTenor { get; set; }
        public DayCountConvention DayCountConvention { get; set; }
        public Interpolation Interpolation { get; set; }
        public ExtrapolationShort ExtrapolationShort { get; set; }
        public ExtrapolationLong ExtrapolationLong { get; set; }
        public OutputFrequency OutputFrequency { get; set; } = new OutputFrequency();
        public OutputType OutputType { get; set; }
    }
}
