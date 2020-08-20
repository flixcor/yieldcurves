using System;
using ExampleService.Domain;
using Xunit;

namespace ExampleService.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var events = MarketCurve.Name("name").AddInstrument("instrument").GetUncommittedEvents();
            Assert.Collection(events,
                (e) => Assert.IsType<MarketCurveNamed>(e.Content),
                (e) => Assert.IsType<InstrumentAdded>(e.Content)
            );
        }
    }
}
