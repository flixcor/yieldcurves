using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using CalculationEngine.Service.ActorModel.Commands;
using CalculationEngine.Service.Domain;
using Common.Core;
using Common.Events;

namespace CalculationEngine.Service.ActorModel.Actors
{
    public class MarketCurveActor : IdempotentActor
    {
        private readonly Dictionary<Date, IActorRef> _marketCurvesForDate = new Dictionary<Date, IActorRef>();
        private readonly Dictionary<Guid, IActorRef> _recipeActors = new Dictionary<Guid, IActorRef>();

        private readonly Dictionary<Guid, IEventWrapper<ICurvePointAdded>> _dateLags = new Dictionary<Guid, IEventWrapper<ICurvePointAdded>>();
        private readonly List<Date> _dates = new List<Date>();
        private readonly List<Guid> _recipeIds = new List<Guid>();

        private readonly Guid _marketCurveId;
        public override string PersistenceId { get => _marketCurveId.ToString(); }

        public MarketCurveActor(Guid marketCurveId)
        {
            _marketCurveId = marketCurveId;

            IdempotentEvent<ICurvePointAdded>(Handle, Recover);
            IdempotentEvent<IInstrumentPricingPublished>(Handle, Recover);
            IdempotentEvent<ICurveRecipeCreated>(Handle, Recover);

            Command<Calculate>(e => Handle(e));
        }

        #region Events
        #region CurvePointAdded
        private void Handle(IEventWrapper<ICurvePointAdded> e)
        {
            Context.ActorSelection("../../instruments").Tell(new SendMeInstrumentPricingPublished(e.Content.InstrumentId));
        }

        private void Recover(IEventWrapper<ICurvePointAdded> e) => _dateLags.Add(e.Content.InstrumentId, e);
        #endregion

        #region InstrumentPricingPublished
        private void Handle(IEventWrapper<IInstrumentPricingPublished> e)
        {
            if (_dateLags.TryGetValue(e.Content.InstrumentId, out var point))
            {
                var max = -point.Content.DateLag;

                for (var i = 0; i <= max; i++)
                {
                    var date = Date.FromString(e.Content.AsOfDate).AddDays(i);

                    var actor = GetDateActor(date);

                    actor.Tell(e);
                    actor.Tell(new SendMeCalculate(_dateLags.Values));
                }
            }
        }

        private void Recover(IEventWrapper<IInstrumentPricingPublished> e)
        {
            if (_dateLags.TryGetValue(e.Content.InstrumentId, out var point))
            {
                var max = -point.Content.DateLag;

                for (var i = 0; i <= max; i++)
                {
                    var date = Date.FromString(e.Content.AsOfDate).AddDays(i);

                    if (!_dates.Any(x => x == date))
                    {
                        _dates.Add(date);
                    }
                }
            }
        }
        #endregion

        #region CurveRecipeCreated
        private void Handle(IEventWrapper<ICurveRecipeCreated> e)
        {
            var recipeActor = GetRecipeActor(e.AggregateId);
            recipeActor.Tell(e);

            foreach (var item in _dates)
            {
                var dateActor = GetDateActor(item);
                dateActor.Tell(new SendMeCalculate(_dateLags.Values.ToList()), recipeActor);
            }
        }

        private void Recover(IEventWrapper<ICurveRecipeCreated> e)
        {
            _recipeIds.Add(e.AggregateId);
        }
        #endregion
        #endregion

        #region Commands
        private void Handle(Calculate e)
        {
            foreach (var item in _recipeIds)
            {
                var actor = GetRecipeActor(item);
                actor.Tell(e);
            }
        }
        #endregion

        #region UtilityMethods
        private IActorRef GetDateActor(Date asOfDate)
        {
            var date = asOfDate;

            if (!_marketCurvesForDate.TryGetValue(date, out var marketCurveForDateActor))
            {
                marketCurveForDateActor = Context.ActorOf(Props.Create<MarketCurveForDateActor>(_marketCurveId, date), $"date-{asOfDate}");
                _marketCurvesForDate.Add(asOfDate, marketCurveForDateActor);
            }

            return marketCurveForDateActor;
        }

        private IActorRef GetRecipeActor(Guid id)
        {
            if (!_recipeActors.TryGetValue(id, out var recipeActor))
            {
                recipeActor = Context.ActorOf(Props.Create<RecipeActor>(id), $"recipe-{id}");
                _recipeActors.Add(id, recipeActor);
            }

            return recipeActor;
        } 
        #endregion
    }
}
