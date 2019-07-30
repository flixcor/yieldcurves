using System;
using System.Collections.Generic;
using System.Linq;
using CalculationEngine.Service.ActorModel.Commands;
using Common.Core.Events;

namespace CalculationEngine.Service.ActorModel.Actors
{
    public class InstrumentActor : IdempotentActor
    {
        private readonly IDictionary<DateTime, InstrumentPricingPublished> _pricings = new Dictionary<DateTime, InstrumentPricingPublished>();

        private readonly Guid _instrumentId;
        public override string PersistenceId => _instrumentId.ToString();

        public InstrumentActor(Guid instrumentId)
        {
            _instrumentId = instrumentId;
            IdempotentEvent<InstrumentPricingPublished>(Ignore, Recover);
            Command<SendMeInstrumentPricingPublished>(Handle);
        }

        private void Recover(InstrumentPricingPublished e)
        {
            if (!_pricings.TryGetValue(e.AsOfDate, out var pricing))
            {
                _pricings.Add(e.AsOfDate, e);
            }
            else if(pricing.AsAtDate < e.AsAtDate)
            {
                _pricings[e.AsOfDate] = e;
            }
        }

        private void Handle(SendMeInstrumentPricingPublished c)
        {
            foreach (var item in _pricings.OrderBy(x=> x.Key).Select(x=> x.Value))
            {
                Sender.Tell(item, Self);
            }
        }
    }
}
