using Common.Core;
using Common.Core.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CurveRecipes.Query.Service.Features
{
    public class GetCurveRecipeList : IQuery<IEnumerable<GetCurveRecipesDto>>
    {
        public class Handler :
            IHandleQuery<GetCurveRecipeList, IEnumerable<GetCurveRecipesDto>>,
            IHandleEvent<CurveRecipeCreated>
        {
            private readonly IReadModelRepository<GetCurveRecipesDto> _readModelRepository;

            public Handler(IReadModelRepository<GetCurveRecipesDto> readModelRepository)
            {
                _readModelRepository = readModelRepository ?? throw new ArgumentNullException(nameof(readModelRepository));
            }

            public Task Handle(CurveRecipeCreated @event, CancellationToken cancellationToken)
            {
                var dto = new GetCurveRecipesDto
                {
                    Id = @event.Id,
                    Name = @event.ShortName
                };

                return _readModelRepository.Insert(dto);
            }

            public Task<IEnumerable<GetCurveRecipesDto>> Handle(GetCurveRecipeList query, CancellationToken cancellationToken)
            {
                return _readModelRepository.GetAll();
            }
        }
    }

    public class GetCurveRecipesDto : ReadObject
    {
        public string Name { get; set; }
    }
}
