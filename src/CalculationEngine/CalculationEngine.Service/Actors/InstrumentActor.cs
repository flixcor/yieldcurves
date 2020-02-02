using System;
using System.Collections.Generic;
using System.Linq;
using CalculationEngine.Service.ActorModel.Commands;
using Common.Core;
using Common.Events;

namespace CalculationEngine.Service.ActorModel.Actors
{
    public class InstrumentActor : IdempotentActor
    {
        private readonly IDictionary<string, IEventWrapper<IInstrumentPricingPublished>> _pricings = new Dictionary<string, IEventWrapper<IInstrumentPricingPublished>>();

        private readonly Guid _instrumentId;
        public override string PersistenceId => _instrumentId.ToString();

        public InstrumentActor(Guid instrumentId)
        {
            _instrumentId = instrumentId;
            IdempotentEvent<IInstrumentPricingPublished>(Ignore, Recover);
            Command<SendMeInstrumentPricingPublished>(Handle);
        }

        private void Recover(IEventWrapper<IInstrumentPricingPublished> wrapper)
        {
            var e = wrapper.Content;

            if (!_pricings.TryGetValue(e.AsOfDate, out var pricing))
            {
                _pricings.Add(e.AsOfDate, wrapper);
            }
            else if(pricing.Content.AsAtDate < e.AsAtDate)
            {
                _pricings[e.AsOfDate] = wrapper;
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
