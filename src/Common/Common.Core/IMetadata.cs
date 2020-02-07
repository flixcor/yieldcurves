using System.Collections.Generic;

namespace Common.Core
{
    public interface IMetadata : Google.Protobuf.IMessage
    {
        IReadOnlyDictionary<string, string> Values { get; }
    }
}
