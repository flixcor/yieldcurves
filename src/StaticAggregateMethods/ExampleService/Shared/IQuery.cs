namespace ExampleService.Shared
{
    public interface IQuery<T>
    {
        T Handle(T input);
    }
}
