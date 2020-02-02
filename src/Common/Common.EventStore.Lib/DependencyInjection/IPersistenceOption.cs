using Microsoft.Extensions.DependencyInjection;

namespace Common.EventStore.Lib.DependencyInjection
{
    public interface IPersistenceOption
    {
        public IServiceCollection Services { get; }
    }
}
