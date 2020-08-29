using System.Linq;
using ExampleService.Lib;
using Lib.Domain;
using Lib.EventSourcing;
using Lib.Features;
using Vocab;
using static Lib.Domain.MarketCurve.Commands;
using static Lib.Domain.MarketCurve.Events;
using static Lib.Features.GetCurve;
using static Lib.Features.GetCurveList;

namespace Lib
{
    public static class Bootstrap
    {
        private const string MarketCurvesUrl = "/marketcurves";
        private const string MarketCurveSingleUrl = MarketCurvesUrl + @"/{id}";

        public static void Setup(IEventStore eventStore)
        {
            Describe.Class<GetCurve.Curve>()
                .Property(x => x.Instruments).Are(Schema.instrument)
                .Property(x=> x.Name).Is(Schema.name);

            var getCurveList = RestMapper.TryMapQuery<GetCurveList, CurveList>(MarketCurvesUrl, (curves) => new
            {
                curves = curves.Curves.Select(c => new
                {
                    @Id = MarketCurvesUrl + "/" + c.Id,
                    c.Name
                })
            });

            var nameAndAddInstrument = RestMapper.TryMapCommand<MarketCurve.Aggregate, MarketCurve.State, NameAndAddInstrument>(
                MarketCurvesUrl)?.WithExpected(new NameAndAddInstrument()
            );

            RestMapper.TryMapCommand<MarketCurve.Aggregate, MarketCurve.State, AddInstrument>(MarketCurveSingleUrl);
            RestMapper.TryMapQuery<GetCurve, GetCurve.Curve>(MarketCurveSingleUrl);

            RestMapper.TryMapIndex(getCurveList.Yield());

            EventMapper.TryMap<InstrumentAddedToCurve>("instrument added");
            EventMapper.TryMap<MarketCurveNamed>("market curve named");

            RestMapper.SetEventStore(eventStore);
        }
    }
}
