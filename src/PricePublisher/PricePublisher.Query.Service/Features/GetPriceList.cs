using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Core.Events;
using Common.Infrastructure.EfCore;
using Microsoft.EntityFrameworkCore;

namespace PricePublisher.Query.Service.Features
{
    public class GetPriceList : IQuery<GetPriceList.Result>
    {
        public DateTime? AsOfDate { get; set; }
        public DateTime? AsAtDate { get; set; }

        public class Result
        {
            public IEnumerable<Dto> Prices { get; set; } = Enumerable.Empty<Dto>();
        }

        public class Dto : ReadObject
        {
            public DateTime AsOfDate { get; set; }
            public DateTime AsAtDate { get; set; }
            public string Vendor { get; set; }
            public string Instrument { get; set; }
            public string PriceType { get; set; }
            public string PriceCurrency { get; set; }
            public double PriceAmount { get; set; }
        }

        public class Handler :
            IHandleQuery<GetPriceList, Result>,
            IHandleEvent<InstrumentCreated>,
            IHandleEvent<InstrumentPricingPublished>
        {
            private readonly GenericDbContext _db;

            public Handler(GenericDbContext db)
            {
                _db = db;
            }

            public async Task<Result> Handle(GetPriceList query, CancellationToken cancellationToken)
            {
                var efQuery = _db.Set<Dto>() as IQueryable<Dto>;

                if (query.AsAtDate.HasValue)
                {
                    efQuery = efQuery.Where(x => x.AsAtDate <= query.AsAtDate.Value);
                }

                if (query.AsOfDate.HasValue)
                {
                    efQuery = efQuery.Where(x => x.AsOfDate.Date == query.AsOfDate.Value.Date);
                }
                
                var result = await efQuery
                    .GroupBy(x => x.Instrument)
                    .Select(x => x.OrderByDescending(y => y.AsAtDate).FirstOrDefault())
                    .ToListAsync(cancellationToken);

                return new Result { Prices = result };
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

                await _db.SaveChangesAsync(cancellationToken);
            }

            public Task Handle(InstrumentCreated @event, CancellationToken cancellationToken)
            {
                _db.Add(new Instrument
                {
                    Id = @event.Id,
                    Description = @event.Description,
                    Vendor = @event.Vendor
                });

                return _db.SaveChangesAsync(cancellationToken);
            }
        }
    }

    public class Instrument : ReadObject
    {
        public string Vendor { get; set; }
        public string Description { get; set; }
    }
}
