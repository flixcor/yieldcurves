using System.Linq;
using Common.Core;
using Common.Events;
using Common.EventStore.Lib;
using static Common.Core.Result;
using static Common.Events.Helpers;

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
            var @event = CurveRecipeCreated(marketCurveId, shortName, description, lastLiquidTenor.NonEmptyString(), dayCountConvention.NonEmptyString(), interpolation.NonEmptyString(), extrapolationShort.NonEmptyString(),
                extrapolationLong.NonEmptyString(), outputFrequency.OutputSeries.NonEmptyString(), outputFrequency.MaximumMaturity.Value, outputType.NonEmptyString());

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
