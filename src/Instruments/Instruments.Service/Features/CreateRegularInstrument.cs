using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Instruments.Domain;

namespace Instruments.Service.Features
{
    public class CreateRegularInstrument : ICommand
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Vendor Vendor { get; set; }

        public class Handler :
            IHandleCommand<CreateRegularInstrument>
        {
            private readonly IRepository _repository;

            public Handler(IRepository repository)
            {
                _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            }

            public async Task<Result> Handle(CreateRegularInstrument command, CancellationToken cancellationToken)
            {
                var instrument = new RegularInstrument(command.Id, command.Vendor, command.Name);
                await _repository.SaveAsync(instrument);
                return Result.Ok();
            }
        }
    }


}
