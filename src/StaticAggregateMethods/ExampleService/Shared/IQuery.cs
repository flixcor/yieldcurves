using System.Threading.Tasks;

namespace ExampleService.Shared
{
    public interface IQuery<T>
    {
        Task<T> Handle();
    }
}
