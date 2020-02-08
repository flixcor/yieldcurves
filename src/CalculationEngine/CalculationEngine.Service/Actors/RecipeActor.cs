using System;
using System.Linq;
using Akka.Actor;
using CalculationEngine.Domain;
using CalculationEngine.Service.ActorModel.Commands;
using CalculationEngine.Service.Domain;
using Common.Core;
using Common.Events;
using Common.EventStore.Lib;
using Common.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace CalculationEngine.Service.ActorModel.Actors
{
    public class RecipeActor : IdempotentActor
    {
        private readonly NonEmptyGuid _id;
        private IEventWrapper<ICurveRecipeCreated> _recipe;

        public RecipeActor(NonEmptyGuid id)
        {
            _id = id;

            IdempotentEvent<ICurveRecipeCreated>(Ignore, Recover);

            Command<Calculate>(Handle);
        }

        private void Handle(Calculate obj)
        {
            if (obj.CurvePoints.All(x => obj.Pricings.Any(y => y.Content.InstrumentId == x.Content.InstrumentId)))
            {
                var result = CurveCalculation.Calculate(obj.AsOfDate, _recipe, obj.CurvePoints.Select(x=> x.Content), obj.Pricings);
                var calc = new CurveCalculationResult().WithResult(_id, obj.AsOfDate, result);

                using var serviceScope = Context.CreateScope();
                var repo = serviceScope.ServiceProvider.GetService<IAggregateRepository>();
                repo.Save(calc).PipeTo(Self);
            }
        }

        private void Recover(IEventWrapper<ICurveRecipeCreated> e)
        {
            _recipe = e;
        }

        public override string PersistenceId => _id.ToString();
    }
}
