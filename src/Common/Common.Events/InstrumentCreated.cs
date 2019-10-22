using System;
using Common.Core;

namespace Common.Events
{
    public class InstrumentCreated : IEvent
    {
        public InstrumentCreated(Guid id, string vendor, string description, bool hasPriceType = false, int version = 0)
        {
            Id = id;
            Vendor = vendor ?? throw new ArgumentNullException(nameof(vendor));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            HasPriceType = hasPriceType;
            Version = version;
        }

        public Guid Id { get; }
        public int Version { get; }
        public string Vendor { get; set; }
        public string Description { get; set; }
        public bool HasPriceType { get; }

    }
}
