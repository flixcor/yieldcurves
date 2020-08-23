using ExampleService.Domain;
using ExampleService.Features;
using ExampleService.Shared;
using static ExampleService.Domain.Commands;
using static ExampleService.Domain.Events;

namespace ExampleService
{
    public static class Bootstrap
    {
        public static void Setup(IAggregateStore aggregateStore)
        {
            InMemoryProjectionStore.TryRegister<CurveList>(GetCurveList.Project);

            var testEndpoint = RestMapper.TryMapQuery<GetCurveList, CurveList>("/marketcurves");
            var commandEndpoint = RestMapper.TryMapCommand(MarketCurve.NameAndAdd, "/marketcurves")?.WithExpected(new NameAndAddInstrument());

            var endpoints = RestMapper.Enumerate(testEndpoint, commandEndpoint);

            RestMapper.TryMapIndex(endpoints);

            EventMapper.TryMap<InstrumentAddedToCurve>("instrument added");
            EventMapper.TryMap<MarketCurveNamed>("market curve named");

            RestMapper.SetAggregateStore(aggregateStore);
        }
    }
}
