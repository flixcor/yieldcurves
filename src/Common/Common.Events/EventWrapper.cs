using System;
using Common.Core;
using NodaTime;
using NodaTime.Serialization.Protobuf;

namespace Common.Events
{
    internal partial class EventWrapper : IEventWrapper, IMessage
    {
        internal EventWrapper(Guid aggregateId, Instant timestamp, int version, IEvent content, long id)
        {
            AggregateId = aggregateId;
            Timestamp = timestamp.ToTimestamp();
            Version = version;
            SetPayload(content);
            Id = id;
        }

        private void SetPayload(IEvent content) => GetPayloadAction(content)();

        private Action GetPayloadAction(IEvent content) => content switch
        {
            InstrumentCreated e => () => InstrumentCreated = e,
            BloombergInstrumentCreated e => () => BloombergInstrumentCreated = e,
            RegularInstrumentCreated e => () => RegularInstrumentCreated = e,
            MarketCurveCreated e => () => MarketCurveCreated = e,
            CurveCalculated e => () => CurveCalculated = e,
            CurveCalculationFailed e => () => CurveCalculationFailed = e,
            CurvePointAdded e => () => CurvePointAdded = e,
            CurveRecipeCreated e => () => CurveRecipeCreated = e,
            InstrumentPricingPublished e => () => InstrumentPricingPublished = e,
            KeyRateShockAdded e => () => KeyRateShockAdded = e,
            ParallelShockAdded e => () => ParallelShockAdded = e,
            _ => throw new NotImplementedException(),
        };

        Guid IEventWrapper.AggregateId => AggregateId;

        Instant IEventWrapper.Timestamp => Timestamp.ToInstant();

        public IEvent GetContent() => PayloadCase switch
        {
            PayloadOneofCase.None => throw new Exception(),
            PayloadOneofCase.InstrumentCreated => InstrumentCreated,
            PayloadOneofCase.BloombergInstrumentCreated => BloombergInstrumentCreated,
            PayloadOneofCase.RegularInstrumentCreated => RegularInstrumentCreated,
            PayloadOneofCase.MarketCurveCreated => MarketCurveCreated,
            PayloadOneofCase.CurveCalculated => CurveCalculated,
            PayloadOneofCase.CurveCalculationFailed => CurveCalculationFailed,
            PayloadOneofCase.CurvePointAdded => CurvePointAdded,
            PayloadOneofCase.CurveRecipeCreated => CurveRecipeCreated,
            PayloadOneofCase.InstrumentPricingPublished => InstrumentPricingPublished,
            PayloadOneofCase.KeyRateShockAdded => KeyRateShockAdded,
            PayloadOneofCase.ParallelShockAdded => ParallelShockAdded,
            _ => throw new Exception(),
        };
    }
}
