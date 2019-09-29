using System;
using System.Collections.Generic;
using MarketCurves.Domain;

namespace MarketCurves.Service.Features.CreateMarketCurve
{
    public class Dto
    {
        public Command Command { get; set; } = new Command
        {
            Id = Guid.NewGuid()
        };

        public IEnumerable<string> Countries { get; set; } = Enum.GetNames(typeof(Country));
        public IEnumerable<string> CurveTypes { get; set; } = Enum.GetNames(typeof(CurveType));
        public IEnumerable<string> FloatingLegs { get; set; } = Enum.GetNames(typeof(FloatingLeg));
    }
}
