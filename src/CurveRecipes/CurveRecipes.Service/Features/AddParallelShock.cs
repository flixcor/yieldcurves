using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Common.Core;
using CurveRecipes.Domain;

namespace CurveRecipes.Service.Features
{
    public class AddParallelShock : ICommand
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public ShockTarget ShockTarget { get; set; }
        public double Shift { get; set; }

        public class Handler : IHandleCommand<AddParallelShock>
        {
            private readonly IMapper _mapper;
            private readonly IRepository _repository;

            public Handler(IMapper mapper, IRepository repository)
            {
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            }

            public async Task<Result> Handle(AddParallelShock command, CancellationToken cancellationToken)
            {
                var order = new Order(command.Order);
                var transformation = _mapper.Map<ParallelShock>(command);

                var c = await _repository.GetByIdAsync<CurveRecipe>(command.Id);

                if (c != null)
                {
                    c.AddTransformation(order, transformation);
                    await _repository.SaveAsync(c);
                    return Result.Ok();
                }

                return Result.Fail("Not found");
            }
        }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<AddParallelShock, ParallelShock>().ConstructUsing(x => new ParallelShock(x.ShockTarget, new Shift(x.Shift)));
            }
        }
    }
}
