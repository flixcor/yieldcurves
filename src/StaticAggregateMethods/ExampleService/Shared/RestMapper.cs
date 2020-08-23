using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using static ExampleService.Shared.Handlers;

namespace ExampleService.Shared
{
    public record Link
    {
        public string? Href { get; init; }
        public string? Method { get; init; }
        public HydraClass? Expects { get; init; }
    }

    public record HydraClass
    {
        public string? @Id { get; init; }
        public string? @Type { get; } = "hydra:Class";
        public IReadOnlyCollection<SupportedProperty> SupportedProperty { get; init; } = Array.Empty<SupportedProperty>();
        public string? Title { get; init; }
    }

    public record SupportedProperty
    {
        public string @Type { get; } = "SupportedProperty";
        public bool Required { get; init; } = true;
        public string? Title { get; init; }
        public dynamic? Default { get; init; }
    }

    public static class RestMapper
    {
        private static readonly Dictionary<Link, RequestDelegate> s_handlers = new Dictionary<Link, RequestDelegate>();

        public static Func<IAggregateStore> GetAggregateStore { get; private set; } = () => throw new Exception();

        public static IEventStore EventStore { get; set; } = new InMemoryEventStore();
        public static void SetAggregateStore(IAggregateStore aggregateStore) => GetAggregateStore = () => aggregateStore;

        public static Link? TryMapIndex(IEnumerable<Link> links)
        {
            var linkArr = links.Cast<dynamic>().ToArray();

            async Task Handle(HttpContext httpContext)
            {
                httpContext.Response.StatusCode = StatusCodes.Status200OK;
                await JsonSerializer.SerializeAsync(httpContext.Response.Body, linkArr, Options, httpContext.RequestAborted);
            }

            var link = new Link { Href = "/", Method = HttpMethods.Get };

            return s_handlers.TryAdd(link, Handle)
                ? link
                : null;
        }

        public static IEnumerable<T> Enumerate<T>(this T? maybeT) where T : class
        {
            if (maybeT is not null)
            {
                yield return maybeT;
            }
        }

        public static IEnumerable<Link> Enumerate(params object?[] maybeTs) => maybeTs.OfType<Link>();

        public static Link? TryMapCommand<TCommand, TState>(Handlers.CommandHandler<TCommand, TState> handler, PathString path) where TCommand : class, new() where TState : class, new()
        {
            var link = new Link { Href = path, Method = HttpMethods.Post };

            async Task Handle(HttpContext httpContext)
            {
                var token = httpContext.RequestAborted;
                var command = await JsonSerializer.DeserializeAsync<TCommand>(httpContext.Request.Body, Options, token);

                if (command is null)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                await AppService.Handle(handler, new CommandEnvelope<TCommand> { Command = command }, EventStore);
                httpContext.Response.StatusCode = StatusCodes.Status202Accepted;
            }

            return s_handlers.TryAdd(link, Handle)
                ? link
                : null;
        }

        public static Link? TryMapQuery<TQuery, TProjection>(PathString path, Func<TProjection, object?>? enrich = null) where TQuery : class, IQuery<TProjection>, new() where TProjection : class, new()
        {
            var link = new Link { Href = path, Method = HttpMethods.Get };

            enrich ??= (x) => x;

            async Task Handle(HttpContext context)
            {
                var token = context.RequestAborted;
                var query = context.Request.GetQueryObject<TQuery>();
                var projection = await InMemoryProjectionStore.GetAsync<TProjection>();

                if (query == null || projection == null)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                var queryResult = query.Handle(projection);

                if (queryResult == null)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                var result = enrich != null ? enrich(queryResult) : queryResult;

                if (result == null)
                {
                    context.Response.StatusCode = StatusCodes.Status204NoContent;
                    return;
                }

                context.Response.StatusCode = StatusCodes.Status200OK;
                await JsonSerializer.SerializeAsync(context.Response.Body, result, result.GetType(), Options, token);
            }



            return s_handlers.TryAdd(link, Handle)
                    ? link
                    : null;
        }

        public static Link WithExpected<T>(this Link link, T t) where T : class
        {
            var type = typeof(T);
            var name = type.Name;
            var supportedProperties = type.GetProperties().Select(p => new SupportedProperty { Title = p.Name, Default = p.GetValue(t) }).ToArray();

            return link with
            {
                Expects = new HydraClass { Id = "hydra:" + name, Title = name, SupportedProperty = supportedProperties }
            };
        }

        public static Link WithRouteValues(this Link link, object routeValues)
            => link with { Href = link?.Href?.ReplaceParameters(routeValues) };

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
            foreach (var (link, handler) in s_handlers)
            {
                endpointRouteBuilder.MapHandler(link, handler);
            }

            return endpointRouteBuilder;
        }

        private static IEndpointConventionBuilder MapHandler(this IEndpointRouteBuilder endpointRouteBuilder, Link? link, RequestDelegate? handler) =>
            link?.Href == null || handler == null
                ? throw new Exception()
                : link.Method?.ToLowerInvariant() switch
                {
                    "get" => endpointRouteBuilder.MapGet(link.Href, handler),
                    "post" => endpointRouteBuilder.MapPost(link.Href, handler),
                    _ => throw new Exception()
                };

        private static T? GetQueryObject<T>(this HttpRequest request) where T : class
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

            var serialized = JsonSerializer.Serialize(ding, Options);
            return JsonSerializer.Deserialize<T>(serialized, Options);
        }

        public static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IgnoreNullValues = true
        }.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
    }
}
