using System.Collections.Generic;
using Common.Core;
using Common.EventStore.Lib;
using static Common.Core.Result;
using static Common.Events.Helpers;
using static Instruments.Domain.Vendor;

namespace Instruments.Domain
{
    public class RegularInstrument : Aggregate<RegularInstrument>
    {
        public Result<RegularInstrument> TryDefine(Vendor vendor, NonEmptyString description)
        {
            var errors = new List<string>();

            if (vendor == Bloomberg)
            {
                return Fail<RegularInstrument>($"{nameof(Vendor)} {nameof(Bloomberg)} cannot be used for a {nameof(RegularInstrument)}. Create a {nameof(BloombergInstrument)}");
            }

            GenerateEvent(RegularInstrumentCreated(vendor.NonEmptyString(), description));
            GenerateEvent(InstrumentCreated(vendor.NonEmptyString(), description));

            return Ok(this);
        }

        protected override void When(IEvent @event)
        {
        }
    }
}
