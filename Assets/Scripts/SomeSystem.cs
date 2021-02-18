using System;
using System.Collections.Generic;
using Flux;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class CustomArgs : EventArgs
{
    
}
public class OtherArgs : EventArgs
{
    
}

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
        BindWhole<SomeData>("SomeData", SetSomeData);
        
        Events.Open(Animal.Boar);
    }

    public override void Update()
    {
        //Debug.Log("SOME");
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            Events.ZipCall(Animal.Boar, 103.3f);
            
            Entities.ForEach((Entity entity, ref Position position, ref Hue hue) =>
            {
                Debug.Log(SomeData);
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
    public override void Initialize()
    {
        base.Initialize();
        
        Events.RelayByVoid(Animal.Boar, Callback);
        Events.Register(Animal.Boar, Callback);
        Events.RelayByCast<CustomArgs>(Animal.Boar, Callback);
        Events.RelayByValue<float>(Animal.Boar, Callback);
    }

    void Callback()
    {
        Debug.Log("VOID");
    }

    void Callback(EventArgs args)
    {
        Debug.Log("STANDARD ARGS");
    }

    void Callback(float number)
    {
        Debug.Log("VALUE ARGS : " + number);
    }

    void Callback(CustomArgs customArgs)
    {
        Debug.Log("CUSTOM ARGS");
    }
    
    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Events.BreakVoidRelay(Animal.Boar, Callback);
            Events.Unregister(Animal.Boar, Callback);
            Events.BreakCastRelay<CustomArgs>(Animal.Boar, Callback);
            Events.BreakValueRelay<float>(Animal.Boar, Callback);
        }
        //Debug.Log("SUB");
    }
}

public class OtherSystem : Flux.System
{
    public override void Update()
    {
        //Debug.Log("OTHER");
    }
}