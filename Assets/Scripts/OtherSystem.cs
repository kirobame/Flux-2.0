using Flux;
using UnityEngine;

[Group(1, 1)]
public class OtherSystem : Flux.System
{
    public override void Update()
    {
        Debug.Log("2");
    }
}