using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Core;
using Common.Core.Extensions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Common.Infrastructure.Controller
{
    public class QueryControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly TypeInfo[] _candidates;

        public QueryControllerFeatureProvider(params Assembly[] assemblies)
        {
            var types = GetAllTypesImplementingOpenGenericType(typeof(IQuery<>), assemblies);

            _candidates = types
                .Select(GetQueryControllerType).ToArray();
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var candidate in _candidates)
            {
                feature.Controllers.Add(candidate);
            }
        }

        private static TypeInfo GetQueryControllerType(Type queryType)
        {
            var argument2 = queryType.GetInterfaces()[0].GenericTypeArguments[0];
            return typeof(QueryController<,>).MakeGenericType(queryType, argument2).GetTypeInfo();
        }

        public static IEnumerable<Type> GetAllTypesImplementingOpenGenericType(Type openGenericType, Assembly[] assembly)
        {
            return from x in assembly.SelectMany(x=> x.GetTypes())
                   from z in x.GetInterfaces()
                   let y = x.BaseType
                   where
                   (y != null && y.IsGenericType &&
                   openGenericType.IsAssignableFrom(y.GetGenericTypeDefinition())) ||
                   (z.IsGenericType &&
                   openGenericType.IsAssignableFrom(z.GetGenericTypeDefinition()))
                   select x;
        }
    }
}
