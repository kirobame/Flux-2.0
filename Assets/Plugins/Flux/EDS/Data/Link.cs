using UnityEngine;

namespace Flux
{
    public abstract class Link
    {
        public abstract void ReceiveData(IData to, Component from);
        public abstract void SendData(IData from, Component to);
    }
    public class Link<T> : Link where T : Component
    {
        public override void ReceiveData(IData to, Component from) => ((IBridge<T>)to).ReceiveDataFrom((T)from);
        public override void SendData(IData from, Component to) => ((IBridge<T>)from).SendDataTo((T)to);
    }
}