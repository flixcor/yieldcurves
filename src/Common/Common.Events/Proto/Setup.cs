using System;
using System.Linq;
using Common.Core;
using Common.Core.Extensions;
using ProtoBuf.Meta;

namespace Common.Events.Proto
{
    internal class Setup
    {
        public void SetUp()
        {
            var eventTypes = typeof(Event).GetDescendantTypes(typeof(Setup).Assembly).OrderBy(x => x.FullName);
            var eventBase = RuntimeTypeModel.Default.Add(typeof(Event), false).Add("Id", "Version");

            var i = 0;
            foreach (var type in eventTypes)
            {
                eventBase.AddSubType(i++, type);
                AddTypeToModel(RuntimeTypeModel.Default, type);
            }
        }

        private void AddTypeToModel(RuntimeTypeModel typeModel, Type type)
        {
            var properties = type.GetProperties().Select(p => p.Name).OrderBy(name => name);//OrderBy added, thanks MG
            typeModel.Add(type, true).Add(properties.ToArray());
        }
    }
}
