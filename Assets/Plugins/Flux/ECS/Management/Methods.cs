namespace Flux
{
    public delegate void WriteTo<T>(ref T component) where T : IComponent;
    public delegate void WW<T1, T2>(ref T1 compOne, ref T2 compTwo) where T1 : IComponent where T2 : IComponent;
}