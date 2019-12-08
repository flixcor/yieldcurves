using System.Collections.Generic;
using Common.Core;

namespace CurveRecipes.Service.Features.AddTransformation
{
    public class AddKeyRateShock
    {
        public int Order { get; set; }
        public string ShockTarget { get; set; } = Domain.ShockTarget.ZeroRates.ToString();
        public double Shift { get; set; }
        public IEnumerable<double> Maturities { get; set; } = new double[] { };
    }
}
