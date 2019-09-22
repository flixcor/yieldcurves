using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Infrastructure
{
    public static class EntityWrapper
    {
        public static EntityWrapper<T> Wrap<T>(this IEnumerable<T> entities) => new EntityWrapper<T>(entities);
    }

    public class EntityWrapper<T> 
    {
        internal EntityWrapper(IEnumerable<T> entities)
        {
            Entities = entities ?? throw new ArgumentNullException(nameof(entities));
        }

        public IEnumerable<T> Entities { get; }
    }
}
