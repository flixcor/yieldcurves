using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using MarketCurves.Domain;

namespace MarketCurves.Service.Features.CreateMarketCurve
{
    public class Handler : 
        IHandleCommand<Command>,
        IHandleQuery<Query, Dto>
    {
        private readonly IRepository _repository;

        public Handler(IRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task<Result> Handle(Command command, CancellationToken cancellationToken)
        {

            var curveResult = MarketCurve.TryCreate(command.Id, command.Country, command.CurveType, command.FloatingLeg);
            return curveResult.Promise(c => _repository.SaveAsync(c));
        }

        public Task<Dto> Handle(Query query, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Dto());
        }
    }
}
