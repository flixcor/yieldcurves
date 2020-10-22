using AutoFixture;
using Lib.Test.Shared;
using Xunit;
using static Contracts.ContractCollection;
using static ExampleService.WriteSide.MarketCurve;
using static ExampleService.WriteSide.MarketCurve.Events;


namespace Lib.Test
{
    using static GivenWhenThen<Aggregate, State>;
    public class MarketCurveTest
    {
        private static readonly Fixture s_fixture = new Fixture();

        [Fact]
        public void NameAndAddInstrument()
        {   
            var initialName = s_fixture.Create<MarketCurveNamed>();
            var command = s_fixture.Create<NameAndAddInstrument>();

            Given(initialName).

            When(command).

            Then(
                new MarketCurveNamed(command.Name),
                new InstrumentAddedToCurve(command.Instrument)
            );
        }

        [Fact]
        public void AddInstrument()
        {
            var command = s_fixture.Create<AddInstrument>();

            When(command).

            Then(
                new InstrumentAddedToCurve(command.Instrument)
            );
        }
    }
}
