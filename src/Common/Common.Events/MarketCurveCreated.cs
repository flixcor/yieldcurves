using System;
using Common.Core;

namespace Common.Events
{
    public interface IMarketCurveCreated : IEvent
    {
        string Country { get; }
        string CurveType { get; }
        string FloatingLeg { get; }
    }

    internal partial class MarketCurveCreated : IMarketCurveCreated
    {
        public MarketCurveCreated(string country, string curveType, string? floatingLeg = null)
        {
            Country = country ?? throw new ArgumentNullException(nameof(country));
            CurveType = curveType ?? throw new ArgumentNullException(nameof(curveType));

            if (!string.IsNullOrWhiteSpace(floatingLeg))
            {
                FloatingLeg = floatingLeg;
            }
        }
    }
}
