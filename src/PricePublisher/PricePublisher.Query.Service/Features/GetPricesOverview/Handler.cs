using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Events;
using Common.Infrastructure.EfCore;
using Common.Infrastructure.SignalR;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace PricePublisher.Query.Service.Features.GetPricesOverview
{
    public class Handler :
            IHandleQuery<Query, IEnumerable<Dto>>,
            IHandleEvent<InstrumentCreated>,
            IHandleEvent<InstrumentPricingPublished>
    {
        private const string GetPricesOverView = nameof(GetPricesOverview);
        private const string AsAtDate = nameof(Query.AsAtDate);
        private const string Instrument = nameof(Instrument);
        private const string PriceType = nameof(PriceType);


        private readonly GenericDbContext _db;
        private readonly ISocketContext _socketContext;

        public Handler(GenericDbContext db, ISocketContext socketContext)
        {
            _db = db;
            _socketContext = socketContext;
        }

        public Task<IEnumerable<Dto>> Handle(Query query, CancellationToken cancellationToken)
        {
            var connection = _db.Database.GetDbConnection();

            var asAtDatePart = query.AsAtDate.HasValue
                ? $"AsAtDate <= '{query.AsAtDate.Value.ToString("yyyy-MM-dd HH:mm:ss.fff")}'"
                : "1 = 1";

            var asOfDatePart = query.AsOfDate.HasValue
                ? $"AsOfDate = '{query.AsOfDate.Value.Date.ToString("yyyy-MM-dd")}'"
                : "1 = 1";

            var querystring = @$"SELECT dto1.*
                                    FROM [PricePublisherQuery].[dbo].[{GetPricesOverView}_Dto] dto1
                                    inner join (select {Instrument}, Max({AsAtDate}) {AsAtDate} 
                                                from [dbo].[{GetPricesOverView}_Dto] 
                                                WHERE {asAtDatePart} AND {asOfDatePart}
                                                group by {Instrument}, {PriceType}
                                    ) as dto2
                                    on dto1.{AsAtDate} = dto2.{AsAtDate} AND dto1.{Instrument} = dto2.{Instrument}";

            return connection.QueryAsync<Dto>(querystring);

            /*
                alternative using LINQ, probably a little less performant:


                IQueryable<Dto> inputQueryable = _db.Set<Dto>();

                if (query.AsOfDate.HasValue)
                {
                    inputQueryable = inputQueryable.Where(x=> x.AsOfDate.Date == query.AsOfDate.Value.Date);
                }

                if (query.AsAtDate.HasValue)
                {
                    inputQueryable = inputQueryable.Where(x=> x.AsAtDate <= query.AsAtDate.Value);
                }

                var maxes = inputQueryable
                    .GroupBy(x=> new {x.Instrument, x.PriceType, x.AsOfDate})
                    .Select(x=> new {x.Key.Instrument, x.Key.PriceType, x.Key.AsOfDate, AsAtDate = x.Max(x=> x.AsAtDate)});

                var joined = _db.Set<Dto>()
                    .Join(
                        inner: maxes,
                        outerKeySelector: x=> new {x.Instrument, x.PriceType, x.AsOfDate.Date, x.AsAtDate },
                        innerKeySelector: x=> new {x.Instrument, x.PriceType, x.AsOfDate.Date, x.AsAtDate },
                        resultSelector: (x, y)=> x
                        );

                return await joined.ToListAsync();

             */
        }

        public async Task Handle(InstrumentPricingPublished @event, CancellationToken cancellationToken)
        {
            var instrument = await _db.FindAsync<Instrument>(@event.InstrumentId);

            var dto = new Dto
            {
                AsAtDate = @event.AsAtDate,
                AsOfDate = @event.AsOfDate,
                Id = @event.AggregateId,
                Instrument = instrument.Description,
                PriceType = @event.PriceType,
                PriceCurrency = @event.PriceCurrency,
                PriceAmount = @event.PriceAmount,
                Vendor = instrument.Vendor
            };
            var task = _socketContext.SendToGroup(nameof(GetPricesOverView), dto);
            _db.Add(dto);
            await task;
        }

        public Task Handle(InstrumentCreated @event, CancellationToken cancellationToken)
        {
            _db.Add(new Instrument
            {
                Id = @event.AggregateId,
                Description = @event.Description,
                Vendor = @event.Vendor
            });

            return Task.CompletedTask;
        }
    }
}
