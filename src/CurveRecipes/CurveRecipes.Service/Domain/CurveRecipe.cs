using System;
using System.Linq;
using Common.Core;
using Common.Core.Events;

namespace CurveRecipes.Domain
{
    public class CurveRecipe : Aggregate<CurveRecipe>
    {
        static CurveRecipe()
        {
            RegisterApplyMethod<CurveRecipeCreated>(Apply);
        }

        private CurveRecipe()
        {
        }

        public CurveRecipe(Guid id, Guid marketCurveId, string shortName, string description, Tenor lastLiquidTenor, DayCountConvention dayCountConvention, Interpolation interpolation,
            ExtrapolationShort extrapolationShort, ExtrapolationLong extrapolationLong, OutputFrequency outputFrequency, OutputType outputType)
        {
            var @event = new CurveRecipeCreated(id, marketCurveId, shortName, description, lastLiquidTenor.ToString(), dayCountConvention.ToString(), interpolation.ToString(),
                extrapolationShort.ToString(), extrapolationLong.ToString(), outputFrequency.OutputSeries.ToString(), outputFrequency.MaximumMaturity.Value, outputType.ToString());

            ApplyEvent(@event);
        }

        public void AddTransformation(Order order, ITransformation transformation)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            if (transformation == null)
            {
                throw new ArgumentNullException(nameof(transformation));
            }

            switch (transformation)
            {
                case ParallelShock parallelShock:
                    var psEvent = new ParallelShockAdded(Id, order.Value, parallelShock.ShockTarget.ToString(), parallelShock.Shift.Value);
                    ApplyEvent(psEvent);
                    break;

                case KeyRateShock keyRateShock:
                    var krsEvent = new KeyRateShockAdded(Id, order.Value, keyRateShock.ShockTarget.ToString(), keyRateShock.Shift.Value, keyRateShock.Maturities.Select(m => m.Value).ToArray());
                    ApplyEvent(krsEvent);
                    break;

                default:
                    throw new ArgumentException($"Did not recognize transformation of type {transformation.GetType()}", nameof(transformation));
            }
        }

        private static void Apply(CurveRecipe curve, CurveRecipeCreated e)
        {
            curve.Id = e.Id;
        }
    }
}
