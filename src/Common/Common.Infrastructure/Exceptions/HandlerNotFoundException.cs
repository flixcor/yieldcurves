using System;
using System.Runtime.Serialization;

namespace Common.Infrastructure.Exceptions
{
    public class HandlerNotFoundException : Exception
    {
        public HandlerNotFoundException(Type handlerType) : base(handlerType.AssemblyQualifiedName)
        {
        }

        // Without this constructor, deserialization will fail
        protected HandlerNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
