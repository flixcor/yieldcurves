using System;
using System.Threading.Tasks;

namespace Common.Core
{
    public interface IMessageBusListener
    {
        Task SubscribeToAll();
    }
}