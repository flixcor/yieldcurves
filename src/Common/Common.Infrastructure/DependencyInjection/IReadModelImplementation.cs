using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure.DependencyInjection
{
    public interface IReadModelImplementation
    {
        IServiceCollection GetServiceCollection();
        IEnumerable<Type> GetUsedTypes();
    }
}
