using ExampleService.Domain;
using ExampleService.Features;
using ExampleService.Shared;

namespace ExampleService
{
    public static class Bootstrap
    {
        public static void Setup(IAggregateStore aggregateStore)
        {
            InMemoryProjectionStore.TryRegister<CurveList>(GetCurveList.Project);

            var testEndpoint = RestMapper.TryMapQuery<GetCurveList, CurveList>("/marketcurves");
            var commandEndpoint = RestMapper.TryMapCommand<NameAndAdd>("/marketcurves")?.WithExpected(new NameAndAdd());

            var endpoints = RestMapper.Enumerate(testEndpoint, commandEndpoint);

            RestMapper.TryMapIndex(endpoints);

            EventMapper.TryMap<InstrumentAdded>("instrument added");
            EventMapper.TryMap<MarketCurveNamed>("market curve named");

            RestMapper.SetAggregateStore(aggregateStore);
        }
    }
}
