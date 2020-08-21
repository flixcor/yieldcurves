using System.Threading.Tasks;
using ExampleService.Shared;

namespace ExampleService.Features
{
    public record Test : IQuery<string?>
    {
        public string? Id { get; init; }
        public string? Name { get; init; }
        public string? Instrument { get; init; }

        public async Task<string?> Handle()
        {
            var command = new NameAndAdd { Id = Id?? System.Guid.NewGuid().ToString(), Instrument = Instrument, Name = Name };
            await command.Handle();
            return command.Id;
        }
    }
}
