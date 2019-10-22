using System;
using Common.Core;

namespace Common.Events
{
    public class MarketCurveCreated : IEvent
    {
        public MarketCurveCreated(Guid id, string country, string curveType, string floatingLeg = null, int version = 0)
        {
            Id = id;
            Country = country ?? throw new ArgumentNullException(nameof(country));
            CurveType = curveType ?? throw new ArgumentNullException(nameof(curveType));
            FloatingLeg = floatingLeg;
            Version = version;
        }

        public Guid Id { get; }
        public int Version { get; }
        public string Country { get; }
        public string CurveType { get; }
        public string FloatingLeg { get; }

    }
}
