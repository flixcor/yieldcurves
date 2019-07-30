using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Core.Events;

namespace Instruments.Query.Service.Features
{
    public class GetInstrumentList : IQuery<IEnumerable<InstrumentDto>>
    {
        public string Vendor { get; set; }

        public class Handler :
            IHandleQuery<GetInstrumentList, IEnumerable<InstrumentDto>>,
            IHandleEvent<RegularInstrumentCreated>,
            IHandleEvent<BloombergInstrumentCreated>
        {
            private readonly IReadModelRepository<InstrumentDto> _repository;

            public Handler(IReadModelRepository<InstrumentDto> repository)
            {
                _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            }

            public Task<IEnumerable<InstrumentDto>> Handle(GetInstrumentList query, CancellationToken cancellationToken)
            {
                return _repository.GetAll();
            }

            public Task Handle(RegularInstrumentCreated @event, CancellationToken cancellationToken)
            {
                var dto = new InstrumentDto { Id = @event.Id, Description = @event.Description, Vendor = @event.Vendor };

                return _repository.Insert(dto);
            }

            public Task Handle(BloombergInstrumentCreated @event, CancellationToken cancellationToken)
            {
                var dto = new InstrumentDto
                {
                    Id = @event.Id,
                    Description = $"{@event.Ticker} {@event.PricingSource} {@event.YellowKey}",
                    Vendor = "Bloomberg"
                };

                return _repository.Insert(dto);
            }
        }
    }

    public class InstrumentDto : ReadObject
    {
        public string Vendor { get; set; }
        public string Description { get; set; }
    }
}
