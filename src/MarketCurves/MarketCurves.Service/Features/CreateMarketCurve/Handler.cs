using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.EventStore.Lib;
using MarketCurves.Domain;
using static Common.Core.Result;

namespace MarketCurves.Service.Features.CreateMarketCurve
{
    public class Handler : 
        IHandleCommand<Command>,
        IHandleQuery<Query, Dto>
    {
        private readonly IAggregateRepository _repository;

        public Handler(IAggregateRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            var c = new MarketCurve().Define(command.Country, command.CurveType, command.FloatingLeg);
            await _repository.Save(c);
            return Ok();
        }

        public Task<Dto> Handle(Query query, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Dto());
        }
    }
}
