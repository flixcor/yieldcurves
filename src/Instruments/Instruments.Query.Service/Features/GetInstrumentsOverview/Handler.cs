using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Core.Events;

namespace Instruments.Query.Service.Features.GetInstrumentsOverview
{
    public class Handler :
            IHandleQuery<Query, IEnumerable<Dto>>,
            IHandleEvent<InstrumentCreated>
    {
        private readonly IReadModelRepository<Dto> _repository;

        public Handler(IReadModelRepository<Dto> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task<IEnumerable<Dto>> Handle(Query query, CancellationToken cancellationToken)
        {
            return _repository.GetAll();
        }

        public Task Handle(InstrumentCreated @event, CancellationToken cancellationToken)
        {
            var dto = new Dto
            {
                Id = @event.Id,
                Description = @event.Description,
                Vendor = @event.Vendor
            };

            return _repository.Insert(dto);
        }
    }
}
