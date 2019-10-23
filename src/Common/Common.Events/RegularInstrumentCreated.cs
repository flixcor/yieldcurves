using System;
using Common.Core;

namespace Common.Events
{
    public class RegularInstrumentCreated : Event
    {
        public RegularInstrumentCreated(Guid id, string vendor, string description) : base(id)
        {
            Vendor = vendor ?? throw new ArgumentNullException(nameof(vendor));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        public string Vendor { get; set; }
        public string Description { get; set; }
    }
}
