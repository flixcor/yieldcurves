using System;
using System.Collections.Immutable;
using Common.Core;
using CurveRecipes.Domain;

namespace CurveRecipes.Service.Features.AddTransformation.AddShock.AddKeyRateShock
{
    public class Command : ICommand
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public ShockTarget ShockTarget { get; set; }
        public double Shift { get; set; }
        public ImmutableArray<double> Maturities { get; set; } = ImmutableArray<double>.Empty;
    }
}
