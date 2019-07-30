using Common.Core;
using Common.Core.Events;
using Common.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MarketCurves.Query.Service.Features
{
    public class GetMarketCurve
    {
        public class Query : IQuery<Maybe<Dto>>
        {
            public Guid Id { get; set; }
        }

        public class Dto : ReadObject
        {
            public string Name { get; set; }
            public IEnumerable<PointDto> CurvePoints { get; set; } = new List<PointDto>();
        }

        public class PointDto
        {
            public Guid InstrumentId { get; set; }
            public string Tenor { get; set; }
            public string Vendor { get; set; }
            public string Name { get; set; }
            public short DateLag { get; set; }
            public bool IsMandatory { get; set; }
            public string PriceType { get; set; }
        }

        public class Handler :
            IHandleQuery<Query, Maybe<Dto>>,
            IHandleEvent<MarketCurveCreated>,
            IHandleEvent<CurvePointAdded>,
            IHandleEvent<BloombergInstrumentCreated>,
            IHandleEvent<RegularInstrumentCreated>
        {
            private readonly IReadModelRepository<Dto> _curveRepo;
            private readonly IReadModelRepository<InstrumentDto> _instrumentRepo;

            public Handler(IReadModelRepository<Dto> curveRepo, IReadModelRepository<InstrumentDto> instrumentRepo)
            {
                _curveRepo = curveRepo ?? throw new ArgumentNullException(nameof(curveRepo));
                _instrumentRepo = instrumentRepo ?? throw new ArgumentNullException(nameof(instrumentRepo));
            }

            public Task<Maybe<Dto>> Handle(Query query, CancellationToken cancellationToken)
            {
                return _curveRepo.Get(query.Id);
            }

            public Task Handle(MarketCurveCreated @event, CancellationToken cancellationToken)
            {
                var dto = new Dto
                {
                    Id = @event.Id,
                    Name = GenerateName(@event)
                };

                return _curveRepo.Insert(dto);
            }

            public async Task Handle(CurvePointAdded @event, CancellationToken cancellationToken)
            {
                var curveResult = await _curveRepo.Get(@event.Id).ToResult();
                var instrumentResult = await _instrumentRepo.Get(@event.InstrumentId).ToResult();

                await Result
                    .Combine(curveResult, instrumentResult)
                    .Promise(() => 
                    {
                        var curve = curveResult.Content;
                        var instrument = instrumentResult.Content;

                        var points = curve.CurvePoints.ToList();
                        points.Add(new PointDto
                        {
                            InstrumentId = @event.InstrumentId,
                            DateLag = @event.DateLag,
                            IsMandatory = @event.IsMandatory,
                            Tenor = @event.Tenor,
                            PriceType = @event.PriceType,
                            Name = instrument.Name,
                            Vendor = instrument.Vendor
                        });

                        curve.CurvePoints = points;

                        return _curveRepo.Update(curve);
                    });
            }

            public Task Handle(BloombergInstrumentCreated @event, CancellationToken cancellationToken)
            {
                var instrument = new InstrumentDto
                {
                    Id = @event.Id,
                    Name = $"{@event.Ticker} {@event.PricingSource} {@event.YellowKey}",
                    Vendor = "Bloomberg"
                };

                return _instrumentRepo.Insert(instrument);
            }

            public Task Handle(RegularInstrumentCreated @event, CancellationToken cancellationToken)
            {
                var instrument = new InstrumentDto
                {
                    Id = @event.Id,
                    Name = @event.Description,
                    Vendor = @event.Vendor
                };

                return _instrumentRepo.Insert(instrument);
            }

            private string GenerateName(MarketCurveCreated @event)
            {
                var stringBuilder = new StringBuilder("M");

                stringBuilder.AppendFormatNonEmptyString("_{0}", @event.Country, @event.CurveType, @event.FloatingLeg);

                return stringBuilder.ToString();
            }
        }
    }

    public class InstrumentDto : ReadObject
    {
        public string Vendor { get; set; }
        public string Name { get; set; }
    }
}
