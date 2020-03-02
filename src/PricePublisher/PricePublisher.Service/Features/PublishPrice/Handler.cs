using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Events;
using Common.Infrastructure;
using PricePublisher.Service.Domain;

namespace PricePublisher.Service.Features.PublishPrice
{
    public class Handler :
        ApplicationService<InstrumentPricing>,
        IHandleCommand<Command>,
        IHandleQuery<Query, Dto>,
        IHandleEvent<IInstrumentCreated>
    {
        private readonly IReadModelRepository<InstrumentDto> _readModelRepository;

        public Handler(IAggregateRepository repository, IReadModelRepository<InstrumentDto> readModelRepository) : base(repository)
        {
            _readModelRepository = readModelRepository ?? throw new ArgumentNullException(nameof(readModelRepository));
        }

        public Task<Either<Error, Nothing>> Handle(Command command, CancellationToken cancellationToken)
        {
            var currencyResult = Currency.FromString(command.PriceCurrency).MapRight(x=> new Price(x, command.PriceAmount));
            var asOfDateResult = Date.TryParse(command.AsOfDate);

            return Handle(cancellationToken, command.Id.NonEmpty(), p =>
            {
                return currencyResult.MapRight(asOfDateResult, (pr, d) => p.Define(d, command.InstrumentId.NonEmpty(), pr, command.PriceType));
            });
        }

        public Task Handle(IEventWrapper<IInstrumentCreated> @event, CancellationToken cancellationToken)
        {
            var dto = new InstrumentDto 
            { 
                Id = @event.AggregateId, 
                HasPriceType = @event.Content.HasPriceType, 
                Name = @event.Content.Description, 
                Vendor = @event.Content.Vendor 
            };

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
