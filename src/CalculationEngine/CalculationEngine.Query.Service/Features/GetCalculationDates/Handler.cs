using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Core.Events;

namespace CalculationEngine.Query.Service.Features.GetCalculationDates
{
    public class Handler :
        IHandleQuery<Query, IEnumerable<Dto>>,
        IHandleEvent<CurveCalculated>
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

        public async Task Handle(CurveCalculated @event, CancellationToken cancellationToken)
        {
            var asOfDate = @event.AsOfDate.ToString("yyyy-MM-dd");

            var existingDto = await _readModelRepository.Single(x => x.AsOfDate == asOfDate);

            if (!existingDto.Found)
            {
                var dto = new Dto
                {
                    Id = Guid.NewGuid(),
                    AsOfDate = asOfDate
                };

                await _readModelRepository.Insert(dto);
            }
        }
    }
}
