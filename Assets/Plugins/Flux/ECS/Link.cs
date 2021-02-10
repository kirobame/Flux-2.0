using System.Collections.Generic;
using UnityEngine;

public abstract class Link
{
    public abstract bool Accepts(IComponent component);
    
    public abstract void SendUnityData(Entity entity);
    public abstract void ReceiveEntityData(Entity entity);
}
public class Link<T> : Link where T : Component
{
    public T Component { get; private set; }

    public IReadOnlyList<int> BridgeKeys => bridgeKeys;
    private List<int> bridgeKeys;
    
    public Link(T component, IEnumerable<int> bridgeKeys)
    {
        Component = component;
        this.bridgeKeys = new List<int>(bridgeKeys);
    }

    public override bool Accepts(IComponent component) => component is IBridge<T>;

    public override void SendUnityData(Entity entity)
    {
        foreach (var bridgeKey in bridgeKeys)
        {
            var bridge = (IBridge<T>)entity[bridgeKey];
            bridge.ReceiveDataFrom(Component);
        }
    }
    public override void ReceiveEntityData(Entity entity)
    {
        foreach (var bridgeKey in bridgeKeys)
        {
            var bridge = (IBridge<T>)entity[bridgeKey];
            bridge.SendDataTo(Component);
        }
    }
}