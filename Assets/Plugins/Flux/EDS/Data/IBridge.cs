using UnityEngine;

namespace Flux
{
    public interface IBridge : IData { }
    public interface IBridge<in T> : IBridge where T : Component
    {
        void ReceiveDataFrom(T component);
        void SendDataTo(T component);
    }
}