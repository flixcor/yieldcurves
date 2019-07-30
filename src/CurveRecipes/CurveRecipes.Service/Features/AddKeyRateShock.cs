using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using CurveRecipes.Domain;

namespace CurveRecipes.Service.Features
{
    public class AddKeyRateShock : ICommand
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public ShockTarget ShockTarget { get; set; }
        public double Shift { get; set; }
        public double[] Maturities { get; set; }

        public class Handler : IHandleCommand<AddKeyRateShock>
        {
            private readonly IRepository _repository;

            public Handler(IRepository repository)
            {
                _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            }

            public async Task<Result> Handle(AddKeyRateShock command, CancellationToken cancellationToken)
            {
                var order = new Order(command.Order);

                var krs = command.ToKeyRateShock();

                var result = await _repository.GetByIdAsync<CurveRecipe>(command.Id);
                result.AddTransformation(order, krs);
                await _repository.SaveAsync(result);

                return Result.Ok();
            }
        }
    }

    public static class AddKeyRateShockExtensions
    {
        public static KeyRateShock ToKeyRateShock(this AddKeyRateShock command)
        {
            var shift = new Shift(command.Shift);
            var maturities = command.Maturities.Select(m => new Maturity(m)).ToArray();

            var result = new KeyRateShock(command.ShockTarget, shift, maturities);

            return result;
        }
    }
}
