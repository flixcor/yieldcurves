using System.Collections.Generic;
using Common.Core;

namespace Common.Events
{
    public interface IKeyRateShockAdded : IEvent
    {
        IEnumerable<double> Maturities { get; }
        int Order { get; }
        double Shift { get; }
        string ShockTarget { get; }
    }

    internal partial class KeyRateShockAdded : IKeyRateShockAdded
    {
        public KeyRateShockAdded(int order, string shockTarget, double shift, double[] maturities)
        {
            Order = order;
            ShockTarget = shockTarget;
            Shift = shift;
            Maturities.Add(maturities);
        }

        IEnumerable<double> IKeyRateShockAdded.Maturities => Maturities;
    }
}
