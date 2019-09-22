using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure.DependencyInjection
{
    public class ReadModelImplementation : IReadModelImplementation
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly IEnumerable<Type> _usedTypes;

        public ReadModelImplementation(IServiceCollection serviceCollection, IEnumerable<Type> usedTypes)
        {
            _serviceCollection = serviceCollection;
            _usedTypes = usedTypes;
        }

        public IEnumerable<Type> GetUsedTypes() => _usedTypes;

        public IServiceCollection GetServiceCollection() => _serviceCollection;
    }
}
