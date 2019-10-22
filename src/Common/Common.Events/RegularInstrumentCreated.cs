using System;
using Common.Core;

namespace Common.Events
{
    public class RegularInstrumentCreated : IEvent
    {
        public RegularInstrumentCreated(Guid id, string vendor, string description, int version = 0)
        {
            Id = id;
            Vendor = vendor ?? throw new ArgumentNullException(nameof(vendor));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Version = version;
        }

        public Guid Id { get; }
        public int Version { get; }
        public string Vendor { get; set; }
        public string Description { get; set; }

    }
}
