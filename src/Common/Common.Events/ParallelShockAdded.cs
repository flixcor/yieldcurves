using System;
using Common.Core;

namespace Common.Events
{
    public class ParallelShockAdded : Event
    {
        public ParallelShockAdded(Guid id, int order, string shockTarget, double shift) : base(id)
        {
            Order = order;
            ShockTarget = shockTarget;
            Shift = shift;
        }

        public int Order { get; }
        public string ShockTarget { get; }
        public double Shift { get; }

    }
}
