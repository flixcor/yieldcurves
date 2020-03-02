using System;
using System.Collections.Generic;
using CurveRecipes.Domain;

namespace CurveRecipes.Service.Features.CreateCurveRecipe
{
    public class Dto
    {
        public Dto(IEnumerable<MarketCurveDto>? marketCurves)
        {
            MarketCurves = marketCurves ?? new List<MarketCurveDto>();
        }

        public Command Command { get; } = new Command { Id = Guid.NewGuid() };
        public IEnumerable<MarketCurveDto> MarketCurves { get; }
        public IEnumerable<string> DayCountConventions { get; } = Enum.GetNames(typeof(DayCountConvention));
        public IEnumerable<string> Interpolations { get; } = Enum.GetNames(typeof(Interpolation));
        public IEnumerable<string> ExtrapolationShorts { get; } = Enum.GetNames(typeof(ExtrapolationShort));
        public IEnumerable<string> ExtrapolationLongs { get; } = Enum.GetNames(typeof(ExtrapolationLong));
        public IEnumerable<string> OutputTypes { get; } = Enum.GetNames(typeof(OutputType));
        public IEnumerable<string> OutputSeries { get; } = Enum.GetNames(typeof(OutputSeries));
    }
}
