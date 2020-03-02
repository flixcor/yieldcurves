using System.Collections.Generic;
using Common.Core;
using Common.EventStore.Lib;
using static Common.Events.Helpers;
using static Instruments.Domain.Vendor;

namespace Instruments.Domain
{
    public class RegularInstrument : Aggregate
    {
        public Either<Error, RegularInstrument> TryDefine(Vendor vendor, NonEmptyString description)
        {
            var errors = new List<string>();

            if (vendor == Bloomberg)
            {
                return new Error($"{nameof(Vendor)} {nameof(Bloomberg)} cannot be used for a {nameof(RegularInstrument)}. Create a {nameof(BloombergInstrument)}");
            }

            GenerateEvent(RegularInstrumentCreated(vendor, description));
            GenerateEvent(InstrumentCreated(vendor, description));

            return this;
        }

        protected override void When(IEvent @event)
        {
        }
    }
}
