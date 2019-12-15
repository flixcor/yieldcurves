using System;
using Common.Core;

namespace Common.Events
{
    public interface IParallelShockAdded : IEvent
    {
        int Order { get; }
        double Shift { get; }
        string ShockTarget { get; }
    }

    internal partial class ParallelShockAdded : IParallelShockAdded
    {
        public ParallelShockAdded(Guid aggregateId, int order, string shockTarget, double shift)
        {
            AggregateId = aggregateId.ToString("N");
            Order = order;
            ShockTarget = shockTarget;
            Shift = shift;
        }

        Guid IEvent.AggregateId => Guid.Parse(AggregateId);

        public IEvent WithVersion(int version)
        {
            var clone = (ParallelShockAdded)MemberwiseClone();
            clone.Version = version;
            return clone;
        }
    }
}
