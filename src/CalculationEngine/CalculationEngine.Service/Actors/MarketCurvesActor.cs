using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Util.Internal;
using Common.Events;

namespace CalculationEngine.Service.ActorModel.Actors
{
    public class MarketCurvesActor : ReceiveActor
    {
        private readonly Dictionary<Guid, IActorRef> _marketCurves = new Dictionary<Guid, IActorRef>();

        public MarketCurvesActor()
        {
            Receive<CurvePointAdded>(e => GetMarketCurve(e.AggregateId).Tell(e));
            Receive<CurveRecipeCreated>(e => GetMarketCurve(e.MarketCurveId).Tell(e));
            Receive<InstrumentPricingPublished>(e =>
            _marketCurves.Values.ForEach(c=> c.Tell(e))
            );
        }

        private IActorRef GetMarketCurve(Guid id)
        {
            if (!_marketCurves.TryGetValue(id, out var marketCurveActor))
            {
                marketCurveActor = Context.ActorOf(Props.Create<MarketCurveActor>(id), id.ToString());
                _marketCurves.Add(id, marketCurveActor);
            }

            return marketCurveActor;
        }
    }
}
