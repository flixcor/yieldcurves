using Common.Core;
using Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public static Result<RegularInstrument> TryCreate(Guid id, Vendor vendor, string description)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(description))
            {
                errors.Add($"{nameof(description)} cannot be empty");
            }

            if (id.Equals(Guid.Empty))
            {
                errors.Add($"{nameof(id)} cannot be empty");
            }

            if (vendor == Bloomberg)
            {
                errors.Add($"{nameof(Vendor)} {nameof(Bloomberg)} cannot be used for a {nameof(RegularInstrument)}. Create a {nameof(BloombergInstrument)}");
            }

            return errors.Any() 
                ? Result.Fail<RegularInstrument>(errors.ToArray()) 
                : Result.Ok(new RegularInstrument(id, vendor, description));
        }

        private RegularInstrument(Guid id, Vendor vendor, string description)
        {
            ApplyEvent(new RegularInstrumentCreated(id, vendor.ToString(), description));
            ApplyEvent(new InstrumentCreated(id, vendor.ToString(), description));
        }

        private static void Apply(RegularInstrument i, RegularInstrumentCreated e)
        {
            i.Id = e.Id;
        }
    }
}
