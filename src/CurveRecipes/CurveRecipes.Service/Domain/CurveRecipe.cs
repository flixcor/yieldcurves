using System;
using System.Collections.Generic;
using System.Linq;
using Common.Core;
using Common.Events;
using Common.EventStore.Lib;
using static Common.Events.Helpers;
using static Common.Core.Result;

namespace CurveRecipes.Domain
{
    public class CurveRecipe : Aggregate<CurveRecipe>
    {
        private int _count = 0;

        public CurveRecipe()
        {
        }

        public CurveRecipe Define(NonEmptyGuid marketCurveId, NonEmptyString shortName, NonEmptyString description, Tenor lastLiquidTenor, DayCountConvention dayCountConvention, Interpolation interpolation, ExtrapolationShort extrapolationShort,
            ExtrapolationLong extrapolationLong, OutputFrequency outputFrequency, OutputType outputType)
        {
            var @event = CurveRecipeCreated(marketCurveId, shortName, description, lastLiquidTenor.ToString(), dayCountConvention.ToString(), interpolation.ToString(), extrapolationShort.ToString(),
                extrapolationLong.ToString(), outputFrequency.OutputSeries.ToString(), outputFrequency.MaximumMaturity.Value, outputType.ToString());

            GenerateEvent(@event);

            return this;
        }

        public Result<CurveRecipe> AddTransformation(ITransformation transformation, Order? order = null)
        {
            order ??= new Order(_count + 1);

            switch (transformation)
            {
                case ParallelShock parallelShock:
                    var psEvent = ParallelShockAdded(order.Value, parallelShock.ShockTarget.ToString(), parallelShock.Shift.Value);
                    GenerateEvent(psEvent);
                    break;

                case KeyRateShock keyRateShock:
                    var krsEvent = KeyRateShockAdded(order.Value, keyRateShock.ShockTarget.ToString(), keyRateShock.Shift.Value, keyRateShock.Maturities.Select(m => m.Value).ToArray());
                    GenerateEvent(krsEvent);
                    break;

                default:
                    return Fail<CurveRecipe>($"Did not recognize {nameof(transformation)} of type {transformation?.GetType()}");
            }

            return Ok(this);
        }

        protected override void When(IEvent @event)
        {
            switch (@event)
            {
                case IKeyRateShockAdded _:
                case IParallelShockAdded _:
                    _count++;
                    break;
            }
        }
    }
}
