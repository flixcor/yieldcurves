using System;
using Common.Core;

namespace Common.Events
{
    public class InstrumentCreated : IEvent
    {
        public InstrumentCreated(Guid aggregateId, string vendor, string description, bool hasPriceType = false, int version = 0)
        {
            AggregateId = aggregateId;
            Version = version;
            Vendor = vendor ?? throw new ArgumentNullException(nameof(vendor));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            HasPriceType = hasPriceType;
        }

        public string Vendor { get; set; }
        public string Description { get; set; }
        public bool HasPriceType { get; }

        public Guid AggregateId { get; }
        public int Version { get; private set; }
		
		public IEvent WithVersion(int version)
		{
			var clone = (dynamic)MemberwiseClone();
			clone.Version = version;
			return clone;
		}
    }
}
