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
            var instrumentResult = RegularInstrument.TryCreate(command.Id, command.Vendor, command.Name);
            return instrumentResult.Promise(i => _repository.SaveAsync(i));
        }
    }
}
