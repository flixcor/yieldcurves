using System;
using System.Collections.Generic;
using Akka.Actor;
using CalculationEngine.Service.ActorModel.Commands;
using Common.Events;

namespace CalculationEngine.Service.ActorModel.Actors
{
    public class InstrumentsActor : ReceiveActor
    {
        private readonly Dictionary<Guid, IActorRef> _instruments = new Dictionary<Guid, IActorRef>();

        public InstrumentsActor()
        {
            Receive<SendMeInstrumentPricingPublished>(x => 
                GetInstrument(x.InstrumentId).Forward(x));
            Receive<InstrumentPricingPublished>(x => GetInstrument(x.InstrumentId).Tell(x));
        }

        private IActorRef GetInstrument(Guid id)
        {
            if (!_instruments.TryGetValue(id, out var marketCurveActor))
            {
                marketCurveActor = Context.ActorOf(Props.Create<InstrumentActor>(id));
                _instruments.Add(id, marketCurveActor);
            }

            return marketCurveActor;
        }
    }
}
