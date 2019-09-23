using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Common.Core;
using CurveRecipes.Domain;

namespace CurveRecipes.Service.Features.AddTransformation.AddShock.AddParallelShock
{
    public class Handler : IHandleCommand<Command>
    {
        private readonly IMapper _mapper;
        private readonly IRepository _repository;

        public Handler(IMapper mapper, IRepository repository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            var order = new Order(command.Order);
            var transformation = _mapper.Map<ParallelShock>(command);

            var c = await _repository.GetByIdAsync<CurveRecipe>(command.Id);

            if (c != null)
            {
                var result = c.AddTransformation(order, transformation);
                return await result.Promise(() => _repository.SaveAsync(c));
            }

            return Result.Fail("Not found");
        }
    }
}
