using ExampleService.Domain;
using ExampleService.Features;
using ExampleService.Shared;
using static ExampleService.Domain.MarketCurve.Commands;
using static ExampleService.Domain.MarketCurve.Events;

namespace ExampleService
{
    public static class Bootstrap
    {
        public static void Setup(IEventStore eventStore)
        {
            Registry.RegisterAll<Program>();
            InMemoryProjectionStore.TryRegister<CurveList>(GetCurveList.Project);

            var testEndpoint = RestMapper.TryMapQuery<GetCurveList, CurveList>("/marketcurves");
            var commandEndpoint = RestMapper.TryMapCommand<MarketCurve.State, NameAndAddInstrument>("/marketcurves")?.WithExpected(new NameAndAddInstrument());

            var endpoints = RestMapper.Enumerate(testEndpoint, commandEndpoint);

            RestMapper.TryMapIndex(endpoints);

            EventMapper.TryMap<InstrumentAddedToCurve>("instrument added");
            EventMapper.TryMap<MarketCurveNamed>("market curve named");

            RestMapper.SetEventStore(eventStore);
        }
    }
}
