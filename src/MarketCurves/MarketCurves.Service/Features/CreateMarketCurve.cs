using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using MarketCurves.Domain;

namespace MarketCurves.Service.Features
{
    public class CreateMarketCurve : ICommand
    {
        public Guid Id { get; set; }
        public Country Country { get; set; }
        public CurveType CurveType { get; set; }
        public FloatingLeg? FloatingLeg { get; set; }

        public class Handler : IHandleCommand<CreateMarketCurve>
        {
            private readonly IRepository _repository;

            public Handler(IRepository repository)
            {
                _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            }

            public async Task<Result> Handle(CreateMarketCurve command, CancellationToken cancellationToken)
            {
                var curve = new MarketCurve(command.Id, command.Country, command.CurveType, command.FloatingLeg);
                await _repository.SaveAsync(curve);
                return Result.Ok();
            }
        }
    }
}
