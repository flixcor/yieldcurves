using System;
using System.Threading.Tasks;

namespace Common.Core
{
    public class Maybe<T> : IMaybe where T : class
    {
        private readonly T _value;

        public bool Found => _value != null;

        public Maybe(T someValue)
        {
            _value = someValue;
        }

        private Maybe()
        {
        }

        public Maybe<TO> Bind<TO>(Func<T, TO> func) where TO : class => _value != null ? func(_value).Maybe() : Maybe<TO>.None();

        public T Coalesce(T otherwise) => _value ?? otherwise;
        public T Coalesce(Func<T> otherwise) => _value ?? otherwise();
        public async Task<T> Coalesce(Func<Task<T>> otherwise) => _value ?? await otherwise();

        public static Maybe<T> None() => new Maybe<T>();

        public Result<T> ToResult() => _value != null ? Result.Ok(_value) : Result.Fail<T>($"{typeof(T).Name} not found");
    }

    public static class MaybeExtensions
    {
        public static Maybe<T> Maybe<T>(this T value) where T : class
        {
            return value != null ? new Maybe<T>(value) : Core.Maybe<T>.None();
        }

        public static async Task<Maybe<T>> Maybe<T>(this ValueTask<T> valueTask) where T : class
        {
            var valueResult = await valueTask;
            return valueResult != null ? new Maybe<T>(valueResult) : Core.Maybe<T>.None();
        }

        public static async Task<Maybe<T>> Maybe<T>(this Task<T> task) where T : ReadObject
        {
            var value = await task;
            return value.Maybe();
        }

        public static async Task<Result<T>> ToResult<T>(this Task<Maybe<T>> task) where T : class
        {
            var maybe = await task;
            return maybe.ToResult();
        }
    }
}
