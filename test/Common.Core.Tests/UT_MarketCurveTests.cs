using System;
using System.Threading.Tasks;
using Common.Tests;
using MarketCurves.Domain;
using MarketCurves.Service.Domain;
using NUnit.Framework;
using static Common.Events.Helpers;

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

            When(c => c.Define(country, type))
                .Then(MarketCurveCreated(country.ToString(), type.ToString()));
        }

        [Test]
        public async Task UT_MarketCurve_AddCurvePoint_happy_flow()
        {
            var id = Guid.NewGuid();
            var tenor = Tenor.FRA10x16;
            var instrument = await Instrument.FromId(Guid.NewGuid(), (id) => Task.FromResult(true));
            var dateLag = new DateLag(-1);
            var priceType = PriceType.BIDPRICE;
            var isMandatory = false;

            Given(MarketCurveCreated(Country.GB.ToString(), CurveType.ECB.ToString()))
                .When(c => c.AddCurvePoint(tenor, instrument, dateLag, priceType, isMandatory))
                    .Then(CurvePointAdded(tenor.ToString(), instrument.Id, dateLag.Value, isMandatory, priceType.ToString()));
        }
    }
}
