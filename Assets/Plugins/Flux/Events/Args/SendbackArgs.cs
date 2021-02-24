using System;

namespace Flux
{
    public class SendbackArgs : EventArgs, ISendback
    {
        public event Action<EventArgs> onDone;

        public void End(EventArgs args) => onDone?.Invoke(args);
    }
}