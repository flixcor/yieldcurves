using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Events;

namespace CurveRecipes.Query.Service.Features.GetCurveRecipesOverview
{
    public class Handler :
            IHandleListQuery<Query, Dto>,
            IHandleEvent<ICurveRecipeCreated>
    {
        private readonly IReadModelRepository<Dto> _readModelRepository;

        public Handler(IReadModelRepository<Dto> readModelRepository)
        {
            _readModelRepository = readModelRepository ?? throw new ArgumentNullException(nameof(readModelRepository));
        }

        public Task Handle(IEventWrapper<ICurveRecipeCreated> @event, CancellationToken cancellationToken)
        {
            var dto = new Dto
            {
                Id = @event.Metadata.AggregateId,
                Name = @event.GetContent().ShortName
            };

            return _readModelRepository.Insert(dto);
        }

        public IAsyncEnumerable<Dto> Handle(Query query, CancellationToken cancellationToken)
        {
            return _readModelRepository.GetAll();
        }
    }
}
