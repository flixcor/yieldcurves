using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Core.Events;
using Common.Infrastructure.EfCore;
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


        public Handler(GenericDbContext db)
        {
            _db = db;
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
        }

        public async Task Handle(InstrumentPricingPublished @event, CancellationToken cancellationToken)
        {
            var instrument = await _db.FindAsync<Instrument>(@event.InstrumentId);

            var dto = new Dto
            {
                AsAtDate = @event.AsAtDate,
                AsOfDate = @event.AsOfDate,
                Id = @event.Id,
                Instrument = instrument.Description,
                PriceType = @event.PriceType,
                PriceCurrency = @event.PriceCurrency,
                PriceAmount = @event.PriceAmount,
                Vendor = instrument.Vendor
            };

            _db.Add(dto);
        }

        public Task Handle(InstrumentCreated @event, CancellationToken cancellationToken)
        {
            _db.Add(new Instrument
            {
                Id = @event.Id,
                Description = @event.Description,
                Vendor = @event.Vendor
            });

            return Task.CompletedTask;
        }
    }
}
