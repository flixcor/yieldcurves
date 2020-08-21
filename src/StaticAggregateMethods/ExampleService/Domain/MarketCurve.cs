namespace ExampleService.Domain
{
    public static class MarketCurve
    {
        public static MarketCurveState Name(MarketCurveState state, string name)
            => state.Raise(new MarketCurveNamed(name));

        public static MarketCurveState AddInstrument(this MarketCurveState state, string instrumentId)
            => state.Raise(new InstrumentAdded(instrumentId));
    }
}
