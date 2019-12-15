using Akka.Actor;
using Common.Events;

namespace CalculationEngine.Service.ActorModel.Actors
{
    public class CalculationEngineActor : ReceiveActor
    {
        private readonly IActorRef _marketCurves = Context.ActorOf(Props.Create<MarketCurvesActor>(), "marketcurves");
        private readonly IActorRef _instruments = Context.ActorOf(Props.Create<InstrumentsActor>(), "instruments");

        public CalculationEngineActor()
        {
            Receive<Common.Core.IEvent>(e => 
            {
                switch (e)
                {
                    case ICurvePointAdded _:
                    case ICurveRecipeCreated _:
                        _marketCurves.Forward(e);
                        break;
                    case IInstrumentPricingPublished _:
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
