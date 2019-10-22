using System;
using System.Linq;
using Akka.Actor;
using CalculationEngine.Domain;
using CalculationEngine.Service.ActorModel.Commands;
using CalculationEngine.Service.Domain;
using Common.Core;
using Common.Events;
using Common.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace CalculationEngine.Service.ActorModel.Actors
{
    public class RecipeActor : IdempotentActor
    {
        private readonly Guid _id;
        private CurveRecipeCreated _recipe;

        public RecipeActor(Guid id)
        {
            _id = id;

            IdempotentEvent<CurveRecipeCreated>(Ignore, Recover);

            Command<Calculate>(Handle);
        }

        private void Handle(Calculate obj)
        {
            if (obj.CurvePoints.All(x=> obj.Pricings.Any(y=> y.InstrumentId == x.InstrumentId)))
            {
                var result = CurveCalculation.Calculate(obj.AsOfDate, _recipe, obj.CurvePoints, obj.Pricings);
                var calc = new CurveCalculationResult(Guid.NewGuid(), _id, obj.AsOfDate, result);

                using var serviceScope = Context.CreateScope();
                var repo = serviceScope.ServiceProvider.GetService<IRepository>();
                repo.SaveAsync(calc).PipeTo(Self); 
            }
        }

        private void Recover(CurveRecipeCreated e)
        {
            _recipe = e;
        }

        public override string PersistenceId => _id.ToString();
    }
}
