using Common.Core;
using Common.Core.Events;
using System;
using static Instruments.Domain.Vendor;

namespace Instruments.Domain
{
    public class RegularInstrument : Aggregate<RegularInstrument>
    {
        static RegularInstrument()
        {
            RegisterApplyMethod<RegularInstrumentCreated>(Apply);
        }

        private RegularInstrument()
        {
        }

        public RegularInstrument(Guid id, Vendor vendor, string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException(description);
            }

            if (id.Equals(Guid.Empty))
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (vendor == Bloomberg)
            {
                throw new ArgumentException($"Vendor {nameof(Bloomberg)} cannot be used for a {nameof(RegularInstrument)}. Create a {nameof(BloombergInstrument)}", nameof(vendor));
            }

            ApplyEvent(new RegularInstrumentCreated(id, vendor.ToString(), description));
        }

        private static void Apply(RegularInstrument i, RegularInstrumentCreated e)
        {
            i.Id = e.Id;
        }
    }
}