using System;
using Common.Infrastructure.Extensions;

namespace Common.Infrastructure.Controller
{
    public static class HelperMethods
    {
        public static string GetFeatureName(Type type)
        {
            var nameSpaceParts = type.FullName.Split('.');
            var length = nameSpaceParts.Length();
            var featureName = nameSpaceParts[Math.Max(0, length - 2)];
            return featureName.PascalToKebabCase();
        }
    }
}
