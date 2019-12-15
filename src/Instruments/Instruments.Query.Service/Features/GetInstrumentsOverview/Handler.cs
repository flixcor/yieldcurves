using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Events;

namespace Instruments.Query.Service.Features.GetInstrumentsOverview
{
    public class Handler :
            IHandleListQuery<Query, Dto>,
            IHandleEvent<IInstrumentCreated>
    {
        private readonly IReadModelRepository<Dto> _repository;

        public Handler(IReadModelRepository<Dto> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public IAsyncEnumerable<Dto> Handle(Query query, CancellationToken cancellationToken)
        {
            return _repository.GetAll();
        }

        public Task Handle(IInstrumentCreated @event, CancellationToken cancellationToken)
        {
            var dto = new Dto
            {
                Id = @event.AggregateId,
                Description = @event.Description,
                Vendor = @event.Vendor
            };

            return _repository.Insert(dto);
        }
    }
}
