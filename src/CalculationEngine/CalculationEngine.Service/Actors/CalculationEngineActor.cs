using Akka.Actor;
using Common.Core.Events;

namespace CalculationEngine.Service.ActorModel.Actors
{
    public class CalculationEngineActor : ReceiveActor
    {
        private readonly IActorRef _marketCurves = Context.ActorOf(Props.Create<MarketCurvesActor>(), "marketcurves");
        private readonly IActorRef _instruments = Context.ActorOf(Props.Create<InstrumentsActor>(), "instruments");

        public CalculationEngineActor()
        {
            Receive<Common.Core.Event>(e => 
            {
                switch (e)
                {
                    case CurvePointAdded _:
                    case CurveRecipeCreated _:
                        _marketCurves.Forward(e);
                        break;
                    case InstrumentPricingPublished _:
                        _marketCurves.Forward(e);
                        _instruments.Forward(e);
                        break;
                    default:
                        break;
                }
            });
        }
    }
}
