using System;
using Common.Core;

namespace Common.Events
{
    public class InstrumentCreated : Event
    {
        public InstrumentCreated(Guid id, string vendor, string description, bool hasPriceType = false) : base(id)
        {
            Vendor = vendor ?? throw new ArgumentNullException(nameof(vendor));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            HasPriceType = hasPriceType;
        }

        public string Vendor { get; set; }
        public string Description { get; set; }
        public bool HasPriceType { get; }

    }
}
