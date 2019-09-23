using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;

namespace CurveRecipes.Service.Features.CreateCurveRecipe
{
    public class QueryHandler : IHandleQuery<Query, Dto>
    {
        private readonly IReadModelRepository<MarketCurveDto> _readModelRepository;

        public QueryHandler(IReadModelRepository<MarketCurveDto> readModelRepository)
        {
            _readModelRepository = readModelRepository ?? throw new ArgumentNullException(nameof(readModelRepository));
        }

        public async Task<Dto> Handle(Query query, CancellationToken cancellationToken)
        {
            var curves = await _readModelRepository.GetAll();

            return new Dto
            {
                Command = new Command
                {
                    Id = Guid.NewGuid()
                },
                MarketCurves = curves.Where(x => x.Tenors.Any())
            };
        }
    }
}
