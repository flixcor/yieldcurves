using Newtonsoft.Json;
using System;

namespace Common.Core.Events
{
    public class ParallelShockAdded : Event
    {
        public ParallelShockAdded(Guid id, int order, string shockTarget, double shift) : base(id)
        {
            Order = order;
            ShockTarget = shockTarget;
            Shift = shift;
        }

        [JsonConstructor]
        protected ParallelShockAdded(Guid id, int order, string shockTarget, double shift, int version) : this(id, order, shockTarget, shift)
        {
            Version = version;
        }

        public int Order { get; }
        public string ShockTarget { get; }
        public double Shift { get; }
    }
}
