using System.Collections.Generic;
using System.Linq;
using Common.Core;
using Common.EventStore.Lib;
using static Common.Events.Helpers;
using static Instruments.Domain.Vendor;

namespace Instruments.Domain
{
    public class RegularInstrument : Aggregate<RegularInstrument>
    {
        private RegularInstrument()
        {
        }

        public static Result<RegularInstrument> TryCreate(Vendor vendor, string description)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(description))
            {
                errors.Add($"{nameof(description)} cannot be empty");
            }

            if (vendor == Bloomberg)
            {
                errors.Add($"{nameof(Vendor)} {nameof(Bloomberg)} cannot be used for a {nameof(RegularInstrument)}. Create a {nameof(BloombergInstrument)}");
            }

            return errors.Any()
                ? Result.Fail<RegularInstrument>(errors.ToArray())
                : Result.Ok(new RegularInstrument(vendor, description));
        }

        protected override void When(IEvent @event)
        {
        }

        private RegularInstrument(Vendor vendor, string description)
        {
            GenerateEvent(RegularInstrumentCreated(vendor.ToString(), description));
            GenerateEvent(InstrumentCreated(vendor.ToString(), description));
        }
    }
}
