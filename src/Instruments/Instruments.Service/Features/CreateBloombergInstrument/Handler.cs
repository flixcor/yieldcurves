using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Infrastructure;
using Instruments.Domain;

namespace Instruments.Service.Features.CreateBloombergInstrument
{
    public class Handler :
            ApplicationService<BloombergInstrument>,
            IHandleCommand<Command>
    {
        public Handler(IAggregateRepository repository) : base(repository)
        {
        }

        public Task<Either<Error, Nothing>> Handle(Command command, CancellationToken cancellationToken)
        {
            var pricingSourceResult = command.PricingSource.TryParseEnum<PricingSource>();
            var yellowKeyResult = command.YellowKey.TryParseEnum<YellowKey>();

            return Handle(cancellationToken, command.Id.NonEmpty(), b =>
                pricingSourceResult.MapRight(yellowKeyResult, (pricingSource, yellowKey)
                    => b.Define(command.Ticker.NonEmpty(), pricingSource, yellowKey)));
        }
    }
}
