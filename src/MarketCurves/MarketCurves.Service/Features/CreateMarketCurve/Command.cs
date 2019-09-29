using System;
using Common.Core;
using MarketCurves.Domain;

namespace MarketCurves.Service.Features.CreateMarketCurve
{
    public class Command : ICommand
    {
        public Guid Id { get; set; }
        public Country Country { get; set; }
        public CurveType CurveType { get; set; }
        public FloatingLeg? FloatingLeg { get; set; }
    }
}
