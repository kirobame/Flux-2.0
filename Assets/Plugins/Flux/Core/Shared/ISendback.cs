using System;

namespace Flux
{
    public interface ISendback
    {
        event Action<EventArgs> onDone;

        void End(EventArgs args);
    }
}