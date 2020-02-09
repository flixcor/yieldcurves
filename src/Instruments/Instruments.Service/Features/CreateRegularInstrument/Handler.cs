using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.EventStore.Lib;
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

        public Task<Result> Handle(Command command, CancellationToken cancellationToken)
            => command.Vendor.TryParseEnum<Vendor>().Promise(
                v => Handle(cancellationToken, command.Id.NonEmpty(),
                    i => i.TryDefine(v, command.Name.NonEmpty())));
    }
}
