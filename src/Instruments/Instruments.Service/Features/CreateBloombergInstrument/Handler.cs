using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.EventStore.Lib;
using Instruments.Domain;

namespace Instruments.Service.Features.CreateBloombergInstrument
{
    public class Handler :
            IHandleCommand<Command>
    {
        private readonly IAggregateRepository _repository;

        public Handler(IAggregateRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            var pricingSourceResult = command.PricingSource.TryParseEnum<PricingSource>();
            var yellowKeyResult = command.YellowKey.TryParseEnum<YellowKey>();

            var instrumentResult = Result.Combine(
                pricingSourceResult, 
                yellowKeyResult, 
                (pricingSource, yellowKey) => new BloombergInstrument().Define(command.Ticker.NonEmpty(), pricingSource, yellowKey));

            return instrumentResult.Promise(i => _repository.Save(i));
        }
    }
}
