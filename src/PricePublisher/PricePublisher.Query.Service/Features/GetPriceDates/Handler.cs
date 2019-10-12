using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Core.Events;

namespace PricePublisher.Query.Service.Features.GetPriceDates
{
    public class Handler :
        IHandleQuery<Query, IEnumerable<Dto>>,
        IHandleEvent<InstrumentPricingPublished>
    {
        private readonly IReadModelRepository<Dto> _repository;

        public Handler(IReadModelRepository<Dto> repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Dto>> Handle(Query query, CancellationToken cancellationToken)
        {
            return _repository.GetAll();
        }

        public async Task Handle(InstrumentPricingPublished @event, CancellationToken cancellationToken)
        {
            var existing = await _repository.Single(x => x.AsOfDate == @event.AsOfDate);

            if (!existing.Found)
            {
                var newDto = new Dto { AsOfDate = @event.AsOfDate };
                await _repository.Insert(newDto);
            }
        }
    }
}
