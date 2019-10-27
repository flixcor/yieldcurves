using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Common.Infrastructure.SignalR
{
    public class GenericHub : Hub
    {
        public async override Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            var httpContext = Context.GetHttpContext();
            var featureName = httpContext.Request.Query["feature"];

            if (!string.IsNullOrWhiteSpace(featureName))
            {
                await Groups.AddToGroupAsync(connectionId, featureName);
            }

            await base.OnConnectedAsync();
        }
    }
}
