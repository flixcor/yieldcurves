using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Core
{
    public class Result
    {
        internal Result(bool isSuccessful, params string[] messages)
        {
            IsSuccessful = isSuccessful;
            Messages = messages.ToImmutableArray();
        }

        public static Result Ok() => new Result(true);

        public static Result Fail(params string[] messages) => new Result(false, messages);

        public static Result<T> Ok<T>(T obj) => new Result<T>(true, Array.Empty<string>(), obj);

        public static Result<T> Fail<T>(params string[] messages) => new Result<T>(false, messages);

        public Result<TOut> Promise<TOut>(Func<Result<TOut>> onSuccess)
        {
            return IsSuccessful 
                ? onSuccess()
                : Fail<TOut>(Messages.ToArray());
        }

        public Result<T> Promise<T>(Func<T> onSuccess)
        {
            return IsSuccessful
                ? Ok(onSuccess())
                : Fail<T>(Messages.ToArray());
        }

        public async Task<Result> Promise(Func<Task> onSuccess)
        {
            if (IsSuccessful)
            {
                await onSuccess();
                return Ok();
            }

            return Fail(Messages.ToArray());
        }

        public static Result Combine(params Result[] results)
        {
            return results.Any(x => !x.IsSuccessful)
                ? Fail(results.SelectMany(x => x.Messages).ToArray())
                : Ok();
        }

        public static Result<C> Combine<A, B, C>(Result<A> resultA, Result<B> resultB, Func<A, B, C> onSuccess)
        {
            if (resultA.IsSuccessful && resultB.IsSuccessful)
            {
                var result = onSuccess(resultA.Content, resultB.Content);
                return Ok(result);
            }

            var messages = resultA.Messages.Concat(resultB.Messages).ToArray();

            return Fail<C>(messages);
        }

        public static async Task<Result> Combine<A, B>(Result<A> resultA, Result<B> resultB, Func<A, B, Task> onSuccess)
        {
            if (resultA.IsSuccessful && resultB.IsSuccessful)
            {
                await onSuccess(resultA.Content, resultB.Content);
                return Ok();
            }

            var messages = resultA.Messages.Concat(resultB.Messages).ToArray();

            return Fail(messages);
        }

        public static Result<C> Combine<A, B, C>(Result<A> resultA, Result<B> resultB, Func<A, B, Result<C>> onSuccess)
        {
            if (resultA.IsSuccessful && resultB.IsSuccessful)
            {
                return onSuccess(resultA.Content, resultB.Content);
            }

            var messages = resultA.Messages.Concat(resultB.Messages).ToArray();

            return Fail<C>(messages);
        }

        public static async Task<Result> Combine<A, B, C>(Result<A> resultA, Result<B> resultB, Result<C> resultC, Func<A, B, C, Task> onSuccess)
        {
            if (resultA.IsSuccessful && resultB.IsSuccessful && resultC.IsSuccessful)
            {
                await onSuccess(resultA.Content, resultB.Content, resultC.Content);
                return Ok();
            }

            var messages = resultA.Messages.Concat(resultB.Messages).Concat(resultC.Messages).ToArray();

            return Fail(messages);
        }

        public static async Task<Result> Combine<A, B, C, D, E, F, G>(Result<A> resultA, Result<B> resultB, Result<C> resultC, Result<D> resultD, Result<E> resultE, Result<F> resultF, Result<G> resultG, 
            Func<A, B, C, D, E, F, G, Task> onSuccess)
        {
            var results = new Result[] { resultA, resultB, resultC, resultD, resultE, resultF, resultG };

            if (results.All(x=> x.IsSuccessful))
            {
                await onSuccess(resultA.Content, resultB.Content, resultC.Content, resultD.Content, resultE.Content, resultF.Content, resultG.Content);
                return Ok();
            }

            return Fail(results.SelectMany(x=> x.Messages).ToArray());
        }

        public static Result<T> Combine<A, B, C, D, E, F, G, T>(Result<A> resultA, Result<B> resultB, Result<C> resultC, Result<D> resultD, Result<E> resultE, Result<F> resultF, Result<G> resultG,
            Func<A, B, C, D, E, F, G, T> onSuccess)
        {
            var results = new Result[] { resultA, resultB, resultC, resultD, resultE, resultF, resultG };

            if (results.All(x => x.IsSuccessful))
            {
                var result = onSuccess(resultA.Content, resultB.Content, resultC.Content, resultD.Content, resultE.Content, resultF.Content, resultG.Content);
                return Ok(result);
            }

            return Fail<T>(results.SelectMany(x => x.Messages).ToArray());
        }

        public bool IsSuccessful { get; }
        public ImmutableArray<string> Messages { get; }
    }

    public class Result<T> : Result
    {
        internal Result(bool success, string[] messages, T content = default) : base(success, messages)
        {
            Content = content;
        }

        public Result<TOut> Promise<TOut>(Func<T, TOut> onSuccess)
        {
            return IsSuccessful
                ? Ok(onSuccess(Content))
                : Fail<TOut>(Messages.ToArray());
        }

        public Result<TOut> Promise<TOut>(Func<T, Result<TOut>> onSuccess)
        {
            return IsSuccessful
                ? onSuccess(Content)
                : Fail<TOut>(Messages.ToArray());
        }

        public async Task<Result> Promise(Func<T, Task> onSuccess)
        {
            if (IsSuccessful)
            {
                await onSuccess(Content);
                return Ok();
            }

            return Fail(Messages.ToArray());
        }

        public Result Promise(Action<T> onSuccess)
        {
            if (IsSuccessful)
            {
                onSuccess(Content);
                return Ok();
            }

            return Fail(Messages.ToArray());
        }

        public async Task<Result> Promise(Func<T, Task<Result>> onSuccess)
        {
            return IsSuccessful
                ? await onSuccess(Content)
                : Fail(Messages.ToArray());
        }

        internal T Content { get; }
    }

    public static class ResultExtensions
    {
        public static Result<T> ToResult<T>(this T? maybe) where T : class
        {
            return maybe == null ? Result.Fail<T>("None") : Result.Ok(maybe);
        }

        public static async Task<Result<T>> ToResult<T>(this Task<T?> maybeTask) where T : class
        {
            var maybe = await maybeTask;
            return maybe.ToResult();
        }

        public static Result<IEnumerable<T>> Convert<T>(this IEnumerable<Result<T>> results)
        {
            if (results.Any(x => !x.IsSuccessful))
            {
                var failureMessages = results.SelectMany(x => x.Messages).ToArray();
                return Result.Fail<IEnumerable<T>>(failureMessages);
            }

            var objects = results.Select(x => x.Content);

            return Result.Ok(objects);
        }

        public static Result<ImmutableArray<T>> Convert<T>(this ImmutableArray<Result<T>> results)
        {
            if (results.Any(x => !x.IsSuccessful))
            {
                var failureMessages = results.SelectMany(x => x.Messages).ToArray();
                return Result.Fail<ImmutableArray<T>>(failureMessages);
            }

            var objects = results.Select(x => x.Content).ToImmutableArray();

            return Result.Ok(objects);
        }

        public static Either<Error, T> TryParseEnum<T>(this string input) where T : struct
        {
            if (!Enum.TryParse<T>(input, true, out var result))
            {
                return new Error($"Unknown value for {typeof(T).Name}: {input}");
            }

            return result;
        }

        public static Either<Error, T?> TryParseOptionalEnum<T>(this string input) where T : struct
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                if (!Enum.TryParse<T>(input, true, out var result))
                {
                    return new Error($"Unknown value for {typeof(T).Name}: {input}");
                }

                return result;
            }

            return (T?)null;
        }

        public static async Task<Result<TOut>> Promise<T, TOut>(this Task<Result<T>> task, Func<T, Task<Result<TOut>>> onSuccess)
        {
            var result = await task;

            if (result.IsSuccessful)
            {
                var onSuccesTask = onSuccess(result.Content);
                return await onSuccesTask;
            }

            return Result.Fail<TOut>(result.Messages.ToArray());
        }


        public static async Task<Result> Promise<T>(this Task<Result<T>> task, Func<T, Task> onSuccess)
        {
            var result = await task;

            if (result.IsSuccessful)
            {
                var t = onSuccess(result.Content);
                await t;
                return Result.Ok();
            }

            return Result.Fail(result.Messages.ToArray());
        }

        public static async Task<Result<TOut>> Promise<T,TOut>(this Task<Result<T>> task, Func<T, Task<TOut>> onSuccess)
        {
            var result = await task;

            if (result.IsSuccessful)
            {
                var t = onSuccess(result.Content);
                return Result.Ok(await t);
            }

            return Result.Fail<TOut>(result.Messages.ToArray());
        }

        public static async Task<Result> Promise<T>(this Task<Result<T>> task, Func<T, Task<Result>> onSuccess)
        {
            var result = await task;

            if (result.IsSuccessful)
            {
                var t = onSuccess(result.Content);
                return await t;
            }

            return Result.Fail(result.Messages.ToArray());
        }

        public static async Task<Either<TNewLeft, TRight>> MapLeft<TLeft, TRight, TNewLeft>(this Task<Either<TLeft, TRight>> task, Func<TLeft, TNewLeft> mapping)
        {
            var result = await task;
            return result.MapLeft(mapping);
        }

        public static async Task<Either<TLeft, TNewRight>> MapRight<TLeft, TRight, TNewRight>(this Task<Either<TLeft, TRight>> task, Func<TRight, TNewRight> mapping)
        {
            var result = await task;
            return result.MapRight(mapping);
        }

        public static async Task<TLeft> MapRight<TLeft, TRight>(this Task<Either<TLeft, TRight>> task, Func<TRight, TLeft> mapping)
        {
            var result = await task;
            return result.Reduce(mapping);
        }

        public static async Task IfNotNull<T>(this Task<T?> task, Func<T, Task> func) where T : class
        {
            var value = await task;

            if (value is null)
            {
                return;
            }

            await func(value);
        }

        public static async Task<TOut?> IfNotNull<T, TOut>(this Task<T?> task, Func<T, Task<TOut>> func) where T : class where TOut : class
        {
            var value = await task;

            if (value is null)
            {
                return null;
            }

            return await func(value);
        }

        public static async Task<TOut?> IfNotNull<T, TOut>(this Task<T?> task, Func<T, TOut> func) where T : class where TOut : class
        {
            var value = await task;

            if (value is null)
            {
                return null;
            }

            return func(value);
        }

        public static ValueTask IfNotNull<T>(T? value, Func<T, Task> func) where T : class
        {
            if (value is null)
            {
                return new ValueTask();
            }

            return new ValueTask(func(value));
        }

        public static Either<Error, T> ToEither<T>(this Result<T> result)
        {
            if (result.IsSuccessful)
            {
                return result.Content;
            }

            return new Error(result.Messages.ToArray());
        }

        public static ValueTask<Either<Error,Nothing>> Reduce(this Either<Error,Task> input)
        {
            return input
                .MapLeft(error => new ValueTask<Either<Error, Nothing>>(error))
                .Reduce(r => new ValueTask<Either<Error, Nothing>>(r.ToEither()));
        }

        public static async Task<Either<Error,Nothing>> ToEither(this Task task)
        {
            await task;
            return Nothing.Instance;
        }

        public static T Reduce<T>(this Either<T, T> either) => either.Reduce(x => x);

        public static TLeft Reduce<TLeft, TRight>(this Either<TLeft, TRight> either) 
            where TRight : TLeft
                => either.Reduce(x => x);

        public static Either<Error, TNewRight> MapRight<TRight1, TRight2, TNewRight>(this Either<Error, TRight1> either1, Either<Error, TRight2> either2, Func<TRight1, TRight2, TNewRight> mapping)
        {
            if (either1 is Right<Error, TRight1> right1 && either2 is Right<Error, TRight2> right2)
            {
                return mapping(right1.Value, right2.Value);
            }

            var objArray = new object[] { either1, either2 };

            var messages = objArray.OfType<Error>().SelectMany(e => e.Messages).ToArray();

            return new Error(messages);
        }

        public static Either<Error, TNewRight> MapRight<TRight1, TRight2, TRight3, TNewRight>(this Either<Error, TRight1> either1, Either<Error, TRight2> either2, Either<Error, TRight3> either3, Func<TRight1, TRight2, TRight3, TNewRight> mapping)
        {
            if (either1 is Right<Error, TRight1> right1 && either2 is Right<Error, TRight2> right2 && either3 is Right<Error, TRight3> right3)
            {
                return mapping(right1.Value, right2.Value, right3.Value);
            }

            var objArray = new object[] { either1, either2, either3};

            var messages = objArray.OfType<Error>().SelectMany(e => e.Messages).ToArray();

            return new Error(messages);
        }

        public static Either<Error, TNewRight> MapRight<TRight1, TRight2, TRight3, TRight4, TRight5, TRight6, TRight7, TNewRight>(this Either<Error, TRight1> either1, Either<Error, TRight2> either2, Either<Error, TRight3> either3, Either<Error, TRight4> either4, Either<Error, TRight5> either5, Either<Error, TRight6> either6, Either<Error, TRight7> either7, Func<TRight1, TRight2, TRight3, TRight4, TRight5, TRight6, TRight7, TNewRight> mapping)
        {
            if (either1 is TRight1 right1 && either2 is TRight2 right2 && either3 is TRight3 right3 && either4 is TRight4 right4 && either5 is TRight5 right5 && either6 is TRight6 right6 && either7 is TRight7 right7)
            {
                return mapping(right1, right2, right3, right4, right5, right6, right7);
            }

            var objArray = new object[] { either1, either2, either3, either4, either5, either6, either7 };

            var messages = objArray.OfType<Error>().SelectMany(e => e.Messages).ToArray();

            return new Error(messages);
        }

        public static Either<Error, ICollection<TRight>> Flatten<TRight>(this IEnumerable<Either<Error, TRight>> eithers)
        {
            var messages = new List<string>();
            ICollection<TRight> rights = new List<TRight>();

            foreach (var item in eithers)
            {
                if (item is Error error)
                {
                    messages.AddRange(error.Messages);
                }
                if (item is TRight right)
                {
                    rights.Add(right);
                }
            }

            return messages.Any() 
                ? new Error(messages) 
                : (Either<Error, ICollection<TRight>>)rights;
        }


        public static Either<Error, TRight> Flatten<TRight>(this Either<Error, Either<Error, TRight>> doubled)
        {
            if (doubled is Either<Error, TRight> sub)
            {
                return sub;
            }

            if (doubled is Error error)
            {
                return error;
            }

            throw new NotImplementedException();
        }
    }

    public struct Nothing
    {
        public static Nothing Instance { get; } = new Nothing();
    }

    public abstract class Either<TLeft, TRight>
    {
        public abstract Either<TNewLeft, TRight> MapLeft<TNewLeft>(Func<TLeft, TNewLeft> mapping);
        public abstract Either<TLeft, TNewRight> MapRight<TNewRight>(Func<TRight, TNewRight> mapping);
        public abstract Either<TLeft, TNewRight> MapRight<TNewRight>(Func<TRight, Either<TLeft, TNewRight>> mapping);
        public abstract TLeft Reduce(Func<TRight, TLeft> mapping);

        public static implicit operator Either<TLeft, TRight>(TLeft left) => new Left<TLeft, TRight>(left);
        public static implicit operator Either<TLeft, TRight>(TRight right) => new Right<TLeft, TRight>(right);
    }

    public class Left<TLeft, TRight> : Either<TLeft, TRight>
    {
        public TLeft Value { get; }

        public Left(TLeft value) => Value = value;

        public override Either<TNewLeft, TRight> MapLeft<TNewLeft>(Func<TLeft, TNewLeft> mapping) 
            => new Left<TNewLeft, TRight>(mapping(Value));

        public override Either<TLeft, TNewRight> MapRight<TNewRight>(Func<TRight, TNewRight> mapping) 
            => new Left<TLeft, TNewRight>(Value);

        public override TLeft Reduce(Func<TRight, TLeft> mapping) => Value;

        public override Either<TLeft, TNewRight> MapRight<TNewRight>(Func<TRight, Either<TLeft, TNewRight>> mapping)
            => new Left<TLeft, TNewRight>(Value);

        public static implicit operator TLeft(Left<TLeft, TRight> left) => left.Value;
    }

    public class Right<TLeft, TRight> : Either<TLeft, TRight>
    {
        public TRight Value { get; }

        public Right(TRight value) => Value = value;

        public override Either<TNewLeft, TRight> MapLeft<TNewLeft>(Func<TLeft, TNewLeft> mapping)
            => new Right<TNewLeft, TRight>(Value);

        public override Either<TLeft, TNewRight> MapRight<TNewRight>(Func<TRight, TNewRight> mapping)
            => new Right<TLeft, TNewRight>(mapping(Value));

        public override TLeft Reduce(Func<TRight, TLeft> mapping) 
            => mapping(Value);

        public override Either<TLeft, TNewRight> MapRight<TNewRight>(Func<TRight, Either<TLeft, TNewRight>> mapping)
            => mapping(Value);

        public static implicit operator TRight(Right<TLeft, TRight> right) => right.Value;
    }

    public class Ok<T>
    {
        public Ok(T data)
        {
            Data = data;
        }

        public T Data { get; }
    }

    public class Error
    {
        public Error(params string[] messages)
        {
            Messages = messages;
        }

        public Error(List<string> messages)
        {
            Messages = messages.ToArray();
        }

        public static implicit operator Error(string message) => new Error(message);

        public string[] Messages { get; }
    }
}
