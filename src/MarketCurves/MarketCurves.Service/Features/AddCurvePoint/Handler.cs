using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Events;
using Common.Infrastructure;
using MarketCurves.Domain;
using static MarketCurves.Service.Domain.Instrument;

namespace MarketCurves.Service.Features.AddCurvePoint
{
    public class Handler :
        ApplicationService<MarketCurve>,
        IHandleCommand<Command>,
        IHandleQuery<Query, Dto>,
        IHandleEvent<IInstrumentCreated>,
        IHandleEvent<ICurvePointAdded>
    {
        private readonly IReadModelRepository<Instrument> _readModelRepository;
        private readonly IReadModelRepository<UsedValues> _usedValues;
        private readonly IReadModelRepository<Instrument> _instruments;

        public Handler(IAggregateRepository repository, IReadModelRepository<Instrument> readModelRepository, IReadModelRepository<UsedValues> usedValues, IReadModelRepository<Instrument> instruments) : base(repository)
        {
            _readModelRepository = readModelRepository ?? throw new ArgumentNullException(nameof(readModelRepository));
            _usedValues = usedValues ?? throw new ArgumentNullException(nameof(usedValues));
            _instruments = instruments ?? throw new ArgumentNullException(nameof(instruments));
        }

        public async Task<Either<Error,Nothing>> Handle(Command command, CancellationToken cancellationToken)
        {
            var dateLag = new DateLag(command.DateLag);
            var instrumentResult = await FromId(command.InstrumentId.NonEmpty(), GetVendor);

            return await Handle(cancellationToken, command.MarketCurveId.NonEmpty(), whatToDo: c => 
                instrumentResult.MapRight(instrument => 
                    c.AddCurvePoint(command.Tenor, instrument, dateLag, command.PriceType, command.IsMandatory)));
        }

        async Task<Either<Error,Vendor>> GetVendor(NonEmptyGuid id)
        {
            var instrument = await _readModelRepository.Get(id);

            if (instrument == null)
            {
                return new Error("Not found");
            }

            return instrument.Vendor.TryParseEnum<Vendor>();
        }

        public Task Handle(IEventWrapper<IInstrumentCreated> @event, CancellationToken cancellationToken)
        {
            var instrument = new Instrument
            {
                Id = @event.AggregateId,
                Vendor = @event.Content.Vendor,
                Name = @event.Content.Description,
                HasPriceType = @event.Content.HasPriceType
            };

            return _readModelRepository.Insert(instrument);
        }

        public async Task<Dto> Handle(Query query, CancellationToken cancellationToken)
        {
            var existing = (await _usedValues.Get(query.MarketCurveId.NonEmpty())) ?? new UsedValues();

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
            var dto = await _usedValues.Get(wrapper.AggregateId);

            if (dto == null)
            {
                dto = new UsedValues
                {
                    Id = wrapper.AggregateId
                };

                await _usedValues.Insert(dto);
            }

            dto.Instruments.Add(wrapper.Content.InstrumentId);
            dto.Tenors.Add(wrapper.Content.Tenor);

            await _usedValues.Update(dto);
        }
    }
}
