using System;
using CurveRecipes.Domain;
using Common.Core.Events;
using Common.Tests;
using NUnit.Framework;

namespace CurveRecipes.Engine.Tests
{
    public class UT_CurveCalculatorTests : SagaTest<CurveCalculator>
    {
        [Test]
        public void UT_created()
        {
            var id = Guid.NewGuid();
            var marketCurveId = Guid.NewGuid();
            var asOfDate = DateTime.Now.Date;

            WhenCreated(() => new CurveCalculator(id, marketCurveId, asOfDate));
            Then(new CurveCalculatorCreated(id, marketCurveId, asOfDate));
        }

        [Test]
        public void UT_calculate_failedEvent_when_invalid_values()
        {
            var id = Guid.NewGuid();
            var marketCurveId = Guid.NewGuid();
            var asOfDate = DateTime.Now.Date;

            var e1 = new InstrumentPricingPublished(Guid.NewGuid(), asOfDate, DateTime.Now, Guid.NewGuid(), "", 0);
            var e2 = new CurvePointAdded(id, "", e1.InstrumentId, 0, true, "");
            var e3 = new CurveRecipeCreated(Guid.NewGuid(), id, "", "", "", "", "", "", "", "", 100, "");

            Given(new CurveCalculatorCreated(id, marketCurveId, asOfDate));
            WhenTransitioned(e1, e2, e3);
            ThenTransitioned(e1, e2, e3);
            Then<CurveCalculationFailed>();
        }

        [Test]
        public void UT_calculate_calculatedEvent_when_valid_values()
        {
            var id = Guid.NewGuid();
            var marketCurveId = Guid.NewGuid();
            var asOfDate = DateTime.Now.Date;

            var e7 = new InstrumentPricingPublished(Guid.NewGuid(), asOfDate, DateTime.Now, Guid.NewGuid(), "EUR", 5);
            var e1 = new CurvePointAdded(marketCurveId, Tenor.Y06.ToString(), e7.InstrumentId, 0, true, "");
            var e2 = new CurveRecipeCreated(Guid.NewGuid(), id, "", "", Tenor.Y51.ToString(), DayCountConvention.ActualActual.ToString(), Interpolation.Linear.ToString(), ExtrapolationShort.Zero.ToString(), ExtrapolationLong.Flat.ToString(), OutputSeries.Annual.ToString(), 100, OutputType.ZeroCoupon.ToString());
            var e5 = new InstrumentPricingPublished(Guid.NewGuid(), asOfDate, DateTime.Now, Guid.NewGuid(), "EUR", 6);
            var e6 = new InstrumentPricingPublished(Guid.NewGuid(), asOfDate, DateTime.Now, Guid.NewGuid(), "EUR", 30);
            var e3 = new CurvePointAdded(marketCurveId, Tenor.Y07.ToString(), e5.InstrumentId, 0, true, "");
            var e4 = new CurvePointAdded(marketCurveId, Tenor.Y50.ToString(), e6.InstrumentId, 0, true, "");
            
            Given(new CurveCalculatorCreated(id, marketCurveId, asOfDate));
            WhenTransitioned(e1, e2, e3, e4, e5, e6, e7);
            ThenTransitioned(e1, e2, e3, e4, e5, e6, e7);
            Then<CurveCalculated>();
        }
    }
}
