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
            IHandleQuery<Query, Dto?>,
            IHandleEvent<IMarketCurveCreated>,
            IHandleEvent<ICurvePointAdded>,
            IHandleEvent<IInstrumentCreated>
    {
        private readonly IReadModelRepository<Dto> _curveRepo;
        private readonly IReadModelRepository<InstrumentDto> _instrumentRepo;

        public Handler(IReadModelRepository<Dto> curveRepo, IReadModelRepository<InstrumentDto> instrumentRepo)
        {
            _curveRepo = curveRepo ?? throw new ArgumentNullException(nameof(curveRepo));
            _instrumentRepo = instrumentRepo ?? throw new ArgumentNullException(nameof(instrumentRepo));
        }

        public Task<Dto?> Handle(Query query, CancellationToken cancellationToken)
        {
            return _curveRepo.Get(query.Id.NonEmpty());
        }

        public Task Handle(IEventWrapper<IMarketCurveCreated> @event, CancellationToken cancellationToken)
        {
            var dto = new Dto
            {
                Id = @event.AggregateId,
                Name = GenerateName(@event.Content)
            };

            return _curveRepo.Insert(dto);
        }

        public Task Handle(IEventWrapper<ICurvePointAdded> wrapper, CancellationToken cancellationToken)
        {
            var @event = wrapper.Content;

            return _curveRepo.Get(wrapper.AggregateId).IfNotNull(curve => 
            {
                return _instrumentRepo.Get(@event.InstrumentId.NonEmpty()).IfNotNull(instrument =>
                {
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

                    curve.CurvePoints = points.OrderBy(x => x.Tenor).ToList();

                    return _curveRepo.Update(curve);
                });
            });
        }

        public Task Handle(IEventWrapper<IInstrumentCreated> wrapper, CancellationToken cancellationToken)
        {
            var @event = wrapper.Content;

            var instrument = new InstrumentDto
            {
                Id = wrapper.AggregateId,
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
