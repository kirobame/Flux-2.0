using Flux;
using UnityEngine;

public struct Position : IBridge<Transform>
{
    public Vector3 value;

    public void ReceiveDataFrom(Transform component) => value = component.position;
    public void SendDataTo(Transform component) => component.position = value;
}