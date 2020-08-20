using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Infrastructure.Extensions;
using Common.Infrastructure.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure.Controller
{
    public class ListQueryController<TQuery, TDto> : ControllerBase where TQuery : IListQuery<TDto> where TDto : new ()
    {
        private static readonly string s_featureName = HelperMethods.GetFeatureName(typeof(TQuery));
        private readonly IHandleListQuery<TQuery, TDto> _handler;

        public ListQueryController(IHandleListQuery<TQuery, TDto> handler)
        {
            _handler = handler;
        }

        private ISocketContext GetSocket() => HttpContext.RequestServices.GetService<ISocketContext>();

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] TQuery query, CancellationToken ct = default)
        {
            var queryResult = await _handler.Handle(query, ct).ToListAsync(ct);
            var socket = GetSocket();

            var result = socket != null
                ? this.ComponentActionResult(queryResult, s_featureName, s_featureName)
                : this.ComponentActionResult(queryResult, s_featureName);

            return result;
        }
    }
}
