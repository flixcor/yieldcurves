using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Core.Events;
using MarketCurves.Domain;

namespace MarketCurves.Service.Features
{
    public static class AddCurvePoint
    {
        public class Command : ICommand
        {
            public Guid MarketCurveId { get; set; }
            public Tenor Tenor { get; set; }
            public Guid InstrumentId { get; set; }
            public short DateLag { get; set; }
            public bool IsMandatory { get; set; }
            public PriceType? PriceType { get; set; }

            public class Handler : IHandleCommand<Command>,
                IHandleEvent<InstrumentCreated>
            {
                private readonly IRepository _repository;
                private readonly IReadModelRepository<Instrument> _readModelRepository;

                public Handler(IRepository repository, IReadModelRepository<Instrument> readModelRepository)
                {
                    _repository = repository ?? throw new ArgumentNullException(nameof(repository));
                    _readModelRepository = readModelRepository ?? throw new ArgumentNullException(nameof(readModelRepository));
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

                public Task Handle(InstrumentCreated @event, CancellationToken cancellationToken)
                {
                    var instrument = new Instrument
                    {
                        Id = @event.Id,
                        Vendor = @event.Vendor,
                        Name = @event.Description,
                        HasPriceType = @event.HasPriceType
                    };

                    return _readModelRepository.Insert(instrument);
                }
            }
        }

        public class Query : IQuery<Query.Dto>
        {
            public Guid MarketCurveId { get; set; }

            public class Handler :
            IHandleQuery<Query, Dto>,
            IHandleEvent<CurvePointAdded>
            {
                private readonly IReadModelRepository<UsedValues> _usedValues;
                private readonly IReadModelRepository<Instrument> _instruments;

                public Handler(IReadModelRepository<UsedValues> usedValues, IReadModelRepository<Instrument> instruments)
                {
                    _instruments = instruments ?? throw new ArgumentNullException(nameof(instruments));
                    _usedValues = usedValues ?? throw new ArgumentNullException(nameof(usedValues));
                }

                public async Task<Dto> Handle(Query query, CancellationToken cancellationToken)
                {
                    var existing =
                    (await _usedValues.Get(query.MarketCurveId))
                    .Coalesce(new UsedValues());

                    var instruments = await _instruments.GetAll();

                    var dto = new Dto
                    {
                        Command = new Command
                        {
                            MarketCurveId = query.MarketCurveId,
                        },

                        Instruments = instruments
                            .Where(x => !existing.Instruments.Contains(x.Id))
                            .Select(x => new Instrument
                            {
                                Id = x.Id,
                                HasPriceType = x.HasPriceType,
                                Vendor = x.Vendor,
                                Name = x.Name
                            }),

                        Tenors = Enum.GetNames(typeof(Tenor))
                            .Where(x => !existing.Tenors.Contains(x))
                    };

                    return dto;
                }

                public async Task Handle(CurvePointAdded @event, CancellationToken cancellationToken)
                {
                    var dto = await (await _usedValues.Get(@event.Id))
                    .Coalesce(async () =>
                    {
                        var newDto = new UsedValues
                        {
                            Id = @event.Id
                        };

                        await _usedValues.Insert(newDto);

                        return newDto;
                    });

                    dto.Instruments.Add(@event.InstrumentId);
                    dto.Tenors.Add(@event.Tenor);

                    await _usedValues.Update(dto);
                }
            }

            public class Dto
            {
                public Command Command { get; set; }
                public IEnumerable<Instrument> Instruments { get; set; } = new List<Instrument>();
                public IEnumerable<string> Tenors { get; set; } = new List<string>();
                public IEnumerable<string> PriceTypes { get; set; } = Enum.GetNames(typeof(PriceType));
            }
        }
    }

    public class Instrument : ReadObject
    {
        public string Vendor { get; set; }
        public string Name { get; set; }
        public bool HasPriceType { get; set; }
    }

    public class UsedValues : ReadObject
    {
        public IList<Guid> Instruments { get; set; } = new List<Guid>();
        public IList<string> Tenors { get; set; } = new List<string>();
    }
}
