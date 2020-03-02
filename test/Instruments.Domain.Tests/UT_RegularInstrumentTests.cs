using System;
using Common.Core;
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
            var desc = "description".NonEmpty();

            When((r) => r.TryDefine(vendor, desc))
                .Then(
                    RegularInstrumentCreated(vendor, desc),
                    InstrumentCreated(vendor, desc));
        }

        [Test]
        public void UT_new_regularInstrument_with_BB_generates_error()
        {
            var vendor = Vendor.Bloomberg;
            var desc = "description".NonEmpty();

            Assert.False(Aggregate.TryDefine(vendor, desc).MapLeft(x=> false).Reduce(x=> true));
        }
    }
}
