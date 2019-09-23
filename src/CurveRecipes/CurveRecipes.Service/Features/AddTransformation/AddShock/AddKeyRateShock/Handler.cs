using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using CurveRecipes.Domain;

namespace CurveRecipes.Service.Features.AddTransformation.AddShock.AddKeyRateShock
{
    public class Handler : IHandleCommand<Command>
    {
        private readonly IRepository _repository;

        public Handler(IRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            var order = new Order(command.Order);

            var krsResult = ToKeyRateShock(command);

            return krsResult.Promise(async krs =>
            {
                var curve = await _repository.GetByIdAsync<CurveRecipe>(command.Id);
                var result = curve.AddTransformation(order, krs);
                return await result.Promise(() => _repository.SaveAsync(curve));
            });
        }

        private static Result<KeyRateShock> ToKeyRateShock(Command command)
        {
            var shift = new Shift(command.Shift);
            var maturitiesResult = command.Maturities
                .Select(m => Maturity.TryCreate(m))
                .Convert();

            return maturitiesResult
                .Promise(m => new KeyRateShock(command.ShockTarget, shift, m.ToArray()));
        }
    }
}
