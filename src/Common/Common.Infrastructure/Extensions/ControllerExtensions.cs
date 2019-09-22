using System.IO;
using System.Linq;
using Common.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace Common.Infrastructure.Extensions
{
    public static class ControllerExtensions
    {
        private static string GetBaseUrl(this ControllerBase controller) => 
            $"{controller.Request.Scheme}://{controller.Request.Host}{controller.Request.PathBase}";

        private static string GetComponentUrl(this ControllerBase controller, string componentName)
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

        public static IActionResult ComponentActionResult<T>(this ControllerBase controller, T obj, string componentName) where T : class
        {
            var url = controller.GetComponentUrl(componentName);
            
            return obj != null
                ? (ActionResult)controller.Ok(FrontendComponent.Create(obj, url))
                : controller.NotFound();
        }

        public static IActionResult ComponentActionResult<T>(this ControllerBase controller, Maybe<T> maybe, string componentName) where T : class
        {
            var url = controller.GetComponentUrl(componentName);
            
            return maybe.Found
                ? (ActionResult)controller.Ok(FrontendComponent.Create(maybe.ToResult().Content, url))
                : controller.NotFound();
        }

        public static IActionResult ComponentActionResult<T>(this ControllerBase controller, Result<T> result, string componentName) where T : class
        {
            var url = controller.GetComponentUrl(componentName);
            
            return result.IsSuccessful
                ? (ActionResult)controller.Ok(FrontendComponent.Create(result.Content, url))
                : controller.BadRequest(result.Messages);
        }

        public static IActionResult HubComponentActionResult<T>(this ControllerBase controller, T t, string componentName) where T : class
        {
            var hub = $"{controller.GetBaseUrl()}/hub";

            var url = controller.GetComponentUrl(componentName);
            
            return t != null
                ? (ActionResult)controller.Ok(FrontendComponent.Create(t, url, hub))
                : controller.NotFound();
        }
    }
}
