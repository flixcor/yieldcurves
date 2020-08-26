namespace Lib.EventSourcing
{
    public interface IQuery<T>
    {
        T Handle(T input);
    }
}
