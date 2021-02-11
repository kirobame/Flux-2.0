using UnityEngine;
using Flux;

[Group(1, 2)]
public class SomeSystem : Flux.System
{
    public override void Update()
    {
        Debug.Log("1");
        if (Input.GetKeyDown(KeyCode.P))
        {
            Access.ForEach((ref Position position, ref Hue hue) =>
            {
                position.value += Vector3.one;
                
                var color = hue.value;
                color.r += 0.1f;
                hue.value = color;
            });
        }
    }
}

public struct Position : IBridge<Transform>
{
    public Vector3 value;

    public void ReceiveDataFrom(Transform source) => value = source.position;
    public void SendDataTo(Transform destination) => destination.position = value;
}

public struct Hue : IBridge<SpriteRenderer>
{
    public Color value;

    public void ReceiveDataFrom(SpriteRenderer source) => value = source.color;
    public void SendDataTo(SpriteRenderer destination) => destination.color = value;
}