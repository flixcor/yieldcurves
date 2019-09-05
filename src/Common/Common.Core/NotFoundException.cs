using System;
using System.Runtime.Serialization;

namespace Common.Core
{
    [Serializable]
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }

        // Without this constructor, deserialization will fail
        protected NotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public NotFoundException()
        {
        }

        public NotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
