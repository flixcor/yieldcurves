using System;
using Common.Core;

namespace Common.Events
{
    public class ParallelShockAdded : IEvent
    {
        public ParallelShockAdded(Guid id, int order, string shockTarget, double shift, int version = 0)
        {
            Id = id;
            Order = order;
            ShockTarget = shockTarget;
            Shift = shift;
            Version = version;
        }

        public Guid Id { get; }
        public int Version { get; }
        public int Order { get; }
        public string ShockTarget { get; }
        public double Shift { get; }

    }
}
