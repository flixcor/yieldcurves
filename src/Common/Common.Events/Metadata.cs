using System.Collections.Generic;
using Common.Core;

namespace Common.Events
{
    internal partial class Metadata : IMetadata, IMessage
    {
        internal Metadata(IDictionary<string, string> values) => Values.Add(values);

        IReadOnlyDictionary<string, string> IMetadata.Values => Values;
    }
}
