using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common.Core.Extensions
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> FindDirectDescendants<T>(params Assembly[] assemblies)
        {
            var assembliesToScan = assemblies ?? AppDomain.CurrentDomain.GetAssemblies();
            return assembliesToScan.SelectMany(t => t.GetTypes()).Where(x => x.BaseType == typeof(T));
        }

        public static IEnumerable<Type> GetDescendantTypes(this Type baseType, params Assembly[] assemblies)
        {
            var assembliesToScan = assemblies != null && assemblies.Any() 
                ? assemblies 
                : AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => !a.GlobalAssemblyCache);

            var types = assembliesToScan.SelectMany(s => s.GetTypes());

            return types.Where(myType => myType.IsClass && !myType.IsAbstract
                && (myType.IsSubclassOf(baseType) || myType.IsSubclassOfRawGeneric(baseType)));
        }

        public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        public static T CreateInstance<T>(this Type type, params Type[] genericTypeArguments)
        {
            return type.CreateInstance<T>(Array.Empty<object>(), genericTypeArguments);
        }

        public static T CreateInstance<T>(this Type type, object[] args, params Type[] genericTypeArguments)
        {
            if (type.IsGenericType && genericTypeArguments != null && genericTypeArguments.Any())
            {
                var specificType = type.MakeGenericType(genericTypeArguments);

                return (T)Activator.CreateInstance(specificType, args);
            }

            return (T)Activator.CreateInstance(type, args);
        }

        public static bool TryMakeGenericType(this Type type, out Type genericType, params Type[] typeArguments)
        {
            try
            {
                genericType = type.MakeGenericType(typeArguments);
            }
            catch (ArgumentException)
            {
                genericType = null;
                return false;
            }
            return true;
        }

        public static bool TryMakeGenericType(this Type type, params Type[] typeArguments)
        {
            return type.TryMakeGenericType(out _, typeArguments);
        }
    }
}
