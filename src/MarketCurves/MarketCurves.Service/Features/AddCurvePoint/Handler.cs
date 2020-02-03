using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Events;
using Common.EventStore.Lib.EfCore;
using Common.Infrastructure.Extensions;
using MarketCurves.Domain;

namespace MarketCurves.Service.Features.AddCurvePoint
{
    public class Handler : IHandleCommand<Command>,
        IHandleQuery<Query, Dto>,
        IHandleEvent<IInstrumentCreated>,
        IHandleEvent<ICurvePointAdded>
    {
        private readonly IAggregateRepository _repository;
        private readonly IReadModelRepository<Instrument> _readModelRepository;
        private readonly IReadModelRepository<UsedValues> _usedValues;
        private readonly IReadModelRepository<Instrument> _instruments;

        public Handler(IAggregateRepository repository, IReadModelRepository<Instrument> readModelRepository, IReadModelRepository<UsedValues> usedValues, IReadModelRepository<Instrument> instruments)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _readModelRepository = readModelRepository ?? throw new ArgumentNullException(nameof(readModelRepository));
            _usedValues = usedValues ?? throw new ArgumentNullException(nameof(usedValues));
            _instruments = instruments ?? throw new ArgumentNullException(nameof(instruments));
        }

        public Task<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            return _readModelRepository
                .Get(command.InstrumentId)
                .ToResult()
                .Promise(async i =>
                {
                    if (i.HasPriceType && command.PriceType == null)
                    {
                        return Result.Fail();
                    }

                    var dateLag = new DateLag(command.DateLag);

                    var c = await _repository.GetByIdAsync<MarketCurve>(command.MarketCurveId);

                    if (c != null)
                    {
                        var result = c.AddCurvePoint(command.Tenor, command.InstrumentId, dateLag, command.PriceType, command.IsMandatory);
                        return await result.Promise(() => _repository.SaveAsync(c));
                    }

                    return Result.Fail();
                });
        }

        public Task Handle(IEventWrapper<IInstrumentCreated> @event, CancellationToken cancellationToken)
        {
            var instrument = new Instrument
            {
                Id = @event.Metadata.AggregateId,
                Vendor = @event.Content.Vendor,
                Name = @event.Content.Description,
                HasPriceType = @event.Content.HasPriceType
            };

            return _readModelRepository.Insert(instrument);
        }

        public async Task<Dto> Handle(Query query, CancellationToken cancellationToken)
        {
            var existing = (await _usedValues.Get(query.MarketCurveId)) ?? new UsedValues();

            var instruments = await _instruments
                .GetAll()
                .Where(x => !existing.Instruments.Contains(x.Id))
                .ToListAsync();

            var dto = new Dto
            {
                Command = new Command
                {
                    MarketCurveId = query.MarketCurveId,
                },

                Instruments = instruments,

                Tenors = Enum.GetNames(typeof(Tenor))
                    .Where(x => !existing.Tenors.Contains(x))
            };

            return dto;
        }

        public async Task Handle(IEventWrapper<ICurvePointAdded> wrapper, CancellationToken cancellationToken)
        {
            var dto = await _usedValues.Get(wrapper.Metadata.AggregateId);

            if (dto == null)
            {
                dto = new UsedValues
                {
                    Id = wrapper.Metadata.AggregateId
                };

                await _usedValues.Insert(dto);
            }

            dto.Instruments.Add(wrapper.Content.InstrumentId);
            dto.Tenors.Add(wrapper.Content.Tenor);

            await _usedValues.Update(dto);
        }
    }
}
