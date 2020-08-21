using System;
using System.Threading.Tasks;
using ExampleService.Domain;
using ExampleService.Shared;

namespace ExampleService.Features
{
    public record NameAndAdd : AppService<MarketCurveState>
    {
        public string? Id { get; init; } = Guid.NewGuid().ToString();
        public string? Name { get; init; }
        public string? Instrument { get; init; }
        protected override string GetId() => Id ?? throw new Exception();

        protected override ValueTask<MarketCurveState> HandleInternal(MarketCurveState aggregate)
        {
            if (Name is null || Instrument is null)
            {
                throw new Exception();
            }

            var result = MarketCurve.Name(aggregate, Name).AddInstrument(Instrument);
            return ValueTask.FromResult(result);
        }
    }
}
