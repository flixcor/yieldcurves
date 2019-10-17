using System;
using System.Threading.Tasks;
using Common.Core;
using Common.Infrastructure.Extensions;
using Common.Infrastructure.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure.Controller
{
    public class QueryController<TQuery, TDto> : ControllerBase where TQuery : IQuery<TDto> where TDto : class
    {
        private static readonly string _featureName = GetFeatureName();

        private bool HasSocket() => HttpContext.RequestServices.GetService<ISocketContext>() != null;

        private IRequestMediator GetMediator() => HttpContext.RequestServices.GetService<IRequestMediator>();

        private static string GetFeatureName()
        {
            var nameSpaceParts = typeof(TQuery).FullName.Split('.');
            var length = nameSpaceParts.Length();
            var featureName = nameSpaceParts[Math.Max(0, length - 2)];
            return featureName.PascalToKebabCase();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] TQuery query)
        {
            var result = await GetMediator().Send(query);

            return HasSocket() 
                ? this.HubComponentActionResult(result, _featureName) 
                : this.ComponentActionResult(result, _featureName);
        }
    }
}
