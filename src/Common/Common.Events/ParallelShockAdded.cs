using System;
using Common.Core;

namespace Common.Events
{
    public class ParallelShockAdded : IEvent
    {
        public ParallelShockAdded(Guid aggregateId, int order, string shockTarget, double shift, int version = 0)
        {
            AggregateId = aggregateId;
            Version = version;
            Order = order;
            ShockTarget = shockTarget;
            Shift = shift;
        }

        public int Order { get; }
        public string ShockTarget { get; }
        public double Shift { get; }

        public Guid AggregateId { get; }
        public int Version { get; private set; }
		
		public IEvent WithVersion(int version)
		{
			var clone = (dynamic)MemberwiseClone();
			clone.Version = version;
			return clone;
		}
    }
}
