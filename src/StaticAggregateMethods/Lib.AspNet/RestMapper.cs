using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;
using Lib.Aggregates;
using Lib.EventSourcing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;

namespace Lib.AspNet
{
    public record Link(string Href, string Method)
    {
        public HydraClass? Expects { get; init; }
    }

    public class HydraClass
    {
        [JsonPropertyName("@id")]
        public string? @Id { get; init; }
        [JsonPropertyName("@type")]
        public string @Type { get; } = "hydra:Class";
        public IReadOnlyCollection<SupportedProperty> SupportedProperty { get; init; } = Array.Empty<SupportedProperty>();
        public string? Title { get; init; }
    }

    public class SupportedProperty
    {
        [JsonPropertyName("@type")]
        public string @Type { get; } = "SupportedProperty";
        public bool Required { get; init; } = true;
        public string? Title { get; init; }
        public dynamic? Default { get; init; }
    }

    public static class RestMapper
    {
        private static readonly Dictionary<Link, RequestDelegate> s_handlers = new Dictionary<Link, RequestDelegate>();
        private static readonly Dictionary<Type, object> s_aggregates = new Dictionary<Type, object>();

        private static bool TryAddAggregate<Aggregate, State>() where Aggregate : IAggregate<State>, new() where State : class
        {
            var type = typeof(Aggregate);

            if (s_aggregates.ContainsKey(type))
            {
                return false;
            }

            s_aggregates.Add(type, new Aggregate());
            return true;
        }

        private static IAggregate<State> GetAggregate<Aggregate, State>() where Aggregate : IAggregate<State>, new() where State : class
        {
            return s_aggregates[typeof(Aggregate)] as IAggregate<State> ?? throw new Exception();
        }

        public static Func<IEventStore> GetEventStore { get; private set; } = () => throw new Exception();
        public static void SetEventStore(IEventStore eventStore) => GetEventStore = () => eventStore;

        public static Link? TryMapIndex(IEnumerable<Link> links)
        {
            var linkArr = links.Cast<dynamic>().ToArray();

            async Task Handle(HttpContext httpContext)
            {
                httpContext.Response.StatusCode = StatusCodes.Status200OK;
                await JsonSerializer.SerializeAsync(httpContext.Response.Body, linkArr, Options, httpContext.RequestAborted);
            }

            var link = new Link("/", HttpMethods.Get);

            return s_handlers.TryAdd(link, Handle)
                ? link
                : null;
        }

        public static IEnumerable<T> Yield<T>(this T? maybeT) where T : class
        {
            if (maybeT != null)
            {
                yield return maybeT;
            }
        }

        public static IEnumerable<Link> Enumerate(params object[] maybeTs) => maybeTs.OfType<Link>();

        public static Link TryMapCommand<Aggregate, State, Command>(string path, Command? expected = null) where Command : class where State : class where Aggregate : IAggregate<State>, new()
        {
            TryAddAggregate<Aggregate, State>();

            var link = new Link(path, HttpMethods.Post);

            if (expected != null)
            {
                link = link.WithExpected(expected);
            }

            s_handlers.TryAdd(link, Handle<Aggregate, State, Command>);
            return link;
        }

        private static async Task Handle<Aggregate, State, Command>(HttpContext context)
            where Aggregate : IAggregate<State>, new()
            where State : class
            where Command : class
        {
            var token = context.RequestAborted;
            var command = await JsonSerializer.DeserializeAsync<Command>(context.Request.Body, Options, token);

            if (command is null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            var aggregate = GetAggregate<Aggregate, State>();

            var id = context.GetRouteValue("id");

            var commandEnvelope = new CommandEnvelope<Command>
            {
                Command = command,
                AggregateId = id is string aggregateId ? aggregateId : Guid.NewGuid().ToString()
            };

            var maybePosition = await AppService.Handle(commandEnvelope, aggregate, GetEventStore(), token);
            context.Response.StatusCode = StatusCodes.Status200OK;

            if (maybePosition is long p)
            {
                await context.Response.WriteAsync(p.ToString(), token);
            }
        }

        public static Link TryMapQuery<TQuery, TModel>(string path, Func<TModel, object>? enrich = null) where TQuery : class, IQuery<TModel> where TModel : class?
        {
            var link = new Link(path, HttpMethods.Get);

            enrich ??= (x) => x!;

            s_handlers.TryAdd(link, Handle);
            return link;

            async Task Handle(HttpContext context)
            {
                var token = context.RequestAborted;
                var query = context.Request.GetQueryObject<TQuery>();

                if (query == null)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                var (position, queryResult) = query.Handle();

                var requestHeaders = context.Request.GetTypedHeaders();

                var minimumPositions = requestHeaders.IfMatch.TryParsePositions();

                if (minimumPositions.Any(p => p > position))
                {
                    context.Response.StatusCode = StatusCodes.Status412PreconditionFailed;
                    context.Response.Headers.Add(HeaderNames.RetryAfter, "1");
                    return;
                }

                var maximumPositions = requestHeaders.IfNoneMatch.TryParsePositions().ToList();

                if (maximumPositions.Count > 0 && maximumPositions.All(x => x == position))
                {
                    context.Response.StatusCode = StatusCodes.Status304NotModified;
                    return;
                }

                var result = enrich(queryResult);

                if (result is Dictionary<string, object> dict)
                {
                    var matchingHandlers = s_handlers.Where(x => x.Key.Href == link.Href && !HttpMethods.IsGet(x.Key.Method)).Select(x => new
                    {
                        method = x.Key.Method,
                        expected = x.Key.Expects
                    });

                    dict["actions"] = matchingHandlers.ToArray();
                    result = dict;
                }

                if (result == null)
                {
                    context.Response.StatusCode = StatusCodes.Status204NoContent;
                    return;
                }

                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = MediaTypeNames.Application.Json;

                var etag = new EntityTagHeaderValue($@"""{ position }""");
                context.Response.GetTypedHeaders().ETag = etag;

                await context.Response.WriteAsJsonAsync(result, result.GetType(), Options, token);
                await context.Response.CompleteAsync();
            }
        }

        private static IEnumerable<long> TryParsePositions(this IEnumerable<EntityTagHeaderValue> values)
        {
            foreach (var item in values)
            {
                if (item.Tag.HasValue && long.TryParse(item.Tag.Value.Replace("\"", string.Empty), out var p))
                {
                    yield return p;
                }
            }
        }

        public static Link WithExpected<T>(this Link link, T t) where T : class
        {
            var type = typeof(T);
            var name = type.Name;
            var supportedProperties = type.GetProperties().Select(p => new SupportedProperty { Title = p.Name, Default = p.GetValue(t) }).ToArray();

            return link with { Expects = new HydraClass { Id = "hydra:" + name, Title = name, SupportedProperty = supportedProperties } };
        }

        public static Link WithRouteValues(this Link link, object routeValues)
            => link with { Href = link.Href.ReplaceParameters(routeValues) };

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
                if (link != null && handler != null)
                {
                    endpointRouteBuilder.MapMethods(link.Href ?? string.Empty, link.Method.Yield(), handler);
                }

            }

            return endpointRouteBuilder;
        }

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

            var ding = dict.OfType<string>().ToDictionary(k => k, v => (object?)dict[v]);

            foreach (var (key, value) in request.RouteValues)
            {
                ding.TryAdd(key, value);
            }

            var serialized = JsonSerializer.Serialize(ding, Options);
            return JsonSerializer.Deserialize<T>(serialized, Options);
        }

        public static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IgnoreNullValues = true
        };
    }
}
