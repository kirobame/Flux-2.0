using UnityEngine;

namespace Flux.EDS
{
    public abstract class Link
    {
        public abstract void ReceiveData(IData to, Component from);
        public abstract void SendData(IData from, Component to);
    }

    public class Link<T> : Link where T : Component
    {
        public override void ReceiveData(IData to, Component from)
        {
            switch (to)
            {
                case IReader<T> reader :
                    reader.ReceiveDataFrom((T)from);
                    break;
                
                case IBridge<T> bridge :
                    bridge.ReceiveDataFrom((T)from);
                    break;
            }
        }
        public override void SendData(IData from, Component to)
        {
            switch (from)
            {
                case IWriter<T> writer :
                    writer.SendDataTo((T)to);
                    break;
                
                case IBridge<T> bridge :
                    bridge.SendDataTo((T)to);
                    break;
            }
        }
    }
}