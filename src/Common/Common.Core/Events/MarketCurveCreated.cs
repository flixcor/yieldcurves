using Common.Core;
using Newtonsoft.Json;
using System;

namespace Common.Core.Events
{
    public class MarketCurveCreated : Event
    {
        public MarketCurveCreated(Guid id, string country, string curveType, string floatingLeg = null) : base(id)
        {
            Country = country ?? throw new ArgumentNullException(nameof(country));
            CurveType = curveType ?? throw new ArgumentNullException(nameof(curveType));
            FloatingLeg = floatingLeg;
        }

        [JsonConstructor]
        protected MarketCurveCreated(Guid id, string country, string curveType, string floatingLeg, int version) : this(id, country, curveType, floatingLeg)
        {
            Version = version;
        }

        public string Country { get; }
        public string CurveType { get; }
        public string FloatingLeg { get; }
    }
}
