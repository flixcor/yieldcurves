using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Core.Events;
using MarketCurves.Domain;

namespace MarketCurves.Service.Features
{
    public class AddCurvePoint : ICommand
    {
        public Guid MarketCurveId { get; set; }
        public Tenor Tenor { get; set; }
        public Guid InstrumentId { get; set; }
        public short DateLag { get; set; }
        public bool IsMandatory { get; set; }
        public PriceType? PriceType { get; set; }

        public class Handler : IHandleCommand<AddCurvePoint>,
            IHandleEvent<BloombergInstrumentCreated>,
            IHandleEvent<RegularInstrumentCreated>
        {
            private readonly IRepository _repository;
            private readonly IReadModelRepository<Instrument> _readModelRepository;

            public Handler(IRepository repository, IReadModelRepository<Instrument> readModelRepository)
            {
                _repository = repository ?? throw new ArgumentNullException(nameof(repository));
                _readModelRepository = readModelRepository ?? throw new ArgumentNullException(nameof(readModelRepository));
            }

            public Task<Result> Handle(AddCurvePoint command, CancellationToken cancellationToken)
            {
                return _readModelRepository
                    .Get(command.InstrumentId)
                    .ToResult()
                    .Promise(async i => 
                    {
                        if (i.Vendor.HasPriceType() && command.PriceType == null)
                        {
                            return;
                        }

                        var dateLag = new DateLag(command.DateLag);

                        var c = await _repository.GetByIdAsync<MarketCurve>(command.MarketCurveId);

                        if (c != null)
                        {
                            c.AddCurvePoint(command.Tenor, command.InstrumentId, dateLag, command.PriceType, command.IsMandatory);
                            await _repository.SaveAsync(c);
                        }
                    });
            }

            public Task Handle(BloombergInstrumentCreated @event, CancellationToken cancellationToken)
            {
                var instrument = new Instrument
                {
                    Id = @event.Id,
                    Vendor = Vendor.Bloomberg,
                    Name = $"{@event.Ticker} {@event.YellowKey} {@event.PricingSource}"
                };

                return _readModelRepository.Insert(instrument);
            }

            public Task Handle(RegularInstrumentCreated @event, CancellationToken cancellationToken)
            {
                var instrument = new Instrument
                {
                    Id = @event.Id,
                    Vendor = Enum.Parse<Vendor>(@event.Vendor, true),
                    Name = @event.Description
                };

                return _readModelRepository.Insert(instrument);
            }
        }
    }

    public class Instrument : ReadObject
    {
        public Vendor Vendor { get; set; }
        public string Name { get; set; }
    }
}
