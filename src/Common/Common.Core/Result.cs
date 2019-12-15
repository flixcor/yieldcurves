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

        public T Content { get; }
    }

    public static class ResultExtensions
    {
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

        public static Result<T> TryParseEnum<T>(this string input) where T : struct
        {
            return !Enum.TryParse<T>(input, true, out var result)
                ? Result.Fail<T>($"Unknown value for {typeof(T).Name}: {input}")
                : Result.Ok(result);
        }

        public static Result<T?> TryParseOptionalEnum<T>(this string input) where T : struct
        {
            T? optionalResult = null;

            if (!string.IsNullOrWhiteSpace(input))
            {
                if (!Enum.TryParse<T>(input, true, out var result))
                {
                    return Result.Fail<T?>($"Unknown value for {typeof(T).Name}: {input}");
                }

                optionalResult = result;
            }

            return Result.Ok(optionalResult);
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
    }

    public struct Nothing
    {

    }
}
