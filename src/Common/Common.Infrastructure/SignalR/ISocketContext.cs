using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;

namespace Common.Infrastructure.SignalR
{
    public interface ISocketContext
    {
        Task SendToAll(ReadObject readObject, bool isUpdate = false, CancellationToken cancellationToken = default);
        Task SendToGroup(string group, ReadObject readObject, bool isUpdate = false, CancellationToken cancellationToken = default);
    }

    public interface ISocketContext<TQuery, TDto> : ISocketContext where TQuery : IQuery<TDto>
    {

    }
}
