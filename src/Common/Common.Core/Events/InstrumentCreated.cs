using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Common.Core.Events
{
    public class InstrumentCreated : Event
    {
        public InstrumentCreated(Guid id, string vendor, string description, bool hasPriceType = false) : base(id)
        {
            Vendor = vendor ?? throw new ArgumentNullException(nameof(vendor));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            HasPriceType = hasPriceType;
        }

        [JsonConstructor]
        protected InstrumentCreated(Guid id, string vendor, string description, int version, bool hasPriceType) : this(id, vendor, description, hasPriceType)
        {
            Version = version;
        }

        public string Vendor { get; set; }
        public string Description { get; set; }
        public bool HasPriceType { get; }
    }
}
