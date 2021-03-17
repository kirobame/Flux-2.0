namespace Flux
{
    public interface IInjectable<in T>
    {
        void Inject(T value);
    }
}