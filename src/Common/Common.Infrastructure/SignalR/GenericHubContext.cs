﻿using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Microsoft.AspNetCore.SignalR;

namespace Common.Infrastructure.SignalR
{
    public class GenericHubContext : ISocketContext
    {
        private readonly IHubContext<GenericHub> _hubContext;

        public GenericHubContext(IHubContext<GenericHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task SendToAll(ReadObject readObject, bool isUpdate = false, CancellationToken cancellationToken = default)
        {
            var message = isUpdate
                ? "update"
                : "insert";

            return _hubContext.Clients.All.SendAsync(message, readObject, cancellationToken);
        }

        public Task SendToUser(IEvent @event, string userId, long preparePosition, long commitPosition, CancellationToken cancellationToken = default)
        {
            var user = _hubContext.Clients.User(userId);

            return user != null
                ? user.SendAsync(@event.GetType().Name, @event, preparePosition, commitPosition, cancellationToken)
                : Task.CompletedTask;
        }

        public Task SendToGroup(string group, ReadObject readObject, bool isUpdate = false, CancellationToken cancellationToken = default)
        {
            var message = isUpdate
                ? "update"
                : "insert";

            var hubGroup = _hubContext.Clients.Group(group);

            return hubGroup.SendAsync(message, readObject, cancellationToken);
        }
    }
}
