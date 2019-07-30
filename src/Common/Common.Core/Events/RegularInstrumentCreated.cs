using Newtonsoft.Json;
using System;

namespace Common.Core.Events
{
    public class RegularInstrumentCreated : Event
    {
        public RegularInstrumentCreated(Guid id, string vendor, string description) : base(id)
        {
            Vendor = vendor ?? throw new ArgumentNullException(nameof(vendor));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        [JsonConstructor]
        protected RegularInstrumentCreated(Guid id, string vendor, string description, int version) : this(id, vendor, description)
        {
            Version = version;
        }

        public string Vendor { get; set; }
        public string Description { get; set; }
    }
}