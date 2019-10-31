using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Events;

namespace PricePublisher.Query.Service.Features.GetPriceDates
{
    public class Handler :
        IHandleListQuery<Query, Dto>,
        IHandleEvent<InstrumentPricingPublished>
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

        public async Task Handle(InstrumentPricingPublished @event, CancellationToken cancellationToken)
        {
            var asOfDateString = @event.AsOfDate.ToString("yyyy-MM-dd");
            var existing = await _repository.Single(x => x.AsOfDate == asOfDateString);

            if (!existing.Found)
            {
                var newDto = new Dto { AsOfDate = asOfDateString };
                await _repository.Insert(newDto);
            }
        }
    }
}
