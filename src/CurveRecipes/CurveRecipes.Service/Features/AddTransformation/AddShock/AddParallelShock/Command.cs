using System;
using Common.Core;
using CurveRecipes.Domain;

namespace CurveRecipes.Service.Features.AddTransformation.AddShock.AddParallelShock
{
    public class Command : ICommand
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public ShockTarget ShockTarget { get; set; }
        public double Shift { get; set; }
    }
}
