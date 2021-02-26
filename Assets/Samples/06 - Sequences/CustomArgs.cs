using System;
using Flux;

namespace Example06
{
    // IWrapper exposes a value for any use
    // ISendback allows to specify that control can be given to the sender of the EventArgs
    public class CustomArgs : EventArgs, IWrapper<float>, ISendback 
    {
        public event Action<EventArgs> onDone;

        public CustomArgs(float value) => Value = value;
        
        public float Value { get; private set; }

        public void End(EventArgs args) => onDone?.Invoke(args);
    }
}