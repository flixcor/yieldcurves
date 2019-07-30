using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Instruments.Domain;

namespace Instruments.Service.Features
{
    public class CreateBloombergInstrument : ICommand
    {
        public Guid Id { get; set; }
        public string Ticker { get; set; }
        public PricingSource PricingSource { get; set; }
        public YellowKey YellowKey { get; set; }

        public class Handler :
            IHandleCommand<CreateBloombergInstrument>
        {
            private readonly IRepository _repository;

            public Handler(IRepository repository)
            {
                _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            }

            public async Task<Result> Handle(CreateBloombergInstrument command, CancellationToken cancellationToken)
            {
                var instrument = new BloombergInstrument(command.Id, command.Ticker, command.PricingSource, command.YellowKey);

                await _repository.SaveAsync(instrument);
                return Result.Ok();
            }
        }
    }
}
