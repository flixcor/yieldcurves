using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Util.Internal;
using Common.Core;
using Common.Events;

namespace CalculationEngine.Service.ActorModel.Actors
{
    public class MarketCurvesActor : ReceiveActor
    {
        private readonly Dictionary<Guid, IActorRef> _marketCurves = new Dictionary<Guid, IActorRef>();

        public MarketCurvesActor()
        {
            Receive<IEventWrapper<ICurvePointAdded>>(e => GetMarketCurve(e.Metadata.AggregateId).Tell(e));
            Receive<IEventWrapper<ICurveRecipeCreated>>(e => GetMarketCurve(e.GetContent().MarketCurveId).Tell(e));
            Receive<IEventWrapper<IInstrumentPricingPublished>>(e =>
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
