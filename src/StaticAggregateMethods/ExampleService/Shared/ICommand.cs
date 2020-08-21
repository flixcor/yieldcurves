using System;
using System.Threading.Tasks;

namespace ExampleService.Shared
{
    public interface ICommand
    {
        Task Handle();
    }
}
