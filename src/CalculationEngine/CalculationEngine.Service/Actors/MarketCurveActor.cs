using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using CalculationEngine.Service.ActorModel.Commands;
using Common.Core.Events;

namespace CalculationEngine.Service.ActorModel.Actors
{
    public class MarketCurveActor : IdempotentActor
    {
        private readonly Dictionary<DateTime, IActorRef> _marketCurvesForDate = new Dictionary<DateTime, IActorRef>();
        private readonly Dictionary<Guid, IActorRef> _recipeActors = new Dictionary<Guid, IActorRef>();

        private readonly Dictionary<Guid, CurvePointAdded> _dateLags = new Dictionary<Guid, CurvePointAdded>();
        private readonly List<DateTime> _dateTimes = new List<DateTime>();
        private readonly List<Guid> _recipeIds = new List<Guid>();

        private readonly Guid _marketCurveId;
        public override string PersistenceId { get => _marketCurveId.ToString(); }

        public MarketCurveActor(Guid marketCurveId)
        {
            _marketCurveId = marketCurveId;

            IdempotentEvent<CurvePointAdded>(Handle, Recover);
            IdempotentEvent<InstrumentPricingPublished>(Handle, Recover);
            IdempotentEvent<CurveRecipeCreated>(Handle, Recover);

            Command<Calculate>(e => Handle(e));
        }

        #region Events
        #region CurvePointAdded
        private void Handle(CurvePointAdded e)
        {
            Context.ActorSelection("../../instruments").Tell(new SendMeInstrumentPricingPublished(e.InstrumentId));
        }

        private void Recover(CurvePointAdded e) => _dateLags.Add(e.InstrumentId, e);
        #endregion

        #region InstrumentPricingPublished
        private void Handle(InstrumentPricingPublished e)
        {
            if (_dateLags.TryGetValue(e.InstrumentId, out var point))
            {
                var max = -point.DateLag;

                for (var i = 0; i <= max; i++)
                {
                    var date = e.AsOfDate.Date.AddDays(i);

                    var actor = GetDateActor(date);

                    actor.Tell(e);
                    actor.Tell(new SendMeCalculate(_dateLags.Values));
                }
            }
        }

        private void Recover(InstrumentPricingPublished e)
        {
            if (_dateLags.TryGetValue(e.InstrumentId, out var point))
            {
                var max = -point.DateLag;

                for (var i = 0; i <= max; i++)
                {
                    var date = e.AsOfDate.Date.AddDays(i);

                    if (!_dateTimes.Any(x => x == date))
                    {
                        _dateTimes.Add(date);
                    }
                }
            }
        }
        #endregion

        #region CurveRecipeCreated
        private void Handle(CurveRecipeCreated e)
        {
            var recipeActor = GetRecipeActor(e.Id);
            recipeActor.Tell(e);

            foreach (var item in _dateTimes)
            {
                var dateActor = GetDateActor(item);
                dateActor.Tell(new SendMeCalculate(_dateLags.Values.ToList()), recipeActor);
            }
        }

        private void Recover(CurveRecipeCreated e)
        {
            _recipeIds.Add(e.Id);
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
        private IActorRef GetDateActor(DateTime asOfDate)
        {
            var date = asOfDate.Date;

            if (!_marketCurvesForDate.TryGetValue(date, out var marketCurveForDateActor))
            {
                marketCurveForDateActor = Context.ActorOf(Props.Create<MarketCurveForDateActor>(_marketCurveId, date), $"date-{asOfDate.ToString("yyyyMMdd")}");
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
