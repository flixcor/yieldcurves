using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Events;
using PricePublisher.Service.Domain;

namespace PricePublisher.Service.Features.PublishPrice
{
    public class Handler :
        IHandleCommand<Command>,
        IHandleQuery<Query, Dto>,
        IHandleEvent<IInstrumentCreated>
    {
        private readonly IRepository _repository;
        private readonly IReadModelRepository<InstrumentDto> _readModelRepository;
        private readonly Func<DateTime> _currentDateTimeFactory;

        public Handler(IRepository repository, IReadModelRepository<InstrumentDto> readModelRepository, Func<DateTime> currentDateTimeFactory)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _readModelRepository = readModelRepository ?? throw new ArgumentNullException(nameof(readModelRepository));
            _currentDateTimeFactory = currentDateTimeFactory ?? throw new ArgumentNullException(nameof(currentDateTimeFactory));
        }

        public async Task<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            var instrumentResult = await _readModelRepository.Get(command.InstrumentId).ToResult();
            var currencyResult = Currency.FromString(command.PriceCurrency);
            var asOfDateResult = Date.TryParse(command.AsOfDate);

            var result = await Result
                .Combine(instrumentResult, currencyResult, asOfDateResult, (_, currency, asOfDate) =>
                {
                    var price = new Price(currency, command.PriceAmount);
                    var pricing = new InstrumentPricing(command.Id, asOfDate, _currentDateTimeFactory(), command.InstrumentId, price, command.PriceType);

                    return _repository.SaveAsync(pricing);
                });

            return result;
        }

        public Task Handle(IInstrumentCreated @event, CancellationToken cancellationToken)
        {
            var dto = new InstrumentDto { Id = @event.AggregateId, HasPriceType = @event.HasPriceType, Name = @event.Description, Vendor = @event.Vendor };
            return _readModelRepository.Insert(dto);
        }

        public async Task<Dto> Handle(Query query, CancellationToken cancellationToken)
        {
            var instruments = await _readModelRepository.GetAll().ToListAsync(cancellationToken);

            return new Dto
            {
                Instruments = instruments
            };
        }
    }
}
