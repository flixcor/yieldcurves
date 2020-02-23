using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Infrastructure;
using MarketCurves.Domain;

namespace MarketCurves.Service.Features.CreateMarketCurve
{
    public class Handler :
        ApplicationService<MarketCurve>,
        IHandleCommand<Command>,
        IHandleQuery<Query, Dto>
    {

        public Handler(IAggregateRepository repository) : base(repository)
        {
        }

        public Task<Either<Error, Nothing>> Handle(Command command, CancellationToken cancellationToken)
            => Handle(cancellationToken, command.Id.NonEmpty(), c =>
            {
                c.Define(command.Country, command.CurveType, command.FloatingLeg);
            });

        public Task<Dto> Handle(Query query, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Dto());
        }
    }
}
