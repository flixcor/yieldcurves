using System;
using System.Collections.Immutable;
using Common.Core;

namespace Common.Events
{
    public class KeyRateShockAdded : IEvent
    {
        public KeyRateShockAdded(Guid id, int order, string shockTarget, double shift, double[] maturities, int version = 0)
        {
            Id = id;
            Order = order;
            ShockTarget = shockTarget;
            Shift = shift;
            Version = version;
            Maturities = maturities.ToImmutableArray();
        }

        public Guid Id { get; }
        public int Version { get; }
        public int Order { get; }
        public string ShockTarget { get; }
        public double Shift { get; }

        public ImmutableArray<double> Maturities { get; }
    }
}
