using System;
using Common.Core;

namespace Common.Events
{
    public class RegularInstrumentCreated : IEvent
    {
        public RegularInstrumentCreated(Guid aggregateId, string vendor, string description)
        {
            AggregateId = aggregateId;
            Vendor = vendor ?? throw new ArgumentNullException(nameof(vendor));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        public string Vendor { get; set; }
        public string Description { get; set; }
        public Guid AggregateId { get; }
        public int Version { get; private set; }
		
		public IEvent WithVersion(int version)
		{
			var clone = (RegularInstrumentCreated)MemberwiseClone();
			clone.Version = version;
			return clone;
		}
    }
}
