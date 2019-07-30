using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Core.Events;
using PricePublisher.Domain;

namespace PricePublisher.Service.Features
{
    public class PublishPrice : ICommand
    {
        public Guid Id { get; set; }
        public DateTime AsOfDate { get; set; }
        public DateTime AsAtDate { get; set; }
        public Guid InstrumentId { get; set; }
        public string PriceCurrency { get; set; }
        public double PriceAmount { get; set; }
        public PriceType? PriceType { get; set; }


        public class Handler : 
            IHandleCommand<PublishPrice>,
            IHandleEvent<BloombergInstrumentCreated>,
            IHandleEvent<RegularInstrumentCreated>
        {
            private readonly IRepository _repository;
            private readonly IReadModelRepository<InstrumentDto> _readModelRepository;

            public Handler(IRepository repository, IReadModelRepository<InstrumentDto> readModelRepository)
            {
                _repository = repository ?? throw new ArgumentNullException(nameof(repository));
                _readModelRepository = readModelRepository ?? throw new ArgumentNullException(nameof(readModelRepository));
            }

            public async Task<Result> Handle(PublishPrice command, CancellationToken cancellationToken)
            {
                var instrumentResult = await _readModelRepository.Get(command.InstrumentId).ToResult();
                var currencyResult = Currency.FromString(command.PriceCurrency);

                return await Result
                    .Combine(instrumentResult, currencyResult)
                    .Promise(() =>
                    {
                        var currency = currencyResult.Content;

                        var price = new Price(currency, command.PriceAmount);
                        var pricing = new InstrumentPricing(command.Id, command.AsOfDate, command.AsAtDate, command.InstrumentId, price, command.PriceType);

                        return _repository.SaveAsync(pricing);
                    });
            }

            public Task Handle(BloombergInstrumentCreated @event, CancellationToken cancellationToken)
            {
                var dto = new InstrumentDto { Id = @event.Id };
                return _readModelRepository.Insert(dto);
            }

            public Task Handle(RegularInstrumentCreated @event, CancellationToken cancellationToken)
            {
                var dto = new InstrumentDto { Id = @event.Id };
                return _readModelRepository.Insert(dto);
            }
        }
    }

    public class InstrumentDto : ReadObject
    {
    }
}
