using System;
using Common.Tests;
using Instruments.Domain;
using NUnit.Framework;
using static Common.Events.Helpers;

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

            When((r) => r.TryDefine(vendor, desc))
                .Then(
                    RegularInstrumentCreated(vendor.ToString(), desc),
                    InstrumentCreated(vendor.ToString(), desc));
        }

        [Test]
        public void UT_new_regularInstrument_with_BB_generates_error()
        {
            var vendor = Vendor.Bloomberg;
            var desc = "description";

            Assert.False(Aggregate.TryDefine(vendor, desc).IsSuccessful);
        }
    }
}
