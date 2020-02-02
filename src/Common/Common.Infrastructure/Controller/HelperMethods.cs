using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Core.Extensions;

namespace Common.Infrastructure.Controller
{
    public static class HelperMethods
    {
        public static string GetFeatureName(Type type)
        {
            var fullName = type.FullName ?? string.Empty;
            var nameSpaceParts = fullName.Split('.');
            var length = nameSpaceParts.Length();
            var featureName = nameSpaceParts[Math.Max(0, length - 2)];
            return featureName.PascalToKebabCase();
        }

        public static IEnumerable<Type> GetAllTypesImplementingOpenGenericType(Type openGenericType, Assembly[] assembly)
        {
            return from x in assembly.SelectMany(x => x.GetTypes())
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
