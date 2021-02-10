using UnityEngine;

public interface IBridge : IComponent { }
public interface IBridge<in T> : IBridge where T : Component
{
    void ReceiveDataFrom(T source);
    void SendDataTo(T destination);
}