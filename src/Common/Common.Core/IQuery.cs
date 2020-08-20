
using System.Collections.Generic;

namespace Common.Core
{
    public interface IQuery<TDto> where TDto: new()
    {
    }

    public interface IListQuery<TDto> where TDto : new()
    {
    }
}
