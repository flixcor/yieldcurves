﻿namespace ExampleService.Domain
{
    public static class MarketCurve
    {
        public static MarketCurveState Name(string name)
            => new MarketCurveState().Raise(new MarketCurveNamed
            {
                Name = name
            });

        public static MarketCurveState AddInstrument(this MarketCurveState state, string instrumentId)
            => state.Raise(new InstrumentAdded
            {
                InstrumentId = instrumentId
            });
    }
}