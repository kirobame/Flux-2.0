using Flux;
using UnityEngine;

[UpdateOrder("Root", "Any/OtherSystem")]
public class SomeSystem : Flux.System
{
    public override void Update()
    {
        Debug.Log("SOME");
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            Entities.ForEach((Entity entity, ref Position position, ref Hue hue) =>
            {
                position.value += Vector3.one;
                Entities.MarkDirty<Transform>(entity, position);
                
                hue.value.r += 0.1f;
                Entities.MarkDirty<SpriteRenderer>(entity, hue);
            }, new SomeFlag(2));
        }
    }
}

public class OtherSystem : Flux.System
{
    public override void Update()
    {
        Debug.Log("OTHER");
    }
}

[UpdateOrder("OtherSystem", "Any/Any")]
public class SubSystem : Flux.System
{
    public override void Update()
    {
        Debug.Log("SUB");
    }
}