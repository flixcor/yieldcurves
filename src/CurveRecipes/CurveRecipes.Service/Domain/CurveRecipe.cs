using System.Linq;
using Common.Core;
using Common.Events;
using Common.EventStore.Lib;
using static Common.Events.Helpers;

namespace CurveRecipes.Domain
{
    public class CurveRecipe : Aggregate
    {
        private int _count = 0;

        public CurveRecipe Define(NonEmptyGuid marketCurveId, NonEmptyString shortName, NonEmptyString description, Tenor lastLiquidTenor, DayCountConvention dayCountConvention, Interpolation interpolation, ExtrapolationShort extrapolationShort,
            ExtrapolationLong extrapolationLong, OutputFrequency outputFrequency, OutputType outputType)
        {
            var @event = CurveRecipeCreated(marketCurveId, shortName, description, lastLiquidTenor, dayCountConvention, interpolation, extrapolationShort,
                extrapolationLong, outputFrequency.OutputSeries, outputFrequency.MaximumMaturity.Value, outputType);

            GenerateEvent(@event);

            return this;
        }

        public Either<Error, CurveRecipe> AddTransformation(ITransformation transformation, Order? order = null)
        {
            order ??= new Order(_count + 1);

            switch (transformation)
            {
                case ParallelShock parallelShock:
                    var psEvent = ParallelShockAdded(order.Value, parallelShock.ShockTarget.ToString(), parallelShock.Shift.Value);
                    GenerateEvent(psEvent);
                    return this;

                case KeyRateShock keyRateShock:
                    var krsEvent = KeyRateShockAdded(order.Value, keyRateShock.ShockTarget.ToString(), keyRateShock.Shift.Value, keyRateShock.Maturities.Select(m => m.Value).ToArray());
                    GenerateEvent(krsEvent);
                    return this;

                default:
                    return new Error($"Did not recognize {nameof(transformation)} of type {transformation?.GetType()}");
            }
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
