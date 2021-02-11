using System.Collections.Generic;
using UnityEngine;

namespace Flux
{
    public abstract class Link
    {
        public abstract void SendUnityData(IComponent component);
        public abstract void ReceiveEntityData(IComponent component);
    }

    public class Link<T> : Link where T : Component
    {
        public Link(T component) => Component = component;
    
        public T Component { get; private set; }
    
        public override void SendUnityData(IComponent component)
        {
            if (!(component is IBridge<T> bridge)) return;
            bridge.ReceiveDataFrom(Component);
        }
        public override void ReceiveEntityData(IComponent component)
        {
            if (!(component is IBridge<T> bridge)) return;
            bridge.SendDataTo(Component);
        }
    }
}