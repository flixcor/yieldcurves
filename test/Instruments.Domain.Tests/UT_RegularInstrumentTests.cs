using System;
using Common.Events;
using Common.Tests;
using Instruments.Domain;
using NUnit.Framework;

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

            WhenCreated(() => RegularInstrument.TryCreate(id, vendor, desc).Content);
            Then(
                new RegularInstrumentCreated(id, vendor.ToString(), desc), 
                new InstrumentCreated(id, vendor.ToString(), desc));
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
