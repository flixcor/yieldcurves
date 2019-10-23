using System;
using System.Collections.Immutable;
using Common.Core;

namespace Common.Events
{
    public class KeyRateShockAdded : Event
    {
        public KeyRateShockAdded(Guid id, int order, string shockTarget, double shift, double[] maturities) : base(id)
        {
            Order = order;
            ShockTarget = shockTarget;
            Shift = shift;
            Maturities = maturities.ToImmutableArray();
        }

        public int Order { get; }
        public string ShockTarget { get; }
        public double Shift { get; }

        public ImmutableArray<double> Maturities { get; }
    }
}
