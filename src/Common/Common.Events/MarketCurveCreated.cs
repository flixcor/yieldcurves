using System;
using Common.Core;

namespace Common.Events
{
    public class MarketCurveCreated : IEvent
    {
        public MarketCurveCreated(Guid aggregateId, string country, string curveType, string floatingLeg = null, int version = 0)
        {
            AggregateId = aggregateId;
            Version = version;
            Country = country ?? throw new ArgumentNullException(nameof(country));
            CurveType = curveType ?? throw new ArgumentNullException(nameof(curveType));
            FloatingLeg = floatingLeg;
        }

        public string Country { get; }
        public string CurveType { get; }
        public string FloatingLeg { get; }

        public Guid AggregateId { get; }
        public int Version { get; }
		
		public IEvent WithVersion(int version)
		{
			throw new NotImplementedException();
		}
    }
}
