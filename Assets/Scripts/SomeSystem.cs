using System;
using System.Collections.Generic;
using Flux;
using UnityEngine;
using UnityEngine.AddressableAssets;

[UpdateOrder("Root", "OtherSystem/Any")]
public class SomeSystem : BindedSystem
{
    protected void SetColor(Color value) => Color = value;
    protected Color Color { get; set; }

    protected void SetStep(Vector3 value) => Step = value;
    protected Vector3 Step { get; set; }

    protected void SetSomeData(SomeData value) => SomeData = value;
    protected SomeData SomeData { get; set; }
    
    public override void Initialize()
    {
        base.Initialize();
        AddPackage<SomeData>("SomeData", SetSomeData);
    }

    public override void Update()
    {
        Debug.Log("SOME");
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            Entities.ForEach((Entity entity, ref Position position, ref Hue hue) =>
            {
                position.value += SomeData.Step;
                Entities.MarkDirty<Transform>(entity, position);
                
                hue.value += SomeData.ColorStep;
                Entities.MarkDirty<SpriteRenderer>(entity, hue);
            }, Country.France | Country.Britain);
        }
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

public class OtherSystem : Flux.System
{
    public override void Update()
    {
        Debug.Log("OTHER");
    }
}