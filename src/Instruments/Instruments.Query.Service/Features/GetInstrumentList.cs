using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Core.Events;
using Instruments.Query.Service.Infrastructure;
using Microsoft.AspNetCore.SignalR;

namespace Instruments.Query.Service.Features
{
    public class GetInstrumentList : IQuery<GetInstrumentList.Result>
    {
        public string Vendor { get; set; }

        public class Result
        {
            public Result(IEnumerable<InstrumentDto> instruments)
            {
                Instruments = instruments ?? new List<InstrumentDto>();
            }

            public IEnumerable<InstrumentDto> Instruments { get; set; }
        }

        public class Handler :
            IHandleQuery<GetInstrumentList, Result>,
            IHandleEvent<InstrumentCreated>
        {
            private readonly IReadModelRepository<InstrumentDto> _repository;
            private readonly IHubContext<InstrumentHub> _instrumentHub;

            public Handler(IReadModelRepository<InstrumentDto> repository, IHubContext<InstrumentHub> instrumentHub)
            {
                _repository = repository ?? throw new ArgumentNullException(nameof(repository));
                _instrumentHub = instrumentHub;
            }

            public async Task<Result> Handle(GetInstrumentList query, CancellationToken cancellationToken)
            {
                var instruments = await _repository.GetAll();
                return new Result(instruments);
            }

            public async Task Handle(InstrumentCreated @event, CancellationToken cancellationToken)
            {
                var dto = new InstrumentDto
                {
                    Id = @event.Id,
                    Description = @event.Description,
                    Vendor = @event.Vendor
                };

                await _repository.Insert(dto);
                await _instrumentHub.Clients.All.SendAsync(@event.GetType().Name, @event);
            }
        }
    }

    public class InstrumentDto : ReadObject
    {
        public string Vendor { get; set; }
        public string Description { get; set; }
    }
}
