using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Core.Extensions;
using Common.Infrastructure.Extensions;
using Common.Infrastructure.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure.Controller
{
    public class QueryController<TQuery, TDto> : ControllerBase where TQuery : IQuery<TDto> where TDto : class
    {
        private static readonly string s_featureName = GetFeatureName();
        private readonly IHandleListQuery<TQuery, TDto> _handler;

        public QueryController(IHandleListQuery<TQuery, TDto> handler)
        {
            _handler = handler;
        }

        private ISocketContext GetSocket() => HttpContext.RequestServices.GetService<ISocketContext>();

        private static string GetFeatureName()
        {
            var nameSpaceParts = typeof(TQuery).FullName.Split('.');
            var length = nameSpaceParts.Length();
            var featureName = nameSpaceParts[Math.Max(0, length - 2)];
            return featureName.PascalToKebabCase();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] TQuery query, CancellationToken ct = default)
        {
            var result = await _handler.Handle(query, ct).ToListAsync();
            var socket = GetSocket();

            return socket != null
                ? this.ComponentActionResult(result, s_featureName, s_featureName)
                : this.ComponentActionResult(result, s_featureName);
        }
    }
}
