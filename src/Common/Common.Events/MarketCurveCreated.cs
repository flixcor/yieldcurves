using System;
using Common.Core;

namespace Common.Events
{
    public class MarketCurveCreated : Event
    {
        public MarketCurveCreated(Guid id, string country, string curveType, string floatingLeg = null) : base(id)
        {
            Country = country ?? throw new ArgumentNullException(nameof(country));
            CurveType = curveType ?? throw new ArgumentNullException(nameof(curveType));
            FloatingLeg = floatingLeg;
        }

        public string Country { get; }
        public string CurveType { get; }
        public string FloatingLeg { get; }

    }
}
