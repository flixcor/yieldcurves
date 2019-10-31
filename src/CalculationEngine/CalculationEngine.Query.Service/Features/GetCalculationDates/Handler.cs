﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Events;
using Common.Infrastructure.Extensions;

namespace CalculationEngine.Query.Service.Features.GetCalculationDates
{
    public class Handler :
        IHandleListQuery<Query, Dto>,
        IHandleEvent<CurveCalculated>
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
