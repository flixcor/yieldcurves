using System;
using System.Collections.Generic;
using System.Linq;
using CalculationEngine.Service.ActorModel.Commands;
using CalculationEngine.Service.Domain;
using Common.Core;
using Common.Events;

namespace CalculationEngine.Service.ActorModel.Actors
{
    public class MarketCurveForDateActor : IdempotentActor
    {
        private readonly Dictionary<Guid, IEventWrapper<IInstrumentPricingPublished>> _pricings = new Dictionary<Guid, IEventWrapper<IInstrumentPricingPublished>>();

        private readonly Guid _marketCurveId;
        private readonly Date _asOfDate;

        public MarketCurveForDateActor(Guid marketCurveId, Date asOfDate)
        {
            _marketCurveId = marketCurveId;
            _asOfDate = asOfDate;

            IdempotentEvent<IInstrumentPricingPublished>(Ignore, Recover);
            Command<SendMeCalculate>(Handle);
        }

        public void Recover(IEventWrapper<IInstrumentPricingPublished> wrapper)
        {
            var e = wrapper.GetContent();

            if (!_pricings.TryGetValue(e.InstrumentId, out var pricing))
            {
                _pricings.Add(e.InstrumentId, wrapper);
            }
            else if (Date.FromString(pricing.GetContent().AsOfDate) < Date.FromString(e.AsOfDate) || (pricing.GetContent().AsOfDate == e.AsOfDate && pricing.GetContent().AsAtDate < e.AsAtDate))
            {
                _pricings[e.InstrumentId] = wrapper;
            }
        }

        public void Handle(SendMeCalculate q)
        {
            if (q.CurvePoints.All(x => _pricings.ContainsKey(x.GetContent().InstrumentId)))
            {
                Sender.Tell(new Calculate(_asOfDate, q.CurvePoints, _pricings.Values), Self);
            }
        }


        public override string PersistenceId => $"{_marketCurveId}|{_asOfDate}";
    }
}
