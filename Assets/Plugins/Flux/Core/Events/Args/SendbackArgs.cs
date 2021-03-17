using System;

namespace Flux.Event
{
    public class SendbackArgs : EventArgs, ISendback
    {
        public event Action<EventArgs> onDone;

        public void End(EventArgs args) => onDone?.Invoke(args);
    }
}