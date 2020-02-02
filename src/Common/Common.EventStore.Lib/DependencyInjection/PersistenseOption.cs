using Microsoft.Extensions.DependencyInjection;

namespace Common.EventStore.Lib.DependencyInjection
{
    internal class PersistenseOption : IPersistenceOption
    {
        public PersistenseOption(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
