using Flux;
using UnityEngine;

public struct Hue : IBridge<SpriteRenderer>
{
    public Color value;

    public void ReceiveDataFrom(SpriteRenderer component) => value = component.color;
    public void SendDataTo(SpriteRenderer component) => component.color = value;
}