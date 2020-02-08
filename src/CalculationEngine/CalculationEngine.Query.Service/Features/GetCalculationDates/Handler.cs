using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Events;

namespace CalculationEngine.Query.Service.Features.GetCalculationDates
{
    public class Handler :
        IHandleListQuery<Query, Dto>,
        IHandleEvent<ICurveCalculated>
    {
        private readonly IReadModelRepository<Dto> _readModelRepository;

        public Handler(IReadModelRepository<Dto> readModelRepository)
        {
            _readModelRepository = readModelRepository ?? throw new ArgumentNullException(nameof(readModelRepository));
        }

        public IAsyncEnumerable<Dto> Handle(Query query, CancellationToken cancellationToken)
        {
            return _readModelRepository.GetAll();
        }

        public async Task Handle(IEventWrapper<ICurveCalculated> @event, CancellationToken cancellationToken)
        {
            var asOfDate = @event.Content.AsOfDate;

            var existingDto = await _readModelRepository.Single(x => x.AsOfDate == asOfDate);

            if (existingDto != null)
            {
                var dto = new Dto
                {
                    Id = NonEmpty.Guid(),
                    AsOfDate = asOfDate
                };

                await _readModelRepository.Insert(dto);
            }
        }
    }
}
