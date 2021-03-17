namespace Flux
{
    public interface IYieldInstruction
    {
        object Wait();
        float Increment();
    }
}