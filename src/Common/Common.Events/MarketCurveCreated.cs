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
        public MarketCurveCreated(Guid aggregateId, string country, string curveType, string floatingLeg = null)
        {
            AggregateId = aggregateId.ToString("N");
            Country = country ?? throw new ArgumentNullException(nameof(country));
            CurveType = curveType ?? throw new ArgumentNullException(nameof(curveType));

            if (!string.IsNullOrWhiteSpace(floatingLeg))
            {
                FloatingLeg = floatingLeg;
            }
        }

        Guid IEvent.AggregateId => Guid.Parse(AggregateId);

        public IEvent WithVersion(int version)
        {
            var clone = (MarketCurveCreated)MemberwiseClone();
            clone.Version = version;
            return clone;
        }
    }
}
