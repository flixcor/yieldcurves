using System.Collections.Generic;
using System.Linq;
using Lib.AspNet;
using Lib.Domain;
using Lib.EventSourcing;
using Lib.Features;
using Vocab;
using static Lib.Domain.MarketCurve.Events;
using static Lib.Features.GetCurveList;
using static Contracts.ContractCollection;

namespace Lib
{
    public static class Bootstrap
    {
        private const string MarketCurvesUrl = "/marketcurves";
        private const string MarketCurveSingleUrl = MarketCurvesUrl + @"/{id}";
        private const string AddInstrumentUrl = MarketCurveSingleUrl + "/instruments";

        public static void Setup(IEventStore eventStore)
        {
            Contracts.ContractCollection.Setup();

            Describe.Class<GetCurve.Curve>()
                .Property(x => x.Instruments).Are(Schema.instrument)
                .Property(x => x.Name).Is(Schema.name);

            var getCurve = RestMapper.TryMapQuery<GetCurve, GetCurve.Curve?>(MarketCurveSingleUrl, (c) => c is null ? null : new Dictionary<string, object> 
            { 
                ["@id"] = MarketCurveSingleUrl.Replace(@"{id}", c.Id),
                [Schema.name] = c.Name,
                ["instruments"] = c.Instruments,
            });

            var getCurveList = RestMapper.TryMapQuery<GetCurveList, CurveList>(MarketCurvesUrl, (curves) => new Dictionary<string, object>
            {
                ["curves"] = curves.Curves.Select(c => new Dictionary<string, object>
                {
                    ["@id"] = getCurve.WithRouteValues(new { id = c.Id }).Href,
                    [Schema.name] = c.Name
                })
            });

            var nameAndAddInstrument = RestMapper.TryMapCommand<MarketCurve.Aggregate, MarketCurve.State, NameAndAddInstrument>(MarketCurvesUrl, new NameAndAddInstrument(string.Empty, string.Empty));

            RestMapper.TryMapCommand<MarketCurve.Aggregate, MarketCurve.State, AddInstrument>(AddInstrumentUrl, new AddInstrument(string.Empty));
            

            RestMapper.TryMapIndex(getCurveList.Yield());

            EventMapper.TryMap<InstrumentAddedToCurve>("instrument added");
            EventMapper.TryMap<MarketCurveNamed>("market curve named");

            RestMapper.SetEventStore(eventStore);
        }
    }
}
