using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace Common.Infrastructure.Extensions
{
    public static class ControllerExtensions
    {
        public static string GetBaseUrl(this ControllerBase controller) =>
            $"{controller.Request.Scheme}://{controller.Request.Host}{controller.Request.PathBase}";

        public static string GetComponentUrl(this ControllerBase controller, string componentName)
        {
            var baseUrl = controller.GetBaseUrl();

            using var physicalProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
            var contents = physicalProvider.GetDirectoryContents("wwwroot");
            var fileName = contents
                .Where(x => x.Name.StartsWith($"{componentName}.") && x.Name.EndsWith("umd.js"))
                .OrderByDescending(f => f.LastModified)
                .Select(x => x.Name)
                .FirstOrDefault();

            return fileName == null
                ? null
                : $"{baseUrl}/{fileName}";
        }

        public static IActionResult ComponentActionResult(this ControllerBase controller, object t, string componentName, string hubName = null)
        {
            if (t == null)
            {
                return controller.NotFound();
            }

            if (t is IMaybe maybe)
            {
                if (!maybe.Found)
                {
                    return controller.NotFound();
                }

                t = ((dynamic)t).ToResult().Content;
            }

            var componentUrl = controller.GetComponentUrl(componentName);

            var hubUrl = !string.IsNullOrWhiteSpace(hubName)
                ? $"{controller.GetBaseUrl()}/hub?feature={hubName}"
                : null;

            return controller.Ok(FrontendComponent.Create(t, componentUrl, hubUrl));
        }

        public static IAsyncEnumerable<FrontendComponent<T>> FrontEndComponentAsyncEnumerable<T>(this ControllerBase controller, IAsyncEnumerable<T> t, string componentName, string hubName = null) where T : class
        {
            var componentUrl = controller.GetComponentUrl(componentName);

            var hubUrl = !string.IsNullOrWhiteSpace(hubName)
                ? $"{controller.GetBaseUrl()}/hub?feature={hubName}"
                : null;

            return t.Select(x=> FrontendComponent.Create(x, componentUrl, hubUrl))
        }
    }
}
