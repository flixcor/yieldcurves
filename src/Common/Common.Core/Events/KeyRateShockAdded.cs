using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Common.Core.Events
{
    public class KeyRateShockAdded : Event
    {
        public KeyRateShockAdded(Guid id, int order, string shockTarget, double shift, double[] maturities) : base(id)
        {
            Order = order;
            ShockTarget = shockTarget;
            Shift = shift;
            Maturities = maturities;
        }

        [JsonConstructor]
        protected KeyRateShockAdded(Guid id, int order, string shockTarget, double shift, int version, double[] maturities) : this(id, order, shockTarget, shift, maturities)
        {
            Version = version;
        }

        public int Order { get; }
        public string ShockTarget { get; }
        public double Shift { get; }
        public ICollection<double> Maturities { get; }
    }
}
