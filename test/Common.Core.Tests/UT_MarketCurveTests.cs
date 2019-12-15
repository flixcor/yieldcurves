using System;
using Common.Tests;
using MarketCurves.Domain;
using NUnit.Framework;
using static Common.Events.Create;

namespace UnitTests
{
    public class UT_MarketCurveTests : AggregateTest<MarketCurve>
    {
        [Test]
        public void UT_new_marketCurve_generates_matching_event()
        {
            var id = Guid.NewGuid();
            var country = Country.GB;
            var type = CurveType.BONDSPREAD;

            WhenCreated(() => MarketCurve.TryCreate(id, country, type).Content);
            Then(MarketCurveCreated(id, country.ToString(), type.ToString()));
        }

        [Test]
        public void UT_MarketCurve_AddCurvePoint_happy_flow()
        {
            var id = Guid.NewGuid();
            var tenor = Tenor.FRA10x16;
            var instrumentId = Guid.NewGuid();
            var dateLag = new DateLag(-1);
            var priceType = PriceType.BIDPRICE;
            var isMandatory = false;

            Given(MarketCurveCreated(id, Country.GB.ToString(), CurveType.ECB.ToString()));
            When(c => c.AddCurvePoint(tenor, instrumentId, dateLag, priceType, isMandatory));
            Then(CurvePointAdded(id, tenor.ToString(), instrumentId, dateLag.Value, isMandatory, priceType.ToString()));
        }
    }
}
