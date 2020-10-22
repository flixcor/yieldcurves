using AutoFixture;
using Lib.Test.Shared;
using Xunit;
using static Contracts.ContractCollection;
using static ExampleService.WriteSide.MarketCurve;
using static ExampleService.WriteSide.MarketCurve.Events;


namespace Lib.Test
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
