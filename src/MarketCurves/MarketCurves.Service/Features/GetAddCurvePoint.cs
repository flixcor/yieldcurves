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
    public class GetAddCurvePoint : IQuery<AddCurvePointDto>
    {
        public Guid MarketCurveId { get; set; }

        public class Handler :
        IHandleQuery<GetAddCurvePoint, AddCurvePointDto>,
        IHandleEvent<CurvePointAdded>
        {
            private readonly IReadModelRepository<UsedValues> _usedValues;
            private readonly IReadModelRepository<Instrument> _instruments;

            public Handler(IReadModelRepository<UsedValues> usedValues, IReadModelRepository<Instrument> instruments)
            {
                _instruments = instruments ?? throw new ArgumentNullException(nameof(instruments));
                _usedValues = usedValues ?? throw new ArgumentNullException(nameof(usedValues));
            }

            public async Task<AddCurvePointDto> Handle(GetAddCurvePoint query, CancellationToken cancellationToken)
            {
                var existing =
                (await _usedValues.Get(query.MarketCurveId))
                .Coalesce(new UsedValues());

                var instruments = await _instruments.GetAll();

                var dto = new AddCurvePointDto
                {
                    Command = new AddCurvePoint
                    {
                        MarketCurveId = query.MarketCurveId,
                    },

                    Instruments = instruments
                        .Where(x => !existing.Instruments.Contains(x.Id))
                        .Select(x => new UsedInstrument
                        {
                            Id = x.Id,
                            HasPriceType = x.Vendor.HasPriceType(),
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
    }

    public class AddCurvePointDto
    {
        public AddCurvePoint Command { get; set; }
        public IEnumerable<UsedInstrument> Instruments { get; set; } = new List<UsedInstrument>();
        public IEnumerable<string> Tenors { get; set; } = new List<string>();
        public IEnumerable<string> PriceTypes { get; set; } = Enum.GetNames(typeof(PriceType));
    }

    public class UsedValues : ReadObject
    {
        public IList<Guid> Instruments { get; set; } = new List<Guid>();
        public IList<string> Tenors { get; set; } = new List<string>();
    }

    public class UsedInstrument
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool HasPriceType { get; set; }
    }
}
