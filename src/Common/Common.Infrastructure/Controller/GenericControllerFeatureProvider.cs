﻿using System;
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
            var commandTypes = assemblies.SelectMany(x=> x.GetTypes()).Where(x=> typeof(IRequest).IsAssignableFrom(x));

            var queryControllers = queryTypes.Select(GetQueryControllerType);
            var commandControllers = commandTypes.Select(GetCommandControllerType);


            _candidates = queryControllers.Concat(commandControllers).ToArray();
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

        private static TypeInfo GetCommandControllerType(Type commandType)
        {
            return typeof(CommandController<>).MakeGenericType(commandType).GetTypeInfo();
        }
    }
}
