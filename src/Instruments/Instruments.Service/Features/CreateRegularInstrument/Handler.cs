using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Infrastructure;
using Instruments.Domain;

namespace Instruments.Service.Features.CreateRegularInstrument
{
    public class Handler :
            ApplicationService<RegularInstrument>,
            IHandleCommand<Command>
    {
        public Handler(IAggregateRepository repository) : base(repository)
        {
        }

        public Task<Either<Error, Nothing>> Handle(Command command, CancellationToken cancellationToken)
            => Handle(cancellationToken, command.Id.NonEmpty(), i =>
                command.Vendor.TryParseEnum<Vendor>()
                    .MapRight(v => i.TryDefine(v, command.Name.NonEmpty()))
            );
    }
}
