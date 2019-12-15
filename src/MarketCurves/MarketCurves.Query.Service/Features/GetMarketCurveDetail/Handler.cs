﻿using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Events;
using Common.Core.Extensions;

namespace MarketCurves.Query.Service.Features.GetMarketCurveDetail
{
    public class Handler :
            IHandleQuery<Query, Maybe<Dto>>,
            IHandleEvent<IMarketCurveCreated>,
            IHandleEvent<ICurvePointAdded>,
            IHandleEvent<IBloombergInstrumentCreated>,
            IHandleEvent<IRegularInstrumentCreated>
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

        public Task Handle(IMarketCurveCreated @event, CancellationToken cancellationToken)
        {
            var dto = new Dto
            {
                Id = @event.AggregateId,
                Name = GenerateName(@event)
            };

            return _curveRepo.Insert(dto);
        }

        public async Task Handle(ICurvePointAdded @event, CancellationToken cancellationToken)
        {
            var curveResult = await _curveRepo.Get(@event.AggregateId).ToResult();
            var instrumentResult = await _instrumentRepo.Get(@event.InstrumentId).ToResult();

            await Result
                .Combine(curveResult, instrumentResult, (c, i) =>
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

                    curve.CurvePoints = points.OrderBy(x=> x.Tenor).ToList();

                    return _curveRepo.Update(curve);
                });
        }

        public Task Handle(IBloombergInstrumentCreated @event, CancellationToken cancellationToken)
        {
            var instrument = new InstrumentDto
            {
                Id = @event.AggregateId,
                Name = $"{@event.Ticker} {@event.PricingSource} {@event.YellowKey}",
                Vendor = "Bloomberg"
            };

            return _instrumentRepo.Insert(instrument);
        }

        public Task Handle(IRegularInstrumentCreated @event, CancellationToken cancellationToken)
        {
            var instrument = new InstrumentDto
            {
                Id = @event.AggregateId,
                Name = @event.Description,
                Vendor = @event.Vendor
            };

            return _instrumentRepo.Insert(instrument);
        }

        private string GenerateName(IMarketCurveCreated @event)
        {
            var stringBuilder = new StringBuilder("M");

            stringBuilder.AppendFormatNonEmptyString("_{0}", @event.Country, @event.CurveType, @event.FloatingLeg);

            return stringBuilder.ToString();
        }
    }
}
