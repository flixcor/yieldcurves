using System.Threading;
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
        private static readonly string s_featureName = HelperMethods.GetFeatureName(typeof(TQuery));
        private readonly IHandleQuery<TQuery, TDto> _handler;

        public QueryController(IHandleQuery<TQuery, TDto> handler)
        {
            _handler = handler;
        }

        private ISocketContext GetSocket() => HttpContext.RequestServices.GetService<ISocketContext>();

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] TQuery query, CancellationToken ct = default)
        {
            var result = (await _handler.Handle(query, ct));
            var socket = GetSocket();

            return socket != null
                ? this.ComponentActionResult(result, s_featureName, s_featureName)
                : this.ComponentActionResult(result, s_featureName);
        }
    }
}
