using System;
using Common.Tests;
using Instruments.Domain;
using NUnit.Framework;
using static Common.Events.Create;

namespace UnitTests
{
    public class UT_RegularInstrumentTests : AggregateTest<RegularInstrument>
    {
        [Test]
        public void UT_new_regularInstrument_generates_matching_event()
        {
            var id = Guid.NewGuid();
            var vendor = Vendor.UBS;
            var desc = "description";

            WhenCreated(() => RegularInstrument.TryCreate(id, vendor, desc).Content)
                .Then(
                    RegularInstrumentCreated(id, vendor.ToString(), desc),
                    InstrumentCreated(id, vendor.ToString(), desc));
        }

        [Test]
        public void UT_new_regularInstrument_with_BB_generates_error()
        {
            var id = Guid.NewGuid();
            var vendor = Vendor.Bloomberg;
            var desc = "description";

            Assert.False(RegularInstrument.TryCreate(id, vendor, desc).IsSuccessful);
        }
    }
}
