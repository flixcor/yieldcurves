using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Core.Events;
using PricePublisher.Domain;

namespace PricePublisher.Service.Features
{
    public class PublishPrice
    {
        public class Command : ICommand
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public DateTime AsOfDate { get; set; }
            public Guid InstrumentId { get; set; }
            public string PriceCurrency { get; set; }
            public double PriceAmount { get; set; }
            public PriceType? PriceType { get; set; }

            public class Handler :
                IHandleCommand<Command>,
                IHandleEvent<BloombergInstrumentCreated>,
                IHandleEvent<RegularInstrumentCreated>
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

                    return await Result
                        .Combine(instrumentResult, currencyResult)
                        .Promise(() =>
                        {
                            var currency = currencyResult.Content;

                            var price = new Price(currency, command.PriceAmount);
                            var pricing = new InstrumentPricing(command.Id, command.AsOfDate, _currentDateTimeFactory(), command.InstrumentId, price, command.PriceType);

                            return _repository.SaveAsync(pricing);
                        });
                }

                public Task Handle(BloombergInstrumentCreated @event, CancellationToken cancellationToken)
                {
                    var dto = new InstrumentDto { Id = @event.Id, HasPriceType = true };
                    return _readModelRepository.Insert(dto);
                }

                public Task Handle(RegularInstrumentCreated @event, CancellationToken cancellationToken)
                {
                    var dto = new InstrumentDto { Id = @event.Id, HasPriceType = @event.Vendor.HasPriceType() };
                    return _readModelRepository.Insert(dto);
                }
            }
        }

        public class Query : IQuery<Query.Dto>
        {
            public class Handler : IHandleQuery<Query, Query.Dto>
            {
                private readonly IReadModelRepository<InstrumentDto> _readModelRepository;

                public Handler(IReadModelRepository<InstrumentDto> readModelRepository)
                {
                    _readModelRepository = readModelRepository ?? throw new ArgumentNullException(nameof(readModelRepository));
                }

                public async Task<Dto> Handle(Query query, CancellationToken cancellationToken)
                {
                    return new Dto
                    {
                        Instruments = await _readModelRepository.GetAll()
                    };
                }
            }

            public class Dto
            {
                public Command Command { get; set; } = new Command();

                public string[] PriceTypes { get; } = Enum.GetNames(typeof(PriceType));
                public IEnumerable<InstrumentDto> Instruments { get; set; } = new List<InstrumentDto>();
            }

        }



    }

    public static class Extensions
    {
        public static bool HasPriceType(this string vendor)
        {
            switch (vendor.ToLower())
            {
                case "bloomberg":
                    return true;
                default:
                    return false;
            }
        }

    }

    public class InstrumentDto : ReadObject
    {
        public bool HasPriceType { get; set; }
    }
}
