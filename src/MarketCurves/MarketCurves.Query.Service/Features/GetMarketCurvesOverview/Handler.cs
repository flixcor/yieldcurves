using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Events;
using Common.Core.Extensions;

namespace MarketCurves.Query.Service.Features.GetMarketCurvesOverview
{
    public class Handler :
            IHandleQuery<Query, IEnumerable<Dto>>,
            IHandleEvent<MarketCurveCreated>
    {
        private readonly IReadModelRepository<Dto> _readModelRepository;

        public Handler(IReadModelRepository<Dto> readModelRepository)
        {
            _readModelRepository = readModelRepository ?? throw new ArgumentNullException(nameof(readModelRepository));
        }

        public Task<IEnumerable<Dto>> Handle(Query query, CancellationToken cancellationToken)
        {
            return _readModelRepository.GetAll();
        }

        public Task Handle(MarketCurveCreated @event, CancellationToken cancellationToken)
        {
            var curve = new Dto
            {
                Id = @event.Id,
                Name = GenerateName(@event)
            };

            return _readModelRepository.Insert(curve);
        }

        private string GenerateName(MarketCurveCreated @event)
        {
            var stringBuilder = new StringBuilder("M");

            stringBuilder.AppendFormatNonEmptyString("_{0}", @event.Country, @event.CurveType, @event.FloatingLeg);

            return stringBuilder.ToString();
        }
    }
}
