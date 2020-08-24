using AutoFixture;
using Xunit;
using static ExampleService.Domain.MarketCurve;
using static ExampleService.Domain.MarketCurve.Commands;
using static ExampleService.Domain.MarketCurve.Events;
using static ExampleService.Test.Shared.GivenWhenThen<ExampleService.Program>;


namespace ExampleService.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var fixture = new Fixture();
            var initialName = fixture.Create<MarketCurveNamed>();
            var command = fixture.Create<NameAndAddInstrument>();

            Given<State>(initialName).

            When(command).

            Then(
                new MarketCurveNamed(command.Name),
                new InstrumentAddedToCurve(command.Instrument)
            );
        }
    }
}
