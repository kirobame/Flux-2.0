using UnityEngine;

namespace Flux
{
    public abstract class Link
    {
        public abstract void ReceiveData(ISection to, Component from);
        public abstract void SendData(ISection from, Component to);
    }
    public class Link<T> : Link where T : Component
    {
        public override void ReceiveData(ISection to, Component from) => ((IBridge<T>)to).ReceiveDataFrom((T)from);
        public override void SendData(ISection from, Component to) => ((IBridge<T>)from).SendDataTo((T)to);
    }
}