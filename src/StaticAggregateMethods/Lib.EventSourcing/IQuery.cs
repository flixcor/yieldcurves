namespace Lib.EventSourcing
{
    public interface IQuery<T>
    {
        (long, T) Handle();
    }
}
