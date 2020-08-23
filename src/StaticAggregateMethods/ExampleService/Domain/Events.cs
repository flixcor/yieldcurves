namespace ExampleService.Domain
{
    public static class Events
    {
        public record MarketCurveNamed(string Name);

        public record InstrumentAddedToCurve(string InstrumentId);
    }
}
