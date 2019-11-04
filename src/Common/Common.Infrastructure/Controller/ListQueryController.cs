using System;
using System.Collections.Generic;
using System.Threading;
using Common.Core;
using Common.Core.Extensions;
using Common.Infrastructure.Extensions;
using Common.Infrastructure.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure.Controller
{
    public class ListQueryController<TQuery, TDto> : ControllerBase where TQuery : IListQuery<TDto> where TDto : class
    {
        private static readonly string s_featureName = HelperMethods.GetFeatureName(typeof(TQuery));
        private readonly IHandleListQuery<TQuery, TDto> _handler;

        public ListQueryController(IHandleListQuery<TQuery, TDto> handler)
        {
            _handler = handler;
        }

        private ISocketContext GetSocket() => HttpContext.RequestServices.GetService<ISocketContext>();

        [HttpGet]
        public IAsyncEnumerable<RealtimeFrontendComponent<TDto>> Get([FromQuery] TQuery query, CancellationToken ct = default)
        {
            var queryResult = _handler.Handle(query, ct);
            var socket = GetSocket();

            var result = socket != null
                ? this.FrontEndComponentAsyncEnumerable(queryResult, s_featureName, s_featureName)
                : this.FrontEndComponentAsyncEnumerable(queryResult, s_featureName);

            return result;
        }
    }
}
