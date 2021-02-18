namespace Flux
{
    public interface IWrapper { }
    public interface IWrapper<out T> : IWrapper
    {
        T Value { get; }
    }
}