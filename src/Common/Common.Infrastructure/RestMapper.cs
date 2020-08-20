using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Web;
using Common.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure
{
    public class Link
    {
        public Link(string href, HttpMethod method)
        {
            Href = href ?? throw new ArgumentNullException(nameof(href));
            Method = method ?? throw new ArgumentNullException(nameof(method));
        }

        public string Href { get; }
        public HttpMethod Method { get; }

    }

    public class Link<T> : Link
    {
        public Link(string href, HttpMethod method, T expected) : base(href, method)
        {
            Expected = expected;
        }

        public T Expected { get; }
    }

    public static class RestMapper
    {


        private static readonly Dictionary<PathString, (Type, Type, Func<object, object>?)> s_queries = new Dictionary<PathString, (Type, Type, Func<object, object>?)>();
        private static readonly Dictionary<PathString, Type> s_commands = new Dictionary<PathString, Type>();

        private static readonly Dictionary<Type, Link> s_linkIndex = new Dictionary<Type, Link>();

        public static Link TryMapListQuery<TQuery, TDto>(PathString path, Func<TDto, object>? enrich = null) where TQuery : IListQuery<TDto> where TDto : new()
        {
            if (s_queries.TryAdd(path, (typeof(TQuery), typeof(TDto), enrich as Func<object, object>)))
            {
                var link = new Link(path, HttpMethod.Get);

                if (s_linkIndex.TryAdd(typeof(TQuery), link))
                {
                    return link;
                }
                
            }

            return s_linkIndex[typeof(TQuery)];
        }

        public static Link TryMapQuery<TQuery, TDto>(PathString path, Func<TDto, object>? enrich = null) where TQuery : IQuery<TDto> where TDto : new()
        {
            if (s_queries.TryAdd(path, (typeof(TQuery), typeof(TDto), enrich as Func<object, object>)))
            {
                var link = new Link(path, HttpMethod.Get);

                if (s_linkIndex.TryAdd(typeof(TQuery), link))
                {
                    return link;
                }

            }

            return s_linkIndex[typeof(TQuery)];
        }

        public static Link TryMapCommand<TCommand>(PathString path) where TCommand : ICommand
        {
            if (s_commands.TryAdd(path, typeof(TCommand)))
            {
                var link = new Link(path, HttpMethod.Get);

                if (s_linkIndex.TryAdd(typeof(TCommand), link))
                {
                    return link;
                }

            }

            return s_linkIndex[typeof(TCommand)];
        }

        public static Link<T> WithExpected<T>(this Link link, T expected) => new Link<T>(link.Href, link.Method, expected);
        public static Link<T> WithRouteValues<T>(this Link<T> link, object routeValues) => new Link<T>(link.Href.ReplaceParameters(routeValues), link.Method, link.Expected);
        public static Link WithRouteValues(this Link link, object routeValues) => new Link(link.Href.ReplaceParameters(routeValues), link.Method);

        public static string ReplaceParameters(this string s, object routeValues)
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


        public static IEndpointRouteBuilder RegisterCommandsAndQueries(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            RegisterCommands(endpointRouteBuilder);
            RegisterQueries(endpointRouteBuilder);

            return endpointRouteBuilder;
        }

        private static void RegisterQueries(IEndpointRouteBuilder endpointRouteBuilder)
        {
            foreach (var (path, (queryType, dtoType, enrich)) in s_queries)
            {
                endpointRouteBuilder.MapGet(path, async (httpContext) =>
                {
                    var token = httpContext.RequestAborted;
                    var handlerType = typeof(IHandleQuery<,>).MakeGenericType(queryType, dtoType);
                    var handler = httpContext.RequestServices.GetRequiredService(handlerType) as dynamic;

                    if (handler == null)
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        return;
                    }

                    var query = httpContext.Request.GetFromQueryString(queryType);

                    if (query == null)
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                        return;
                    }

                    var result = await handler.Handle(query, token);

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
                    await JsonSerializer.SerializeAsync(httpContext.Response.Body, result, dtoType);
                });
            }
        }

        public static object? GetFromQueryString(this HttpRequest request, Type type)
        {
            var dict = HttpUtility.ParseQueryString(request.QueryString.Value);

            if (dict == null)
            {
                return null;
            }

            var ding = dict.Cast<string>().ToDictionary(k => k, v => (object)dict[v]);


            request.RouteValues.ToList().ForEach(keValuePair => 
            {
                var (key, value) = keValuePair;
                ding.TryAdd(key, value);
            });

            var serialized = JsonSerializer.Serialize(ding);
            return JsonSerializer.Deserialize(serialized, type);
        }

        private static void RegisterCommands(IEndpointRouteBuilder endpointRouteBuilder)
        {
            foreach (var (path, commandType) in s_commands)
            {
                endpointRouteBuilder.MapPost(path, async (httpContext) =>
                {
                    var token = httpContext.RequestAborted;
                    var handlerType = typeof(IHandleCommand<>).MakeGenericType(commandType);
                    var handler = httpContext.RequestServices.GetRequiredService(handlerType) as dynamic;

                    if (handler == null)
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        return;
                    }

                    if (!(await JsonSerializer.DeserializeAsync(httpContext.Request.Body, commandType, cancellationToken: token) is ICommand command))
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                        return;
                    }

                    Either<Error, Nothing> result = await handler.Handle(command, token);

                    httpContext.Response.StatusCode = result
                        .MapLeft(_ => StatusCodes.Status400BadRequest)
                        .Reduce(_ => StatusCodes.Status200OK);
                });
            }
        }
    }
}
