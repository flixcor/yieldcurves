using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Events;

namespace PricePublisher.Query.Service.Features.GetPriceDates
{
    public class Handler :
        IHandleListQuery<Query, Dto>,
        IHandleEvent<IInstrumentPricingPublished>
    {
        private readonly IReadModelRepository<Dto> _repository;

        public Handler(IReadModelRepository<Dto> repository)
        {
            _repository = repository;
        }

        public IAsyncEnumerable<Dto> Handle(Query query, CancellationToken cancellationToken)
        {
            return _repository.GetAll();
        }

        public async Task Handle(IEventWrapper<IInstrumentPricingPublished> @event, CancellationToken cancellationToken)
        {
            var asOfDate = @event.Content.AsOfDate;
            var existing = await _repository.Single(x => x.AsOfDate == asOfDate);

            if (existing == null)
            {
                var newDto = new Dto { AsOfDate = asOfDate };
                await _repository.Insert(newDto);
            }
        }
    }
}
