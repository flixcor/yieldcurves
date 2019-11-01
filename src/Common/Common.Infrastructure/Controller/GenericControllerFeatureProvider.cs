using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Core;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using static Common.Infrastructure.Controller.HelperMethods;

namespace Common.Infrastructure.Controller
{
    public class GenericControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly TypeInfo[] _candidates;

        public GenericControllerFeatureProvider(params Assembly[] assemblies)
        {
            var queryTypes = GetAllTypesImplementingOpenGenericType(typeof(IQuery<>), assemblies);
            var listQueryTypes = GetAllTypesImplementingOpenGenericType(typeof(IListQuery<>), assemblies);
            var commandTypes = assemblies.SelectMany(x=> x.GetTypes()).Where(x=> typeof(ICommand).IsAssignableFrom(x));

            var queryControllers = queryTypes.Select(GetQueryControllerType);
            var listQueryControllers = listQueryTypes.Select(GetListQueryControllerType);
            var commandControllers = commandTypes.Select(GetCommandControllerType);


            _candidates = queryControllers
                .Concat(listQueryControllers)
                .Concat(commandControllers)
                .ToArray();
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

        private static TypeInfo GetListQueryControllerType(Type listQueryType)
        {
            var argument2 = listQueryType.GetInterfaces()[0].GenericTypeArguments[0];
            return typeof(ListQueryController<,>).MakeGenericType(listQueryType, argument2).GetTypeInfo();
        }

        private static TypeInfo GetCommandControllerType(Type commandType)
        {
            return typeof(CommandController<>).MakeGenericType(commandType).GetTypeInfo();
        }
    }
}
