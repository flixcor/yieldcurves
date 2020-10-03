using System.Collections.Generic;
using System.Linq;
using Lib.AspNet;
using Lib.Domain;
using Lib.EventSourcing;
using Lib.Features;
using Vocab;
using static Lib.Domain.MarketCurve.Commands;
using static Lib.Domain.MarketCurve.Events;
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
                .Property(x => x.Name).Is(Schema.name);

            var getCurve = RestMapper.TryMapQuery<GetCurve, GetCurve.Curve?>(MarketCurveSingleUrl);

            var getCurveList = RestMapper.TryMapQuery<GetCurveList, CurveList>(MarketCurvesUrl, (curves) => new Dictionary<string, object>
            {
                ["curves"] = curves.Curves.Select(c => new Dictionary<string, object>
                {
                    ["@id"] = getCurve.WithRouteValues(new { id = c.Id }).Href,
                    [Schema.name] = c.Name
                })
            });

            var nameAndAddInstrument = RestMapper.TryMapCommand<MarketCurve.Aggregate, MarketCurve.State, NameAndAddInstrument>(MarketCurvesUrl, new NameAndAddInstrument(string.Empty, string.Empty));

            RestMapper.TryMapCommand<MarketCurve.Aggregate, MarketCurve.State, AddInstrument>(MarketCurveSingleUrl);
            

            RestMapper.TryMapIndex(getCurveList.Yield());

            EventMapper.TryMap<InstrumentAddedToCurve>("instrument added");
            EventMapper.TryMap<MarketCurveNamed>("market curve named");

            RestMapper.SetEventStore(eventStore);
        }
    }
}
