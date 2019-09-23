using System.Threading;
using System.Threading.Tasks;
using Common.Core;

namespace Instruments.Service.Features.GetCommands
{
    public class Handler : IHandleQuery<Query, Dto>
    {
        public Task<Dto> Handle(Query query, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Dto());
        }
    }
}
