﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Web;
using ExampleService.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

namespace ExampleService
{
    public record Link
    {
        public string? Href { get; init; }
        public HttpMethod? Method { get; init; }
    }

    public record Link<T> : Link where T : class
    {
        public T? Expected { get; init; }
    }

    public static class RestMapper
    {
        private static readonly Dictionary<PathString, (Type, Type, Func<object, object>?)> s_queries
            = new Dictionary<PathString, (Type, Type, Func<object, object>?)>();

        private static readonly Dictionary<PathString, Type> s_commands = new Dictionary<PathString, Type>();

        private static readonly Dictionary<Type, Link> s_linkIndex = new Dictionary<Type, Link>();

        private static readonly HashSet<Link> s_hash = new HashSet<Link>();

        private static Func<IEnumerable<Link>>? s_index = default;

        public static Func<IAggregateStore> GetAggregateStore { get; private set; } = () => throw new Exception();

        public static void SetAggregateStore(IAggregateStore aggregateStore) => GetAggregateStore = () => aggregateStore;

        public static Link? TryMapIndex(Func<IEnumerable<Link>> generator)
        {
            var link = new Link { Href = "/", Method = HttpMethod.Get };

            if (s_hash.Add(link))
            {
                s_index = generator;
                return link;
            }

            return null;
        }

        public static IEnumerable<T> Enumerate<T>(this T? maybeT) where T : class
        {
            if (maybeT is not null)
            {
                yield return maybeT;
            }
        }

        public static IEnumerable<Link> Enumerate(params object?[] maybeTs) => maybeTs.OfType<Link>();

        public static Link? TryMapCommand<TCommand>(PathString path) where TCommand : ICommand, new()
        {
            var link = new Link { Href = path, Method = HttpMethod.Post };

            return s_hash.Add(link) && s_commands.TryAdd(path, typeof(TCommand))
                && s_linkIndex.TryAdd(typeof(TCommand), link)
                    ? link
                    : null;
        }

        public static Link? TryMapQuery<TQuery, TDto>(PathString path, Func<TDto, object>? enrich = null) where TQuery : IQuery<TDto>, new()
        {
            var link = new Link { Href = path, Method = HttpMethod.Get };
            var castEnrich = enrich as Func<object, object>;

            return s_hash.Add(link)
                && s_linkIndex.TryAdd(typeof(TQuery), link)
                && s_queries.TryAdd(path, (typeof(TQuery), typeof(TDto), castEnrich))
                    ? link
                    : null;
        }

        public static Link<T> WithExpected<T>(this Link link, T expected) where T : class
            => new Link<T> { Href = link.Href, Method = link.Method, Expected = expected };
        public static Link<T> WithRouteValues<T>(this Link<T> link, object routeValues) where T : class
            => new Link<T> { Href = link?.Href?.ReplaceParameters(routeValues), Method = link?.Method, Expected = link?.Expected };
        public static Link WithRouteValues(this Link link, object routeValues)
            => new Link { Href = link?.Href?.ReplaceParameters(routeValues), Method = link?.Method };

        private static string ReplaceParameters(this string s, object routeValues)
        {
            var ret = s;

            foreach (var prop in routeValues.GetType().GetProperties())
            {
                var template = "{" + prop.Name + "}";
                var value = prop.GetValue(routeValues);
                ret = ret.Replace(template, value?.ToString());
            }

            return ret;
        }


        public static IEndpointRouteBuilder MapCommandsAndQueries(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            if (s_index != null)
            {
                RegisterIndex(endpointRouteBuilder, s_index);
            }

            RegisterCommands(endpointRouteBuilder);
            RegisterQueries(endpointRouteBuilder);

            return endpointRouteBuilder;
        }

        private static void RegisterIndex(IEndpointRouteBuilder endpointRouteBuilder, Func<IEnumerable<Link>> generator)
        {
            endpointRouteBuilder.MapGet("/", async httpContext =>
            {
                var result = generator().ToArray();
                httpContext.Response.StatusCode = StatusCodes.Status200OK;
                await JsonSerializer.SerializeAsync(httpContext.Response.Body, result, s_options, httpContext.RequestAborted);
            });
        }

        private static void RegisterQueries(IEndpointRouteBuilder endpointRouteBuilder)
        {
            foreach (var (path, (queryType, dtoType, enrich)) in s_queries)
            {
                endpointRouteBuilder.MapGet(path, async (httpContext) =>
                {
                    var token = httpContext.RequestAborted;

                    var query = httpContext.Request.GetQueryObject(queryType);

                    if (query == null)
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                        return;
                    }

                    var result = await (query as dynamic).Handle();

                    if (result == null)
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status204NoContent;
                        return;
                    }
                    if (enrich != null)
                    {
                        result = enrich(result);
                    }

                    httpContext.Response.StatusCode = StatusCodes.Status200OK;
                    await JsonSerializer.SerializeAsync(httpContext.Response.Body, result, s_options, token);
                });
            }
        }

        private static void RegisterCommands(IEndpointRouteBuilder endpointRouteBuilder)
        {
            foreach (var (path, commandType) in s_commands)
            {
                endpointRouteBuilder.MapPost(path, async (httpContext) =>
                {
                    var token = httpContext.RequestAborted;

                    if (!(await JsonSerializer.DeserializeAsync(httpContext.Request.Body, commandType, s_options, token) is ICommand command))
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                        return;
                    }

                    await command.Handle();
                    httpContext.Response.StatusCode = StatusCodes.Status202Accepted;
                });
            }
        }

        private static object? GetQueryObject(this HttpRequest request, Type type)
        {
            var queryString = request.QueryString.Value;

            if (queryString == null)
            {
                return null;
            }

            var dict = HttpUtility.ParseQueryString(queryString);

            if (dict == null)
            {
                return null;
            }

            var ding = dict.Cast<string>().ToDictionary(k => k, v => (object?)dict[v]);

            request?.RouteValues.ToList().ForEach(keValuePair =>
            {
                var (key, value) = keValuePair;
                ding.TryAdd(key, value);
            });

            var serialized = JsonSerializer.Serialize(ding, s_options);
            return JsonSerializer.Deserialize(serialized, type, s_options);
        }

        private static readonly JsonSerializerOptions s_options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
    }
}
