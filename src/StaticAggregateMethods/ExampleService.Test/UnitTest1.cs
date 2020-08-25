using AutoFixture;
using Xunit;
using static ExampleService.Domain.MarketCurve.Commands;
using static ExampleService.Domain.MarketCurve.Events;
using static ExampleService.Domain.MarketCurve;
using ExampleService.Test.Shared;


namespace ExampleService.Test
{
    using static GivenWhenThen<Aggregate, State>;
    public class UnitTest1
    {
        

        [Fact]
        public void Test1()
        {
            var fixture = new Fixture();
            var initialName = fixture.Create<MarketCurveNamed>();
            var command = fixture.Create<NameAndAddInstrument>();

            Given(initialName).

            When(command).

            Then(
                new MarketCurveNamed(command.Name),
                new InstrumentAddedToCurve(command.Instrument)
            );
        }
    }
}
