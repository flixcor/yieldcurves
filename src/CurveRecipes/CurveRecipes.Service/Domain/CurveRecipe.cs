using System;
using System.Collections.Generic;
using System.Linq;
using Common.Core;
using Common.Events;
using static Common.Events.Create;

namespace CurveRecipes.Domain
{
    public class CurveRecipe : Aggregate<CurveRecipe>
    {
        static CurveRecipe()
        {
            RegisterApplyMethod<ICurveRecipeCreated>(Apply);
            RegisterApplyMethod<IKeyRateShockAdded>(Apply);
            RegisterApplyMethod<IParallelShockAdded>(Apply);
        }

        private int _count = 0;

        private CurveRecipe()
        {
        }

        public static Result<CurveRecipe> TryCreate(Guid id, Guid marketCurveId, string shortName, string description, Tenor lastLiquidTenor, DayCountConvention dayCountConvention, Interpolation interpolation,
            ExtrapolationShort extrapolationShort, ExtrapolationLong extrapolationLong, OutputFrequency outputFrequency, OutputType outputType)
        {
            var errors = new List<string>();

            if (id.Equals(Guid.Empty))
            {
                errors.Add($"{nameof(id)} cannot be empty");
            }

            if (marketCurveId.Equals(Guid.Empty))
            {
                errors.Add($"{nameof(marketCurveId)} cannot be empty");
            }
            
            if (string.IsNullOrWhiteSpace(shortName))
            {
                errors.Add($"{nameof(shortName)} cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                errors.Add($"{nameof(description)} cannot be empty");
            }

            if (outputFrequency == null)
            {
                errors.Add($"{nameof(outputFrequency)} cannot be empty");
            }

            if (errors.Any())
            {
                return Result.Fail<CurveRecipe>(errors.ToArray());
            }

            return Result.Ok(new CurveRecipe(id, marketCurveId, shortName, description, lastLiquidTenor, dayCountConvention, interpolation,
            extrapolationShort, extrapolationLong, outputFrequency, outputType));
        }

        private CurveRecipe(Guid id, Guid marketCurveId, string shortName, string description, Tenor lastLiquidTenor, DayCountConvention dayCountConvention, Interpolation interpolation,
            ExtrapolationShort extrapolationShort, ExtrapolationLong extrapolationLong, OutputFrequency outputFrequency, OutputType outputType)
        {
            var @event = CurveRecipeCreated(id, marketCurveId, shortName, description, lastLiquidTenor.ToString(), dayCountConvention.ToString(), interpolation.ToString(),
                extrapolationShort.ToString(), extrapolationLong.ToString(), outputFrequency.OutputSeries.ToString(), outputFrequency.MaximumMaturity.Value, outputType.ToString());

            ApplyEvent(@event);
        }

        public Result AddTransformation(ITransformation transformation, Order order = null)
        {
            order ??= new Order(_count + 1);

            if (transformation == null)
            {
                return Result.Fail($"{nameof(transformation)} cannot be empty");
            }

            switch (transformation)
            {
                case ParallelShock parallelShock:
                    var psEvent = ParallelShockAdded(Id, order.Value, parallelShock.ShockTarget.ToString(), parallelShock.Shift.Value);
                    ApplyEvent(psEvent);
                    break;

                case KeyRateShock keyRateShock:
                    var krsEvent = KeyRateShockAdded(Id, order.Value, keyRateShock.ShockTarget.ToString(), keyRateShock.Shift.Value, keyRateShock.Maturities.Select(m => m.Value).ToArray());
                    ApplyEvent(krsEvent);
                    break;

                default:
                    return Result.Fail($"Did not recognize {nameof(transformation)} of type {transformation?.GetType()}");
            }

            return Result.Ok();
        }

        private static void Apply(CurveRecipe curve, ICurveRecipeCreated e)
        {
            curve.Id = e.AggregateId;
        }

        private static void Apply(CurveRecipe curve, IKeyRateShockAdded e)
        {
            curve._count++;
        }

        private static void Apply(CurveRecipe curve, IParallelShockAdded e)
        {
            curve._count++;
        }
    }
}
