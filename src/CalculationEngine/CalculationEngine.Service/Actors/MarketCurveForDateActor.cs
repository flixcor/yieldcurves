using System;
using System.Collections.Generic;
using System.Linq;
using CalculationEngine.Service.ActorModel.Commands;
using Common.Core.Events;

namespace CalculationEngine.Service.ActorModel.Actors
{
    public class MarketCurveForDateActor : IdempotentActor
    {
        private readonly Dictionary<Guid, InstrumentPricingPublished> _pricings = new Dictionary<Guid, InstrumentPricingPublished>();

        private readonly Guid _marketCurveId;
        private readonly DateTime _asOfDate;

        public MarketCurveForDateActor(Guid marketCurveId, DateTime asOfDate)
        {
            _marketCurveId = marketCurveId;
            _asOfDate = asOfDate;

            IdempotentEvent<InstrumentPricingPublished>(Ignore, Recover);
            Command<SendMeCalculate>(Handle);
        }

        public void Recover(InstrumentPricingPublished e)
        {
            if (!_pricings.TryGetValue(e.InstrumentId, out var pricing))
            {
                _pricings.Add(e.InstrumentId, e);
            }
            else if (pricing.AsOfDate.Date < e.AsOfDate.Date || (pricing.AsOfDate.Date == e.AsOfDate.Date && pricing.AsAtDate < e.AsAtDate))
            {
                _pricings[e.InstrumentId] = e;
            }
        }

        public void Handle(SendMeCalculate q)
        {
            if (q.CurvePoints.All(x => _pricings.ContainsKey(x.InstrumentId)))
            {
                Sender.Tell(new Calculate(_asOfDate, q.CurvePoints, _pricings.Values), Self);
            }
        }


        public override string PersistenceId => $"{_marketCurveId}|{_asOfDate.Date.ToString("yyyy-MM-dd")}";
    }
}
