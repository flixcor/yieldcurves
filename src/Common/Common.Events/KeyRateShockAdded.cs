using System;
using System.Collections.Immutable;
using Common.Core;

namespace Common.Events
{
    public class KeyRateShockAdded : IEvent
    {
        public KeyRateShockAdded(Guid aggregateId, int order, string shockTarget, double shift, double[] maturities, int version = 0)
        {
            AggregateId = aggregateId;
            Version = version;
            Order = order;
            ShockTarget = shockTarget;
            Shift = shift;
            Maturities = maturities.ToImmutableArray();
        }

        public int Order { get; }
        public string ShockTarget { get; }
        public double Shift { get; }

        public ImmutableArray<double> Maturities { get; }
        public Guid AggregateId { get; }
        public int Version { get; }
		
		public IEvent WithVersion(int version)
		{
			throw new NotImplementedException();
		}
    }
}
