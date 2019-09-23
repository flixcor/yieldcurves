using System;
using System.Collections.Generic;
using CurveRecipes.Domain;

namespace CurveRecipes.Service.Features.CreateCurveRecipe
{
    public class Dto
    {
        public Command Command { get; set; }
        public IEnumerable<MarketCurveDto> MarketCurves { get; set; } = new List<MarketCurveDto>();
        public IEnumerable<string> DayCountConventions { get; set; } = Enum.GetNames(typeof(DayCountConvention));
        public IEnumerable<string> Interpolations { get; set; } = Enum.GetNames(typeof(Interpolation));
        public IEnumerable<string> ExtrapolationShorts { get; set; } = Enum.GetNames(typeof(ExtrapolationShort));
        public IEnumerable<string> ExtrapolationLongs { get; set; } = Enum.GetNames(typeof(ExtrapolationLong));
        public IEnumerable<string> OutputTypes { get; set; } = Enum.GetNames(typeof(OutputType));
        public IEnumerable<string> OutputSeries { get; set; } = Enum.GetNames(typeof(OutputSeries));
    }
}
