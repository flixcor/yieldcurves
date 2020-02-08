using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.EventStore.Lib;
using Instruments.Domain;

namespace Instruments.Service.Features.CreateRegularInstrument
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
            return command.Vendor.TryParseEnum<Vendor>().Promise(v =>
                new RegularInstrument().TryDefine(v, command.Name).Promise(i =>
                    _repository.Save(i)
                )
            );
        }
    }
}
