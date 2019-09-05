using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        public ImmutableArray<double> Maturities { get; set; } = ImmutableArray<double>.Empty;

        public class Handler : IHandleCommand<AddKeyRateShock>
        {
            private readonly IRepository _repository;

            public Handler(IRepository repository)
            {
                _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            }

            public Task<Result> Handle(AddKeyRateShock command, CancellationToken cancellationToken)
            {
                var order = new Order(command.Order);

                var krsResult = command.ToKeyRateShock();

                return krsResult.Promise(async krs => 
                {
                    var curve = await _repository.GetByIdAsync<CurveRecipe>(command.Id);
                    var result = curve.AddTransformation(order, krs);
                    return await result.Promise(() => _repository.SaveAsync(curve));
                });
            }
        }
    }

    public static class AddKeyRateShockExtensions
    {
        public static Result<KeyRateShock> ToKeyRateShock(this AddKeyRateShock command)
        {
            var shift = new Shift(command.Shift);
            var maturitiesResult = command.Maturities
                .Select(m => Maturity.TryCreate(m))
                .Convert();

            return maturitiesResult
                .Promise(m=> new KeyRateShock(command.ShockTarget, shift, m.ToArray()));
        }
    }
}
