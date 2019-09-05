using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using MarketCurves.Domain;

namespace MarketCurves.Service.Features
{
    public static class CreateMarketCurve
    {
        public class Command : ICommand
        {
            public Guid Id { get; set; }
            public Country Country { get; set; }
            public CurveType CurveType { get; set; }
            public FloatingLeg? FloatingLeg { get; set; }

            public class Handler : IHandleCommand<Command>
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
            }
        }

        public class Query : IQuery<Query.Dto>
        {
            public class Handler : IHandleQuery<Query, Dto>
            {
                public Task<Dto> Handle(Query query, CancellationToken cancellationToken)
                {
                    return Task.FromResult(new Dto());
                }
            }

            public class Dto
            {
                public Command Command { get; set; } = new Command
                {
                    Id = Guid.NewGuid()
                };

                public IEnumerable<string> Countries { get; set; } = Enum.GetNames(typeof(Country));
                public IEnumerable<string> CurveTypes { get; set; } = Enum.GetNames(typeof(CurveType));
                public IEnumerable<string> FloatingLegs { get; set; } = Enum.GetNames(typeof(FloatingLeg));
            }
        }
    }
}
