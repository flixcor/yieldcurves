using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.EventStore.Lib;
using Common.Infrastructure;
using CurveRecipes.Domain;
using Newtonsoft.Json.Linq;

namespace CurveRecipes.Service.Features.AddTransformation
{
    public class Handler :
        ApplicationService<CurveRecipe>,
        IHandleQuery<Query, Dto>,
        IHandleCommand<Command>
    {
        public Handler(IAggregateRepository repository) : base(repository)
        {
        }

        public Task<Dto> Handle(Query query, CancellationToken cancellationToken)
        {
            var result = new Dto(query.RecipeId);
            return Task.FromResult(result);
        }

        public Either<Error,ITransformation> TryMap(string transformationName, JObject transformation) => transformationName switch
        {
            nameof(KeyRateShock) => TryMap<AddKeyRateShock>(transformation)
                .MapRight(c => TryMap(c)),

            nameof(ParallelShock) => TryMap<AddParallelShock>(transformation)
                .MapRight(c => TryMap(c)),

            _ => new Error("No transformation found"),
        };

        private Either<Error, T> TryMap<T>(JObject transformation)
        {
            var casted = transformation.ToObject<T>();

            if (casted == null)
            {
                return new Error($"Could not parse {nameof(transformation)} to {typeof(T).Name}");
            }

            return casted;
        }

        private static Either<Error, ITransformation> TryMap(AddKeyRateShock command)
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

        private static Either<Error, ITransformation> TryMap(AddParallelShock command)
        {
            return command.ShockTarget.TryParseEnum<ShockTarget>()
                .MapRight(st =>
                {
                    var shift = new Shift(command.Shift);
                    var ps = new ParallelShock(st, shift);
                    return (ITransformation)ps;
                });
        }

        public Task<Either<Error,Nothing>> Handle(Command command, CancellationToken cancellationToken)
        {
            return Handle(cancellationToken, command.Id.NonEmpty(), c =>
            {
                TryMap(command.TransformationName, command.Transformation)
                    .MapRight(t => 
                    {
                        return c.AddTransformation(t);
                    });
            });
        }
    }
}
