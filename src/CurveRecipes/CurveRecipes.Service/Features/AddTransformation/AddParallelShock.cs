using Common.Core;

namespace CurveRecipes.Service.Features.AddTransformation
{
    public class AddParallelShock
    {
        public string ShockTarget { get; set; } = Domain.ShockTarget.ZeroRates.ToString();
        public double Shift { get; set; }
    }
}
