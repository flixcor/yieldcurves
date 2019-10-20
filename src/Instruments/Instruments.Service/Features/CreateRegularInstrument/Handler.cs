using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Instruments.Domain;

namespace Instruments.Service.Features.CreateRegularInstrument
{
    public class Handler :
            IHandleCommand<Command>
    {
        private readonly IRepository _repository;

        public Handler(IRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            return command.Vendor.TryParseEnum<Vendor>().Promise(v =>
                RegularInstrument.TryCreate(command.Id, v, command.Name).Promise(i =>
                    _repository.SaveAsync(i)
                )
            );
        }
    }
}
