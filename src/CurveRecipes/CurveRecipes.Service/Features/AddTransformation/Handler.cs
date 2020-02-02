using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.EventStore.Lib.EfCore;
using CurveRecipes.Domain;
using Newtonsoft.Json.Linq;

namespace CurveRecipes.Service.Features.AddTransformation
{
    public class Handler :
        IHandleQuery<Query, Dto>,
        IHandleCommand<Command>
    {
        private readonly IAggregateRepository _repository;

        public Handler(IAggregateRepository repository)
        {
            _repository = repository;
        }

        public Task<Dto> Handle(Query query, CancellationToken cancellationToken)
        {
            var result = new Dto(query.RecipeId);
            return Task.FromResult(result);
        }

        public Result<ITransformation> TryMap(string transformationName, JObject transformation) => transformationName switch
        {
            nameof(KeyRateShock) => TryMap<AddKeyRateShock>(transformation)
                .Promise(c => TryMap(c)),

            nameof(ParallelShock) => TryMap<AddParallelShock>(transformation)
                .Promise(c => TryMap(c)),

            _ => Result.Fail<ITransformation>("No transformation found"),
        };

        private Result<T> TryMap<T>(JObject transformation)
        {
            var casted = transformation.ToObject<T>();

            return casted != null
                ? Result.Ok(casted)
                : Result.Fail<T>($"Could not parse {nameof(transformation)} to {typeof(T).Name}");
        }

        private static Result<ITransformation> TryMap(AddKeyRateShock command)
        {
            var shockTargetResult = command.ShockTarget.TryParseEnum<ShockTarget>();
            var maturitiesResult = command.Maturities
                .Select(m => Maturity.TryCreate(m))
                .Convert();

            return Result.Combine(shockTargetResult, maturitiesResult, (st, m) =>
            {
                var shift = new Shift(command.Shift);
                var krs = new KeyRateShock(st, shift, m.ToArray());
                return (ITransformation)krs;
            });
        }

        private static Result<ITransformation> TryMap(AddParallelShock command)
        {
            return command.ShockTarget.TryParseEnum<ShockTarget>()
                .Promise(st =>
                {
                    var shift = new Shift(command.Shift);
                    var ps = new ParallelShock(st, shift);
                    return (ITransformation)ps;
                });
        }

        public Task<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            var transformationResult = TryMap(command.TransformationName, command.Transformation);

            return transformationResult.Promise(async t =>
            {
                var c = await _repository.GetByIdAsync<CurveRecipe>(command.Id);

                if (c != null)
                {
                    var result = await c.AddTransformation(t)
                        .Promise(() => _repository.SaveAsync(c));

                    return result;
                }

                return Result.Fail("Not found");
            });
        }
    }
}
