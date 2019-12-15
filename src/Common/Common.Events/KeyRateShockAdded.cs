using System;
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
        public KeyRateShockAdded(Guid aggregateId, int order, string shockTarget, double shift, double[] maturities)
        {
            AggregateId = aggregateId.ToString("N");
            Order = order;
            ShockTarget = shockTarget;
            Shift = shift;
            Maturities.Add(maturities);
        }

        IEnumerable<double> IKeyRateShockAdded.Maturities => Maturities;

        Guid IEvent.AggregateId => Guid.Parse(AggregateId);

        public IEvent WithVersion(int version)
        {
            var clone = (KeyRateShockAdded)MemberwiseClone();
            clone.Version = version;
            return clone;
        }
    }
}
